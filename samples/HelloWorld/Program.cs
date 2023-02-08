// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using static Alimer.Bindings.SDL.SDL;
using static Alimer.Bindings.SDL.SDL.SDL_LogPriority;
using static Alimer.Bindings.SDL.SDL.SDL_WindowFlags;
using static Alimer.Bindings.SDL.SDL.SDL_EventType;
using Alimer.Bindings.SDL;
using System.Reflection.Metadata;

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

        //if (SDL_Vulkan_LoadLibrary() < 0)
        //{
        //    return;
        //}

        // create the window
        SDL_WindowFlags flags = SDL_WINDOW_ALLOW_HIGHDPI | SDL_WINDOW_SHOWN | SDL_WINDOW_RESIZABLE; // | SDL_WINDOW_VULKAN;
        SDL_Window window = SDL_CreateWindow("Hello World", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 800, 600, flags);
        var id = SDL_GetWindowID(window);
        //string[] extensions = SDL_Vulkan_GetInstanceExtensions(window);
        SDL_GetWindowSizeInPixels(window, out int width, out int height);

        SDL_SysWMinfo info = new();
        SDL_VERSION(out info.version);
        SDL_GetWindowWMInfo(window, ref info);

        SDL_SetWindowResizable(window, false);

        //var test3 = SDL_GetNumVideoDisplays();
        var test2 = SDL_GetCurrentVideoDriver();

        bool done = false;
        while (!done)
        {
            SDL_Event evt;
            while (SDL_PollEvent(&evt) == 1)
            {
                if (evt.type == SDL_QUIT)
                    done = true;
            }
        }

        SDL_DestroyWindow(window);
        SDL_Quit();
    }

#if NET6_0_OR_GREATER
    [UnmanagedCallersOnly]
#endif
    private static unsafe void OnLog(IntPtr userdata, int category, SDL_LogPriority priority, sbyte* messagePtr)
    {
        string message = new(messagePtr);
        Console.WriteLine($"SDL: {message}");
    }
}
