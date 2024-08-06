// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3;

public enum SDL_Button : uint
{
    Left = SDL3.SDL_BUTTON_LEFT,
    Middle = SDL3.SDL_BUTTON_MIDDLE,
    Right = SDL3.SDL_BUTTON_RIGHT,
    X1 = SDL3.SDL_BUTTON_X1,
    X2 = SDL3.SDL_BUTTON_X2,
}


[Flags]
public enum SDL_MouseButtonFlags : uint
{
    Left = SDL3.SDL_BUTTON_LMASK,
    Middle = SDL3.SDL_BUTTON_MMASK,
    Right = SDL3.SDL_BUTTON_RMASK,
    X1 = SDL3.SDL_BUTTON_X1MASK,
    X2 = SDL3.SDL_BUTTON_X2MASK,
}

partial class SDL3
{
    public const uint SDL_BUTTON_LMASK = (1U << ((1) - 1));
    public const uint SDL_BUTTON_MMASK = (1U << ((2) - 1));
    public const uint SDL_BUTTON_RMASK = (1U << ((3) - 1));
    public const uint SDL_BUTTON_X1MASK = (1U << ((4) - 1));
    public const uint SDL_BUTTON_X2MASK = (1U << ((5) - 1));

    public static SDL_MouseButtonFlags SDL_BUTTON(SDL_Button button) => (SDL_MouseButtonFlags)(1 << ((int)button - 1));

    public static unsafe ReadOnlySpan<SDL_MouseID> SDL_GetMice()
    {
        SDL_MouseID* ptr = SDL_GetMice(out int count);
        return new(ptr, count);
    }
}
