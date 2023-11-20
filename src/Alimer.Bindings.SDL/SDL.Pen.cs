// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

unsafe partial class SDL
{
    public const uint SDL_PEN_INVALID = 0;
    public const uint SDL_PEN_MOUSEID = unchecked((uint)-2);
	public const int SDL_PEN_INFO_UNKNOWN = -1;

    public static readonly uint SDL_PEN_DOWN_MASK = SDL_PEN_CAPABILITY(SDL_PEN_FLAG_DOWN_BIT_INDEX);
    public static readonly uint SDL_PEN_INK_MASK = SDL_PEN_CAPABILITY(SDL_PEN_FLAG_INK_BIT_INDEX);
    public static readonly uint SDL_PEN_ERASER_MASK = SDL_PEN_CAPABILITY(SDL_PEN_FLAG_ERASER_BIT_INDEX);
    public static readonly uint SDL_PEN_AXIS_PRESSURE_MASK = SDL_PEN_AXIS_CAPABILITY(SDL_PenAxis.Pressure);
    public static readonly uint SDL_PEN_AXIS_XTILT_MASK = SDL_PEN_AXIS_CAPABILITY(SDL_PenAxis.Xtilt);
    public static readonly uint SDL_PEN_AXIS_YTILT_MASK = SDL_PEN_AXIS_CAPABILITY(SDL_PenAxis.Ytilt);
    public static readonly uint SDL_PEN_AXIS_DISTANCE_MASK = SDL_PEN_AXIS_CAPABILITY(SDL_PenAxis.Distance);
    public static readonly uint SDL_PEN_AXIS_ROTATION_MASK = SDL_PEN_AXIS_CAPABILITY(SDL_PenAxis.Rotation);
    public static readonly uint SDL_PEN_AXIS_SLIDER_MASK = SDL_PEN_AXIS_CAPABILITY(SDL_PenAxis.Slider);
    public static readonly uint SDL_PEN_AXIS_BIDIRECTIONAL_MASKS = SDL_PEN_AXIS_XTILT_MASK | SDL_PEN_AXIS_YTILT_MASK;

    public static uint SDL_PEN_CAPABILITY(int capbit) => 1u << (capbit);
    public static uint SDL_PEN_CAPABILITY(uint capbit) => 1u << ((int)capbit);
    public static uint SDL_PEN_AXIS_CAPABILITY(SDL_PenAxis axis) => SDL_PEN_CAPABILITY((int)axis) + SDL_PEN_FLAG_AXIS_BIT_OFFSET;

    public static string SDL_GetPenNameString(SDL_PenID instance_id)
    {
        return GetStringOrEmpty(SDL_GetPenName(instance_id));
    }
}
