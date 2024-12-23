// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CppAst;

namespace Generator;

partial class CsCodeGenerator
{
    private static readonly HashSet<string> s_ignoreHandles = new(StringComparer.OrdinalIgnoreCase)
    {
        "SDL_bool",
        "SDL_malloc_func",
        "SDL_calloc_func",
        "SDL_realloc_func",
        "SDL_free_func",
        "SDL_CompareCallback_r",
        "SDL_iconv_t",
        "SDL_WindowsMessageHook",
        "SDL_LogOutputFunction",
        "MSG",
        "XEvent",
        "SDL_X11EventHook",
        "SDL_GLContext",
        "SDL_EGLDisplay",
        "SDL_EGLConfig",
        "SDL_EGLSurface",
        "SDL_EGLAttrib",
        "SDL_EGLint",
        "SDL_HitTest",
        "SDL_JoystickGUID",
        "SDL_EventFilter",
        "VkInstance",
        "VkSurfaceKHR",
        "SDL_vulkanInstance",
        "SDL_vulkanSurface",
        "SDL_MetalView",
        "SDL_blit",
        "SDL_WindowFlags",
        "SDL_Event",
        "SDL_HapticEffect",
        "SDL_EventAction",
        "VkInstance_T",
        "VkSurfaceKHR_T",
        "VkPhysicalDevice",
        "VkAllocationCallbacks",
        "SDL_TLSID",
        "SDL_ThreadFunction",
        "SDL_MouseButtonFlags",
        "SDL_PenCapabilityFlags",
        "SDL_GLContextFlag",
    };

    private readonly HashSet<string> _generatedPointerHandles = [];

    private void CollectHandles(CppCompilation compilation)
    {
        foreach (CppTypedef typedef in compilation.Typedefs)
        {
            if (s_csNameMappings.ContainsKey(typedef.Name))
                continue;

            if (s_ignoreHandles.Contains(typedef.Name) || _collectedHandles.ContainsKey(typedef.Name))
                continue;

            if (typedef.Name == "SDL_TimerCallback"
                || typedef.Name == "SDL_Surface"
                || typedef.Name.EndsWith("Callback")
                || typedef.Name.EndsWith("_func"))
            {
                continue;
            }

            string elementTypeName = GetCsTypeName(typedef.ElementType);
           _collectedHandles.Add(typedef.Name, Tuple.Create(elementTypeName, (CppTypeDeclaration)typedef));
        }

        foreach (CppClass? cppClass in compilation.Classes)
        {
            if (cppClass.ClassKind == CppClassKind.Struct &&
                cppClass.SizeOf != 0)
            {
                continue;
            }

            if (s_ignoreHandles.Contains(cppClass.Name) || _collectedHandles.ContainsKey(cppClass.Name))
                continue;

            string handleName = cppClass.Name;
            if (handleName == "SDL_GLContextState")
            {
                handleName = "SDL_GLContext";
                AddCsMapping(cppClass.Name, handleName);
            }

           _collectedHandles.Add(handleName, Tuple.Create("nint", (CppTypeDeclaration)cppClass));
        }
    }

    private void GenerateHandles()
    {
        string visibility = _options.PublicVisiblity ? "public" : "internal";

        // Generate Functions
        using CodeWriter writer = new(Path.Combine(_options.OutputPath, "Handles.cs"),
            true,
            _options.Namespace, ["System.Diagnostics", "System.Diagnostics.CodeAnalysis"]
            );

        // First generate primitive types
        foreach (KeyValuePair<string, Tuple<string, CppTypeDeclaration>> handlePair in _collectedHandles)
        {
            string csName = handlePair.Key;
            if (s_csNameMappings.ContainsKey(csName))
                continue;

            string elementTypeName = handlePair.Value.Item1;
            bool isPrimitive = elementTypeName != "nint";
            if (!isPrimitive)
                continue;

            writer.WriteComment(handlePair.Value.Item2.Comment?.ChildrenToString() ?? string.Empty);

            bool generateEnum = false;
            if (csName.EndsWith("Flags")
                || csName == "SDL_BlendMode"
                || csName == "SDL_Keymod"
                || csName == "SDL_GPUShaderFormat"
                || csName == "SDL_GLProfile"
                || csName == "SDL_GLContextFlag"
                || csName == "SDL_GLContextReleaseFlag"
                || csName == "SDL_GLContextResetNotification")
            {
                writer.WriteLine("[Flags]");
                generateEnum = true;
            }

            if (csName == "SDL_Keycode")
            {
                generateEnum = true;
            }

            using (writer.PushBlock($"public enum {csName} : {elementTypeName}"))
            {
                if (generateEnum)
                {
                    string constantPrefix = string.Empty;

                    if (csName == "SDL_InitFlags")
                    {
                        constantPrefix = "SDL_INIT_";
                    }
                    else if (csName == "SDL_BlendMode")
                    {
                        constantPrefix = "SDL_BLENDMODE_";
                    }
                    else if (csName == "SDL_GlobFlags")
                    {
                        constantPrefix = "SDL_GLOB_";
                    }
                    else if (csName == "SDL_Keycode")
                    {
                        constantPrefix = "SDLK_";
                    }
                    else if (csName == "SDL_Keymod")
                    {
                        constantPrefix = "SDL_KMOD_";
                    }
                    else if (csName == "SDL_MessageBoxFlags")
                    {
                        constantPrefix = "SDL_MESSAGEBOX_";
                    }
                    else if (csName == "SDL_MessageBoxButtonFlags")
                    {
                        constantPrefix = "SDL_MESSAGEBOX_BUTTON_";
                    }
                    else if (csName == "SDL_PenCapabilityFlags")
                    {
                        constantPrefix = "SDL_PEN_";
                    }
                    else if (csName == "SDL_SurfaceFlags")
                    {
                        constantPrefix = "SDL_SURFACE_";
                    }
                    else if (csName == "SDL_GPUTextureUsageFlags")
                    {
                        constantPrefix = "SDL_GPU_TEXTUREUSAGE_";
                    }
                    else if (csName == "SDL_GPUBufferUsageFlags")
                    {
                        constantPrefix = "SDL_GPU_BUFFERUSAGE_";
                    }
                    else if (csName == "SDL_GPUColorComponentFlags")
                    {
                        constantPrefix = "SDL_GPU_COLORCOMPONENT_";
                    }
                    else if (csName == "SDL_GPUShaderFormat")
                    {
                        constantPrefix = "SDL_GPU_SHADERFORMAT_";
                    }
                    else if (csName == "SDL_GLProfile")
                    {
                        constantPrefix = "SDL_GL_CONTEXT_PROFILE_";
                    }
                    else if (csName == "SDL_GLContextFlag")
                    {
                        constantPrefix = "SDL_GL_CONTEXT_";
                    }
                    else if (csName == "SDL_GLContextReleaseFlag")
                    {
                        constantPrefix = "SDL_GL_CONTEXT_RELEASE_BEHAVIOR_";
                    }
                    else if (csName == "SDL_GLContextResetNotification")
                    {
                        constantPrefix = "SDL_GL_CONTEXT_RESET_";
                    }

                    if (!string.IsNullOrEmpty(constantPrefix))
                    {
                        foreach (CppMacro macro in _collectedMacros)
                        {
                            if (!macro.Name.StartsWith(constantPrefix))
                                continue;

                            if (csName == "SDL_MessageBoxFlags")
                            {
                                if (macro.Name.StartsWith("SDL_MESSAGEBOX_BUTTON_"))
                                    continue;
                            }
                            else if (csName == "SDL_Keycode")
                            {
                                if (macro.Name == "SDLK_SCANCODE_MASK")
                                    continue;
                            }
                            else if (csName == "SDL_GLContextResetNotification")
                            {
                                if (macro.Name == "SDL_GL_CONTEXT_RESET_ISOLATION_FLAG")
                                    continue;
                            }

                            string enumItemName = GetEnumItemName(csName, macro.Name, constantPrefix);
                            writer.WriteLine($"{enumItemName} = " + _options.ClassName + $".{macro.Name},");
                        }

                        if (csName == "SDL_GPUColorComponentFlags")
                        {
                            writer.WriteLine("All = R | G | B | A");
                        }
                    }
                }
            }
            writer.WriteLine();
        }


        foreach (KeyValuePair<string, Tuple<string, CppTypeDeclaration>> handlePair in _collectedHandles)
        {
            string csName = handlePair.Key;
            if (s_csNameMappings.ContainsKey(csName))
                continue;

            string elementTypeName = handlePair.Value.Item1;
            bool isPrimitive = elementTypeName != "nint";
            if (isPrimitive)
                continue;

            writer.WriteComment(handlePair.Value.Item2.Comment?.ChildrenToString() ?? string.Empty);

            writer.WriteLine($"[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
            string typeDeclaration = $"{visibility} readonly partial struct {csName}({elementTypeName} value) : IEquatable<{csName}>";
            _generatedPointerHandles.Add(csName);

            using (writer.PushBlock(typeDeclaration))
            {
                writer.WriteLine($"public readonly {elementTypeName} Value = value;");

                if (!isPrimitive)
                {
                    writer.WriteLine($"public bool IsNull => Value == 0;");
                    writer.WriteLine($"public bool IsNotNull => Value != 0;");
                    writer.WriteLine($"public static {csName} Null => new(0);");
                }

                writer.WriteLine($"public static implicit operator {elementTypeName}({csName} value) => value.Value;");
                writer.WriteLine($"public static implicit operator {csName}({elementTypeName} value) => new(value);");

                writer.WriteLine($"public static bool operator ==({csName} left, {csName} right) => left.Value == right.Value;");
                writer.WriteLine($"public static bool operator !=({csName} left, {csName} right) => left.Value != right.Value;");
                if (isPrimitive)
                {
                    writer.WriteLine($"public static bool operator <({csName} left, {csName} right) => left.Value < right.Value;");
                    writer.WriteLine($"public static bool operator <=({csName} left, {csName} right) => left.Value <= right.Value;");
                    writer.WriteLine($"public static bool operator >({csName} left, {csName} right) => left.Value > right.Value;");
                    writer.WriteLine($"public static bool operator >=({csName} left, {csName} right) => left.Value >= right.Value;");

                    using (writer.PushBlock("public int CompareTo(object? obj)"))
                    {
                        using (writer.PushBlock($"if (obj is {csName} other)"))
                        {
                            writer.WriteLine("return CompareTo(other);");
                        }

                        writer.WriteLine($"return (obj is null) ? 1 : throw new ArgumentException($\"obj is not an instance of {nameof(csName)}.\");");
                    }

                    writer.WriteLine($"public int CompareTo({csName} other) => Value.CompareTo(other.Value);");
                }
                else
                {
                    writer.WriteLine($"public static bool operator ==({csName} left, {elementTypeName} right) => left.Value == right;");
                    writer.WriteLine($"public static bool operator !=({csName} left, {elementTypeName} right) => left.Value != right;");
                }

                writer.WriteLine($"public bool Equals({csName} other) => Value.Equals(other.Value);");

                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override bool Equals([NotNullWhen(true)] object? obj) => (obj is {csName} other) && Equals(other);");

                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override int GetHashCode() => Value.GetHashCode();");

                if (isPrimitive)
                {
                    writer.WriteLine("public override string ToString() => Value.ToString();");
                    writer.WriteLine("public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);");
                }
                else
                {
                    writer.WriteLine($"private string DebuggerDisplay => $\"{{nameof({csName})}} [0x{{Value.ToString(\"X\")}}]\";");
                }
            }

            writer.WriteLine();
        }
    }
}
