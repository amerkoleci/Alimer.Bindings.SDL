// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
#if NET6_0_OR_GREATER
using SystemNativeLibrary = System.Runtime.InteropServices.NativeLibrary;
#endif

namespace Alimer.Native.SDL;

internal sealed class NativeLibrary : IDisposable
{
    private static readonly ILibraryLoader _loader = GetPlatformDefaultLoader();

    public NativeLibrary(params string[] names)
    {
        foreach (string name in names)
        {
            foreach (string path in EnumeratePossibleLibraryLoadTargets(name))
            {
                Handle = _loader.LoadNativeLibrary(path);

                if (Handle != 0)
                    break;
            }
        }
    }

    /// <summary>
    /// The operating system handle of the loaded library.
    /// </summary>
    public nint Handle { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        _loader.FreeNativeLibrary(Handle);
    }

    private static IEnumerable<string> EnumeratePossibleLibraryLoadTargets(string name)
    {
        if (!string.IsNullOrEmpty(AppContext.BaseDirectory))
        {
            yield return Path.Combine(AppContext.BaseDirectory, name);
        }
        if (TryLocateNativeAssetInPlatformFolder(name, out string? platformResolvedPath))
        {
            yield return platformResolvedPath!;
        }
        yield return name;
    }

    private static bool TryLocateNativeAssetInPlatformFolder(string name, out string? platformResolvedPath)
    {
        foreach (string rid in GetRuntimeIdentifiers())
        {
            string nativeLibsPath = Path.Combine(AppContext.BaseDirectory, $@"runtimes\{rid}\native");

            if (Directory.Exists(nativeLibsPath))
            {
                platformResolvedPath = Path.Combine(nativeLibsPath, name);
                return true;
            }
        }

        platformResolvedPath = null;
        return false;
    }

    public static IEnumerable<string> GetRuntimeIdentifiers()
    {
#if NET6_0_OR_GREATER
        yield return RuntimeInformation.RuntimeIdentifier;
#endif
        var arch = RuntimeInformation.ProcessArchitecture;
        bool isArm = RuntimeInformation.ProcessArchitecture == Architecture.Arm || RuntimeInformation.ProcessArchitecture == Architecture.Arm64;

        string archName = (IntPtr.Size == 8)
            ? isArm ? "arm64" : "x64"
            : isArm ? "arm" : "x86";

#if NET6_0_OR_GREATER
        if (OperatingSystem.IsWindows())
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
        {
            yield return $"win10-{archName}";
            yield return $"win-{archName}";
        }
#if NET6_0_OR_GREATER
        else if (OperatingSystem.IsLinux())
#else
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
#endif
        {
            yield return $"linux-{archName}";
        }
#if NET6_0_OR_GREATER
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
#else
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
#endif
        {
            yield return "osx-universal";
            yield return $"linux-{archName}";
        }

        yield return "unknown";
    }

    /// <summary>
    /// Loads a function pointer with the given name.
    /// </summary>
    /// <param name="name">The name of the native export.</param>
    /// <returns>A function pointer for the given name, or 0 if no function with that name exists.</returns>
    public IntPtr LoadFunction(string name)
    {
        return _loader.LoadFunctionPointer(Handle, name);
    }

    /// <summary>
    /// Loads a function whose signature matches the given delegate type's signature.
    /// </summary>
    /// <typeparam name="T">The type of delegate to return.</typeparam>
    /// <param name="name">The name of the native export.</param>
    /// <returns>A delegate wrapping the native function.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no function with the given name
    /// is exported from the native library.</exception>
    public T LoadFunction<T>(string name)
    {
        IntPtr functionPtr = _loader.LoadFunctionPointer(Handle, name);
        if (functionPtr == IntPtr.Zero)
        {
            throw new InvalidOperationException($"No function was found with the name {name}.");
        }

        return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
    }


    private static ILibraryLoader GetPlatformDefaultLoader()
    {
#if NET6_0_OR_GREATER
        return new SystemNativeLibraryLoader();
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new Win32LibraryLoader();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
            RuntimeInformation.OSDescription.ToUpper().Contains("BSD"))
        {
            return new BsdLibraryLoader();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new UnixLibraryLoader();
        }

        throw new PlatformNotSupportedException("This platform cannot load native libraries.");
#endif
    }

    private interface ILibraryLoader
    {
        nint LoadNativeLibrary(string name);

        void FreeNativeLibrary(nint handle);

        nint LoadFunctionPointer(nint handle, string name);
    }

#if NET6_0_OR_GREATER
    private class SystemNativeLibraryLoader : ILibraryLoader
    {
        public nint LoadNativeLibrary(string name)
        {
            if (SystemNativeLibrary.TryLoad(name, out nint lib))
            {
                return lib;
            }

            return 0;
        }

        public void FreeNativeLibrary(nint handle)
        {
            SystemNativeLibrary.Free(handle);
        }

        public nint LoadFunctionPointer(nint handle, string name)
        {
            if (SystemNativeLibrary.TryGetExport(handle, name, out nint ptr))
            {
                return ptr;
            }

            return 0;
        }
    }
#else
    private class Win32LibraryLoader : ILibraryLoader
    {
        public nint LoadNativeLibrary(string name)
        {
            return Kernel32.LoadLibrary(name);
        }

        public void FreeNativeLibrary(nint handle)
        {
            Kernel32.FreeLibrary(handle);
        }

        public nint LoadFunctionPointer(nint handle, string name)
        {
            return Kernel32.GetProcAddress(handle, name);
        }
    }

    private class BsdLibraryLoader : ILibraryLoader
    {
        public nint LoadNativeLibrary(string name)
        {
            return Libc.dlopen(name, Libc.RTLD_NOW);
        }

        public void FreeNativeLibrary(nint handle)
        {
            Libc.dlclose(handle);
        }

        public nint LoadFunctionPointer(nint handle, string name)
        {
            return Libc.dlsym(handle, name);
        }
    }

    private class UnixLibraryLoader : ILibraryLoader
    {
        public nint LoadNativeLibrary(string name)
        {
            return Libdl.dlopen(name, Libdl.RTLD_NOW);
        }

        public void FreeNativeLibrary(nint handle)
        {
            Libdl.dlclose(handle);
        }

        public nint LoadFunctionPointer(nint handle, string name)
        {
            return Libdl.dlsym(handle, name);
        }
    }

    internal static class Kernel32
    {
        [DllImport("kernel32")]
        public static extern nint LoadLibrary(string fileName);

        [DllImport("kernel32")]
        public static extern nint GetProcAddress(nint module, string procName);

        [DllImport("kernel32")]
        public static extern int FreeLibrary(nint module);
    }

    internal static class Libc
    {
        private const string LibName = "libc";

        public const int RTLD_NOW = 0x002;

        [DllImport(LibName)]
        public static extern nint dlopen(string fileName, int flags);

        [DllImport(LibName)]
        public static extern nint dlsym(nint handle, string name);

        [DllImport(LibName)]
        public static extern int dlclose(nint handle);

        [DllImport(LibName)]
        public static extern string dlerror();
    }

    internal static class Libdl
    {
        private const string LibName = "libdl";

        public const int RTLD_NOW = 0x002;

        [DllImport(LibName)]
        public static extern nint dlopen(string fileName, int flags);

        [DllImport(LibName)]
        public static extern nint dlsym(nint handle, string name);

        [DllImport(LibName)]
        public static extern int dlclose(nint handle);

        [DllImport(LibName)]
        public static extern string dlerror();
    }
#endif
}

