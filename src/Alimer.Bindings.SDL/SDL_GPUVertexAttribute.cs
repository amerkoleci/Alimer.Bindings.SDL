// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPUVertexAttribute
{
    public SDL_GPUVertexAttribute(
        uint location,
        uint bufferSlot,
        SDL_GPUVertexElementFormat format,
        uint offset
        )
    {
        this.location = location;
        this.buffer_slot = bufferSlot;
        this.format = format;
        this.offset = offset;
    }
}
