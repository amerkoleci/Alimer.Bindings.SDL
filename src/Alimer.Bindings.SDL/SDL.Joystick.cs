// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

unsafe partial class SDL3
{
    public static string SDL_GetJoystickNameString(SDL_Joystick joystick)
    {
        return GetStringOrEmpty(SDL_GetJoystickName(joystick));
    }

    public static string SDL_GetJoystickNameForIDString(SDL_JoystickID instance_id)
    {
        return GetStringOrEmpty(SDL_GetJoystickNameForID(instance_id));
    }

    public static ReadOnlySpan<SDL_JoystickID> SDL_GetJoysticks()
    {
        SDL_JoystickID* ptr = SDL_GetJoysticks(out int count);
        return new(ptr, count);
    }

    public static string SDL_GetJoystickSerialString(SDL_Joystick joystick)
    {
        return GetStringOrEmpty(SDL_GetJoystickSerial(joystick));
    }
}
