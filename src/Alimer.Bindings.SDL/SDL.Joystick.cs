// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

unsafe partial class SDL3
{
    public static ReadOnlySpan<SDL_JoystickID> SDL_GetJoysticks()
    {
        SDL_JoystickID* ptr = SDL_GetJoysticks(out int count);
        return new(ptr, count);
    }
}
