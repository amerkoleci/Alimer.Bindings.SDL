// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPUStencilOpState
{
    /// <summary>
    /// A built-in description with default values.
    /// </summary>
    public static SDL_GPUStencilOpState Default => new(SDL_GPUStencilOp.Keep, SDL_GPUStencilOp.Keep, SDL_GPUStencilOp.Keep, SDL_GPUCompareOp.Always);

    public SDL_GPUStencilOpState(
        SDL_GPUStencilOp failOp = SDL_GPUStencilOp.Keep,
        SDL_GPUStencilOp passOp = SDL_GPUStencilOp.Keep,
        SDL_GPUStencilOp depthFailOp = SDL_GPUStencilOp.Keep,
        SDL_GPUCompareOp compareOp = SDL_GPUCompareOp.Always)
    {
        this.failOp = failOp;
        this.passOp = passOp;
        this.depthFailOp = depthFailOp;
        this.compareOp = compareOp;
    }
}
