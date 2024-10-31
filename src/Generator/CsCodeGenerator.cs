// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

public partial class CsCodeGenerator
{
    private static readonly HashSet<string> s_keywords =
    [
        "object",
        "event",
        "base",
        "lock",
        "string",
        "override",
        "internal",
        "private",
        "public",
    ];

    private static readonly Dictionary<string, string> s_csNameMappings = new()
    {
        //{ "bool", "SDLBool" },
        { "Sint8", "sbyte" },
        { "uint8_t", "byte" },
        { "Uint8", "byte" },
        { "Uint16", "ushort" },
        { "uint16_t", "ushort" },
        { "uint32_t", "uint" },
        { "Uint32", "uint" },
        { "uint64_t", "ulong" },
        { "int8_t", "sbyte" },
        { "Sint32", "int" },
        { "int32_t", "int" },
        { "int16_t", "short" },
        { "Sint16", "short" },
        { "int64_t", "long" },
        { "int64_t*", "long*" },
        { "Sint64", "long" },
        { "Uint64", "ulong" },
        { "char", "byte" },
        { "wchar_t", "char" },
        { "size_t", "nuint" },
        { "intptr_t", "nint" },
        { "uintptr_t", "nuint" },

        { "SDL_FunctionPointer", "delegate* unmanaged<void>" },
        { "SDL_GUID", "Guid" },
        { "SDL_Point", "Point" },
        { "SDL_FPoint", "PointF" },
        { "SDL_Rect", "Rectangle" },
        { "SDL_FRect", "RectangleF" },
        { "SDL_EGLDisplay", "nint" },
        { "SDL_EGLConfig", "nint" },
        { "SDL_EGLSurface", "nint" },
        { "SDL_EGLAttrib", "nint" },
        { "SDL_EGLint", "int" },
        { "SDL_MetalView", "nint" },
        { "HWND", "nint" },
        { "HDC", "nint" },
        { "HINSTANCE", "nint" },
        { "UINT", "uint" },
        { "WPARAM", "nuint" },
        { "LPARAM", "nint" },
        { "SDL_eventaction", "SDL_EventAction" },
        { "SDL_iconv_t", "SDL_iconv_data_t" },
        // Vulkan
        { "VkAllocationCallbacks", "nint" },
        { "VkInstance", "nint" },
        { "VkInstance_T", "nint" },
        { "VkPhysicalDevice", "nint" },
        { "VkPhysicalDevice_T", "nint" },
        { "VkSurfaceKHR", "ulong" },
        { "VkSurfaceKHR_T", "ulong" },
        // Until we understand how to treat this
        { "SDL_BlitMap", "nint" },
        { "SDL_Time", "long" },

    };

    private static readonly HashSet<string> s_knownTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "SDL_Rect",
    };

    private readonly CsCodeGeneratorOptions _options = new();

    private readonly List<CppEnum> _collectedEnums = [];
    private readonly Dictionary<string, CppFunctionType> _collectedCallbackTypedes = [];
    private readonly Dictionary<string, Tuple<string, CppTypeDeclaration>> _collectedHandles = [];
    private readonly List<CppClass> _collectedStructAndUnions = [];
    private readonly List<CppFunction> _collectedFunctions = [];

    public CsCodeGenerator(CsCodeGeneratorOptions options)
    {
        _options = options;
    }

    public void Collect(CppCompilation compilation)
    {
        CollectConstants(compilation);
        CollectEnums(compilation);
        CollectHandles(compilation);
        CollectStructAndUnions(compilation);
        CollectCommands(compilation);
    }

    public void Generate()
    {
        GenerateEnums();
        GenerateConstants();
        GenerateHandles();
        GenerateStructAndUnions();
        GenerateCommands();
    }

    public static void AddCsMapping(string typeName, string csTypeName)
    {
        s_csNameMappings[typeName] = csTypeName;
    }

    private static string NormalizeFieldName(string name)
    {
        if (s_keywords.Contains(name))
            return "@" + name;

        return name;
    }

    private static string GetCsCleanName(string name)
    {
        if (s_csNameMappings.TryGetValue(name, out string? mappedName))
        {
            return GetCsCleanName(mappedName);
        }
        else if (name.StartsWith("PFN"))
        {
            return "IntPtr";
        }

        return name;
    }

    private string GetCsTypeName(CppType? type)
    {
        if (type is CppPrimitiveType primitiveType)
        {
            return GetCsTypeName(primitiveType);
        }

        if (type is CppQualifiedType qualifiedType)
        {
            return GetCsTypeName(qualifiedType.ElementType);
        }

        if (type is CppEnum enumType)
        {
            string enumCsName = GetCsCleanName(enumType.Name);
            return enumCsName;
        }

        if (type is CppTypedef typedef)
        {
            if (typedef.ElementType is CppClass classElementType)
            {
                return GetCsTypeName(classElementType);
            }
            else if (typedef.ElementType is CppPointerType)
            {
                return GetCsTypeName(typedef.ElementType);
            }

            string typeDefCsName = GetCsCleanName(typedef.Name);
            return typeDefCsName;
        }

        if (type is CppClass @class)
        {
            string className = GetCsCleanName(@class.Name);
            if (className == "SDL_RWops"
                || className == "IDirect3DDevice9"
                || className == "ID3D11Device"
                || className == "ID3D12Device")
                return "nint";

            return className;
        }

        if (type is CppPointerType pointerType)
        {
            string csPointerTypeName = GetCsTypeName(pointerType);
            if (csPointerTypeName == "void")
                return "nint";

            if (csPointerTypeName == "IntPtr" || csPointerTypeName == "nint")
                return csPointerTypeName;

            if (!s_knownTypes.Contains(csPointerTypeName) && !_generatedPointerHandles.Contains(csPointerTypeName))
                return csPointerTypeName + "*";

            return csPointerTypeName;
        }

        if (type is CppArrayType arrayType)
        {
            return GetCsTypeName(arrayType.ElementType) + "*";
        }

        if (type is CppFunctionType functionType)
        {
            StringBuilder builder = new();
            foreach (CppParameter parameter in functionType.Parameters)
            {
                string paramCsType = GetCsTypeName(parameter.Type);
                builder.Append(paramCsType).Append(", ");
            }

            string returnCsName = GetCsTypeName(functionType.ReturnType);
            builder.Append(returnCsName);

            string callingConventionCall = string.Empty;
            if (!string.IsNullOrEmpty(_options.CallingConvention))
            {
                callingConventionCall = $"[{_options.CallingConvention}]";
            }

            return $"delegate* unmanaged{callingConventionCall}<{builder}>";
        }

        return string.Empty;
    }

    private string GetCsTypeName(CppPrimitiveType primitiveType)
    {
        switch (primitiveType.Kind)
        {
            case CppPrimitiveKind.Void:
                return "void";

            case CppPrimitiveKind.Bool:
                return _options.BooleanType;

            case CppPrimitiveKind.Char:
                return "byte";

            case CppPrimitiveKind.WChar:
                return "char";

            case CppPrimitiveKind.Short:
                return "short";
            case CppPrimitiveKind.Int:
                return "int";

            case CppPrimitiveKind.LongLong:
                return "long";

            case CppPrimitiveKind.UnsignedChar:
                return "byte";

            case CppPrimitiveKind.UnsignedShort:
                return "ushort";
            case CppPrimitiveKind.UnsignedInt:
                return "uint";

            case CppPrimitiveKind.UnsignedLongLong:
                return "ulong";

            case CppPrimitiveKind.Float:
                return "float";
            case CppPrimitiveKind.Double:
                return "double";
            case CppPrimitiveKind.LongDouble:
                return "double";

            case CppPrimitiveKind.Long:
                return _options.MapCLongToIntPtr ? "nint" : "global::System.Runtime.InteropServices.CLong";

            case CppPrimitiveKind.UnsignedLong:
                return _options.MapCLongToIntPtr ? "nuint" : "global::System.Runtime.InteropServices.CULong";

            default:
                return string.Empty;
        }
    }

    private string GetCsTypeName(CppPointerType pointerType)
    {
        if (pointerType.ElementType is CppQualifiedType qualifiedType)
        {
            if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
            {
                if (primitiveType.Kind == CppPrimitiveKind.Void && qualifiedType.Qualifier == CppTypeQualifier.Const)
                {
                    // const void*
                    return "void";
                }

                return GetCsTypeName(primitiveType);
            }
            else if (qualifiedType.ElementType is CppClass @classType)
            {
                return GetCsTypeName(@classType);
            }
            else if (qualifiedType.ElementType is CppPointerType subPointerType)
            {
                return GetCsTypeName(subPointerType) + "*";
            }
            else if (qualifiedType.ElementType is CppTypedef typedef)
            {
                return GetCsTypeName(typedef);
            }
            else if (qualifiedType.ElementType is CppEnum @enum)
            {
                return GetCsTypeName(@enum);
            }

            return GetCsTypeName(qualifiedType.ElementType);
        }

        return GetCsTypeName(pointerType.ElementType);
    }
}
