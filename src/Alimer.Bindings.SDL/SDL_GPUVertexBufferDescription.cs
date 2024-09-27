// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPUVertexBufferDescription
{
    public SDL_GPUVertexBufferDescription(
        uint slot,
        uint pitch,
        SDL_GPUVertexInputRate inputRate = SDL_GPUVertexInputRate.Vertex,
        uint instanceStepRate = 0)
    {
        this.slot = slot;
        this.pitch = pitch;
        this.input_rate = inputRate;
        this.instance_step_rate = instanceStepRate;
    }
}
