// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace SDL;

unsafe partial class SDL
{
    public static ReadOnlySpan<SDL_CameraDeviceID> SDL_GetCameraDevices()
    {
        SDL_CameraDeviceID* ptr = SDL_GetCameraDevices(out int count);
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

    public static string SDL_GetCameraDeviceNameString(SDL_CameraDeviceID instance_id)
    {
        return GetStringOrEmpty(SDL_GetCameraDeviceName(instance_id));
    }
}
