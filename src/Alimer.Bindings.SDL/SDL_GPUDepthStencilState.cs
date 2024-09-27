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
        this.enable_depth_test = depthTestEnable;
        this.enable_depth_write = depthWriteEnable;
        this.compare_op = depthCompareOp;
        this.enable_stencil_test = false;
        this.back_stencil_state = SDL_GPUStencilOpState.Default;
        this.front_stencil_state = SDL_GPUStencilOpState.Default;
        this.compare_mask = byte.MaxValue;
        this.write_mask = byte.MaxValue;
    }

    public SDL_GPUDepthStencilState(
        bool depthTestEnable,
        bool depthWriteEnable,
        SDL_GPUCompareOp depthCompareOp,
        bool stencilTestEnable,
        in SDL_GPUStencilOpState backStencilState,
        in SDL_GPUStencilOpState frontStencilState,
        byte compareMask = byte.MaxValue,
        byte writeMask = byte.MaxValue)
    {
        this.enable_depth_test = depthTestEnable;
        this.enable_depth_write = depthWriteEnable;
        this.compare_op = depthCompareOp;
        this.enable_stencil_test = stencilTestEnable;
        this.back_stencil_state = backStencilState;
        this.front_stencil_state = frontStencilState;
        this.compare_mask = compareMask;
        this.write_mask = writeMask;
    }
}
