// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static SDL.SDL;
using static SDL.SDL_EventType;
using static SDL.SDL_GLattr;
using static SDL.SDL_GLprofile;
using static SDL.SDL_LogPriority;
using System.Drawing;
using SDL;

namespace HelloWorld;

public static unsafe class Program
{
    public static void Main()
    {
#if DEBUG
        SDL_LogSetAllPriority(SDL_LOG_PRIORITY_VERBOSE);
#endif

        SDL_LogSetOutputFunction(OnLog);

        SDL_GetVersion(out SDL_version version);

        string str = SDL_GetPlatformString();

        // Init SDL
        if (SDL_Init(SDL_InitFlags.Timer | SDL_InitFlags.Video | SDL_InitFlags.Gamepad) != 0)
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

        SDL_GL_SetAttribute(SDL_GL_DOUBLEBUFFER, 1);
        SDL_GL_SetAttribute(SDL_GL_DEPTH_SIZE, 24);
        SDL_GL_SetAttribute(SDL_GL_STENCIL_SIZE, 8);

        // Enable native IME.
        SDL_SetHint(SDL_HINT_IME_SHOW_UI, true);

        // create the window
        SDL_WindowFlags flags = SDL_WindowFlags.Resizable | SDL_WindowFlags.OpenGL;
        SDL_Window window = SDL_CreateWindowWithPosition("Hello World", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 800, 600, flags);

        SDL_GLContext gl_context = SDL_GL_CreateContext(window);
        SDL_GL_MakeCurrent(window, gl_context);
        SDL_GL_SetSwapInterval(1);
        SDL_ShowWindow(window);

        var id = SDL_GetWindowID(window);
        SDL_GetWindowSizeInPixels(window, out int width, out int height);

        SDL_SysWMinfo info = new();
        SDL_GetWindowWMInfo(window, &info);

        var display = SDL_GetDisplayForWindow(window);
        display = SDL_GetDisplayForPoint(Point.Empty);

        var primary = SDL_GetPrimaryDisplay();
        var dispName = SDL_GetDisplayName(primary);
        var test3 = SDL_GetNumVideoDrivers();
        //var test2 = SDL_GetCurrentVideoDriver();
        ReadOnlySpan<SDL_DisplayID> displays = SDL_GetDisplays();
        for(int i = 0; i < displays.Length; i++)
        {
            dispName = SDL_GetDisplayName(displays[i]);
        }

        var driversAudio = SDL_GetNumAudioDrivers();
        for (int i = 0; i < driversAudio; i++)
        {
            dispName = SDL_GetAudioDriver(i);
        }

        ReadOnlySpan<SDL_JoystickID> joystics = SDL_GetJoysticks();

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

        SDL_GL_DeleteContext(gl_context);
        SDL_DestroyWindow(window);
        SDL_Quit();
    }

    private static void OnLog(SDL_LogCategory category, SDL_LogPriority priority, string message)
    {
        Console.WriteLine($"SDL: {message}");
    }
}
