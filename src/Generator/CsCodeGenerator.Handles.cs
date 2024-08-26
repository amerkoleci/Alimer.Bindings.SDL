// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
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
    };

    private static readonly HashSet<string> s_generatedPointerHandles = [];

    private static void CollectHandles(CppCompilation compilation)
    {
        foreach (CppTypedef typedef in compilation.Typedefs)
        {
            if (s_csNameMappings.ContainsKey(typedef.Name))
                continue;

            if (s_ignoreHandles.Contains(typedef.Name) || s_collectedHandles.ContainsKey(typedef.Name))
                continue;

            if (typedef.Name == "SDL_TimerCallback"
                || typedef.Name == "SDL_Surface"
                || typedef.Name.EndsWith("Callback")
                || typedef.Name.EndsWith("_func"))
            {
                continue;
            }

            string elementTypeName = GetCsTypeName(typedef.ElementType);
            s_collectedHandles.Add(typedef.Name, elementTypeName);
        }

        foreach (CppClass? cppClass in compilation.Classes)
        {
            if (cppClass.ClassKind == CppClassKind.Struct &&
                cppClass.SizeOf != 0)
            {
                continue;
            }

            if (s_ignoreHandles.Contains(cppClass.Name) || s_collectedHandles.ContainsKey(cppClass.Name))
                continue;

            string handleName = cppClass.Name;
            if (handleName == "SDL_GLContextState")
            {
                handleName = "SDL_GLContext";
                AddCsMapping(cppClass.Name, handleName);
            }

            s_collectedHandles.Add(handleName, "nint");
        }
    }

    private static void GenerateHandles()
    {
        string visibility = s_options.PublicVisiblity ? "public" : "internal";

        // Generate Functions
        using CodeWriter writer = new(Path.Combine(s_options.OutputPath, "Handles.cs"),
            true,
            s_options.Namespace, ["System.Diagnostics", "System.Diagnostics.CodeAnalysis"]
            );

        // First generate primitive types
        foreach (KeyValuePair<string, string> handlePair in s_collectedHandles)
        {
            string csName = handlePair.Key;
            if (s_csNameMappings.ContainsKey(csName))
                continue;

            string elementTypeName = handlePair.Value;
            bool isPrimitive = elementTypeName != "nint";
            if (!isPrimitive)
                continue;

            bool generateEnum = false;
            if (csName.EndsWith("Flags")
                || csName == "SDL_BlendMode"
                || csName == "SDL_Keymod")
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

                    if (!string.IsNullOrEmpty(constantPrefix))
                    {
                        foreach (CppMacro macro in s_collectedMacros)
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

                            string enumItemName = GetEnumItemName(csName, macro.Name, constantPrefix);
                            writer.WriteLine($"{enumItemName} = SDL3.{macro.Name},");
                        }
                    }
                }
            }
            writer.WriteLine();
        }


        foreach (KeyValuePair<string, string> handlePair in s_collectedHandles)
        {
            string csName = handlePair.Key;
            if (s_csNameMappings.ContainsKey(csName))
                continue;

            string elementTypeName = handlePair.Value;
            bool isPrimitive = elementTypeName != "nint";
            if (isPrimitive)
                continue;

            writer.WriteLine($"[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
            string typeDeclaration = $"{visibility} readonly partial struct {csName}({elementTypeName} value) : IEquatable<{csName}>";
            s_generatedPointerHandles.Add(csName);

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
