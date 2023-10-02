// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

public readonly partial struct SDL_TimerID : IComparable, IComparable<SDL_TimerID>, IEquatable<SDL_TimerID>, IFormattable
{
    public readonly int Value;

    public SDL_TimerID(int value)
    {
        Value = value;
    }

    public static bool operator ==(SDL_TimerID left, SDL_TimerID right) => left.Value == right.Value;

    public static bool operator !=(SDL_TimerID left, SDL_TimerID right) => left.Value != right.Value;

    public static bool operator <(SDL_TimerID left, SDL_TimerID right) => left.Value < right.Value;

    public static bool operator <=(SDL_TimerID left, SDL_TimerID right) => left.Value <= right.Value;

    public static bool operator >(SDL_TimerID left, SDL_TimerID right) => left.Value > right.Value;

    public static bool operator >=(SDL_TimerID left, SDL_TimerID right) => left.Value >= right.Value;

    public static implicit operator int(SDL_TimerID value) => value.Value;

    public static implicit operator SDL_TimerID(int value) => new (value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_TimerID other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_TimerID)}.");
    }

    public int CompareTo(SDL_TimerID other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_TimerID other) && Equals(other);

    public bool Equals(SDL_TimerID other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
