// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
{
    private static readonly HashSet<string> s_ignoreHandles = new(StringComparer.OrdinalIgnoreCase)
    {
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
        "SDL_Keycode",
        "SDL_HapticEffect",
    };

    private static void CollectHandles(CppCompilation compilation)
    {
        foreach (CppTypedef typedef in compilation.Typedefs)
        {
            if (s_ignoreHandles.Contains(typedef.Name) || s_collectedHandles.ContainsKey(typedef.Name))
                continue;

            if (typedef.Name == "SDL_TimerCallback"
                || typedef.Name == "SDL_Surface"
                || typedef.Name.EndsWith("Callback"))
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

            s_collectedHandles.Add(cppClass.Name, "nint");
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

        foreach (KeyValuePair<string, string> handlePair in s_collectedHandles)
        {
            string csName = handlePair.Key;
            string elementTypeName = handlePair.Value;
            bool isPrimitive = elementTypeName != "nint";
            string typeDeclaration;
            if (isPrimitive)
            {
                typeDeclaration = $"{visibility} readonly partial struct {csName}({elementTypeName} value) : IComparable, IComparable<{csName}>, IEquatable<{csName}>, IFormattable";
            }
            else
            {
                writer.WriteLine($"[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
                typeDeclaration = $"{visibility} readonly partial struct {csName}({elementTypeName} value) : IEquatable<{csName}>";
            }
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
