#region License
/* SDL2# - C# Wrapper for SDL2
 *
 * Copyright (c) 2013-2021 Ethan Lee.
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. If you use this software in a
 * product, an acknowledgment in the product documentation would be
 * appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source distribution.
 *
 * Ethan "flibitijibibo" Lee <flibitijibibo@flibitijibibo.com>
 *
 */
#endregion

// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Alimer.Bindings.SDL;

public static unsafe class SDL
{
    private static readonly NativeLibrary s_sdl2Lib = LoadNativeLibrary();

    private static NativeLibrary LoadNativeLibrary()
    {
#if NET6_0_OR_GREATER
        if (OperatingSystem.IsWindows())
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
        {
            return new NativeLibrary("SDL2.dll");
        }
#if NET6_0_OR_GREATER
        else if (OperatingSystem.IsLinux())
#else
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
#endif
        {
            return new NativeLibrary("libSDL2-2.0.so.0", "libSDL2-2.0.so.1", "libSDL2-2.0.so");
        }
#if NET6_0_OR_GREATER
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
#else
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
#endif
        {
            return new NativeLibrary("libSDL2.dylib");
        }
        else
        {
            Debug.WriteLine("Unknown SDL platform. Attempting to load \"SDL2\"");
            return new NativeLibrary("SDL2");
        }
    }

    public enum SDL_bool
    {
        SDL_FALSE = 0,
        SDL_TRUE = 1
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SDL_version
    {
        public byte major;
        public byte minor;
        public byte patch;
    }

    private static readonly delegate* unmanaged[Cdecl]<byte*> s_SDL_GetError = (delegate* unmanaged[Cdecl]<byte*>)s_sdl2Lib.LoadFunction(nameof(SDL_GetError));
    private static readonly delegate* unmanaged[Cdecl]<out SDL_version, void> s_SDL_GetVersion = (delegate* unmanaged[Cdecl]<out SDL_version, void>)s_sdl2Lib.LoadFunction(nameof(SDL_GetVersion));

    public static string? SDL_GetError() => GetString(s_SDL_GetError());
    public static void SDL_GetVersion(out SDL_version version) => s_SDL_GetVersion(out version);

    #region Marshal
    private static string? GetString(byte* ptr)
    {
        if (ptr == null)
        {
            return default;
        }
        int characters = 0;
        while (ptr[characters] != 0)
        {
            characters++;
        }

        return Encoding.UTF8.GetString(ptr, characters);
    }

    /* Used for stack allocated string marshaling. */
    private static int Utf8Size(string str)
    {
        if (str == null)
        {
            return 0;
        }
        return (str.Length * 4) + 1;
    }
    private static unsafe byte* Utf8Encode(string str, byte* buffer, int bufferSize)
    {
        if (str == null)
        {
            return (byte*)0;
        }
        fixed (char* strPtr = str)
        {
            Encoding.UTF8.GetBytes(strPtr, str.Length + 1, buffer, bufferSize);
        }
        return buffer;
    }
    #endregion
}
