// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

public readonly partial struct SDL_MouseID : IComparable, IComparable<SDL_MouseID>, IEquatable<SDL_MouseID>, IFormattable
{
    public readonly uint Value;

    public SDL_MouseID(uint value)
    {
        Value = value;
    }

    public static bool operator ==(SDL_MouseID left, SDL_MouseID right) => left.Value == right.Value;

    public static bool operator !=(SDL_MouseID left, SDL_MouseID right) => left.Value != right.Value;

    public static bool operator <(SDL_MouseID left, SDL_MouseID right) => left.Value < right.Value;

    public static bool operator <=(SDL_MouseID left, SDL_MouseID right) => left.Value <= right.Value;

    public static bool operator >(SDL_MouseID left, SDL_MouseID right) => left.Value > right.Value;

    public static bool operator >=(SDL_MouseID left, SDL_MouseID right) => left.Value >= right.Value;

    public static implicit operator uint(SDL_MouseID value) => value.Value;

    public static implicit operator SDL_MouseID(uint value) => new (value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_MouseID other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_MouseID)}.");
    }

    public int CompareTo(SDL_MouseID other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_MouseID other) && Equals(other);

    public bool Equals(SDL_MouseID other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
