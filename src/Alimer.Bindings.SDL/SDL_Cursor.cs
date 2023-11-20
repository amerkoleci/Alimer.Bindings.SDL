// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace SDL;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Cursor(nint handle) : IEquatable<SDL_Cursor>
{
    public nint Handle { get; } = handle; public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static SDL_Cursor Null => new(0);
    public static implicit operator SDL_Cursor(nint handle) => new(handle);
    public static bool operator ==(SDL_Cursor left, SDL_Cursor right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Cursor left, SDL_Cursor right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Cursor left, nint right) => left.Handle == right;
    public static bool operator !=(SDL_Cursor left, nint right) => left.Handle != right;
    public bool Equals(SDL_Cursor other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Cursor handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(SDL_Cursor)} [0x{Handle:X}]";
}

