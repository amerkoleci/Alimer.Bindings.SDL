// Copyright (c) Amer Koleci and Contributors.
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

[Flags]
public enum SDL_WindowFlags : uint
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

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate SDL_bool SDL_X11EventHook(nint userdata, nint xevent);

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
                if (NativeLibrary.TryLoad("libSDL3.so", assembly, searchPath, out nativeLibrary))
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

    public static uint SDL_FOURCC(byte A, byte B, byte C, byte D)
    {
        return (uint)(A | (B << 8) | (C << 16) | (D << 24));
    }

    [LibraryImport(LibName)]
    private static partial void SDL_free(void* memblock);

    public static int SDL_Init(SDL_InitFlags flags) => SDL_Init((uint)flags);

    public static int SDL_InitSubSystem(SDL_InitFlags flags) => SDL_InitSubSystem((uint)flags);

    public static void SDL_QuitSubSystem(SDL_InitFlags flags) => SDL_QuitSubSystem((uint)flags);

    public static uint SDL_WasInit(SDL_InitFlags flags) => SDL_WasInit((uint)flags);

    #region SDL_platform.h
    public static ReadOnlySpan<byte> SDL_GetPlatformSpan()
    {
        return GetUtf8Span(SDL_GetPlatform());
    }

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
    public static SDL_bool SDL_SetHint(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
    {
        fixed (byte* pName = name)
        fixed (byte* pValue = value)
            return SDL_SetHint(pName, pValue);
    }

    public static SDL_bool SDL_SetHint(string name, string value)
    {
        fixed (byte* pName = name.GetUtf8Span())
        {
            fixed (byte* pValue = value.GetUtf8Span())
            {
                return SDL_SetHint(pName, pValue);
            }
        }
    }

    public static SDL_bool SDL_SetHintWithPriority(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value, SDL_HintPriority priority)
    {
        fixed (byte* pName = name)
        {
            fixed (byte* pValue = value)
            {
                return SDL_SetHintWithPriority(pName, pValue, priority);
            }
        }
    }

    public static SDL_bool SDL_SetHintWithPriority(string name, string value, SDL_HintPriority priority)
    {
        fixed (byte* pName = name.GetUtf8Span())
        {
            fixed (byte* pValue = value.GetUtf8Span())
            {
                return SDL_SetHintWithPriority(pName, pValue, priority);
            }
        }
    }

    public static SDL_bool SDL_SetHint(ReadOnlySpan<byte> name, bool value)
    {
        fixed (byte* pName = name)
        fixed (byte* pValue = (value ? "1"u8 : "0"u8))
            return SDL_SetHint(pName, pValue);
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

    public static int SDL_SetError(ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
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

    private static SDL_LogOutputFunction? s_logCallback;

    public static void SDL_LogSetPriority(SDL_LogCategory category, SDL_LogPriority priority)
    {
        SDL_LogSetPriority((int)category, priority);
    }

    public static void SDL_LogSetOutputFunction(SDL_LogOutputFunction? callback)
    {
        s_logCallback = callback;

        Internal_SDL_LogSetOutputFunction(callback != null ? &OnNativeMessageCallback : null, IntPtr.Zero);
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_LogSetOutputFunction), CallingConvention = CallingConvention.Cdecl)]
    private static extern void Internal_SDL_LogSetOutputFunction(delegate* unmanaged<nint, int, SDL_LogPriority, sbyte*, void> callback, IntPtr userdata);

    [UnmanagedCallersOnly]
    private static unsafe void OnNativeMessageCallback(nint userdata, int category, SDL_LogPriority priority, sbyte* messagePtr)
    {
        string message = new(messagePtr);

        if (s_logCallback != null)
        {
            s_logCallback((SDL_LogCategory)category, priority, message);
        }
    }
    #endregion

    #region SDL_misc.h
    public static int SDL_OpenURL(ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
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
    private static partial byte* SDL_GetRevision();

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

    public static SDL_Window SDL_CreateWindow(byte* title, int w, int h, SDL_WindowFlags flags)
    {
        return SDL_CreateWindow(title, w, h, (uint)flags);
    }

    public static SDL_Window SDL_CreateWindow(ReadOnlySpan<byte> title, int width, int height, SDL_WindowFlags flags)
    {
        fixed (byte* pName = title)
        {
            return SDL_CreateWindow(pName, width, height, (uint)flags);
        }
    }

    public static SDL_Window SDL_CreateWindow(string title, int width, int height, SDL_WindowFlags flags)
    {
        fixed (byte* pName = title.GetUtf8Span())
        {
            return SDL_CreateWindow(pName, width, height, (uint)flags);
        }
    }

    public static SDL_Window SDL_CreatePopupWindow(SDL_Window parent, int offset_x, int offset_y, int w, int h, SDL_WindowFlags flags)
    {
        return SDL_CreatePopupWindow(parent, offset_x, offset_y, w, h, (uint)flags);
    }

    public static int SDL_SetWindowTitle(SDL_Window window, ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
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

    public static string SDL_GetWindowTitleString(SDL_Window window)
    {
        return GetStringOrEmpty(SDL_GetWindowTitle(window));
    }

    public static int SDL_GL_LoadLibrary(ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
        {
            return SDL_GL_LoadLibrary(pName);
        }
    }

    public static delegate* unmanaged<void> SDL_GL_GetProcAddress(ReadOnlySpan<byte> proc)
    {
        fixed (byte* pName = proc)
        {
            return SDL_GL_GetProcAddress(pName);
        }
    }

    public static delegate* unmanaged<void> SDL_GL_GetProcAddress(string proc)
    {
        return SDL_GL_GetProcAddress(proc.GetUtf8Span());
    }

    public static delegate* unmanaged<void> SDL_EGL_GetProcAddress(ReadOnlySpan<byte> proc)
    {
        fixed (byte* pName = proc)
        {
            return SDL_EGL_GetProcAddress(pName);
        }
    }

    public static delegate* unmanaged<void> SDL_EGL_GetProcAddress(string proc)
    {
        return SDL_EGL_GetProcAddress(proc.GetUtf8Span());
    }

    public static bool SDL_GL_ExtensionSupported(ReadOnlySpan<byte> extension)
    {
        fixed (byte* pName = extension)
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
    public static partial int SDL_SetPropertyWithCleanup(SDL_PropertiesID properties, sbyte* name, nint value, delegate* unmanaged<nint, nint, void> cleanup, nint userdata);

    public static int SDL_SetProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, nint value)
    {
        fixed (byte* pName = name)
        {
            return SDL_SetProperty(properties, pName, value);
        }
    }

    public static int SDL_SetProperty(SDL_PropertiesID properties, string name, nint value)
    {
        return SDL_SetProperty(properties, name.GetUtf8Span(), value);
    }

    public static int SDL_SetStringProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
    {
        fixed (byte* pName = name)
        fixed (byte* pValue = value)
            return SDL_SetStringProperty(properties, pName, pValue);
    }

    public static int SDL_SetStringProperty(SDL_PropertiesID properties, string name, string? value)
    {
        return SDL_SetStringProperty(properties, name.GetUtf8Span(), value.GetUtf8Span());
    }

    public static int SDL_SetNumberProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, long value)
    {
        fixed (byte* pName = name)
        {
            return SDL_SetNumberProperty(properties, pName, value);
        }
    }

    public static int SDL_SetNumberProperty(SDL_PropertiesID properties, string name, long value)
    {
        return SDL_SetNumberProperty(properties, name.GetUtf8Span(), value);
    }

    public static int SDL_SetFloatProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, float value)
    {
        fixed (byte* pName = name)
        {
            return SDL_SetFloatProperty(properties, pName, value);
        }
    }

    public static int SDL_SetFloatProperty(SDL_PropertiesID properties, string name, float value)
    {
        return SDL_SetFloatProperty(properties, name.GetUtf8Span(), value);
    }

    public static int SDL_SetBooleanProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, bool value)
    {
        fixed (byte* pName = name)
        {
            return SDL_SetBooleanProperty(properties, pName, value ? SDL_TRUE : SDL_FALSE);
        }
    }

    public static int SDL_SetBooleanProperty(SDL_PropertiesID properties, string name, bool value)
    {
        return SDL_SetBooleanProperty(properties, name.GetUtf8Span(), value);
    }

    public static SDL_PropertyType SDL_GetPropertyType(SDL_PropertiesID properties, ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
        {
            return SDL_GetPropertyType(properties, pName);
        }
    }

    public static SDL_PropertyType SDL_GetPropertyType(SDL_PropertiesID properties, string name)
    {
        return SDL_GetPropertyType(properties, name.GetUtf8Span());
    }

    public static nint SDL_GetProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, nint default_value = 0)
    {
        fixed (byte* pName = name)
        {
            return SDL_GetProperty(properties, pName, default_value);
        }
    }

    public static nint SDL_GetProperty(SDL_PropertiesID properties, string name, nint default_value = 0)
    {
        return SDL_GetProperty(properties, name.GetUtf8Span(), default_value);
    }

    public static string? SDL_GetStringProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, ReadOnlySpan<byte> defaultValue)
    {
        fixed (byte* pName = name)
        fixed (byte* pDefaultValue = defaultValue)
            return GetString(SDL_GetStringProperty(properties, pName, pDefaultValue));
    }

    public static string? SDL_GetStringProperty(SDL_PropertiesID properties, string name, string default_value = "")
    {
        return SDL_GetStringProperty(properties, name.GetUtf8Span(), default_value.GetUtf8Span());
    }

    public static long SDL_GetNumberProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, long defaultValue = 0)
    {
        fixed (byte* pName = name)
        {
            return SDL_GetNumberProperty(properties, pName, defaultValue);
        }
    }

    public static long SDL_GetNumberProperty(SDL_PropertiesID properties, string name, long defaultValue = 0)
    {
        return SDL_GetNumberProperty(properties, name.GetUtf8Span(), defaultValue);
    }

    public static float SDL_GetFloatProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, float defaultValue = default)
    {
        fixed (byte* pName = name)
        {
            return SDL_GetFloatProperty(properties, pName, defaultValue);
        }
    }

    public static float SDL_GetFloatProperty(SDL_PropertiesID properties, string name, float defaultValue = 0)
    {
        return SDL_GetFloatProperty(properties, name.GetUtf8Span(), defaultValue);
    }

    public static bool SDL_GetBooleanProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, bool defaultValue = default)
    {
        fixed (byte* pName = name)
        {
            return SDL_GetBooleanProperty(properties, pName, defaultValue ? SDL_TRUE : SDL_FALSE) == SDL_TRUE;
        }
    }

    public static bool SDL_GetBooleanProperty(SDL_PropertiesID properties, string name, bool defaultValue = false)
    {
        return SDL_GetBooleanProperty(properties, name.GetUtf8Span(), defaultValue);
    }
    #endregion

    public static bool SDL_PollEvent(SDL_Event* @event) => SDL_PollEventPrivate(@event) == SDL_TRUE;

    #region SDL_vulkan.h
    public static int SDL_Vulkan_LoadLibrary()
    {
        return SDL_Vulkan_LoadLibrary((byte*)null);
    }

    public static int SDL_Vulkan_LoadLibrary(ReadOnlySpan<byte> path)
    {
        fixed (byte* pPath = path)
        {
            return SDL_Vulkan_LoadLibrary(pPath);
        }
    }

    public static nint SDL_Vulkan_LoadLibrary(string path)
    {
        return SDL_Vulkan_LoadLibrary(path.GetUtf8Span());
    }

    [LibraryImport(LibName, EntryPoint = "SDL_Vulkan_GetInstanceExtensions")]
    public static partial byte** SDL_Vulkan_GetInstanceExtensions(out int count);

    public static string[] SDL_Vulkan_GetInstanceExtensions()
    {
        byte** strings = SDL_Vulkan_GetInstanceExtensions(out int count);
        string[] names = new string[count];
        for (int i = 0; i < count; i++)
        {
            names[i] = GetString(strings[i])!;
        }

        return names;
    }

    [LibraryImport(LibName, EntryPoint = "SDL_Vulkan_CreateSurface")]
    public static partial SDL_bool SDL_Vulkan_CreateSurface(SDL_Window window, nint instance, nint* allocator, ulong* surface);
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

    public static string SDL_GetKeyNameString(SDL_KeyCode key)
    {
        return GetStringOrEmpty(SDL_GetKeyName(key));
    }


    public static SDL_KeyCode SDL_GetKeyFromName(ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
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
    [LibraryImport(LibName)]
    public static partial nint SDL_AndroidGetJNIEnv();

    /* IntPtr refers to a jobject */
    [LibraryImport(LibName)]
    public static partial nint SDL_AndroidGetActivity();

    [LibraryImport(LibName)]
    public static partial SDL_bool SDL_IsAndroidTV();

    [LibraryImport(LibName)]
    public static partial SDL_bool SDL_IsChromebook();

    [LibraryImport(LibName)]
    public static partial SDL_bool SDL_IsDeXMode();

    [LibraryImport(LibName)]
    public static partial void SDL_AndroidBackButton();

    [LibraryImport(LibName, EntryPoint = "SDL_AndroidGetInternalStoragePath")]
    private static partial byte* INTERNAL_SDL_AndroidGetInternalStoragePath();

    public static string? SDL_AndroidGetInternalStoragePath()
    {
        return GetString(
            INTERNAL_SDL_AndroidGetInternalStoragePath()
        );
    }

    [LibraryImport(LibName)]
    public static partial int SDL_AndroidGetExternalStorageState();

    [LibraryImport(LibName, EntryPoint = "SDL_AndroidGetExternalStoragePath")]
    private static partial byte* INTERNAL_SDL_AndroidGetExternalStoragePath();

    public static string? SDL_AndroidGetExternalStoragePath()
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
    /// <inheritdoc cref="Unsafe.AsPointer{T}(ref T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* AsPointer<T>(ref T source) where T : unmanaged => (T*)Unsafe.AsPointer(ref source);

    /// <inheritdoc cref="Unsafe.As{TFrom, TTo}(ref TFrom)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly TTo AsReadOnly<TFrom, TTo>(in TFrom source) => ref Unsafe.As<TFrom, TTo>(ref AsRef(in source));

    /// <inheritdoc cref="Unsafe.AsRef{T}(in T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T AsRef<T>(in T source) => ref Unsafe.AsRef(in source);

    /// <inheritdoc cref="MemoryMarshal.CreateReadOnlySpan{T}(ref T, int)" />
    public static ReadOnlySpan<T> CreateReadOnlySpan<T>(scoped in T reference, int length) => MemoryMarshal.CreateReadOnlySpan(ref AsRef(in reference), length);

    /// <summary>Returns a pointer to the element of the span at index zero.</summary>
    /// <typeparam name="T">The type of items in <paramref name="span" />.</typeparam>
    /// <param name="span">The span from which the pointer is retrieved.</param>
    /// <returns>A pointer to the item at index zero of <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPointerUnsafe<T>(this Span<T> span)
        where T : unmanaged => (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

    /// <summary>Returns a pointer to the element of the span at index zero.</summary>
    /// <typeparam name="T">The type of items in <paramref name="span" />.</typeparam>
    /// <param name="span">The span from which the pointer is retrieved.</param>
    /// <returns>A pointer to the item at index zero of <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPointerUnsafe<T>(this ReadOnlySpan<T> span)
        where T : unmanaged => (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

    /// <summary>Returns a pointer to the element of the span at index zero.</summary>
    /// <typeparam name="T">The type of items in <paramref name="span" />.</typeparam>
    /// <param name="span">The span from which the pointer is retrieved.</param>
    /// <returns>A pointer to the item at index zero of <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPointer<T>(this ReadOnlySpan<T> span)
        where T : unmanaged => AsPointer(ref AsRef(in span.GetReference()));

    /// <inheritdoc cref="MemoryMarshal.GetReference{T}(ReadOnlySpan{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T GetReference<T>(this ReadOnlySpan<T> span) => ref MemoryMarshal.GetReference(span);

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
    public static ReadOnlySpan<byte> GetUtf8Span(this string? source)
    {
        ReadOnlySpan<byte> result;

        if (source is not null)
        {
            int maxLength = Encoding.UTF8.GetMaxByteCount(source.Length);
            byte[] bytes = new byte[maxLength + 1];

            int length = Encoding.UTF8.GetBytes(source, bytes);
            result = bytes.AsSpan(0, length);
        }
        else
        {
            result = null;
        }

        return result;
    }

    /// <summary>Gets a span for a null-terminated UTF8 character sequence.</summary>
    /// <param name="source">The pointer to a null-terminated UTF8 character sequence.</param>
    /// <param name="maxLength">The maximum length of <paramref name="source" /> or <c>-1</c> if the maximum length is unknown.</param>
    /// <returns>A span that starts at <paramref name="source" /> and extends to <paramref name="maxLength" /> or the first null character, whichever comes first.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> GetUtf8Span(byte* source, int maxLength = -1)
        => (source != null) ? GetUtf8Span(in source[0], maxLength) : null;

    /// <summary>Gets a span for a null-terminated UTF8 character sequence.</summary>
    /// <param name="source">The reference to a null-terminated UTF8 character sequence.</param>
    /// <param name="maxLength">The maximum length of <paramref name="source" /> or <c>-1</c> if the maximum length is unknown.</param>
    /// <returns>A span that starts at <paramref name="source" /> and extends to <paramref name="maxLength" /> or the first null character, whichever comes first.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> GetUtf8Span(ref readonly byte source, int maxLength = -1)
    {
        ReadOnlySpan<byte> result;

        if (!IsNullRef(in source))
        {
            if (maxLength < 0)
            {
                maxLength = int.MaxValue;
            }

            result = CreateReadOnlySpan(in source, maxLength);
            var length = result.IndexOf((byte)'\0');

            if (length >= 0)
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
    public static string? GetString(this ReadOnlySpan<byte> span) => span.GetPointerUnsafe() != null ? Encoding.UTF8.GetString(span) : null;

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetStringOrEmpty(this ReadOnlySpan<byte> span) => span.GetPointerUnsafe() != null ? Encoding.UTF8.GetString(span) : string.Empty;


    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetString(this ReadOnlySpan<char> span) => span.GetPointerUnsafe() != null ? new string(span) : null;

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetString(byte* source, int maxLength = -1)
    {
        return GetUtf8Span(source, maxLength).GetString();
    }

    /// <summary>Gets a string for a given span.</summary>
    /// <param name="span">The span for which to create the string.</param>
    /// <returns>A string created from <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetStringOrEmpty(byte* source, int maxLength = -1)
    {
        return GetUtf8Span(source, maxLength).GetStringOrEmpty();
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
