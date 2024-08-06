// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3;

public unsafe partial struct SDL_TextInputEvent
{
    public readonly string? GetText() => SDL3.GetString(text);
}

public unsafe partial struct SDL_TextEditingEvent
{
    public readonly string? GetText() => SDL3.GetString(text);
}

public unsafe partial struct SDL_DropEvent
{
    public readonly string? GetSource() => SDL3.GetString(source);

    public readonly string? GetData() => SDL3.GetString(data);
}

public partial struct SDL_MouseButtonEvent
{
    public SDL_Button Button => (SDL_Button)button;
}

public partial struct SDL_GamepadAxisEvent
{
    public SDL_GamepadAxis Axis => (SDL_GamepadAxis)axis;
}

public partial struct SDL_GamepadButtonEvent
{
    public SDL_GamepadButton Button => (SDL_GamepadButton)button;
}
