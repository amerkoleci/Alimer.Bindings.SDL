// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace Alimer.Bindings.SDL;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Cursor : IEquatable<SDL_Cursor>
{
    public SDL_Cursor(IntPtr handle) { Handle = handle; }
    public IntPtr Handle { get; }
    public bool IsNull => Handle == IntPtr.Zero;
    public bool IsNotNull => Handle != IntPtr.Zero;
    public static SDL_Cursor Null => new(IntPtr.Zero);
    public static implicit operator SDL_Cursor(IntPtr handle) => new(handle);
    public static bool operator ==(SDL_Cursor left, SDL_Cursor right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Cursor left, SDL_Cursor right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Cursor left, IntPtr right) => left.Handle == right;
    public static bool operator !=(SDL_Cursor left, IntPtr right) => left.Handle != right;
    public bool Equals(SDL_Cursor other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Cursor handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(SDL_Cursor)} [0x{Handle:X}]";
}

