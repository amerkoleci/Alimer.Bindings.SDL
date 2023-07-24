// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL;

public readonly unsafe partial struct SDL_GLContext : IComparable, IComparable<SDL_GLContext>, IEquatable<SDL_GLContext>, IFormattable
{
    public readonly void* Value;

    public SDL_GLContext(void* value)
    {
        Value = value;
    }

    public static SDL_GLContext NULL => new(null);

    public static bool operator ==(SDL_GLContext left, SDL_GLContext right) => left.Value == right.Value;

    public static bool operator !=(SDL_GLContext left, SDL_GLContext right) => left.Value != right.Value;

    public static bool operator <(SDL_GLContext left, SDL_GLContext right) => left.Value < right.Value;

    public static bool operator <=(SDL_GLContext left, SDL_GLContext right) => left.Value <= right.Value;

    public static bool operator >(SDL_GLContext left, SDL_GLContext right) => left.Value > right.Value;

    public static bool operator >=(SDL_GLContext left, SDL_GLContext right) => left.Value >= right.Value;

    public static explicit operator SDL_GLContext(void* value) => new(value);

    public static implicit operator void*(SDL_GLContext value) => value.Value;

    public static explicit operator SDL_GLContext(nint value) => new(unchecked((void*)(value)));

    public static implicit operator nint(SDL_GLContext value) => (nint)(value.Value);


    public static explicit operator SDL_GLContext(nuint value) => new(unchecked((void*)(value)));

    public static implicit operator nuint(SDL_GLContext value) => (nuint)(value.Value);

    public int CompareTo(object? obj)
    {
        if (obj is SDL_GLContext other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException($"obj is not an instance of {nameof(SDL_GLContext)}.");
    }

    public int CompareTo(SDL_GLContext other) => ((nuint)(Value)).CompareTo((nuint)(other.Value));

    public override bool Equals(object? obj) => (obj is SDL_GLContext other) && Equals(other);

    public bool Equals(SDL_GLContext other) => ((nuint)(Value)).Equals((nuint)(other.Value));

    public override int GetHashCode() => ((nuint)(Value)).GetHashCode();

    public override string ToString() => ((nuint)(Value)).ToString((sizeof(nint) == 4) ? "X8" : "X16");

    public string ToString(string? format, IFormatProvider? formatProvider) => ((nuint)(Value)).ToString(format, formatProvider);
}
