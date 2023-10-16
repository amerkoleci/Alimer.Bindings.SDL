// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

public readonly partial struct SDL_PropertiesID : IComparable, IComparable<SDL_PropertiesID>, IEquatable<SDL_PropertiesID>, IFormattable
{
    public readonly uint Value;

    public SDL_PropertiesID(uint value)
    {
        Value = value;
    }

    public static bool operator ==(SDL_PropertiesID left, SDL_PropertiesID right) => left.Value == right.Value;

    public static bool operator !=(SDL_PropertiesID left, SDL_PropertiesID right) => left.Value != right.Value;

    public static bool operator <(SDL_PropertiesID left, SDL_PropertiesID right) => left.Value < right.Value;

    public static bool operator <=(SDL_PropertiesID left, SDL_PropertiesID right) => left.Value <= right.Value;

    public static bool operator >(SDL_PropertiesID left, SDL_PropertiesID right) => left.Value > right.Value;

    public static bool operator >=(SDL_PropertiesID left, SDL_PropertiesID right) => left.Value >= right.Value;

    public static implicit operator uint(SDL_PropertiesID value) => value.Value;

    public static implicit operator SDL_PropertiesID(uint value) => new (value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_PropertiesID other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_PropertiesID)}.");
    }

    public int CompareTo(SDL_PropertiesID other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_PropertiesID other) && Equals(other);

    public bool Equals(SDL_PropertiesID other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
