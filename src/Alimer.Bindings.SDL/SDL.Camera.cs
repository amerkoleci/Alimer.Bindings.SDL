// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace SDL3;

unsafe partial class SDL3
{
    public static ReadOnlySpan<SDL_CameraID> SDL_GetCameras()
    {
        SDL_CameraID* ptr = SDL_GetCameras(out int count);
        return new(ptr, count);
    }
}
