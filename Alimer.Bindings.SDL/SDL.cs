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

using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static Alimer.Bindings.SDL.SDL.SDL_bool;

namespace Alimer.Bindings.SDL;

public static unsafe class SDL
{
    public static event DllImportResolver? ResolveLibrary;

    private static readonly TempNativeLibrary s_sdl2Lib = LoadNativeLibrary();
    private const string LibName = "SDL3";

    static SDL()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static nint OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (TryResolveLibrary(libraryName, assembly, searchPath, out nint nativeLibrary))
        {
            return nativeLibrary;
        }

        if (libraryName.Equals(LibName) && TryResolveSDL3(assembly, searchPath, out nativeLibrary))
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

    private static bool TryResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, out nint nativeLibrary)
    {
        var resolveLibrary = ResolveLibrary;

        if (resolveLibrary != null)
        {
            var resolvers = resolveLibrary.GetInvocationList();

            foreach (DllImportResolver resolver in resolvers)
            {
                nativeLibrary = resolver(libraryName, assembly, searchPath);

                if (nativeLibrary != 0)
                {
                    return true;
                }
            }
        }

        nativeLibrary = 0;
        return false;
    }

    private static TempNativeLibrary LoadNativeLibrary()
    {
        if (OperatingSystem.IsWindows())
        {
            return new TempNativeLibrary("SDL3.dll");
        }
        else if (OperatingSystem.IsLinux())
        {
            return new TempNativeLibrary("libSDL2-2.0.so.0", "libSDL2-2.0.so.1", "libSDL2-2.0.so");
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            return new TempNativeLibrary("libSDL3.dylib");
        }
        else
        {
            Debug.WriteLine("Unknown SDL platform. Attempting to load \"SDL2\"");
            return new TempNativeLibrary("SDL3");
        }
    }

    // https://github.com/libsdl-org/SDL/blob/main/docs/README-migration.md

    public enum SDL_bool
    {
        SDL_FALSE = 0,
        SDL_TRUE = 1
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_free(void* memblock);

    #region SDL.h
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
    private static readonly delegate* unmanaged[Cdecl]<byte*> s_SDL_GetPlatform = (delegate* unmanaged[Cdecl]<byte*>)LoadFunction(nameof(SDL_GetPlatform));

    public static string SDL_GetPlatform() => GetString(s_SDL_GetPlatform())!;
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
    public const string SDL_HINT_VIDEO_X11_XVIDMODE =
        "SDL_VIDEO_X11_XVIDMODE";
    public const string SDL_HINT_VIDEO_X11_XINERAMA =
        "SDL_VIDEO_X11_XINERAMA";
    public const string SDL_HINT_VIDEO_X11_XRANDR =
        "SDL_VIDEO_X11_XRANDR";
    public const string SDL_HINT_GRAB_KEYBOARD =
        "SDL_GRAB_KEYBOARD";
    public const string SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS =
        "SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS";
    public const string SDL_HINT_IDLE_TIMER_DISABLED =
        "SDL_IOS_IDLE_TIMER_DISABLED";
    public const string SDL_HINT_ORIENTATIONS =
        "SDL_IOS_ORIENTATIONS";
    public const string SDL_HINT_XINPUT_ENABLED =
        "SDL_XINPUT_ENABLED";
    public const string SDL_HINT_GAMECONTROLLERCONFIG =
        "SDL_GAMECONTROLLERCONFIG";
    public const string SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS =
        "SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS";
    public const string SDL_HINT_ALLOW_TOPMOST =
        "SDL_ALLOW_TOPMOST";
    public const string SDL_HINT_TIMER_RESOLUTION =
        "SDL_TIMER_RESOLUTION";
    public const string SDL_HINT_RENDER_SCALE_QUALITY =
        "SDL_RENDER_SCALE_QUALITY";

    /* Only available in SDL 2.0.1 or higher. */
    public const string SDL_HINT_VIDEO_HIGHDPI_DISABLED =
        "SDL_VIDEO_HIGHDPI_DISABLED";

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
    public const string SDL_HINT_RENDER_LOGICAL_SIZE_MODE =
        "SDL_RENDER_LOGICAL_SIZE_MODE";
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

    /* Only available in 2.0.11 or higher. */
    public const string SDL_HINT_VIDO_X11_WINDOW_VISUALID =
        "SDL_VIDEO_X11_WINDOW_VISUALID";
    public const string SDL_HINT_GAMECONTROLLER_USE_BUTTON_LABELS =
        "SDL_GAMECONTROLLER_USE_BUTTON_LABELS";
    public const string SDL_HINT_VIDEO_EXTERNAL_CONTEXT =
        "SDL_VIDEO_EXTERNAL_CONTEXT";
    public const string SDL_HINT_JOYSTICK_HIDAPI_GAMECUBE =
        "SDL_JOYSTICK_HIDAPI_GAMECUBE";
    public const string SDL_HINT_DISPLAY_USABLE_BOUNDS =
        "SDL_DISPLAY_USABLE_BOUNDS";
    public const string SDL_HINT_VIDEO_X11_FORCE_EGL =
        "SDL_VIDEO_X11_FORCE_EGL";
    public const string SDL_HINT_GAMECONTROLLERTYPE =
        "SDL_GAMECONTROLLERTYPE";

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
    public const string SDL_HINT_MOUSE_RELATIVE_SCALING =
        "SDL_MOUSE_RELATIVE_SCALING";
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

    public enum SDL_HintPriority
    {
        SDL_HINT_DEFAULT,
        SDL_HINT_NORMAL,
        SDL_HINT_OVERRIDE
    }

    private static readonly delegate* unmanaged[Cdecl]<void> s_SDL_ClearHints = (delegate* unmanaged[Cdecl]<void>)LoadFunction(nameof(SDL_ClearHints));
    private static readonly delegate* unmanaged[Cdecl]<byte*, byte*> s_SDL_GetHint = (delegate* unmanaged[Cdecl]<byte*, byte*>)LoadFunction(nameof(SDL_GetHint));
    private static readonly delegate* unmanaged[Cdecl]<byte*, byte*, SDL_bool> s_SDL_SetHint = (delegate* unmanaged[Cdecl]<byte*, byte*, SDL_bool>)LoadFunction(nameof(SDL_SetHint));
    private static readonly delegate* unmanaged[Cdecl]<byte*, byte*, SDL_HintPriority, SDL_bool> s_SDL_SetHintWithPriority = (delegate* unmanaged[Cdecl]<byte*, byte*, SDL_HintPriority, SDL_bool>)LoadFunction(nameof(SDL_SetHintWithPriority));

    public static void SDL_ClearHints() => s_SDL_ClearHints();

    public static string SDL_GetHint(string name)
    {
        int utf8NameBufSize = Utf8Size(name);
        byte* utf8Name = stackalloc byte[utf8NameBufSize];
        return GetString(s_SDL_GetHint(Utf8Encode(name, utf8Name, utf8NameBufSize)));
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

        return s_SDL_SetHint(Utf8Encode(
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

        return s_SDL_SetHintWithPriority(
            Utf8Encode(name, utf8Name, utf8NameBufSize),
            Utf8Encode(value, utf8Value, utf8ValueBufSize),
            priority
        );
    }
    #endregion

    #region SDL_error.h
    private static readonly delegate* unmanaged[Cdecl]<void> s_SDL_ClearError = (delegate* unmanaged[Cdecl]<void>)LoadFunction(nameof(SDL_ClearError));
    private static readonly delegate* unmanaged[Cdecl]<byte*> s_SDL_GetError = (delegate* unmanaged[Cdecl]<byte*>)LoadFunction(nameof(SDL_GetError));
    private static readonly delegate* unmanaged[Cdecl]<byte*, void> s_SDL_SetError = (delegate* unmanaged[Cdecl]<byte*, void>)LoadFunction(nameof(SDL_SetError));

    public static void SDL_ClearError() => s_SDL_ClearError();
    public static string SDL_GetError() => GetString(s_SDL_GetError());

    public static void SDL_SetError(string fmtAndArglist)
    {
        int utf8FmtAndArglistBufSize = Utf8Size(fmtAndArglist);
        byte* utf8FmtAndArglist = stackalloc byte[utf8FmtAndArglistBufSize];
        s_SDL_SetError(
            Utf8Encode(fmtAndArglist, utf8FmtAndArglist, utf8FmtAndArglistBufSize)
        );
    }
    #endregion

    #region SDL_log.h
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

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LogSetAllPriority(SDL_LogPriority priority);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LogSetPriority(int category, SDL_LogPriority priority);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LogResetPriorities();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_LogPriority SDL_LogGetPriority(int category);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LogSetOutputFunction(delegate* unmanaged<IntPtr, int, SDL_LogPriority, sbyte*, void> callback, IntPtr userdata);
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

    private static readonly delegate* unmanaged[Cdecl]<out SDL_version, void> s_SDL_GetVersion = (delegate* unmanaged[Cdecl]<out SDL_version, void>)LoadFunction(nameof(SDL_GetVersion));
    private static readonly delegate* unmanaged[Cdecl]<byte*> s_SDL_GetRevision = (delegate* unmanaged[Cdecl]<byte*>)LoadFunction(nameof(SDL_GetRevision));
    private static readonly delegate* unmanaged[Cdecl]<int> s_SDL_GetRevisionNumber = (delegate* unmanaged[Cdecl]<int>)LoadFunction(nameof(SDL_GetRevisionNumber));

    public static void SDL_GetVersion(out SDL_version version) => s_SDL_GetVersion(out version);
    public static string SDL_GetRevision() => GetString(s_SDL_GetRevision());
    public static int SDL_GetRevisionNumber() => s_SDL_GetRevisionNumber();
    #endregion

    #region SDL_video.h

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

    public enum SDL_WindowEventID : byte
    {
        SDL_WINDOWEVENT_NONE,
        SDL_WINDOWEVENT_SHOWN,
        SDL_WINDOWEVENT_HIDDEN,
        SDL_WINDOWEVENT_EXPOSED,
        SDL_WINDOWEVENT_MOVED,
        SDL_WINDOWEVENT_RESIZED,
        SDL_WINDOWEVENT_SIZE_CHANGED,
        SDL_WINDOWEVENT_MINIMIZED,
        SDL_WINDOWEVENT_MAXIMIZED,
        SDL_WINDOWEVENT_RESTORED,
        SDL_WINDOWEVENT_ENTER,
        SDL_WINDOWEVENT_LEAVE,
        SDL_WINDOWEVENT_FOCUS_GAINED,
        SDL_WINDOWEVENT_FOCUS_LOST,
        SDL_WINDOWEVENT_CLOSE,
        /* Only available in 2.0.5 or higher. */
        SDL_WINDOWEVENT_TAKE_FOCUS,
        SDL_WINDOWEVENT_HIT_TEST,
        /* Only available in 2.0.18 or higher. */
        SDL_WINDOWEVENT_ICCPROF_CHANGED,
        SDL_WINDOWEVENT_DISPLAY_CHANGED
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

    /* Only available in 2.0.16 or higher. */
    public enum SDL_FlashOperation
    {
        SDL_FLASH_CANCEL,
        SDL_FLASH_BRIEFLY,
        SDL_FLASH_UNTIL_FOCUSED
    }

    [Flags]
    public enum SDL_WindowFlags : uint
    {
        SDL_WINDOW_FULLSCREEN = 0x00000001,   /**< window is in fullscreen mode */
        SDL_WINDOW_OPENGL = 0x00000002,   /**< window usable with OpenGL context */
        /* 0x00000004 was SDL_WINDOW_SHOWN in SDL2, please reserve this bit for sdl2-compat. */
        SDL_WINDOW_HIDDEN = 0x00000008,   /**< window is not visible */
        SDL_WINDOW_BORDERLESS = 0x00000010,   /**< no window decoration */
        SDL_WINDOW_RESIZABLE = 0x00000020,   /**< window can be resized */
        SDL_WINDOW_MINIMIZED = 0x00000040,   /**< window is minimized */
        SDL_WINDOW_MAXIMIZED = 0x00000080,   /**< window is maximized */
        SDL_WINDOW_MOUSE_GRABBED = 0x00000100,   /**< window has grabbed mouse input */
        SDL_WINDOW_INPUT_FOCUS = 0x00000200,   /**< window has input focus */
        SDL_WINDOW_MOUSE_FOCUS = 0x00000400,   /**< window has mouse focus */
        /* 0x00001000 was SDL_WINDOW_FULLSCREEN_DESKTOP in SDL2, please reserve this bit for sdl2-compat. */
        SDL_WINDOW_FOREIGN = 0x00000800,   /**< window not created by SDL */
        /* 0x00002000 was SDL_WINDOW_ALLOW_HIGHDPI in SDL2, please reserve this bit for sdl2-compat. */
        SDL_WINDOW_MOUSE_CAPTURE = 0x00004000,   /**< window has mouse captured (unrelated to MOUSE_GRABBED) */
        SDL_WINDOW_ALWAYS_ON_TOP = 0x00008000,   /**< window should always be above others */
        SDL_WINDOW_SKIP_TASKBAR = 0x00010000,   /**< window should not be added to the taskbar */
        SDL_WINDOW_UTILITY = 0x00020000,   /**< window should be treated as a utility window */
        SDL_WINDOW_TOOLTIP = 0x00040000,   /**< window should be treated as a tooltip */
        SDL_WINDOW_POPUP_MENU = 0x00080000,   /**< window should be treated as a popup menu */
        SDL_WINDOW_KEYBOARD_GRABBED = 0x00100000,   /**< window has grabbed keyboard input */
        SDL_WINDOW_VULKAN = 0x10000000,   /**< window usable for Vulkan surface */
        SDL_WINDOW_METAL = 0x20000000,   /**< window usable for Metal view */
    }

    /* Only available in 2.0.4 or higher. */
    public enum SDL_HitTestResult
    {
        SDL_HITTEST_NORMAL,     /* Region is normal. No special properties. */
        SDL_HITTEST_DRAGGABLE,      /* Region can drag entire window. */
        SDL_HITTEST_RESIZE_TOPLEFT,
        SDL_HITTEST_RESIZE_TOP,
        SDL_HITTEST_RESIZE_TOPRIGHT,
        SDL_HITTEST_RESIZE_RIGHT,
        SDL_HITTEST_RESIZE_BOTTOMRIGHT,
        SDL_HITTEST_RESIZE_BOTTOM,
        SDL_HITTEST_RESIZE_BOTTOMLEFT,
        SDL_HITTEST_RESIZE_LEFT
    }

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

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_DisplayMode
    {
        public uint format;
        public int w;
        public int h;
        public int refresh_rate;
        public IntPtr driverdata;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SDL_HitTestResult SDL_HitTest(IntPtr win, IntPtr area, IntPtr data);

    private static readonly delegate* unmanaged[Cdecl]<int, int, SDL_WindowFlags, out SDL_Window, out nint, int> s_SDL_CreateWindowAndRenderer = (delegate* unmanaged[Cdecl]<int, int, SDL_WindowFlags, out SDL_Window, out nint, int>)LoadFunction(nameof(SDL_CreateWindowAndRenderer));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Window, out int, out int, void> s_SDL_GetWindowSize = (delegate* unmanaged[Cdecl]<SDL_Window, out int, out int, void>)LoadFunction(nameof(SDL_GetWindowSize));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Window, int, int, void> s_SDL_SetWindowSize = (delegate* unmanaged[Cdecl]<SDL_Window, int, int, void>)LoadFunction(nameof(SDL_SetWindowSize));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Window, byte*, void> s_SDL_SetWindowTitle = (delegate* unmanaged[Cdecl]<SDL_Window, byte*, void>)LoadFunction(nameof(SDL_SetWindowTitle));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Window, uint, int> s_SDL_SetWindowFullscreen = (delegate* unmanaged[Cdecl]<SDL_Window, uint, int>)LoadFunction(nameof(SDL_SetWindowFullscreen));
    private static readonly delegate* unmanaged[Cdecl]<void> s_SDL_DisableScreenSaver = (delegate* unmanaged[Cdecl]<void>)LoadFunction(nameof(SDL_DisableScreenSaver));
    private static readonly delegate* unmanaged[Cdecl]<void> s_SDL_EnableScreenSaver = (delegate* unmanaged[Cdecl]<void>)LoadFunction(nameof(SDL_EnableScreenSaver));

    [DllImport(LibName, EntryPoint = "SDL_CreateWindow", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe IntPtr INTERNAL_SDL_CreateWindow(byte* title, int w, int h, uint flags);

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

    public static int SDL_CreateWindowAndRenderer(
        int width,
        int height,
        SDL_WindowFlags flags,
        out SDL_Window window,
        out nint renderer
        )
    {
        return s_SDL_CreateWindowAndRenderer(width, height, flags, out window, out renderer);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Window SDL_CreateWindowFrom(nint data);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_DestroyWindow(SDL_Window window);

    public static void SDL_GetWindowSize(SDL_Window window, out int width, out int height)
    {
        s_SDL_GetWindowSize(window, out width, out height);
    }

    public static void SDL_SetWindowSize(SDL_Window window, int width, int height)
    {
        s_SDL_SetWindowSize(window, width, height);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GetWindowSizeInPixels(SDL_Window window, out int width, out int height);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_ShowWindow(SDL_Window window);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_HideWindow(SDL_Window window);

    public static void SDL_SetWindowTitle(SDL_Window window, string title)
    {
        int utf8TitleBufSize = Utf8Size(title);
        byte* utf8Title = stackalloc byte[utf8TitleBufSize];
        s_SDL_SetWindowTitle(
            window,
            Utf8Encode(title, utf8Title, utf8TitleBufSize)
        );
    }

    public static int SDL_SetWindowFullscreen(SDL_Window window, SDL_WindowFlags flags)
    {
        return s_SDL_SetWindowFullscreen(window, (uint)flags);
    }

    public static void SDL_DisableScreenSaver() => s_SDL_DisableScreenSaver();
    public static void SDL_EnableScreenSaver() => s_SDL_EnableScreenSaver();

    private static readonly delegate* unmanaged[Cdecl]<int, ref SDL_DisplayMode, out SDL_DisplayMode, nint> s_SDL_GetClosestDisplayMode = (delegate* unmanaged[Cdecl]<int, ref SDL_DisplayMode, out SDL_DisplayMode, nint>)LoadFunction(nameof(SDL_GetClosestDisplayMode));
    private static readonly delegate* unmanaged[Cdecl]<int, out SDL_DisplayMode, int> s_SDL_GetCurrentDisplayMode = (delegate* unmanaged[Cdecl]<int, out SDL_DisplayMode, int>)LoadFunction(nameof(SDL_GetCurrentDisplayMode));
    private static readonly delegate* unmanaged[Cdecl]<int, out SDL_DisplayMode, int> s_SDL_GetDesktopDisplayMode = (delegate* unmanaged[Cdecl]<int, out SDL_DisplayMode, int>)LoadFunction(nameof(SDL_GetDesktopDisplayMode));
    private static readonly delegate* unmanaged[Cdecl]<byte*> s_SDL_GetCurrentVideoDriver = (delegate* unmanaged[Cdecl]<byte*>)LoadFunction(nameof(SDL_GetCurrentVideoDriver));
    private static readonly delegate* unmanaged[Cdecl]<int, byte*> s_SDL_GetDisplayName = (delegate* unmanaged[Cdecl]<int, byte*>)LoadFunction(nameof(SDL_GetDisplayName));

    public static nint SDL_GetClosestDisplayMode(int displayIndex, ref SDL_DisplayMode mode, out SDL_DisplayMode closest)
    {
        return s_SDL_GetClosestDisplayMode(displayIndex, ref mode, out closest);
    }

    public static int SDL_GetCurrentDisplayMode(int displayIndex, out SDL_DisplayMode mode)
    {
        return s_SDL_GetCurrentDisplayMode(displayIndex, out mode);
    }

    public static string SDL_GetCurrentVideoDriver()
    {
        return GetString(s_SDL_GetCurrentVideoDriver());
    }

    public static int SDL_GetDesktopDisplayMode(int displayIndex, out SDL_DisplayMode mode)
    {
        return s_SDL_GetDesktopDisplayMode(displayIndex, out mode);
    }

    public static string SDL_GetDisplayName(int index)
    {
        return GetString(s_SDL_GetDisplayName(index));
    }

    private static readonly delegate* unmanaged[Cdecl]<int, out Rectangle, int> s_SDL_GetDisplayBounds = (delegate* unmanaged[Cdecl]<int, out Rectangle, int>)LoadFunction(nameof(SDL_GetDisplayBounds));
    private static readonly delegate* unmanaged[Cdecl]<int, out float, out float, out float, int> s_SDL_GetDisplayDPI = (delegate* unmanaged[Cdecl]<int, out float, out float, out float, int>)LoadFunction(nameof(SDL_GetDisplayDPI));
    private static readonly delegate* unmanaged[Cdecl]<int, SDL_DisplayOrientation> s_SDL_GetDisplayOrientation = (delegate* unmanaged[Cdecl]<int, SDL_DisplayOrientation>)LoadFunction(nameof(SDL_GetDisplayOrientation));
    //private static readonly delegate* unmanaged[Cdecl]<uint, int*, SDL_DisplayMode*> s_SDL_GetFullscreenDisplayModes = (delegate* unmanaged[Cdecl]<uint, int*, SDL_DisplayMode*>)LoadFunction(nameof(SDL_GetFullscreenDisplayModes));
    private static readonly delegate* unmanaged[Cdecl]<int*, uint> s_SDL_GetDisplays = (delegate* unmanaged[Cdecl]<int*, uint>)LoadFunction(nameof(SDL_GetDisplays));
    private static readonly delegate* unmanaged[Cdecl]<int> s_SDL_GetNumVideoDrivers = (delegate* unmanaged[Cdecl]<int>)LoadFunction(nameof(SDL_GetNumVideoDrivers));
    private static readonly delegate* unmanaged[Cdecl]<int, byte*> s_SDL_GetVideoDriver = (delegate* unmanaged[Cdecl]<int, byte*>)LoadFunction(nameof(SDL_GetVideoDriver));

    public static int SDL_GetDisplayBounds(int displayIndex, out Rectangle rect)
    {
        return s_SDL_GetDisplayBounds(displayIndex, out rect);
    }

    public static int SDL_GetDisplayDPI(int displayIndex, out float ddpi, out float hdpi, out float vdpi)
    {
        return s_SDL_GetDisplayDPI(displayIndex, out ddpi, out hdpi, out vdpi);
    }

    public static SDL_DisplayOrientation SDL_GetDisplayOrientation(int displayIndex)
    {
        return s_SDL_GetDisplayOrientation(displayIndex);
    }

    /* Only available in 2.0.5 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetDisplayUsableBounds(
        int displayIndex,
        out Rectangle rect
    );

    // typedef Uint32 SDL_DisplayID;
    // typedef Uint32 SDL_WindowID;

    //public static int SDL_GetNumDisplayModes(uint displayID, int* count) => s_SDL_GetFullscreenDisplayModes(displayID, count);

    public static uint SDL_GetDisplays(int* count) => s_SDL_GetDisplays(count);
    public static int SDL_GetNumVideoDrivers() => s_SDL_GetNumVideoDrivers();

    public static string SDL_GetVideoDriver(int index) => GetString(s_SDL_GetVideoDriver(index));

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
    public static extern int SDL_GetWindowDisplayIndex(
        SDL_Window window
    );

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
    public static extern uint SDL_GetWindowID(SDL_Window window);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Window SDL_GetWindowFromID(uint id);

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

    private static readonly delegate* unmanaged[Cdecl]<SDL_Window, out int, out int, void> s_SDL_GetWindowMaximumSize = (delegate* unmanaged[Cdecl]<SDL_Window, out int, out int, void>)LoadFunction(nameof(SDL_GetWindowMaximumSize));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Window, out int, out int, void> s_SDL_GetWindowMinimumSize = (delegate* unmanaged[Cdecl]<SDL_Window, out int, out int, void>)LoadFunction(nameof(SDL_GetWindowMinimumSize));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Window, out int, out int, void> s_SDL_GetWindowPosition = (delegate* unmanaged[Cdecl]<SDL_Window, out int, out int, void>)LoadFunction(nameof(SDL_GetWindowPosition));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Window, SDL_Surface> s_SDL_GetWindowSurface = (delegate* unmanaged[Cdecl]<SDL_Window, SDL_Surface>)LoadFunction(nameof(SDL_GetWindowSurface));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Window, byte*> s_SDL_GetWindowTitle = (delegate* unmanaged[Cdecl]<SDL_Window, byte*>)LoadFunction(nameof(SDL_GetWindowTitle));

    public static void SDL_GetWindowMaximumSize(SDL_Window window, out int max_w, out int max_h)
        => s_SDL_GetWindowMaximumSize(window, out max_w, out max_h);

    public static void SDL_GetWindowMinimumSize(SDL_Window window, out int min_w, out int min_h)
        => s_SDL_GetWindowMinimumSize(window, out min_w, out min_h);

    public static void SDL_GetWindowPosition(SDL_Window window, out int x, out int y)
        => s_SDL_GetWindowPosition(window, out x, out y);

    public static SDL_Surface SDL_GetWindowSurface(SDL_Window window) => s_SDL_GetWindowSurface(window);

    public static string SDL_GetWindowTitle(SDL_Window window) => GetString(s_SDL_GetWindowTitle(window));

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

    [DllImport(LibName, EntryPoint = "SDL_GL_LoadLibrary", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int INTERNAL_SDL_GL_LoadLibrary(byte* path);
    public static unsafe int SDL_GL_LoadLibrary(string path)
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
    public static extern int SDL_GL_GetSwapInterval();

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

    /* window refers to an SDL_Window* */
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
    public static extern int SDL_SetWindowFullscreenMode(SDL_Window window, SDL_DisplayMode* mode);

    /* window refers to an SDL_Window* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetWindowFullscreen(
        SDL_Window window,
        uint flags
    );

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

    private static readonly delegate* unmanaged[Cdecl]<SDL_BlendFactor, SDL_BlendFactor, SDL_BlendOperation, SDL_BlendFactor, SDL_BlendFactor, SDL_BlendOperation, SDL_BlendMode> s_SDL_ComposeCustomBlendMode = (delegate* unmanaged[Cdecl]<SDL_BlendFactor, SDL_BlendFactor, SDL_BlendOperation, SDL_BlendFactor, SDL_BlendFactor, SDL_BlendOperation, SDL_BlendMode>)LoadFunction(nameof(SDL_ComposeCustomBlendMode));

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
        // private int dummy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_SysWMinfo
    {
        public uint version;
        public SDL_SYSWM_TYPE subsystem;
        public INTERNAL_SysWMDriverUnion info;
    }

    public const int SDL_SYSWM_CURRENT_VERSION = 1;

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetWindowWMInfo(nint window, SDL_SysWMinfo* info, uint version = SDL_SYSWM_CURRENT_VERSION);
    #endregion

    #region SDL_cpuinfo.h
    private static readonly delegate* unmanaged[Cdecl]<int> s_SDL_GetCPUCount = (delegate* unmanaged[Cdecl]<int>)LoadFunction(nameof(SDL_GetCPUCount));
    private static readonly delegate* unmanaged[Cdecl]<int> s_SDL_GetCPUCacheLineSize = (delegate* unmanaged[Cdecl]<int>)LoadFunction(nameof(SDL_GetCPUCacheLineSize));
    private static readonly delegate* unmanaged[Cdecl]<SDL_bool> s_SDL_HasRDTSC = (delegate* unmanaged[Cdecl]<SDL_bool>)LoadFunction(nameof(SDL_HasRDTSC));
    private static readonly delegate* unmanaged[Cdecl]<SDL_bool> s_SDL_HasAltiVec = (delegate* unmanaged[Cdecl]<SDL_bool>)LoadFunction(nameof(SDL_HasAltiVec));

    public static int SDL_GetCPUCount() => s_SDL_GetCPUCount();
    public static int SDL_GetCPUCacheLineSize() => s_SDL_GetCPUCacheLineSize();
    public static SDL_bool SDL_HasRDTSC() => s_SDL_HasRDTSC();
    public static SDL_bool SDL_HasAltiVec() => s_SDL_HasAltiVec();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasMMX();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_Has3DNow();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasSSE();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasSSE2();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasSSE3();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasSSE41();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasSSE42();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasAVX();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasAVX2();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasAVX512F();

    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasNEON();

    ///* Only available in 2.0.1 or higher. */
    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern int SDL_GetSystemRAM();

    ///* Only available in SDL 2.0.10 or higher. */
    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern uint SDL_SIMDGetAlignment();

    ///* Only available in SDL 2.0.10 or higher. */
    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern IntPtr SDL_SIMDAlloc(uint len);

    ///* Only available in SDL 2.0.14 or higher. */
    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern IntPtr SDL_SIMDRealloc(IntPtr ptr, uint len);

    ///* Only available in SDL 2.0.10 or higher. */
    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern void SDL_SIMDFree(IntPtr ptr);

    ///* Only available in SDL 2.0.11 or higher. */
    //[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    //public static extern SDL_bool SDL_HasARMSIMD();
    #endregion

    #region SDL_events.h
    /* General keyboard/mouse state definitions. */
    public const byte SDL_PRESSED = 1;
    public const byte SDL_RELEASED = 0;

    /* Default size is according to SDL2 default. */
    public const int SDL_TEXTEDITINGEVENT_TEXT_SIZE = 32;
    public const int SDL_TEXTINPUTEVENT_TEXT_SIZE = 32;

    /* The types of events that can be delivered. */
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

    /* Only available in 2.0.4 or higher. */
    public enum SDL_MouseWheelDirection : uint
    {
        SDL_MOUSEWHEEL_NORMAL,
        SDL_MOUSEWHEEL_FLIPPED
    }

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
        public uint displayID; // event, lolC#
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
        public UInt32 windowID;
        public UInt32 which;
        public byte button; /* button id */
        public byte state; /* SDL_PRESSED or SDL_RELEASED */
        public byte clicks; /* 1 for single-click, 2 for double-click, etc. */
        private byte padding1;
        public Int32 x;
        public Int32 y;
    }
#pragma warning restore 0169

    /* Mouse wheel event structure (event.wheel.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_MouseWheelEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public UInt32 windowID;
        public UInt32 which;
        public Int32 x; /* amount scrolled horizontally */
        public Int32 y; /* amount scrolled vertically */
        public UInt32 direction; /* Set to one of the SDL_MOUSEWHEEL_* defines */
        public float preciseX; /* Requires >= 2.0.18 */
        public float preciseY; /* Requires >= 2.0.18 */
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
    /* Joystick trackball motion event structure (event.jball.*) */
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_JoyBallEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public Int32 which; /* SDL_JoystickID */
        public byte ball;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public Int16 xrel;
        public Int16 yrel;
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
        public short axisValue; /* value, lolC# */
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
        public UInt32 which;
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
        public Int64 fingerId; // SDL_GestureID
        public float x;
        public float y;
        public float dx;
        public float dy;
        public float pressure;
        public uint windowID;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_MultiGestureEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public Int64 touchId; // SDL_TouchID
        public float dTheta;
        public float dDist;
        public float x;
        public float y;
        public UInt16 numFingers;
        public UInt16 padding;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_DollarGestureEvent
    {
        public SDL_EventType type;
        public ulong timestamp;
        public Int64 touchId; // SDL_TouchID
        public Int64 gestureId; // SDL_GestureID
        public UInt32 numFingers;
        public float error;
        public float x;
        public float y;
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
        public IntPtr file;
        public UInt32 windowID;
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
        public SDL_JoyBallEvent jball;
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
        public SDL_GamepadAxisEvent caxis;
        [FieldOffset(0)]
        public SDL_GamepadButtonEvent cbutton;
        [FieldOffset(0)]
        public SDL_GamepadDeviceEvent cdevice;
        [FieldOffset(0)]
        public SDL_GamepadTouchpadEvent ctouchpad;
        [FieldOffset(0)]
        public SDL_GamepadSensorEvent csensor;
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
        public SDL_MultiGestureEvent mgesture;
        [FieldOffset(0)]
        public SDL_DollarGestureEvent dgesture;
        [FieldOffset(0)]
        public SDL_DropEvent drop;
        [FieldOffset(0)]
        private fixed byte padding[56];
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int SDL_EventFilter(
        nint userdata, // void*
        nint sdlevent // SDL_Event* event, lolC#
    );

    private static readonly delegate* unmanaged[Cdecl]<SDL_Event*, int, SDL_eventaction, SDL_EventType, SDL_EventType, int> s_SDL_PeepEvents = (delegate* unmanaged[Cdecl]<SDL_Event*, int, SDL_eventaction, SDL_EventType, SDL_EventType, int>)LoadFunction(nameof(SDL_PeepEvents));
    private static readonly delegate* unmanaged[Cdecl]<SDL_EventType, SDL_bool> s_SDL_HasEvent = (delegate* unmanaged[Cdecl]<SDL_EventType, SDL_bool>)LoadFunction(nameof(SDL_HasEvent));
    private static readonly delegate* unmanaged[Cdecl]<SDL_EventType, SDL_EventType, SDL_bool> s_SDL_HasEvents = (delegate* unmanaged[Cdecl]<SDL_EventType, SDL_EventType, SDL_bool>)LoadFunction(nameof(SDL_HasEvents));
    private static readonly delegate* unmanaged[Cdecl]<SDL_EventType, void> s_SDL_FlushEvent = (delegate* unmanaged[Cdecl]<SDL_EventType, void>)LoadFunction(nameof(SDL_FlushEvent));
    private static readonly delegate* unmanaged[Cdecl]<SDL_EventType, SDL_EventType, void> s_SDL_FlushEvents = (delegate* unmanaged[Cdecl]<SDL_EventType, SDL_EventType, void>)LoadFunction(nameof(SDL_FlushEvents));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Event*, int> s_SDL_WaitEvent = (delegate* unmanaged[Cdecl]<SDL_Event*, int>)LoadFunction(nameof(SDL_WaitEvent));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Event*, int, int> s_SDL_WaitEventTimeout = (delegate* unmanaged[Cdecl]<SDL_Event*, int, int>)LoadFunction(nameof(SDL_WaitEventTimeout));
    private static readonly delegate* unmanaged[Cdecl]<ref SDL_Event, int> s_SDL_PushEvent = (delegate* unmanaged[Cdecl]<ref SDL_Event, int>)LoadFunction(nameof(SDL_PushEvent));
    private static readonly delegate* unmanaged[Cdecl]<SDL_EventFilter, nint, void> s_SDL_SetEventFilter = (delegate* unmanaged[Cdecl]<SDL_EventFilter, nint, void>)LoadFunction(nameof(SDL_SetEventFilter));
    private static readonly delegate* unmanaged[Cdecl]<out nint, out nint, SDL_bool> s_SDL_GetEventFilter = (delegate* unmanaged[Cdecl]<out nint, out nint, SDL_bool>)LoadFunction(nameof(SDL_GetEventFilter));
    private static readonly delegate* unmanaged[Cdecl]<SDL_EventFilter, nint, void> s_SDL_AddEventWatch = (delegate* unmanaged[Cdecl]<SDL_EventFilter, nint, void>)LoadFunction(nameof(SDL_AddEventWatch));
    private static readonly delegate* unmanaged[Cdecl]<SDL_EventFilter, nint, void> s_SDL_DelEventWatch = (delegate* unmanaged[Cdecl]<SDL_EventFilter, nint, void>)LoadFunction(nameof(SDL_DelEventWatch));
    private static readonly delegate* unmanaged[Cdecl]<SDL_EventFilter, nint, void> s_SDL_FilterEvents = (delegate* unmanaged[Cdecl]<SDL_EventFilter, nint, void>)LoadFunction(nameof(SDL_FilterEvents));

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

    public static unsafe int SDL_PeepEvents(SDL_Event* events, int numevents, SDL_eventaction action, SDL_EventType minType, SDL_EventType maxType)
    {
        return s_SDL_PeepEvents(events, numevents, action, minType, maxType);
    }

    /// <summary>
    /// Checks to see if certain events are in the event queue
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool SDL_HasEvent(SDL_EventType type) => s_SDL_HasEvent(type) == SDL_TRUE;

    public static bool SDL_HasEvents(SDL_EventType minType, SDL_EventType maxType)
    {
        return s_SDL_HasEvents(minType, maxType) == SDL_TRUE;
    }

    /// <summary>
    /// Clears events from the event queue
    /// </summary>
    /// <param name="type"></param>
    public static void SDL_FlushEvent(SDL_EventType type) => s_SDL_FlushEvent(type);

    public static void SDL_FlushEvents(SDL_EventType min, SDL_EventType max) => SDL_FlushEvents(min, max);

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
    public static int SDL_WaitEvent(SDL_Event* @event) => s_SDL_WaitEvent(@event);

    /// <summary>
    /// Waits until the specified timeout (in ms) for the next event
    /// </summary>
    /// <param name="_event"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static int SDL_WaitEventTimeout(SDL_Event* @event, int timeout)
    {
        return s_SDL_WaitEventTimeout(@event, timeout);
    }

    public static int SDL_PushEvent(ref SDL_Event @event) => s_SDL_PushEvent(ref @event);

    public static void SDL_SetEventFilter(SDL_EventFilter filter, nint userdata)
    {
        s_SDL_SetEventFilter(filter, userdata);
    }

    /* userdata refers to a void* */
    private static SDL_bool SDL_GetEventFilter(out nint filter, out nint userdata)
    {
        return s_SDL_GetEventFilter(out filter, out userdata);
    }

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

    public static void SDL_AddEventWatch(SDL_EventFilter filter, nint userdata)
    {
        s_SDL_AddEventWatch(filter, userdata);
    }

    public static void SDL_DelEventWatch(SDL_EventFilter filter, nint userdata)
    {
        s_SDL_DelEventWatch(filter, userdata);
    }

    public static void SDL_FilterEvents(SDL_EventFilter filter, nint userdata)
    {
        s_SDL_FilterEvents(filter, userdata);
    }

    /* These are for SDL_EventState() */
    public const int SDL_QUERY = -1;
    public const int SDL_IGNORE = 0;
    public const int SDL_DISABLE = 0;
    public const int SDL_ENABLE = 1;

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

    private static readonly delegate* unmanaged[Cdecl]<int, uint> s_SDL_RegisterEvents = (delegate* unmanaged[Cdecl]<int, uint>)LoadFunction(nameof(SDL_RegisterEvents));

    public static uint SDL_RegisterEvents(int numEvents) => s_SDL_RegisterEvents(numEvents);
    #endregion

    #region SDL_scancode.h
    /* Scancodes based off USB keyboard page (0x07) */
    public enum SDL_Scancode
    {
        SDL_SCANCODE_UNKNOWN = 0,

        SDL_SCANCODE_A = 4,
        SDL_SCANCODE_B = 5,
        SDL_SCANCODE_C = 6,
        SDL_SCANCODE_D = 7,
        SDL_SCANCODE_E = 8,
        SDL_SCANCODE_F = 9,
        SDL_SCANCODE_G = 10,
        SDL_SCANCODE_H = 11,
        SDL_SCANCODE_I = 12,
        SDL_SCANCODE_J = 13,
        SDL_SCANCODE_K = 14,
        SDL_SCANCODE_L = 15,
        SDL_SCANCODE_M = 16,
        SDL_SCANCODE_N = 17,
        SDL_SCANCODE_O = 18,
        SDL_SCANCODE_P = 19,
        SDL_SCANCODE_Q = 20,
        SDL_SCANCODE_R = 21,
        SDL_SCANCODE_S = 22,
        SDL_SCANCODE_T = 23,
        SDL_SCANCODE_U = 24,
        SDL_SCANCODE_V = 25,
        SDL_SCANCODE_W = 26,
        SDL_SCANCODE_X = 27,
        SDL_SCANCODE_Y = 28,
        SDL_SCANCODE_Z = 29,

        SDL_SCANCODE_1 = 30,
        SDL_SCANCODE_2 = 31,
        SDL_SCANCODE_3 = 32,
        SDL_SCANCODE_4 = 33,
        SDL_SCANCODE_5 = 34,
        SDL_SCANCODE_6 = 35,
        SDL_SCANCODE_7 = 36,
        SDL_SCANCODE_8 = 37,
        SDL_SCANCODE_9 = 38,
        SDL_SCANCODE_0 = 39,

        SDL_SCANCODE_RETURN = 40,
        SDL_SCANCODE_ESCAPE = 41,
        SDL_SCANCODE_BACKSPACE = 42,
        SDL_SCANCODE_TAB = 43,
        SDL_SCANCODE_SPACE = 44,

        SDL_SCANCODE_MINUS = 45,
        SDL_SCANCODE_EQUALS = 46,
        SDL_SCANCODE_LEFTBRACKET = 47,
        SDL_SCANCODE_RIGHTBRACKET = 48,
        SDL_SCANCODE_BACKSLASH = 49,
        SDL_SCANCODE_NONUSHASH = 50,
        SDL_SCANCODE_SEMICOLON = 51,
        SDL_SCANCODE_APOSTROPHE = 52,
        SDL_SCANCODE_GRAVE = 53,
        SDL_SCANCODE_COMMA = 54,
        SDL_SCANCODE_PERIOD = 55,
        SDL_SCANCODE_SLASH = 56,

        SDL_SCANCODE_CAPSLOCK = 57,

        SDL_SCANCODE_F1 = 58,
        SDL_SCANCODE_F2 = 59,
        SDL_SCANCODE_F3 = 60,
        SDL_SCANCODE_F4 = 61,
        SDL_SCANCODE_F5 = 62,
        SDL_SCANCODE_F6 = 63,
        SDL_SCANCODE_F7 = 64,
        SDL_SCANCODE_F8 = 65,
        SDL_SCANCODE_F9 = 66,
        SDL_SCANCODE_F10 = 67,
        SDL_SCANCODE_F11 = 68,
        SDL_SCANCODE_F12 = 69,

        SDL_SCANCODE_PRINTSCREEN = 70,
        SDL_SCANCODE_SCROLLLOCK = 71,
        SDL_SCANCODE_PAUSE = 72,
        SDL_SCANCODE_INSERT = 73,
        SDL_SCANCODE_HOME = 74,
        SDL_SCANCODE_PAGEUP = 75,
        SDL_SCANCODE_DELETE = 76,
        SDL_SCANCODE_END = 77,
        SDL_SCANCODE_PAGEDOWN = 78,
        SDL_SCANCODE_RIGHT = 79,
        SDL_SCANCODE_LEFT = 80,
        SDL_SCANCODE_DOWN = 81,
        SDL_SCANCODE_UP = 82,

        SDL_SCANCODE_NUMLOCKCLEAR = 83,
        SDL_SCANCODE_KP_DIVIDE = 84,
        SDL_SCANCODE_KP_MULTIPLY = 85,
        SDL_SCANCODE_KP_MINUS = 86,
        SDL_SCANCODE_KP_PLUS = 87,
        SDL_SCANCODE_KP_ENTER = 88,
        SDL_SCANCODE_KP_1 = 89,
        SDL_SCANCODE_KP_2 = 90,
        SDL_SCANCODE_KP_3 = 91,
        SDL_SCANCODE_KP_4 = 92,
        SDL_SCANCODE_KP_5 = 93,
        SDL_SCANCODE_KP_6 = 94,
        SDL_SCANCODE_KP_7 = 95,
        SDL_SCANCODE_KP_8 = 96,
        SDL_SCANCODE_KP_9 = 97,
        SDL_SCANCODE_KP_0 = 98,
        SDL_SCANCODE_KP_PERIOD = 99,

        SDL_SCANCODE_NONUSBACKSLASH = 100,
        SDL_SCANCODE_APPLICATION = 101,
        SDL_SCANCODE_POWER = 102,
        SDL_SCANCODE_KP_EQUALS = 103,
        SDL_SCANCODE_F13 = 104,
        SDL_SCANCODE_F14 = 105,
        SDL_SCANCODE_F15 = 106,
        SDL_SCANCODE_F16 = 107,
        SDL_SCANCODE_F17 = 108,
        SDL_SCANCODE_F18 = 109,
        SDL_SCANCODE_F19 = 110,
        SDL_SCANCODE_F20 = 111,
        SDL_SCANCODE_F21 = 112,
        SDL_SCANCODE_F22 = 113,
        SDL_SCANCODE_F23 = 114,
        SDL_SCANCODE_F24 = 115,
        SDL_SCANCODE_EXECUTE = 116,
        SDL_SCANCODE_HELP = 117,
        SDL_SCANCODE_MENU = 118,
        SDL_SCANCODE_SELECT = 119,
        SDL_SCANCODE_STOP = 120,
        SDL_SCANCODE_AGAIN = 121,
        SDL_SCANCODE_UNDO = 122,
        SDL_SCANCODE_CUT = 123,
        SDL_SCANCODE_COPY = 124,
        SDL_SCANCODE_PASTE = 125,
        SDL_SCANCODE_FIND = 126,
        SDL_SCANCODE_MUTE = 127,
        SDL_SCANCODE_VOLUMEUP = 128,
        SDL_SCANCODE_VOLUMEDOWN = 129,
        /* not sure whether there's a reason to enable these */
        /*	SDL_SCANCODE_LOCKINGCAPSLOCK = 130, */
        /*	SDL_SCANCODE_LOCKINGNUMLOCK = 131, */
        /*	SDL_SCANCODE_LOCKINGSCROLLLOCK = 132, */
        SDL_SCANCODE_KP_COMMA = 133,
        SDL_SCANCODE_KP_EQUALSAS400 = 134,

        SDL_SCANCODE_INTERNATIONAL1 = 135,
        SDL_SCANCODE_INTERNATIONAL2 = 136,
        SDL_SCANCODE_INTERNATIONAL3 = 137,
        SDL_SCANCODE_INTERNATIONAL4 = 138,
        SDL_SCANCODE_INTERNATIONAL5 = 139,
        SDL_SCANCODE_INTERNATIONAL6 = 140,
        SDL_SCANCODE_INTERNATIONAL7 = 141,
        SDL_SCANCODE_INTERNATIONAL8 = 142,
        SDL_SCANCODE_INTERNATIONAL9 = 143,
        SDL_SCANCODE_LANG1 = 144,
        SDL_SCANCODE_LANG2 = 145,
        SDL_SCANCODE_LANG3 = 146,
        SDL_SCANCODE_LANG4 = 147,
        SDL_SCANCODE_LANG5 = 148,
        SDL_SCANCODE_LANG6 = 149,
        SDL_SCANCODE_LANG7 = 150,
        SDL_SCANCODE_LANG8 = 151,
        SDL_SCANCODE_LANG9 = 152,

        SDL_SCANCODE_ALTERASE = 153,
        SDL_SCANCODE_SYSREQ = 154,
        SDL_SCANCODE_CANCEL = 155,
        SDL_SCANCODE_CLEAR = 156,
        SDL_SCANCODE_PRIOR = 157,
        SDL_SCANCODE_RETURN2 = 158,
        SDL_SCANCODE_SEPARATOR = 159,
        SDL_SCANCODE_OUT = 160,
        SDL_SCANCODE_OPER = 161,
        SDL_SCANCODE_CLEARAGAIN = 162,
        SDL_SCANCODE_CRSEL = 163,
        SDL_SCANCODE_EXSEL = 164,

        SDL_SCANCODE_KP_00 = 176,
        SDL_SCANCODE_KP_000 = 177,
        SDL_SCANCODE_THOUSANDSSEPARATOR = 178,
        SDL_SCANCODE_DECIMALSEPARATOR = 179,
        SDL_SCANCODE_CURRENCYUNIT = 180,
        SDL_SCANCODE_CURRENCYSUBUNIT = 181,
        SDL_SCANCODE_KP_LEFTPAREN = 182,
        SDL_SCANCODE_KP_RIGHTPAREN = 183,
        SDL_SCANCODE_KP_LEFTBRACE = 184,
        SDL_SCANCODE_KP_RIGHTBRACE = 185,
        SDL_SCANCODE_KP_TAB = 186,
        SDL_SCANCODE_KP_BACKSPACE = 187,
        SDL_SCANCODE_KP_A = 188,
        SDL_SCANCODE_KP_B = 189,
        SDL_SCANCODE_KP_C = 190,
        SDL_SCANCODE_KP_D = 191,
        SDL_SCANCODE_KP_E = 192,
        SDL_SCANCODE_KP_F = 193,
        SDL_SCANCODE_KP_XOR = 194,
        SDL_SCANCODE_KP_POWER = 195,
        SDL_SCANCODE_KP_PERCENT = 196,
        SDL_SCANCODE_KP_LESS = 197,
        SDL_SCANCODE_KP_GREATER = 198,
        SDL_SCANCODE_KP_AMPERSAND = 199,
        SDL_SCANCODE_KP_DBLAMPERSAND = 200,
        SDL_SCANCODE_KP_VERTICALBAR = 201,
        SDL_SCANCODE_KP_DBLVERTICALBAR = 202,
        SDL_SCANCODE_KP_COLON = 203,
        SDL_SCANCODE_KP_HASH = 204,
        SDL_SCANCODE_KP_SPACE = 205,
        SDL_SCANCODE_KP_AT = 206,
        SDL_SCANCODE_KP_EXCLAM = 207,
        SDL_SCANCODE_KP_MEMSTORE = 208,
        SDL_SCANCODE_KP_MEMRECALL = 209,
        SDL_SCANCODE_KP_MEMCLEAR = 210,
        SDL_SCANCODE_KP_MEMADD = 211,
        SDL_SCANCODE_KP_MEMSUBTRACT = 212,
        SDL_SCANCODE_KP_MEMMULTIPLY = 213,
        SDL_SCANCODE_KP_MEMDIVIDE = 214,
        SDL_SCANCODE_KP_PLUSMINUS = 215,
        SDL_SCANCODE_KP_CLEAR = 216,
        SDL_SCANCODE_KP_CLEARENTRY = 217,
        SDL_SCANCODE_KP_BINARY = 218,
        SDL_SCANCODE_KP_OCTAL = 219,
        SDL_SCANCODE_KP_DECIMAL = 220,
        SDL_SCANCODE_KP_HEXADECIMAL = 221,

        SDL_SCANCODE_LCTRL = 224,
        SDL_SCANCODE_LSHIFT = 225,
        SDL_SCANCODE_LALT = 226,
        SDL_SCANCODE_LGUI = 227,
        SDL_SCANCODE_RCTRL = 228,
        SDL_SCANCODE_RSHIFT = 229,
        SDL_SCANCODE_RALT = 230,
        SDL_SCANCODE_RGUI = 231,

        SDL_SCANCODE_MODE = 257,

        /* These come from the USB consumer page (0x0C) */
        SDL_SCANCODE_AUDIONEXT = 258,
        SDL_SCANCODE_AUDIOPREV = 259,
        SDL_SCANCODE_AUDIOSTOP = 260,
        SDL_SCANCODE_AUDIOPLAY = 261,
        SDL_SCANCODE_AUDIOMUTE = 262,
        SDL_SCANCODE_MEDIASELECT = 263,
        SDL_SCANCODE_WWW = 264,
        SDL_SCANCODE_MAIL = 265,
        SDL_SCANCODE_CALCULATOR = 266,
        SDL_SCANCODE_COMPUTER = 267,
        SDL_SCANCODE_AC_SEARCH = 268,
        SDL_SCANCODE_AC_HOME = 269,
        SDL_SCANCODE_AC_BACK = 270,
        SDL_SCANCODE_AC_FORWARD = 271,
        SDL_SCANCODE_AC_STOP = 272,
        SDL_SCANCODE_AC_REFRESH = 273,
        SDL_SCANCODE_AC_BOOKMARKS = 274,

        /* These come from other sources, and are mostly mac related */
        SDL_SCANCODE_BRIGHTNESSDOWN = 275,
        SDL_SCANCODE_BRIGHTNESSUP = 276,
        SDL_SCANCODE_DISPLAYSWITCH = 277,
        SDL_SCANCODE_KBDILLUMTOGGLE = 278,
        SDL_SCANCODE_KBDILLUMDOWN = 279,
        SDL_SCANCODE_KBDILLUMUP = 280,
        SDL_SCANCODE_EJECT = 281,
        SDL_SCANCODE_SLEEP = 282,

        SDL_SCANCODE_APP1 = 283,
        SDL_SCANCODE_APP2 = 284,

        /* These come from the USB consumer page (0x0C) */
        SDL_SCANCODE_AUDIOREWIND = 285,
        SDL_SCANCODE_AUDIOFASTFORWARD = 286,

        /* This is not a key, simply marks the number of scancodes
         * so that you know how big to make your arrays. */
        SDL_NUM_SCANCODES = 512
    }
    #endregion

    #region SDL_keycode.h
    public const int SDLK_SCANCODE_MASK = (1 << 30);
    public static SDL_Keycode SDL_SCANCODE_TO_KEYCODE(SDL_Scancode X)
    {
        return (SDL_Keycode)((int)X | SDLK_SCANCODE_MASK);
    }

    public enum SDL_Keycode
    {
        SDLK_UNKNOWN = 0,

        SDLK_RETURN = '\r',
        SDLK_ESCAPE = 27, // '\033'
        SDLK_BACKSPACE = '\b',
        SDLK_TAB = '\t',
        SDLK_SPACE = ' ',
        SDLK_EXCLAIM = '!',
        SDLK_QUOTEDBL = '"',
        SDLK_HASH = '#',
        SDLK_PERCENT = '%',
        SDLK_DOLLAR = '$',
        SDLK_AMPERSAND = '&',
        SDLK_QUOTE = '\'',
        SDLK_LEFTPAREN = '(',
        SDLK_RIGHTPAREN = ')',
        SDLK_ASTERISK = '*',
        SDLK_PLUS = '+',
        SDLK_COMMA = ',',
        SDLK_MINUS = '-',
        SDLK_PERIOD = '.',
        SDLK_SLASH = '/',
        SDLK_0 = '0',
        SDLK_1 = '1',
        SDLK_2 = '2',
        SDLK_3 = '3',
        SDLK_4 = '4',
        SDLK_5 = '5',
        SDLK_6 = '6',
        SDLK_7 = '7',
        SDLK_8 = '8',
        SDLK_9 = '9',
        SDLK_COLON = ':',
        SDLK_SEMICOLON = ';',
        SDLK_LESS = '<',
        SDLK_EQUALS = '=',
        SDLK_GREATER = '>',
        SDLK_QUESTION = '?',
        SDLK_AT = '@',
        /*
        Skip uppercase letters
        */
        SDLK_LEFTBRACKET = '[',
        SDLK_BACKSLASH = '\\',
        SDLK_RIGHTBRACKET = ']',
        SDLK_CARET = '^',
        SDLK_UNDERSCORE = '_',
        SDLK_BACKQUOTE = '`',
        SDLK_a = 'a',
        SDLK_b = 'b',
        SDLK_c = 'c',
        SDLK_d = 'd',
        SDLK_e = 'e',
        SDLK_f = 'f',
        SDLK_g = 'g',
        SDLK_h = 'h',
        SDLK_i = 'i',
        SDLK_j = 'j',
        SDLK_k = 'k',
        SDLK_l = 'l',
        SDLK_m = 'm',
        SDLK_n = 'n',
        SDLK_o = 'o',
        SDLK_p = 'p',
        SDLK_q = 'q',
        SDLK_r = 'r',
        SDLK_s = 's',
        SDLK_t = 't',
        SDLK_u = 'u',
        SDLK_v = 'v',
        SDLK_w = 'w',
        SDLK_x = 'x',
        SDLK_y = 'y',
        SDLK_z = 'z',

        SDLK_CAPSLOCK = (int)SDL_Scancode.SDL_SCANCODE_CAPSLOCK | SDLK_SCANCODE_MASK,

        SDLK_F1 = (int)SDL_Scancode.SDL_SCANCODE_F1 | SDLK_SCANCODE_MASK,
        SDLK_F2 = (int)SDL_Scancode.SDL_SCANCODE_F2 | SDLK_SCANCODE_MASK,
        SDLK_F3 = (int)SDL_Scancode.SDL_SCANCODE_F3 | SDLK_SCANCODE_MASK,
        SDLK_F4 = (int)SDL_Scancode.SDL_SCANCODE_F4 | SDLK_SCANCODE_MASK,
        SDLK_F5 = (int)SDL_Scancode.SDL_SCANCODE_F5 | SDLK_SCANCODE_MASK,
        SDLK_F6 = (int)SDL_Scancode.SDL_SCANCODE_F6 | SDLK_SCANCODE_MASK,
        SDLK_F7 = (int)SDL_Scancode.SDL_SCANCODE_F7 | SDLK_SCANCODE_MASK,
        SDLK_F8 = (int)SDL_Scancode.SDL_SCANCODE_F8 | SDLK_SCANCODE_MASK,
        SDLK_F9 = (int)SDL_Scancode.SDL_SCANCODE_F9 | SDLK_SCANCODE_MASK,
        SDLK_F10 = (int)SDL_Scancode.SDL_SCANCODE_F10 | SDLK_SCANCODE_MASK,
        SDLK_F11 = (int)SDL_Scancode.SDL_SCANCODE_F11 | SDLK_SCANCODE_MASK,
        SDLK_F12 = (int)SDL_Scancode.SDL_SCANCODE_F12 | SDLK_SCANCODE_MASK,

        SDLK_PRINTSCREEN = (int)SDL_Scancode.SDL_SCANCODE_PRINTSCREEN | SDLK_SCANCODE_MASK,
        SDLK_SCROLLLOCK = (int)SDL_Scancode.SDL_SCANCODE_SCROLLLOCK | SDLK_SCANCODE_MASK,
        SDLK_PAUSE = (int)SDL_Scancode.SDL_SCANCODE_PAUSE | SDLK_SCANCODE_MASK,
        SDLK_INSERT = (int)SDL_Scancode.SDL_SCANCODE_INSERT | SDLK_SCANCODE_MASK,
        SDLK_HOME = (int)SDL_Scancode.SDL_SCANCODE_HOME | SDLK_SCANCODE_MASK,
        SDLK_PAGEUP = (int)SDL_Scancode.SDL_SCANCODE_PAGEUP | SDLK_SCANCODE_MASK,
        SDLK_DELETE = 127,
        SDLK_END = (int)SDL_Scancode.SDL_SCANCODE_END | SDLK_SCANCODE_MASK,
        SDLK_PAGEDOWN = (int)SDL_Scancode.SDL_SCANCODE_PAGEDOWN | SDLK_SCANCODE_MASK,
        SDLK_RIGHT = (int)SDL_Scancode.SDL_SCANCODE_RIGHT | SDLK_SCANCODE_MASK,
        SDLK_LEFT = (int)SDL_Scancode.SDL_SCANCODE_LEFT | SDLK_SCANCODE_MASK,
        SDLK_DOWN = (int)SDL_Scancode.SDL_SCANCODE_DOWN | SDLK_SCANCODE_MASK,
        SDLK_UP = (int)SDL_Scancode.SDL_SCANCODE_UP | SDLK_SCANCODE_MASK,

        SDLK_NUMLOCKCLEAR = (int)SDL_Scancode.SDL_SCANCODE_NUMLOCKCLEAR | SDLK_SCANCODE_MASK,
        SDLK_KP_DIVIDE = (int)SDL_Scancode.SDL_SCANCODE_KP_DIVIDE | SDLK_SCANCODE_MASK,
        SDLK_KP_MULTIPLY = (int)SDL_Scancode.SDL_SCANCODE_KP_MULTIPLY | SDLK_SCANCODE_MASK,
        SDLK_KP_MINUS = (int)SDL_Scancode.SDL_SCANCODE_KP_MINUS | SDLK_SCANCODE_MASK,
        SDLK_KP_PLUS = (int)SDL_Scancode.SDL_SCANCODE_KP_PLUS | SDLK_SCANCODE_MASK,
        SDLK_KP_ENTER = (int)SDL_Scancode.SDL_SCANCODE_KP_ENTER | SDLK_SCANCODE_MASK,
        SDLK_KP_1 = (int)SDL_Scancode.SDL_SCANCODE_KP_1 | SDLK_SCANCODE_MASK,
        SDLK_KP_2 = (int)SDL_Scancode.SDL_SCANCODE_KP_2 | SDLK_SCANCODE_MASK,
        SDLK_KP_3 = (int)SDL_Scancode.SDL_SCANCODE_KP_3 | SDLK_SCANCODE_MASK,
        SDLK_KP_4 = (int)SDL_Scancode.SDL_SCANCODE_KP_4 | SDLK_SCANCODE_MASK,
        SDLK_KP_5 = (int)SDL_Scancode.SDL_SCANCODE_KP_5 | SDLK_SCANCODE_MASK,
        SDLK_KP_6 = (int)SDL_Scancode.SDL_SCANCODE_KP_6 | SDLK_SCANCODE_MASK,
        SDLK_KP_7 = (int)SDL_Scancode.SDL_SCANCODE_KP_7 | SDLK_SCANCODE_MASK,
        SDLK_KP_8 = (int)SDL_Scancode.SDL_SCANCODE_KP_8 | SDLK_SCANCODE_MASK,
        SDLK_KP_9 = (int)SDL_Scancode.SDL_SCANCODE_KP_9 | SDLK_SCANCODE_MASK,
        SDLK_KP_0 = (int)SDL_Scancode.SDL_SCANCODE_KP_0 | SDLK_SCANCODE_MASK,
        SDLK_KP_PERIOD = (int)SDL_Scancode.SDL_SCANCODE_KP_PERIOD | SDLK_SCANCODE_MASK,

        SDLK_APPLICATION = (int)SDL_Scancode.SDL_SCANCODE_APPLICATION | SDLK_SCANCODE_MASK,
        SDLK_POWER = (int)SDL_Scancode.SDL_SCANCODE_POWER | SDLK_SCANCODE_MASK,
        SDLK_KP_EQUALS = (int)SDL_Scancode.SDL_SCANCODE_KP_EQUALS | SDLK_SCANCODE_MASK,
        SDLK_F13 = (int)SDL_Scancode.SDL_SCANCODE_F13 | SDLK_SCANCODE_MASK,
        SDLK_F14 = (int)SDL_Scancode.SDL_SCANCODE_F14 | SDLK_SCANCODE_MASK,
        SDLK_F15 = (int)SDL_Scancode.SDL_SCANCODE_F15 | SDLK_SCANCODE_MASK,
        SDLK_F16 = (int)SDL_Scancode.SDL_SCANCODE_F16 | SDLK_SCANCODE_MASK,
        SDLK_F17 = (int)SDL_Scancode.SDL_SCANCODE_F17 | SDLK_SCANCODE_MASK,
        SDLK_F18 = (int)SDL_Scancode.SDL_SCANCODE_F18 | SDLK_SCANCODE_MASK,
        SDLK_F19 = (int)SDL_Scancode.SDL_SCANCODE_F19 | SDLK_SCANCODE_MASK,
        SDLK_F20 = (int)SDL_Scancode.SDL_SCANCODE_F20 | SDLK_SCANCODE_MASK,
        SDLK_F21 = (int)SDL_Scancode.SDL_SCANCODE_F21 | SDLK_SCANCODE_MASK,
        SDLK_F22 = (int)SDL_Scancode.SDL_SCANCODE_F22 | SDLK_SCANCODE_MASK,
        SDLK_F23 = (int)SDL_Scancode.SDL_SCANCODE_F23 | SDLK_SCANCODE_MASK,
        SDLK_F24 = (int)SDL_Scancode.SDL_SCANCODE_F24 | SDLK_SCANCODE_MASK,
        SDLK_EXECUTE = (int)SDL_Scancode.SDL_SCANCODE_EXECUTE | SDLK_SCANCODE_MASK,
        SDLK_HELP = (int)SDL_Scancode.SDL_SCANCODE_HELP | SDLK_SCANCODE_MASK,
        SDLK_MENU = (int)SDL_Scancode.SDL_SCANCODE_MENU | SDLK_SCANCODE_MASK,
        SDLK_SELECT = (int)SDL_Scancode.SDL_SCANCODE_SELECT | SDLK_SCANCODE_MASK,
        SDLK_STOP = (int)SDL_Scancode.SDL_SCANCODE_STOP | SDLK_SCANCODE_MASK,
        SDLK_AGAIN = (int)SDL_Scancode.SDL_SCANCODE_AGAIN | SDLK_SCANCODE_MASK,
        SDLK_UNDO = (int)SDL_Scancode.SDL_SCANCODE_UNDO | SDLK_SCANCODE_MASK,
        SDLK_CUT = (int)SDL_Scancode.SDL_SCANCODE_CUT | SDLK_SCANCODE_MASK,
        SDLK_COPY = (int)SDL_Scancode.SDL_SCANCODE_COPY | SDLK_SCANCODE_MASK,
        SDLK_PASTE = (int)SDL_Scancode.SDL_SCANCODE_PASTE | SDLK_SCANCODE_MASK,
        SDLK_FIND = (int)SDL_Scancode.SDL_SCANCODE_FIND | SDLK_SCANCODE_MASK,
        SDLK_MUTE = (int)SDL_Scancode.SDL_SCANCODE_MUTE | SDLK_SCANCODE_MASK,
        SDLK_VOLUMEUP = (int)SDL_Scancode.SDL_SCANCODE_VOLUMEUP | SDLK_SCANCODE_MASK,
        SDLK_VOLUMEDOWN = (int)SDL_Scancode.SDL_SCANCODE_VOLUMEDOWN | SDLK_SCANCODE_MASK,
        SDLK_KP_COMMA = (int)SDL_Scancode.SDL_SCANCODE_KP_COMMA | SDLK_SCANCODE_MASK,
        SDLK_KP_EQUALSAS400 =
        (int)SDL_Scancode.SDL_SCANCODE_KP_EQUALSAS400 | SDLK_SCANCODE_MASK,

        SDLK_ALTERASE = (int)SDL_Scancode.SDL_SCANCODE_ALTERASE | SDLK_SCANCODE_MASK,
        SDLK_SYSREQ = (int)SDL_Scancode.SDL_SCANCODE_SYSREQ | SDLK_SCANCODE_MASK,
        SDLK_CANCEL = (int)SDL_Scancode.SDL_SCANCODE_CANCEL | SDLK_SCANCODE_MASK,
        SDLK_CLEAR = (int)SDL_Scancode.SDL_SCANCODE_CLEAR | SDLK_SCANCODE_MASK,
        SDLK_PRIOR = (int)SDL_Scancode.SDL_SCANCODE_PRIOR | SDLK_SCANCODE_MASK,
        SDLK_RETURN2 = (int)SDL_Scancode.SDL_SCANCODE_RETURN2 | SDLK_SCANCODE_MASK,
        SDLK_SEPARATOR = (int)SDL_Scancode.SDL_SCANCODE_SEPARATOR | SDLK_SCANCODE_MASK,
        SDLK_OUT = (int)SDL_Scancode.SDL_SCANCODE_OUT | SDLK_SCANCODE_MASK,
        SDLK_OPER = (int)SDL_Scancode.SDL_SCANCODE_OPER | SDLK_SCANCODE_MASK,
        SDLK_CLEARAGAIN = (int)SDL_Scancode.SDL_SCANCODE_CLEARAGAIN | SDLK_SCANCODE_MASK,
        SDLK_CRSEL = (int)SDL_Scancode.SDL_SCANCODE_CRSEL | SDLK_SCANCODE_MASK,
        SDLK_EXSEL = (int)SDL_Scancode.SDL_SCANCODE_EXSEL | SDLK_SCANCODE_MASK,

        SDLK_KP_00 = (int)SDL_Scancode.SDL_SCANCODE_KP_00 | SDLK_SCANCODE_MASK,
        SDLK_KP_000 = (int)SDL_Scancode.SDL_SCANCODE_KP_000 | SDLK_SCANCODE_MASK,
        SDLK_THOUSANDSSEPARATOR =
        (int)SDL_Scancode.SDL_SCANCODE_THOUSANDSSEPARATOR | SDLK_SCANCODE_MASK,
        SDLK_DECIMALSEPARATOR =
        (int)SDL_Scancode.SDL_SCANCODE_DECIMALSEPARATOR | SDLK_SCANCODE_MASK,
        SDLK_CURRENCYUNIT = (int)SDL_Scancode.SDL_SCANCODE_CURRENCYUNIT | SDLK_SCANCODE_MASK,
        SDLK_CURRENCYSUBUNIT =
        (int)SDL_Scancode.SDL_SCANCODE_CURRENCYSUBUNIT | SDLK_SCANCODE_MASK,
        SDLK_KP_LEFTPAREN = (int)SDL_Scancode.SDL_SCANCODE_KP_LEFTPAREN | SDLK_SCANCODE_MASK,
        SDLK_KP_RIGHTPAREN = (int)SDL_Scancode.SDL_SCANCODE_KP_RIGHTPAREN | SDLK_SCANCODE_MASK,
        SDLK_KP_LEFTBRACE = (int)SDL_Scancode.SDL_SCANCODE_KP_LEFTBRACE | SDLK_SCANCODE_MASK,
        SDLK_KP_RIGHTBRACE = (int)SDL_Scancode.SDL_SCANCODE_KP_RIGHTBRACE | SDLK_SCANCODE_MASK,
        SDLK_KP_TAB = (int)SDL_Scancode.SDL_SCANCODE_KP_TAB | SDLK_SCANCODE_MASK,
        SDLK_KP_BACKSPACE = (int)SDL_Scancode.SDL_SCANCODE_KP_BACKSPACE | SDLK_SCANCODE_MASK,
        SDLK_KP_A = (int)SDL_Scancode.SDL_SCANCODE_KP_A | SDLK_SCANCODE_MASK,
        SDLK_KP_B = (int)SDL_Scancode.SDL_SCANCODE_KP_B | SDLK_SCANCODE_MASK,
        SDLK_KP_C = (int)SDL_Scancode.SDL_SCANCODE_KP_C | SDLK_SCANCODE_MASK,
        SDLK_KP_D = (int)SDL_Scancode.SDL_SCANCODE_KP_D | SDLK_SCANCODE_MASK,
        SDLK_KP_E = (int)SDL_Scancode.SDL_SCANCODE_KP_E | SDLK_SCANCODE_MASK,
        SDLK_KP_F = (int)SDL_Scancode.SDL_SCANCODE_KP_F | SDLK_SCANCODE_MASK,
        SDLK_KP_XOR = (int)SDL_Scancode.SDL_SCANCODE_KP_XOR | SDLK_SCANCODE_MASK,
        SDLK_KP_POWER = (int)SDL_Scancode.SDL_SCANCODE_KP_POWER | SDLK_SCANCODE_MASK,
        SDLK_KP_PERCENT = (int)SDL_Scancode.SDL_SCANCODE_KP_PERCENT | SDLK_SCANCODE_MASK,
        SDLK_KP_LESS = (int)SDL_Scancode.SDL_SCANCODE_KP_LESS | SDLK_SCANCODE_MASK,
        SDLK_KP_GREATER = (int)SDL_Scancode.SDL_SCANCODE_KP_GREATER | SDLK_SCANCODE_MASK,
        SDLK_KP_AMPERSAND = (int)SDL_Scancode.SDL_SCANCODE_KP_AMPERSAND | SDLK_SCANCODE_MASK,
        SDLK_KP_DBLAMPERSAND =
        (int)SDL_Scancode.SDL_SCANCODE_KP_DBLAMPERSAND | SDLK_SCANCODE_MASK,
        SDLK_KP_VERTICALBAR =
        (int)SDL_Scancode.SDL_SCANCODE_KP_VERTICALBAR | SDLK_SCANCODE_MASK,
        SDLK_KP_DBLVERTICALBAR =
        (int)SDL_Scancode.SDL_SCANCODE_KP_DBLVERTICALBAR | SDLK_SCANCODE_MASK,
        SDLK_KP_COLON = (int)SDL_Scancode.SDL_SCANCODE_KP_COLON | SDLK_SCANCODE_MASK,
        SDLK_KP_HASH = (int)SDL_Scancode.SDL_SCANCODE_KP_HASH | SDLK_SCANCODE_MASK,
        SDLK_KP_SPACE = (int)SDL_Scancode.SDL_SCANCODE_KP_SPACE | SDLK_SCANCODE_MASK,
        SDLK_KP_AT = (int)SDL_Scancode.SDL_SCANCODE_KP_AT | SDLK_SCANCODE_MASK,
        SDLK_KP_EXCLAM = (int)SDL_Scancode.SDL_SCANCODE_KP_EXCLAM | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMSTORE = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMSTORE | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMRECALL = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMRECALL | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMCLEAR = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMCLEAR | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMADD = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMADD | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMSUBTRACT =
        (int)SDL_Scancode.SDL_SCANCODE_KP_MEMSUBTRACT | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMMULTIPLY =
        (int)SDL_Scancode.SDL_SCANCODE_KP_MEMMULTIPLY | SDLK_SCANCODE_MASK,
        SDLK_KP_MEMDIVIDE = (int)SDL_Scancode.SDL_SCANCODE_KP_MEMDIVIDE | SDLK_SCANCODE_MASK,
        SDLK_KP_PLUSMINUS = (int)SDL_Scancode.SDL_SCANCODE_KP_PLUSMINUS | SDLK_SCANCODE_MASK,
        SDLK_KP_CLEAR = (int)SDL_Scancode.SDL_SCANCODE_KP_CLEAR | SDLK_SCANCODE_MASK,
        SDLK_KP_CLEARENTRY = (int)SDL_Scancode.SDL_SCANCODE_KP_CLEARENTRY | SDLK_SCANCODE_MASK,
        SDLK_KP_BINARY = (int)SDL_Scancode.SDL_SCANCODE_KP_BINARY | SDLK_SCANCODE_MASK,
        SDLK_KP_OCTAL = (int)SDL_Scancode.SDL_SCANCODE_KP_OCTAL | SDLK_SCANCODE_MASK,
        SDLK_KP_DECIMAL = (int)SDL_Scancode.SDL_SCANCODE_KP_DECIMAL | SDLK_SCANCODE_MASK,
        SDLK_KP_HEXADECIMAL =
        (int)SDL_Scancode.SDL_SCANCODE_KP_HEXADECIMAL | SDLK_SCANCODE_MASK,

        SDLK_LCTRL = (int)SDL_Scancode.SDL_SCANCODE_LCTRL | SDLK_SCANCODE_MASK,
        SDLK_LSHIFT = (int)SDL_Scancode.SDL_SCANCODE_LSHIFT | SDLK_SCANCODE_MASK,
        SDLK_LALT = (int)SDL_Scancode.SDL_SCANCODE_LALT | SDLK_SCANCODE_MASK,
        SDLK_LGUI = (int)SDL_Scancode.SDL_SCANCODE_LGUI | SDLK_SCANCODE_MASK,
        SDLK_RCTRL = (int)SDL_Scancode.SDL_SCANCODE_RCTRL | SDLK_SCANCODE_MASK,
        SDLK_RSHIFT = (int)SDL_Scancode.SDL_SCANCODE_RSHIFT | SDLK_SCANCODE_MASK,
        SDLK_RALT = (int)SDL_Scancode.SDL_SCANCODE_RALT | SDLK_SCANCODE_MASK,
        SDLK_RGUI = (int)SDL_Scancode.SDL_SCANCODE_RGUI | SDLK_SCANCODE_MASK,

        SDLK_MODE = (int)SDL_Scancode.SDL_SCANCODE_MODE | SDLK_SCANCODE_MASK,

        SDLK_AUDIONEXT = (int)SDL_Scancode.SDL_SCANCODE_AUDIONEXT | SDLK_SCANCODE_MASK,
        SDLK_AUDIOPREV = (int)SDL_Scancode.SDL_SCANCODE_AUDIOPREV | SDLK_SCANCODE_MASK,
        SDLK_AUDIOSTOP = (int)SDL_Scancode.SDL_SCANCODE_AUDIOSTOP | SDLK_SCANCODE_MASK,
        SDLK_AUDIOPLAY = (int)SDL_Scancode.SDL_SCANCODE_AUDIOPLAY | SDLK_SCANCODE_MASK,
        SDLK_AUDIOMUTE = (int)SDL_Scancode.SDL_SCANCODE_AUDIOMUTE | SDLK_SCANCODE_MASK,
        SDLK_MEDIASELECT = (int)SDL_Scancode.SDL_SCANCODE_MEDIASELECT | SDLK_SCANCODE_MASK,
        SDLK_WWW = (int)SDL_Scancode.SDL_SCANCODE_WWW | SDLK_SCANCODE_MASK,
        SDLK_MAIL = (int)SDL_Scancode.SDL_SCANCODE_MAIL | SDLK_SCANCODE_MASK,
        SDLK_CALCULATOR = (int)SDL_Scancode.SDL_SCANCODE_CALCULATOR | SDLK_SCANCODE_MASK,
        SDLK_COMPUTER = (int)SDL_Scancode.SDL_SCANCODE_COMPUTER | SDLK_SCANCODE_MASK,
        SDLK_AC_SEARCH = (int)SDL_Scancode.SDL_SCANCODE_AC_SEARCH | SDLK_SCANCODE_MASK,
        SDLK_AC_HOME = (int)SDL_Scancode.SDL_SCANCODE_AC_HOME | SDLK_SCANCODE_MASK,
        SDLK_AC_BACK = (int)SDL_Scancode.SDL_SCANCODE_AC_BACK | SDLK_SCANCODE_MASK,
        SDLK_AC_FORWARD = (int)SDL_Scancode.SDL_SCANCODE_AC_FORWARD | SDLK_SCANCODE_MASK,
        SDLK_AC_STOP = (int)SDL_Scancode.SDL_SCANCODE_AC_STOP | SDLK_SCANCODE_MASK,
        SDLK_AC_REFRESH = (int)SDL_Scancode.SDL_SCANCODE_AC_REFRESH | SDLK_SCANCODE_MASK,
        SDLK_AC_BOOKMARKS = (int)SDL_Scancode.SDL_SCANCODE_AC_BOOKMARKS | SDLK_SCANCODE_MASK,

        SDLK_BRIGHTNESSDOWN =
        (int)SDL_Scancode.SDL_SCANCODE_BRIGHTNESSDOWN | SDLK_SCANCODE_MASK,
        SDLK_BRIGHTNESSUP = (int)SDL_Scancode.SDL_SCANCODE_BRIGHTNESSUP | SDLK_SCANCODE_MASK,
        SDLK_DISPLAYSWITCH = (int)SDL_Scancode.SDL_SCANCODE_DISPLAYSWITCH | SDLK_SCANCODE_MASK,
        SDLK_KBDILLUMTOGGLE =
        (int)SDL_Scancode.SDL_SCANCODE_KBDILLUMTOGGLE | SDLK_SCANCODE_MASK,
        SDLK_KBDILLUMDOWN = (int)SDL_Scancode.SDL_SCANCODE_KBDILLUMDOWN | SDLK_SCANCODE_MASK,
        SDLK_KBDILLUMUP = (int)SDL_Scancode.SDL_SCANCODE_KBDILLUMUP | SDLK_SCANCODE_MASK,
        SDLK_EJECT = (int)SDL_Scancode.SDL_SCANCODE_EJECT | SDLK_SCANCODE_MASK,
        SDLK_SLEEP = (int)SDL_Scancode.SDL_SCANCODE_SLEEP | SDLK_SCANCODE_MASK,
        SDLK_APP1 = (int)SDL_Scancode.SDL_SCANCODE_APP1 | SDLK_SCANCODE_MASK,
        SDLK_APP2 = (int)SDL_Scancode.SDL_SCANCODE_APP2 | SDLK_SCANCODE_MASK,

        SDLK_AUDIOREWIND = (int)SDL_Scancode.SDL_SCANCODE_AUDIOREWIND | SDLK_SCANCODE_MASK,
        SDLK_AUDIOFASTFORWARD = (int)SDL_Scancode.SDL_SCANCODE_AUDIOFASTFORWARD | SDLK_SCANCODE_MASK
    }

    /// <summary>
    /// Key modifiers
    /// </summary>
    [Flags]
    public enum SDL_Keymod : ushort
    {
        SDL_KMOD_NONE = 0x0000,
        SDL_KMOD_LSHIFT = 0x0001,
        SDL_KMOD_RSHIFT = 0x0002,
        SDL_KMOD_LCTRL = 0x0040,
        SDL_KMOD_RCTRL = 0x0080,
        SDL_KMOD_LALT = 0x0100,
        SDL_KMOD_RALT = 0x0200,
        SDL_KMOD_LGUI = 0x0400,
        SDL_KMOD_RGUI = 0x0800,
        SDL_KMOD_NUM = 0x1000,
        SDL_KMOD_CAPS = 0x2000,
        SDL_KMOD_MODE = 0x4000,
        SDL_KMOD_SCROLL = 0x8000,

        SDL_KMOD_CTRL = SDL_KMOD_LCTRL | SDL_KMOD_RCTRL,
        SDL_KMOD_SHIFT = SDL_KMOD_LSHIFT | SDL_KMOD_RSHIFT,
        SDL_KMOD_ALT = SDL_KMOD_LALT | SDL_KMOD_RALT,
        SDL_KMOD_GUI = SDL_KMOD_LGUI | SDL_KMOD_RGUI,

        SDL_KMOD_RESERVED = SDL_KMOD_SCROLL /* This is for source-level compatibility with SDL 2.0.0. */
    }
    #endregion

    #region SDL_keyboard.h
    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_Keysym
    {
        public SDL_Scancode scancode;
        public SDL_Keycode sym;
        public SDL_Keymod mod; /* UInt16 */
        public uint unicode; /* Deprecated */
    }

    private static readonly delegate* unmanaged[Cdecl]<nint> s_SDL_GetKeyboardFocus = (delegate* unmanaged[Cdecl]<nint>)LoadFunction(nameof(SDL_GetKeyboardFocus));
    private static readonly delegate* unmanaged[Cdecl]<out int, nint> s_SDL_GetKeyboardState = (delegate* unmanaged[Cdecl]<out int, nint>)LoadFunction(nameof(SDL_GetKeyboardState));
    private static readonly delegate* unmanaged[Cdecl]<SDL_Keymod> s_SDL_GetModState = (delegate* unmanaged[Cdecl]<SDL_Keymod>)LoadFunction(nameof(SDL_GetModState));

    /* Get the window which has kbd focus */
    /* Return type is an SDL_Window pointer */
    public static nint SDL_GetKeyboardFocus() => s_SDL_GetKeyboardFocus();

    /* Get a snapshot of the keyboard state. */
    /* Return value is a pointer to a UInt8 array */
    /* Numkeys returns the size of the array if non-null */
    public static nint SDL_GetKeyboardState(out int numKeys) => s_SDL_GetKeyboardState(out numKeys);

    /// <summary>
    /// Get the current key modifier state for the keyboard. 
    /// </summary>
    /// <returns></returns>
    public static SDL_Keymod SDL_GetModState() => s_SDL_GetModState();

    /* Set the current key modifier state */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetModState(SDL_Keymod modstate);

    /* Get the key code corresponding to the given scancode
     * with the current keyboard layout.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Keycode SDL_GetKeyFromScancode(SDL_Scancode scancode);

    /* Get the scancode for the given keycode */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Scancode SDL_GetScancodeFromKey(SDL_Keycode key);

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
    private static extern byte* INTERNAL_SDL_GetKeyName(SDL_Keycode key);
    public static string SDL_GetKeyName(SDL_Keycode key)
    {
        return GetString(INTERNAL_SDL_GetKeyName(key));
    }

    /* Get a key code from a human-readable name */
    [DllImport(LibName, EntryPoint = "SDL_GetKeyFromName", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe SDL_Keycode INTERNAL_SDL_GetKeyFromName(
        byte* name
    );
    public static unsafe SDL_Keycode SDL_GetKeyFromName(string name)
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
    public static extern SDL_bool SDL_IsTextInputActive();

    /* Stop receiving any text input events, hide onscreen kbd */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_StopTextInput();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_ClearComposition();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_IsTextInputShown();

    /* Set the rectangle used for text input, hint for IME */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetTextInputRect(ref Rectangle rect);

    /* Does the platform support an on-screen keyboard? */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_HasScreenKeyboardSupport();

    /* Is the on-screen keyboard shown for a given window? */
    /* window is an SDL_Window pointer */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_IsScreenKeyboardShown(SDL_Window window);
    #endregion

    #region SDL_mouse.c
    private static readonly delegate* unmanaged[Cdecl]<SDL_Window> s_SDL_GetMouseFocus = (delegate* unmanaged[Cdecl]<SDL_Window>)LoadFunction(nameof(SDL_GetMouseFocus));
    private static readonly delegate* unmanaged[Cdecl]<out int, out int, uint> s_SDL_GetMouseState = (delegate* unmanaged[Cdecl]<out int, out int, uint>)LoadFunction(nameof(SDL_GetMouseState));
    private static readonly delegate* unmanaged[Cdecl]<out int, out int, uint> s_SDL_GetGlobalMouseState = (delegate* unmanaged[Cdecl]<out int, out int, uint>)LoadFunction(nameof(SDL_GetGlobalMouseState));

    private static readonly delegate* unmanaged[Cdecl]<SDL_bool, int> s_SDL_CaptureMouse = (delegate* unmanaged[Cdecl]<SDL_bool, int>)LoadFunction(nameof(SDL_CaptureMouse));
    private static readonly delegate* unmanaged[Cdecl]<SDL_bool> s_SDL_GetRelativeMouseMode = (delegate* unmanaged[Cdecl]<SDL_bool>)LoadFunction(nameof(SDL_GetRelativeMouseMode));
    private static readonly delegate* unmanaged[Cdecl]<void*, void*, int, int, int, int, SDL_Cursor> s_SDL_CreateCursor = (delegate* unmanaged[Cdecl]<void*, void*, int, int, int, int, SDL_Cursor>)LoadFunction(nameof(SDL_CreateCursor));

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

    /* Get the window which currently has mouse focus */
    /* Return value is an SDL_Window pointer */
    public static SDL_Window SDL_GetMouseFocus() => s_SDL_GetMouseFocus();

    /// <summary>
    /// Get the current state of the mouse
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static uint SDL_GetMouseState(out int x, out int y) => s_SDL_GetMouseState(out x, out y);

    /* Get the current state of the mouse, in relation to the desktop.
     * Only available in 2.0.4 or higher.
     */
    public static uint SDL_GetGlobalMouseState(out int x, out int y) => s_SDL_GetGlobalMouseState(out x, out y);

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
    public static int SDL_CaptureMouse(bool enabled) => s_SDL_CaptureMouse(enabled ? SDL_TRUE : SDL_FALSE);

    /// <summary>
    /// Query if the relative mouse mode is enabled
    /// </summary>
    /// <returns></returns>
    public static bool SDL_GetRelativeMouseMode() => s_SDL_GetRelativeMouseMode() == SDL_TRUE;

    /* Create a cursor from bitmap data (amd mask) in MSB format.
     * data and mask are byte arrays, and w must be a multiple of 8.
     * return value is an SDL_Cursor pointer.
     */
    public static SDL_Cursor SDL_CreateCursor(
        IntPtr data,
        IntPtr mask,
        int w,
        int h,
        int hot_x,
        int hot_y
    )
    {
        return s_SDL_CreateCursor(data.ToPointer(), mask.ToPointer(), w, h, hot_x, hot_y);
    }

    /* Create a cursor from an SDL_Surface.
     * IntPtr refers to an SDL_Cursor*, surface to an SDL_Surface*
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Cursor SDL_CreateColorCursor(
        IntPtr surface,
        int hot_x,
        int hot_y
    );

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
    #region SDL_touch.h

    public const uint SDL_TOUCH_MOUSEID = uint.MaxValue;

    public struct SDL_Finger
    {
        public long id; // SDL_FingerID
        public float x;
        public float y;
        public float pressure;
    }

    /* Only available in 2.0.10 or higher. */
    public enum SDL_TouchDeviceType
    {
        SDL_TOUCH_DEVICE_INVALID = -1,
        SDL_TOUCH_DEVICE_DIRECT,            /* touch screen with window-relative coordinates */
        SDL_TOUCH_DEVICE_INDIRECT_ABSOLUTE, /* trackpad with absolute device coordinates */
        SDL_TOUCH_DEVICE_INDIRECT_RELATIVE  /* trackpad with screen cursor-relative coordinates */
    }

    private static readonly delegate* unmanaged[Cdecl]<int> s_SDL_GetNumTouchDevices = (delegate* unmanaged[Cdecl]<int>)LoadFunction(nameof(SDL_GetNumTouchDevices));
    private static readonly delegate* unmanaged[Cdecl]<int, byte*> s_SDL_GetTouchName = (delegate* unmanaged[Cdecl]<int, byte*>)LoadFunction(nameof(SDL_GetTouchName));

    /// <summary>
    /// Get the number of registered touch devices.
    /// </summary>
    /// <returns></returns>
    public static int SDL_GetNumTouchDevices() => s_SDL_GetNumTouchDevices();

    /**
     *  \brief Get the touch ID with the given index, or 0 if the index is invalid.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern long SDL_GetTouchDevice(int index);

    /**
     *  \brief Get the number of active fingers for a given touch device.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumTouchFingers(long touchID);

    /**
     *  \brief Get the finger object of the given touch, with the given index.
     *  Returns pointer to SDL_Finger.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GetTouchFinger(long touchID, int index);

    /* Only available in 2.0.10 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_TouchDeviceType SDL_GetTouchDeviceType(long touchID);

    /* Only available in 2.0.22 or higher. */
    public static string SDL_GetTouchName(int index)
    {
        return GetString(s_SDL_GetTouchName(index));
    }
    #endregion

    #region SDL_joystick.h

    public const byte SDL_HAT_CENTERED = 0x00;
    public const byte SDL_HAT_UP = 0x01;
    public const byte SDL_HAT_RIGHT = 0x02;
    public const byte SDL_HAT_DOWN = 0x04;
    public const byte SDL_HAT_LEFT = 0x08;
    public const byte SDL_HAT_RIGHTUP = SDL_HAT_RIGHT | SDL_HAT_UP;
    public const byte SDL_HAT_RIGHTDOWN = SDL_HAT_RIGHT | SDL_HAT_DOWN;
    public const byte SDL_HAT_LEFTUP = SDL_HAT_LEFT | SDL_HAT_UP;
    public const byte SDL_HAT_LEFTDOWN = SDL_HAT_LEFT | SDL_HAT_DOWN;

    public enum SDL_JoystickPowerLevel
    {
        SDL_JOYSTICK_POWER_UNKNOWN = -1,
        SDL_JOYSTICK_POWER_EMPTY,
        SDL_JOYSTICK_POWER_LOW,
        SDL_JOYSTICK_POWER_MEDIUM,
        SDL_JOYSTICK_POWER_FULL,
        SDL_JOYSTICK_POWER_WIRED,
        SDL_JOYSTICK_POWER_MAX
    }

    public enum SDL_JoystickType
    {
        SDL_JOYSTICK_TYPE_UNKNOWN,
        SDL_JOYSTICK_TYPE_GAMECONTROLLER,
        SDL_JOYSTICK_TYPE_WHEEL,
        SDL_JOYSTICK_TYPE_ARCADE_STICK,
        SDL_JOYSTICK_TYPE_FLIGHT_STICK,
        SDL_JOYSTICK_TYPE_DANCE_PAD,
        SDL_JOYSTICK_TYPE_GUITAR,
        SDL_JOYSTICK_TYPE_DRUM_KIT,
        SDL_JOYSTICK_TYPE_ARCADE_PAD
    }

    /* Only available in 2.0.14 or higher. */
    public const float SDL_IPHONE_MAX_GFORCE = 5.0f;

    /* joystick refers to an SDL_Joystick*.
    * Only available in 2.0.9 or higher.
    */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickRumble(
        SDL_Joystick joystick,
        ushort low_frequency_rumble,
        ushort high_frequency_rumble,
        uint duration_ms
    );

    /* joystick refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickRumbleTriggers(
        SDL_Joystick joystick,
        ushort left_rumble,
        ushort right_rumble,
        uint duration_ms
    );

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_JoystickClose(SDL_Joystick joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickEventState(int state);

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern short SDL_JoystickGetAxis(
        SDL_Joystick joystick,
        int axis
    );

    /* joystick refers to an SDL_Joystick*.
     * Only available in 2.0.6 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickGetAxisInitialState(
        SDL_Joystick joystick,
        int axis,
        out short state
    );

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickGetBall(
        SDL_Joystick joystick,
        int ball,
        out int dx,
        out int dy
    );

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte SDL_JoystickGetButton(
        SDL_Joystick joystick,
        int button
    );

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte SDL_JoystickGetHat(
        SDL_Joystick joystick,
        int hat
    );

    private static readonly delegate* unmanaged[Cdecl]<SDL_Joystick, byte*> s_SDL_JoystickName = (delegate* unmanaged[Cdecl]<SDL_Joystick, byte*>)LoadFunction(nameof(SDL_JoystickName));
    private static readonly delegate* unmanaged[Cdecl]<int, byte*> s_SDL_JoystickNameForIndex = (delegate* unmanaged[Cdecl]<int, byte*>)LoadFunction(nameof(SDL_JoystickNameForIndex));

    public static string SDL_JoystickName(SDL_Joystick joystick)
    {
        return GetString(s_SDL_JoystickName(joystick));
    }

    public static string SDL_JoystickNameForIndex(int deviceIndex)
    {
        return GetString(s_SDL_JoystickNameForIndex(deviceIndex));
    }

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickNumAxes(SDL_Joystick joystick);

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickNumBalls(SDL_Joystick joystick);

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickNumButtons(SDL_Joystick joystick);

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickNumHats(SDL_Joystick joystick);

    /* IntPtr refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Joystick SDL_JoystickOpen(int device_index);

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UpdateJoysticks();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_JoystickID* SDL_GetJoysticks(int* count);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Guid SDL_JoystickGetDeviceGUID(
        int device_index
    );

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Guid SDL_JoystickGetGUID(
        SDL_Joystick joystick
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_JoystickGetGUIDString(
        Guid guid,
        byte[] pszGUID,
        int cbGUID
    );

    [DllImport(LibName, EntryPoint = "SDL_JoystickGetGUIDFromString", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe Guid INTERNAL_SDL_JoystickGetGUIDFromString(
        byte* pchGUID
    );
    public static unsafe Guid SDL_JoystickGetGUIDFromString(string pchGuid)
    {
        int utf8PchGuidBufSize = Utf8Size(pchGuid);
        byte* utf8PchGuid = stackalloc byte[utf8PchGuidBufSize];
        return INTERNAL_SDL_JoystickGetGUIDFromString(
            Utf8Encode(pchGuid, utf8PchGuid, utf8PchGuidBufSize)
        );
    }

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_JoystickGetDeviceVendor(int device_index);

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_JoystickGetDeviceProduct(int device_index);

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_JoystickGetDeviceProductVersion(int device_index);

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_JoystickType SDL_JoystickGetDeviceType(int device_index);

    /* int refers to an SDL_JoystickID.
     * Only available in 2.0.6 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickGetDeviceInstanceID(int device_index);

    /* joystick refers to an SDL_Joystick*.
     * Only available in 2.0.6 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_JoystickGetVendor(SDL_Joystick joystick);

    /* joystick refers to an SDL_Joystick*.
     * Only available in 2.0.6 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_JoystickGetProduct(SDL_Joystick joystick);

    /* joystick refers to an SDL_Joystick*.
     * Only available in 2.0.6 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_JoystickGetProductVersion(SDL_Joystick joystick);

    /* joystick refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, EntryPoint = "SDL_JoystickGetSerial", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_JoystickGetSerial(
        SDL_Joystick joystick
    );
    public static string SDL_JoystickGetSerial(
        SDL_Joystick joystick
    )
    {
        return GetString(
            INTERNAL_SDL_JoystickGetSerial(joystick)
        );
    }

    /* joystick refers to an SDL_Joystick*.
     * Only available in 2.0.6 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_JoystickType SDL_JoystickGetType(SDL_Joystick joystick);

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickGetAttached(SDL_Joystick joystick);

    /* int refers to an SDL_JoystickID, joystick to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickInstanceID(SDL_Joystick joystick);

    /* joystick refers to an SDL_Joystick*.
     * Only available in 2.0.4 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_JoystickPowerLevel SDL_JoystickCurrentPowerLevel(
        SDL_Joystick joystick
    );

    /* int refers to an SDL_JoystickID, IntPtr to an SDL_Joystick*.
     * Only available in 2.0.4 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Joystick SDL_JoystickFromInstanceID(int instance_id);

    /* Only available in 2.0.7 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LockJoysticks();

    /* Only available in 2.0.7 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UnlockJoysticks();

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.11 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Joystick SDL_JoystickFromPlayerIndex(int player_index);

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.11 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_JoystickSetPlayerIndex(
        SDL_Joystick joystick,
        int player_index
    );

    /* Int32 refers to an SDL_JoystickType.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickAttachVirtual(
        Int32 type,
        int naxes,
        int nbuttons,
        int nhats
    );

    /* Only available in 2.0.14 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickDetachVirtual(int device_index);

    /* Only available in 2.0.14 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickIsVirtual(int device_index);

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickSetVirtualAxis(
        IntPtr joystick,
        int axis,
        Int16 value
    );

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickSetVirtualButton(
        IntPtr joystick,
        int button,
        byte value
    );

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickSetVirtualHat(
        IntPtr joystick,
        int hat,
        byte value
    );

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickHasLED(IntPtr joystick);

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickHasRumble(IntPtr joystick);

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickHasRumbleTriggers(IntPtr joystick);

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickSetLED(
        IntPtr joystick,
        byte red,
        byte green,
        byte blue
    );

    /* joystick refers to an SDL_Joystick*.
     * data refers to a const void*.
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_JoystickSendEffect(
        IntPtr joystick,
        IntPtr data,
        int size
    );
    #endregion

    #region SDL_gamecontroller.h

    public enum SDL_GameControllerBindType
    {
        SDL_CONTROLLER_BINDTYPE_NONE,
        SDL_CONTROLLER_BINDTYPE_BUTTON,
        SDL_CONTROLLER_BINDTYPE_AXIS,
        SDL_CONTROLLER_BINDTYPE_HAT
    }

    public enum SDL_GameControllerAxis
    {
        SDL_CONTROLLER_AXIS_INVALID = -1,
        SDL_CONTROLLER_AXIS_LEFTX,
        SDL_CONTROLLER_AXIS_LEFTY,
        SDL_CONTROLLER_AXIS_RIGHTX,
        SDL_CONTROLLER_AXIS_RIGHTY,
        SDL_CONTROLLER_AXIS_TRIGGERLEFT,
        SDL_CONTROLLER_AXIS_TRIGGERRIGHT,
        SDL_CONTROLLER_AXIS_MAX
    }

    public enum SDL_GameControllerButton
    {
        SDL_CONTROLLER_BUTTON_INVALID = -1,
        SDL_CONTROLLER_BUTTON_A,
        SDL_CONTROLLER_BUTTON_B,
        SDL_CONTROLLER_BUTTON_X,
        SDL_CONTROLLER_BUTTON_Y,
        SDL_CONTROLLER_BUTTON_BACK,
        SDL_CONTROLLER_BUTTON_GUIDE,
        SDL_CONTROLLER_BUTTON_START,
        SDL_CONTROLLER_BUTTON_LEFTSTICK,
        SDL_CONTROLLER_BUTTON_RIGHTSTICK,
        SDL_CONTROLLER_BUTTON_LEFTSHOULDER,
        SDL_CONTROLLER_BUTTON_RIGHTSHOULDER,
        SDL_CONTROLLER_BUTTON_DPAD_UP,
        SDL_CONTROLLER_BUTTON_DPAD_DOWN,
        SDL_CONTROLLER_BUTTON_DPAD_LEFT,
        SDL_CONTROLLER_BUTTON_DPAD_RIGHT,
        SDL_CONTROLLER_BUTTON_MISC1,
        SDL_CONTROLLER_BUTTON_PADDLE1,
        SDL_CONTROLLER_BUTTON_PADDLE2,
        SDL_CONTROLLER_BUTTON_PADDLE3,
        SDL_CONTROLLER_BUTTON_PADDLE4,
        SDL_CONTROLLER_BUTTON_TOUCHPAD,
        SDL_CONTROLLER_BUTTON_MAX,
    }

    public enum SDL_GameControllerType
    {
        SDL_CONTROLLER_TYPE_UNKNOWN = 0,
        SDL_CONTROLLER_TYPE_XBOX360,
        SDL_CONTROLLER_TYPE_XBOXONE,
        SDL_CONTROLLER_TYPE_PS3,
        SDL_CONTROLLER_TYPE_PS4,
        SDL_CONTROLLER_TYPE_NINTENDO_SWITCH_PRO,
        SDL_CONTROLLER_TYPE_VIRTUAL,        /* Requires >= 2.0.14 */
        SDL_CONTROLLER_TYPE_PS5,        /* Requires >= 2.0.14 */
        SDL_CONTROLLER_TYPE_AMAZON_LUNA,    /* Requires >= 2.0.16 */
        SDL_CONTROLLER_TYPE_GOOGLE_STADIA   /* Requires >= 2.0.16 */
    }

    // FIXME: I'd rather this somehow be private...
    [StructLayout(LayoutKind.Sequential)]
    public struct INTERNAL_GameControllerButtonBind_hat
    {
        public int hat;
        public int hat_mask;
    }

    // FIXME: I'd rather this somehow be private...
    [StructLayout(LayoutKind.Explicit)]
    public struct INTERNAL_GameControllerButtonBind_union
    {
        [FieldOffset(0)]
        public int button;
        [FieldOffset(0)]
        public int axis;
        [FieldOffset(0)]
        public INTERNAL_GameControllerButtonBind_hat hat;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_GameControllerButtonBind
    {
        public SDL_GameControllerBindType bindType;
        public INTERNAL_GameControllerButtonBind_union value;
    }

    /* This exists to deal with C# being stupid about blittable types. */
    [StructLayout(LayoutKind.Sequential)]
    private struct INTERNAL_SDL_GameControllerButtonBind
    {
        public int bindType;
        /* Largest data type in the union is two ints in size */
        public int unionVal0;
        public int unionVal1;
    }

    [DllImport(LibName, EntryPoint = "SDL_GameControllerAddMapping", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int INTERNAL_SDL_GameControllerAddMapping(byte* mappingString);

    public static unsafe int SDL_GameControllerAddMapping(
        string mappingString
    )
    {
        byte* utf8MappingString = Utf8EncodeHeap(mappingString);
        int result = INTERNAL_SDL_GameControllerAddMapping(
            utf8MappingString
        );

        NativeMemory.Free(utf8MappingString);
        return result;
    }

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerNumMappings();

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, EntryPoint = "SDL_GameControllerMappingForIndex", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerMappingForIndex(int mapping_index);
    public static string SDL_GameControllerMappingForIndex(int mapping_index)
    {
        return GetString(
            INTERNAL_SDL_GameControllerMappingForIndex(
                mapping_index
            ),
            true
        );
    }

    /* THIS IS AN RWops FUNCTION! */
    [DllImport(LibName, EntryPoint = "SDL_GameControllerAddMappingsFromRW", CallingConvention = CallingConvention.Cdecl)]
    private static extern int INTERNAL_SDL_GameControllerAddMappingsFromRW(
        IntPtr rw,
        int freerw
    );
    public static int SDL_GameControllerAddMappingsFromFile(string file)
    {
        IntPtr rwops = SDL_RWFromFile(file, "rb");
        return INTERNAL_SDL_GameControllerAddMappingsFromRW(rwops, 1);
    }

    [DllImport(LibName, EntryPoint = "SDL_GameControllerMappingForGUID", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerMappingForGUID(
        Guid guid
    );
    public static string SDL_GameControllerMappingForGUID(Guid guid)
    {
        return GetString(
            INTERNAL_SDL_GameControllerMappingForGUID(guid),
            true
        );
    }

    /* gamecontroller refers to an SDL_GameController* */
    [DllImport(LibName, EntryPoint = "SDL_GameControllerMapping", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerMapping(
        IntPtr gamecontroller
    );
    public static string SDL_GameControllerMapping(
        IntPtr gamecontroller
    )
    {
        return GetString(
            INTERNAL_SDL_GameControllerMapping(
                gamecontroller
            ),
            true
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_IsGameController(int joystick_index);

    [DllImport(LibName, EntryPoint = "SDL_GameControllerNameForIndex", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerNameForIndex(
        int joystick_index
    );
    public static string SDL_GameControllerNameForIndex(
        int joystick_index
    )
    {
        return GetString(
            INTERNAL_SDL_GameControllerNameForIndex(joystick_index)
        );
    }

    /* Only available in 2.0.9 or higher. */
    [DllImport(LibName, EntryPoint = "SDL_GameControllerMappingForDeviceIndex", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerMappingForDeviceIndex(
        int joystick_index
    );
    public static string SDL_GameControllerMappingForDeviceIndex(
        int joystick_index
    )
    {
        return GetString(
            INTERNAL_SDL_GameControllerMappingForDeviceIndex(joystick_index),
            true
        );
    }

    /* IntPtr refers to an SDL_GameController* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GameControllerOpen(int joystick_index);

    /* gamecontroller refers to an SDL_GameController* */
    [DllImport(LibName, EntryPoint = "SDL_GameControllerName", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerName(
        IntPtr gamecontroller
    );
    public static string SDL_GameControllerName(
        IntPtr gamecontroller
    )
    {
        return GetString(
            INTERNAL_SDL_GameControllerName(gamecontroller)
        );
    }

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.6 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GameControllerGetVendor(
        IntPtr gamecontroller
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.6 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GameControllerGetProduct(
        IntPtr gamecontroller
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.6 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GameControllerGetProductVersion(
        IntPtr gamecontroller
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, EntryPoint = "SDL_GameControllerGetSerial", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerGetSerial(
        IntPtr gamecontroller
    );
    public static string SDL_GameControllerGetSerial(
        IntPtr gamecontroller
    )
    {
        return GetString(
            INTERNAL_SDL_GameControllerGetSerial(gamecontroller)
        );
    }

    /* gamecontroller refers to an SDL_GameController* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GameControllerGetAttached(
        IntPtr gamecontroller
    );

    /* IntPtr refers to an SDL_Joystick*
     * gamecontroller refers to an SDL_GameController*
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GameControllerGetJoystick(
        IntPtr gamecontroller
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerEventState(int state);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GameControllerUpdate();

    [DllImport(LibName, EntryPoint = "SDL_GameControllerGetAxisFromString", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe SDL_GameControllerAxis INTERNAL_SDL_GameControllerGetAxisFromString(
        byte* pchString
    );
    public static unsafe SDL_GameControllerAxis SDL_GameControllerGetAxisFromString(
        string pchString
    )
    {
        int utf8PchStringBufSize = Utf8Size(pchString);
        byte* utf8PchString = stackalloc byte[utf8PchStringBufSize];
        return INTERNAL_SDL_GameControllerGetAxisFromString(
            Utf8Encode(pchString, utf8PchString, utf8PchStringBufSize)
        );
    }

    [DllImport(LibName, EntryPoint = "SDL_GameControllerGetStringForAxis", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerGetStringForAxis(
        SDL_GameControllerAxis axis
    );
    public static string SDL_GameControllerGetStringForAxis(
        SDL_GameControllerAxis axis
    )
    {
        return GetString(
            INTERNAL_SDL_GameControllerGetStringForAxis(
                axis
            )
        );
    }

    /* gamecontroller refers to an SDL_GameController* */
    [DllImport(LibName, EntryPoint = "SDL_GameControllerGetBindForAxis", CallingConvention = CallingConvention.Cdecl)]
    private static extern INTERNAL_SDL_GameControllerButtonBind INTERNAL_SDL_GameControllerGetBindForAxis(
        IntPtr gamecontroller,
        SDL_GameControllerAxis axis
    );
    public static SDL_GameControllerButtonBind SDL_GameControllerGetBindForAxis(
        IntPtr gamecontroller,
        SDL_GameControllerAxis axis
    )
    {
        // This is guaranteed to never be null
        INTERNAL_SDL_GameControllerButtonBind dumb = INTERNAL_SDL_GameControllerGetBindForAxis(
            gamecontroller,
            axis
        );
        SDL_GameControllerButtonBind result = new SDL_GameControllerButtonBind();
        result.bindType = (SDL_GameControllerBindType)dumb.bindType;
        result.value.hat.hat = dumb.unionVal0;
        result.value.hat.hat_mask = dumb.unionVal1;
        return result;
    }

    /* gamecontroller refers to an SDL_GameController* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern short SDL_GameControllerGetAxis(
        IntPtr gamecontroller,
        SDL_GameControllerAxis axis
    );

    [DllImport(LibName, EntryPoint = "SDL_GameControllerGetButtonFromString", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe SDL_GameControllerButton INTERNAL_SDL_GameControllerGetButtonFromString(
        byte* pchString
    );
    public static unsafe SDL_GameControllerButton SDL_GameControllerGetButtonFromString(
        string pchString
    )
    {
        int utf8PchStringBufSize = Utf8Size(pchString);
        byte* utf8PchString = stackalloc byte[utf8PchStringBufSize];
        return INTERNAL_SDL_GameControllerGetButtonFromString(
            Utf8Encode(pchString, utf8PchString, utf8PchStringBufSize)
        );
    }

    [DllImport(LibName, EntryPoint = "SDL_GameControllerGetStringForButton", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerGetStringForButton(
        SDL_GameControllerButton button
    );
    public static string SDL_GameControllerGetStringForButton(
        SDL_GameControllerButton button
    )
    {
        return GetString(
            INTERNAL_SDL_GameControllerGetStringForButton(button)
        );
    }

    /* gamecontroller refers to an SDL_GameController* */
    [DllImport(LibName, EntryPoint = "SDL_GameControllerGetBindForButton", CallingConvention = CallingConvention.Cdecl)]
    private static extern INTERNAL_SDL_GameControllerButtonBind INTERNAL_SDL_GameControllerGetBindForButton(
        IntPtr gamecontroller,
        SDL_GameControllerButton button
    );
    public static SDL_GameControllerButtonBind SDL_GameControllerGetBindForButton(
        IntPtr gamecontroller,
        SDL_GameControllerButton button
    )
    {
        // This is guaranteed to never be null
        INTERNAL_SDL_GameControllerButtonBind dumb = INTERNAL_SDL_GameControllerGetBindForButton(
            gamecontroller,
            button
        );
        SDL_GameControllerButtonBind result = new SDL_GameControllerButtonBind();
        result.bindType = (SDL_GameControllerBindType)dumb.bindType;
        result.value.hat.hat = dumb.unionVal0;
        result.value.hat.hat_mask = dumb.unionVal1;
        return result;
    }

    /* gamecontroller refers to an SDL_GameController* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte SDL_GameControllerGetButton(
        IntPtr gamecontroller,
        SDL_GameControllerButton button
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.9 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerRumble(
        IntPtr gamecontroller,
        UInt16 low_frequency_rumble,
        UInt16 high_frequency_rumble,
        UInt32 duration_ms
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerRumbleTriggers(
        IntPtr gamecontroller,
        UInt16 left_rumble,
        UInt16 right_rumble,
        UInt32 duration_ms
    );

    /* gamecontroller refers to an SDL_GameController* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GameControllerClose(
        IntPtr gamecontroller
    );

    /* gamecontroller refers to an SDL_GameController*
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, EntryPoint = "SDL_GameControllerGetAppleSFSymbolsNameForButton", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerGetAppleSFSymbolsNameForButton(
        IntPtr gamecontroller,
        SDL_GameControllerButton button
    );
    public static string SDL_GameControllerGetAppleSFSymbolsNameForButton(
        IntPtr gamecontroller,
        SDL_GameControllerButton button
    )
    {
        return GetString(
            INTERNAL_SDL_GameControllerGetAppleSFSymbolsNameForButton(gamecontroller, button)
        );
    }

    /* gamecontroller refers to an SDL_GameController*
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, EntryPoint = "SDL_GameControllerGetAppleSFSymbolsNameForAxis", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GameControllerGetAppleSFSymbolsNameForAxis(
        IntPtr gamecontroller,
        SDL_GameControllerAxis axis
    );
    public static string SDL_GameControllerGetAppleSFSymbolsNameForAxis(
        IntPtr gamecontroller,
        SDL_GameControllerAxis axis
    )
    {
        return GetString(
            INTERNAL_SDL_GameControllerGetAppleSFSymbolsNameForAxis(gamecontroller, axis)
        );
    }

    /* int refers to an SDL_JoystickID, IntPtr to an SDL_GameController*.
     * Only available in 2.0.4 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GameControllerFromInstanceID(int joyid);

    /* Only available in 2.0.11 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_GameControllerType SDL_GameControllerTypeForIndex(
        int joystick_index
    );

    /* IntPtr refers to an SDL_GameController*.
     * Only available in 2.0.11 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_GameControllerType SDL_GameControllerGetType(
        IntPtr gamecontroller
    );

    /* IntPtr refers to an SDL_GameController*.
     * Only available in 2.0.11 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GameControllerFromPlayerIndex(
        int player_index
    );

    /* IntPtr refers to an SDL_GameController*.
     * Only available in 2.0.11 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GameControllerSetPlayerIndex(
        IntPtr gamecontroller,
        int player_index
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GameControllerHasLED(
        IntPtr gamecontroller
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GameControllerHasRumble(
        IntPtr gamecontroller
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GameControllerHasRumbleTriggers(
        IntPtr gamecontroller
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerSetLED(
        IntPtr gamecontroller,
        byte red,
        byte green,
        byte blue
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GameControllerHasAxis(
        IntPtr gamecontroller,
        SDL_GameControllerAxis axis
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GameControllerHasButton(
        IntPtr gamecontroller,
        SDL_GameControllerButton button
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerGetNumTouchpads(
        IntPtr gamecontroller
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerGetNumTouchpadFingers(
        IntPtr gamecontroller,
        int touchpad
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerGetTouchpadFinger(
        IntPtr gamecontroller,
        int touchpad,
        int finger,
        out byte state,
        out float x,
        out float y,
        out float pressure
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GameControllerHasSensor(
        IntPtr gamecontroller,
        SDL_SensorType type
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerSetSensorEnabled(
        IntPtr gamecontroller,
        SDL_SensorType type,
        SDL_bool enabled
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GameControllerIsSensorEnabled(
        IntPtr gamecontroller,
        SDL_SensorType type
    );

    /* gamecontroller refers to an SDL_GameController*.
     * data refers to a float*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerGetSensorData(
        IntPtr gamecontroller,
        SDL_SensorType type,
        IntPtr data,
        int num_values
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerGetSensorData(
        IntPtr gamecontroller,
        SDL_SensorType type,
        [In] float[] data,
        int num_values
    );

    /* gamecontroller refers to an SDL_GameController*.
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern float SDL_GameControllerGetSensorDataRate(
        IntPtr gamecontroller,
        SDL_SensorType type
    );

    /* gamecontroller refers to an SDL_GameController*.
     * data refers to a const void*.
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GameControllerSendEffect(
        IntPtr gamecontroller,
        IntPtr data,
        int size
    );
    #endregion

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

    #region SDL_sensor.h

    /// <summary>
    /// The different sensors defined by SDL.
    /// </summary>
    public enum SDL_SensorType : int
    {
        /// <summary>
        /// Returned for an invalid sensor
        /// </summary>
        SDL_SENSOR_INVALID = -1,
        /// <summary>
        /// Unknown sensor type
        /// </summary>
        SDL_SENSOR_UNKNOWN,
        /// <summary>
        /// Accelerometer
        /// </summary>
        SDL_SENSOR_ACCEL,
        /// <summary>
        /// Gyroscope
        /// </summary>
        SDL_SENSOR_GYRO,
        /// <summary>
        /// Accelerometer for left Joy-Con controller and Wii nunchuk
        /// </summary>
        SDL_SENSOR_ACCEL_L,
        /// <summary>
        /// Gyroscope for left Joy-Con controller
        /// </summary>
        SDL_SENSOR_GYRO_L,
        /// <summary>
        /// Accelerometer for right Joy-Con controller
        /// </summary>
        SDL_SENSOR_ACCEL_R,
        /// <summary>
        /// Gyroscope for right Joy-Con controller 
        /// </summary>
        SDL_SENSOR_GYRO_R
    }

    public const float SDL_STANDARD_GRAVITY = 9.80665f;

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_NumSensors();

    [DllImport(LibName, EntryPoint = "SDL_SensorGetDeviceName", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_SensorGetDeviceName(int device_index);
    public static string SDL_SensorGetDeviceName(int device_index)
    {
        return GetString(INTERNAL_SDL_SensorGetDeviceName(device_index));
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_SensorType SDL_SensorGetDeviceType(int device_index);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SensorGetDeviceNonPortableType(int device_index);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Int32 SDL_SensorGetDeviceInstanceID(int device_index);

    /* IntPtr refers to an SDL_Sensor* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_SensorOpen(int device_index);

    /* IntPtr refers to an SDL_Sensor* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_SensorFromInstanceID(
        Int32 instance_id
    );

    /* sensor refers to an SDL_Sensor* */
    [DllImport(LibName, EntryPoint = "SDL_SensorGetName", CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_SensorGetName(IntPtr sensor);
    public static string SDL_SensorGetName(IntPtr sensor)
    {
        return GetString(INTERNAL_SDL_SensorGetName(sensor));
    }

    /* sensor refers to an SDL_Sensor* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_SensorType SDL_SensorGetType(IntPtr sensor);

    /* sensor refers to an SDL_Sensor* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SensorGetNonPortableType(IntPtr sensor);

    /* sensor refers to an SDL_Sensor* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Int32 SDL_SensorGetInstanceID(IntPtr sensor);

    /* sensor refers to an SDL_Sensor* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SensorGetData(
        IntPtr sensor,
        float[] data,
        int num_values
    );

    /* sensor refers to an SDL_Sensor* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SensorClose(IntPtr sensor);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SensorUpdate();

    /* Only available in 2.0.14 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LockSensors();

    /* Only available in 2.0.14 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UnlockSensors();
    #endregion

    #region SDL_audio.h

    public const ushort SDL_AUDIO_MASK_BITSIZE = 0xFF;
    public const ushort SDL_AUDIO_MASK_DATATYPE = (1 << 8);
    public const ushort SDL_AUDIO_MASK_ENDIAN = (1 << 12);
    public const ushort SDL_AUDIO_MASK_SIGNED = (1 << 15);

    public static ushort SDL_AUDIO_BITSIZE(ushort x)
    {
        return (ushort)(x & SDL_AUDIO_MASK_BITSIZE);
    }

    public static bool SDL_AUDIO_ISFLOAT(ushort x)
    {
        return (x & SDL_AUDIO_MASK_DATATYPE) != 0;
    }

    public static bool SDL_AUDIO_ISBIGENDIAN(ushort x)
    {
        return (x & SDL_AUDIO_MASK_ENDIAN) != 0;
    }

    public static bool SDL_AUDIO_ISSIGNED(ushort x)
    {
        return (x & SDL_AUDIO_MASK_SIGNED) != 0;
    }

    public static bool SDL_AUDIO_ISINT(ushort x)
    {
        return (x & SDL_AUDIO_MASK_DATATYPE) == 0;
    }

    public static bool SDL_AUDIO_ISLITTLEENDIAN(ushort x)
    {
        return (x & SDL_AUDIO_MASK_ENDIAN) == 0;
    }

    public static bool SDL_AUDIO_ISUNSIGNED(ushort x)
    {
        return (x & SDL_AUDIO_MASK_SIGNED) == 0;
    }

    public const ushort AUDIO_U8 = 0x0008;
    public const ushort AUDIO_S8 = 0x8008;
    public const ushort AUDIO_U16LSB = 0x0010;
    public const ushort AUDIO_S16LSB = 0x8010;
    public const ushort AUDIO_U16MSB = 0x1010;
    public const ushort AUDIO_S16MSB = 0x9010;
    public const ushort AUDIO_U16 = AUDIO_U16LSB;
    public const ushort AUDIO_S16 = AUDIO_S16LSB;
    public const ushort AUDIO_S32LSB = 0x8020;
    public const ushort AUDIO_S32MSB = 0x9020;
    public const ushort AUDIO_S32 = AUDIO_S32LSB;
    public const ushort AUDIO_F32LSB = 0x8120;
    public const ushort AUDIO_F32MSB = 0x9120;
    public const ushort AUDIO_F32 = AUDIO_F32LSB;

    public static readonly ushort AUDIO_U16SYS =
        BitConverter.IsLittleEndian ? AUDIO_U16LSB : AUDIO_U16MSB;
    public static readonly ushort AUDIO_S16SYS =
        BitConverter.IsLittleEndian ? AUDIO_S16LSB : AUDIO_S16MSB;
    public static readonly ushort AUDIO_S32SYS =
        BitConverter.IsLittleEndian ? AUDIO_S32LSB : AUDIO_S32MSB;
    public static readonly ushort AUDIO_F32SYS =
        BitConverter.IsLittleEndian ? AUDIO_F32LSB : AUDIO_F32MSB;

    public const uint SDL_AUDIO_ALLOW_FREQUENCY_CHANGE = 0x00000001;
    public const uint SDL_AUDIO_ALLOW_FORMAT_CHANGE = 0x00000002;
    public const uint SDL_AUDIO_ALLOW_CHANNELS_CHANGE = 0x00000004;
    public const uint SDL_AUDIO_ALLOW_SAMPLES_CHANGE = 0x00000008;
    public const uint SDL_AUDIO_ALLOW_ANY_CHANGE = (
        SDL_AUDIO_ALLOW_FREQUENCY_CHANGE |
        SDL_AUDIO_ALLOW_FORMAT_CHANGE |
        SDL_AUDIO_ALLOW_CHANNELS_CHANGE |
        SDL_AUDIO_ALLOW_SAMPLES_CHANGE
    );

    public const int SDL_MIX_MAXVOLUME = 128;

    public enum SDL_AudioStatus
    {
        SDL_AUDIO_STOPPED,
        SDL_AUDIO_PLAYING,
        SDL_AUDIO_PAUSED
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_AudioSpec
    {
        public int freq;
        public ushort format; // SDL_AudioFormat
        public byte channels;
        public byte silence;
        public ushort samples;
        public uint size;
        public SDL_AudioCallback callback;
        public IntPtr userdata; // void*
    }

    /* userdata refers to a void*, stream to a Uint8 */
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SDL_AudioCallback(
        IntPtr userdata,
        IntPtr stream,
        int len
    );

#if TODO
    [DllImport(nativeLibName, EntryPoint = "SDL_AudioInit", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe int INTERNAL_SDL_AudioInit(
    byte* driver_name
);
    public static unsafe int SDL_AudioInit(string driver_name)
    {
        int utf8DriverNameBufSize = Utf8Size(driver_name);
        byte* utf8DriverName = stackalloc byte[utf8DriverNameBufSize];
        return INTERNAL_SDL_AudioInit(
            Utf8Encode(driver_name, utf8DriverName, utf8DriverNameBufSize)
        );
    }

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_AudioQuit();

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_CloseAudio();

    /* dev refers to an SDL_AudioDeviceID */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_CloseAudioDevice(uint dev);

    /* audio_buf refers to a malloc()'d buffer from SDL_LoadWAV */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_FreeWAV(IntPtr audio_buf);

    [DllImport(nativeLibName, EntryPoint = "SDL_GetAudioDeviceName", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr INTERNAL_SDL_GetAudioDeviceName(
        int index,
        int iscapture
    );
    public static string SDL_GetAudioDeviceName(
        int index,
        int iscapture
    )
    {
        return UTF8_ToManaged(
            INTERNAL_SDL_GetAudioDeviceName(index, iscapture)
        );
    }

    /* dev refers to an SDL_AudioDeviceID */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_AudioStatus SDL_GetAudioDeviceStatus(
        uint dev
    );

    [DllImport(nativeLibName, EntryPoint = "SDL_GetAudioDriver", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr INTERNAL_SDL_GetAudioDriver(int index);
    public static string SDL_GetAudioDriver(int index)
    {
        return UTF8_ToManaged(
            INTERNAL_SDL_GetAudioDriver(index)
        );
    }

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_AudioStatus SDL_GetAudioStatus();

    [DllImport(nativeLibName, EntryPoint = "SDL_GetCurrentAudioDriver", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr INTERNAL_SDL_GetCurrentAudioDriver();
    public static string SDL_GetCurrentAudioDriver()
    {
        return UTF8_ToManaged(INTERNAL_SDL_GetCurrentAudioDriver());
    }

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumAudioDevices(int iscapture);

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumAudioDrivers();

    /* audio_buf refers to a malloc()'d buffer, IntPtr to an SDL_AudioSpec* */
    /* THIS IS AN RWops FUNCTION! */
    [DllImport(nativeLibName, EntryPoint = "SDL_LoadWAV_RW", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr INTERNAL_SDL_LoadWAV_RW(
        IntPtr src,
        int freesrc,
        out SDL_AudioSpec spec,
        out IntPtr audio_buf,
        out uint audio_len
    );
    public static IntPtr SDL_LoadWAV(
        string file,
        out SDL_AudioSpec spec,
        out IntPtr audio_buf,
        out uint audio_len
    )
    {
        IntPtr rwops = SDL_RWFromFile(file, "rb");
        return INTERNAL_SDL_LoadWAV_RW(
            rwops,
            1,
            out spec,
            out audio_buf,
            out audio_len
        );
    }

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LockAudio();

    /* dev refers to an SDL_AudioDeviceID */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LockAudioDevice(uint dev);

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_MixAudio(
        [Out()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 2)]
                byte[] dst,
        [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 2)]
                byte[] src,
        uint len,
        int volume
    );

    /* format refers to an SDL_AudioFormat */
    /* This overload allows raw pointers to be passed for dst and src. */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_MixAudioFormat(
        IntPtr dst,
        IntPtr src,
        ushort format,
        uint len,
        int volume
    );

    /* format refers to an SDL_AudioFormat */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_MixAudioFormat(
        [Out()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 3)]
                byte[] dst,
        [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 3)]
                byte[] src,
        ushort format,
        uint len,
        int volume
    );

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_OpenAudio(
        ref SDL_AudioSpec desired,
        out SDL_AudioSpec obtained
    );

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_OpenAudio(
        ref SDL_AudioSpec desired,
        IntPtr obtained
    );

    /* uint refers to an SDL_AudioDeviceID */
    /* This overload allows for IntPtr.Zero (null) to be passed for device. */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern unsafe uint SDL_OpenAudioDevice(
        IntPtr device,
        int iscapture,
        ref SDL_AudioSpec desired,
        out SDL_AudioSpec obtained,
        int allowed_changes
    );

    /* uint refers to an SDL_AudioDeviceID */
    [DllImport(nativeLibName, EntryPoint = "SDL_OpenAudioDevice", CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe uint INTERNAL_SDL_OpenAudioDevice(
        byte* device,
        int iscapture,
        ref SDL_AudioSpec desired,
        out SDL_AudioSpec obtained,
        int allowed_changes
    );
    public static unsafe uint SDL_OpenAudioDevice(
        string device,
        int iscapture,
        ref SDL_AudioSpec desired,
        out SDL_AudioSpec obtained,
        int allowed_changes
    )
    {
        int utf8DeviceBufSize = Utf8Size(device);
        byte* utf8Device = stackalloc byte[utf8DeviceBufSize];
        return INTERNAL_SDL_OpenAudioDevice(
            Utf8Encode(device, utf8Device, utf8DeviceBufSize),
            iscapture,
            ref desired,
            out obtained,
            allowed_changes
        );
    }

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_PauseAudio(int pause_on);

    /* dev refers to an SDL_AudioDeviceID */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_PauseAudioDevice(
        uint dev,
        int pause_on
    );

    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UnlockAudio();

    /* dev refers to an SDL_AudioDeviceID */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UnlockAudioDevice(uint dev);

    /* dev refers to an SDL_AudioDeviceID, data to a void*
     * Only available in 2.0.4 or higher.
     */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_QueueAudio(
        uint dev,
        IntPtr data,
        UInt32 len
    );

    /* dev refers to an SDL_AudioDeviceID, data to a void*
     * Only available in 2.0.5 or higher.
     */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint SDL_DequeueAudio(
        uint dev,
        IntPtr data,
        uint len
    );

    /* dev refers to an SDL_AudioDeviceID
     * Only available in 2.0.4 or higher.
     */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern UInt32 SDL_GetQueuedAudioSize(uint dev);

    /* dev refers to an SDL_AudioDeviceID
     * Only available in 2.0.4 or higher.
     */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_ClearQueuedAudio(uint dev);

    /* src_format and dst_format refer to SDL_AudioFormats.
     * IntPtr refers to an SDL_AudioStream*.
     * Only available in 2.0.7 or higher.
     */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_NewAudioStream(
        ushort src_format,
        byte src_channels,
        int src_rate,
        ushort dst_format,
        byte dst_channels,
        int dst_rate
    );

    /* stream refers to an SDL_AudioStream*, buf to a void*.
     * Only available in 2.0.7 or higher.
     */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_AudioStreamPut(
        IntPtr stream,
        IntPtr buf,
        int len
    );

    /* stream refers to an SDL_AudioStream*, buf to a void*.
     * Only available in 2.0.7 or higher.
     */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_AudioStreamGet(
        IntPtr stream,
        IntPtr buf,
        int len
    );

    /* stream refers to an SDL_AudioStream*.
     * Only available in 2.0.7 or higher.
     */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_AudioStreamAvailable(IntPtr stream);

    /* stream refers to an SDL_AudioStream*.
     * Only available in 2.0.7 or higher.
     */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_AudioStreamClear(IntPtr stream);

    /* stream refers to an SDL_AudioStream*.
     * Only available in 2.0.7 or higher.
     */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_FreeAudioStream(IntPtr stream);

    /* Only available in 2.0.16 or higher. */
    [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetAudioDeviceSpec(
        int index,
        int iscapture,
        out SDL_AudioSpec spec
    );
#endif
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
    public static extern IntPtr SDL_RenderGetD3D9Device(IntPtr renderer);

    /* renderer refers to an SDL_Renderer*
     * IntPtr refers to an ID3D11Device*
     * Only available in 2.0.16 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_RenderGetD3D11Device(IntPtr renderer);

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
    public static nint LoadFunction(string name)
    {
        return s_sdl2Lib.LoadFunction(name);
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

    public static unsafe string GetString(byte* s, bool freePtr = false)
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

    private static unsafe byte* Utf8EncodeHeap(string str)
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


    private static unsafe byte* Utf8Encode(string str, byte* buffer, int bufferSize)
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
