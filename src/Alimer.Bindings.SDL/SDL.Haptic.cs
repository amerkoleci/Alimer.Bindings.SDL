// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static SDL3.SDL3;

namespace SDL3;

public enum SDL_HapticEffectType : ushort
{
    Constant = SDL_HAPTIC_CONSTANT,
    Sine = SDL_HAPTIC_SINE,
    LeftRight = SDL_HAPTIC_LEFTRIGHT,
    Triangle = SDL_HAPTIC_TRIANGLE,
    SawToothUp = SDL_HAPTIC_SAWTOOTHUP,
    SawToothDown = SDL_HAPTIC_SAWTOOTHDOWN,
    Spring = SDL_HAPTIC_SPRING,
    Damper = SDL_HAPTIC_DAMPER,
    Inertia = SDL_HAPTIC_INERTIA,
    Friction = SDL_HAPTIC_FRICTION,
    Custom = SDL_HAPTIC_CUSTOM,
    Gain = SDL_HAPTIC_GAIN,
    AutoCenter = SDL_HAPTIC_AUTOCENTER,
    Status = SDL_HAPTIC_STATUS,
    Pause = SDL_HAPTIC_PAUSE,
}

public enum SDL_HapticDirectionType : byte
{
    Polar = SDL_HAPTIC_POLAR,
    Cartesian = SDL_HAPTIC_CARTESIAN,
    Spherical = SDL_HAPTIC_SPHERICAL,
    SteeringAxis = SDL_HAPTIC_STEERING_AXIS,
}

unsafe partial class SDL3
{
    /* SDL_HapticEffect type */
    public const ushort SDL_HAPTIC_CONSTANT = (1 << 0);
    public const ushort SDL_HAPTIC_SINE = (1 << 1);
    public const ushort SDL_HAPTIC_LEFTRIGHT = (1 << 2);
    public const ushort SDL_HAPTIC_TRIANGLE = (1 << 3);
    public const ushort SDL_HAPTIC_SAWTOOTHUP = (1 << 4);
    public const ushort SDL_HAPTIC_SAWTOOTHDOWN = (1 << 5);
    public const ushort SDL_HAPTIC_SPRING = (1 << 7);
    public const ushort SDL_HAPTIC_DAMPER = (1 << 8);
    public const ushort SDL_HAPTIC_INERTIA = (1 << 9);
    public const ushort SDL_HAPTIC_FRICTION = (1 << 10);
    public const ushort SDL_HAPTIC_CUSTOM = (1 << 11);
    public const ushort SDL_HAPTIC_GAIN = (1 << 12);
    public const ushort SDL_HAPTIC_AUTOCENTER = (1 << 13);
    public const ushort SDL_HAPTIC_STATUS = (1 << 14);
    public const ushort SDL_HAPTIC_PAUSE = (1 << 15);

    /* SDL_HapticDirection type */
    public const byte SDL_HAPTIC_POLAR = 0;
    public const byte SDL_HAPTIC_CARTESIAN = 1;
    public const byte SDL_HAPTIC_SPHERICAL = 2;
    public const byte SDL_HAPTIC_STEERING_AXIS = 3;

    /* SDL_HapticRunEffect */
    public const uint SDL_HAPTIC_INFINITY = 4294967295U;

    public static ReadOnlySpan<SDL_HapticID> SDL_GetHaptics()
    {
        SDL_HapticID* ptr = SDL_GetHaptics(out int count);
        return new(ptr, count);
    }

    public static string SDL_GetHapticNameForIDString(SDL_HapticID instance_id)
    {
        return GetStringOrEmpty(SDL_GetHapticNameForID(instance_id));
    }

    public static string SDL_GetHapticNameString(SDL_Haptic haptic)
    {
        return GetStringOrEmpty(SDL_GetHapticName(haptic));
    }
}
