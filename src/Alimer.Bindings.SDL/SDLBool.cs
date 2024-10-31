// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Data.Common;
using System.Runtime.CompilerServices;

namespace SDL3;

public readonly struct SDLBool : IEquatable<SDLBool>
{
    internal const byte FALSE_VALUE = 0;
    internal const byte TRUE_VALUE = 1;

    private readonly byte _value;

    public static SDLBool True => new(TRUE_VALUE);
    public static SDLBool False => new(FALSE_VALUE);

    [Obsolete("Never explicitly construct an SDL bool.")]
    public SDLBool()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SDLBool" /> struct.
    /// </summary>
    /// <param name="value">The value to set.</param>
    internal SDLBool(byte value)
    {
        _value = value;
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns>true if <paramref name="other" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(SDLBool other) => _value == other._value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDLBool rawBool && Equals(rawBool);

    /// <inheritdoc/>
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    /// Implements the ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(SDLBool left, SDLBool right) => left.Equals(right);

    /// <summary>
    /// Implements the !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(SDLBool left, SDLBool right) => !left.Equals(right);


    /// <summary>
    /// Performs an explicit conversion from <see cref="SDLBool"/> to <see cref="bool"/>.
    /// </summary>
    /// <param name="value">The <see cref="SDLBool"/> value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator bool(SDLBool value) => value._value != FALSE_VALUE;

    /// <summary>
    /// Performs an explicit conversion from <see cref="bool"/> to <see cref="VkBool32"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator SDLBool(bool value) => new(value ? TRUE_VALUE : FALSE_VALUE);

    /// <inheritdoc/>
    public override string ToString() => _value != 0 ? "True" : "False";
}
