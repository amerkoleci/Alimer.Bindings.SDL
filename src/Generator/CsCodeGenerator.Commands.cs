// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Text;
using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
{
    private static string GetFunctionPointerSignature(CppFunction function)
    {
        return GetFunctionPointerSignature(function.ReturnType, function.Parameters);
    }

    private static string GetFunctionPointerSignature(CppType returnType, CppContainerList<CppParameter> parameters)
    {
        StringBuilder builder = new();
        foreach (CppParameter parameter in parameters)
        {
            string paramCsType = GetCsTypeName(parameter.Type, false);

            //if (CanBeUsedAsOutput(parameter.Type, out CppTypeDeclaration? cppTypeDeclaration))
            //{
            //    builder.Append("out ");
            //    paramCsType = GetCsTypeName(cppTypeDeclaration, false);
            //}

            builder.Append(paramCsType).Append(", ");
        }

        string returnCsName = GetCsTypeName(returnType, false);
        builder.Append(returnCsName);

        return $"delegate* unmanaged<{builder}>";
    }

    private static void CollectCommands(CppCompilation compilation)
    {
        foreach (CppTypedef typedef in compilation.Typedefs)
        {
            if (typedef.Name == "WGPUProc" ||
                !typedef.Name.EndsWith("Callback"))
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
            if (cppFunction.Name == "SDL_LogMessageV" ||
                cppFunction.Name == "SDL_LogGetOutputFunction" ||
                cppFunction.Name == "SDL_LogSetOutputFunction")
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
        foreach (KeyValuePair<string, CppFunctionType> pair in s_collectedCallbackTypedes)
        {
            CppFunctionType functionType = pair.Value;
            //string functionPointerSignature = GetFunctionPointerSignature(functionType);
            //AddCsMapping(typedef.Name, functionPointerSignature);

            string returnCsName = GetCsTypeName(functionType.ReturnType, false);
            string argumentsString = GetParameterSignature(functionType);

            writer.WriteLine($"[UnmanagedFunctionPointer(CallingConvention.Cdecl)]");
            writer.WriteLine($"{visibility} unsafe delegate {returnCsName} {pair.Key}({argumentsString});");
            writer.WriteLine();
        }

        using (writer.PushBlock($"{visibility} unsafe partial class {s_options.ClassName}"))
        {
            foreach (CppFunction cppFunction in s_collectedFunctions)
            {
                string name = cppFunction.Name;
                if (s_options.GenerateFunctionPointers)
                {
                    string functionPointerSignature = GetFunctionPointerSignature(cppFunction);
                    writer.WriteLine($"private static {functionPointerSignature} {name}_ptr;");
                }

                WriteFunctionInvocation(writer, cppFunction, s_options.GenerateFunctionPointers);
            }

            if (s_options.GenerateFunctionPointers)
            {
                WriteCommands(writer, "GenLoadCommands", s_collectedFunctions);
            }
        }
    }

    private static void WriteCommands(CodeWriter writer, string name, List<CppFunction> commands)
    {
        using (writer.PushBlock($"private static void {name}()"))
        {
            foreach (CppFunction instanceCommand in commands)
            {
                string commandName = instanceCommand.Name;
                string functionPointerSignature = GetFunctionPointerSignature(instanceCommand);

                if (commandName.EndsWith("Drop"))
                {
                    //commandName = commandName.Replace("Drop", "Release");
                    writer.WriteLine($"{commandName}_ptr = ({functionPointerSignature}) LoadFunctionPointer(\"{commandName}\");");
                }
                else
                {
                    writer.WriteLine($"{commandName}_ptr = ({functionPointerSignature}) LoadFunctionPointer(nameof({commandName}));");
                }
            }
        }
    }

    private static void WriteFunctionInvocation(CodeWriter writer, CppFunction cppFunction, bool useFunctionPointers)
    {
        if (cppFunction.Name == "SDL_AddGamepadMappingsFromRW")
        {
        }

        string returnCsName = GetCsTypeName(cppFunction.ReturnType, false);
        string argumentsString = GetParameterSignature(cppFunction);
        string functionName = cppFunction.Name;

        string modifier = "public static";
        if (!useFunctionPointers)
        {
            modifier += " extern";
            writer.WriteLine($"[DllImport(LibName, CallingConvention = CallingConvention.Cdecl, EntryPoint = \"{cppFunction.Name}\")]");
        }

        if (useFunctionPointers)
        {
            using (writer.PushBlock($"{modifier} {returnCsName} {functionName}({argumentsString})"))
            {
                if (returnCsName != "void")
                {
                    writer.Write("return ");
                }

                writer.Write($"{cppFunction.Name}_ptr(");

                int index = 0;
                foreach (CppParameter cppParameter in cppFunction.Parameters)
                {
                    string paramCsName = GetParameterName(cppParameter.Name);

                    //if (CanBeUsedAsOutput(cppParameter.Type, out CppTypeDeclaration? cppTypeDeclaration))
                    //{
                    //    writer.Write("out ");
                    //}

                    writer.Write($"{paramCsName}");

                    if (index < cppFunction.Parameters.Count - 1)
                    {
                        writer.Write(", ");
                    }

                    index++;
                }

                writer.WriteLine(");");
            }
        }
        else
        {
            writer.WriteLine($"{modifier} {returnCsName} {functionName}({argumentsString});");
        }

        writer.WriteLine();

        if (returnCsName == "void" &&
            (cppFunction.Name.EndsWith("SetLabel") ||
            cppFunction.Name.EndsWith("InsertDebugMarker") ||
            cppFunction.Name.EndsWith("PushDebugGroup")
            ))
        {
            IEnumerable<CppParameter> parameters = cppFunction.Parameters.Take(cppFunction.Parameters.Count - 1);
            string paramCsName = GetParameterName(cppFunction.Parameters.Last().Name);
            argumentsString = GetParameterSignature(cppFunction.Name, parameters);

            using (writer.PushBlock($"public static void {cppFunction.Name}({argumentsString}, ReadOnlySpan<sbyte> {paramCsName})"))
            {
                string pointerName = "p" + char.ToUpperInvariant(paramCsName[0]) + paramCsName.Substring(1);
                using (writer.PushBlock($"fixed (sbyte* {pointerName} = {paramCsName})"))
                {
                    if (useFunctionPointers)
                    {
                        writer.Write($"{cppFunction.Name}_ptr(");
                    }
                    else
                    {
                        writer.Write($"{cppFunction.Name}(");
                    }

                    int index = 0;
                    foreach (CppParameter cppParameter in parameters)
                    {
                        string localParamCsName = GetParameterName(cppParameter.Name);

                        writer.Write($"{localParamCsName}");

                        if (index < cppFunction.Parameters.Count - 1)
                        {
                            writer.Write(", ");
                        }

                        index++;
                    }

                    writer.Write(pointerName);
                    writer.WriteLine(");");
                }
            }

            writer.WriteLine();

            using (writer.PushBlock($"public static void {cppFunction.Name}({argumentsString}, string? {paramCsName} = default)"))
            {
                string instanceParamName = GetParameterName(cppFunction.Parameters[0].Name);
                writer.WriteLine($"{cppFunction.Name}({instanceParamName}, {paramCsName}.GetUtf8Span());");
            }
            writer.WriteLine();
        }
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
            string paramCsTypeName = GetCsTypeName(cppParameter.Type, false);
            string paramCsName = GetParameterName(cppParameter.Name);

            if (paramCsName == "flags")
            {
                if (functionName == "SDL_Init" ||
                    functionName == "SDL_InitSubSystem" ||
                    functionName == "SDL_QuitSubSystem" ||
                    functionName == "SDL_WasInit")
                {
                    paramCsTypeName = "SDL_InitFlags";
                }
            }

            if (paramCsName == "sbyte*" && unsafeStrings == false)
            {
                paramCsName = "ReadOnlySpan<sbyte>";
            }

            argumentBuilder.Append(paramCsTypeName).Append(' ').Append(paramCsName);

            //if (paramCsTypeName == "nint" && paramCsName == "userdata")
            //{
            //    argumentBuilder.Append(" = 0");
            //}

            if (functionName.EndsWith("SetVertexBuffer") || functionName.EndsWith("SetIndexBuffer"))
            {
                if (paramCsName == "offset")
                {
                    argumentBuilder.Append(" = 0");
                }
                else if (paramCsName == "size")
                {
                    argumentBuilder.Append(" = WGPU_WHOLE_SIZE");
                }
            }

            if (functionName.EndsWith("Draw") ||
                functionName.EndsWith("DrawIndexed"))
            {
                if (paramCsName == "instanceCount")
                {
                    argumentBuilder.Append(" = 1");
                }
                else if (paramCsName == "firstVertex" ||
                    paramCsName == "firstIndex" ||
                    paramCsName == "baseVertex" ||
                    paramCsName == "firstInstance")
                {
                    argumentBuilder.Append(" = 0");
                }
            }

            if (index < parameters.Count() - 1)
            {
                argumentBuilder.Append(", ");
            }

            index++;
        }

        return argumentBuilder.ToString();
    }

    private static string GetParameterName(string name)
    {
        if (name == "event")
            return "@event";

        if (name == "object")
            return "@object";

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
