// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

public readonly partial struct SDL_PenID(uint value) : IComparable, IComparable<SDL_PenID>, IEquatable<SDL_PenID>, IFormattable
{
    public readonly uint Value = value;

    public static bool operator ==(SDL_PenID left, SDL_PenID right) => left.Value == right.Value;

    public static bool operator !=(SDL_PenID left, SDL_PenID right) => left.Value != right.Value;

    public static bool operator <(SDL_PenID left, SDL_PenID right) => left.Value < right.Value;

    public static bool operator <=(SDL_PenID left, SDL_PenID right) => left.Value <= right.Value;

    public static bool operator >(SDL_PenID left, SDL_PenID right) => left.Value > right.Value;

    public static bool operator >=(SDL_PenID left, SDL_PenID right) => left.Value >= right.Value;

    public static implicit operator uint(SDL_PenID value) => value.Value;

    public static implicit operator SDL_PenID(uint value) => new (value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_PenID other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_PenID)}.");
    }

    public int CompareTo(SDL_PenID other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_PenID other) && Equals(other);

    public bool Equals(SDL_PenID other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
