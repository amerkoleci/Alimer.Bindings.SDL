// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace SDL;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_AudioStream(nint handle) : IEquatable<SDL_AudioStream>
{
    public readonly nint Handle = handle;

    public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static SDL_AudioStream Null => new(0);

    public static implicit operator nint(SDL_AudioStream handle) => handle.Handle;
    public static implicit operator SDL_AudioStream(nint handle) => new(handle);

    public static bool operator ==(SDL_AudioStream left, SDL_AudioStream right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_AudioStream left, SDL_AudioStream right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_AudioStream left, nint right) => left.Handle == right;
    public static bool operator !=(SDL_AudioStream left, nint right) => left.Handle != right;

    public bool Equals(SDL_AudioStream other) => Handle == other.Handle;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_AudioStream handle && Equals(handle);

    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();

    private string DebuggerDisplay => $"{nameof(SDL_AudioStream)} [0x{Handle:X}]";
}
