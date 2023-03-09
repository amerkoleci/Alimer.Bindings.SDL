// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Alimer.Bindings.SDL;

public readonly partial struct SDL_DisplayID : IComparable, IComparable<SDL_DisplayID>, IEquatable<SDL_DisplayID>, IFormattable
{
    public readonly uint Value;

    public SDL_DisplayID(uint value)
    {
        Value = value;
    }

    public static bool operator ==(SDL_DisplayID left, SDL_DisplayID right) => left.Value == right.Value;

    public static bool operator !=(SDL_DisplayID left, SDL_DisplayID right) => left.Value != right.Value;

    public static bool operator <(SDL_DisplayID left, SDL_DisplayID right) => left.Value < right.Value;

    public static bool operator <=(SDL_DisplayID left, SDL_DisplayID right) => left.Value <= right.Value;

    public static bool operator >(SDL_DisplayID left, SDL_DisplayID right) => left.Value > right.Value;

    public static bool operator >=(SDL_DisplayID left, SDL_DisplayID right) => left.Value >= right.Value;

    public static implicit operator uint(SDL_DisplayID value) => value.Value;

    public static implicit operator SDL_DisplayID(uint value) => new (value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_DisplayID other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_DisplayID)}.");
    }

    public int CompareTo(SDL_DisplayID other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_DisplayID other) && Equals(other);

    public bool Equals(SDL_DisplayID other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
