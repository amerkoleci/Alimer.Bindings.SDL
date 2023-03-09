// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using SystemNativeLibrary = System.Runtime.InteropServices.NativeLibrary;

namespace Alimer.Bindings.SDL;

internal sealed class TempNativeLibrary : IDisposable
{
    private static readonly ILibraryLoader _loader = GetPlatformDefaultLoader();

    public TempNativeLibrary(params string[] names)
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
            string nativeLibsPath = Path.Combine(AppContext.BaseDirectory, "runtimes", rid, "native");

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
        yield return RuntimeInformation.RuntimeIdentifier;

        var arch = RuntimeInformation.ProcessArchitecture;
        bool isArm = RuntimeInformation.ProcessArchitecture == Architecture.Arm || RuntimeInformation.ProcessArchitecture == Architecture.Arm64;

        string archName = (IntPtr.Size == 8)
            ? isArm ? "arm64" : "x64"
            : isArm ? "arm" : "x86";

        if (OperatingSystem.IsWindows())
        {
            yield return $"win10-{archName}";
            yield return $"win-{archName}";
        }
        else if (OperatingSystem.IsLinux())
        {
            yield return $"linux-{archName}";
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
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
    public nint LoadFunction(string name)
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
        return new SystemNativeLibraryLoader();
    }

    private interface ILibraryLoader
    {
        nint LoadNativeLibrary(string name);

        void FreeNativeLibrary(nint handle);

        nint LoadFunctionPointer(nint handle, string name);
    }

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
}

