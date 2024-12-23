// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static SDL3.SDL3;

namespace SDL3;

#region Enums
public enum SDL_bool : byte
{
    SDL_FALSE = SDL3.SDL_FALSE,
    SDL_TRUE = SDL3.SDL_TRUE
}

[Flags]
public enum SDL_WindowFlags : ulong
{
    None = 0,
    /// <unmanaged>SDL_WINDOW_FULLSCREEN</unmanaged>
    Fullscreen = SDL_WINDOW_FULLSCREEN,
    /// <unmanaged>SDL_WINDOW_OPENGL</unmanaged>
    OpenGL = SDL_WINDOW_OPENGL,
    /// <unmanaged>SDL_WINDOW_OCCLUDED</unmanaged>
    Occluded = SDL_WINDOW_OCCLUDED,
    /// <unmanaged>SDL_WINDOW_HIDDEN</unmanaged>
    Hidden = SDL_WINDOW_HIDDEN,
    /// <unmanaged>SDL_WINDOW_BORDERLESS</unmanaged>
    Borderless = SDL_WINDOW_BORDERLESS,
    /// <unmanaged>SDL_WINDOW_RESIZABLE</unmanaged>
    Resizable = SDL_WINDOW_RESIZABLE,
    /// <unmanaged>SDL_WINDOW_MINIMIZED</unmanaged>
    Minimized = SDL_WINDOW_MINIMIZED,
    /// <unmanaged>SDL_WINDOW_MAXIMIZED</unmanaged>
    Maximized = SDL_WINDOW_MAXIMIZED,
    /// <unmanaged>SDL_WINDOW_MOUSE_GRABBED</unmanaged>
    MouseGrabbed = SDL_WINDOW_MOUSE_GRABBED,
    /// <unmanaged>SDL_WINDOW_INPUT_FOCUS</unmanaged>
    InputFocus = SDL_WINDOW_INPUT_FOCUS,
    /// <unmanaged>SDL_WINDOW_MOUSE_FOCUS</unmanaged>
    MouseFocus = SDL_WINDOW_MOUSE_FOCUS,
    /// <unmanaged>SDL_WINDOW_EXTERNAL</unmanaged>
    External = SDL_WINDOW_EXTERNAL,
    /// <unmanaged>SDL_WINDOW_HIGH_PIXEL_DENSITY</unmanaged>
    HighPixelDensity = SDL_WINDOW_HIGH_PIXEL_DENSITY,
    /// <unmanaged>SDL_WINDOW_MOUSE_CAPTURE</unmanaged>
    MouseCapture = SDL_WINDOW_MOUSE_CAPTURE,
    /// <unmanaged>SDL_WINDOW_ALWAYS_ON_TOP</unmanaged>
    AlwaysOnTop = SDL_WINDOW_ALWAYS_ON_TOP,
    /// <unmanaged>SDL_WINDOW_UTILITY</unmanaged>
    Utility = SDL_WINDOW_UTILITY,
    /// <unmanaged>SDL_WINDOW_TOOLTIP</unmanaged>
    Tooltip = SDL_WINDOW_TOOLTIP,
    /// <unmanaged>SDL_WINDOW_POPUP_MENU</unmanaged>
    PopupMenu = SDL_WINDOW_POPUP_MENU,
    /// <unmanaged>SDL_WINDOW_KEYBOARD_GRABBED</unmanaged>
    KeyboardGrabbed = SDL_WINDOW_KEYBOARD_GRABBED,
    /// <unmanaged>SDL_WINDOW_VULKAN</unmanaged>
    Vulkan = SDL_WINDOW_VULKAN,
    /// <unmanaged>SDL_WINDOW_METAL</unmanaged>
    Metal = SDL_WINDOW_METAL,
    /// <unmanaged>SDL_WINDOW_TRANSPARENT</unmanaged>
    Transparent = SDL_WINDOW_TRANSPARENT,
    /// <unmanaged>SDL_WINDOW_NOT_FOCUSABLE</unmanaged>
    NotFocusable = SDL_WINDOW_NOT_FOCUSABLE,
}
#endregion

public delegate void SDL_LogOutputFunction(SDL_LogCategory category, SDL_LogPriority priority, string? message);

// https://github.com/libsdl-org/SDL/blob/main/docs/README-migration.md

public static unsafe partial class SDL3
{
    private const DllImportSearchPath DefaultDllImportSearchPath = DllImportSearchPath.ApplicationDirectory | DllImportSearchPath.UserDirectories | DllImportSearchPath.UseDllDirectoryForDependencies;

    /// <summary>
    /// Raised whenever a native library is loaded by SDL.
    /// Handlers can be added to this event to customize how libraries are loaded, and they will be used first whenever a new native library is being resolved.
    /// </summary>
    public static event DllImportResolver? SDL_DllImporterResolver;

    private const string LibName = "SDL3";

    static SDL3()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static IntPtr OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName != LibName)
        {
            return IntPtr.Zero;
        }

        IntPtr nativeLibrary = IntPtr.Zero;
        DllImportResolver? resolver = SDL_DllImporterResolver;
        if (resolver != null)
        {
            nativeLibrary = resolver(libraryName, assembly, searchPath);
        }

        if (nativeLibrary != IntPtr.Zero)
        {
            return nativeLibrary;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (NativeLibrary.TryLoad("SDL3.dll", assembly, searchPath, out nativeLibrary))
            {
                return nativeLibrary;
            }
        }
        else
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (NativeLibrary.TryLoad("libSDL3.so", assembly, searchPath, out nativeLibrary))
                {
                    return nativeLibrary;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (NativeLibrary.TryLoad("libSDL3.dylib", assembly, searchPath, out nativeLibrary))
                {
                    return nativeLibrary;
                }

                if (NativeLibrary.TryLoad("/usr/local/opt/SDL3/lib/libSDL3.dylib", assembly, searchPath, out nativeLibrary))
                {
                    return nativeLibrary;
                }
            }
        }

        if (NativeLibrary.TryLoad("libSDL3", assembly, searchPath, out nativeLibrary))
        {
            return nativeLibrary;
        }

        if (NativeLibrary.TryLoad("SDL3", assembly, searchPath, out nativeLibrary))
        {
            return nativeLibrary;
        }

        return 0;
    }

    public const SDL_bool SDL_FALSE = (SDL_bool)(0);
    public const SDL_bool SDL_TRUE = (SDL_bool)(1);

    //[NativeTypeName("#define SDL_SIZE_MAX SIZE_MAX")]
    public const ulong SDL_SIZE_MAX = 0xffffffffffffffffUL;

    public static uint SDL_FOURCC(byte A, byte B, byte C, byte D)
    {
        return (uint)(A | (B << 8) | (C << 16) | (D << 24));
    }

    public static ulong SDL_SECONDS_TO_NS(ulong seconds) => seconds * SDL_NS_PER_SECOND;
    public static ulong SDL_NS_TO_SECONDS(ulong ns) => ns / SDL_NS_PER_SECOND;

    [LibraryImport(LibName, EntryPoint = "SDL_free")]
    public static partial nint SDL_free(void* mem);

    #region SDL_hints.h
    public static SDLBool SDL_SetHint(ReadOnlySpan<byte> name, bool value)
    {
        fixed (byte* pName = name)
        fixed (byte* pValue = (value ? "1"u8 : "0"u8))
            return SDL_SetHint(pName, pValue);
    }

    public static SDLBool SDL_SetHint(string name, bool value) => SDL_SetHint(name, value ? "1" : "0");
    #endregion

    #region SDL_log.h

    private static SDL_LogOutputFunction? s_logCallback;

    public static void SDL_SetLogPriority(SDL_LogCategory category, SDL_LogPriority priority)
    {
        SDL_SetLogPriority((int)category, priority);
    }

    /// <inheritdoc cref="SDL_SetLogOutputFunction(delegate* unmanaged{nint, int, SDL_LogPriority, byte*, void}, nint)" />
    public static void SDL_SetLogOutputFunction(SDL_LogOutputFunction? callback)
    {
        s_logCallback = callback;

        SDL_SetLogOutputFunction(callback != null ? &OnNativeMessageCallback : null, IntPtr.Zero);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe void OnNativeMessageCallback(nint userdata, int category, SDL_LogPriority priority, byte* messagePtr)
    {
        string? message = ConvertToManaged(messagePtr);

        s_logCallback?.Invoke((SDL_LogCategory)category, priority, message);
    }
    #endregion

    #region SDL_version.h, SDL_revision.h

    public static int SDL_VERSIONNUM(int major, int minor, int patch) =>
            ((major) * 1000000 + (minor) * 1000 + (patch));

    public static int SDL_VERSIONNUM_MAJOR(int version) => ((version) / 1000000);

    public static int SDL_VERSIONNUM_MINOR(int version) => (((version) / 1000) % 1000);

    public static int SDL_VERSIONNUM_MICRO(int version) => ((version) % 1000);

    public static readonly int SDL_VERSION = SDL_VERSIONNUM(SDL_MAJOR_VERSION, SDL_MINOR_VERSION, SDL_MICRO_VERSION);

    public static bool SDL_VERSION_ATLEAST(int X, int Y, int Z) => SDL_VERSION >= SDL_VERSIONNUM(X, Y, Z);
    #endregion

    #region SDL_video.h

    public const uint SDL_WINDOWPOS_UNDEFINED_MASK = 0x1FFF0000;
    public const uint SDL_WINDOWPOS_CENTERED_MASK = 0x2FFF0000;
    public const int SDL_WINDOWPOS_UNDEFINED = 0x1FFF0000;
    public const int SDL_WINDOWPOS_CENTERED = 0x2FFF0000;

    public static uint SDL_WINDOWPOS_UNDEFINED_DISPLAY(SDL_DisplayID X) => (SDL_WINDOWPOS_UNDEFINED_MASK | (uint)X);

    public static bool SDL_WINDOWPOS_ISUNDEFINED(uint X) => (X & 0xFFFF0000) == SDL_WINDOWPOS_UNDEFINED_MASK;

    public static uint SDL_WINDOWPOS_CENTERED_DISPLAY(SDL_DisplayID X) => (SDL_WINDOWPOS_CENTERED_MASK | (uint)X);

    public static bool SDL_WINDOWPOS_ISCENTERED(uint X)
    {
        return (X & 0xFFFF0000) == SDL_WINDOWPOS_CENTERED_MASK;
    }

    public static ReadOnlySpan<SDL_DisplayID> SDL_GetDisplays()
    {
        SDL_DisplayID* displaysPtr = SDL_GetDisplays(out int count);
        return new(displaysPtr, count);
    }

    //public static ReadOnlySpan<SDL_DisplayMode> SDL_GetFullscreenDisplayModes(SDL_DisplayID displayID)
    //{
    //    SDL_DisplayMode** displaysModePtr = SDL_GetFullscreenDisplayModes(displayID, out int count);
    //    return new(displaysModePtr, count);
    //}

    [LibraryImport(LibName)]
    public static partial SDL_DisplayID SDL_GetDisplayForPoint(in Point point);

    [LibraryImport(LibName)]
    public static partial SDL_DisplayID SDL_GetDisplayForRect(in Rectangle rect);

    public static SDLBool SDL_GL_SetAttribute(SDL_GLAttr attr, bool value) => SDL_GL_SetAttribute(attr, value ? 1 : 0);

    public static SDLBool SDL_GL_SetAttribute(SDL_GLAttr attr, SDL_GLProfile profile) => SDL_GL_SetAttribute(attr, (int)profile);
    #endregion

    [LibraryImport(LibName, EntryPoint = "SDL_PollEvent")]
    public static partial SDLBool SDL_PollEvent(out SDL_Event @event);

    public static int SDL_PeepEvents(SDL_Event[] events, SDL_EventAction action, SDL_EventType minType, SDL_EventType maxType)
    {
        fixed (SDL_Event* eventsPtr = events)
            return SDL_PeepEvents(eventsPtr, events.Length, action, minType, maxType);
    }

    public static int SDL_PeepEvents(Span<SDL_Event> events, SDL_EventAction action, SDL_EventType minType, SDL_EventType maxType)
    {
        fixed (SDL_Event* eventsPtr = events)
            return SDL_PeepEvents(eventsPtr, events.Length, action, minType, maxType);
    }

    #region SDL_vulkan.h
    public static SDLBool SDL_Vulkan_LoadLibrary() => SDL_Vulkan_LoadLibrary((byte*)null);

    public static string[] SDL_Vulkan_GetInstanceExtensions()
    {
        byte** strings = SDL_Vulkan_GetInstanceExtensions(out uint count);
        string[] names = new string[count];
        for (int i = 0; i < count; i++)
        {
            names[i] = ConvertToManaged(strings[i])!;
        }

        return names;
    }
    #endregion

    #region SDL_syswm.h
    public static readonly int SDL_SYSWM_INFO_SIZE_V1 = (16 * (sizeof(void*) >= 8 ? sizeof(void*) : sizeof(ulong)));
    public static readonly int SDL_SYSWM_CURRENT_INFO_SIZE = SDL_SYSWM_INFO_SIZE_V1;
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
    public static int SDL_SCANCODE_TO_KEYCODE(SDL_Scancode X)
    {
        return ((int)X | (int)SDLK_SCANCODE_MASK);
    }
    #endregion

    #region SDL_timer.h
    public static ulong SDL_MS_TO_NS(ulong MS) => MS * SDL_NS_PER_MS;
    public static ulong SDL_NS_TO_MS(ulong NS) => NS / SDL_NS_PER_MS;
    public static ulong SDL_US_TO_NS(ulong US) => US * SDL_NS_PER_US;
    public static ulong SDL_NS_TO_US(ulong NS) => NS / SDL_NS_PER_US;
    #endregion


    #region Marshal
    /// <summary>Converts an unmanaged string to a managed version.</summary>
    /// <param name="unmanaged">The unmanaged string to convert.</param>
    /// <returns>A managed string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ConvertToManaged(byte* unmanaged) => Utf8CustomMarshaller.ConvertToManaged(unmanaged);

    /// <summary>Converts an unmanaged string to a managed version.</summary>
    /// <param name="unmanaged">The unmanaged string to convert.</param>
    /// <returns>A managed string.</returns>
    public static string? ConvertToManaged(byte* unmanaged, int maxLength) => Utf8CustomMarshaller.ConvertToManaged(unmanaged, maxLength);
    #endregion
}
