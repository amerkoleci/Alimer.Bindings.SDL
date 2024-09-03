// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPUVertexBinding
{
    public SDL_GPUVertexBinding(
        uint binding,
        uint stride,
        SDL_GPUVertexInputRate inputRate = SDL_GPUVertexInputRate.Vertex,
        uint instanceStepRate = 0)
    {
        this.binding = binding;
        this.stride = stride;
        this.inputRate = inputRate;
        this.instanceStepRate = instanceStepRate;
    }
}
