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
using static SDL.SDL_bool;

namespace SDL;

#region Enums
public enum SDL_bool
{
    SDL_FALSE = 0,
    SDL_TRUE = 1
}

public enum SDL_LogCategory
{
    SDL_LOG_CATEGORY_APPLICATION,
    SDL_LOG_CATEGORY_ERROR,
    SDL_LOG_CATEGORY_ASSERT,
    SDL_LOG_CATEGORY_SYSTEM,
    SDL_LOG_CATEGORY_AUDIO,
    SDL_LOG_CATEGORY_VIDEO,
    SDL_LOG_CATEGORY_RENDER,
    SDL_LOG_CATEGORY_INPUT,
    SDL_LOG_CATEGORY_TEST,

    /* Reserved for future SDL library use */
    SDL_LOG_CATEGORY_RESERVED1,
    SDL_LOG_CATEGORY_RESERVED2,
    SDL_LOG_CATEGORY_RESERVED3,
    SDL_LOG_CATEGORY_RESERVED4,
    SDL_LOG_CATEGORY_RESERVED5,
    SDL_LOG_CATEGORY_RESERVED6,
    SDL_LOG_CATEGORY_RESERVED7,
    SDL_LOG_CATEGORY_RESERVED8,
    SDL_LOG_CATEGORY_RESERVED9,
    SDL_LOG_CATEGORY_RESERVED10,

    /* Beyond this point is reserved for application use, e.g.
    enum {
        MYAPP_CATEGORY_AWESOME1 = SDL_LOG_CATEGORY_CUSTOM,
        MYAPP_CATEGORY_AWESOME2,
        MYAPP_CATEGORY_AWESOME3,
        ...
    };
    */
    SDL_LOG_CATEGORY_CUSTOM
}

public enum SDL_LogPriority
{
    SDL_LOG_PRIORITY_VERBOSE = 1,
    SDL_LOG_PRIORITY_DEBUG,
    SDL_LOG_PRIORITY_INFO,
    SDL_LOG_PRIORITY_WARN,
    SDL_LOG_PRIORITY_ERROR,
    SDL_LOG_PRIORITY_CRITICAL,
    SDL_NUM_LOG_PRIORITIES
}

public enum SDL_InitFlags : uint
{
    SDL_INIT_TIMER = 0x00000001,
    SDL_INIT_AUDIO = 0x00000010,
    SDL_INIT_VIDEO = 0x00000020,  /**< `SDL_INIT_VIDEO` implies `SDL_INIT_EVENTS` */
    SDL_INIT_JOYSTICK = 0x00000200,  /**< `SDL_INIT_JOYSTICK` implies `SDL_INIT_EVENTS` */
    SDL_INIT_HAPTIC = 0x00001000,
    SDL_INIT_GAMEPAD = 0x00002000,  /**< `SDL_INIT_GAMEPAD` implies `SDL_INIT_JOYSTICK` */
    SDL_INIT_EVENTS = 0x00004000,
    SDL_INIT_SENSOR = 0x00008000,

    SDL_INIT_EVERYTHING = (SDL_INIT_TIMER | SDL_INIT_AUDIO | SDL_INIT_VIDEO |
        SDL_INIT_EVENTS | SDL_INIT_JOYSTICK | SDL_INIT_HAPTIC |
        SDL_INIT_GAMEPAD | SDL_INIT_SENSOR)
}

public enum SDL_HintPriority
{
    SDL_HINT_DEFAULT,
    SDL_HINT_NORMAL,
    SDL_HINT_OVERRIDE
}

public enum SDL_EventType : uint
{
    SDL_FIRSTEVENT = 0,

    /// <summary>
    /// User-requested quit
    /// </summary>
    SDL_QUIT = 0x100,

    /* iOS/Android/WinRT app events */

    /// <summary>
    /// The application is being terminated by the OS
    /// </summary>
    /// <remarks>
    /// Called on iOS in applicationWillTerminate()
    /// Called on Android in onDestroy()
    /// </remarks>
    SDL_EVENT_TERMINATING,
    /// <summary>
    /// The application is low on memory, free memory if possible.
    /// </summary>
    /// <remarks>
    /// Called on iOS in applicationDidReceiveMemoryWarning()
    /// Called on Android in onLowMemory()
    /// </remarks>
    SDL_EVENT_LOW_MEMORY,
    SDL_EVENT_WILL_ENTER_BACKGROUND,
    SDL_EVENT_DID_ENTER_BACKGROUND,
    SDL_EVENT_WILL_ENTER_FOREGROUND,
    SDL_EVENT_DID_ENTER_FOREGROUND,

    /// <summary>
    /// The user's locale preferences have changed.
    /// </summary>
    SDL_EVENT_LOCALE_CHANGED,

    /* 0x150 was SDL_DISPLAYEVENT, reserve the number for sdl2-compat */
    SDL_EVENT_DISPLAY_ORIENTATION = 0x151, /**< Display orientation has changed to data1 */
    SDL_EVENT_DISPLAY_CONNECTED,           /**< Display has been added to the system */
    SDL_EVENT_DISPLAY_DISCONNECTED,        /**< Display has been removed from the system */
    SDL_EVENT_DISPLAY_MOVED,               /**< Display has changed position */
    SDL_EVENT_DISPLAY_SCALE_CHANGED,       /**< Display has changed desktop display scale */
    SDL_EVENT_DISPLAY_FIRST = SDL_EVENT_DISPLAY_ORIENTATION,
    SDL_EVENT_DISPLAY_LAST = SDL_EVENT_DISPLAY_SCALE_CHANGED,

    /* Window events */
    /* 0x200 was SDL_WINDOWEVENT, reserve the number for sdl2-compat */
    SDL_EVENT_SYSWM = 0x201,        /**< System specific event */
    SDL_EVENT_WINDOW_SHOWN,             /**< Window has been shown */
    SDL_EVENT_WINDOW_HIDDEN,            /**< Window has been hidden */
    SDL_EVENT_WINDOW_EXPOSED,           /**< Window has been exposed and should be redrawn */
    SDL_EVENT_WINDOW_MOVED,             /**< Window has been moved to data1, data2 */
    SDL_EVENT_WINDOW_RESIZED,           /**< Window has been resized to data1xdata2 */
    SDL_EVENT_WINDOW_PIXEL_SIZE_CHANGED,/**< The pixel size of the window has changed to data1xdata2 */
    SDL_EVENT_WINDOW_MINIMIZED,         /**< Window has been minimized */
    SDL_EVENT_WINDOW_MAXIMIZED,         /**< Window has been maximized */
    SDL_EVENT_WINDOW_RESTORED,          /**< Window has been restored to normal size and position */
    SDL_EVENT_WINDOW_MOUSE_ENTER,       /**< Window has gained mouse focus */
    SDL_EVENT_WINDOW_MOUSE_LEAVE,       /**< Window has lost mouse focus */
    SDL_EVENT_WINDOW_FOCUS_GAINED,      /**< Window has gained keyboard focus */
    SDL_EVENT_WINDOW_FOCUS_LOST,        /**< Window has lost keyboard focus */
    SDL_EVENT_WINDOW_CLOSE_REQUESTED,   /**< The window manager requests that the window be closed */
    SDL_EVENT_WINDOW_TAKE_FOCUS,        /**< Window is being offered a focus (should SetWindowInputFocus() on itself or a subwindow, or ignore) */
    SDL_EVENT_WINDOW_HIT_TEST,          /**< Window had a hit test that wasn't SDL_HITTEST_NORMAL */
    SDL_EVENT_WINDOW_ICCPROF_CHANGED,   /**< The ICC profile of the window's display has changed */
    SDL_EVENT_WINDOW_DISPLAY_CHANGED,   /**< Window has been moved to display data1 */
    SDL_EVENT_WINDOW_FIRST = SDL_EVENT_WINDOW_SHOWN,
    SDL_EVENT_WINDOW_LAST = SDL_EVENT_WINDOW_DISPLAY_CHANGED,

    /* Keyboard events */
    SDL_EVENT_KEY_DOWN = 0x300, /**< Key pressed */
    SDL_EVENT_KEY_UP,                  /**< Key released */
    SDL_EVENT_TEXT_EDITING,            /**< Keyboard text editing (composition) */
    SDL_EVENT_TEXT_INPUT,              /**< Keyboard text input */
    SDL_EVENT_KEYMAP_CHANGED,          /**< Keymap changed due to a system event such as an
                                            input language or keyboard layout change. */
    SDL_EVENT_TEXT_EDITING_EXT,        /**< Extended keyboard text editing (composition) */

    /* Mouse events */
    SDL_EVENT_MOUSE_MOTION = 0x400, /**< Mouse moved */
    SDL_EVENT_MOUSE_BUTTON_DOWN,       /**< Mouse button pressed */
    SDL_EVENT_MOUSE_BUTTON_UP,         /**< Mouse button released */
    SDL_EVENT_MOUSE_WHEEL,             /**< Mouse wheel motion */

    /* Joystick events */
    SDL_EVENT_JOYSTICK_AXIS_MOTION = 0x600, /**< Joystick axis motion */
    SDL_EVENT_JOYSTICK_HAT_MOTION = 0x602, /**< Joystick hat position change */
    SDL_EVENT_JOYSTICK_BUTTON_DOWN,          /**< Joystick button pressed */
    SDL_EVENT_JOYSTICK_BUTTON_UP,            /**< Joystick button released */
    SDL_EVENT_JOYSTICK_ADDED,         /**< A new joystick has been inserted into the system */
    SDL_EVENT_JOYSTICK_REMOVED,       /**< An opened joystick has been removed */
    SDL_EVENT_JOYSTICK_BATTERY_UPDATED,      /**< Joystick battery level change */

    /* Gamepad events */
    SDL_EVENT_GAMEPAD_AXIS_MOTION = 0x650, /**< Gamepad axis motion */
    SDL_EVENT_GAMEPAD_BUTTON_DOWN,          /**< Gamepad button pressed */
    SDL_EVENT_GAMEPAD_BUTTON_UP,            /**< Gamepad button released */
    SDL_EVENT_GAMEPAD_ADDED,               /**< A new gamepad has been inserted into the system */
    SDL_EVENT_GAMEPAD_REMOVED,             /**< An opened gamepad has been removed */
    SDL_EVENT_GAMEPAD_REMAPPED,            /**< The gamepad mapping was updated */
    SDL_EVENT_GAMEPAD_TOUCHPAD_DOWN,        /**< Gamepad touchpad was touched */
    SDL_EVENT_GAMEPAD_TOUCHPAD_MOTION,      /**< Gamepad touchpad finger was moved */
    SDL_EVENT_GAMEPAD_TOUCHPAD_UP,          /**< Gamepad touchpad finger was lifted */
    SDL_EVENT_GAMEPAD_SENSOR_UPDATE,        /**< Gamepad sensor was updated */

    /* Touch events */
    SDL_EVENT_FINGER_DOWN = 0x700,
    SDL_EVENT_FINGER_UP,
    SDL_EVENT_FINGER_MOTION,

    /* 0x800, 0x801, and 0x802 were the Gesture events from SDL2. Do not reuse these values! sdl2-compat needs them! */

    /* Clipboard events */
    SDL_EVENT_CLIPBOARD_UPDATE = 0x900, /**< The clipboard or primary selection changed */

    /* Drag and drop events */
    SDL_EVENT_DROP_FILE = 0x1000, /**< The system requests a file open */
    SDL_EVENT_DROP_TEXT,                 /**< text/plain drag-and-drop event */
    SDL_EVENT_DROP_BEGIN,                /**< A new set of drops is beginning (NULL filename) */
    SDL_EVENT_DROP_COMPLETE,             /**< Current set of drops is now complete (NULL filename) */
    SDL_EVENT_DROP_POSITION,             /**< Position while moving over the window */

    /* Audio hotplug events */
    SDL_EVENT_AUDIO_DEVICE_ADDED = 0x1100, /**< A new audio device is available */
    SDL_EVENT_AUDIO_DEVICE_REMOVED,        /**< An audio device has been removed. */

    /* Sensor events */
    SDL_EVENT_SENSOR_UPDATE = 0x1200,     /**< A sensor was updated */

    /* Render events */
    SDL_EVENT_RENDER_TARGETS_RESET = 0x2000, /**< The render targets have been reset and their contents need to be updated */
    SDL_EVENT_RENDER_DEVICE_RESET, /**< The device has been reset and all textures need to be recreated */

    /* Internal events */
    SDL_EVENT_POLL_SENTINEL = 0x7F00, /**< Signals the end of an event poll cycle */

    /** Events ::SDL_EVENT_USER through ::SDL_EVENT_LAST are for your use,
     *  and should be allocated with SDL_RegisterEvents()
     */
    SDL_EVENT_USER = 0x8000,

    /**
     *  This last event is only for bounding internal arrays
     */
    SDL_EVENT_LAST = 0xFFFF
}

public enum SDL_FlashOperation
{
    SDL_FLASH_CANCEL,
    SDL_FLASH_BRIEFLY,
    SDL_FLASH_UNTIL_FOCUSED
}


/* Only available in 2.0.4 or higher. */
public enum SDL_HitTestResult
{
    /// <summary>
    /// Region is normal. No special properties.
    /// </summary>
    SDL_HITTEST_NORMAL,
    /// <summary>
    /// Region can drag entire window.
    /// </summary>
    SDL_HITTEST_DRAGGABLE,
    SDL_HITTEST_RESIZE_TOPLEFT,
    SDL_HITTEST_RESIZE_TOP,
    SDL_HITTEST_RESIZE_TOPRIGHT,
    SDL_HITTEST_RESIZE_RIGHT,
    SDL_HITTEST_RESIZE_BOTTOMRIGHT,
    SDL_HITTEST_RESIZE_BOTTOM,
    SDL_HITTEST_RESIZE_BOTTOMLEFT,
    SDL_HITTEST_RESIZE_LEFT
}

public enum SDL_GLattr
{
    SDL_GL_RED_SIZE,
    SDL_GL_GREEN_SIZE,
    SDL_GL_BLUE_SIZE,
    SDL_GL_ALPHA_SIZE,
    SDL_GL_BUFFER_SIZE,
    SDL_GL_DOUBLEBUFFER,
    SDL_GL_DEPTH_SIZE,
    SDL_GL_STENCIL_SIZE,
    SDL_GL_ACCUM_RED_SIZE,
    SDL_GL_ACCUM_GREEN_SIZE,
    SDL_GL_ACCUM_BLUE_SIZE,
    SDL_GL_ACCUM_ALPHA_SIZE,
    SDL_GL_STEREO,
    SDL_GL_MULTISAMPLEBUFFERS,
    SDL_GL_MULTISAMPLESAMPLES,
    SDL_GL_ACCELERATED_VISUAL,
    SDL_GL_RETAINED_BACKING,
    SDL_GL_CONTEXT_MAJOR_VERSION,
    SDL_GL_CONTEXT_MINOR_VERSION,
    SDL_GL_CONTEXT_FLAGS,
    SDL_GL_CONTEXT_PROFILE_MASK,
    SDL_GL_SHARE_WITH_CURRENT_CONTEXT,
    SDL_GL_FRAMEBUFFER_SRGB_CAPABLE,
    SDL_GL_CONTEXT_RELEASE_BEHAVIOR,
    SDL_GL_CONTEXT_RESET_NOTIFICATION,
    SDL_GL_CONTEXT_NO_ERROR,
    SDL_GL_FLOATBUFFERS,
    SDL_GL_EGL_PLATFORM
}

[Flags]
public enum SDL_GLprofile
{
    SDL_GL_CONTEXT_PROFILE_CORE = 0x0001,
    SDL_GL_CONTEXT_PROFILE_COMPATIBILITY = 0x0002,
    SDL_GL_CONTEXT_PROFILE_ES = 0x0004 /**< GLX_CONTEXT_ES2_PROFILE_BIT_EXT */
}

[Flags]
public enum SDL_GLcontext
{
    SDL_GL_CONTEXT_DEBUG_FLAG = 0x0001,
    SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG = 0x0002,
    SDL_GL_CONTEXT_ROBUST_ACCESS_FLAG = 0x0004,
    SDL_GL_CONTEXT_RESET_ISOLATION_FLAG = 0x0008
}

[Flags]
public enum SDL_GLcontextReleaseFlag
{
    SDL_GL_CONTEXT_RELEASE_BEHAVIOR_NONE = 0x0000,
    SDL_GL_CONTEXT_RELEASE_BEHAVIOR_FLUSH = 0x0001
}

[Flags]
public enum SDL_GLContextResetNotification
{
    SDL_GL_CONTEXT_RESET_NO_NOTIFICATION = 0x0000,
    SDL_GL_CONTEXT_RESET_LOSE_CONTEXT = 0x0001
}

public enum SDL_DisplayEventID : byte
{
    SDL_DISPLAYEVENT_NONE,
    SDL_DISPLAYEVENT_ORIENTATION,
    SDL_DISPLAYEVENT_CONNECTED, /* Requires >= 2.0.14 */
    SDL_DISPLAYEVENT_DISCONNECTED   /* Requires >= 2.0.14 */
}

public enum SDL_DisplayOrientation
{
    SDL_ORIENTATION_UNKNOWN,
    SDL_ORIENTATION_LANDSCAPE,
    SDL_ORIENTATION_LANDSCAPE_FLIPPED,
    SDL_ORIENTATION_PORTRAIT,
    SDL_ORIENTATION_PORTRAIT_FLIPPED
}

public enum SDL_MouseWheelDirection : uint
{
    SDL_MOUSEWHEEL_NORMAL,
    SDL_MOUSEWHEEL_FLIPPED
}

public enum SDL_SYSWM_TYPE
{
    SDL_SYSWM_UNKNOWN,
    SDL_SYSWM_ANDROID,
    SDL_SYSWM_COCOA,
    SDL_SYSWM_HAIKU,
    SDL_SYSWM_KMSDRM,
    SDL_SYSWM_RISCOS,
    SDL_SYSWM_UIKIT,
    SDL_SYSWM_VIVANTE,
    SDL_SYSWM_WAYLAND,
    SDL_SYSWM_WINDOWS,
    SDL_SYSWM_WINRT,
    SDL_SYSWM_X11
}

[Flags]
public enum SDL_BlendMode
{
    SDL_BLENDMODE_NONE = 0x00000000,
    SDL_BLENDMODE_BLEND = 0x00000001,
    SDL_BLENDMODE_ADD = 0x00000002,
    SDL_BLENDMODE_MOD = 0x00000004,
    SDL_BLENDMODE_MUL = 0x00000008, /* >= 2.0.11 */
    SDL_BLENDMODE_INVALID = 0x7FFFFFFF
}

public enum SDL_BlendOperation
{
    SDL_BLENDOPERATION_ADD = 0x1,
    SDL_BLENDOPERATION_SUBTRACT = 0x2,
    SDL_BLENDOPERATION_REV_SUBTRACT = 0x3,
    SDL_BLENDOPERATION_MINIMUM = 0x4,
    SDL_BLENDOPERATION_MAXIMUM = 0x5
}

public enum SDL_BlendFactor
{
    SDL_BLENDFACTOR_ZERO = 0x1,
    SDL_BLENDFACTOR_ONE = 0x2,
    SDL_BLENDFACTOR_SRC_COLOR = 0x3,
    SDL_BLENDFACTOR_ONE_MINUS_SRC_COLOR = 0x4,
    SDL_BLENDFACTOR_SRC_ALPHA = 0x5,
    SDL_BLENDFACTOR_ONE_MINUS_SRC_ALPHA = 0x6,
    SDL_BLENDFACTOR_DST_COLOR = 0x7,
    SDL_BLENDFACTOR_ONE_MINUS_DST_COLOR = 0x8,
    SDL_BLENDFACTOR_DST_ALPHA = 0x9,
    SDL_BLENDFACTOR_ONE_MINUS_DST_ALPHA = 0xA
}

#endregion

#region Structs
[StructLayout(LayoutKind.Sequential)]
public struct SDL_DisplayMode
{
    public SDL_DisplayID displayID;
    public uint format;
    public int pixel_w;
    public int pixel_h;
    public int screen_w;
    public int screen_h;
    public float display_scale;
    public float refresh_rate;
    public IntPtr driverdata;
}
#endregion

public delegate void SDL_LogOutputFunction(SDL_LogCategory category, SDL_LogPriority priority, string description);

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

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_free(void* memblock);

    #region SDL.h

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_Init(SDL_InitFlags flags);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_InitSubSystem(SDL_InitFlags flags);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_Quit();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_QuitSubSystem(SDL_InitFlags flags);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint SDL_WasInit(SDL_InitFlags flags);
    #endregion

    #region SDL_platform.h
    [DllImport(LibName, EntryPoint = nameof(SDL_GetPlatform), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetPlatform();

    public static string SDL_GetPlatform()
    {
        return GetString(INTERNAL_SDL_GetPlatform());
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

    public const string SDL_HINT_FRAMEBUFFER_ACCELERATION =
        "SDL_FRAMEBUFFER_ACCELERATION";
    public const string SDL_HINT_RENDER_DRIVER =
        "SDL_RENDER_DRIVER";
    public const string SDL_HINT_RENDER_OPENGL_SHADERS =
        "SDL_RENDER_OPENGL_SHADERS";
    public const string SDL_HINT_RENDER_DIRECT3D_THREADSAFE =
        "SDL_RENDER_DIRECT3D_THREADSAFE";
    public const string SDL_HINT_RENDER_VSYNC =
        "SDL_RENDER_VSYNC";
    public const string SDL_HINT_VIDEO_X11_XRANDR =
        "SDL_VIDEO_X11_XRANDR";
    public const string SDL_HINT_GRAB_KEYBOARD =
        "SDL_GRAB_KEYBOARD";
    public const string SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS =
        "SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS";
    public const string SDL_HINT_ORIENTATIONS =
        "SDL_IOS_ORIENTATIONS";
    public const string SDL_HINT_XINPUT_ENABLED =
        "SDL_XINPUT_ENABLED";

    public const string SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS = "SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS";
    public const string SDL_HINT_ALLOW_TOPMOST =
        "SDL_ALLOW_TOPMOST";
    public const string SDL_HINT_TIMER_RESOLUTION =
        "SDL_TIMER_RESOLUTION";
    public const string SDL_HINT_RENDER_SCALE_QUALITY =
        "SDL_RENDER_SCALE_QUALITY";

    /* Only available in SDL 2.0.2 or higher. */
    public const string SDL_HINT_MAC_CTRL_CLICK_EMULATE_RIGHT_CLICK =
        "SDL_MAC_CTRL_CLICK_EMULATE_RIGHT_CLICK";
    public const string SDL_HINT_VIDEO_WIN_D3DCOMPILER =
        "SDL_VIDEO_WIN_D3DCOMPILER";
    public const string SDL_HINT_MOUSE_RELATIVE_MODE_WARP =
        "SDL_MOUSE_RELATIVE_MODE_WARP";
    public const string SDL_HINT_VIDEO_WINDOW_SHARE_PIXEL_FORMAT =
        "SDL_VIDEO_WINDOW_SHARE_PIXEL_FORMAT";
    public const string SDL_HINT_VIDEO_ALLOW_SCREENSAVER =
        "SDL_VIDEO_ALLOW_SCREENSAVER";
    public const string SDL_HINT_ACCELEROMETER_AS_JOYSTICK =
        "SDL_ACCELEROMETER_AS_JOYSTICK";
    public const string SDL_HINT_VIDEO_MAC_FULLSCREEN_SPACES =
        "SDL_VIDEO_MAC_FULLSCREEN_SPACES";

    /* Only available in SDL 2.0.3 or higher. */
    public const string SDL_HINT_WINRT_PRIVACY_POLICY_URL =
        "SDL_WINRT_PRIVACY_POLICY_URL";
    public const string SDL_HINT_WINRT_PRIVACY_POLICY_LABEL =
        "SDL_WINRT_PRIVACY_POLICY_LABEL";
    public const string SDL_HINT_WINRT_HANDLE_BACK_BUTTON =
        "SDL_WINRT_HANDLE_BACK_BUTTON";

    /* Only available in SDL 2.0.4 or higher. */
    public const string SDL_HINT_NO_SIGNAL_HANDLERS =
        "SDL_NO_SIGNAL_HANDLERS";
    public const string SDL_HINT_IME_INTERNAL_EDITING =
        "SDL_IME_INTERNAL_EDITING";
    public const string SDL_HINT_ANDROID_SEPARATE_MOUSE_AND_TOUCH =
        "SDL_ANDROID_SEPARATE_MOUSE_AND_TOUCH";
    public const string SDL_HINT_EMSCRIPTEN_KEYBOARD_ELEMENT =
        "SDL_EMSCRIPTEN_KEYBOARD_ELEMENT";
    public const string SDL_HINT_THREAD_STACK_SIZE =
        "SDL_THREAD_STACK_SIZE";
    public const string SDL_HINT_WINDOW_FRAME_USABLE_WHILE_CURSOR_HIDDEN =
        "SDL_WINDOW_FRAME_USABLE_WHILE_CURSOR_HIDDEN";
    public const string SDL_HINT_WINDOWS_ENABLE_MESSAGELOOP =
        "SDL_WINDOWS_ENABLE_MESSAGELOOP";
    public const string SDL_HINT_WINDOWS_NO_CLOSE_ON_ALT_F4 =
        "SDL_WINDOWS_NO_CLOSE_ON_ALT_F4";
    public const string SDL_HINT_XINPUT_USE_OLD_JOYSTICK_MAPPING =
        "SDL_XINPUT_USE_OLD_JOYSTICK_MAPPING";
    public const string SDL_HINT_MAC_BACKGROUND_APP =
        "SDL_MAC_BACKGROUND_APP";
    public const string SDL_HINT_VIDEO_X11_NET_WM_PING =
        "SDL_VIDEO_X11_NET_WM_PING";
    public const string SDL_HINT_ANDROID_APK_EXPANSION_MAIN_FILE_VERSION =
        "SDL_ANDROID_APK_EXPANSION_MAIN_FILE_VERSION";
    public const string SDL_HINT_ANDROID_APK_EXPANSION_PATCH_FILE_VERSION =
        "SDL_ANDROID_APK_EXPANSION_PATCH_FILE_VERSION";

    /* Only available in 2.0.5 or higher. */
    public const string SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH =
        "SDL_MOUSE_FOCUS_CLICKTHROUGH";
    public const string SDL_HINT_BMP_SAVE_LEGACY_FORMAT =
        "SDL_BMP_SAVE_LEGACY_FORMAT";
    public const string SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING =
        "SDL_WINDOWS_DISABLE_THREAD_NAMING";
    public const string SDL_HINT_APPLE_TV_REMOTE_ALLOW_ROTATION =
        "SDL_APPLE_TV_REMOTE_ALLOW_ROTATION";

    /* Only available in 2.0.6 or higher. */
    public const string SDL_HINT_AUDIO_RESAMPLING_MODE =
        "SDL_AUDIO_RESAMPLING_MODE";
    public const string SDL_HINT_MOUSE_NORMAL_SPEED_SCALE =
        "SDL_MOUSE_NORMAL_SPEED_SCALE";
    public const string SDL_HINT_MOUSE_RELATIVE_SPEED_SCALE =
        "SDL_MOUSE_RELATIVE_SPEED_SCALE";
    public const string SDL_HINT_TOUCH_MOUSE_EVENTS =
        "SDL_TOUCH_MOUSE_EVENTS";
    public const string SDL_HINT_WINDOWS_INTRESOURCE_ICON =
        "SDL_WINDOWS_INTRESOURCE_ICON";
    public const string SDL_HINT_WINDOWS_INTRESOURCE_ICON_SMALL =
        "SDL_WINDOWS_INTRESOURCE_ICON_SMALL";

    /* Only available in 2.0.8 or higher. */
    public const string SDL_HINT_IOS_HIDE_HOME_INDICATOR =
        "SDL_IOS_HIDE_HOME_INDICATOR";
    public const string SDL_HINT_TV_REMOTE_AS_JOYSTICK =
        "SDL_TV_REMOTE_AS_JOYSTICK";
    public const string SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR =
        "SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR";

    /* Only available in 2.0.9 or higher. */
    public const string SDL_HINT_MOUSE_DOUBLE_CLICK_TIME =
        "SDL_MOUSE_DOUBLE_CLICK_TIME";
    public const string SDL_HINT_MOUSE_DOUBLE_CLICK_RADIUS =
        "SDL_MOUSE_DOUBLE_CLICK_RADIUS";
    public const string SDL_HINT_JOYSTICK_HIDAPI =
        "SDL_JOYSTICK_HIDAPI";
    public const string SDL_HINT_JOYSTICK_HIDAPI_PS4 =
        "SDL_JOYSTICK_HIDAPI_PS4";
    public const string SDL_HINT_JOYSTICK_HIDAPI_PS4_RUMBLE =
        "SDL_JOYSTICK_HIDAPI_PS4_RUMBLE";
    public const string SDL_HINT_JOYSTICK_HIDAPI_STEAM =
        "SDL_JOYSTICK_HIDAPI_STEAM";
    public const string SDL_HINT_JOYSTICK_HIDAPI_SWITCH =
        "SDL_JOYSTICK_HIDAPI_SWITCH";
    public const string SDL_HINT_JOYSTICK_HIDAPI_XBOX =
        "SDL_JOYSTICK_HIDAPI_XBOX";
    public const string SDL_HINT_ENABLE_STEAM_CONTROLLERS =
        "SDL_ENABLE_STEAM_CONTROLLERS";
    public const string SDL_HINT_ANDROID_TRAP_BACK_BUTTON =
        "SDL_ANDROID_TRAP_BACK_BUTTON";

    /* Only available in 2.0.10 or higher. */
    public const string SDL_HINT_MOUSE_TOUCH_EVENTS =
        "SDL_MOUSE_TOUCH_EVENTS";
    public const string SDL_HINT_GAMECONTROLLERCONFIG_FILE =
        "SDL_GAMECONTROLLERCONFIG_FILE";
    public const string SDL_HINT_ANDROID_BLOCK_ON_PAUSE =
        "SDL_ANDROID_BLOCK_ON_PAUSE";
    public const string SDL_HINT_RENDER_BATCHING =
        "SDL_RENDER_BATCHING";
    public const string SDL_HINT_EVENT_LOGGING =
        "SDL_EVENT_LOGGING";
    public const string SDL_HINT_WAVE_RIFF_CHUNK_SIZE =
        "SDL_WAVE_RIFF_CHUNK_SIZE";
    public const string SDL_HINT_WAVE_TRUNCATION =
        "SDL_WAVE_TRUNCATION";
    public const string SDL_HINT_WAVE_FACT_CHUNK =
        "SDL_WAVE_FACT_CHUNK";

    public const string SDL_HINT_VIDO_X11_WINDOW_VISUALID = "SDL_VIDEO_X11_WINDOW_VISUALID";
    public const string SDL_HINT_VIDEO_EXTERNAL_CONTEXT = "SDL_VIDEO_EXTERNAL_CONTEXT";
    public const string SDL_HINT_JOYSTICK_HIDAPI_GAMECUBE = "SDL_JOYSTICK_HIDAPI_GAMECUBE";
    public const string SDL_HINT_DISPLAY_USABLE_BOUNDS = "SDL_DISPLAY_USABLE_BOUNDS";
    public const string SDL_HINT_VIDEO_FORCE_EGL = "SDL_VIDEO_FORCE_EGL";
    public const string SDL_HINT_GAMECONTROLLERTYPE = "SDL_GAMECONTROLLERTYPE";

    /* Only available in 2.0.14 or higher. */
    public const string SDL_HINT_JOYSTICK_HIDAPI_CORRELATE_XINPUT =
        "SDL_JOYSTICK_HIDAPI_CORRELATE_XINPUT"; /* NOTE: This was removed in 2.0.16. */
    public const string SDL_HINT_JOYSTICK_RAWINPUT =
        "SDL_JOYSTICK_RAWINPUT";
    public const string SDL_HINT_AUDIO_DEVICE_APP_NAME =
        "SDL_AUDIO_DEVICE_APP_NAME";
    public const string SDL_HINT_AUDIO_DEVICE_STREAM_NAME =
        "SDL_AUDIO_DEVICE_STREAM_NAME";
    public const string SDL_HINT_PREFERRED_LOCALES =
        "SDL_PREFERRED_LOCALES";
    public const string SDL_HINT_THREAD_PRIORITY_POLICY =
        "SDL_THREAD_PRIORITY_POLICY";
    public const string SDL_HINT_EMSCRIPTEN_ASYNCIFY =
        "SDL_EMSCRIPTEN_ASYNCIFY";
    public const string SDL_HINT_LINUX_JOYSTICK_DEADZONES =
        "SDL_LINUX_JOYSTICK_DEADZONES";
    public const string SDL_HINT_ANDROID_BLOCK_ON_PAUSE_PAUSEAUDIO =
        "SDL_ANDROID_BLOCK_ON_PAUSE_PAUSEAUDIO";
    public const string SDL_HINT_JOYSTICK_HIDAPI_PS5 =
        "SDL_JOYSTICK_HIDAPI_PS5";
    public const string SDL_HINT_THREAD_FORCE_REALTIME_TIME_CRITICAL =
        "SDL_THREAD_FORCE_REALTIME_TIME_CRITICAL";
    public const string SDL_HINT_JOYSTICK_THREAD =
        "SDL_JOYSTICK_THREAD";
    public const string SDL_HINT_AUTO_UPDATE_JOYSTICKS =
        "SDL_AUTO_UPDATE_JOYSTICKS";
    public const string SDL_HINT_AUTO_UPDATE_SENSORS =
        "SDL_AUTO_UPDATE_SENSORS";
    public const string SDL_HINT_JOYSTICK_HIDAPI_PS5_RUMBLE =
        "SDL_JOYSTICK_HIDAPI_PS5_RUMBLE";

    /* Only available in 2.0.16 or higher. */
    public const string SDL_HINT_WINDOWS_FORCE_MUTEX_CRITICAL_SECTIONS =
        "SDL_WINDOWS_FORCE_MUTEX_CRITICAL_SECTIONS";
    public const string SDL_HINT_WINDOWS_FORCE_SEMAPHORE_KERNEL =
        "SDL_WINDOWS_FORCE_SEMAPHORE_KERNEL";
    public const string SDL_HINT_JOYSTICK_HIDAPI_PS5_PLAYER_LED =
        "SDL_JOYSTICK_HIDAPI_PS5_PLAYER_LED";
    public const string SDL_HINT_WINDOWS_USE_D3D9EX =
        "SDL_WINDOWS_USE_D3D9EX";
    public const string SDL_HINT_JOYSTICK_HIDAPI_JOY_CONS =
        "SDL_JOYSTICK_HIDAPI_JOY_CONS";
    public const string SDL_HINT_JOYSTICK_HIDAPI_STADIA =
        "SDL_JOYSTICK_HIDAPI_STADIA";
    public const string SDL_HINT_JOYSTICK_HIDAPI_SWITCH_HOME_LED =
        "SDL_JOYSTICK_HIDAPI_SWITCH_HOME_LED";
    public const string SDL_HINT_ALLOW_ALT_TAB_WHILE_GRABBED =
        "SDL_ALLOW_ALT_TAB_WHILE_GRABBED";
    public const string SDL_HINT_KMSDRM_REQUIRE_DRM_MASTER =
        "SDL_KMSDRM_REQUIRE_DRM_MASTER";
    public const string SDL_HINT_AUDIO_DEVICE_STREAM_ROLE =
        "SDL_AUDIO_DEVICE_STREAM_ROLE";
    public const string SDL_HINT_X11_FORCE_OVERRIDE_REDIRECT =
        "SDL_X11_FORCE_OVERRIDE_REDIRECT";
    public const string SDL_HINT_JOYSTICK_HIDAPI_LUNA =
        "SDL_JOYSTICK_HIDAPI_LUNA";
    public const string SDL_HINT_JOYSTICK_RAWINPUT_CORRELATE_XINPUT =
        "SDL_JOYSTICK_RAWINPUT_CORRELATE_XINPUT";
    public const string SDL_HINT_AUDIO_INCLUDE_MONITORS =
        "SDL_AUDIO_INCLUDE_MONITORS";
    public const string SDL_HINT_VIDEO_WAYLAND_ALLOW_LIBDECOR =
        "SDL_VIDEO_WAYLAND_ALLOW_LIBDECOR";

    /* Only available in 2.0.18 or higher. */
    public const string SDL_HINT_VIDEO_EGL_ALLOW_TRANSPARENCY = "SDL_VIDEO_EGL_ALLOW_TRANSPARENCY";
    public const string SDL_HINT_APP_NAME = "SDL_APP_NAME";
    public const string SDL_HINT_SCREENSAVER_INHIBIT_ACTIVITY_NAME = "SDL_SCREENSAVER_INHIBIT_ACTIVITY_NAME";
    public const string SDL_HINT_IME_SHOW_UI = "SDL_IME_SHOW_UI";
    public const string SDL_HINT_WINDOW_NO_ACTIVATION_WHEN_SHOWN = "SDL_WINDOW_NO_ACTIVATION_WHEN_SHOWN";
    public const string SDL_HINT_POLL_SENTINEL = "SDL_POLL_SENTINEL";
    public const string SDL_HINT_JOYSTICK_DEVICE = "SDL_JOYSTICK_DEVICE";
    public const string SDL_HINT_LINUX_JOYSTICK_CLASSIC = "SDL_LINUX_JOYSTICK_CLASSIC";

    /* Only available in 2.0.20 or higher. */
    public const string SDL_HINT_RENDER_LINE_METHOD =
        "SDL_RENDER_LINE_METHOD";

    /* Only available in 2.0.22 or higher. */
    public const string SDL_HINT_FORCE_RAISEWINDOW =
        "SDL_HINT_FORCE_RAISEWINDOW";
    public const string SDL_HINT_IME_SUPPORT_EXTENDED_TEXT =
        "SDL_IME_SUPPORT_EXTENDED_TEXT";
    public const string SDL_HINT_JOYSTICK_GAMECUBE_RUMBLE_BRAKE =
        "SDL_JOYSTICK_GAMECUBE_RUMBLE_BRAKE";
    public const string SDL_HINT_JOYSTICK_ROG_CHAKRAM =
        "SDL_JOYSTICK_ROG_CHAKRAM";
    public const string SDL_HINT_MOUSE_RELATIVE_MODE_CENTER =
        "SDL_MOUSE_RELATIVE_MODE_CENTER";
    public const string SDL_HINT_MOUSE_AUTO_CAPTURE =
        "SDL_MOUSE_AUTO_CAPTURE";
    public const string SDL_HINT_VITA_TOUCH_MOUSE_DEVICE =
        "SDL_HINT_VITA_TOUCH_MOUSE_DEVICE";
    public const string SDL_HINT_VIDEO_WAYLAND_PREFER_LIBDECOR =
        "SDL_VIDEO_WAYLAND_PREFER_LIBDECOR";
    public const string SDL_HINT_VIDEO_FOREIGN_WINDOW_OPENGL =
        "SDL_VIDEO_FOREIGN_WINDOW_OPENGL";
    public const string SDL_HINT_VIDEO_FOREIGN_WINDOW_VULKAN =
        "SDL_VIDEO_FOREIGN_WINDOW_VULKAN";
    public const string SDL_HINT_X11_WINDOW_TYPE =
        "SDL_X11_WINDOW_TYPE";
    public const string SDL_HINT_QUIT_ON_LAST_WINDOW_CLOSE =
        "SDL_QUIT_ON_LAST_WINDOW_CLOSE";

    public const string SDL_HINT_VIDEO_DRIVER = "SDL_VIDEO_DRIVER";
    public const string SDL_HINT_AUDIO_DRIVER = "SDL_AUDIO_DRIVER";

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_ClearHints();

    [DllImport(LibName, EntryPoint = "SDL_GetHint", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetHint(byte* name);

    [DllImport(LibName, EntryPoint = "SDL_SetHint", CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_bool INTERNAL_SDL_SetHint(byte* name, byte* value);

    [DllImport(LibName, EntryPoint = "SDL_SetHintWithPriority", CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_bool INTERNAL_SDL_SetHintWithPriority(byte* name, byte* value, SDL_HintPriority priority);

    public static string SDL_GetHint(string name)
    {
        int utf8NameBufSize = Utf8Size(name);
        byte* utf8Name = stackalloc byte[utf8NameBufSize];
        return GetString(INTERNAL_SDL_GetHint(Utf8Encode(name, utf8Name, utf8NameBufSize)));
    }

    public static bool SDL_SetHint(string name, bool value)
    {
        return SDL_SetHint(name, value ? "1" : "0");
    }

    public static bool SDL_SetHint(string name, string value)
    {
        int utf8NameBufSize = Utf8Size(name);
        byte* utf8Name = stackalloc byte[utf8NameBufSize];

        int utf8ValueBufSize = Utf8Size(value);
        byte* utf8Value = stackalloc byte[utf8ValueBufSize];

        return INTERNAL_SDL_SetHint(Utf8Encode(
            name, utf8Name, utf8NameBufSize),
            Utf8Encode(value, utf8Value, utf8ValueBufSize)
            ) == SDL_bool.SDL_TRUE;
    }

    public static SDL_bool SDL_SetHintWithPriority(string name, string value, SDL_HintPriority priority)
    {
        int utf8NameBufSize = Utf8Size(name);
        byte* utf8Name = stackalloc byte[utf8NameBufSize];

        int utf8ValueBufSize = Utf8Size(value);
        byte* utf8Value = stackalloc byte[utf8ValueBufSize];

        return INTERNAL_SDL_SetHintWithPriority(
            Utf8Encode(name, utf8Name, utf8NameBufSize),
            Utf8Encode(value, utf8Value, utf8ValueBufSize),
            priority
        );
    }
    #endregion

    #region SDL_error.h
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_ClearError();

    [DllImport(LibName, EntryPoint = nameof(SDL_GetError), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetError();

    [DllImport(LibName, EntryPoint = nameof(SDL_SetError), CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void INTERNAL_SDL_SetError(byte* fmtAndArglist);

    public static string SDL_GetError()
    {
        return GetString(INTERNAL_SDL_GetError());
    }

    public static void SDL_SetError(string fmtAndArglist)
    {
        int utf8FmtAndArglistBufSize = Utf8Size(fmtAndArglist);
        byte* utf8FmtAndArglist = stackalloc byte[utf8FmtAndArglistBufSize];
        INTERNAL_SDL_SetError(
            Utf8Encode(fmtAndArglist, utf8FmtAndArglist, utf8FmtAndArglistBufSize)
        );
    }
    #endregion

    #region SDL_log.h

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LogSetAllPriority(SDL_LogPriority priority);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LogSetPriority(int category, SDL_LogPriority priority);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LogResetPriorities();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_LogPriority SDL_LogGetPriority(int category);

    private static SDL_LogOutputFunction? _logCallback;

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

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetVersion(out SDL_version ver);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetRevision), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetRevision();

    public static string SDL_GetRevision() => GetString(INTERNAL_SDL_GetRevision());
    #endregion

    #region SDL_video.h

    public const int SDL_WINDOWPOS_UNDEFINED_MASK = 0x1FFF0000;
    public const int SDL_WINDOWPOS_CENTERED_MASK = 0x2FFF0000;
    public const int SDL_WINDOWPOS_UNDEFINED = 0x1FFF0000;
    public const int SDL_WINDOWPOS_CENTERED = 0x2FFF0000;

    public static int SDL_WINDOWPOS_UNDEFINED_DISPLAY(int X)
    {
        return (SDL_WINDOWPOS_UNDEFINED_MASK | X);
    }

    public static bool SDL_WINDOWPOS_ISUNDEFINED(int X)
    {
        return (X & 0xFFFF0000) == SDL_WINDOWPOS_UNDEFINED_MASK;
    }

    public static int SDL_WINDOWPOS_CENTERED_DISPLAY(int X)
    {
        return (SDL_WINDOWPOS_CENTERED_MASK | X);
    }

    public static bool SDL_WINDOWPOS_ISCENTERED(int X)
    {
        return (X & 0xFFFF0000) == SDL_WINDOWPOS_CENTERED_MASK;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SDL_HitTestResult SDL_HitTest(IntPtr win, IntPtr area, IntPtr data);

    [DllImport(LibName, EntryPoint = "SDL_CreateWindow", CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_Window INTERNAL_SDL_CreateWindow(byte* title, int w, int h, uint flags);

    public static SDL_Window SDL_CreateWindow(string title, int width, int height, SDL_WindowFlags flags)
    {
        int utf8TitleBufSize = Utf8Size(title);
        byte* utf8Title = stackalloc byte[utf8TitleBufSize];
        return INTERNAL_SDL_CreateWindow(
            Utf8Encode(title, utf8Title, utf8TitleBufSize),
            width, height,
            (uint)flags
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_CreateWindowAndRenderer(int width, int height, SDL_WindowFlags windowFlags, out SDL_Window window, out SDL_Renderer renderer);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_Window SDL_CreatePopupWindow(SDL_Window parent, int offset_x, int offset_y, int w, int h, SDL_WindowFlags flags);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Window SDL_CreateWindowFrom(nint data);

    [DllImport(LibName, EntryPoint = "SDL_CreateWindowWithPosition", CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_Window INTERNAL_SDL_CreateWindowWithPosition(byte* title, int x, int y, int w, int h, uint flags);

    public static SDL_Window SDL_CreateWindowWithPosition(string title, int x, int y, int width, int height, SDL_WindowFlags flags)
    {
        int utf8TitleBufSize = Utf8Size(title);
        byte* utf8Title = stackalloc byte[utf8TitleBufSize];
        return INTERNAL_SDL_CreateWindowWithPosition(
            Utf8Encode(title, utf8Title, utf8TitleBufSize),
            x, y,
            width, height,
            (uint)flags
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_DestroyWindow(SDL_Window window);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GetWindowSize(SDL_Window window, out int width, out int height);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowSize(SDL_Window window, int width, int height);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GetWindowSizeInPixels(SDL_Window window, out int width, out int height);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_ShowWindow(SDL_Window window);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_HideWindow(SDL_Window window);

    [DllImport(LibName, EntryPoint = nameof(SDL_SetWindowTitle), CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe void INTERNAL_SDL_SetWindowTitle(SDL_Window window, byte* title);

    public static void SDL_SetWindowTitle(SDL_Window window, string title)
    {
        int utf8TitleBufSize = Utf8Size(title);
        byte* utf8Title = stackalloc byte[utf8TitleBufSize];
        INTERNAL_SDL_SetWindowTitle(
            window,
            Utf8Encode(title, utf8Title, utf8TitleBufSize)
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetWindowFullscreenMode(SDL_Window window, SDL_DisplayMode* mode);

    [DllImport(LibName, EntryPoint = nameof(SDL_SetWindowFullscreen), CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int INTERNAL_SDL_SetWindowFullscreen(SDL_Window window, SDL_bool fullscreen);

    public static int SDL_SetWindowFullscreen(SDL_Window window, bool fullscreen)
    {
        return INTERNAL_SDL_SetWindowFullscreen(window, fullscreen ? SDL_TRUE : SDL_FALSE);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_DisableScreenSaver();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_EnableScreenSaver();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayMode* SDL_GetClosestFullscreenDisplayMode(SDL_DisplayID displayID, int w, int h, float refresh_rate);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayMode* SDL_GetCurrentDisplayMode(SDL_DisplayID displayIndex);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetCurrentVideoDriver), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetCurrentVideoDriver();

    public static string SDL_GetCurrentVideoDriver()
    {
        return GetString(INTERNAL_SDL_GetCurrentVideoDriver());
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayMode* SDL_GetDesktopDisplayMode(SDL_DisplayID displayID, out SDL_DisplayMode mode);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetDisplayBounds(SDL_DisplayID displayID, out Rectangle rect);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayOrientation SDL_GetCurrentDisplayOrientation(SDL_DisplayID displayID);

    /* Only available in 2.0.5 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetDisplayUsableBounds(SDL_DisplayID displayID, out Rectangle rect);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayMode* SDL_GetFullscreenDisplayModes(SDL_DisplayID displayID, out int count);


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayID* SDL_GetDisplays(out int count);

    public static ReadOnlySpan<SDL_DisplayID> SDL_GetDisplays()
    {
        SDL_DisplayID* displaysPtr = SDL_GetDisplays(out int count);
        return new(displaysPtr, count);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayID SDL_GetPrimaryDisplay();

    [DllImport(LibName, EntryPoint = nameof(SDL_GetDisplayName), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetDisplayName(SDL_DisplayID displayID);

    public static string SDL_GetDisplayName(SDL_DisplayID displayID)
    {
        return GetString(INTERNAL_SDL_GetDisplayName(displayID));
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumVideoDrivers();

    [DllImport(LibName, EntryPoint = nameof(SDL_GetVideoDriver), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetVideoDriver(int index);

    public static string SDL_GetVideoDriver(int index) => GetString(INTERNAL_SDL_GetVideoDriver(index));

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayID SDL_GetDisplayForPoint(in Point point);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayID SDL_GetDisplayForRect(in Rectangle rect);

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern float SDL_GetWindowBrightness(
        SDL_Window window
    );

    /* window refers to an SDL_Window*
     * Only available in 2.0.5 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetWindowOpacity(
        SDL_Window window,
        float opacity
    );

    /* window refers to an SDL_Window*
     * Only available in 2.0.5 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetWindowOpacity(
        SDL_Window window,
        out float out_opacity
    );

    /* modal_window and parent_window refer to an SDL_Window*s
     * Only available in 2.0.5 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetWindowModalFor(SDL_Window modal_window, SDL_Window parent_window);

    /* window refers to an SDL_Window*
     * Only available in 2.0.5 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetWindowInputFocus(SDL_Window window);

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

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayID SDL_GetDisplayForWindow(SDL_Window window);

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_DisplayMode* SDL_GetWindowFullscreenMode(SDL_Window window);

    /* IntPtr refers to a void*
     * window refers to an SDL_Window*
     * mode refers to a size_t*
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GetWindowICCProfile(
        SDL_Window window,
        out IntPtr mode
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_WindowFlags SDL_GetWindowFlags(SDL_Window window);



    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Window SDL_GetWindowFromID(uint id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Window SDL_GetWindowParent(SDL_Window window);

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetWindowGammaRamp(
        SDL_Window window,
        [Out()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
                ushort[] red,
        [Out()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
                ushort[] green,
        [Out()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
                ushort[] blue
    );

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GetWindowGrab(SDL_Window window);

    /* window refers to an SDL_Window*
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GetWindowKeyboardGrab(SDL_Window window);

    /* window refers to an SDL_Window*
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GetWindowMouseGrab(SDL_Window window);

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint SDL_GetWindowPixelFormat(
        SDL_Window window
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GetWindowMaximumSize(SDL_Window window, out int max_w, out int max_h);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GetWindowMinimumSize(SDL_Window window, out int min_w, out int min_h);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GetWindowPosition(SDL_Window window, out int x, out int y);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Surface SDL_GetWindowSurface(SDL_Window window);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetWindowTitle), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetWindowTitle(SDL_Window window);

    public static string SDL_GetWindowTitle(IntPtr window)
    {
        return GetString(
            INTERNAL_SDL_GetWindowTitle(window)
        );
    }

    /* texture refers to an SDL_Texture* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_BindTexture(
        SDL_Window texture,
        out float texw,
        out float texh
    );

    /* IntPtr and window refer to an SDL_GLContext and SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_GLContext SDL_GL_CreateContext(SDL_Window window);

    /* context refers to an SDL_GLContext */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_DeleteContext(SDL_GLContext context);

    [DllImport(LibName, EntryPoint = nameof(SDL_GL_LoadLibrary), CallingConvention = CallingConvention.Cdecl)]
    private static extern int INTERNAL_SDL_GL_LoadLibrary(byte* path);

    public static int SDL_GL_LoadLibrary(string path)
    {
        byte* utf8Path = Utf8EncodeHeap(path);
        int result = INTERNAL_SDL_GL_LoadLibrary(
            utf8Path
        );
        NativeMemory.Free(utf8Path);
        return result;
    }

    /* IntPtr refers to a function pointer, proc to a const char* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GL_GetProcAddress(IntPtr proc);

    /* IntPtr refers to a function pointer */
    public static unsafe IntPtr SDL_GL_GetProcAddress(string proc)
    {
        int utf8ProcBufSize = Utf8Size(proc);
        byte* utf8Proc = stackalloc byte[utf8ProcBufSize];
        return SDL_GL_GetProcAddress(
            (IntPtr)Utf8Encode(proc, utf8Proc, utf8ProcBufSize)
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_EGL_GetProcAddress(IntPtr proc);

    /* IntPtr refers to a function pointer */
    public static unsafe IntPtr SDL_EGL_GetProcAddress(string proc)
    {
        int utf8ProcBufSize = Utf8Size(proc);
        byte* utf8Proc = stackalloc byte[utf8ProcBufSize];
        return SDL_EGL_GetProcAddress(
            (IntPtr)Utf8Encode(proc, utf8Proc, utf8ProcBufSize)
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GL_UnloadLibrary();

    [DllImport(LibName, EntryPoint = "SDL_GL_ExtensionSupported", CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_bool INTERNAL_SDL_GL_ExtensionSupported(byte* extension);

    public static bool SDL_GL_ExtensionSupported(string extension)
    {
        int utf8ExtensionBufSize = Utf8Size(extension);
        byte* utf8Extension = stackalloc byte[utf8ExtensionBufSize];
        return INTERNAL_SDL_GL_ExtensionSupported(
            Utf8Encode(extension, utf8Extension, utf8ExtensionBufSize)
        ) == SDL_TRUE;
    }

    /* Only available in SDL 2.0.2 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GL_ResetAttributes();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_GetAttribute(
        SDL_GLattr attr,
        out int value
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_GetSwapInterval(int* interval);

    /* window and context refer to an SDL_Window* and SDL_GLContext */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_MakeCurrent(SDL_Window window, SDL_GLContext context);

    /* IntPtr refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Window SDL_GL_GetCurrentWindow();

    /* IntPtr refers to an SDL_Context */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_GLContext SDL_GL_GetCurrentContext();


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_SetAttribute(
        SDL_GLattr attr,
        int value
    );

    public static int SDL_GL_SetAttribute(SDL_GLattr attr, SDL_GLprofile profile)
    {
        return SDL_GL_SetAttribute(attr, (int)profile);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_SetSwapInterval(int interval);

    /// <summary>
    /// Update a window with OpenGL rendering.
    /// </summary>
    /// <param name="window"></param>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_SwapWindow(SDL_Window window);

    /* texture refers to an SDL_Texture* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_UnbindTexture(SDL_Window texture);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_ScreenSaverEnabled();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_RaiseWindow(SDL_Window window);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_MaximizeWindow(SDL_Window window);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_MinimizeWindow(SDL_Window window);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_RestoreWindow(SDL_Window window);

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetWindowBrightness(
        SDL_Window window,
        float brightness
    );

    /* IntPtr and userdata are void*, window is an SDL_Window* */
    [DllImport(LibName, EntryPoint = "SDL_SetWindowData", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe IntPtr INTERNAL_SDL_SetWindowData(
        SDL_Window window,
        byte* name,
        IntPtr userdata
    );
    public static unsafe IntPtr SDL_SetWindowData(
        SDL_Window window,
        string name,
        IntPtr userdata
    )
    {
        int utf8NameBufSize = Utf8Size(name);
        byte* utf8Name = stackalloc byte[utf8NameBufSize];
        return INTERNAL_SDL_SetWindowData(
            window,
            Utf8Encode(name, utf8Name, utf8NameBufSize),
            userdata
        );
    }

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetWindowGammaRamp(
        SDL_Window window,
        [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
                ushort[] red,
        [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
                ushort[] green,
        [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)]
                ushort[] blue
    );

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowGrab(
        SDL_Window window,
        SDL_bool grabbed
    );

    /* window refers to an SDL_Window*
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowKeyboardGrab(
        SDL_Window window,
        SDL_bool grabbed
    );

    /* window refers to an SDL_Window*
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowMouseGrab(
        SDL_Window window,
        SDL_bool grabbed
    );


    /* window refers to an SDL_Window*, icon to an SDL_Surface* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowIcon(
        SDL_Window window,
        IntPtr icon
    );

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowMaximumSize(
        SDL_Window window,
        int max_w,
        int max_h
    );

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowMinimumSize(
        SDL_Window window,
        int min_w,
        int min_h
    );

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowPosition(SDL_Window window, int x, int y);

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowBordered(
        SDL_Window window,
        SDL_bool bordered
    );

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetWindowBordersSize(
        SDL_Window window,
        out int top,
        out int left,
        out int bottom,
        out int right
    );

    [DllImport(LibName, EntryPoint = "SDL_SetWindowResizable", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int INTERNAL_SDL_SetWindowResizable(nint window, SDL_bool resizable);

    public static int SDL_SetWindowResizable(in SDL_Window window, bool resizable)
    {
        return INTERNAL_SDL_SetWindowResizable(window, resizable ? SDL_TRUE : SDL_FALSE);
    }

    /* window refers to an SDL_Window*
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowAlwaysOnTop(
        SDL_Window window,
        SDL_bool on_top
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_UpdateWindowSurface(SDL_Window window);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_UpdateWindowSurfaceRects(
        SDL_Window window,
        [In] Rectangle[] rects,
        int numrects
    );

    [DllImport(LibName, EntryPoint = "SDL_VideoInit", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int INTERNAL_SDL_VideoInit(
        byte* driver_name
    );
    public static unsafe int SDL_VideoInit(string driver_name)
    {
        int utf8DriverNameBufSize = Utf8Size(driver_name);
        byte* utf8DriverName = stackalloc byte[utf8DriverNameBufSize];
        return INTERNAL_SDL_VideoInit(
            Utf8Encode(driver_name, utf8DriverName, utf8DriverNameBufSize)
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_VideoQuit();

    /* window refers to an SDL_Window*, callback_data to a void*
     * Only available in 2.0.4 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetWindowHitTest(
        SDL_Window window,
        SDL_HitTest callback,
        IntPtr callback_data
    );

    /* IntPtr refers to an SDL_Window*
     * Only available in 2.0.4 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Window SDL_GetGrabbedWindow();

    /* window refers to an SDL_Window*
     * rect refers to an SDL_Rect*
     * This overload allows for IntPtr.Zero (null) to be passed for rect.
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetWindowMouseRect(
        SDL_Window window,
        in Rectangle rect
    );

    /* window refers to an SDL_Window*
     * IntPtr refers to an SDL_Rect*
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GetWindowMouseRect(
        SDL_Window window
    );

    /* window refers to an SDL_Window*
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_FlashWindow(
        SDL_Window window,
        SDL_FlashOperation operation
    );
    #endregion

    #region SDL_blendmode.h

    /* Only available in 2.0.6 or higher. */
    public static SDL_BlendMode SDL_ComposeCustomBlendMode(
        SDL_BlendFactor srcColorFactor,
        SDL_BlendFactor dstColorFactor,
        SDL_BlendOperation colorOperation,
        SDL_BlendFactor srcAlphaFactor,
        SDL_BlendFactor dstAlphaFactor,
        SDL_BlendOperation alphaOperation
    )
    {
        return SDL_ComposeCustomBlendMode(
            srcColorFactor, dstColorFactor, colorOperation,
            srcAlphaFactor, dstAlphaFactor, alphaOperation
            );
    }
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

    [DllImport(LibName, EntryPoint = "SDL_Vulkan_GetVkGetInstanceProcAddr", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr Internal_SDL_Vulkan_GetVkGetInstanceProcAddr();

    public static delegate* unmanaged<nint, sbyte*, delegate* unmanaged<void>> SDL_Vulkan_GetVkGetInstanceProcAddr()
    {
        return (delegate* unmanaged<nint, sbyte*, delegate* unmanaged<void>>)Internal_SDL_Vulkan_GetVkGetInstanceProcAddr();
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_Vulkan_UnloadLibrary();

    [DllImport(LibName, EntryPoint = "SDL_Vulkan_GetInstanceExtensions", CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_bool Internal_SDL_Vulkan_GetInstanceExtensions(uint* pCount, byte** pNames);

    /* window refers to an SDL_Window*, pNames to a const char**.
     * Only available in 2.0.6 or higher.
     * This overload allows for IntPtr.Zero (null) to be passed for pNames.
     */
    public static bool SDL_Vulkan_GetInstanceExtensions(uint* count, byte** pNames)
    {
        return Internal_SDL_Vulkan_GetInstanceExtensions(count, pNames) == SDL_TRUE;
    }

    public static string[] SDL_Vulkan_GetInstanceExtensions()
    {
        string[] names = Array.Empty<string>();

        uint count;
        bool result = Internal_SDL_Vulkan_GetInstanceExtensions(&count, null) == SDL_TRUE;
        if (result == true)
        {
            byte** strings = stackalloc byte*[(int)count];
            names = new string[count];
            Internal_SDL_Vulkan_GetInstanceExtensions(&count, strings);

            for (uint i = 0; i < count; i++)
            {
                names[i] = GetString(strings[i]);
            }
        }

        return names;
    }

    [DllImport(LibName, EntryPoint = "SDL_Vulkan_CreateSurface", CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_bool Internal_SDL_Vulkan_CreateSurface(nint window, nint instance, out ulong surface);

    public static bool SDL_Vulkan_CreateSurface(SDL_Window window, nint instance, out ulong surface)
    {
        return Internal_SDL_Vulkan_CreateSurface(window, instance, out surface) == SDL_TRUE;
    }
    #endregion

    #region SDL_syswm.h

    // FIXME: I wish these weren't public...
    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNAL_windows_wminfo
    {
        public nint window; // Refers to an HWND
        public nint hdc; // Refers to an HDC
        public nint hinstance; // Refers to an HINSTANCE
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNAL_winrt_wminfo
    {
        public nint window; // Refers to an IInspectable*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNAL_x11_wminfo
    {
        public nint display; // Refers to a Display*
        public int screen;
        public nint window; // Refers to a Window (XID, use ToInt64!)
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNAL_cocoa_wminfo
    {
        public IntPtr window; // Refers to an NSWindow*
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNAL_uikit_wminfo
    {
        public nint window; // Refers to a UIWindow*
        public uint framebuffer;
        public uint colorbuffer;
        public uint resolveFramebuffer;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNAL_wayland_wminfo
    {
        public nint display; // Refers to a wl_display*
        public nint surface; // Refers to a wl_surface*
        public nint egl_window; // Refers to an egl_window*, requires >= 2.0.16
        public nint xdg_surface; // Refers to an xdg_surface*, requires >= 2.0.16
        public nint xdg_toplevel; // Referes to an xdg_toplevel*, requires >= 2.0.18
        public nint xdg_popup;
        public nint xdg_positioner;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNAL_android_wminfo
    {
        public nint window; // Refers to an ANativeWindow
        public nint surface; // Refers to an EGLSurface
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNAL_vivante_wminfo
    {
        public IntPtr display; // Refers to an EGLNativeDisplayType
        public IntPtr window; // Refers to an EGLNativeWindowType
    }

    /* Only available in 2.0.16 or higher. */
    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNAL_kmsdrm_wminfo
    {
        int dev_index;
        int drm_fd;
        IntPtr gbm_dev; // Refers to a gbm_device*
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INTERNAL_SysWMDriverUnion
    {
        [FieldOffset(0)]
        public INTERNAL_windows_wminfo win;
        [FieldOffset(0)]
        public INTERNAL_winrt_wminfo winrt;
        [FieldOffset(0)]
        public INTERNAL_x11_wminfo x11;
        [FieldOffset(0)]
        public INTERNAL_cocoa_wminfo cocoa;
        [FieldOffset(0)]
        public INTERNAL_uikit_wminfo uikit;
        [FieldOffset(0)]
        public INTERNAL_wayland_wminfo wl;
        [FieldOffset(0)]
        public INTERNAL_android_wminfo android;
        [FieldOffset(0)]
        public INTERNAL_vivante_wminfo vivante;
        [FieldOffset(0)]
        public INTERNAL_kmsdrm_wminfo ksmdrm;

        //[FieldOffset(0)]
        //private fixed IntPtr* dummy_ptrs[14];
        [FieldOffset(0)]
        private fixed ulong dummy_ints[14];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_SysWMinfo
    {
        public uint version;
        public SDL_SYSWM_TYPE subsystem;

        private fixed uint padding[2];

        public INTERNAL_SysWMDriverUnion info;
    }

    public const int SDL_SYSWM_CURRENT_VERSION = 1;
    public static readonly int SDL_SYSWM_INFO_SIZE_V1 = (16 * (sizeof(void*) >= 8 ? sizeof(void*) : sizeof(ushort)));
    public static readonly int SDL_SYSWM_CURRENT_INFO_SIZE = SDL_SYSWM_INFO_SIZE_V1;

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetWindowWMInfo(SDL_Window window, SDL_SysWMinfo* info, uint version = SDL_SYSWM_CURRENT_VERSION);
    #endregion

    #region SDL_cpuinfo.h
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetCPUCount();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetCPUCacheLineSize();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasRDTSC();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasAltiVec();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasMMX();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasSSE();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasSSE2();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasSSE3();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasSSE41();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasSSE42();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasAVX();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasAVX2();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasAVX512F();

    /* Only available in SDL 2.0.11 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasARMSIMD();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasNEON();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasLSX();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasLASX();

    /* Only available in 2.0.1 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetSystemRAM();

    /* Only available in SDL 2.0.10 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern nuint SDL_SIMDGetAlignment();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void* SDL_aligned_alloc(nuint alignment, nuint size);

    public static void* SDL_aligned_alloc(nuint size) => SDL_aligned_alloc(SDL_SIMDGetAlignment(), size);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_aligned_free(void* ptr);

    #endregion

    #region SDL_events.h
    /* General keyboard/mouse state definitions. */
    public const byte SDL_PRESSED = 1;
    public const byte SDL_RELEASED = 0;

    /* Default size is according to SDL2 default. */
    public const int SDL_TEXTEDITINGEVENT_TEXT_SIZE = 32;
    public const int SDL_TEXTINPUTEVENT_TEXT_SIZE = 32;


    /// <summary>
    /// Fields shared by every event
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_CommonEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
    }

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_DisplayEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public SDL_DisplayID displayID;
        public int data1;
    }
#pragma warning restore 0169

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    /* Window state change event data (event.window.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_WindowEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public uint windowID;
        public int data1;
        public int data2;
    }
#pragma warning restore 0169

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    /* Keyboard button event structure (event.key.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_KeyboardEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public uint windowID;
        public byte state;
        public byte repeat; /* non-zero if this is a repeat */
        private byte padding2;
        private byte padding3;
        public SDL_Keysym keysym;
    }
#pragma warning restore 0169

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SDL_TextEditingEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public uint windowID;
        public fixed byte text[SDL_TEXTEDITINGEVENT_TEXT_SIZE];
        public int start;
        public int length;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SDL_TextEditingExtEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public uint windowID;
        public IntPtr text; /* char*, free with SDL_free */
        public int start;
        public int length;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SDL_TextInputEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public uint windowID;
        public fixed byte text[SDL_TEXTINPUTEVENT_TEXT_SIZE];
    }

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    /* Mouse motion event structure (event.motion.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_MouseMotionEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public uint windowID; /* SDL_WindowID */
        public uint which; /* SDL_MouseID */
        /// <summary>
        /// The current button state 
        /// </summary>
        public uint state;
        public float x;
        public float y;
        public float xrel;
        public float yrel;
    }
#pragma warning restore 0169

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    /* Mouse button event structure (event.button.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_MouseButtonEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public UInt32 windowID; /* SDL_WindowID */
        public UInt32 which; /* SDL_MouseID*/
        public byte button; /* button id */
        public byte state; /* SDL_PRESSED or SDL_RELEASED */
        public byte clicks; /* 1 for single-click, 2 for double-click, etc. */
        private byte padding1;
        public float x;
        public float y;
    }
#pragma warning restore 0169

    /* Mouse wheel event structure (event.wheel.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_MouseWheelEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public UInt32 windowID; /* SDL_WindowID */
        public UInt32 which;    /* SDL_MouseID */
        /// <summary>
        /// The amount scrolled horizontally, positive to the right and negative to the left 
        /// </summary>
        public float x;
        /// <summary>
        /// The amount scrolled vertically, positive away from the user and negative toward the user
        /// </summary>
        public float y;
        /// <summary>
        /// When FLIPPED the values in X and Y will be opposite. Multiply by -1 to change them back 
        /// </summary>
        public SDL_MouseWheelDirection direction;
        /// <summary>
        /// X coordinate, relative to window.
        /// </summary>
        public float mouseX;
        /// <summary>
        /// Y coordinate, relative to window.
        /// </summary>
        public float mouseY;
    }

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    /* Joystick axis motion event structure (event.jaxis.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_JoyAxisEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public SDL_JoystickID which;
        public byte axis;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public short axisValue; /* value, lolC# */
        public ushort padding4;
    }
#pragma warning restore 0169

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    /* Joystick hat position change event struct (event.jhat.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_JoyHatEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public SDL_JoystickID which;
        public byte hat; /* index of the hat */
        public byte hatValue; /* value, lolC# */
        private byte padding1;
        private byte padding2;
    }
#pragma warning restore 0169

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    /* Joystick button event structure (event.jbutton.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_JoyButtonEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public SDL_JoystickID which;
        public byte button;
        public byte state; /* SDL_PRESSED or SDL_RELEASED */
        private byte padding1;
        private byte padding2;
    }
#pragma warning restore 0169
    /// <summary>
    /// Joystick device event structure (event.jdevice.*)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_JoyDeviceEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public SDL_JoystickID which;
    }

    /// <summary>
    /// Joysick battery level change event structure (event.jbattery.*)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_JoyBatteryEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public SDL_JoystickID which;
        /// <summary>
        /// The joystick battery level
        /// </summary>
        public SDL_JoystickPowerLevel level;
    }

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    /// <summary>
    /// Gamepad axis motion event structure (event.gaxis.*)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_GamepadAxisEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public SDL_JoystickID which;
        public byte axis;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        /// <summary>
        /// The axis value (range: -32768 to 32767)
        /// </summary>
        public short value;
        private ushort padding4;
    }
#pragma warning restore 0169

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    /// <summary>
    /// Gamepad button event structure (event.gbutton.*)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_GamepadButtonEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public SDL_JoystickID which;
        public byte button;
        public byte state;
        private byte padding1;
        private byte padding2;
    }
#pragma warning restore 0169

    /// <summary>
    /// Gamepad device event structure (event.gdevice.*)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_GamepadDeviceEvent
    {
        public SDL_EventType type;
        /// <summary>
        /// In nanoseconds, populated using SDL_GetTicksNS()
        /// </summary>
        public ulong timestamp;
        /// <summary>
        /// The joystick instance id
        /// </summary>
        public SDL_JoystickID which;
    }

    /// <summary>
    /// Gamepad touchpad event structure (event.gtouchpad.*)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_GamepadTouchpadEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public SDL_JoystickID which;
        public int touchpad;
        public int finger;
        public float x;
        public float y;
        public float pressure;
    }

    /// <summary>
    /// Gamepad sensor event structure (event.gsensor.*)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_GamepadSensorEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public SDL_JoystickID which;
        public SDL_SensorType sensor;
        public unsafe fixed float data[2];
        public ulong sensor_timestamp;
    }

    // Ignore private members used for padding in this struct
#pragma warning disable 0169
    /* Audio device event (event.adevice.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_AudioDeviceEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public UInt32 which; /* SDL_AudioDeviceID */
        public byte iscapture;
        private byte padding1;
        private byte padding2;
        private byte padding3;
    }
#pragma warning restore 0169

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_TouchFingerEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public Int64 touchId; // SDL_TouchID
        public Int64 fingerId; // SDL_FingerID
        public float x;
        public float y;
        public float dx;
        public float dy;
        public float pressure;
        public uint windowID;
    }

    /* File open request by system (event.drop.*), enabled by
     * default
     */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_DropEvent
    {
        public SDL_EventType type;
        public ulong timestamp;

        /* char* filename, to be freed.
         * Access the variable EXACTLY ONCE like this:
         * string s = SDL.UTF8_ToManaged(evt.drop.file, true);
         */
        public sbyte* file;
        public uint windowID;
        public float x;
        public float y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SDL_SensorEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public Int32 which;
        public fixed float data[6];
        public ulong sensor_timestamp;
    }

    /* The "quit requested" event */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_QuitEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
    }

    /* A user defined event (event.user.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_UserEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public uint windowID;
        public int code;
        public nint data1; /* user-defined */
        public nint data2; /* user-defined */
    }

    /* A video driver dependent event (event.syswm.*), disabled */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_SysWMEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public nint msg; /* SDL_SysWMmsg*, system-dependent*/
    }

    /* General event structure */
    // C# doesn't do unions, so we do this ugly thing. */
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct SDL_Event
    {
        [FieldOffset(0)]
        public SDL_EventType type;
        [FieldOffset(0)]
        public SDL_CommonEvent common;
        [FieldOffset(0)]
        public SDL_DisplayEvent display;
        [FieldOffset(0)]
        public SDL_WindowEvent window;
        [FieldOffset(0)]
        public SDL_KeyboardEvent key;
        [FieldOffset(0)]
        public SDL_TextEditingEvent edit;
        [FieldOffset(0)]
        public SDL_TextEditingExtEvent editExt;
        [FieldOffset(0)]
        public SDL_TextInputEvent text;
        [FieldOffset(0)]
        public SDL_MouseMotionEvent motion;
        [FieldOffset(0)]
        public SDL_MouseButtonEvent button;
        [FieldOffset(0)]
        public SDL_MouseWheelEvent wheel;
        [FieldOffset(0)]
        public SDL_JoyAxisEvent jaxis;
        [FieldOffset(0)]
        public SDL_JoyHatEvent jhat;
        [FieldOffset(0)]
        public SDL_JoyButtonEvent jbutton;

        /// <summary>
        /// Joystick device change event data
        /// </summary>
        [FieldOffset(0)]
        public SDL_JoyDeviceEvent jdevice;

        /// <summary>
        /// Joystick battery event data.
        /// </summary>
        [FieldOffset(0)]
        public SDL_JoyBatteryEvent jbattery;

        [FieldOffset(0)]
        public SDL_GamepadAxisEvent gaxis;
        [FieldOffset(0)]
        public SDL_GamepadButtonEvent gbutton;
        [FieldOffset(0)]
        public SDL_GamepadDeviceEvent gdevice;
        [FieldOffset(0)]
        public SDL_GamepadTouchpadEvent gtouchpad;
        [FieldOffset(0)]
        public SDL_GamepadSensorEvent gsensor;
        [FieldOffset(0)]
        public SDL_AudioDeviceEvent adevice;
        [FieldOffset(0)]
        public SDL_SensorEvent sensor;
        [FieldOffset(0)]
        public SDL_QuitEvent quit;
        [FieldOffset(0)]
        public SDL_UserEvent user;
        [FieldOffset(0)]
        public SDL_SysWMEvent syswm;
        [FieldOffset(0)]
        public SDL_TouchFingerEvent tfinger;
        [FieldOffset(0)]
        public SDL_DropEvent drop;
        [FieldOffset(0)]
        private fixed byte padding[128];
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int SDL_EventFilter(
        nint userdata, // void*
        nint sdlevent // SDL_Event* event, lolC#
    );

    /// <summary>
    /// Pump the event loop, getting events from the input devices
    /// </summary>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_PumpEvents();

    public enum SDL_eventaction
    {
        SDL_ADDEVENT,
        SDL_PEEKEVENT,
        SDL_GETEVENT
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_PeepEvents(SDL_Event* events, int numevents, SDL_eventaction action, SDL_EventType minType, SDL_EventType maxType);

    /// <summary>
    /// Checks to see if certain events are in the event queue
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasEvent(SDL_EventType type);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasEvents(
            SDL_EventType minType,
            SDL_EventType maxType
        );

    /// <summary>
    /// Clears events from the event queue
    /// </summary>
    /// <param name="type"></param>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_FlushEvent(SDL_EventType type);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_FlushEvents(SDL_EventType min, SDL_EventType max);

    /// <summary>
    /// Polls for currently pending events
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_PollEvent(SDL_Event* @event);

    /// <summary>
    /// Waits indefinitely for the next event
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_WaitEvent(SDL_Event* @event);

    /// <summary>
    /// Waits until the specified timeout (in ms) for the next event
    /// </summary>
    /// <param name="_event"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_WaitEventTimeout(SDL_Event* @event, int timeout);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_PushEvent(SDL_Event* @event);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetEventFilter(SDL_EventFilter filter, nint userdata);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetEventFilter), CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_bool SDL_GetEventFilter(out nint filter, out nint userdata);

    public static bool SDL_GetEventFilter(out SDL_EventFilter? filter, out nint userdata)
    {
        bool retVal = SDL_GetEventFilter(out nint result, out userdata) == SDL_TRUE;
        if (result != 0)
        {
            filter = Marshal.GetDelegateForFunctionPointer<SDL_EventFilter>(result);
        }
        else
        {
            filter = null;
        }

        return retVal;
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_AddEventWatch(SDL_EventFilter filter, nint userdata);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_DelEventWatch(SDL_EventFilter filter, nint userdata);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_FilterEvents(SDL_EventFilter filter, nint userdata);

    /* This function allows you to enable/disable certain events */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_SetEventEnabled(SDL_EventType type, SDL_bool state);

    [DllImport(LibName, EntryPoint = "SDL_EventEnabled", CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_bool SDL_EventEnabled_Private(SDL_EventType type);

    public static void SDL_SetEventEnabled(SDL_EventType type, bool state)
    {
        SDL_SetEventEnabled(type, state ? SDL_TRUE : SDL_FALSE);
    }

    public static bool SDL_EventEnabled(SDL_EventType type)
    {
        return SDL_EventEnabled_Private(type) == SDL_TRUE;
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint SDL_RegisterEvents(int numevents);
    #endregion

    #region SDL_keycode.h
    public static SDL_KeyCode SDL_SCANCODE_TO_KEYCODE(SDL_Scancode X)
    {
        return (SDL_KeyCode)((int)X | SDLK_SCANCODE_MASK);
    }
    #endregion

    #region SDL_keyboard.h
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_Keysym
    {
        public SDL_Scancode scancode;
        public SDL_KeyCode sym;
        public SDL_Keymod mod; /* UInt16 */
        public uint unicode; /* Deprecated */
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Window SDL_GetKeyboardFocus();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GetKeyboardState(out int numkeys);

    /// <summary>
    /// Get the current key modifier state for the keyboard. 
    /// </summary>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Keymod SDL_GetModState();

    /* Set the current key modifier state */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetModState(SDL_Keymod modstate);

    /* Get the key code corresponding to the given scancode
     * with the current keyboard layout.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_KeyCode SDL_GetKeyFromScancode(SDL_Scancode scancode);

    /* Get the scancode for the given keycode */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Scancode SDL_GetScancodeFromKey(SDL_KeyCode key);

    /* Wrapper for SDL_GetScancodeName */
    [DllImport(LibName, EntryPoint = "SDL_GetScancodeName", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetScancodeName(SDL_Scancode scancode);
    public static string SDL_GetScancodeName(SDL_Scancode scancode)
    {
        return GetString(
            INTERNAL_SDL_GetScancodeName(scancode)
        );
    }

    /* Get a scancode from a human-readable name */
    [DllImport(LibName, EntryPoint = "SDL_GetScancodeFromName", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe SDL_Scancode INTERNAL_SDL_GetScancodeFromName(
        byte* name
    );
    public static unsafe SDL_Scancode SDL_GetScancodeFromName(string name)
    {
        int utf8NameBufSize = Utf8Size(name);
        byte* utf8Name = stackalloc byte[utf8NameBufSize];
        return INTERNAL_SDL_GetScancodeFromName(
            Utf8Encode(name, utf8Name, utf8NameBufSize)
        );
    }

    /* Wrapper for SDL_GetKeyName */
    [DllImport(LibName, EntryPoint = "SDL_GetKeyName", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetKeyName(SDL_KeyCode key);
    public static string SDL_GetKeyName(SDL_KeyCode key)
    {
        return GetString(INTERNAL_SDL_GetKeyName(key));
    }

    /* Get a key code from a human-readable name */
    [DllImport(LibName, EntryPoint = "SDL_GetKeyFromName", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe SDL_KeyCode INTERNAL_SDL_GetKeyFromName(
        byte* name
    );
    public static unsafe SDL_KeyCode SDL_GetKeyFromName(string name)
    {
        int utf8NameBufSize = Utf8Size(name);
        byte* utf8Name = stackalloc byte[utf8NameBufSize];
        return INTERNAL_SDL_GetKeyFromName(
            Utf8Encode(name, utf8Name, utf8NameBufSize)
        );
    }

    /* Start accepting Unicode text input events, show keyboard */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_StartTextInput();

    /* Check if unicode input events are enabled */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_TextInputActive();

    /* Stop receiving any text input events, hide onscreen kbd */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_StopTextInput();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_ClearComposition();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_TextInputShown();

    /* Set the rectangle used for text input, hint for IME */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetTextInputRect(ref Rectangle rect);

    /* Does the platform support an on-screen keyboard? */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasScreenKeyboardSupport();

    /* Is the on-screen keyboard shown for a given window? */
    /* window is an SDL_Window pointer */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_ScreenKeyboardShown(SDL_Window window);
    #endregion

    #region SDL_mouse.c
    /* Note: SDL_Cursor is a typedef normally. We'll treat it as
     * an IntPtr, because C# doesn't do typedefs. Yay!
     */

    /* System cursor types */
    public enum SDL_SystemCursor
    {
        SDL_SYSTEM_CURSOR_ARROW,    // Arrow
        SDL_SYSTEM_CURSOR_IBEAM,    // I-beam
        SDL_SYSTEM_CURSOR_WAIT,     // Wait
        SDL_SYSTEM_CURSOR_CROSSHAIR,    // Crosshair
        SDL_SYSTEM_CURSOR_WAITARROW,    // Small wait cursor (or Wait if not available)
        SDL_SYSTEM_CURSOR_SIZENWSE, // Double arrow pointing northwest and southeast
        SDL_SYSTEM_CURSOR_SIZENESW, // Double arrow pointing northeast and southwest
        SDL_SYSTEM_CURSOR_SIZEWE,   // Double arrow pointing west and east
        SDL_SYSTEM_CURSOR_SIZENS,   // Double arrow pointing north and south
        SDL_SYSTEM_CURSOR_SIZEALL,  // Four pointed arrow pointing north, south, east, and west
        SDL_SYSTEM_CURSOR_NO,       // Slashed circle or crossbones
        SDL_SYSTEM_CURSOR_HAND,     // Hand
        SDL_NUM_SYSTEM_CURSORS
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Window SDL_GetMouseFocus();

    /// <summary>
    /// Get the current state of the mouse
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint SDL_GetMouseState(out int x, out int y);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint SDL_GetGlobalMouseState(out int x, out int y);

    /* Get the mouse state with relative coords*/
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint SDL_GetRelativeMouseState(out int x, out int y);

    /* Set the mouse cursor's position (within a window) */
    /* window is an SDL_Window pointer */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_WarpMouseInWindow(SDL_Window window, int x, int y);

    /* Set the mouse cursor's position in global screen space.
     * Only available in 2.0.4 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_WarpMouseGlobal(int x, int y);

    /* Enable/Disable relative mouse mode (grabs mouse, rel coords) */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetRelativeMouseMode(SDL_bool enabled);

    /// <summary>
    /// Capture the mouse, to track input outside an SDL window.
    /// </summary>
    /// <param name="enabled"></param>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_CaptureMouse(SDL_bool enabled);

    /// <summary>
    /// Query if the relative mouse mode is enabled
    /// </summary>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GetRelativeMouseMode();

    /* Create a cursor from bitmap data (amd mask) in MSB format.
     * data and mask are byte arrays, and w must be a multiple of 8.
     * return value is an SDL_Cursor pointer.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Cursor SDL_CreateCursor(
        void* data,
        void* mask,
        int w, int h, int hot_x, int hot_y
    );

    /* Create a cursor from an SDL_Surface.
     * IntPtr refers to an SDL_Cursor*, surface to an SDL_Surface*
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Cursor SDL_CreateColorCursor(SDL_Surface surface, int hot_x, int hot_y);

    /* Create a cursor from a system cursor id.
     * return value is an SDL_Cursor pointer
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Cursor SDL_CreateSystemCursor(SDL_SystemCursor id);

    /* Set the active cursor.
     * cursor is an SDL_Cursor pointer
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetCursor(SDL_Cursor cursor);

    /* Return the active cursor
     * return value is an SDL_Cursor pointer
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Cursor SDL_GetCursor();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Cursor SDL_GetDefaultCursor();

    /* Frees a cursor created with one of the CreateCursor functions.
     * cursor in an SDL_Cursor pointer
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_DestroyCursor(SDL_Cursor cursor);

    /* Toggle whether or not the cursor is shown */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_ShowCursor(int toggle);

    public static uint SDL_BUTTON(uint X)
    {
        // If only there were a better way of doing this in C#
        return (uint)(1 << ((int)X - 1));
    }

    public const uint SDL_BUTTON_LEFT = 1;
    public const uint SDL_BUTTON_MIDDLE = 2;
    public const uint SDL_BUTTON_RIGHT = 3;
    public const uint SDL_BUTTON_X1 = 4;
    public const uint SDL_BUTTON_X2 = 5;
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
    public const byte SDL_HAPTIC_STEERING_AXIS = 3; /* Requires >= 2.0.14 */

    /* SDL_HapticRunEffect */
    public const uint SDL_HAPTIC_INFINITY = 4294967295U;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SDL_HapticDirection
    {
        public byte type;
        public fixed int dir[3];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_HapticConstant
    {
        // Header
        public ushort type;
        public SDL_HapticDirection direction;
        // Replay
        public uint length;
        public ushort delay;
        // Trigger
        public ushort button;
        public ushort interval;
        // Constant
        public short level;
        // Envelope
        public ushort attack_length;
        public ushort attack_level;
        public ushort fade_length;
        public ushort fade_level;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_HapticPeriodic
    {
        // Header
        public ushort type;
        public SDL_HapticDirection direction;
        // Replay
        public uint length;
        public ushort delay;
        // Trigger
        public ushort button;
        public ushort interval;
        // Periodic
        public ushort period;
        public short magnitude;
        public short offset;
        public ushort phase;
        // Envelope
        public ushort attack_length;
        public ushort attack_level;
        public ushort fade_length;
        public ushort fade_level;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SDL_HapticCondition
    {
        // Header
        public ushort type;
        public SDL_HapticDirection direction;
        // Replay
        public uint length;
        public ushort delay;
        // Trigger
        public ushort button;
        public ushort interval;
        // Condition
        public fixed ushort right_sat[3];
        public fixed ushort left_sat[3];
        public fixed short right_coeff[3];
        public fixed short left_coeff[3];
        public fixed ushort deadband[3];
        public fixed short center[3];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_HapticRamp
    {
        // Header
        public ushort type;
        public SDL_HapticDirection direction;
        // Replay
        public uint length;
        public ushort delay;
        // Trigger
        public ushort button;
        public ushort interval;
        // Ramp
        public short start;
        public short end;
        // Envelope
        public ushort attack_length;
        public ushort attack_level;
        public ushort fade_length;
        public ushort fade_level;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_HapticLeftRight
    {
        // Header
        public ushort type;
        // Replay
        public uint length;
        // Rumble
        public ushort large_magnitude;
        public ushort small_magnitude;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_HapticCustom
    {
        // Header
        public ushort type;
        public SDL_HapticDirection direction;
        // Replay
        public uint length;
        public ushort delay;
        // Trigger
        public ushort button;
        public ushort interval;
        // Custom
        public byte channels;
        public ushort period;
        public ushort samples;
        public IntPtr data; // Uint16*
                            // Envelope
        public ushort attack_length;
        public ushort attack_level;
        public ushort fade_length;
        public ushort fade_level;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct SDL_HapticEffect
    {
        [FieldOffset(0)]
        public ushort type;
        [FieldOffset(0)]
        public SDL_HapticConstant constant;
        [FieldOffset(0)]
        public SDL_HapticPeriodic periodic;
        [FieldOffset(0)]
        public SDL_HapticCondition condition;
        [FieldOffset(0)]
        public SDL_HapticRamp ramp;
        [FieldOffset(0)]
        public SDL_HapticLeftRight leftright;
        [FieldOffset(0)]
        public SDL_HapticCustom custom;
    }

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_HapticClose(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_HapticDestroyEffect(
        IntPtr haptic,
        int effect
    );

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticEffectSupported(
        IntPtr haptic,
        ref SDL_HapticEffect effect
    );

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticGetEffectStatus(
        IntPtr haptic,
        int effect
    );

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticIndex(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, EntryPoint = "SDL_HapticName", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_HapticName(int device_index);
    public static string SDL_HapticName(int device_index)
    {
        return GetString(INTERNAL_SDL_HapticName(device_index));
    }

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticNewEffect(
        IntPtr haptic,
        ref SDL_HapticEffect effect
    );

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticNumAxes(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticNumEffects(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticNumEffectsPlaying(IntPtr haptic);

    /* IntPtr refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_HapticOpen(int device_index);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticOpened(int device_index);

    /* IntPtr refers to an SDL_Haptic*, joystick to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_HapticOpenFromJoystick(
        IntPtr joystick
    );

    /* IntPtr refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_HapticOpenFromMouse();

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticPause(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint SDL_HapticQuery(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticRumbleInit(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticRumblePlay(
        IntPtr haptic,
        float strength,
        uint length
    );

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticRumbleStop(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticRumbleSupported(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticRunEffect(
        IntPtr haptic,
        int effect,
        uint iterations
    );

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticSetAutocenter(
        IntPtr haptic,
        int autocenter
    );

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticSetGain(
        IntPtr haptic,
        int gain
    );

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticStopAll(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticStopEffect(
        IntPtr haptic,
        int effect
    );

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticUnpause(IntPtr haptic);

    /* haptic refers to an SDL_Haptic* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_HapticUpdateEffect(
        IntPtr haptic,
        int effect,
        ref SDL_HapticEffect data
    );

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickIsHaptic(IntPtr joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_MouseIsHaptic();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_NumHaptics();
    #endregion

    #region SDL_timer.h

    /* System timers rely on different OS mechanisms depending on
     * which operating system SDL2 is compiled against.
     */

    /* Compare tick values, return true if A has passed B. Introduced in SDL 2.0.1,
     * but does not require it (it was a macro).
     */
    public static bool SDL_TICKS_PASSED(UInt32 A, UInt32 B)
    {
        return ((Int32)(B - A) <= 0);
    }

    /* Delays the thread's processing based on the milliseconds parameter */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_Delay(uint ms);

    /// <summary>
    /// Get the number of milliseconds since SDL library initialization.
    /// </summary>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong SDL_GetTicks();

    /// <summary>
    /// Get the number of nanoseconds since SDL library initialization.
    /// </summary>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong SDL_GetTicksNS();

    /* Get the current value of the high resolution counter */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong SDL_GetPerformanceCounter();

    /* Get the count per second of the high resolution counter */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong SDL_GetPerformanceFrequency();

    /* param refers to a void* */
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate UInt32 SDL_TimerCallback(UInt32 interval, IntPtr param);

    /* int refers to an SDL_TimerID, param to a void* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_AddTimer(
        UInt32 interval,
        SDL_TimerCallback callback,
        IntPtr param
    );

    /* id refers to an SDL_TimerID */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_RemoveTimer(int id);
    #endregion

    #region SDL_system.h

    /* Windows */

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr SDL_WindowsMessageHook(
        IntPtr userdata,
        IntPtr hWnd,
        uint message,
        ulong wParam,
        long lParam
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowsMessageHook(
        SDL_WindowsMessageHook callback,
        IntPtr userdata
    );

    /* renderer refers to an SDL_Renderer*
     * IntPtr refers to an IDirect3DDevice9*
     * Only available in 2.0.1 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GetRenderD3D9Device(SDL_Renderer renderer);

    /* renderer refers to an SDL_Renderer*
     * IntPtr refers to an ID3D11Device*
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GetRenderD3D11Device(SDL_Renderer renderer);

    /* iOS */

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SDL_iPhoneAnimationCallback(IntPtr p);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_iPhoneSetAnimationCallback(
        SDL_Window window, /* SDL_Window* */
        int interval,
        SDL_iPhoneAnimationCallback callback,
        IntPtr callbackParam
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_iPhoneSetEventPump(SDL_bool enabled);

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

    public enum SDL_WinRT_DeviceFamily
    {
        SDL_WINRT_DEVICEFAMILY_UNKNOWN,
        SDL_WINRT_DEVICEFAMILY_DESKTOP,
        SDL_WINRT_DEVICEFAMILY_MOBILE,
        SDL_WINRT_DEVICEFAMILY_XBOX
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_WinRT_DeviceFamily SDL_WinRTGetDeviceFamily();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_IsTablet();
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
    public static string? GetString(sbyte* source, int maxLength = -1)
    {
        return GetUtf8Span(source, maxLength).GetString();
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
