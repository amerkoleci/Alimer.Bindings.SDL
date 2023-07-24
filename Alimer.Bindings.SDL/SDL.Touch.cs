// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

public enum SDL_TouchDeviceType
{
    SDL_TOUCH_DEVICE_INVALID = -1,
    SDL_TOUCH_DEVICE_DIRECT,            /* touch screen with window-relative coordinates */
    SDL_TOUCH_DEVICE_INDIRECT_ABSOLUTE, /* trackpad with absolute device coordinates */
    SDL_TOUCH_DEVICE_INDIRECT_RELATIVE  /* trackpad with screen cursor-relative coordinates */
}

public struct SDL_Finger
{
    public long id; // SDL_FingerID
    public float x;
    public float y;
    public float pressure;
}

unsafe partial class SDL
{
    public const uint SDL_TOUCH_MOUSEID = uint.MaxValue;
    public const uint SDL_MOUSE_TOUCHID = uint.MaxValue;

    /// <summary>
    /// Get the number of registered touch devices.
    /// </summary>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumTouchDevices();

    /**
     *  \brief Get the touch ID with the given index, or 0 if the index is invalid.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_TouchID SDL_GetTouchDevice(int index);

    /// <summary>
    /// Get the number of active fingers for a given touch device.
    /// </summary>
    /// <param name="touchID"></param>
    /// <returns></returns>
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumTouchFingers(SDL_TouchID touchID);

    /**
     *  \brief Get the finger object of the given touch, with the given index.
     *  Returns pointer to SDL_Finger.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Finger* SDL_GetTouchFinger(SDL_TouchID touchID, int index);

    /* Only available in 2.0.10 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_TouchDeviceType SDL_GetTouchDeviceType(SDL_TouchID touchID);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetTouchName), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetTouchName(int index);

    public static string SDL_GetTouchName(int index)
    {
        return GetString(INTERNAL_SDL_GetTouchName(index));
    }
}
