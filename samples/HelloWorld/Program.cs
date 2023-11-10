// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static SDL.SDL;
using System.Drawing;
using SDL;

namespace HelloWorld;

public static unsafe class Program
{
    public static void Main()
    {
#if DEBUG
        SDL_LogSetAllPriority(SDL_LogPriority.Verbose);
#endif

        SDL_LogSetOutputFunction(OnLog);

        SDL_GetVersion(out SDL_version version);

        string str = SDL_GetPlatformString();

        // Init SDL
        if (SDL_Init(SDL_InitFlags.Timer | SDL_InitFlags.Video | SDL_InitFlags.Gamepad) != 0)
        {
            var error = SDL_GetErrorString();
            throw new Exception($"Failed to start SDL2: {error}");
        }

        if (SDL_Vulkan_LoadLibrary() < 0)
        {
            return;
        }

        var vkGetInstanceProcAddr = SDL_Vulkan_GetVkGetInstanceProcAddr();
        string[] extensions = SDL_Vulkan_GetInstanceExtensions();

        SDL_GL_SetAttribute(SDL_GLattr.ContextMajorVersion, 3);
        SDL_GL_SetAttribute(SDL_GLattr.ContextMinorVersion, 3);
        SDL_GL_SetAttribute(SDL_GLattr.ContextProfileMask, SDL_GLprofile.Core);
        //SDL_GL_SetAttribute(SDL_GL_CONTEXT_FLAGS, SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG); // Always required on Mac

        SDL_GL_SetAttribute(SDL_GLattr.Doublebuffer, true);
        SDL_GL_SetAttribute(SDL_GLattr.DepthSize, 24);
        SDL_GL_SetAttribute(SDL_GLattr.StencilSize, 8);

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

        nint hwnd = (nint)SDL_GetProperty(SDL_GetWindowProperties(window), "SDL.window.win32.hwnd");

        var display = SDL_GetDisplayForWindow(window);
        display = SDL_GetDisplayForPoint(Point.Empty);

        var primary = SDL_GetPrimaryDisplay();
        var dispName = SDL_GetDisplayNameString(primary);
        var test3 = SDL_GetNumVideoDrivers();
        //var test2 = SDL_GetCurrentVideoDriver();
        ReadOnlySpan<SDL_DisplayID> displays = SDL_GetDisplays();
        for(int i = 0; i < displays.Length; i++)
        {
            dispName = SDL_GetDisplayNameString(displays[i]);
        }

        var driversAudio = SDL_GetNumAudioDrivers();
        for (int i = 0; i < driversAudio; i++)
        {
            dispName = SDL_GetAudioDriverString(i);
        }

        ReadOnlySpan<SDL_JoystickID> joystics = SDL_GetJoysticks();

        bool done = false;
        while (!done)
        {
            SDL_Event evt;
            while (SDL_PollEvent(&evt))
            {
                if (evt.type == SDL_EventType.Quit)
                {
                    done = true;
                }

                if (evt.type == SDL_EventType.WindowCloseRequested && evt.window.windowID == SDL_GetWindowID(window))
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
