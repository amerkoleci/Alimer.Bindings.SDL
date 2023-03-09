// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace Alimer.Bindings.SDL;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Gamepad : IEquatable<SDL_Gamepad>
{
    public readonly nint Handle;

    public SDL_Gamepad(nint handle)
    {
        Handle = handle;
    }

    public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static SDL_Gamepad Null => new(0);

    public static implicit operator nint(SDL_Gamepad handle) => handle.Handle;
    public static implicit operator SDL_Gamepad(nint handle) => new(handle);

    public static bool operator ==(SDL_Gamepad left, SDL_Gamepad right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Gamepad left, SDL_Gamepad right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Gamepad left, nint right) => left.Handle == right;
    public static bool operator !=(SDL_Gamepad left, nint right) => left.Handle != right;

    public bool Equals(SDL_Gamepad other) => Handle == other.Handle;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Gamepad handle && Equals(handle);

    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();

    private string DebuggerDisplay => $"{nameof(SDL_Gamepad)} [0x{Handle:X}]";
}

