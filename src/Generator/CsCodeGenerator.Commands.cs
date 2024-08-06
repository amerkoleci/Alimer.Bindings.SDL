// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
{
    private static void CollectCommands(CppCompilation compilation)
    {
        foreach (CppTypedef typedef in compilation.Typedefs)
        {
            if (!typedef.Name.EndsWith("Callback"))
            {
                continue;
            }

            if (typedef.Name == "SDL_EGLAttribArrayCallback"
                || typedef.Name == "SDL_EGLIntArrayCallback")
            {
                continue;
            }

            if (typedef.ElementType is not CppPointerType pointerType)
            {
                continue;
            }

            CppFunctionType functionType = (CppFunctionType)pointerType.ElementType;
            s_collectedCallbackTypedes.Add(typedef.Name, functionType);
        }

        foreach (CppFunction? cppFunction in compilation.Functions)
        {
            if (cppFunction.Name == "SDL_LogMessageV"
                || cppFunction.Name == "SDL_GetLogOutputFunction"
                || cppFunction.Name == "SDL_SetLogOutputFunction"
                || cppFunction.Name == "SDL_EGL_SetEGLAttributeCallbacks"
                || cppFunction.Name == "SDL_Vulkan_GetInstanceExtensions"
                || cppFunction.Name == "SDL_Vulkan_CreateSurface"
                || cppFunction.Name == "SDL_GUIDToString"
                || cppFunction.Name == "SDL_GUIDFromString"
                || cppFunction.Name == "SDL_SetPropertyWithCleanup"
                || cppFunction.Name == "SDL_IOprintf"
                || cppFunction.Name == "SDL_IOvprintf"
                )
            {
                continue;
            }

            // Avoid generating function with va_list arguments
            bool ignoreFunction = false;
            foreach (CppParameter? parameter in cppFunction.Parameters)
            {
                string paramCsType = GetCsTypeName(parameter.Type);
                if (paramCsType == "va_list")
                {
                    ignoreFunction = true;
                    break;
                }
            }

            if (ignoreFunction)
                continue;

            s_collectedFunctions.Add(cppFunction);
        }
    }

    private static void GenerateCommands()
    {
        string visibility = s_options.PublicVisiblity ? "public" : "internal";

        // Generate Functions
        using CodeWriter writer = new(Path.Combine(s_options.OutputPath, "Commands.cs"),
            true,
            s_options.Namespace,
            ["System", "System.Runtime.InteropServices", "System.Drawing"]
            );

        // Generate callback
        if (s_options.GenerateCallbackTypes)
        {
            foreach (KeyValuePair<string, CppFunctionType> pair in s_collectedCallbackTypedes)
            {
                CppFunctionType functionType = pair.Value;

                string returnCsName = GetCsTypeName(functionType.ReturnType);
                string argumentsString = GetParameterSignature(functionType);

                writer.WriteLine($"[UnmanagedFunctionPointer(CallingConvention.Cdecl)]");
                writer.WriteLine($"{visibility} unsafe delegate {returnCsName} {pair.Key}({argumentsString});");
                writer.WriteLine();
            }
        }

        using (writer.PushBlock($"{visibility} unsafe partial class {s_options.ClassName}"))
        {
            foreach (CppFunction cppFunction in s_collectedFunctions)
            {
                WriteFunctionInvocation(writer, cppFunction);
            }
        }
    }

    private static void WriteFunctionInvocation(CodeWriter writer, CppFunction cppFunction)
    {
        if (cppFunction.Name == "SDL_SendGamepadEffect")
        {

        }

        string returnCsName = GetCsTypeName(cppFunction.ReturnType);
        string argumentsString = GetParameterSignature(cppFunction);
        string functionName = cppFunction.Name;

        writer.WriteLine($"[LibraryImport(LibName, EntryPoint = \"{cppFunction.Name}\")]");
        if (returnCsName == "SDL_bool")
        {
            writer.WriteLine("[return: MarshalAs(UnmanagedType.Bool)]");
            returnCsName = "bool";
        }

        writer.WriteLine($"public static partial {returnCsName} {functionName}({argumentsString});");
        writer.WriteLine();
    }

    public static string GetParameterSignature(CppFunction cppFunction, bool unsafeStrings = true)
    {
        return GetParameterSignature(cppFunction.Name, cppFunction.Parameters, unsafeStrings);
    }

    public static string GetParameterSignature(CppFunctionType cppFunctionType, bool unsafeStrings = true)
    {
        return GetParameterSignature(cppFunctionType.FullName, cppFunctionType.Parameters, unsafeStrings);
    }

    private static string GetParameterSignature(string functionName, IEnumerable<CppParameter> parameters, bool unsafeStrings = true)
    {
        var argumentBuilder = new StringBuilder();
        int index = 0;

        foreach (CppParameter cppParameter in parameters)
        {
            string direction = string.Empty;
            string paramCsTypeName;
            string paramCsName = GetParameterName(cppParameter.Name);

            // Callback parameters
            if (cppParameter.Type is CppTypedef typedef
                && typedef.ElementType is CppPointerType pointerType
                && pointerType.ElementType is CppFunctionType functionType)
            {
                paramCsTypeName = GetCallbackMemberSignature(functionType);
            }
            else if (cppParameter.Type is CppPointerType cppParameterPointerType
                && cppParameterPointerType.ElementType is CppTypedef cppParameterPointerElementType
                && cppParameterPointerElementType.ElementType is CppPointerType cppParameterPointerElementTypeDef
                && cppParameterPointerElementTypeDef.ElementType is CppFunctionType cppParameterPointerFunctionType)
            {
                paramCsTypeName = GetCallbackMemberSignature(cppParameterPointerFunctionType);
            }
            else
            {
                paramCsTypeName = GetCsTypeName(cppParameter.Type);

                if (paramCsTypeName == "byte*" && unsafeStrings == false)
                {
                    paramCsName = "ReadOnlySpan<sbyte>";
                }
            }

            if (functionName.Contains("Get", StringComparison.OrdinalIgnoreCase))
            {
                if (paramCsName == "count"
                    || paramCsName == "x"
                    || paramCsName == "y"
                    || paramCsName == "w"
                    || paramCsName == "h")
                {
                    if (paramCsTypeName == "int*")
                        paramCsTypeName = "out int";
                    else if (paramCsTypeName == "uint*")
                        paramCsTypeName = "out uint";
                }
            }

            if (functionName == "SDL_PeepEvents"
                || functionName == "SDL_HasEvents"
                || functionName == "SDL_FlushEvents")
            {
                if (cppParameter.Name == "minType" || cppParameter.Name == "maxType")
                {
                    paramCsTypeName = "SDL_EventType";
                }
            }

            if (functionName == "SDL_HasEvent"
                || functionName == "SDL_FlushEvent"
                || functionName == "SDL_SetEventEnabled"
                || functionName == "SDL_EventEnabled")
            {
                if (cppParameter.Name == "type")
                {
                    paramCsTypeName = "SDL_EventType";
                }
            }

            argumentBuilder.Append(paramCsTypeName).Append(' ').Append(paramCsName);

            if (index < parameters.Count() - 1)
            {
                argumentBuilder.Append(", ");
            }

            index++;
        }

        return argumentBuilder.ToString();
    }

    private static string GetCallbackMemberSignature(CppFunctionType functionType)
    {
        StringBuilder builder = new();
        foreach (CppParameter parameter in functionType.Parameters)
        {
            string paramCsType = GetCsTypeName(parameter.Type);
            // Otherwise we get interop issues with non blittable types
            if (paramCsType == "WGPUBool")
                paramCsType = "uint";
            builder.Append(paramCsType).Append(", ");
        }

        string returnCsName = GetCsTypeName(functionType.ReturnType);
        // Otherwise we get interop issues with non blittable types
        if (returnCsName == "WGPUBool")
            returnCsName = "uint";

        builder.Append(returnCsName);

        return $"delegate* unmanaged<{builder}>";
    }

    private static string GetParameterName(string name)
    {
        name = NormalizeFieldName(name);

        if (name.Length <= 1)
            return name;

        if (name.StartsWith('p')
            && char.IsUpper(name[1]))
        {
            name = char.ToLower(name[1]) + name.Substring(2);
            return GetParameterName(name);
        }

        return name;
    }

    private static bool CanBeUsedAsOutput(CppType type, out CppTypeDeclaration? elementTypeDeclaration)
    {
        if (type is CppPointerType pointerType)
        {
            if (pointerType.ElementType is CppTypedef typedef)
            {
                elementTypeDeclaration = typedef;
                return true;
            }
            else if (pointerType.ElementType is CppClass @class
                && @class.ClassKind != CppClassKind.Class
                && @class.SizeOf > 0)
            {
                elementTypeDeclaration = @class;
                return true;
            }
            else if (pointerType.ElementType is CppEnum @enum
                && @enum.SizeOf > 0)
            {
                elementTypeDeclaration = @enum;
                return true;
            }
        }

        elementTypeDeclaration = null;
        return false;
    }
}
