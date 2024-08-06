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

    public static string SDL_GetCameraDriverString(int index)
    {
        return GetStringOrEmpty(SDL_GetCameraDriver(index));
    }

    public static string SDL_GetCurrentCameraDriverString()
    {
        return GetStringOrEmpty(SDL_GetCurrentCameraDriver());
    }

    public static string SDL_GetCameraNameString(SDL_CameraID instance_id)
    {
        return GetStringOrEmpty(SDL_GetCameraName(instance_id));
    }
}
