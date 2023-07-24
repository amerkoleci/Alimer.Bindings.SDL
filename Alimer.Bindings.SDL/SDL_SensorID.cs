// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

public readonly partial struct SDL_SensorID : IComparable, IComparable<SDL_SensorID>, IEquatable<SDL_SensorID>, IFormattable
{
    public readonly uint Value;

    public SDL_SensorID(uint value)
    {
        Value = value;
    }

    public static bool operator ==(SDL_SensorID left, SDL_SensorID right) => left.Value == right.Value;

    public static bool operator !=(SDL_SensorID left, SDL_SensorID right) => left.Value != right.Value;

    public static bool operator <(SDL_SensorID left, SDL_SensorID right) => left.Value < right.Value;

    public static bool operator <=(SDL_SensorID left, SDL_SensorID right) => left.Value <= right.Value;

    public static bool operator >(SDL_SensorID left, SDL_SensorID right) => left.Value > right.Value;

    public static bool operator >=(SDL_SensorID left, SDL_SensorID right) => left.Value >= right.Value;

    public static implicit operator uint(SDL_SensorID value) => value.Value;

    public static implicit operator SDL_SensorID(uint value) => new (value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_SensorID other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_SensorID)}.");
    }

    public int CompareTo(SDL_SensorID other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_SensorID other) && Equals(other);

    public bool Equals(SDL_SensorID other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
