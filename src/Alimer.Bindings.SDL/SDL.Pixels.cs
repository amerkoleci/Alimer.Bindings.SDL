// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using static SDL3.SDL3;
using static SDL3.SDL_bool;

namespace SDL3;

unsafe partial class SDL3
{
    public static SDL_PixelFormat SDL_DEFINE_PIXELFOURCC(byte A, byte B, byte C, byte D) => (SDL_PixelFormat)SDL_FOURCC(A, B, C, D);

    public static SDL_PixelFormat SDL_DEFINE_PIXELFORMAT(int type, int order, int layout, int bits, int bytes)
        => (SDL_PixelFormat)((1 << 28) | ((type) << 24) | ((order) << 20) | ((layout) << 16) |
                             ((bits) << 8) | ((bytes) << 0));

    public static byte SDL_PIXELFLAG(uint X)
    {
        return (byte)((X >> 28) & 0x0F);
    }

    public static SDL_PixelType SDL_PIXELTYPE(SDL_PixelFormat X)
    {
        return (SDL_PixelType)(((uint)X >> 24) & 0x0F);
    }

    public static SDL_PackedOrder SDL_PIXELORDER(SDL_PixelFormat X)
    {
        return (SDL_PackedOrder)(((uint)X >> 20) & 0x0F);
    }

    public static SDL_PackedLayout SDL_PIXELLAYOUT(SDL_PixelFormat X)
    {
        return (SDL_PackedLayout)(((uint)X >> 16) & 0x0F);
    }

    public static byte SDL_BITSPERPIXEL(uint X)
    {
        return (byte)((X >> 8) & 0xFF);
    }

    public static byte SDL_BYTESPERPIXEL(SDL_PixelFormat X)
    {
        if (SDL_ISPIXELFORMAT_FOURCC(X))
        {
            if ((X == SDL_PixelFormat.Yuy2) || (X == SDL_PixelFormat.Uyvy) || (X == SDL_PixelFormat.Yvyu))
            {
                return 2;
            }
            return 1;
        }
        return (byte)((uint)X & 0xFF);
    }

    public static bool SDL_ISPIXELFORMAT_INDEXED(SDL_PixelFormat format)
    {
        if (SDL_ISPIXELFORMAT_FOURCC(format))
        {
            return false;
        }
        SDL_PixelType pType = SDL_PIXELTYPE(format);
        return (
            pType == SDL_PixelType.Index1 ||
            pType == SDL_PixelType.Index4 ||
            pType == SDL_PixelType.Index8
        );
    }

    public static bool SDL_ISPIXELFORMAT_PACKED(SDL_PixelFormat format)
    {
        if (SDL_ISPIXELFORMAT_FOURCC(format))
        {
            return false;
        }
        SDL_PixelType pType = SDL_PIXELTYPE(format);
        return (
            pType == SDL_PixelType.Packed8 ||
            pType == SDL_PixelType.Packed16 ||
            pType == SDL_PixelType.Packed32
        );
    }

    public static bool SDL_ISPIXELFORMAT_ARRAY(SDL_PixelFormat format)
    {
        if (SDL_ISPIXELFORMAT_FOURCC(format))
        {
            return false;
        }

        SDL_PixelType pType = SDL_PIXELTYPE(format);
        return (
            pType == SDL_PixelType.ArrayU8 ||
            pType == SDL_PixelType.ArrayU16 ||
            pType == SDL_PixelType.ArrayU32 ||
            pType == SDL_PixelType.ArrayF16 ||
            pType == SDL_PixelType.ArrayF32
        );
    }

    public static bool SDL_ISPIXELFORMAT_ALPHA(SDL_PixelFormat format)
    {
        if (!SDL_ISPIXELFORMAT_PACKED(format))
            return false;

        SDL_PackedOrder pOrder = SDL_PIXELORDER(format);
        return (
            pOrder == SDL_PackedOrder.Argb ||
            pOrder == SDL_PackedOrder.Rgba ||
            pOrder == SDL_PackedOrder.Abgr ||
            pOrder == SDL_PackedOrder.Bgra
        );
    }

    public static bool SDL_ISPIXELFORMAT_10BIT(SDL_PixelFormat format)
    {
        if (SDL_PIXELTYPE(format) == SDL_PixelType.Packed32 && SDL_PIXELLAYOUT(format) == SDL_PackedLayout._2101010)
            return true;

        return false;
    }

    public static bool SDL_ISPIXELFORMAT_FOURCC(SDL_PixelFormat format)
    {
        return (format == SDL_PixelFormat.Unknown) && (SDL_PIXELFLAG((uint)format) != 1);
    }

    public static uint SDL_DEFINE_COLORSPACE(
        SDL_ColorType type,
        SDL_ColorRange range,
        SDL_ColorPrimaries primaries,
        SDL_TransferCharacteristics transfer,
        SDL_MatrixCoefficients matrix,
        SDL_ChromaLocation chroma
        )
    {
        return (((uint)(type) << 28) | ((uint)(range) << 24) | ((uint)(chroma) << 20) | ((uint)(primaries) << 10) | ((uint)(transfer) << 5) | ((uint)(matrix) << 0));
    }

    public static SDL_ColorType SDL_COLORSPACETYPE(uint space)
    {
        return (SDL_ColorType)(((space) >> 28) & 0x0F);
    }

    public static SDL_ColorRange SDL_COLORSPACERANGE(uint space)
    {
        return (SDL_ColorRange)(((space) >> 24) & 0x0F);
    }

    public static SDL_ChromaLocation SDL_COLORSPACECHROMA(uint space)
    {
        return (SDL_ChromaLocation)(((space) >> 20) & 0x0F);
    }

    public static SDL_ColorPrimaries SDL_COLORSPACEPRIMARIES(uint space)
    {
        return (SDL_ColorPrimaries)(((space) >> 10) & 0x1F);
    }

    public static SDL_TransferCharacteristics SDL_COLORSPACETRANSFER(uint space)
    {
        return (SDL_TransferCharacteristics)(((space) >> 5) & 0x1F);
    }

    public static SDL_MatrixCoefficients SDL_COLORSPACEMATRIX(uint space)
    {
        return (SDL_MatrixCoefficients)((space) & 0x1F);
    }

    public static bool SDL_ISCOLORSPACE_YUV_BT601(uint space)
    {
        return (SDL_COLORSPACEMATRIX(space) == SDL_MatrixCoefficients.Bt601 || SDL_COLORSPACEMATRIX(space) == SDL_MatrixCoefficients.Bt470bg);
    }

    public static bool SDL_ISCOLORSPACE_YUV_BT709(uint space)
    {
        return SDL_COLORSPACEMATRIX(space) == SDL_MatrixCoefficients.Bt709;
    }

    public static bool SDL_ISCOLORSPACE_LIMITED_RANGE(uint space)
    {
        return SDL_COLORSPACERANGE(space) == SDL_ColorRange.Limited;
    }

    public static bool SDL_ISCOLORSPACE_FULL_RANGE(uint space)
    {
        return SDL_COLORSPACERANGE(space) == SDL_ColorRange.Full;
    }

    public static SDL_Surface* SDL_CreateSurfaceFrom<T>(T[] source, int width, int height, int pitch, SDL_PixelFormat format)
        where T : unmanaged
    {
        ReadOnlySpan<T> span = source.AsSpan();

        return SDL_CreateSurfaceFrom(span, width, height, format, pitch);
    }

    public static SDL_Surface* SDL_CreateSurfaceFrom<T>(ReadOnlySpan<T> source, int width, int height, SDL_PixelFormat format, int pitch)
        where T : unmanaged
    {
        return SDL_CreateSurfaceFrom(ref MemoryMarshal.GetReference(source), width, height, format, pitch);
    }

    public static SDL_Surface* SDL_CreateSurfaceFrom<T>(ref T source, int width, int height, SDL_PixelFormat format, int pitch)
         where T : unmanaged
    {
        fixed (void* sourcePointer = &source)
        {
            return SDL_CreateSurfaceFrom(width, height, format, (nint)sourcePointer, pitch);
        }
    }
}
