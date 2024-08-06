// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

unsafe partial class SDL3
{
    public static ushort SDL_AUDIO_BITSIZE(SDL_AudioFormat x)
    {
        return (ushort)((uint)x & SDL_AUDIO_MASK_BITSIZE);
    }

    public static ushort SDL_AUDIO_BYTESIZE(SDL_AudioFormat x) => (ushort)(SDL_AUDIO_BITSIZE(x) / 8);

    public static bool SDL_AUDIO_ISFLOAT(SDL_AudioFormat x)
    {
        return ((uint)x & SDL_AUDIO_MASK_FLOAT) != 0;
    }

    public static bool SDL_AUDIO_ISBIGENDIAN(SDL_AudioFormat x)
    {
        return ((uint)x & SDL_AUDIO_MASK_BIG_ENDIAN) != 0;
    }

    public static bool SDL_AUDIO_ISSIGNED(SDL_AudioFormat x)
    {
        return ((uint)x & SDL_AUDIO_MASK_SIGNED) != 0;
    }

    public static bool SDL_AUDIO_ISINT(SDL_AudioFormat x) => !SDL_AUDIO_ISFLOAT(x);
    public static bool SDL_AUDIO_ISLITTLEENDIAN(SDL_AudioFormat x) => !SDL_AUDIO_ISBIGENDIAN(x);
    public static bool SDL_AUDIO_ISUNSIGNED(SDL_AudioFormat x) => !SDL_AUDIO_ISSIGNED(x);

    public static int SDL_AUDIO_FRAMESIZE(in SDL_AudioSpec spec) => SDL_AUDIO_BYTESIZE(spec.format) * spec.channels;

    public static ReadOnlySpan<SDL_AudioDeviceID> SDL_GetAudioPlaybackDevices()
    {
        SDL_AudioDeviceID* ptr = SDL_GetAudioPlaybackDevices(out int count);
        return new(ptr, count);
    }

    public static ReadOnlySpan<SDL_AudioDeviceID> SDL_GetAudioRecordingDevices()
    {
        SDL_AudioDeviceID* ptr = SDL_GetAudioRecordingDevices(out int count);
        return new(ptr, count);
    }
}
