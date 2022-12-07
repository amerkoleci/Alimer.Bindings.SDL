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
using System.Runtime.InteropServices;
using System.Text;

namespace Alimer.Bindings.SDL;

public static unsafe class SDL
{
    private static readonly NativeLibrary s_sdl2Lib = LoadNativeLibrary();

    private static NativeLibrary LoadNativeLibrary()
    {
#if NET6_0_OR_GREATER
        if (OperatingSystem.IsWindows())
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
        {
            return new NativeLibrary("SDL2.dll");
        }
#if NET6_0_OR_GREATER
        else if (OperatingSystem.IsLinux())
#else
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
#endif
        {
            return new NativeLibrary("libSDL2-2.0.so.0", "libSDL2-2.0.so.1", "libSDL2-2.0.so");
        }
#if NET6_0_OR_GREATER
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
#else
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
#endif
        {
            return new NativeLibrary("libSDL2.dylib");
        }
        else
        {
            Debug.WriteLine("Unknown SDL platform. Attempting to load \"SDL2\"");
            return new NativeLibrary("SDL2");
        }
    }

    public enum SDL_bool
    {
        SDL_FALSE = 0,
        SDL_TRUE = 1
    }

    #region SDL.h
    public const uint SDL_INIT_TIMER = 0x00000001;
    public const uint SDL_INIT_AUDIO = 0x00000010;
    public const uint SDL_INIT_VIDEO = 0x00000020;
    public const uint SDL_INIT_JOYSTICK = 0x00000200;
    public const uint SDL_INIT_HAPTIC = 0x00001000;
    public const uint SDL_INIT_GAMECONTROLLER = 0x00002000;
    public const uint SDL_INIT_EVENTS = 0x00004000;
    public const uint SDL_INIT_SENSOR = 0x00008000;
    public const uint SDL_INIT_NOPARACHUTE = 0x00100000;
    public const uint SDL_INIT_EVERYTHING = (
        SDL_INIT_TIMER | SDL_INIT_AUDIO | SDL_INIT_VIDEO |
        SDL_INIT_EVENTS | SDL_INIT_JOYSTICK | SDL_INIT_HAPTIC |
        SDL_INIT_GAMECONTROLLER | SDL_INIT_SENSOR
    );

    private static readonly delegate* unmanaged[Cdecl]<uint, int> s_SDL_Init = (delegate* unmanaged[Cdecl]<uint, int>)LoadFunction(nameof(SDL_Init));
    private static readonly delegate* unmanaged[Cdecl]<uint, int> s_SDL_InitSubSystem = (delegate* unmanaged[Cdecl]<uint, int>)LoadFunction(nameof(SDL_InitSubSystem));
    private static readonly delegate* unmanaged[Cdecl]<void> s_SDL_Quit = (delegate* unmanaged[Cdecl]<void>)LoadFunction(nameof(SDL_Quit));
    private static readonly delegate* unmanaged[Cdecl]<uint, void> s_SDL_QuitSubSystem = (delegate* unmanaged[Cdecl]<uint, void>)LoadFunction(nameof(SDL_QuitSubSystem));
    private static readonly delegate* unmanaged[Cdecl]<uint, uint> s_SDL_WasInit = (delegate* unmanaged[Cdecl]<uint, uint>)LoadFunction(nameof(SDL_WasInit));

    public static int SDL_Init(uint flags) => s_SDL_Init(flags);
    public static int SDL_InitSubSystem(uint flags) => s_SDL_InitSubSystem(flags);
    public static void SDL_Quit() => s_SDL_Quit();
    public static void SDL_QuitSubSystem(uint flags) => s_SDL_QuitSubSystem(flags);
    public static uint SDL_WasInit(uint flags) => s_SDL_WasInit(flags);

    #endregion

    #region SDL_platform.h
    private static readonly delegate* unmanaged[Cdecl]<byte*> s_SDL_GetPlatform = (delegate* unmanaged[Cdecl]<byte*>)LoadFunction(nameof(SDL_GetPlatform));

    public static string SDL_GetPlatform() => GetString(s_SDL_GetPlatform())!;
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
    public const string SDL_HINT_VIDEO_EGL_ALLOW_TRANSPARENCY =
        "SDL_VIDEO_EGL_ALLOW_TRANSPARENCY";
    public const string SDL_HINT_APP_NAME =
        "SDL_APP_NAME";
    public const string SDL_HINT_SCREENSAVER_INHIBIT_ACTIVITY_NAME =
        "SDL_SCREENSAVER_INHIBIT_ACTIVITY_NAME";
    public const string SDL_HINT_IME_SHOW_UI =
        "SDL_IME_SHOW_UI";
    public const string SDL_HINT_WINDOW_NO_ACTIVATION_WHEN_SHOWN =
        "SDL_WINDOW_NO_ACTIVATION_WHEN_SHOWN";
    public const string SDL_HINT_POLL_SENTINEL =
        "SDL_POLL_SENTINEL";
    public const string SDL_HINT_JOYSTICK_DEVICE =
        "SDL_JOYSTICK_DEVICE";
    public const string SDL_HINT_LINUX_JOYSTICK_CLASSIC =
        "SDL_LINUX_JOYSTICK_CLASSIC";

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

    public const string SDL_HINT_WINDOWS_DPI_AWARENESS = "SDL_WINDOWS_DPI_AWARENESS";
    public const string SDL_HINT_WINDOWS_DPI_SCALING = "SDL_WINDOWS_DPI_SCALING";

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

    private static readonly delegate* unmanaged[Cdecl]<SDL_LogPriority, void> s_SDL_LogSetAllPriority = (delegate* unmanaged[Cdecl]<SDL_LogPriority, void>)LoadFunction(nameof(SDL_LogSetAllPriority));
#if NET6_0_OR_GREATER
    private static readonly delegate* unmanaged[Cdecl]<delegate* unmanaged<IntPtr, int, SDL_LogPriority, sbyte*, void>, IntPtr, void> s_SDL_LogSetOutputFunction = (delegate* unmanaged[Cdecl]<delegate* unmanaged<IntPtr, int, SDL_LogPriority, sbyte*, void>, IntPtr, void>)LoadFunction(nameof(SDL_LogSetOutputFunction));
#else
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void SDL_LogOutputFunction(IntPtr userdata, int category, SDL_LogPriority priority, sbyte* message);

    private static readonly delegate* unmanaged[Cdecl]<SDL_LogOutputFunction, IntPtr, void> s_SDL_LogSetOutputFunction = (delegate* unmanaged[Cdecl]<SDL_LogOutputFunction, IntPtr, void>)LoadFunction(nameof(SDL_LogSetOutputFunction));
#endif

    public static void SDL_LogSetAllPriority(SDL_LogPriority priority) => s_SDL_LogSetAllPriority(priority);

#if NET6_0_OR_GREATER
    public static void SDL_LogSetOutputFunction(delegate* unmanaged<IntPtr, int, SDL_LogPriority, sbyte*, void> callback, IntPtr userdata)
#else
    public static void SDL_LogSetOutputFunction(SDL_LogOutputFunction callback, IntPtr userdata)
#endif
    {
        s_SDL_LogSetOutputFunction(callback, userdata);
    }
    #endregion

    #region SDL_version.h, SDL_revision.h

    /* Similar to the headers, this is the version we're expecting to be
     * running with. You will likely want to check this somewhere in your
     * program!
     */
    public const int SDL_MAJOR_VERSION = 2;
    public const int SDL_MINOR_VERSION = 0;
    public const int SDL_PATCHLEVEL = 22;

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
        SDL_GL_CONTEXT_EGL,
        SDL_GL_CONTEXT_FLAGS,
        SDL_GL_CONTEXT_PROFILE_MASK,
        SDL_GL_SHARE_WITH_CURRENT_CONTEXT,
        SDL_GL_FRAMEBUFFER_SRGB_CAPABLE,
        SDL_GL_CONTEXT_RELEASE_BEHAVIOR,
        SDL_GL_CONTEXT_RESET_NOTIFICATION,  /* Requires >= 2.0.6 */
        SDL_GL_CONTEXT_NO_ERROR,        /* Requires >= 2.0.6 */
    }

    [Flags]
    public enum SDL_GLprofile
    {
        SDL_GL_CONTEXT_PROFILE_CORE = 0x0001,
        SDL_GL_CONTEXT_PROFILE_COMPATIBILITY = 0x0002,
        SDL_GL_CONTEXT_PROFILE_ES = 0x0004
    }

    [Flags]
    public enum SDL_GLcontext
    {
        SDL_GL_CONTEXT_DEBUG_FLAG = 0x0001,
        SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG = 0x0002,
        SDL_GL_CONTEXT_ROBUST_ACCESS_FLAG = 0x0004,
        SDL_GL_CONTEXT_RESET_ISOLATION_FLAG = 0x0008
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
        SDL_WINDOW_FULLSCREEN = 0x00000001,
        SDL_WINDOW_OPENGL = 0x00000002,
        SDL_WINDOW_SHOWN = 0x00000004,
        SDL_WINDOW_HIDDEN = 0x00000008,
        SDL_WINDOW_BORDERLESS = 0x00000010,
        SDL_WINDOW_RESIZABLE = 0x00000020,
        SDL_WINDOW_MINIMIZED = 0x00000040,
        SDL_WINDOW_MAXIMIZED = 0x00000080,
        SDL_WINDOW_MOUSE_GRABBED = 0x00000100,
        SDL_WINDOW_INPUT_FOCUS = 0x00000200,
        SDL_WINDOW_MOUSE_FOCUS = 0x00000400,
        SDL_WINDOW_FULLSCREEN_DESKTOP =
            (SDL_WINDOW_FULLSCREEN | 0x00001000),
        SDL_WINDOW_FOREIGN = 0x00000800,
        SDL_WINDOW_ALLOW_HIGHDPI = 0x00002000,  /* Requires >= 2.0.1 */
        SDL_WINDOW_MOUSE_CAPTURE = 0x00004000,  /* Requires >= 2.0.4 */
        SDL_WINDOW_ALWAYS_ON_TOP = 0x00008000,  /* Requires >= 2.0.5 */
        SDL_WINDOW_SKIP_TASKBAR = 0x00010000,   /* Requires >= 2.0.5 */
        SDL_WINDOW_UTILITY = 0x00020000,    /* Requires >= 2.0.5 */
        SDL_WINDOW_TOOLTIP = 0x00040000,    /* Requires >= 2.0.5 */
        SDL_WINDOW_POPUP_MENU = 0x00080000, /* Requires >= 2.0.5 */
        SDL_WINDOW_KEYBOARD_GRABBED = 0x00100000,   /* Requires >= 2.0.16 */
        SDL_WINDOW_VULKAN = 0x10000000, /* Requires >= 2.0.6 */
        SDL_WINDOW_METAL = 0x2000000,   /* Requires >= 2.0.14 */

        SDL_WINDOW_INPUT_GRABBED =
            SDL_WINDOW_MOUSE_GRABBED,
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
        public IntPtr driverdata; // void*
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate SDL_HitTestResult SDL_HitTest(IntPtr win, IntPtr area, IntPtr data);

    private static readonly delegate* unmanaged[Cdecl]<byte*, int, int, int, int, SDL_WindowFlags, nint> s_SDL_CreateWindow = (delegate* unmanaged[Cdecl]<byte*, int, int, int, int, SDL_WindowFlags, nint>)LoadFunction(nameof(SDL_CreateWindow));
    private static readonly delegate* unmanaged[Cdecl]<int, int, SDL_WindowFlags, out nint, out nint, int> s_SDL_CreateWindowAndRenderer = (delegate* unmanaged[Cdecl]<int, int, SDL_WindowFlags, out nint, out nint, int>)LoadFunction(nameof(SDL_CreateWindowAndRenderer));
    private static readonly delegate* unmanaged[Cdecl]<nint, nint> s_SDL_CreateWindowFrom = (delegate* unmanaged[Cdecl]<nint, nint>)LoadFunction(nameof(SDL_CreateWindowFrom));
    private static readonly delegate* unmanaged[Cdecl]<nint, void> s_SDL_DestroyWindow = (delegate* unmanaged[Cdecl]<nint, void>)LoadFunction(nameof(SDL_DestroyWindow));

    public static nint SDL_CreateWindow(string title,
       int x,
       int y,
       int w,
       int h,
       SDL_WindowFlags flags)
    {
        int utf8TitleBufSize = Utf8Size(title);
        byte* utf8Title = stackalloc byte[utf8TitleBufSize];
        return s_SDL_CreateWindow(
            Utf8Encode(title, utf8Title, utf8TitleBufSize),
            x, y, w, h,
            flags
        );
    }

    public static int SDL_CreateWindowAndRenderer(
        int width,
        int height,
        SDL_WindowFlags flags,
        out nint window,
        out nint renderer
        )
    {
        return s_SDL_CreateWindowAndRenderer(width, height, flags, out window, out renderer);
    }

    public static nint SDL_CreateWindowFrom(nint data) => s_SDL_CreateWindowFrom(data);
    public static void SDL_DestroyWindow(nint window) => s_SDL_DestroyWindow(window);
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

    /* Used for stack allocated string marshaling. */
    private static int Utf8Size(string str)
    {
        if (str == null)
        {
            return 0;
        }
        return (str.Length * 4) + 1;
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
