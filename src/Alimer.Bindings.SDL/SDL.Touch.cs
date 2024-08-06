// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

unsafe partial class SDL3
{
    public const uint SDL_TOUCH_MOUSEID = uint.MaxValue;
    public const long SDL_MOUSE_TOUCHID = -1;

    public static ReadOnlySpan<SDL_TouchID> SDL_GetTouchDevices()
    {
        SDL_TouchID* ptr = SDL_GetTouchDevices(out int count);
        return new(ptr, count);
    }
}
