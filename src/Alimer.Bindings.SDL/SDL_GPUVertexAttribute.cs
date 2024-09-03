// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPUVertexAttribute
{
    public SDL_GPUVertexAttribute(
        uint location,
        uint binding,
        SDL_GPUVertexElementFormat format,
        uint offset
        )
    {
        this.location = location;
        this.binding = binding;
        this.format = format;
        this.offset = offset;
    }
}
