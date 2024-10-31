// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Generator;

public sealed class CsCodeGeneratorOptions
{
    public string OutputPath { get; set; } = null!;
    public string ClassName { get; set; } = null!;
    public string? Namespace { get; set; }
    public bool PublicVisiblity { get; set; } = true;
    public bool EnumWriteUnmanagedTag { get; set; } = true;
    public bool GenerateCallbackTypes { get; set; }
    public bool MapCLongToIntPtr { get; set; }
    public string CallingConvention { get; set; } = "Cdecl";
    public string BooleanType { get; set; } = "SDLBool";
    public string? BooleanMarshalType { get;set; }
}
