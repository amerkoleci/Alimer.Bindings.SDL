﻿// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CppAst;

namespace Generator;

public static partial class CsCodeGenerator
{
    private static void GenerateHandles(CppCompilation compilation)
    {
        string visibility = s_options.PublicVisiblity ? "public" : "internal";

        // Generate Functions
        using var writer = new CodeWriter(Path.Combine(s_options.OutputPath, "Handles.cs"),
            true,
            s_options.Namespace, ["System.Diagnostics"]
            );

        foreach (CppTypedef typedef in compilation.Typedefs)
        {
            if (typedef.Name == "SDL_WindowsMessageHook"
                || typedef.Name == "SDL_TimerCallback"
                || typedef.Name == "SDL_LogOutputFunction"
                || typedef.Name == "SDL_Surface")
            {
                continue;
            }

            if (typedef.ElementType is not CppPointerType)
            {
                continue;
            }
            bool isDispatchable = true;

            string csName = typedef.Name;

            writer.WriteLine($"[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
            using (writer.PushBlock($"{visibility} readonly partial struct {csName} : IEquatable<{csName}>"))
            {
                string handleType = isDispatchable ? "nint" : "ulong";
                string nullValue = "0";

                writer.WriteLine($"public {csName}({handleType} handle) {{ Handle = handle; }}");
                writer.WriteLine($"public {handleType} Handle {{ get; }}");
                writer.WriteLine($"public bool IsNull => Handle == 0;");
                writer.WriteLine($"public bool IsNotNull => Handle != 0;");

                writer.WriteLine($"public static {csName} Null => new({nullValue});");
                writer.WriteLine($"public static implicit operator {csName}({handleType} handle) => new(handle);");
                writer.WriteLine($"public static bool operator ==({csName} left, {csName} right) => left.Handle == right.Handle;");
                writer.WriteLine($"public static bool operator !=({csName} left, {csName} right) => left.Handle != right.Handle;");
                writer.WriteLine($"public static bool operator ==({csName} left, {handleType} right) => left.Handle == right;");
                writer.WriteLine($"public static bool operator !=({csName} left, {handleType} right) => left.Handle != right;");
                writer.WriteLine($"public bool Equals({csName} other) => Handle == other.Handle;");
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override bool Equals(object? obj) => obj is {csName} handle && Equals(handle);");
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override int GetHashCode() => Handle.GetHashCode();");
                writer.WriteLine($"private string DebuggerDisplay => $\"{{nameof({csName})}} [0x{{Handle.ToString(\"X\")}}]\";");
            }

            writer.WriteLine();
        }
    }
}
