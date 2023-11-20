// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

public readonly partial struct SDL_FingerID(long value) : IComparable, IComparable<SDL_FingerID>, IEquatable<SDL_FingerID>, IFormattable
{
    public readonly long Value = value;

    public static bool operator ==(SDL_FingerID left, SDL_FingerID right) => left.Value == right.Value;

    public static bool operator !=(SDL_FingerID left, SDL_FingerID right) => left.Value != right.Value;

    public static bool operator <(SDL_FingerID left, SDL_FingerID right) => left.Value < right.Value;

    public static bool operator <=(SDL_FingerID left, SDL_FingerID right) => left.Value <= right.Value;

    public static bool operator >(SDL_FingerID left, SDL_FingerID right) => left.Value > right.Value;

    public static bool operator >=(SDL_FingerID left, SDL_FingerID right) => left.Value >= right.Value;

    public static implicit operator long(SDL_FingerID value) => value.Value;

    public static implicit operator SDL_FingerID(long value) => new(value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_FingerID other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_FingerID)}.");
    }

    public int CompareTo(SDL_FingerID other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_FingerID other) && Equals(other);

    public bool Equals(SDL_FingerID other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
