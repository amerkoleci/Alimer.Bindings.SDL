#region License
/* SDL2# - C# Wrapper for SDL2
 *
 * Copyright (c) 2013-2021 Ethan Lee.
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. If you use this software in a
 * product, an acknowledgment in the product documentation would be
 * appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source distribution.
 *
 * Ethan "flibitijibibo" Lee <flibitijibibo@flibitijibibo.com>
 *
 */
#endregion

// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static SDL.SDL;
using static SDL.SDL_bool;

namespace SDL;

#region Enums
public enum SDL_bool
{
    SDL_FALSE = 0,
    SDL_TRUE = 1
}

public enum SDL_HapticEffectType : ushort
{
    Constant = SDL_HAPTIC_CONSTANT,
    Sine = SDL_HAPTIC_SINE,
    LeftRight = SDL_HAPTIC_LEFTRIGHT,
    Triangle = SDL_HAPTIC_TRIANGLE,
    SawToothUp = SDL_HAPTIC_SAWTOOTHUP,
    SawToothDown = SDL_HAPTIC_SAWTOOTHDOWN,
    Spring = SDL_HAPTIC_SPRING,
    Damper = SDL_HAPTIC_DAMPER,
    Inertia = SDL_HAPTIC_INERTIA,
    Friction = SDL_HAPTIC_FRICTION,
    Custom = SDL_HAPTIC_CUSTOM,
    Gain = SDL_HAPTIC_GAIN,
    AutoCenter = SDL_HAPTIC_AUTOCENTER,
    Status = SDL_HAPTIC_STATUS,
    Pause = SDL_HAPTIC_PAUSE,
}

public enum SDL_HapticDirectionType : byte
{
    Polar = SDL_HAPTIC_POLAR,
    Cartesian = SDL_HAPTIC_CARTESIAN,
    Spherical = SDL_HAPTIC_SPHERICAL,
    SteeringAxis = SDL_HAPTIC_STEERING_AXIS,
}

public enum SDL_WinRT_DeviceFamily
{
    Unknown,
    Desktop,
    Mobile,
    Xbox
}

#endregion

public delegate void SDL_LogOutputFunction(SDL_LogCategory category, SDL_LogPriority priority, string description);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate IntPtr SDL_WindowsMessageHook(
    IntPtr userdata,
    IntPtr hWnd,
    uint message,
    ulong wParam,
    long lParam
);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate int SDL_EventFilter(nint userdata, nint @event);

// https://github.com/libsdl-org/SDL/blob/main/docs/README-migration.md

public static unsafe partial class SDL
{
    private const string LibName = "SDL3";

    static SDL()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static nint OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName.Equals(LibName) && TryResolveSDL3(assembly, searchPath, out nint nativeLibrary))
        {
            return nativeLibrary;
        }

        return IntPtr.Zero;
    }

    private static bool TryResolveSDL3(Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (NativeLibrary.TryLoad("SDL3.dll", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }
        else
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (NativeLibrary.TryLoad("libSDL3-0.0.so", assembly, searchPath, out nativeLibrary))
                {
                    return true;
                }

                if (NativeLibrary.TryLoad("libSDL3-0.0.so.0", assembly, searchPath, out nativeLibrary))
                {
                    return true;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (NativeLibrary.TryLoad("libSDL3.dylib", assembly, searchPath, out nativeLibrary))
                {
                    return true;
                }

                if (NativeLibrary.TryLoad("/usr/local/opt/SDL3/lib/libSDL3.dylib", assembly, searchPath, out nativeLibrary))
                {
                    return true;
                }
            }

            if (NativeLibrary.TryLoad("libSDL3", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }

        return false;
    }

    [LibraryImport(LibName)]
    private static partial void SDL_free(void* memblock);

    #region SDL_platform.h
    public static string SDL_GetPlatformString()
    {
        return GetString(SDL_GetPlatform())!;
    }
    #endregion

    #region SDL_rwops.h
    /* IntPtr refers to an SDL_RWops* */
    [DllImport(LibName, EntryPoint = "SDL_RWFromFile", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe IntPtr INTERNAL_SDL_RWFromFile(
        byte* file,
        byte* mode
    );
    public static unsafe IntPtr SDL_RWFromFile(
        string file,
        string mode
    )
    {
        byte* utf8File = Utf8EncodeHeap(file);
        byte* utf8Mode = Utf8EncodeHeap(mode);
        IntPtr rwOps = INTERNAL_SDL_RWFromFile(
            utf8File,
            utf8Mode
        );
        NativeMemory.Free(utf8Mode);
        NativeMemory.Free(utf8File);
        return rwOps;
    }
    #endregion

    #region SDL_hints.h
    public static SDL_bool SDL_SetHint(ReadOnlySpan<sbyte> name, ReadOnlySpan<sbyte> value)
    {
        fixed (sbyte* pName = name)
        {
            fixed (sbyte* pValue = value)
            {
                return SDL_SetHint(pName, pValue);
            }
        }
    }

    public static SDL_bool SDL_SetHint(string name, string value)
    {
        fixed (sbyte* pName = name.GetUtf8Span())
        {
            fixed (sbyte* pValue = value.GetUtf8Span())
            {
                return SDL_SetHint(pName, pValue);
            }
        }
    }

    public static SDL_bool SDL_SetHintWithPriority(ReadOnlySpan<sbyte> name, ReadOnlySpan<sbyte> value, SDL_HintPriority priority)
    {
        fixed (sbyte* pName = name)
        {
            fixed (sbyte* pValue = value)
            {
                return SDL_SetHintWithPriority(pName, pValue, priority);
            }
        }
    }

    public static SDL_bool SDL_SetHintWithPriority(string name, string value, SDL_HintPriority priority)
    {
        fixed (sbyte* pName = name.GetUtf8Span())
        {
            fixed (sbyte* pValue = value.GetUtf8Span())
            {
                return SDL_SetHintWithPriority(pName, pValue, priority);
            }
        }
    }

    public static SDL_bool SDL_SetHint(string name, bool value)
    {
        return SDL_SetHint(name, value ? "1" : "0");
    }
    #endregion

    #region SDL_error.h
    public static readonly int SDL_OutOfMemory = SDL_Error(SDL_errorcode.Enomem);
    public static readonly int SDL_Unsupported = SDL_Error(SDL_errorcode.Unsupported);
    //public static string SDL_InvalidParamError(string param)
    //{
    //    return SDL_SetError("Parameter '%s' is invalid", param);
    //}

    public static string? SDL_GetErrorString()
    {
        return GetString(SDL_GetError());
    }

    public static int SDL_SetError(ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
        {
            return SDL_SetError(pName);
        }
    }

    public static int SDL_SetError(string text)
    {
        return SDL_SetError(text.GetUtf8Span());
    }
    #endregion

    #region SDL_log.h

    private static SDL_LogOutputFunction? _logCallback;

    public static void SDL_LogSetPriority(SDL_LogCategory category, SDL_LogPriority priority)
    {
        SDL_LogSetPriority((int)category, priority);
    }

    public static void SDL_LogSetOutputFunction(SDL_LogOutputFunction? callback)
    {
        _logCallback = callback;

        Internal_SDL_LogSetOutputFunction(callback != null ? &OnNativeMessageCallback : null, IntPtr.Zero);
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_LogSetOutputFunction), CallingConvention = CallingConvention.Cdecl)]
    private static extern void Internal_SDL_LogSetOutputFunction(delegate* unmanaged<nint, int, SDL_LogPriority, sbyte*, void> callback, IntPtr userdata);

    [UnmanagedCallersOnly]
    private static unsafe void OnNativeMessageCallback(nint userdata, int category, SDL_LogPriority priority, sbyte* messagePtr)
    {
        string message = new(messagePtr);

        if (_logCallback != null)
        {
            _logCallback((SDL_LogCategory)category, priority, message);
        }
    }
    #endregion

    #region SDL_misc.h
    public static int SDL_OpenURL(ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
        {
            return SDL_OpenURL(pName);
        }
    }

    public static int SDL_OpenURL(string text)
    {
        return SDL_OpenURL(text.GetUtf8Span());
    }
    #endregion

    #region SDL_version.h, SDL_revision.h

    /* Similar to the headers, this is the version we're expecting to be
     * running with. You will likely want to check this somewhere in your
     * program!
     */
    public const int SDL_MAJOR_VERSION = 2;
    public const int SDL_MINOR_VERSION = 26;
    public const int SDL_PATCHLEVEL = 1;

    public static readonly int SDL_COMPILEDVERSION = SDL_VERSIONNUM(
        SDL_MAJOR_VERSION,
        SDL_MINOR_VERSION,
        SDL_PATCHLEVEL
    );

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_version
    {
        public byte major;
        public byte minor;
        public byte patch;
    }

    public static void SDL_VERSION(out SDL_version x)
    {
        x.major = SDL_MAJOR_VERSION;
        x.minor = SDL_MINOR_VERSION;
        x.patch = SDL_PATCHLEVEL;
    }

    public static int SDL_VERSIONNUM(int X, int Y, int Z)
    {
        return (X * 1000) + (Y * 100) + Z;
    }

    public static bool SDL_VERSION_ATLEAST(int X, int Y, int Z)
    {
        return (SDL_COMPILEDVERSION >= SDL_VERSIONNUM(X, Y, Z));
    }

    [LibraryImport(LibName)]
    public static partial int SDL_GetVersion(out SDL_version ver);

    [LibraryImport(LibName, EntryPoint = nameof(SDL_GetRevision))]
    private static partial sbyte* SDL_GetRevision();

    public static string SDL_GetRevisionString() => GetStringOrEmpty(SDL_GetRevision());
    #endregion

    #region SDL_video.h

    public const uint SDL_WINDOWPOS_UNDEFINED_MASK = 0x1FFF0000;
    public const uint SDL_WINDOWPOS_CENTERED_MASK = 0x2FFF0000;
    public const int SDL_WINDOWPOS_UNDEFINED = 0x1FFF0000;
    public const int SDL_WINDOWPOS_CENTERED = 0x2FFF0000;

    public static uint SDL_WINDOWPOS_UNDEFINED_DISPLAY(uint X)
    {
        return (SDL_WINDOWPOS_UNDEFINED_MASK | X);
    }

    public static bool SDL_WINDOWPOS_ISUNDEFINED(uint X)
    {
        return (X & 0xFFFF0000) == SDL_WINDOWPOS_UNDEFINED_MASK;
    }

    public static uint SDL_WINDOWPOS_CENTERED_DISPLAY(uint X)
    {
        return (SDL_WINDOWPOS_CENTERED_MASK | X);
    }

    public static bool SDL_WINDOWPOS_ISCENTERED(uint X)
    {
        return (X & 0xFFFF0000) == SDL_WINDOWPOS_CENTERED_MASK;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SDL_HitTestResult SDL_HitTest(IntPtr win, IntPtr area, IntPtr data);

    public static SDL_Window SDL_CreateWindow(ReadOnlySpan<sbyte> title, int width, int height, SDL_WindowFlags flags)
    {
        fixed (sbyte* pName = title)
        {
            return SDL_CreateWindow(pName, width, height, flags);
        }
    }

    public static SDL_Window SDL_CreateWindow(string title, int width, int height, SDL_WindowFlags flags)
    {
        fixed (sbyte* pName = title.GetUtf8Span())
        {
            return SDL_CreateWindow(pName, width, height, flags);
        }
    }

    //[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern int SDL_CreateWindowAndRenderer(int width, int height, SDL_WindowFlags windowFlags, out SDL_Window window, out SDL_Renderer renderer);

    public static SDL_Window SDL_CreateWindowWithPosition(string title, int x, int y, int width, int height, SDL_WindowFlags flags)
    {
        fixed (sbyte* pName = title.GetUtf8Span())
        {
            return SDL_CreateWindowWithPosition(pName, x, y, width, height, flags);
        }
    }

    public static SDL_Window SDL_CreateWindowWithPosition(ReadOnlySpan<sbyte> title, int x, int y, int width, int height, SDL_WindowFlags flags)
    {
        fixed (sbyte* pName = title)
        {
            return SDL_CreateWindowWithPosition(pName, x, y, width, height, flags);
        }
    }

    public static int SDL_SetWindowTitle(SDL_Window window, ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
        {
            return SDL_SetWindowTitle(window, pName);
        }
    }

    public static int SDL_SetWindowTitle(SDL_Window window, string text)
    {
        return SDL_SetWindowTitle(window, text.GetUtf8Span());
    }

    public static int SDL_SetWindowFullscreen(SDL_Window window, bool fullscreen)
    {
        return SDL_SetWindowFullscreen(window, fullscreen ? SDL_TRUE : SDL_FALSE);
    }

    public static string SDL_GetCurrentVideoDriverString()
    {
        return GetStringOrEmpty(SDL_GetCurrentVideoDriver());
    }

    public static ReadOnlySpan<SDL_DisplayID> SDL_GetDisplays()
    {
        SDL_DisplayID* displaysPtr = SDL_GetDisplays(out int count);
        return new(displaysPtr, count);
    }

    public static string SDL_GetDisplayNameString(SDL_DisplayID displayID)
    {
        return GetStringOrEmpty(SDL_GetDisplayName(displayID));
    }

    public static string SDL_GetVideoDriverString(int index) => GetStringOrEmpty(SDL_GetVideoDriver(index));

    [LibraryImport(LibName)]
    public static partial SDL_DisplayID SDL_GetDisplayForPoint(in Point point);

    [LibraryImport(LibName)]
    public static partial SDL_DisplayID SDL_GetDisplayForRect(in Rectangle rect);

    /* window refers to an SDL_Window*, IntPtr to a void* */
    [DllImport(LibName, EntryPoint = "SDL_GetWindowData", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe IntPtr INTERNAL_SDL_GetWindowData(
        SDL_Window window,
        byte* name
    );
    public static unsafe IntPtr SDL_GetWindowData(
        SDL_Window window,
        string name
    )
    {
        int utf8NameBufSize = Utf8Size(name);
        byte* utf8Name = stackalloc byte[utf8NameBufSize];
        return INTERNAL_SDL_GetWindowData(
            window,
            Utf8Encode(name, utf8Name, utf8NameBufSize)
        );
    }

    public static string SDL_GetWindowTitleString(IntPtr window)
    {
        return GetStringOrEmpty(SDL_GetWindowTitle(window));
    }

    public static int SDL_GL_LoadLibrary(ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
        {
            return SDL_GL_LoadLibrary(pName);
        }
    }

    public static delegate* unmanaged<void> SDL_GL_GetProcAddress(ReadOnlySpan<sbyte> proc)
    {
        fixed (sbyte* pName = proc)
        {
            return SDL_GL_GetProcAddress(pName);
        }
    }

    public static delegate* unmanaged<void> SDL_GL_GetProcAddress(string proc)
    {
        return SDL_GL_GetProcAddress(proc.GetUtf8Span());
    }

    public static delegate* unmanaged<void> SDL_EGL_GetProcAddress(ReadOnlySpan<sbyte> proc)
    {
        fixed (sbyte* pName = proc)
        {
            return SDL_EGL_GetProcAddress(pName);
        }
    }

    public static delegate* unmanaged<void> SDL_EGL_GetProcAddress(string proc)
    {
        return SDL_EGL_GetProcAddress(proc.GetUtf8Span());
    }

    public static bool SDL_GL_ExtensionSupported(ReadOnlySpan<sbyte> extension)
    {
        fixed (sbyte* pName = extension)
        {
            return SDL_GL_ExtensionSupported(pName) == SDL_TRUE;
        }
    }

    public static bool SDL_GL_ExtensionSupported(string extension)
    {
        return SDL_GL_ExtensionSupported(extension.GetUtf8Span());
    }

    public static int SDL_GL_SetAttribute(SDL_GLattr attr, bool value)
    {
        return SDL_GL_SetAttribute(attr, value ? 1 : 0);
    }

    public static int SDL_GL_SetAttribute(SDL_GLattr attr, SDL_GLprofile profile)
    {
        return SDL_GL_SetAttribute(attr, (int)profile);
    }
    #endregion

    #region SDL_properties.h
    [LibraryImport(LibName)]
    public static partial SDL_PropertiesID SDL_CreateProperties();

    [LibraryImport(LibName)]
    public static partial void SDL_DestroyProperties(SDL_PropertiesID propertiesID);

    [LibraryImport(LibName)]
    public static partial int SDL_LockProperties(SDL_PropertiesID propertiesID);

    [LibraryImport(LibName)]
    public static partial void SDL_UnlockProperties(SDL_PropertiesID propertiesID);

    [LibraryImport(LibName)]
    public static partial int SDL_SetProperty(SDL_PropertiesID properties, sbyte* name, nint value, delegate* unmanaged<nint, nint, void> cleanup, nint userdata);

    [LibraryImport(LibName)]
    public static partial nint SDL_GetProperty(SDL_PropertiesID properties, sbyte* name);

    [LibraryImport(LibName)]
    public static partial int SDL_ClearProperty(SDL_PropertiesID properties, sbyte* name);
    #endregion

    #region SDL_vulkan.h
    [DllImport(LibName, EntryPoint = "SDL_Vulkan_LoadLibrary", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int INTERNAL_SDL_Vulkan_LoadLibrary(
            byte* path
        );

    public static int SDL_Vulkan_LoadLibrary()
    {
        int result = INTERNAL_SDL_Vulkan_LoadLibrary(null);
        return result;
    }

    public static int SDL_Vulkan_LoadLibrary(string path)
    {
        byte* utf8Path = Utf8EncodeHeap(path);
        int result = INTERNAL_SDL_Vulkan_LoadLibrary(
            utf8Path
        );
        NativeMemory.Free(utf8Path);
        return result;
    }

    public static string[] SDL_Vulkan_GetInstanceExtensions()
    {
        string[] names = Array.Empty<string>();

        bool result = SDL_Vulkan_GetInstanceExtensions(out uint count, null) == SDL_TRUE;
        if (result == true)
        {
            sbyte** strings = stackalloc sbyte*[(int)count];
            names = new string[count];
            SDL_Vulkan_GetInstanceExtensions(out count, strings);

            for (uint i = 0; i < count; i++)
            {
                names[i] = GetString(strings[i])!;
            }
        }

        return names;
    }

    [LibraryImport(LibName, EntryPoint = "SDL_Vulkan_CreateSurface")]
    public static partial SDL_bool SDL_Vulkan_CreateSurface(SDL_Window window, nint instance, out ulong surface);
    #endregion

    #region SDL_syswm.h
    public static readonly int SDL_SYSWM_INFO_SIZE_V1 = (16 * (sizeof(void*) >= 8 ? sizeof(void*) : sizeof(ulong)));
    public static readonly int SDL_SYSWM_CURRENT_INFO_SIZE = SDL_SYSWM_INFO_SIZE_V1;
    #endregion

    #region SDL_stdinc.h
    [LibraryImport(LibName)]
    public static partial void* SDL_aligned_alloc(nuint alignment, nuint size);

    public static void* SDL_aligned_alloc(nuint size) => SDL_aligned_alloc(SDL_SIMDGetAlignment(), size);

    [LibraryImport(LibName)]
    public static partial void SDL_aligned_free(void* ptr);

    #endregion

    #region SDL_events.h

    //[DllImport(LibName, EntryPoint = nameof(SDL_GetEventFilter), CallingConvention = CallingConvention.Cdecl)]
    //private static extern SDL_bool SDL_GetEventFilter(out nint filter, out nint userdata);

    //public static bool SDL_GetEventFilter(out SDL_EventFilter? filter, out nint userdata)
    //{
    //    bool retVal = SDL_GetEventFilter(out nint result, out userdata) == SDL_TRUE;
    //    if (result != 0)
    //    {
    //        filter = Marshal.GetDelegateForFunctionPointer<SDL_EventFilter>(result);
    //    }
    //    else
    //    {
    //        filter = null;
    //    }

    //    return retVal;
    //}
    #endregion

    #region SDL_keycode.h
    public static SDL_KeyCode SDL_SCANCODE_TO_KEYCODE(SDL_Scancode X)
    {
        return (SDL_KeyCode)((int)X | SDLK_SCANCODE_MASK);
    }
    #endregion

    #region SDL_keyboard.h
    public static string SDL_GetScancodeNameString(SDL_Scancode scancode)
    {
        return GetStringOrEmpty(SDL_GetScancodeName(scancode));
    }

    /* Get a scancode from a human-readable name */
    public static SDL_Scancode SDL_GetScancodeFromName(ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
        {
            return SDL_GetScancodeFromName(pName);
        }
    }

    public static SDL_Scancode SDL_GetScancodeFromName(string text)
    {
        return SDL_GetScancodeFromName(text.GetUtf8Span());
    }

    public static string SDL_GetKeyNameString(SDL_KeyCode key)
    {
        return GetStringOrEmpty(SDL_GetKeyName(key));
    }


    public static SDL_KeyCode SDL_GetKeyFromName(ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
        {
            return SDL_GetKeyFromName(pName);
        }
    }

    public static SDL_KeyCode SDL_GetKeyFromName(string text)
    {
        return SDL_GetKeyFromName(text.GetUtf8Span());
    }
    #endregion

    #region SDL_mouse.c
    public static uint SDL_BUTTON(uint X)
    {
        // If only there were a better way of doing this in C#
        return (uint)(1 << ((int)X - 1));
    }

    public static readonly uint SDL_BUTTON_LMASK = SDL_BUTTON(SDL_BUTTON_LEFT);
    public static readonly uint SDL_BUTTON_MMASK = SDL_BUTTON(SDL_BUTTON_MIDDLE);
    public static readonly uint SDL_BUTTON_RMASK = SDL_BUTTON(SDL_BUTTON_RIGHT);
    public static readonly uint SDL_BUTTON_X1MASK = SDL_BUTTON(SDL_BUTTON_X1);
    public static readonly uint SDL_BUTTON_X2MASK = SDL_BUTTON(SDL_BUTTON_X2);
    #endregion

    #region SDL_haptic.h

    /* SDL_HapticEffect type */
    public const ushort SDL_HAPTIC_CONSTANT = (1 << 0);
    public const ushort SDL_HAPTIC_SINE = (1 << 1);
    public const ushort SDL_HAPTIC_LEFTRIGHT = (1 << 2);
    public const ushort SDL_HAPTIC_TRIANGLE = (1 << 3);
    public const ushort SDL_HAPTIC_SAWTOOTHUP = (1 << 4);
    public const ushort SDL_HAPTIC_SAWTOOTHDOWN = (1 << 5);
    public const ushort SDL_HAPTIC_SPRING = (1 << 7);
    public const ushort SDL_HAPTIC_DAMPER = (1 << 8);
    public const ushort SDL_HAPTIC_INERTIA = (1 << 9);
    public const ushort SDL_HAPTIC_FRICTION = (1 << 10);
    public const ushort SDL_HAPTIC_CUSTOM = (1 << 11);
    public const ushort SDL_HAPTIC_GAIN = (1 << 12);
    public const ushort SDL_HAPTIC_AUTOCENTER = (1 << 13);
    public const ushort SDL_HAPTIC_STATUS = (1 << 14);
    public const ushort SDL_HAPTIC_PAUSE = (1 << 15);

    /* SDL_HapticDirection type */
    public const byte SDL_HAPTIC_POLAR = 0;
    public const byte SDL_HAPTIC_CARTESIAN = 1;
    public const byte SDL_HAPTIC_SPHERICAL = 2;
    public const byte SDL_HAPTIC_STEERING_AXIS = 3;

    /* SDL_HapticRunEffect */
    public const uint SDL_HAPTIC_INFINITY = 4294967295U;

    public static string SDL_HapticNameString(int device_index)
    {
        return GetStringOrEmpty(SDL_HapticName(device_index));
    }
    #endregion

    #region SDL_timer.h
    public static ulong SDL_MS_TO_NS(ulong MS) => MS * SDL_NS_PER_MS;
    public static ulong SDL_NS_TO_MS(ulong NS) => NS / SDL_NS_PER_MS;
    public static ulong SDL_US_TO_NS(ulong US) => US * SDL_NS_PER_US;
    public static ulong SDL_NS_TO_US(ulong NS) => NS / SDL_NS_PER_US;
    #endregion

    #region SDL_system.h
    /* iOS */

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SDL_iPhoneAnimationCallback(IntPtr p);

    [LibraryImport(LibName)]
    public static partial int SDL_iPhoneSetAnimationCallback(
        SDL_Window window, /* SDL_Window* */
        int interval,
        SDL_iPhoneAnimationCallback callback,
        IntPtr callbackParam
    );

    [LibraryImport(LibName)]
    public static partial void SDL_iPhoneSetEventPump(SDL_bool enabled);

    /* Android */

    public const int SDL_ANDROID_EXTERNAL_STORAGE_READ = 0x01;
    public const int SDL_ANDROID_EXTERNAL_STORAGE_WRITE = 0x02;

    /* IntPtr refers to a JNIEnv* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_AndroidGetJNIEnv();

    /* IntPtr refers to a jobject */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_AndroidGetActivity();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_IsAndroidTV();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_IsChromebook();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_IsDeXMode();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_AndroidBackButton();

    [DllImport(LibName, EntryPoint = "SDL_AndroidGetInternalStoragePath", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_AndroidGetInternalStoragePath();

    public static string SDL_AndroidGetInternalStoragePath()
    {
        return GetString(
            INTERNAL_SDL_AndroidGetInternalStoragePath()
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_AndroidGetExternalStorageState();

    [DllImport(LibName, EntryPoint = "SDL_AndroidGetExternalStoragePath", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_AndroidGetExternalStoragePath();

    public static string SDL_AndroidGetExternalStoragePath()
    {
        return GetString(
            INTERNAL_SDL_AndroidGetExternalStoragePath()
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetAndroidSDKVersion();

    /* Only available in 2.0.14 or higher. */
    [DllImport(LibName, EntryPoint = "SDL_AndroidRequestPermission", CallingConvention = CallingConvention.Cdecl)]
    private static unsafe extern SDL_bool INTERNAL_SDL_AndroidRequestPermission(
        byte* permission
    );
    public static unsafe SDL_bool SDL_AndroidRequestPermission(
        string permission
    )
    {
        byte* permissionPtr = Utf8EncodeHeap(permission);
        SDL_bool result = INTERNAL_SDL_AndroidRequestPermission(
            permissionPtr
        );

        NativeMemory.Free(permissionPtr);
        return result;
    }

    /* Only available in 2.0.16 or higher. */
    [DllImport(LibName, EntryPoint = "SDL_AndroidShowToast", CallingConvention = CallingConvention.Cdecl)]
    private static unsafe extern int INTERNAL_SDL_AndroidShowToast(
        byte* message,
        int duration,
        int gravity,
        int xOffset,
        int yOffset
    );
    public static unsafe int SDL_AndroidShowToast(
        string message,
        int duration,
        int gravity,
        int xOffset,
        int yOffset
    )
    {
        byte* messagePtr = Utf8EncodeHeap(message);
        int result = INTERNAL_SDL_AndroidShowToast(
            messagePtr,
            duration,
            gravity,
            xOffset,
            yOffset
        );

        NativeMemory.Free(messagePtr);
        return result;
    }

    /* WinRT */
    [LibraryImport(LibName)]
    public static partial SDL_WinRT_DeviceFamily SDL_WinRTGetDeviceFamily();
    #endregion

    #region Marshal
    /// <inheritdoc cref="Unsafe.SizeOf{T}" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint SizeOf<T>() => unchecked((uint)Unsafe.SizeOf<T>());

    /// <inheritdoc cref="Unsafe.AsPointer{T}(ref T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AsPointer<T>(ref T source) where T : unmanaged => (T*)Unsafe.AsPointer(ref source);

    /// <inheritdoc cref="Unsafe.As{TFrom, TTo}(ref TFrom)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly TTo AsReadOnly<TFrom, TTo>(in TFrom source) => ref Unsafe.As<TFrom, TTo>(ref AsRef(in source));

    /// <summary>Reinterprets the given native integer as a reference.</summary>
    /// <typeparam name="T">The type of the reference.</typeparam>
    /// <param name="source">The native integer to reinterpret.</param>
    /// <returns>A reference to a value of type <typeparamref name="T" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsRef<T>(nint source) => ref Unsafe.AsRef<T>((void*)source);

    /// <summary>Reinterprets the given native unsigned integer as a reference.</summary>
    /// <typeparam name="T">The type of the reference.</typeparam>
    /// <param name="source">The native unsigned integer to reinterpret.</param>
    /// <returns>A reference to a value of type <typeparamref name="T" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsRef<T>(nuint source) => ref Unsafe.AsRef<T>((void*)source);

    /// <inheritdoc cref="Unsafe.AsRef{T}(in T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsRef<T>(in T source) => ref Unsafe.AsRef(in source);

    /// <inheritdoc cref="MemoryMarshal.CreateReadOnlySpan{T}(ref T, int)" />
    public static ReadOnlySpan<T> CreateReadOnlySpan<T>(scoped in T reference, int length) => MemoryMarshal.CreateReadOnlySpan(ref AsRef(in reference), length);

    // <summary>Returns a pointer to the element of the span at index zero.</summary>
    /// <typeparam name="T">The type of items in <paramref name="span" />.</typeparam>
    /// <param name="span">The span from which the pointer is retrieved.</param>
    /// <returns>A pointer to the item at index zero of <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPointer<T>(this Span<T> span)
        where T : unmanaged => AsPointer(ref span.GetReference());

    /// <summary>Returns a pointer to the element of the span at index zero.</summary>
    /// <typeparam name="T">The type of items in <paramref name="span" />.</typeparam>
    /// <param name="span">The span from which the pointer is retrieved.</param>
    /// <returns>A pointer to the item at index zero of <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPointer<T>(this ReadOnlySpan<T> span)
        where T : unmanaged => AsPointer(ref AsRef(in span.GetReference()));

    /// <inheritdoc cref="MemoryMarshal.GetReference{T}(Span{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetReference<T>(this Span<T> span) => ref MemoryMarshal.GetReference(span);

    /// <inheritdoc cref="MemoryMarshal.GetReference{T}(ReadOnlySpan{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T GetReference<T>(this ReadOnlySpan<T> span) => ref MemoryMarshal.GetReference(span);

    /// <inheritdoc cref="Unsafe.As{TFrom, TTo}(ref TFrom)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref TTo As<TFrom, TTo>(ref TFrom source)
        => ref Unsafe.As<TFrom, TTo>(ref source);

    /// <inheritdoc cref="Unsafe.As{TFrom, TTo}(ref TFrom)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<TTo> As<TFrom, TTo>(this ReadOnlySpan<TFrom> span)
        where TFrom : unmanaged
        where TTo : unmanaged
    {
        return CreateReadOnlySpan(in AsReadOnly<TFrom, TTo>(in span.GetReference()), span.Length);
    }

    /// <inheritdoc cref="Unsafe.IsNullRef{T}(ref T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(in T source) => Unsafe.IsNullRef(ref AsRef(in source));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<sbyte> GetUtf8Span(this string? source)
    {
        ReadOnlySpan<byte> result;

        if (source is not null)
        {
            var maxLength = Encoding.UTF8.GetMaxByteCount(source.Length);
            var bytes = new byte[maxLength + 1];

            var length = Encoding.UTF8.GetBytes(source, bytes);
            result = bytes.AsSpan(0, length);
        }
        else
        {
            result = null;
        }

        return result.As<byte, sbyte>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<sbyte> GetUtf8Span(sbyte* source, int maxLength = -1)
    {
        return (source != null) ? GetUtf8Span(in source[0], maxLength) : null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<sbyte> GetUtf8Span(in sbyte source, int maxLength = -1)
    {
        ReadOnlySpan<sbyte> result;

        if (!IsNullRef(in source))
        {
            if (maxLength < 0)
            {
                maxLength = int.MaxValue;
            }

            result = CreateReadOnlySpan(in source, maxLength);
            var length = result.IndexOf((sbyte)'\0');

            if (length != -1)
            {
                result = result.Slice(0, length);
            }
        }
        else
        {
            result = null;
        }

        return result;
    }

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetString(this ReadOnlySpan<sbyte> span)
    {
        return span.GetPointer() != null ? Encoding.UTF8.GetString(span.As<sbyte, byte>()) : null;
    }


    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetStringOrEmpty(this ReadOnlySpan<sbyte> span)
    {
        return span.GetPointer() != null ? Encoding.UTF8.GetString(span.As<sbyte, byte>()) : string.Empty;
    }

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetString(sbyte* source, int maxLength = -1)
    {
        return GetUtf8Span(source, maxLength).GetString();
    }

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetStringOrEmpty(sbyte* source, int maxLength = -1)
    {
        return GetUtf8Span(source, maxLength).GetStringOrEmpty();
    }

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetString(this ReadOnlySpan<ushort> span)
    {
        return span.GetPointer() != null ? new string(span.As<ushort, char>()) : null;
    }

    private static string GetString(byte* ptr)
    {
        if (ptr == null)
        {
            return string.Empty;
        }

        int characters = 0;
        while (ptr[characters] != 0)
        {
            characters++;
        }

        return Encoding.UTF8.GetString(ptr, characters);
    }

    public static string GetString(byte* s, bool freePtr = false)
    {
        if (s == null)
        {
            return string.Empty;
        }

        /* We get to do strlen ourselves! */
        byte* ptr = (byte*)s;
        while (*ptr != 0)
        {
            ptr++;
        }

        /* TODO: This #ifdef is only here because the equivalent
         * .NET 2.0 constructor appears to be less efficient?
         * Here's the pretty version, maybe steal this instead:
         *
        string result = new string(
            (sbyte*) s, // Also, why sbyte???
            0,
            (int) (ptr - (byte*) s),
            System.Text.Encoding.UTF8
        );
         * See the CoreCLR source for more info.
         * -flibit
         */
        /* Modern C# lets you just send the byte*, nice! */
        string result = Encoding.UTF8.GetString(s, (int)(ptr - s));

        /* Some SDL functions will malloc, we have to free! */
        if (freePtr)
        {
            SDL_free(s);
        }

        return result;
    }


    /* Used for stack allocated string marshaling. */
    private static int Utf8Size(string str)
    {
        if (str == null)
        {
            return 0;
        }
        return (str.Length * 4) + 1;
    }

    private static byte* Utf8EncodeHeap(string str)
    {
        if (str == null)
        {
            return (byte*)0;
        }

        int bufferSize = Utf8Size(str);
        byte* buffer = (byte*)NativeMemory.Alloc((nuint)bufferSize);
        fixed (char* strPtr = str)
        {
            Encoding.UTF8.GetBytes(strPtr, str.Length + 1, buffer, bufferSize);
        }
        return buffer;
    }


    private static byte* Utf8Encode(string str, byte* buffer, int bufferSize)
    {
        if (str == null)
        {
            return (byte*)0;
        }
        fixed (char* strPtr = str)
        {
            Encoding.UTF8.GetBytes(strPtr, str.Length + 1, buffer, bufferSize);
        }
        return buffer;
    }
    #endregion
}
