// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace SDL;

unsafe partial class SDL
{
    public const ushort SDL_AUDIO_MASK_BITSIZE = 0xFF;
    public const ushort SDL_AUDIO_MASK_FLOAT = 1 << 8;
    public const ushort SDL_AUDIO_MASK_BIG_ENDIAN = (1 << 12);
    public const ushort SDL_AUDIO_MASK_SIGNED = (1 << 15);

    public static ushort SDL_AUDIO_BITSIZE(ushort x)
    {
        return (ushort)(x & SDL_AUDIO_MASK_BITSIZE);
    }

    public static bool SDL_AUDIO_ISFLOAT(ushort x)
    {
        return (x & SDL_AUDIO_MASK_FLOAT) != 0;
    }

    public static bool SDL_AUDIO_ISBIGENDIAN(ushort x)
    {
        return (x & SDL_AUDIO_MASK_BIG_ENDIAN) != 0;
    }

    public static bool SDL_AUDIO_ISSIGNED(ushort x)
    {
        return (x & SDL_AUDIO_MASK_SIGNED) != 0;
    }

    public static bool SDL_AUDIO_ISINT(ushort x) => !SDL_AUDIO_ISFLOAT(x);
    public static bool SDL_AUDIO_ISLITTLEENDIAN(ushort x) => !SDL_AUDIO_ISBIGENDIAN(x);
    public static bool SDL_AUDIO_ISUNSIGNED(ushort x) => !SDL_AUDIO_ISSIGNED(x);

    public const ushort AUDIO_U8 = 0x0008;
    public const ushort AUDIO_S8 = 0x8008;
    public const ushort AUDIO_S16LSB = 0x8010;
    public const ushort AUDIO_S16MSB = 0x9010;
    public const ushort AUDIO_S16 = AUDIO_S16LSB;
    public const ushort AUDIO_S32LSB = 0x8020;
    public const ushort AUDIO_S32MSB = 0x9020;
    public const ushort AUDIO_S32 = AUDIO_S32LSB;
    public const ushort AUDIO_F32LSB = 0x8120;
    public const ushort AUDIO_F32MSB = 0x9120;
    public const ushort AUDIO_F32 = AUDIO_F32LSB;

    public static readonly ushort AUDIO_S16SYS =
        BitConverter.IsLittleEndian ? AUDIO_S16LSB : AUDIO_S16MSB;
    public static readonly ushort AUDIO_S32SYS =
        BitConverter.IsLittleEndian ? AUDIO_S32LSB : AUDIO_S32MSB;
    public static readonly ushort AUDIO_F32SYS =
        BitConverter.IsLittleEndian ? AUDIO_F32LSB : AUDIO_F32MSB;

    public const uint SDL_AUDIO_ALLOW_FREQUENCY_CHANGE = 0x00000001;
    public const uint SDL_AUDIO_ALLOW_FORMAT_CHANGE = 0x00000002;
    public const uint SDL_AUDIO_ALLOW_CHANNELS_CHANGE = 0x00000004;
    public const uint SDL_AUDIO_ALLOW_SAMPLES_CHANGE = 0x00000008;
    public const uint SDL_AUDIO_ALLOW_ANY_CHANGE = (
        SDL_AUDIO_ALLOW_FREQUENCY_CHANGE |
        SDL_AUDIO_ALLOW_FORMAT_CHANGE |
        SDL_AUDIO_ALLOW_CHANNELS_CHANGE |
        SDL_AUDIO_ALLOW_SAMPLES_CHANGE
    );

    public const int SDL_MIX_MAXVOLUME = 128;

    public static string SDL_GetAudioDriverString(int index)
    {
        return GetStringOrEmpty(SDL_GetAudioDriver(index));
    }


    public static string SDL_GetCurrentAudioDriverString()
    {
        return GetStringOrEmpty(SDL_GetCurrentAudioDriver());
    }

    public static ReadOnlySpan<SDL_AudioDeviceID> SDL_GetAudioOutputDevices()
    {
        SDL_AudioDeviceID* ptr = SDL_GetAudioOutputDevices(out int count);
        return new(ptr, count);
    }

    public static ReadOnlySpan<SDL_AudioDeviceID> SDL_GetAudioCaptureDevices()
    {
        SDL_AudioDeviceID* ptr = SDL_GetAudioCaptureDevices(out int count);
        return new(ptr, count);
    }

    public static string SDL_GetAudioDeviceNameString(SDL_AudioDeviceID deviceId)
    {
        return GetStringOrEmpty(SDL_GetAudioDeviceName(deviceId));
    }

    public static int SDL_LoadWAV(ReadOnlySpan<sbyte> path, SDL_AudioSpec* spec, byte** audio_buf, uint* audio_len)
    {
        fixed (sbyte* pPath = path)
        {
            return SDL_LoadWAV(pPath, spec, audio_buf, audio_len);
        }
    }

    public static int SDL_LoadWAV(string path, SDL_AudioSpec* spec, byte** audio_buf, uint* audio_len)
    {
        fixed (sbyte* pPath = path.GetUtf8Span())
        {
            return SDL_LoadWAV(pPath, spec, audio_buf, audio_len);
        }
    }
}
