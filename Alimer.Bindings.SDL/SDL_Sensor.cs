// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Sensor : IEquatable<SDL_Sensor>
{
    public readonly nint Handle;

    public SDL_Sensor(nint handle)
    {
        Handle = handle;
    }

    public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static SDL_Sensor Null => new(0);

    public static implicit operator nint(SDL_Sensor handle) => handle.Handle;
    public static implicit operator SDL_Sensor(nint handle) => new(handle);

    public static bool operator ==(SDL_Sensor left, SDL_Sensor right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Sensor left, SDL_Sensor right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Sensor left, nint right) => left.Handle == right;
    public static bool operator !=(SDL_Sensor left, nint right) => left.Handle != right;

    public bool Equals(SDL_Sensor other) => Handle == other.Handle;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Sensor handle && Equals(handle);

    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();

    private string DebuggerDisplay => $"{nameof(SDL_Sensor)} [0x{Handle:X}]";
}

