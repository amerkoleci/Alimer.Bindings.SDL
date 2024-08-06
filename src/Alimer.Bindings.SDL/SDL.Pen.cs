// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

[Flags]
public enum SDL_PenCapabilityFlags : uint
{
    Down = SDL3.SDL_PEN_DOWN_MASK,
    Ink = SDL3.SDL_PEN_INK_MASK,
    Eraser = SDL3.SDL_PEN_ERASER_MASK,
    Pressure = SDL3.SDL_PEN_AXIS_PRESSURE_MASK,
    SDL_PEN_AXIS_XTILT_MASK = SDL3.SDL_PEN_AXIS_XTILT_MASK,
    SDL_PEN_AXIS_YTILT_MASK = SDL3.SDL_PEN_AXIS_YTILT_MASK,
    SDL_PEN_AXIS_DISTANCE_MASK = SDL3.SDL_PEN_AXIS_DISTANCE_MASK,
    SDL_PEN_AXIS_ROTATION_MASK = SDL3.SDL_PEN_AXIS_ROTATION_MASK,
    SDL_PEN_AXIS_SLIDER_MASK = SDL3.SDL_PEN_AXIS_SLIDER_MASK,
    SDL_PEN_AXIS_BIDIRECTIONAL_MASKS = SDL3.SDL_PEN_AXIS_BIDIRECTIONAL_MASKS,
}

unsafe partial class SDL3
{
    public const SDL_MouseID SDL_PEN_MOUSEID = unchecked((SDL_MouseID)(-2));

    public const SDL_PenID SDL_PEN_INVALID = ((SDL_PenID)(0));

    public const int SDL_PEN_INFO_UNKNOWN = (-1);

    public const uint SDL_PEN_DOWN_MASK = (1U << (13));

    public const uint SDL_PEN_INK_MASK = (1U << (14));

    public const uint SDL_PEN_ERASER_MASK = (1U << (15));

    public const uint SDL_PEN_AXIS_PRESSURE_MASK = (1U << ((int)(SDL_PEN_AXIS_PRESSURE) + 16));

    public const uint SDL_PEN_AXIS_XTILT_MASK = (1U << ((int)(SDL_PEN_AXIS_XTILT) + 16));

    public const uint SDL_PEN_AXIS_YTILT_MASK = (1U << ((int)(SDL_PEN_AXIS_YTILT) + 16));

    public const uint SDL_PEN_AXIS_DISTANCE_MASK = (1U << ((int)(SDL_PEN_AXIS_DISTANCE) + 16));

    public const uint SDL_PEN_AXIS_ROTATION_MASK = (1U << ((int)(SDL_PEN_AXIS_ROTATION) + 16));

    public const uint SDL_PEN_AXIS_SLIDER_MASK = (1U << ((int)(SDL_PEN_AXIS_SLIDER) + 16));

    public const uint SDL_PEN_AXIS_BIDIRECTIONAL_MASKS = ((1U << ((int)(SDL_PEN_AXIS_XTILT) + 16)) | (1U << ((int)(SDL_PEN_AXIS_YTILT) + 16)));

    public static SDL_PenCapabilityFlags SDL_PEN_CAPABILITY(int capbit) => (SDL_PenCapabilityFlags)(1ul << (capbit));

    public static SDL_PenCapabilityFlags SDL_PEN_AXIS_CAPABILITY(SDL_PenAxis axis) => SDL_PEN_CAPABILITY((int)axis + SDL_PEN_FLAG_AXIS_BIT_OFFSET);

    public static unsafe ReadOnlySpan<SDL_PenID> SDL_GetPens()
    {
        SDL_PenID* ptr = SDL_GetPens(out int count);
        return new(ptr, count);
    }
}
