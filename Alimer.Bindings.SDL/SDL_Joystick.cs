// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Joystick : IEquatable<SDL_Joystick>
{
    public readonly nint Handle;
    public SDL_Joystick(nint handle)
    {
        Handle = handle;
    }

    public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static SDL_Joystick Null => new(0);

    public static implicit operator nint(SDL_Joystick handle) => handle.Handle;
    public static implicit operator SDL_Joystick(nint handle) => new(handle);

    public static bool operator ==(SDL_Joystick left, SDL_Joystick right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Joystick left, SDL_Joystick right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Joystick left, nint right) => left.Handle == right;
    public static bool operator !=(SDL_Joystick left, nint right) => left.Handle != right;
    public bool Equals(SDL_Joystick other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Joystick handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(SDL_Joystick)} [0x{Handle:X}]";
}

