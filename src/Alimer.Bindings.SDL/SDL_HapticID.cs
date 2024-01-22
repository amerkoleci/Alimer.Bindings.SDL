// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

public readonly partial struct SDL_HapticID(uint value) : IComparable, IComparable<SDL_HapticID>, IEquatable<SDL_HapticID>, IFormattable
{
    public readonly uint Value = value;

    public static bool operator ==(SDL_HapticID left, SDL_HapticID right) => left.Value == right.Value;

    public static bool operator !=(SDL_HapticID left, SDL_HapticID right) => left.Value != right.Value;

    public static bool operator <(SDL_HapticID left, SDL_HapticID right) => left.Value < right.Value;

    public static bool operator <=(SDL_HapticID left, SDL_HapticID right) => left.Value <= right.Value;

    public static bool operator >(SDL_HapticID left, SDL_HapticID right) => left.Value > right.Value;

    public static bool operator >=(SDL_HapticID left, SDL_HapticID right) => left.Value >= right.Value;

    public static implicit operator uint(SDL_HapticID value) => value.Value;

    public static implicit operator SDL_HapticID(uint value) => new (value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_HapticID other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_HapticID)}.");
    }

    public int CompareTo(SDL_HapticID other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_HapticID other) && Equals(other);

    public bool Equals(SDL_HapticID other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
