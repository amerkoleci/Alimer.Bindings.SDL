// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace Alimer.Bindings.SDL;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Joystick : IEquatable<SDL_Joystick>
{
    public SDL_Joystick(IntPtr handle) { Handle = handle; }
    public IntPtr Handle { get; }
    public bool IsNull => Handle == IntPtr.Zero;
    public bool IsNotNull => Handle != IntPtr.Zero;
    public static SDL_Joystick Null => new(IntPtr.Zero);
    public static implicit operator SDL_Joystick(IntPtr handle) => new(handle);
    public static bool operator ==(SDL_Joystick left, SDL_Joystick right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Joystick left, SDL_Joystick right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Joystick left, IntPtr right) => left.Handle == right;
    public static bool operator !=(SDL_Joystick left, IntPtr right) => left.Handle != right;
    public bool Equals(SDL_Joystick other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Joystick handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(SDL_Joystick)} [0x{Handle:X}]";
}

