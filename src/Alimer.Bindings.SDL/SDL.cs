// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using static SDL3.SDL3;
using static SDL3.SDL_bool;

namespace SDL3;

#region Enums
public enum SDL_bool : int
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

public static unsafe partial class SDL3
{
    private const string LibName = "SDL3";

    static SDL3()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static nint OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName.Equals(LibName) && TryResolveSDL3(assembly, searchPath, out nint nativeLibrary))
        {
            return nativeLibrary;
        }

        return 0;
    }

    private static bool TryResolveSDL3(Assembly assembly, DllImportSearchPath? searchPath, out nint nativeLibrary)
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

            if (NativeLibrary.TryLoad("SDL3", assembly, searchPath, out nativeLibrary))
            {
                return true;
            }
        }

        return false;
    }

    //[NativeTypeName("#define SDL_SIZE_MAX SIZE_MAX")]
    public const ulong SDL_SIZE_MAX = 0xffffffffffffffffUL;

    public const SDL_AudioFormat SDL_AUDIO_S16 = SDL_AudioFormat.S16le;
    public const SDL_AudioFormat SDL_AUDIO_S32 = SDL_AudioFormat.S32le;
    public const SDL_AudioFormat SDL_AUDIO_F32 = SDL_AudioFormat.F32le;

    public static uint SDL_FOURCC(byte A, byte B, byte C, byte D)
    {
        return (uint)(A | (B << 8) | (C << 16) | (D << 24));
    }

    public static ulong SDL_SECONDS_TO_NS(ulong seconds) => seconds * SDL_NS_PER_SECOND;
    public static ulong SDL_NS_TO_SECONDS(ulong ns) => ns / SDL_NS_PER_SECOND;

    [LibraryImport(LibName, EntryPoint = "SDL_free")]
    public static partial nint SDL_free(void* mem);

    #region SDL_platform.h
    public static ReadOnlySpan<byte> SDL_GetPlatformSpan()
    {
        return GetUtf8Span(SDL_GetPlatform());
    }

    public static string SDL_GetPlatformString()
    {
        return GetStringOrEmpty(SDL_GetPlatform())!;
    }
    #endregion

    #region SDL_hints.h
    public static bool SDL_SetHint(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value)
    {
        fixed (byte* pName = name)
        fixed (byte* pValue = value)
            return SDL_SetHint(pName, pValue);
    }

    public static bool SDL_SetHint(string name, string value)
    {
        fixed (byte* pName = name.GetUtf8Span())
        {
            fixed (byte* pValue = value.GetUtf8Span())
            {
                return SDL_SetHint(pName, pValue);
            }
        }
    }

    public static bool SDL_SetHintWithPriority(ReadOnlySpan<byte> name, ReadOnlySpan<byte> value, SDL_HintPriority priority)
    {
        fixed (byte* pName = name)
        {
            fixed (byte* pValue = value)
            {
                return SDL_SetHintWithPriority(pName, pValue, priority);
            }
        }
    }

    public static bool SDL_SetHintWithPriority(string name, string value, SDL_HintPriority priority)
    {
        fixed (byte* pName = name.GetUtf8Span())
        {
            fixed (byte* pValue = value.GetUtf8Span())
            {
                return SDL_SetHintWithPriority(pName, pValue, priority);
            }
        }
    }

    public static bool SDL_SetHint(ReadOnlySpan<byte> name, bool value)
    {
        fixed (byte* pName = name)
        fixed (byte* pValue = (value ? "1"u8 : "0"u8))
            return SDL_SetHint(pName, pValue);
    }

    public static bool SDL_SetHint(string name, bool value)
    {
        return SDL_SetHint(name, value ? "1" : "0");
    }
    #endregion

    #region SDL_error.h
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

    public static void SDL_SetLogPriority(SDL_LogCategory category, SDL_LogPriority priority)
    {
        SDL_SetLogPriority((int)category, priority);
    }

    public static void SDL_SetLogOutputFunction(SDL_LogOutputFunction? callback)
    {
        s_logCallback = callback;

        Internal_SDL_SetLogOutputFunction(callback != null ? &OnNativeMessageCallback : null, IntPtr.Zero);
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_SetLogOutputFunction), CallingConvention = CallingConvention.Cdecl)]
    private static extern void Internal_SDL_SetLogOutputFunction(delegate* unmanaged<nint, int, SDL_LogPriority, sbyte*, void> callback, IntPtr userdata);

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

    public static int SDL_VERSIONNUM(int major, int minor, int patch) =>
            ((major) * 1000000 + (minor) * 1000 + (patch));

    public static int SDL_VERSIONNUM_MAJOR(int version) => ((version) / 1000000);

    public static int SDL_VERSIONNUM_MINOR(int version) => (((version) / 1000) % 1000);

    public static int SDL_VERSIONNUM_MICRO(int version) => ((version) % 1000);

    public static readonly int SDL_VERSION = SDL_VERSIONNUM(SDL_MAJOR_VERSION, SDL_MINOR_VERSION, SDL_MICRO_VERSION);

    public static bool SDL_VERSION_ATLEAST(int X, int Y, int Z) => SDL_VERSION >= SDL_VERSIONNUM(X, Y, Z);

    public static string SDL_GetRevisionString() => GetStringOrEmpty(SDL_GetRevision());
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

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SDL_HitTestResult SDL_HitTest(IntPtr win, IntPtr area, IntPtr data);

    public static SDL_Window SDL_CreateWindow(ReadOnlySpan<byte> title, int width, int height, SDL_WindowFlags flags)
    {
        fixed (byte* pName = title)
        {
            return SDL_CreateWindow(pName, width, height, flags);
        }
    }

    public static SDL_Window SDL_CreateWindow(string title, int width, int height, SDL_WindowFlags flags)
    {
        fixed (byte* pName = title.GetUtf8Span())
        {
            return SDL_CreateWindow(pName, width, height, flags);
        }
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
        return SDL_SetWindowFullscreen(window, fullscreen ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
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

    //public static ReadOnlySpan<SDL_DisplayMode> SDL_GetFullscreenDisplayModes(SDL_DisplayID displayID)
    //{
    //    SDL_DisplayMode** displaysModePtr = SDL_GetFullscreenDisplayModes(displayID, out int count);
    //    return new(displaysModePtr, count);
    //}

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
            return SDL_GL_ExtensionSupported(pName);
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
    public static int SDL_SetPointerProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, nint value)
    {
        fixed (byte* pName = name)
        {
            return SDL_SetPointerProperty(properties, pName, value);
        }
    }

    public static int SDL_SetPointerProperty(SDL_PropertiesID properties, string name, nint value)
    {
        return SDL_SetPointerProperty(properties, name.GetUtf8Span(), value);
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
            return SDL_SetBooleanProperty(properties, pName, value ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
        }
    }

    public static int SDL_SetBooleanProperty(SDL_PropertiesID properties, string name, bool value)
    {
        return SDL_SetBooleanProperty(properties, name.GetUtf8Span(), value);
    }

    public static bool SDL_HasProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
        {
            return SDL_HasProperty(properties, pName);
        }
    }

    public static bool SDL_HasProperty(SDL_PropertiesID properties, string name)
    {
        return SDL_HasProperty(properties, name.GetUtf8Span());
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

    public static nint SDL_GetPointerProperty(SDL_PropertiesID properties, ReadOnlySpan<byte> name, nint defaultValue = 0)
    {
        fixed (byte* pName = name)
        {
            return SDL_GetPointerProperty(properties, pName, defaultValue);
        }
    }

    public static nint SDL_GetPointerProperty(SDL_PropertiesID properties, string name, nint defaultValue = 0)
    {
        return SDL_GetPointerProperty(properties, name.GetUtf8Span(), defaultValue);
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
            return SDL_GetBooleanProperty(properties, pName, defaultValue ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
        }
    }

    public static bool SDL_GetBooleanProperty(SDL_PropertiesID properties, string name, bool defaultValue = false)
    {
        return SDL_GetBooleanProperty(properties, name.GetUtf8Span(), defaultValue);
    }
    #endregion

    [LibraryImport(LibName, EntryPoint = "SDL_PollEvent")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SDL_PollEvent(out SDL_Event @event);


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

    #region SDL_system.h
    /* Android */
    public static string? SDL_GetAndroidInternalStoragePathString()
    {
        return GetString(SDL_GetAndroidInternalStoragePath());
    }

    public static string? SDL_GetAndroidExternalStoragePathString()
    {
        return GetString(SDL_GetAndroidExternalStoragePath());
    }

    public static unsafe bool SDL_RequestAndroidPermission(string permission, delegate* unmanaged<nint, byte*, SDL_bool, nint> cb, nint userdata)
    {
        fixed (byte* pPermission = permission.GetUtf8Span())
        {
            return SDL_RequestAndroidPermission(pPermission, cb, userdata) == (int)SDL_TRUE;
        }
    }

    public static int SDL_ShowAndroidToast(string message, int duration, int gravity, int xOffset, int yOffset)
    {
        fixed (byte* pMessage = message.GetUtf8Span())
        {
            return SDL_ShowAndroidToast(pMessage, duration, gravity, xOffset, yOffset);
        }
    }
    #endregion

    #region Marshal
    /// <inheritdoc cref="MemoryMarshal.CreateReadOnlySpan{T}(ref T, int)" />
    public static ReadOnlySpan<T> CreateReadOnlySpan<T>(scoped in T reference, int length) => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in reference), length);

    /// <summary>Returns a pointer to the element of the span at index zero.</summary>
    /// <typeparam name="T">The type of items in <paramref name="span" />.</typeparam>
    /// <param name="span">The span from which the pointer is retrieved.</param>
    /// <returns>A pointer to the item at index zero of <paramref name="span" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* GetPointerUnsafe<T>(this ReadOnlySpan<T> span)
        where T : unmanaged => (T*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span));

    /// <inheritdoc cref="Unsafe.IsNullRef{T}(ref T)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNullRef<T>(in T source) => Unsafe.IsNullRef(ref Unsafe.AsRef(in source));

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
    #endregion
}
