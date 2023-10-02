// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace SDL;

public delegate void ClipboardDataCallback(nint userData, string mimeType, out nuint size);
public delegate void ClipboardCleanupCallback(nint userData);

unsafe partial class SDL
{
    public static int SDL_SetClipboardText(ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
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
        return GetString(SDL_GetClipboardText());
    }

    public static int SDL_SetPrimarySelectionText(ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
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

    public static nint SDL_GetClipboardData(ReadOnlySpan<sbyte> mimeType, nuint* size)
    {
        fixed (sbyte* pName = mimeType)
        {
            return SDL_GetClipboardData(pName, size);
        }
    }

    public static nint SDL_GetClipboardData(string mimeType, nuint* size)
    {
        return SDL_GetClipboardData(mimeType.GetUtf8Span(), size);
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_HasClipboardData), CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_bool INTERNAL_SDL_HasClipboardData(byte* mime_type);

    public static bool SDL_HasClipboardData(string mimeType)
    {
        byte* utf8Text = Utf8EncodeHeap(mimeType);
        SDL_bool result = INTERNAL_SDL_HasClipboardData(utf8Text);
        NativeMemory.Free(utf8Text);
        return result == SDL_bool.SDL_TRUE;
    }

    private static ClipboardDataCallback? s_clipboardDataCallback;
    private static ClipboardCleanupCallback? s_clipboardCleanupCallback;

    public static void SDL_SetClipboardData(
        ClipboardDataCallback? callback,
        ClipboardCleanupCallback? cleanup,
        nint userData)
    {
        s_clipboardDataCallback = callback;
        s_clipboardCleanupCallback = cleanup;

        Internal_SDL_SetClipboardData(
            callback != null ? &OnNativeClipboardCallback : null,
            cleanup != null ? &OnNativeCleanupCallback : null,
            userData,
            null,
            0);
    }

    public static void SDL_SetClipboardData(
        ClipboardDataCallback? callback,
        ClipboardCleanupCallback? cleanup,
        nint userData,
        string[] mimeTypes)
    {
        s_clipboardDataCallback = callback;
        s_clipboardCleanupCallback = cleanup;

        byte** mimeTypesPtr = stackalloc byte*[mimeTypes.Length];
        for(int i  = 0; i < mimeTypes.Length; i++)
        {
            mimeTypesPtr[i] = Utf8EncodeHeap(mimeTypes[i]);
        }

        Internal_SDL_SetClipboardData(
            callback != null ? &OnNativeClipboardCallback : null,
            cleanup != null ? &OnNativeCleanupCallback : null,
            userData,
            mimeTypesPtr,
            (nuint)mimeTypes.Length);

        for (int i = 0; i < mimeTypes.Length; i++)
        {
            NativeMemory.Free(mimeTypesPtr[i]);
        }
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_SetClipboardData), CallingConvention = CallingConvention.Cdecl)]
    private static extern void Internal_SDL_SetClipboardData(
        delegate* unmanaged<nint, sbyte*, nuint*, void> callback,
        delegate* unmanaged<nint, void> cleanup,
        nint userdata,
        byte** mime_types, nuint num_mime_types);

    [UnmanagedCallersOnly]
    private static void OnNativeClipboardCallback(nint userdata, sbyte* mimeTypePtr, nuint* size)
    {
        string mimeType = new(mimeTypePtr);

        if (s_clipboardDataCallback != null)
        {
            s_clipboardDataCallback(userdata, mimeType, out nuint sizeCallback);
            *size = sizeCallback;
        }
    }

    [UnmanagedCallersOnly]
    private static void OnNativeCleanupCallback(nint userdata)
    {
        if (s_clipboardCleanupCallback != null)
        {
            s_clipboardCleanupCallback(userdata);
        }
    }
}
