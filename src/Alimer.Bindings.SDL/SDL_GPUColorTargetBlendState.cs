// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPUColorTargetBlendState
{
    public static SDL_GPUColorTargetBlendState Opaque => new(false, SDL_GPUBlendFactor.One, SDL_GPUBlendFactor.Zero, SDL_GPUBlendOp.Add, SDL_GPUBlendFactor.One, SDL_GPUBlendFactor.Zero, SDL_GPUBlendOp.Add);
    public static SDL_GPUColorTargetBlendState AlphaBlend => new(true, SDL_GPUBlendFactor.One, SDL_GPUBlendFactor.OneMinusSrcAlpha, SDL_GPUBlendOp.Add, SDL_GPUBlendFactor.One, SDL_GPUBlendFactor.OneMinusSrcAlpha, SDL_GPUBlendOp.Add);
    public static SDL_GPUColorTargetBlendState Additive => new(true, SDL_GPUBlendFactor.SrcAlpha, SDL_GPUBlendFactor.One, SDL_GPUBlendOp.Add, SDL_GPUBlendFactor.SrcAlpha, SDL_GPUBlendFactor.One, SDL_GPUBlendOp.Add);
    public static SDL_GPUColorTargetBlendState NonPremultiplied => new(true, SDL_GPUBlendFactor.SrcAlpha, SDL_GPUBlendFactor.OneMinusSrcAlpha, SDL_GPUBlendOp.Add, SDL_GPUBlendFactor.SrcAlpha, SDL_GPUBlendFactor.OneMinusSrcAlpha, SDL_GPUBlendOp.Add);

    public SDL_GPUColorTargetBlendState(
        bool blendEnable = false,
        SDL_GPUBlendFactor srcColorBlendFactor = SDL_GPUBlendFactor.One,
        SDL_GPUBlendFactor dstColorBlendFactor = SDL_GPUBlendFactor.Zero,
        SDL_GPUBlendOp colorBlendOp = SDL_GPUBlendOp.Add,
        SDL_GPUBlendFactor srcAlphaBlendFactor = SDL_GPUBlendFactor.One,
        SDL_GPUBlendFactor dstAlphaBlendFactor = SDL_GPUBlendFactor.Zero,
        SDL_GPUBlendOp alphaBlendOp = SDL_GPUBlendOp.Add,
        SDL_GPUColorComponentFlags colorWriteMask = SDL_GPUColorComponentFlags.All)
    {
        this.enable_blend = blendEnable;
        this.src_color_blendfactor = srcColorBlendFactor;
        this.dst_color_blendfactor = dstColorBlendFactor;
        this.color_blend_op = colorBlendOp;
        this.src_alpha_blendfactor = srcAlphaBlendFactor;
        this.dst_alpha_blendfactor = dstAlphaBlendFactor;
        this.alpha_blend_op = alphaBlendOp;
        this.color_write_mask = colorWriteMask;
    }
}
