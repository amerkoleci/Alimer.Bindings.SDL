// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPUDepthStencilValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SDL_GPUDepthStencilValue"/> structure.
    /// </summary>
    /// <param name="depth">The depth clear value.</param>
    /// <param name="stencil">The stencil clear value.</param>
    public SDL_GPUDepthStencilValue(float depth, byte stencil)
    {
        this.depth = depth;
        this.stencil = stencil;
    }
}
