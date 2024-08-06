// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3;

unsafe partial class SDL3
{
    public static string SDL_GetScancodeNameString(SDL_Scancode scancode)
    {
        return GetStringOrEmpty(SDL_GetScancodeName(scancode));
    }

    /* Get a scancode from a human-readable name */
    public static SDL_Scancode SDL_GetScancodeFromName(ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
        {
            return SDL_GetScancodeFromName(pName);
        }
    }

    public static SDL_Scancode SDL_GetScancodeFromName(string text)
    {
        return SDL_GetScancodeFromName(text.GetUtf8Span());
    }

    public static string SDL_GetKeyNameString(SDL_Keycode key)
    {
        return GetStringOrEmpty(SDL_GetKeyName(key));
    }

    public static SDL_Keycode SDL_GetKeyFromName(ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return SDL_GetKeyFromName(pName);
    }

    public static SDL_Keycode SDL_GetKeyFromName(string text)
    {
        return SDL_GetKeyFromName(text.GetUtf8Span());
    }
}
