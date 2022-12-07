// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using static Alimer.Bindings.SDL.SDL;
using static Alimer.Bindings.SDL.SDL.SDL_LogPriority;
using static Alimer.Bindings.SDL.SDL.SDL_WindowFlags;

namespace HelloWorld;

public static class Program
{
#if !NET6_0_OR_GREATER
    private static readonly SDL_LogOutputFunction _logCallback = OnLog;
#endif

    public static unsafe void Main()
    {
#if DEBUG
        SDL_LogSetAllPriority(SDL_LOG_PRIORITY_VERBOSE);
#endif

#if NET6_0_OR_GREATER
        SDL_LogSetOutputFunction(&OnLog, IntPtr.Zero);
#else
        SDL_LogSetOutputFunction(_logCallback, IntPtr.Zero);
#endif

        SDL_GetVersion(out SDL_version version);

        // DPI aware on Windows
        SDL_SetHint(SDL_HINT_WINDOWS_DPI_AWARENESS, "permonitorv2");
        SDL_SetHint(SDL_HINT_WINDOWS_DPI_SCALING, true);

        // Init SDL
        if (SDL_Init(SDL_INIT_TIMER | SDL_INIT_VIDEO | SDL_INIT_JOYSTICK | SDL_INIT_GAMECONTROLLER | SDL_INIT_EVENTS) != 0)
        {
            var error = SDL_GetError();
            throw new Exception($"Failed to start SDL2: {error}");
        }

        // create the window
        SDL_WindowFlags flags = SDL_WINDOW_ALLOW_HIGHDPI | SDL_WINDOW_SHOWN;
        nint window = SDL_CreateWindow("Hello World", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 800, 600, flags);
        SDL_DestroyWindow(window);
        SDL_Quit();
    }

#if NET6_0_OR_GREATER
    [UnmanagedCallersOnly]
#endif
    private static unsafe void OnLog(IntPtr userdata, int category, SDL_LogPriority priority, sbyte* messagePtr)
    {
        string message = new(messagePtr);
        //Log.Info($"SDL: {message}");
    }
}
