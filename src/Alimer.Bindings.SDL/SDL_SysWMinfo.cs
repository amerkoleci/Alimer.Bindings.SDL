// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

#pragma warning disable CS0649
namespace SDL;

public partial struct SDL_SysWMinfo_win
{
    public nint window;
    public nint hdc;
    public nint hinstance;
}

public partial struct SDL_SysWMinfo_winrt
{
    /// <summary>
    /// Refers to an IInspectable*
    /// </summary>
    public nint window;
}

[StructLayout(LayoutKind.Sequential)]
public struct SDL_SysWMinfo_x11
{
    public nint display; // Refers to a Display*
    public int screen;
    public nint window; // Refers to a Window (XID, use ToInt64!)
}


[StructLayout(LayoutKind.Sequential)]
public struct SDL_SysWMinfo_cocoa
{
    public IntPtr window; // Refers to an NSWindow*
}

[StructLayout(LayoutKind.Sequential)]
public struct SDL_SysWMinfo_uikit
{
    public nint window; // Refers to a UIWindow*
    public uint framebuffer;
    public uint colorbuffer;
    public uint resolveFramebuffer;
}


[StructLayout(LayoutKind.Sequential)]
public struct SDL_SysWMinfo_wayland
{
    public nint display; // Refers to a wl_display*
    public nint surface; // Refers to a wl_surface*
    public nint egl_window; // Refers to an egl_window*, requires >= 2.0.16
    public nint xdg_surface; // Refers to an xdg_surface*, requires >= 2.0.16
    public nint xdg_toplevel; // Referes to an xdg_toplevel*, requires >= 2.0.18
    public nint xdg_popup;
    public nint xdg_positioner;
}

[StructLayout(LayoutKind.Sequential)]
public struct SDL_SysWMinfo_android
{
    public nint window; // Refers to an ANativeWindow
    public nint surface; // Refers to an EGLSurface
}

[StructLayout(LayoutKind.Sequential)]
public struct SDL_SysWMinfo_vivante
{
    public IntPtr display; // Refers to an EGLNativeDisplayType
    public IntPtr window; // Refers to an EGLNativeWindowType
}

/* Only available in 2.0.16 or higher. */
[StructLayout(LayoutKind.Sequential)]
public struct SDL_SysWMinfo_kmsdrm
{
    int dev_index;
    int drm_fd;
    IntPtr gbm_dev; // Refers to a gbm_device*
}

[StructLayout(LayoutKind.Explicit)]
public partial struct SDL_SysWMinfo_info
{
    [FieldOffset(0)]
    public SDL_SysWMinfo_win win;

    [FieldOffset(0)]
    public SDL_SysWMinfo_winrt winrt;

    [FieldOffset(0)]
    public SDL_SysWMinfo_x11 x11;

    [FieldOffset(0)]
    public SDL_SysWMinfo_cocoa cocoa;

    [FieldOffset(0)]
    public SDL_SysWMinfo_uikit uikit;

    [FieldOffset(0)]
    public SDL_SysWMinfo_wayland wl;

    [FieldOffset(0)]
    public SDL_SysWMinfo_android android;

    [FieldOffset(0)]
    public SDL_SysWMinfo_vivante vivante;

    [FieldOffset(0)]
    public SDL_SysWMinfo_kmsdrm ksmdrm;

    [FieldOffset(0)]
    private dummy_ptrs__FixedBuffer dummy_ptrs;

    [FieldOffset(0)]
    private unsafe fixed ulong dummy_ints[14];

    private unsafe struct dummy_ptrs__FixedBuffer
    {
        public nint e0;
        public nint e1;
        public nint e2;
        public nint e3;
        public nint e4;
        public nint e5;
        public nint e6;
        public nint e7;
        public nint e8;
        public nint e9;
        public nint e10;
        public nint e11;
        public nint e12;
        public nint e13;
    }
}

public partial struct SDL_SysWMinfo
{
    public uint version;
    public SDL_SYSWM_TYPE subsystem;
    private unsafe fixed uint padding[2];
    public SDL_SysWMinfo_info info;
} 
