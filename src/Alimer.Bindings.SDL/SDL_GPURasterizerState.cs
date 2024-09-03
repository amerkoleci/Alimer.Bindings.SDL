// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPURasterizerState
{
    /// <summary>
    /// A built-in description with settings with settings for not culling any primitives.
    /// </summary>
    public static SDL_GPURasterizerState CullNone => new(SDL_GPUCullMode.None);

    /// <summary>
    /// A built-in description with settings for culling primitives with clockwise winding order.
    /// </summary>
    public static SDL_GPURasterizerState CullClockwise => new(SDL_GPUCullMode.Front);

    /// <summary>
    /// A built-in description with settings for culling primitives with counter-clockwise winding order.
    /// </summary>
    public static SDL_GPURasterizerState CullCounterClockwise => new(SDL_GPUCullMode.Back);

    /// <summary>
    /// A built-in description with settings for not culling any primitives and wireframe fill mode.
    /// </summary>
    public static SDL_GPURasterizerState Wireframe => new(SDL_GPUCullMode.Back, SDL_GPUFillMode.Line);

    public SDL_GPURasterizerState(
        SDL_GPUCullMode cullMode,
        SDL_GPUFillMode fillMode = SDL_GPUFillMode.Fill,
        SDL_GPUFrontFace frontFace = SDL_GPUFrontFace.Clockwise,
        bool depthBiasEnable = false,
        float depthBiasConstantFactor = 0.0f,
        float depthBiasClamp = 0.0f,
        float depthBiasSlopeFactor = 0.0f)
    {
        this.fillMode = fillMode;
        this.cullMode = cullMode;
        this.frontFace = frontFace;
        this.depthBiasEnable = depthBiasEnable ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE;
        this.depthBiasConstantFactor = depthBiasConstantFactor;
        this.depthBiasClamp = depthBiasClamp;
        this.depthBiasSlopeFactor = depthBiasSlopeFactor;
    }
}
