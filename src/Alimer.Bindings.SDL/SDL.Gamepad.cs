// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL;

public partial struct SDL_GamepadBinding
{
    [UnscopedRef]
    public ref int input_button
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return ref input.button;
        }
    }

    [UnscopedRef]
    public ref SDL_GamepadBinding_input.SDL_GamepadBinding_axis input_axis
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return ref input.axis;
        }
    }

    [UnscopedRef]
    public ref SDL_GamepadBinding_input.SDL_GamepadBinding_hat input_hat
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return ref input.hat;
        }
    }

    [UnscopedRef]
    public ref SDL_GamepadButton output_button
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return ref output.button;
        }
    }

    [UnscopedRef]
    public ref SDL_GamepadBinding_output.SDL_GamepadBinding_axis output_axis
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return ref output.axis;
        }
    }
}

unsafe partial class SDL
{
    public static ReadOnlySpan<SDL_JoystickID> SDL_GetGamepads()
    {
        SDL_JoystickID* ptr = SDL_GetGamepads(out int count);
        return new(ptr, count);
    }

    //public static ReadOnlySpan<SDL_GamepadBinding> SDL_GetGamepadBindings(SDL_Gamepad gamepad)
    //{
    //    SDL_GamepadBinding* ptr = SDL_GetGamepadBindings(gamepad, out int count);
    //    return new(ptr, count);
    //}

    public static int SDL_AddGamepadMapping(ReadOnlySpan<sbyte> name)
    {
        fixed (sbyte* pName = name)
        {
            return SDL_AddGamepadMapping(pName);
        }
    }

    public static int SDL_AddGamepadMapping(string mappingString)
    {
        return SDL_AddGamepadMapping(mappingString.GetUtf8Span());
    }

    public static string[] SDL_GetGamepadMappings()
    {
        sbyte** strings = SDL_GetGamepadMappings(out int count);
        string[] names = new string[count];
        for (int i = 0; i < count; i++)
        {
            names[i] = GetString(strings[i])!;
        }

        return names;
    }

    /* THIS IS AN RWops FUNCTION! */
    public static int SDL_AddGamepadMappingsFromFile(string file)
    {
        IntPtr rwops = SDL_RWFromFile(file, "rb");
        return SDL_AddGamepadMappingsFromRW(rwops, SDL_bool.SDL_TRUE);
    }

    public static string SDL_GetGamepadMappingForGUIDString(Guid guid)
    {
        return GetString(SDL_GetGamepadMappingForGUID(guid)) ?? string.Empty;
    }

    public static string SDL_GetGamepadMappingStringString(SDL_Gamepad gamepad)
    {
        return GetStringOrEmpty(SDL_GetGamepadMapping(gamepad));
    }

    public static string SDL_GetGamepadInstanceNameString(SDL_JoystickID instance_id)
    {
        return GetStringOrEmpty(SDL_GetGamepadInstanceName(instance_id));
    }

    public static string SDL_GetGamepadInstanceMappingString(SDL_JoystickID instance_id)
    {
        return GetStringOrEmpty(SDL_GetGamepadInstanceMapping(instance_id));
    }

    public static string SDL_GetGamepadNameString(SDL_Gamepad gamepad)
    {
        return GetStringOrEmpty(SDL_GetGamepadName(gamepad));
    }

    public static string SDL_GetGamepadSerialString(SDL_Gamepad gamepad)
    {
        return GetStringOrEmpty(SDL_GetGamepadSerial(gamepad));
    }

    public static SDL_GamepadAxis SDL_GetGamepadAxisFromString(ReadOnlySpan<sbyte> str)
    {
        fixed (sbyte* pName = str)
        {
            return SDL_GetGamepadAxisFromString(pName);
        }
    }

    public static SDL_GamepadAxis SDL_GetGamepadAxisFromString(string str)
    {
        return SDL_GetGamepadAxisFromString(str.GetUtf8Span());
    }

    public static string SDL_GetGamepadStringForAxisString(SDL_GamepadAxis axis)
    {
        return GetStringOrEmpty(SDL_GetGamepadStringForAxis(axis));
    }

    public static SDL_GamepadButton SDL_GetGamepadButtonFromString(ReadOnlySpan<sbyte> str)
    {
        fixed (sbyte* pName = str)
        {
            return SDL_GetGamepadButtonFromString(pName);
        }
    }

    public static SDL_GamepadButton SDL_GetGamepadButtonFromString(string str)
    {
        return SDL_GetGamepadButtonFromString(str.GetUtf8Span());
    }

    public static string SDL_GetGamepadStringForButtonString(SDL_GamepadButton button)
    {
        return GetStringOrEmpty(SDL_GetGamepadStringForButton(button));
    }

    public static string SDL_GetGamepadAppleSFSymbolsNameForButtonString(SDL_Gamepad gamepad, SDL_GamepadButton button)
    {
        return GetString(SDL_GetGamepadAppleSFSymbolsNameForButton(gamepad, button)) ?? string.Empty;
    }

    public static string SDL_GetGamepadAppleSFSymbolsNameForAxisString(SDL_Gamepad gamepad, SDL_GamepadAxis axis)
    {
        return GetString(SDL_GetGamepadAppleSFSymbolsNameForAxis(gamepad, axis)) ?? string.Empty;
    }

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

    public static SDL_GamepadType SDL_GetGamepadTypeFromString(ReadOnlySpan<sbyte> str)
    {
        fixed (sbyte* pName = str)
        {
            return SDL_GetGamepadTypeFromString(pName);
        }
    }

    public static SDL_GamepadType SDL_GetGamepadTypeFromString(string str)
    {
        return SDL_GetGamepadTypeFromString(str.GetUtf8Span());
    }

    public static string SDL_GetGamepadStringForTypeString(SDL_GamepadType type)
    {
        return GetStringOrEmpty(SDL_GetGamepadStringForType(type));
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
