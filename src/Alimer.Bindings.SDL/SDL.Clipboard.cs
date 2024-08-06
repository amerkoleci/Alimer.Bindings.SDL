// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace SDL3;

public delegate void ClipboardDataCallback(nint userData, string mimeType, out nuint size);
public delegate void ClipboardCleanupCallback(nint userData);

unsafe partial class SDL3
{
    public static int SDL_SetClipboardText(ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
        {
            return SDL_SetClipboardText(pName);
        }
    }

    public static int SDL_SetClipboardText(string text)
    {
        return SDL_SetClipboardText(text.GetUtf8Span());
    }

    public static string? SDL_GetClipboardTextString()
    {
        byte* textPtr = SDL_GetClipboardText();
        string? result = GetString(textPtr);
        SDL_free(textPtr);
        return result;
    }

    public static int SDL_SetPrimarySelectionText(ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
        {
            return SDL_SetPrimarySelectionText(pName);
        }
    }

    public static int SDL_SetPrimarySelectionText(string text)
    {
        return SDL_SetPrimarySelectionText(text.GetUtf8Span());
    }

    public static string? SDL_GetPrimarySelectionTextString()
    {
        return GetString(SDL_GetPrimarySelectionText());
    }

    public static nint SDL_GetClipboardData(ReadOnlySpan<byte> mimeType, nuint* size)
    {
        fixed (byte* pName = mimeType)
        {
            return (nint)SDL_GetClipboardData(pName, size);
        }
    }

    public static nint SDL_GetClipboardData(string mimeType, nuint* size)
    {
        return SDL_GetClipboardData(mimeType.GetUtf8Span(), size);
    }

    public static bool SDL_HasClipboardData(ReadOnlySpan<byte> mimeType)
    {
        fixed (byte* pName = mimeType)
        {
            return SDL_HasClipboardData(pName);
        }
    }

    public static bool SDL_HasClipboardData(string mimeType)
    {
        return SDL_HasClipboardData(mimeType.GetUtf8Span());
    }
}
