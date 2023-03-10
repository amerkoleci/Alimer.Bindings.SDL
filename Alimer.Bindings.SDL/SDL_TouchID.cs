// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Alimer.Bindings.SDL;

public readonly partial struct SDL_TouchID : IComparable, IComparable<SDL_TouchID>, IEquatable<SDL_TouchID>, IFormattable
{
    public readonly long Value;

    public SDL_TouchID(long value)
    {
        Value = value;
    }

    public static bool operator ==(SDL_TouchID left, SDL_TouchID right) => left.Value == right.Value;

    public static bool operator !=(SDL_TouchID left, SDL_TouchID right) => left.Value != right.Value;

    public static bool operator <(SDL_TouchID left, SDL_TouchID right) => left.Value < right.Value;

    public static bool operator <=(SDL_TouchID left, SDL_TouchID right) => left.Value <= right.Value;

    public static bool operator >(SDL_TouchID left, SDL_TouchID right) => left.Value > right.Value;

    public static bool operator >=(SDL_TouchID left, SDL_TouchID right) => left.Value >= right.Value;

    public static implicit operator long(SDL_TouchID value) => value.Value;

    public static implicit operator SDL_TouchID(long value) => new(value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_TouchID other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_TouchID)}.");
    }

    public int CompareTo(SDL_TouchID other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_TouchID other) && Equals(other);

    public bool Equals(SDL_TouchID other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
