// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace Alimer.Bindings.SDL;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Window : IEquatable<SDL_Window>
{
    public readonly nint Handle;

    public SDL_Window(nint handle) { Handle = handle; }

    public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static SDL_Window Null => new(0);

    public static implicit operator nint(SDL_Window handle) => handle.Handle;
    public static implicit operator SDL_Window(nint handle) => new(handle);

    public static bool operator ==(SDL_Window left, SDL_Window right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Window left, SDL_Window right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Window left, nint right) => left.Handle == right;
    public static bool operator !=(SDL_Window left, nint right) => left.Handle != right;

    public bool Equals(SDL_Window other) => Handle == other.Handle;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Window handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => $"{nameof(SDL_Window)} [0x{Handle:X}]";
}

