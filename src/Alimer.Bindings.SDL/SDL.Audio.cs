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

    public enum SDL_AudioStatus
    {
        SDL_AUDIO_STOPPED,
        SDL_AUDIO_PLAYING,
        SDL_AUDIO_PAUSED
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_AudioSpec
    {
        public int freq;
        public ushort format; // SDL_AudioFormat
        public byte channels;
        public byte silence;
        public ushort samples;
        private ushort padding;
        public uint size;
        public unsafe delegate* unmanaged<IntPtr, byte*, int, void> callback; /* SDL_AudioCallback */
        public IntPtr userdata;
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumAudioDrivers();

    [DllImport(LibName, EntryPoint = nameof(SDL_GetAudioDriver), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetAudioDriver(int index);

    public static string SDL_GetAudioDriver(int index)
    {
        return GetString(INTERNAL_SDL_GetAudioDriver(index));
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_AudioDeviceID SDL_OpenAudioDevice(SDL_AudioDeviceID deviceId, SDL_AudioSpec* spec);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_CloseAudioDevice(SDL_AudioDeviceID dev);


    [DllImport(LibName, EntryPoint = nameof(SDL_GetCurrentAudioDriver), CallingConvention = CallingConvention.Cdecl)]
    private static extern sbyte* INTERNAL_SDL_GetCurrentAudioDriver();

    public static string SDL_GetCurrentAudioDriver()
    {
        return new(INTERNAL_SDL_GetCurrentAudioDriver());
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_AudioDeviceID* SDL_GetAudioOutputDevices(int* count);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_AudioDeviceID* SDL_GetAudioCaptureDevices(int* count);


    [DllImport(LibName, EntryPoint = nameof(SDL_GetAudioDeviceName), CallingConvention = CallingConvention.Cdecl)]
    private static extern byte* INTERNAL_SDL_GetAudioDeviceName(SDL_AudioDeviceID deviceId);

    public static string SDL_GetAudioDeviceName(SDL_AudioDeviceID deviceId)
    {
        return GetString(INTERNAL_SDL_GetAudioDeviceName(deviceId));
    }

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetAudioDeviceFormat(SDL_AudioDeviceID devid, SDL_AudioSpec* spec);

    // TODO: int SDL_GetDefaultAudioInfo(char **name, SDL_AudioSpec* spec, int iscapture)

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_PlayAudioDevice(SDL_AudioDeviceID dev);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_PauseAudioDevice(SDL_AudioDeviceID dev);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_ResumeAudioDevice(SDL_AudioDeviceID dev);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_bool SDL_IsAudioDevicePaused(SDL_AudioDeviceID dev);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_MixAudioFormat(
        byte* dst,
        byte* src,
        ushort format,
        uint len,
        int volume
    );

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_LockAudioDevice(SDL_AudioDeviceID dev);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UnlockAudioDevice(SDL_AudioDeviceID dev);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_QueueAudio(SDL_AudioDeviceID dev, void* data, uint len);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint SDL_DequeueAudio(SDL_AudioDeviceID dev, void* data, uint len);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern uint SDL_GetQueuedAudioSize(SDL_AudioDeviceID dev);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_ClearQueuedAudio(SDL_AudioDeviceID dev);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_ConvertAudioSamples(
        ushort format,
        uint src_channels,
        int src_rate,
        byte** src_data,
        int src_len,
        ushort dst_format,
        byte dst_channels,
        int dst_rate,
        byte** dst_data,
        int* dst_len);

    [LibraryImport(LibName, EntryPoint = nameof(SDL_LoadWAV))]
    private static partial SDL_AudioSpec* INTERNAL_SDL_LoadWAV_RW(
        IntPtr src,
        int freesrc,
        SDL_AudioSpec* spec,
        out byte* audio_buf,
        out uint audio_len
    );

    public static SDL_AudioSpec* SDL_LoadWAV(
        string file,
        SDL_AudioSpec* spec,
        out byte* audio_buf,
        out uint audio_len
    )
    {
        IntPtr rwops = SDL_RWFromFile(file, "rb");
        return INTERNAL_SDL_LoadWAV_RW(
            rwops,
            1,
            spec,
            out audio_buf,
            out audio_len
        );
    }


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_CreateAudioStream(SDL_AudioSpec* src_spec, SDL_AudioSpec* dst_spec);


    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_DestroyAudioStream(IntPtr stream);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_ClearAudioStream(IntPtr stream);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_FlushAudioStream(IntPtr stream);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetAudioStreamAvailable(IntPtr stream);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetAudioStreamData(IntPtr stream, void* buf, int len);

    [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_PutAudioStreamData(IntPtr stream, void* buf, int len);
}
