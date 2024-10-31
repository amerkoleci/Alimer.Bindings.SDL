// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

partial class CsCodeGenerator
{
    private void CollectCommands(CppCompilation compilation)
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
            _collectedCallbackTypedes.Add(typedef.Name, functionType);
        }

        foreach (CppFunction? cppFunction in compilation.Functions)
        {
            if (cppFunction.Name == "SDL_LogMessageV"
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

            _collectedFunctions.Add(cppFunction);
        }
    }

    private void GenerateCommands()
    {
        string visibility = _options.PublicVisiblity ? "public" : "internal";

        // Generate Functions
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "Commands.cs"),
            true,
            _options.Namespace,
            ["System", "System.Runtime.InteropServices", "System.Drawing"]
            );

        // Generate callback
        if (_options.GenerateCallbackTypes)
        {
            foreach (KeyValuePair<string, CppFunctionType> pair in _collectedCallbackTypedes)
            {
                CppFunctionType functionType = pair.Value;

                string returnCsName = GetCsTypeName(functionType.ReturnType);
                string argumentsString = GetParameterSignature(functionType, false);

                writer.WriteLine($"[UnmanagedFunctionPointer(CallingConvention.Cdecl)]");
                writer.WriteLine($"{visibility} unsafe delegate {returnCsName} {pair.Key}({argumentsString});");
                writer.WriteLine();
            }
        }

        using (writer.PushBlock($"{visibility} unsafe partial class {_options.ClassName}"))
        {
            foreach (CppFunction cppFunction in _collectedFunctions)
            {
                WriteFunctionInvocation(cppFunction, writer);
            }
        }
    }

    private static bool IsString(CppType cppType, out bool isConst)
    {
        if (cppType is CppPointerType cppPointerType)
        {
            if (cppPointerType.ElementType is CppQualifiedType qualifiedType
                && qualifiedType.ElementType is CppPrimitiveType qualifiedTypePrimitiveType)
            {
                if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
                {
                    if (primitiveType.Kind == CppPrimitiveKind.Char
                        && qualifiedType.Qualifier == CppTypeQualifier.Const)
                    {
                        isConst = true;
                        return true;
                    }
                }
            }
            else if (cppPointerType.ElementType is CppPrimitiveType primitiveType
                && primitiveType.Kind == CppPrimitiveKind.Char)
            {
                isConst = false;
                return true;
            }
        }

        isConst = false;
        return false;
    }


    private void WriteFunctionInvocation(CppFunction cppFunction, CodeWriter writer)
    {
        string returnCsName = GetCsTypeName(cppFunction.ReturnType);
        string argumentsString = GetParameterSignature(cppFunction, false);
        string functionName = cppFunction.Name;

        bool stringReturnType = false;
        bool freeReturnString = false;

        writer.WriteComment(cppFunction.Comment?.ChildrenToString() ?? string.Empty);

        writer.WriteLine($"[LibraryImport(LibName, EntryPoint = \"{cppFunction.Name}\")]");
        if (returnCsName == "SDL_bool" || returnCsName == "bool")
        {
            if (string.IsNullOrEmpty(_options.BooleanMarshalType))
            {
                returnCsName = _options.BooleanType;
            }
            else
            {
                writer.WriteLine($"[return: MarshalAs(UnmanagedType.{_options.BooleanMarshalType})]");
                returnCsName = "bool";
            }
        }
        else if (IsString(cppFunction.ReturnType, out bool isConstString))
        {
            stringReturnType = true;
            freeReturnString = !isConstString;
        }

        bool hasStringParameter = false;
        foreach (CppParameter parameter in cppFunction.Parameters)
        {
            if (IsString(parameter.Type, out bool isConst))
            {
                hasStringParameter = true;
                break;
            }
        }

        string nativeFunctionName = functionName;
        if (stringReturnType)
        {
            nativeFunctionName += "Ptr";
        }

        writer.WriteLine($"public static partial {returnCsName} {nativeFunctionName}({argumentsString});");
        writer.WriteLine();

        if (stringReturnType)
        {
            string invokeArgumentsString = GetParameterSignature(cppFunction, true);

            using (writer.PushBlock($"public static string? {functionName}({argumentsString})"))
            {
                if (freeReturnString)
                {
                    writer.WriteLine($"byte* resultPtr = {nativeFunctionName}({invokeArgumentsString});");
                    writer.WriteLine($"string? result = ConvertToManaged(resultPtr);");
                    writer.WriteLine($"SDL_free(resultPtr);");
                    writer.WriteLine($"return result;");
                }
                else
                {
                    writer.WriteLine($"return ConvertToManaged({nativeFunctionName}({invokeArgumentsString}));");
                }
            }
            writer.WriteLine();
        }

        if (hasStringParameter)
        {
            // ReadOnlySpan<byte>
            argumentsString = GetParameterSignature(cppFunction, false, MarshalStringType.Byte);
            writer.WriteLine($"[LibraryImport(LibName, EntryPoint = \"{cppFunction.Name}\")]");
            if (returnCsName == "bool")
                writer.WriteLine($"[return: MarshalAs(UnmanagedType.{_options.BooleanMarshalType})]");
            writer.WriteLine($"public static partial {returnCsName} {nativeFunctionName}({argumentsString});");
            writer.WriteLine();

            // ReadOnlySpan<char>
            argumentsString = GetParameterSignature(cppFunction, false, MarshalStringType.Char);
            writer.WriteLine($"[LibraryImport(LibName, EntryPoint = \"{cppFunction.Name}\")]");
            if (returnCsName == "bool")
                writer.WriteLine($"[return: MarshalAs(UnmanagedType.{_options.BooleanMarshalType})]");
            writer.WriteLine($"public static partial {returnCsName} {nativeFunctionName}({argumentsString});");
            writer.WriteLine();
        }
    }

    private string GetParameterSignature(CppFunction cppFunction, bool invocation, MarshalStringType marshalString = MarshalStringType.None)
    {
        return GetParameterSignature(cppFunction.Name, cppFunction.Parameters, invocation, marshalString);
    }

    private string GetParameterSignature(CppFunctionType cppFunctionType, bool invocation, MarshalStringType marshalString = MarshalStringType.None)
    {
        return GetParameterSignature(cppFunctionType.FullName, cppFunctionType.Parameters, invocation, marshalString);
    }

    private string GetParameterSignature(
        string functionName,
        IEnumerable<CppParameter> parameters,
        bool invocation,
        MarshalStringType marshalString)
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

            if (paramCsTypeName == "SDL_bool" || paramCsTypeName == "bool")
            {
                if (string.IsNullOrEmpty(_options.BooleanMarshalType))
                {
                    paramCsTypeName = _options.BooleanType;
                }
                else
                {
                    argumentBuilder.Append($"[MarshalAs(UnmanagedType.{_options.BooleanMarshalType})] ");
                    paramCsTypeName = "bool";
                }
            }
            else if (marshalString != MarshalStringType.None && IsString(cppParameter.Type, out _))
            {
                if (marshalString == MarshalStringType.Byte)
                {
                    paramCsTypeName = "ReadOnlySpan<byte>";
                }
                else
                {
                    paramCsTypeName = "[global::System.Runtime.InteropServices.Marshalling.MarshalUsing(typeof(Utf8CustomMarshaller))] ReadOnlySpan<char>";
                }
            }

            if (!invocation)
                argumentBuilder.Append(paramCsTypeName).Append(' ');

            argumentBuilder.Append(paramCsName);

            if (index < parameters.Count() - 1)
            {
                argumentBuilder.Append(", ");
            }

            index++;
        }

        return argumentBuilder.ToString();
    }

    private string GetCallbackMemberSignature(CppFunctionType functionType)
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

    private enum MarshalStringType
    {
        None,
        Byte,
        Char
    }
}
