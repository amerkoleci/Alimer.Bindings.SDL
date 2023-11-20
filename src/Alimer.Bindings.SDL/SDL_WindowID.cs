// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

public readonly partial struct SDL_WindowID(uint value) : IComparable, IComparable<SDL_WindowID>, IEquatable<SDL_WindowID>, IFormattable
{
    public readonly uint Value = value;

    public static bool operator ==(SDL_WindowID left, SDL_WindowID right) => left.Value == right.Value;

    public static bool operator !=(SDL_WindowID left, SDL_WindowID right) => left.Value != right.Value;

    public static bool operator <(SDL_WindowID left, SDL_WindowID right) => left.Value < right.Value;

    public static bool operator <=(SDL_WindowID left, SDL_WindowID right) => left.Value <= right.Value;

    public static bool operator >(SDL_WindowID left, SDL_WindowID right) => left.Value > right.Value;

    public static bool operator >=(SDL_WindowID left, SDL_WindowID right) => left.Value >= right.Value;

    public static implicit operator uint(SDL_WindowID value) => value.Value;

    public static implicit operator SDL_WindowID(uint value) => new (value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_WindowID other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_WindowID)}.");
    }

    public int CompareTo(SDL_WindowID other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is SDL_WindowID other) && Equals(other);

    public bool Equals(SDL_WindowID other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}
