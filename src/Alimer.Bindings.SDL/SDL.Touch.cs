// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace SDL;

unsafe partial class SDL
{
    public const uint SDL_TOUCH_MOUSEID = uint.MaxValue;
    public const long SDL_MOUSE_TOUCHID = -1;

    public static string SDL_GetTouchNameString(int index)
    {
        return GetStringOrEmpty(SDL_GetTouchName(index));
    }
}
