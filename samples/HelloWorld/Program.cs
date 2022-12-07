// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Alimer.Native.SDL.SDL;

namespace HelloWorld;

public static class Program
{
    public static unsafe void Main()
    {
        SDL_GetVersion(out SDL_version version);
    }
}
