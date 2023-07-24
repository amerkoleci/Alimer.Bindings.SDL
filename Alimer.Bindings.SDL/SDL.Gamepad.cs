// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

public enum SDL_GamepadAxis
{
    SDL_GAMEPAD_AXIS_INVALID = -1,
    SDL_GAMEPAD_AXIS_LEFTX,
    SDL_GAMEPAD_AXIS_LEFTY,
    SDL_GAMEPAD_AXIS_RIGHTX,
    SDL_GAMEPAD_AXIS_RIGHTY,
    SDL_GAMEPAD_AXIS_LEFT_TRIGGER,
    SDL_GAMEPAD_AXIS_RIGHT_TRIGGER,
    SDL_GAMEPAD_AXIS_MAX
}

public enum SDL_GamepadButton
{
    SDL_GAMEPAD_BUTTON_INVALID = -1,
    SDL_GAMEPAD_BUTTON_A,
    SDL_GAMEPAD_BUTTON_B,
    SDL_GAMEPAD_BUTTON_X,
    SDL_GAMEPAD_BUTTON_Y,
    SDL_GAMEPAD_BUTTON_BACK,
    SDL_GAMEPAD_BUTTON_GUIDE,
    SDL_GAMEPAD_BUTTON_START,
    SDL_GAMEPAD_BUTTON_LEFT_STICK,
    SDL_GAMEPAD_BUTTON_RIGHT_STICK,
    SDL_GAMEPAD_BUTTON_LEFT_SHOULDER,
    SDL_GAMEPAD_BUTTON_RIGHT_SHOULDER,
    SDL_GAMEPAD_BUTTON_DPAD_UP,
    SDL_GAMEPAD_BUTTON_DPAD_DOWN,
    SDL_GAMEPAD_BUTTON_DPAD_LEFT,
    SDL_GAMEPAD_BUTTON_DPAD_RIGHT,
    SDL_GAMEPAD_BUTTON_MISC1,    /* Xbox Series X share button, PS5 microphone button, Nintendo Switch Pro capture button, Amazon Luna microphone button */
    SDL_GAMEPAD_BUTTON_PADDLE1,  /* Xbox Elite paddle P1 (upper left, facing the back) */
    SDL_GAMEPAD_BUTTON_PADDLE2,  /* Xbox Elite paddle P3 (upper right, facing the back) */
    SDL_GAMEPAD_BUTTON_PADDLE3,  /* Xbox Elite paddle P2 (lower left, facing the back) */
    SDL_GAMEPAD_BUTTON_PADDLE4,  /* Xbox Elite paddle P4 (lower right, facing the back) */
    SDL_GAMEPAD_BUTTON_TOUCHPAD, /* PS4/PS5 touchpad button */
    SDL_GAMEPAD_BUTTON_MAX
}

public enum SDL_GamepadType
{
    SDL_GAMEPAD_TYPE_UNKNOWN = 0,
    SDL_GAMEPAD_TYPE_STANDARD,
    SDL_GAMEPAD_TYPE_XBOX360,
    SDL_GAMEPAD_TYPE_XBOXONE,
    SDL_GAMEPAD_TYPE_PS3,
    SDL_GAMEPAD_TYPE_PS4,
    SDL_GAMEPAD_TYPE_PS5,
    SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_PRO,
    SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_LEFT,
    SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_RIGHT,
    SDL_GAMEPAD_TYPE_NINTENDO_SWITCH_JOYCON_PAIR,
    SDL_GAMEPAD_TYPE_MAX
}

unsafe partial class SDL
{
    [DllImport(LibName, EntryPoint = nameof(SDL_AddGamepadMapping), CallingConvention = CallingConvention.Cdecl)]
    private static extern int INTERNAL_SDL_AddGamepadMapping(byte* mappingString);

    public static int SDL_AddGamepadMapping(string mappingString
    )
    {
        byte* utf8MappingString = Utf8EncodeHeap(mappingString);
        int result = INTERNAL_SDL_AddGamepadMapping(
            utf8MappingString
        );

        NativeMemory.Free(utf8MappingString);
        return result;
    }

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumGamepadMappings();

    /* Only available in 2.0.6 or higher. */
    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadMappingForIndex), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadMappingForIndex(int mapping_index);
    public static string SDL_GetGamepadMappingForIndex(int mapping_index)
    {
        return GetString(INTERNAL_SDL_GetGamepadMappingForIndex(mapping_index), true);
    }

    /* THIS IS AN RWops FUNCTION! */
    [DllImport(LibName, EntryPoint = "SDL_AddGamepadMappingsFromRW", CallingConvention = CallingConvention.Cdecl)]
    private static extern int INTERNAL_SDL_AddGamepadMappingsFromRW(
        IntPtr rw,
        int freerw
    );
    public static int SDL_AddGamepadMappingsFromFile(string file)
    {
        IntPtr rwops = SDL_RWFromFile(file, "rb");
        return INTERNAL_SDL_AddGamepadMappingsFromRW(rwops, 1);
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadMappingForGUID), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadMappingForGUIDD(
        Guid guid
    );
    public static string SDL_GetGamepadMappingForGUID(Guid guid)
    {
        return GetString(
            INTERNAL_SDL_GetGamepadMappingForGUIDD(guid),
            true
        );
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadMapping), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadMapping(SDL_Gamepad gamepad);

    public static string SDL_GetGamepadMapping(SDL_Gamepad gamepad)
    {
        return GetString(
            INTERNAL_SDL_GetGamepadMapping(
                gamepad
            ),
            true
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_IsGamepad(SDL_JoystickID instance_id);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadInstanceName), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadInstanceName(SDL_JoystickID instance_id);

    public static string SDL_GetGamepadInstanceName(SDL_JoystickID instance_id)
    {
        return GetString(
            INTERNAL_SDL_GetGamepadInstanceName(instance_id)
        );
    }

    /* Only available in 2.0.9 or higher. */
    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadInstanceMapping), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadInstanceMapping(SDL_JoystickID instance_id);

    public static string SDL_GetGamepadInstanceMapping(SDL_JoystickID instance_id)
    {
        return GetString(
            INTERNAL_SDL_GetGamepadInstanceMapping(instance_id),
            true
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Gamepad SDL_OpenGamepad(SDL_JoystickID instance_id);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadName), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadName(SDL_Gamepad gamepad);

    public static string SDL_GetGamepadName(SDL_Gamepad gamepad)
    {
        return GetString(
            INTERNAL_SDL_GetGamepadName(gamepad)
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GetGamepadVendor(SDL_Gamepad gamepad);


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GetGamepadProduct(SDL_Gamepad gamepad);


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ushort SDL_GetGamepadProductVersion(SDL_Gamepad gamepad);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadSerial), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadSerial(SDL_Gamepad gamepad);

    public static string SDL_GetGamepadSerial(SDL_Gamepad gamepad)
    {
        return GetString(
            INTERNAL_SDL_GetGamepadSerial(gamepad)
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GamepadConnected(SDL_Gamepad gamepad);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Joystick SDL_GetGamepadJoystick(SDL_Gamepad gamepad);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetGamepadEventsEnabled(SDL_bool enabled);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GamepadEventsEnabled();

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UpdateGamepads();

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadAxisFromString), CallingConvention = CallingConvention.Cdecl)]
    private static extern unsafe SDL_GamepadAxis INTERNAL_SDL_GetGamepadAxisFromString(byte* str);

    public static unsafe SDL_GamepadAxis SDL_GetGamepadAxisFromString(
        string str
    )
    {
        int utf8PchStringBufSize = Utf8Size(str);
        byte* utf8PchString = stackalloc byte[utf8PchStringBufSize];
        return INTERNAL_SDL_GetGamepadAxisFromString(
            Utf8Encode(str, utf8PchString, utf8PchStringBufSize)
        );
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadStringForAxis), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadStringForAxis(SDL_GamepadAxis axis);

    public static string SDL_GetGamepadStringForAxis(SDL_GamepadAxis axis)
    {
        return GetString(
            INTERNAL_SDL_GetGamepadStringForAxis(
                axis
            )
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern short SDL_GetGamepadAxis(SDL_Gamepad gamepad, SDL_GamepadAxis axis);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadButtonFromString), CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_GamepadButton INTERNAL_SDL_GetGamepadButtonFromString(
        byte* str
    );
    public static SDL_GamepadButton SDL_GetGamepadButtonFromString(string str)
    {
        int utf8PchStringBufSize = Utf8Size(str);
        byte* utf8PchString = stackalloc byte[utf8PchStringBufSize];
        return INTERNAL_SDL_GetGamepadButtonFromString(
            Utf8Encode(str, utf8PchString, utf8PchStringBufSize)
        );
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadStringForButton), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadStringForButton(SDL_GamepadButton button);

    public static string SDL_GetGamepadStringForButton(SDL_GamepadButton button)
    {
        return GetString(
            INTERNAL_SDL_GetGamepadStringForButton(button)
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte SDL_GetGamepadButton(SDL_Gamepad gamepad, SDL_GamepadButton button);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_RumbleGamepad(
        SDL_Gamepad gamepad,
        ushort low_frequency_rumble,
        ushort high_frequency_rumble,
        uint duration_ms
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_RumbleGamepadTriggers(
        SDL_Gamepad gamepad,
        ushort left_rumble,
        ushort right_rumble,
        uint duration_ms
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_CloseGamepad(SDL_Gamepad gamepad);

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadAppleSFSymbolsNameForButton), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadAppleSFSymbolsNameForButton(
        SDL_Gamepad gamepad,
        SDL_GamepadButton button
    );
    public static string SDL_GetGamepadAppleSFSymbolsNameForButton(SDL_Gamepad gamepad, SDL_GamepadButton button
    )
    {
        return GetString(
            INTERNAL_SDL_GetGamepadAppleSFSymbolsNameForButton(gamepad, button)
        );
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadAppleSFSymbolsNameForAxis), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadAppleSFSymbolsNameForAxis(
        SDL_Gamepad gamepad,
        SDL_GamepadAxis axis
    );

    public static string SDL_GetGamepadAppleSFSymbolsNameForAxis(SDL_Gamepad gamepad, SDL_GamepadAxis axis)
    {
        return GetString(
            INTERNAL_SDL_GetGamepadAppleSFSymbolsNameForAxis(gamepad, axis)
        );
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Gamepad SDL_GetGamepadFromInstanceID(SDL_JoystickID instance_id);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_GamepadType SDL_GetGamepadInstanceType(
        SDL_JoystickID instance_id
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_GamepadType SDL_GetGamepadType(SDL_Gamepad gamepad);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_GamepadType SDL_GetRealGamepadType(SDL_Gamepad gamepad);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Gamepad SDL_GetGamepadFromPlayerIndex(int player_index);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetGamepadPlayerIndex(SDL_Gamepad gamepad, int player_index);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GamepadHasLED(SDL_Gamepad gamepad);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GamepadHasRumble(SDL_Gamepad gamepad);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GamepadHasRumbleTriggers(SDL_Gamepad gamepad);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetGamepadLED(SDL_Gamepad gamepad, byte red, byte green, byte blue);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GamepadHasAxis(SDL_Gamepad gamepad, SDL_GamepadAxis axis);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GamepadHasButton(SDL_Gamepad gamepad, SDL_GamepadButton button);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumGamepadTouchpads(SDL_Gamepad gamepad);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumGamepadTouchpadFingers(SDL_Gamepad gamepad, int touchpad);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetGamepadTouchpadFinger(
        SDL_Gamepad gamepad,
        int touchpad,
        int finger,
        out byte state,
        out float x,
        out float y,
        out float pressure
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GamepadHasSensor(SDL_Gamepad gamepad, SDL_SensorType type);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetGamepadSensorEnabled(SDL_Gamepad gamepad, SDL_SensorType type, SDL_bool enabled);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_GamepadSensorEnabled(SDL_Gamepad gamepad, SDL_SensorType type);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern float SDL_GetGamepadSensorDataRate(SDL_Gamepad gamepad, SDL_SensorType type);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetGamepadSensorData(SDL_Gamepad gamepad, SDL_SensorType type, float* data, int num_values);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SendGamepadEffect(SDL_Gamepad gamepad, void* data, int size);

    public static int SDL_SendGamepadEffect<T>(SDL_Gamepad gamepad, T[] source) where T : unmanaged
    {
        ReadOnlySpan<T> span = source.AsSpan();

        return SDL_SendGamepadEffect(gamepad, span);
    }

    public static int SDL_SendGamepadEffect<T>(SDL_Gamepad gamepad, ReadOnlySpan<T> data) where T : unmanaged
    {
        return SDL_SendGamepadEffect(gamepad, ref MemoryMarshal.GetReference(data), data.Length * sizeof(T));
    }

    public static int SDL_SendGamepadEffect<T>(SDL_Gamepad gamepad, ref T data, int size) where T : unmanaged
    {
        fixed (void* dataPtr = &data)
        {
            return SDL_SendGamepadEffect(gamepad, dataPtr, size);
        }
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadTypeFromString), CallingConvention = CallingConvention.Cdecl)]
    private static extern SDL_GamepadType INTERNAL_SDL_GetGamepadTypeFromString(byte* str);

    public static SDL_GamepadType SDL_GetGamepadTypeFromString(string str)
    {
        byte* strString = Utf8EncodeHeap(str);
        SDL_GamepadType result = INTERNAL_SDL_GetGamepadTypeFromString(strString);
        NativeMemory.Free(strString);
        return result;
    }

    [DllImport(LibName, EntryPoint = nameof(SDL_GetGamepadStringForType), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetGamepadStringForType(SDL_GamepadType type);
    public static string SDL_GetGamepadStringForType(SDL_GamepadType type)
    {
        return GetString(INTERNAL_SDL_GetGamepadStringForType(type), true);
    }

    public static bool SDL_IsJoystickAmazonLunaController(ushort vendor_id, ushort product_id)
    {
        return ((vendor_id == 0x1949 && product_id == 0x0419) ||
                (vendor_id == 0x0171 && product_id == 0x0419));
    }

    public static bool SDL_IsJoystickGoogleStadiaController(ushort vendor_id, ushort product_id)
    {
        return (vendor_id == 0x18d1 && product_id == 0x9400);
    }

    public static bool SDL_IsJoystickNVIDIASHIELDController(ushort vendor_id, ushort product_id)
    {
        return (vendor_id == 0x0955 && (product_id == 0x7210 || product_id == 0x7214));
    }
}
