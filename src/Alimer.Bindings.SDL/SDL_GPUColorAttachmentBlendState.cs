// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPUColorAttachmentBlendState
{
    public static SDL_GPUColorAttachmentBlendState Opaque => new(false, SDL_GPUBlendFactor.One, SDL_GPUBlendFactor.Zero, SDL_GPUBlendOp.Add, SDL_GPUBlendFactor.One, SDL_GPUBlendFactor.Zero, SDL_GPUBlendOp.Add);
    public static SDL_GPUColorAttachmentBlendState AlphaBlend => new(true, SDL_GPUBlendFactor.One, SDL_GPUBlendFactor.OneMinusSrcAlpha, SDL_GPUBlendOp.Add, SDL_GPUBlendFactor.One, SDL_GPUBlendFactor.OneMinusSrcAlpha, SDL_GPUBlendOp.Add);
    public static SDL_GPUColorAttachmentBlendState Additive => new(true, SDL_GPUBlendFactor.SrcAlpha, SDL_GPUBlendFactor.One, SDL_GPUBlendOp.Add, SDL_GPUBlendFactor.SrcAlpha, SDL_GPUBlendFactor.One, SDL_GPUBlendOp.Add);
    public static SDL_GPUColorAttachmentBlendState NonPremultiplied => new(true, SDL_GPUBlendFactor.SrcAlpha, SDL_GPUBlendFactor.OneMinusSrcAlpha, SDL_GPUBlendOp.Add, SDL_GPUBlendFactor.SrcAlpha, SDL_GPUBlendFactor.OneMinusSrcAlpha, SDL_GPUBlendOp.Add);

    public SDL_GPUColorAttachmentBlendState(
        bool blendEnable = false,
        SDL_GPUBlendFactor srcColorBlendFactor = SDL_GPUBlendFactor.One,
        SDL_GPUBlendFactor dstColorBlendFactor = SDL_GPUBlendFactor.Zero,
        SDL_GPUBlendOp colorBlendOp = SDL_GPUBlendOp.Add,
        SDL_GPUBlendFactor srcAlphaBlendFactor = SDL_GPUBlendFactor.One,
        SDL_GPUBlendFactor dstAlphaBlendFactor = SDL_GPUBlendFactor.Zero,
        SDL_GPUBlendOp alphaBlendOp = SDL_GPUBlendOp.Add,
        SDL_GPUColorComponentFlags colorWriteMask = SDL_GPUColorComponentFlags.All)
    {
        this.blendEnable = blendEnable ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE;
        this.srcColorBlendFactor = srcColorBlendFactor;
        this.dstColorBlendFactor = dstColorBlendFactor;
        this.colorBlendOp = colorBlendOp;
        this.srcAlphaBlendFactor = srcAlphaBlendFactor;
        this.dstAlphaBlendFactor = dstAlphaBlendFactor;
        this.alphaBlendOp = alphaBlendOp;
        this.colorWriteMask = colorWriteMask;
    }

}
