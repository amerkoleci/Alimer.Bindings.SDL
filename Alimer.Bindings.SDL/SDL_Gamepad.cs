// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Renderer : IEquatable<SDL_Renderer>
{
    public readonly nint Handle;

    public SDL_Renderer(nint handle)
    {
        Handle = handle;
    }

    public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static SDL_Renderer Null => new(0);

    public static implicit operator nint(SDL_Renderer handle) => handle.Handle;
    public static implicit operator SDL_Renderer(nint handle) => new(handle);

    public static bool operator ==(SDL_Renderer left, SDL_Renderer right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Renderer left, SDL_Renderer right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Renderer left, nint right) => left.Handle == right;
    public static bool operator !=(SDL_Renderer left, nint right) => left.Handle != right;

    public bool Equals(SDL_Renderer other) => Handle == other.Handle;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Renderer handle && Equals(handle);

    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();

    private string DebuggerDisplay => $"{nameof(SDL_Renderer)} [0x{Handle:X}]";
}

