// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using Alimer.Bindings.SDL;
using static Alimer.Bindings.SDL.SDL;
using static Alimer.Bindings.SDL.SDL.SDL_EventType;
using static Alimer.Bindings.SDL.SDL.SDL_InitFlags;
using static Alimer.Bindings.SDL.SDL.SDL_LogPriority;
using static Alimer.Bindings.SDL.SDL.SDL_WindowFlags;
using static Alimer.Bindings.SDL.SDL.SDL_GLattr;
using static Alimer.Bindings.SDL.SDL.SDL_GLprofile;

namespace HelloWorld;

public static class Program
{
    public static unsafe void Main()
    {
#if DEBUG
        SDL_LogSetAllPriority(SDL_LOG_PRIORITY_VERBOSE);
#endif

        SDL_LogSetOutputFunction(&OnLog, IntPtr.Zero);

        SDL_GetVersion(out SDL_version version);

        // Init SDL
        if (SDL_Init(SDL_INIT_TIMER | SDL_INIT_VIDEO | SDL_INIT_GAMEPAD) != 0)
        {
            var error = SDL_GetError();
            throw new Exception($"Failed to start SDL2: {error}");
        }

        //if (SDL_Vulkan_LoadLibrary() < 0)
        //{
        //    return;
        //}
        //
        //var vkGetInstanceProcAddr = SDL_Vulkan_GetVkGetInstanceProcAddr();
        //string[] extensions = SDL_Vulkan_GetInstanceExtensions();

        SDL_GL_SetAttribute(SDL_GL_CONTEXT_MAJOR_VERSION, 3);
        SDL_GL_SetAttribute(SDL_GL_CONTEXT_MINOR_VERSION, 3);
        SDL_GL_SetAttribute(SDL_GL_CONTEXT_PROFILE_MASK, SDL_GL_CONTEXT_PROFILE_CORE);
        //SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG); // Always required on Mac

        // Enable native IME.
        SDL_SetHint(SDL_HINT_IME_SHOW_UI, true);

        // create the window
        SDL_WindowFlags flags = SDL_WINDOW_RESIZABLE | SDL_WINDOW_OPENGL;
        SDL_Window window = SDL_CreateWindow("Hello World", 800, 600, flags);
        SDL_SetWindowPosition(window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
        SDL_GLContext gl_context = SDL_GL_CreateContext(window);
        SDL_GL_MakeCurrent(window, gl_context);
        SDL_GL_SetSwapInterval(1); 

        //SDL_ShowWindow(window);
        //var id = SDL_GetWindowID(window);
        //SDL_GetWindowSizeInPixels(window, out int width, out int height);

        //SDL_MaximizeWindow(window);
        //var test = SDL_GetWindowFullscreenMode(window);
        //SDL_SysWMinfo info = new();
        //SDL_GetWindowWMInfo(window, &info);
        //SDL_SetWindowResizable(window, false);

        //var test3 = SDL_GetNumVideoDisplays();
        //var test2 = SDL_GetCurrentVideoDriver();

        bool done = false;
        while (!done)
        {
            SDL_Event evt;
            while (SDL_PollEvent(&evt) == 1)
            {
                if (evt.type == SDL_QUIT)
                {
                    done = true;
                }

                if (evt.type == SDL_EVENT_WINDOW_CLOSE_REQUESTED && evt.window.windowID == SDL_GetWindowID(window))
                {
                    done = true;
                }
            }

            SDL_GL_SwapWindow(window);
        }

        //SDL_DestroyWindow(window);
        SDL_Quit();
    }

    [UnmanagedCallersOnly]
    private static unsafe void OnLog(IntPtr userdata, int category, SDL_LogPriority priority, sbyte* messagePtr)
    {
        string message = new(messagePtr);
        Console.WriteLine($"SDL: {message}");
    }
}
