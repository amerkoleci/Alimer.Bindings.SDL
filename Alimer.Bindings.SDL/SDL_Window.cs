// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

[Flags]
public enum SDL_WindowFlags : uint
{
    /// <summary>
    /// Window is in fullscreen mode
    /// </summary>
    Fullscreen = 0x00000001,
    /// <summary>
    /// Window usable with OpenGL context
    /// </summary>
    OpenGL = 0x00000002,
    /// <summary>
    /// Window is occluded
    /// </summary>
    Occluded = 0x00000004,
    /// <summary>
    /// Window is not visible
    /// </summary>
    Hidden = 0x00000008,
    /// <summary>
    /// No window decoration
    /// </summary>
    Borderless = 0x00000010,
    /// <summary>
    /// Window can be resized
    /// </summary>
    Resizable = 0x00000020,
    /// <summary>
    /// Window is minimized
    /// </summary>
    Minimized = 0x00000040,
    /// <summary>
    /// Window is maximized
    /// </summary>
    Maximized = 0x00000080,
    /// <summary>
    /// Window has grabbed mouse input
    /// </summary>
    MouseGrabbed = 0x00000100,
    /// <summary>
    /// Window has input focus
    /// </summary>
    InputFocus = 0x00000200,
    /// <summary>
    /// Window has mouse focus
    /// </summary>
    MouseFocus = 0x00000400,
    /// <summary>
    /// Wndow not created by SDL
    /// </summary>
    Foreign = 0x00000800,
    /// <summary>
    /// Window uses high pixel density back buffer if possible
    /// </summary>
    HighPixelDensity = 0x00002000,
    /// <summary>
    /// Window has mouse captured (unrelated to <see cref="SDL_WindowFlags.MouseGrabbed"/>) 
    /// </summary>
    MouseCapture = 0x00004000,
    /// <summary>
    /// Window should always be above others
    /// </summary>
    AlwaysOnTop = 0x00008000,
    /// <summary>
    /// Window should be treated as a utility window, not showing in the task bar and window list 
    /// </summary>
    Utility = 0x00020000,
    /// <summary>
    /// window should be treated as a tooltip and must be created using <see cref="SDL.SDL_CreatePopupWindow(SDL_Window, int, int, int, int, SDL_WindowFlags)"/>
    /// </summary>
    Tooltip = 0x00040000,
    /// <summary>
    /// Window should be treated as a popup menu and must be created using <see cref="SDL.SDL_CreatePopupWindow(SDL_Window, int, int, int, int, SDL_WindowFlags)"/>
    /// </summary>
    PopupMenu = 0x00080000,
    /// <summary>
    /// Window has grabbed keyboard input
    /// </summary>
    KeyboardGrabbed = 0x00100000,
    /// <summary>
    /// Window usable for Vulkan surface
    /// </summary>
    Vulkan = 0x10000000,
    /// <summary>
    /// Window usable for Metal view
    /// </summary>
    Metal = 0x20000000,
    /// <summary>
    /// Window with transparent buffer
    /// </summary>
    Transparent = 0x40000000
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly partial struct SDL_Window : IEquatable<SDL_Window>
{
    public readonly nint Handle;

    public SDL_Window(nint handle)
    {
        Handle = handle;
    }

    public bool IsNull => Handle == 0;
    public bool IsNotNull => Handle != 0;
    public static SDL_Window Null => new(0);

    public static implicit operator nint(SDL_Window handle) => handle.Handle;
    public static implicit operator SDL_Window(nint handle) => new(handle);

    public static bool operator ==(SDL_Window left, SDL_Window right) => left.Handle == right.Handle;
    public static bool operator !=(SDL_Window left, SDL_Window right) => left.Handle != right.Handle;
    public static bool operator ==(SDL_Window left, nint right) => left.Handle == right;
    public static bool operator !=(SDL_Window left, nint right) => left.Handle != right;

    public bool Equals(SDL_Window other) => Handle == other.Handle;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is SDL_Window handle && Equals(handle);

    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();

    private string DebuggerDisplay => $"{nameof(SDL_Window)} [0x{Handle:X}]";
}

