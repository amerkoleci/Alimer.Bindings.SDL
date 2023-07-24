// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Surface : IEquatable<SDL_Surface>
{
    public SDL_Surface(IntPtr handle) { Handle = handle; }
    public IntPtr Handle { get; }
    public bool IsNull => Handle == IntPtr.Zero;
    public bool IsNotNull => Handle != IntPtr.Zero;
    public static SDL_Surface Null => new(IntPtr.Zero);
    public static implicit operator SDL_Surface(IntPtr handle) => new(handle);
    public static bool operator ==(SDL_Surface left, SDL_Surface right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Surface left, SDL_Surface right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Surface left, IntPtr right) => left.Handle == right;
    public static bool operator !=(SDL_Surface left, IntPtr right) => left.Handle != right;
    public bool Equals(SDL_Surface other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Surface handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(SDL_Surface)} [0x{Handle:X}]";
}

