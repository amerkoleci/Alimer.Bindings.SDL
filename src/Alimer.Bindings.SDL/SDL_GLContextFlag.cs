// Copyright (c) Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace SDL3;

/// <summary>
/// Possible flags to be set for the SDL_GL_CONTEXT_FLAGS attribute.<br/>
/// <br/>
/// @since This datatype is available since SDL 3.1.3.
/// </summary>
[Flags]
public enum SDL_GLContextFlag : uint
{
    None = 0,
    Debug = SDL3.SDL_GL_CONTEXT_DEBUG_FLAG,
    ForwardCompatible = SDL3.SDL_GL_CONTEXT_FORWARD_COMPATIBLE_FLAG,
    RobustAccess = SDL3.SDL_GL_CONTEXT_ROBUST_ACCESS_FLAG,
    ResetIsolation = SDL3.SDL_GL_CONTEXT_RESET_ISOLATION_FLAG,
}
