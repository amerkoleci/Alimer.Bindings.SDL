// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

public readonly partial struct SDL_AudioFormat(ushort value) : IComparable, IComparable<SDL_AudioFormat>, IEquatable<SDL_AudioFormat>, IFormattable
{
    public readonly ushort Value = value;

    public static bool operator ==(SDL_AudioFormat left, SDL_AudioFormat right) => left.Value == right.Value;

    public static bool operator !=(SDL_AudioFormat left, SDL_AudioFormat right) => left.Value != right.Value;

    public static bool operator <(SDL_AudioFormat left, SDL_AudioFormat right) => left.Value < right.Value;

    public static bool operator <=(SDL_AudioFormat left, SDL_AudioFormat right) => left.Value <= right.Value;

    public static bool operator >(SDL_AudioFormat left, SDL_AudioFormat right) => left.Value > right.Value;

    public static bool operator >=(SDL_AudioFormat left, SDL_AudioFormat right) => left.Value >= right.Value;

    public static implicit operator ushort(SDL_AudioFormat value) => value.Value;

    public static implicit operator SDL_AudioFormat(ushort value) => new (value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_AudioFormat other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_AudioFormat)}.");
    }

    public int CompareTo(SDL_AudioFormat other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_AudioFormat other) && Equals(other);

    public bool Equals(SDL_AudioFormat other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
