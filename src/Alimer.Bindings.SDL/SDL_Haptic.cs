// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace SDL;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Haptic(nint handle) : IEquatable<SDL_Haptic>
{
    public readonly nint Handle = handle;

    public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static SDL_Haptic Null => new(0);

    public static implicit operator nint(SDL_Haptic handle) => handle.Handle;
    public static implicit operator SDL_Haptic(nint handle) => new(handle);

    public static bool operator ==(SDL_Haptic left, SDL_Haptic right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Haptic left, SDL_Haptic right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Haptic left, nint right) => left.Handle == right;
    public static bool operator !=(SDL_Haptic left, nint right) => left.Handle != right;

    public bool Equals(SDL_Haptic other) => Handle == other.Handle;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Haptic handle && Equals(handle);

    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();

    private string DebuggerDisplay => $"{nameof(SDL_Haptic)} [0x{Handle:X}]";
}
