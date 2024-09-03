// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

partial struct SDL_GPUMultisampleState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SDL_GPUMultisampleState"/> structure.
    /// </summary>
    public SDL_GPUMultisampleState(SDL_GPUSampleCount sampleCount, uint sampleMask)
    {
        this.sampleCount = sampleCount;
        this.sampleMask = sampleMask;
    }
}
