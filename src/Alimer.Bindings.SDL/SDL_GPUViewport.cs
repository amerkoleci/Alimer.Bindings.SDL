// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;

namespace SDL3;

partial struct SDL_GPUViewport : IEquatable<SDL_GPUViewport>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SDL_GPUViewport"/> struct.
    /// </summary>
    /// <param name="width">The width of the viewport in pixels.</param>
    /// <param name="height">The height of the viewport in pixels.</param>
    public SDL_GPUViewport(float width, float height)
    {
        x = 0.0f;
        y = 0.0f;
        this.w = width;
        this.h = height;
        minDepth = 0.0f;
        maxDepth = 1.0f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SDL_GPUViewport"/> struct.
    /// </summary>
    /// <param name="x">The x coordinate of the upper-left corner of the viewport in pixels.</param>
    /// <param name="y">The y coordinate of the upper-left corner of the viewport in pixels.</param>
    /// <param name="width">The width of the viewport in pixels.</param>
    /// <param name="height">The height of the viewport in pixels.</param>
    public SDL_GPUViewport(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.w = width;
        this.h = height;
        minDepth = 0.0f;
        maxDepth = 1.0f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SDL_GPUViewport"/> struct.
    /// </summary>
    /// <param name="x">The x coordinate of the upper-left corner of the viewport in pixels.</param>
    /// <param name="y">The y coordinate of the upper-left corner of the viewport in pixels.</param>
    /// <param name="width">The width of the viewport in pixels.</param>
    /// <param name="height">The height of the viewport in pixels.</param>
    /// <param name="minDepth">The minimum depth of the clip volume.</param>
    /// <param name="maxDepth">The maximum depth of the clip volume.</param>
    public SDL_GPUViewport(float x, float y, float width, float height, float minDepth, float maxDepth)
    {
        this.x = x;
        this.y = y;
        this.w = width;
        this.h = height;
        this.minDepth = minDepth;
        this.maxDepth = maxDepth;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SDL_GPUViewport"/> struct.
    /// </summary>
    /// <param name="extent">The width and height extent of the viewport in pixels.</param>
    public SDL_GPUViewport(in SizeF extent)
    {
        x = 0.0f;
        y = 0.0f;
        w = extent.Width;
        h = extent.Height;
        minDepth = 0.0f;
        maxDepth = 1.0f;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SDL_GPUViewport"/> struct.
    /// </summary>
    /// <param name="bounds">A <see cref="RectangleF"/> that defines the location and size of the viewport in a render target.</param>
    public SDL_GPUViewport(in RectangleF bounds)
    {
        x = bounds.X;
        y = bounds.Y;
        w = bounds.Width;
        h = bounds.Height;
        minDepth = 0.0f;
        maxDepth = 1.0f;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SDL_GPUViewport"/> struct.
    /// </summary>
    /// <param name="bounds">A <see cref="Vector4"/> that defines the location and size of the viewport in a render target.</param>
    public SDL_GPUViewport(in Vector4 bounds)
    {
        x = bounds.X;
        y = bounds.Y;
        w = bounds.Z;
        h = bounds.W;

        minDepth = 0.0f;
        maxDepth = 1.0f;
    }

    /// <inheritdoc/>
    public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is SDL_GPUViewport other && Equals(other);

    /// <inheritdoc/>
    public readonly bool Equals(SDL_GPUViewport other)
    {
        return
            x == other.x &&
            y == other.y &&
            w == other.w &&
            h == other.h &&
            minDepth == other.minDepth &&
            maxDepth == other.maxDepth;
    }

    /// <inheritdoc/>
    public override readonly int GetHashCode() => HashCode.Combine(x, y, w, h, minDepth, maxDepth);

    /// <inheritdoc/>
    public override readonly string ToString() => $"{{X={x},Y={y},Width={w},Height={h},MinDepth={minDepth},MaxDepth={maxDepth}}}";

    /// <summary>
    /// Compares two <see cref="SDL_GPUViewport"/> objects for equality.
    /// </summary>
    /// <param name="left">The <see cref="SDL_GPUViewport"/> on the left hand of the operand.</param>
    /// <param name="right">The <see cref="SDL_GPUViewport"/> on the right hand of the operand.</param>
    /// <returns>
    /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    public static bool operator ==(SDL_GPUViewport left, SDL_GPUViewport right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="SDL_GPUViewport"/> objects for inequality.
    /// </summary>
    /// <param name="left">The <see cref="SDL_GPUViewport"/> on the left hand of the operand.</param>
    /// <param name="right">The <see cref="SDL_GPUViewport"/> on the right hand of the operand.</param>
    /// <returns>
    /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
    /// </returns>
    public static bool operator !=(SDL_GPUViewport left, SDL_GPUViewport right) => !left.Equals(right);
}
