// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace SDL;

public delegate void SDL_ClipboardDataCallback(nint userData, string mimeType, out nuint size);
public delegate void SDL_ClipboardCleanupCallback(nint userData);

unsafe partial class SDL
{
    [DllImport(LibName, EntryPoint = nameof(SDL_SetClipboardText), CallingConvention = CallingConvention.Cdecl)]
    private static extern int INTERNAL_SDL_SetClipboardText(byte* text);

    public static int SDL_SetClipboardText(string text)
    {
        byte* utf8Text = Utf8EncodeHeap(text);
        int result = INTERNAL_SDL_SetClipboardText(utf8Text);
        NativeMemory.Free(utf8Text);
        return result;
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_GetClipboardText), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetClipboardText();

    public static string SDL_GetClipboardText()
    {
        return GetString(INTERNAL_SDL_GetClipboardText(), true);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasClipboardText();

    [DllImport(LibName, EntryPoint = nameof(SDL_SetPrimarySelectionText), CallingConvention = CallingConvention.Cdecl)]
    private static extern int INTERNAL_SDL_SetPrimarySelectionText(byte* text);

    public static int SDL_SetPrimarySelectionText(string text)
    {
        byte* utf8Text = Utf8EncodeHeap(text);
        int result = INTERNAL_SDL_SetPrimarySelectionText(utf8Text);
        NativeMemory.Free(utf8Text);
        return result;
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_GetPrimarySelectionText), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetPrimarySelectionText();

    public static string SDL_GetPrimarySelectionText()
    {
        return GetString(INTERNAL_SDL_GetPrimarySelectionText(), true);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasPrimarySelectionText();

    private static SDL_ClipboardDataCallback? s_clipboardDataCallback;
    private static SDL_ClipboardCleanupCallback? s_clipboardCleanupCallback;

    public static void SDL_SetClipboardData(
        SDL_ClipboardDataCallback? callback,
        SDL_ClipboardCleanupCallback? cleanup,
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
        SDL_ClipboardDataCallback? callback,
        SDL_ClipboardCleanupCallback? cleanup,
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

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_ClearClipboardData();

    [DllImport(LibName, EntryPoint = nameof(SDL_GetClipboardData), CallingConvention = CallingConvention.Cdecl)]
    private static extern void* INTERNAL_SDL_GetClipboardData(byte* mime_type, out nuint size);

    public static void* SDL_GetClipboardData(string mimeType, out nuint size)
    {
        byte* utf8Text = Utf8EncodeHeap(mimeType);
        void* result = INTERNAL_SDL_GetClipboardData(utf8Text, out size);
        NativeMemory.Free(utf8Text);
        return result;
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
}
