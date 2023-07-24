// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace SDL;

public enum SDL_JoystickPowerLevel
{
    SDL_JOYSTICK_POWER_UNKNOWN = -1,
    SDL_JOYSTICK_POWER_EMPTY,   /* <= 5% */
    SDL_JOYSTICK_POWER_LOW,     /* <= 20% */
    SDL_JOYSTICK_POWER_MEDIUM,  /* <= 70% */
    SDL_JOYSTICK_POWER_FULL,    /* <= 100% */
    SDL_JOYSTICK_POWER_WIRED,
    SDL_JOYSTICK_POWER_MAX
}

public enum SDL_JoystickType
{
    SDL_JOYSTICK_TYPE_UNKNOWN,
    SDL_JOYSTICK_TYPE_GAMEPAD,
    SDL_JOYSTICK_TYPE_WHEEL,
    SDL_JOYSTICK_TYPE_ARCADE_STICK,
    SDL_JOYSTICK_TYPE_FLIGHT_STICK,
    SDL_JOYSTICK_TYPE_DANCE_PAD,
    SDL_JOYSTICK_TYPE_GUITAR,
    SDL_JOYSTICK_TYPE_DRUM_KIT,
    SDL_JOYSTICK_TYPE_ARCADE_PAD,
    SDL_JOYSTICK_TYPE_THROTTLE
}

unsafe partial class SDL
{
    public const byte SDL_HAT_CENTERED = 0x00;
    public const byte SDL_HAT_UP = 0x01;
    public const byte SDL_HAT_RIGHT = 0x02;
    public const byte SDL_HAT_DOWN = 0x04;
    public const byte SDL_HAT_LEFT = 0x08;
    public const byte SDL_HAT_RIGHTUP = SDL_HAT_RIGHT | SDL_HAT_UP;
    public const byte SDL_HAT_RIGHTDOWN = SDL_HAT_RIGHT | SDL_HAT_DOWN;
    public const byte SDL_HAT_LEFTUP = SDL_HAT_LEFT | SDL_HAT_UP;
    public const byte SDL_HAT_LEFTDOWN = SDL_HAT_LEFT | SDL_HAT_DOWN;

    /* Only available in 2.0.14 or higher. */
    public const float SDL_IPHONE_MAX_GFORCE = 5.0f;

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_RumbleJoystick(
        SDL_Joystick joystick,
        ushort low_frequency_rumble,
        ushort high_frequency_rumble,
        uint duration_ms
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_RumbleJoystickTriggers(
        SDL_Joystick joystick,
        ushort left_rumble,
        ushort right_rumble,
        uint duration_ms
    );

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_CloseJoystick(SDL_Joystick joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetJoystickEventsEnabled(SDL_bool state);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickEventsEnabled();

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern short SDL_GetJoystickAxis(
        SDL_Joystick joystick,
        int axis
    );

    /* joystick refers to an SDL_Joystick*.
     * Only available in 2.0.6 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GetJoystickAxisInitialState(
        SDL_Joystick joystick,
        int axis,
        out short state
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte SDL_GetJoystickButton(
        SDL_Joystick joystick,
        int button
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GetJoystickFirmwareVersion(SDL_Joystick joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte SDL_GetJoystickHat(SDL_Joystick joystick, int hat);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetJoystickName), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_JoystickName(SDL_Joystick joystick);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetJoystickInstanceName), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetJoystickInstanceName(SDL_JoystickID joystick);

    public static string SDL_GetJoystickName(SDL_Joystick joystick)
    {
        return GetString(INTERNAL_SDL_JoystickName(joystick));
    }

    public static string SDL_GetJoystickInstanceName(SDL_JoystickID instance_id)
    {
        return GetString(INTERNAL_SDL_GetJoystickInstanceName(instance_id));
    }

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumJoystickAxes(SDL_Joystick joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumJoystickHats(SDL_Joystick joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumJoystickButtons(SDL_Joystick joystick);


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Joystick SDL_OpenJoystick(SDL_JoystickID instance_id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UpdateJoysticks();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_JoystickID* SDL_GetJoysticks(out int count);

    public static ReadOnlySpan<SDL_JoystickID> SDL_GetJoysticks()
    {
        SDL_JoystickID* ptr = SDL_GetJoysticks(out int count);
        return new(ptr, count);
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Guid SDL_GetJoystickInstanceGUID(SDL_JoystickID instance_id);

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Guid SDL_GetJoystickGUID(SDL_Joystick joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetJoystickGUIDString(Guid guid, byte* pszGUID, int cbGUID);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetJoystickGUIDFromString), CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe Guid INTERNAL_SDL_JoystickGetGUIDFromString(byte* pchGUID);

    public static unsafe Guid SDL_GetJoystickGUIDFromString(string pchGuid)
    {
        int utf8PchGuidBufSize = Utf8Size(pchGuid);
        byte* utf8PchGuid = stackalloc byte[utf8PchGuidBufSize];
        return INTERNAL_SDL_JoystickGetGUIDFromString(
            Utf8Encode(pchGuid, utf8PchGuid, utf8PchGuidBufSize)
        );
    }

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GetJoystickInstanceVendor(SDL_JoystickID instance_id);

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GetJoystickInstanceProduct(SDL_JoystickID instance_id);

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GetJoystickInstanceProductVersion(SDL_JoystickID instance_id);

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_JoystickType SDL_GetJoystickInstanceType(SDL_JoystickID instance_id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GetJoystickVendor(SDL_Joystick joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GetJoystickProduct(SDL_Joystick joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GetJoystickProductVersion(SDL_Joystick joystick);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetJoystickSerial), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetJoystickSerial(SDL_Joystick joystick);

    public static string SDL_GetJoystickSerial(SDL_Joystick joystick)
    {
        return GetString(
            INTERNAL_SDL_GetJoystickSerial(joystick)
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_JoystickType SDL_JoystickGetType(SDL_Joystick joystick);

    /* joystick refers to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickConnected(SDL_Joystick joystick);

    /* int refers to an SDL_JoystickID, joystick to an SDL_Joystick* */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_JoystickID SDL_GetJoystickInstanceID(SDL_Joystick joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_JoystickPowerLevel SDL_GetJoystickPowerLevel(SDL_Joystick joystick);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Joystick SDL_GetJoystickFromInstanceID(SDL_JoystickID instance_id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_LockJoysticks();

    /* Only available in 2.0.7 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UnlockJoysticks();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Joystick SDL_GetJoystickFromPlayerIndex(int player_index);

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.11 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetJoystickPlayerIndex(SDL_Joystick joystick, int player_index);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_JoystickID SDL_AttachVirtualJoystick(
        SDL_JoystickType type,
        int naxes,
        int nbuttons,
        int nhats
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_DetachVirtualJoystick(SDL_JoystickID instance_id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_IsJoystickVirtual(SDL_JoystickID instance_id);

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetJoystickVirtualAxis(
        SDL_Joystick joystick,
        int axis,
        short value
    );

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetJoystickVirtualButton(
        SDL_Joystick joystick,
        int button,
        byte value
    );

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetJoystickVirtualHat(
        SDL_Joystick joystick,
        int hat,
        byte value
    );

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickHasLED(SDL_Joystick joystick);

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickHasRumble(SDL_Joystick joystick);

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.18 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_JoystickHasRumbleTriggers(SDL_Joystick joystick);

    /* IntPtr refers to an SDL_Joystick*.
     * Only available in 2.0.14 or higher.
     */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetJoystickLED(
        SDL_Joystick joystick,
        byte red,
        byte green,
        byte blue
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SendJoystickEffect(SDL_Joystick joystick, void* data, int size);
}
