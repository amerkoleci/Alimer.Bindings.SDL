// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SDL3;

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

unsafe partial class SDL3
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

    public static string[] SDL_GetGamepadMappings()
    {
        byte** strings = SDL_GetGamepadMappings(out int count);
        string[] names = new string[count];
        for (int i = 0; i < count; i++)
        {
            names[i] = ConvertToManaged(strings[i])!;
        }

        return names;
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
            return SDL_SendGamepadEffect(gamepad, (nint)dataPtr, size);
        }
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
