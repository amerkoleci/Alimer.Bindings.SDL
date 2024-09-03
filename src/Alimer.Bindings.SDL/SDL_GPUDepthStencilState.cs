// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPUDepthStencilState
{
    /// <summary>
    /// A built-in description with settings for not using a depth stencil buffer.
    /// </summary>
    public static SDL_GPUDepthStencilState None => new(false, false, SDL_GPUCompareOp.LessOrEqual);

    /// <summary>
    /// A built-in description with default settings for using a depth stencil buffer.
    /// </summary>
    public static SDL_GPUDepthStencilState Default => new(true, true, SDL_GPUCompareOp.LessOrEqual);

    /// <summary>
    /// A built-in description with settings for enabling a read-only depth stencil buffer.
    /// </summary>
    public static SDL_GPUDepthStencilState Read => new(true, false, SDL_GPUCompareOp.LessOrEqual);

    /// <summary>
    /// A built-in description with default settings for using a reverse depth stencil buffer.
    /// </summary>
    public static SDL_GPUDepthStencilState ReverseZ => new(true, true, SDL_GPUCompareOp.GreaterOrEqual);

    /// <summary>
    /// A built-in description with default settings for using a reverse read-only depth stencil buffer.
    /// </summary>
    public static SDL_GPUDepthStencilState ReadReverseZ => new(true, false, SDL_GPUCompareOp.GreaterOrEqual);

    public SDL_GPUDepthStencilState(
        bool depthTestEnable,
        bool depthWriteEnable,
        SDL_GPUCompareOp depthCompareOp)
    {
        this.depthTestEnable = depthTestEnable ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE;
        this.depthWriteEnable = depthWriteEnable ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE;
        this.compareOp = depthCompareOp;
        this.stencilTestEnable = SDL_bool.SDL_FALSE;
        this.backStencilState = SDL_GPUStencilOpState.Default;
        this.frontStencilState = SDL_GPUStencilOpState.Default;
        this.compareMask = byte.MaxValue;
        this.writeMask = byte.MaxValue;
        this.reference = 0;
    }

    public SDL_GPUDepthStencilState(
        bool depthTestEnable,
        bool depthWriteEnable,
        SDL_GPUCompareOp depthCompareOp,
        bool stencilTestEnable,
        in SDL_GPUStencilOpState backStencilState,
        in SDL_GPUStencilOpState frontStencilState,
        byte compareMask = byte.MaxValue,
        byte writeMask = byte.MaxValue,
        byte reference = 0)
    {
        this.depthTestEnable = depthTestEnable ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE; ;
        this.depthWriteEnable = depthWriteEnable ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE;
        this.compareOp = depthCompareOp;
        this.stencilTestEnable = stencilTestEnable ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE;
        this.backStencilState = backStencilState;
        this.frontStencilState = frontStencilState;
        this.compareMask = compareMask;
        this.writeMask = writeMask;
        this.reference = reference;
    }
}
