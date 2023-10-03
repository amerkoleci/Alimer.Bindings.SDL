// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace SDL;

unsafe partial class SDL
{
    public static ushort SDL_AUDIO_BITSIZE(ushort x)
    {
        return (ushort)(x & SDL_AUDIO_MASK_BITSIZE);
    }

    public static ushort SDL_AUDIO_BYTESIZE(ushort x) => (ushort)(SDL_AUDIO_BITSIZE(x) / 8);

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

    public static ushort SDL_AUDIO_FRAMESIZE(in SDL_AudioSpec spec) => (ushort)(SDL_AUDIO_BYTESIZE(spec.format) * spec.channels);

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
