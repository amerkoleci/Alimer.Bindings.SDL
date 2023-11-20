// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

unsafe partial class SDL
{
    public static string SDL_GetJoystickNameString(SDL_Joystick joystick)
    {
        return GetStringOrEmpty(SDL_GetJoystickName(joystick));
    }

    public static string SDL_GetJoystickInstanceNameString(SDL_JoystickID instance_id)
    {
        return GetStringOrEmpty(SDL_GetJoystickInstanceName(instance_id));
    }

    public static ReadOnlySpan<SDL_JoystickID> SDL_GetJoysticks()
    {
        SDL_JoystickID* ptr = SDL_GetJoysticks(out int count);
        return new(ptr, count);
    }

    public static Guid SDL_GetJoystickGUIDFromString(ReadOnlySpan<sbyte> pchGuid)
    {
        fixed (sbyte* pName = pchGuid)
        {
            return SDL_GetJoystickGUIDFromString(pName);
        }
    }

    public static Guid SDL_GetJoystickGUIDFromString(string pchGuid)
    {
        return SDL_GetJoystickGUIDFromString(pchGuid.GetUtf8Span());
    }

    public static string SDL_GetJoystickSerialString(SDL_Joystick joystick)
    {
        return GetStringOrEmpty(SDL_GetJoystickSerial(joystick));
    }
}
