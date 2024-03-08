// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace SDL;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Camera(nint handle) : IEquatable<SDL_Camera>
{
    public readonly nint Handle = handle;

    public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static SDL_Camera Null => new(0);

    public static implicit operator nint(SDL_Camera handle) => handle.Handle;
    public static implicit operator SDL_Camera(nint handle) => new(handle);

    public static bool operator ==(SDL_Camera left, SDL_Camera right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Camera left, SDL_Camera right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Camera left, nint right) => left.Handle == right;
    public static bool operator !=(SDL_Camera left, nint right) => left.Handle != right;

    public bool Equals(SDL_Camera other) => Handle == other.Handle;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Camera handle && Equals(handle);

    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();

    private string DebuggerDisplay => $"{nameof(SDL_Camera)} [0x{Handle:X}]";
}
