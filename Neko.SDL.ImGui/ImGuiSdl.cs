// This code is based on the original imgui_impl_sdl3.cpp, originally licenced under MIT license
//
// The MIT License (MIT)
// 
// Copyright (c) 2025 VanderCat
// Copyright (c) 2014-2025 Omar Cornut
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.


using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImGuiNET;
using Neko.Sdl;
using Neko.Sdl.Events;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;
using Neko.Sdl.Input;
using Neko.Sdl.Video;
using Rectangle = Neko.Sdl.Rectangle;
using Timer = Neko.Sdl.Time.Timer;

namespace Neko.Sdl.ImGuiBackend;

public static unsafe class ImGuiSdl {
    

    // SDL Data
    public enum GamepadMode { AutoFirst, AutoAll, Manual };


    // Backend data stored in io.BackendPlatformUserData to allow support for multiple Dear ImGui contexts
    // It is STRONGLY preferred that you use docking branch with multi-viewports (== single Dear ImGui context + multiple windows) instead of multiple Dear ImGui contexts.
    // FIXME: multi-context support is not well tested and probably dysfunctional in this backend.
    // FIXME: some shared resources (mouse cursor shape, gamepad) are mishandled when using multi-context.
    private static IPlatformBackend _backend;
    private static IViewportBackend? _viewportBackend;
    private static Pin<IPlatformBackend> _pin;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static char* GetClipboardText(void* context) {
        //TODO: leak
        return (char*) Marshal.StringToCoTaskMemUTF8(_backend.GetClipboardText());
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetClipboardText(void* context, byte* text) {
        _backend.SetClipboardText(Marshal.PtrToStringUTF8((IntPtr)text));
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void PlatformSetImeData(void* context, ImGuiViewport* viewport, ImGuiPlatformImeData* data) {
        _backend.SetImeData(viewport, data);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static bool PlatformOpenInShell(void* context, char* url) {
        return _backend.OpenInShell(Marshal.PtrToStringUTF8((IntPtr)url)??"");
    }
    
    // You can read the io.WantCaptureMouse, io.WantCaptureKeyboard flags to tell if dear imgui wants to use your inputs.
    // - When io.WantCaptureMouse is true, do not dispatch mouse input data to your main application, or clear/overwrite your copy of the mouse data.
    // - When io.WantCaptureKeyboard is true, do not dispatch keyboard input data to your main application, or clear/overwrite your copy of the keyboard data.
    // Generally you may always pass all inputs to dear imgui, and hide them from your application based on those two flags.
    // If you have multiple SDL events and some of them are not meant to be used by dear imgui, you may need to filter events based on their windowID field.
    public static bool ProcessEvent(ref Event e) {
        if (_backend is null) 
            throw new Exception("Context or backend not initialized! Did you call Init()?");
        return true;
        //throw new NotImplementedException();
    }

    public static void Init(IPlatformBackend backend) {
        _backend = backend;
        _pin = _backend.Pin(GCHandleType.Normal);
        var io = ImGui.GetIO();
        io.BackendPlatformUserData = _pin.Pointer;
        var platformIo = (ImGuiPlatformIO*)ImGui.GetPlatformIO();
        delegate*unmanaged[Cdecl]<void*, byte*, void> setClipboardText = &SetClipboardText;
        delegate*unmanaged[Cdecl]<void*, char*> getClipboardText = &GetClipboardText;
        delegate*unmanaged[Cdecl]<void*, ImGuiViewport*, ImGuiPlatformImeData*, void> setImeData = &PlatformSetImeData;
        delegate*unmanaged[Cdecl]<void*, char*, bool> openInShell = &PlatformOpenInShell;
        platformIo->Platform_SetClipboardTextFn = (IntPtr)setClipboardText;
        platformIo->Platform_GetClipboardTextFn = (IntPtr)getClipboardText;
        platformIo->Platform_SetImeDataFn = (IntPtr)setImeData;
        platformIo->Platform_OpenInShellFn = (IntPtr)openInShell;
        // We need SDL_CaptureMouse(), SDL_GetGlobalMouseState() from SDL 2.0.4+ to support multiple viewports.
        //if (io.BackendFlags.HasFlag(ImGuiBackendFlags.PlatformHasViewports))
        //    InitMultiViewportSupport(, sdlGlСontext);
    }

    public static bool HasCaptureAndGlobalMouse => // no amiga in .NET
        !OperatingSystem.IsBrowser() && !OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS();

    public static bool IsApple => OperatingSystem.IsMacOS() | OperatingSystem.IsMacCatalyst() |
                                  OperatingSystem.IsIOS() | OperatingSystem.IsTvOS();

    public static void Shutdown() {
        if (_backend == null)
            throw new InvalidOperationException("No platform backend to shutdown, or already shutdown?");
        _backend.Dispose();
        _pin.Dispose();
    }
    
    public static void NewFrame() {
        if (_backend == null)
            throw new InvalidOperationException(
                "Context or backend not initialized! Did you call Init()?");
        _backend.NewFrame();
    }

    // Helper structure we store in the void* RendererUserData field of each ImGuiViewport to easily retrieve our backend data.

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void CreateWindow(ImGuiViewport* viewport) {
        _viewportBackend!.CreateWindow(viewport);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void DestroyWindow(ImGuiViewport* viewport) {
        _viewportBackend!.DestroyWindow(viewport);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void ShowWindow(ImGuiViewport* viewport) {
        _viewportBackend!.ShowWindow(viewport);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void UpdateWindow(ImGuiViewport* viewport) {
        _viewportBackend!.UpdateWindow(viewport);
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static Vector2 GetWindowPos(ImGuiViewport* viewport) {
        return _viewportBackend!.GetWindowPos(viewport);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetWindowPos(ImGuiViewport* viewport, Vector2 pos) {
        
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static Vector2 GetWindowSize(ImGuiViewport* viewport) {
        return _viewportBackend!.GetWindowSize(viewport);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetWindowSize(ImGuiViewport* viewport, Vector2 size) {
        _viewportBackend!.SetWindowSize(viewport, size);
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static Vector2 GetWindowFramebufferScale(ImGuiViewport* viewport) {
        throw new NotImplementedException();
        // var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        // GetWindowSizeAndFramebufferScale(vd.Window, out _, out var framebuffer_scale);
        // return framebuffer_scale;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetWindowTitle(ImGuiViewport* viewport, byte* title) {
        _viewportBackend!.SetWindowTitle(viewport, Marshal.PtrToStringUTF8((IntPtr)title)??"ERROR");
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetWindowAlpha(ImGuiViewport* viewport, float alpha) {
        _viewportBackend!.SetWindowAlpha(viewport, alpha);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetWindowFocus(ImGuiViewport* viewport, bool focus) {
        _viewportBackend!.SetWindowFocus(viewport, focus);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static bool GetWindowFocus(ImGuiViewport* viewport) {
        return _viewportBackend!.GetWindowFocus(viewport);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static bool GetWindowMinimized(ImGuiViewport* viewport) {
        return _viewportBackend!.GetWindowMinimized(viewport);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void RenderWindow(ImGuiViewport* viewport, void* unused) {
        _viewportBackend!.RenderWindow(viewport, (IntPtr)unused);
    }

    internal static ViewportData GetViewportData(this ImGuiViewportPtr viewport) {
        var vdPin = new Pin<ViewportData>(viewport.PlatformUserData);
        if (!vdPin.TryGetTarget(out var vd)) 
            throw new Exception("Failed to get viewport platform data");
        return vd;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SwapBuffers(ImGuiViewport* viewport, void* unused) {
        _viewportBackend!.SwapBuffers(viewport, (IntPtr)unused);
    }

    // Vulkan support (the Vulkan renderer needs to call a platform-side support function to create the surface)
    // SDL is graceful enough to _not_ need <vulkan/vulkan.h> so we can safely include this.
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int CreateVkSurface(ImGuiViewport* viewport, IntPtr vk_instance, IntPtr vk_allocator, IntPtr* out_vk_surface) {
        return _viewportBackend!.CreateVkSurface(viewport, vk_instance, vk_allocator, ref Unsafe.AsRef<IntPtr>(out_vk_surface));
    }

    public static void InitMultiViewportSupport(Window window, IntPtr sdlGlContext) {
        if (_backend.GetType().IsAssignableTo(typeof(IViewportBackend))) {
            _viewportBackend = (IViewportBackend)_backend;
        }
        else {
            throw new Exception();
        }
        
        // Register platform interface (will be coupled with a renderer interface)
        var platformIo = ImGui.GetPlatformIO();
        delegate*unmanaged[Cdecl]<ImGuiViewport*, void> createWindow = &CreateWindow;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, void> destroyWindow = &DestroyWindow;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, void> showWindow = &ShowWindow;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, void> updateWindow = &UpdateWindow;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, Vector2, void> setWindowPos = &SetWindowPos;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, Vector2> getWindowPos = &GetWindowPos;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, Vector2, void> setWindowSize = &SetWindowSize;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, Vector2> getWindowSize = &GetWindowSize;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, Vector2> getWindowFramebufferScale = &GetWindowFramebufferScale;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, bool, void> setWindowFocus = &SetWindowFocus;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, bool> getWindowFocus = &GetWindowFocus;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, bool> getWindowMinimized = &GetWindowMinimized;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, byte*, void> setWindowTitle = &SetWindowTitle;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, void*, void> renderWindow = &RenderWindow;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, void*, void> swapBuffers = &SwapBuffers;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, float, void> setWindowAlpha = &SetWindowAlpha;
        delegate*unmanaged[Cdecl]<ImGuiViewport*, nint, nint, nint*, int> createVkSurface = &CreateVkSurface;
        platformIo.Platform_CreateWindow = (IntPtr)createWindow;
        platformIo.Platform_DestroyWindow = (IntPtr)destroyWindow;
        platformIo.Platform_ShowWindow = (IntPtr)showWindow;
        platformIo.Platform_UpdateWindow = (IntPtr)updateWindow;
        platformIo.Platform_SetWindowPos = (IntPtr)setWindowPos;
        platformIo.Platform_GetWindowPos = (IntPtr)getWindowPos;
        platformIo.Platform_SetWindowSize = (IntPtr)setWindowSize;
        platformIo.Platform_GetWindowSize = (IntPtr)getWindowSize;
        //platformIo.Platform_GetWindowFramebufferScale = (IntPtr)getWindowFramebufferScale;
        platformIo.Platform_SetWindowFocus = (IntPtr)setWindowFocus;
        platformIo.Platform_GetWindowFocus = (IntPtr)getWindowFocus;
        platformIo.Platform_GetWindowMinimized = (IntPtr)getWindowMinimized;
        platformIo.Platform_SetWindowTitle = (IntPtr)setWindowTitle;
        platformIo.Platform_RenderWindow = (IntPtr)renderWindow;
        platformIo.Platform_SwapBuffers = (IntPtr)swapBuffers;
        platformIo.Platform_SetWindowAlpha = (IntPtr)setWindowAlpha;
        platformIo.Platform_CreateVkSurface = (IntPtr)createVkSurface;

        // Register main window handle (which is owned by the main application, not by us)
        // This is mostly for simplicity and consistency, so that our code (e.g. mouse handling etc.) can use same logic for main and secondary viewports.
        var mainViewport = ImGui.GetMainViewport();
        var vd = new ViewportData {
            Window = window,
            WindowID = window.Id,
            WindowOwned = false,
            GLContext = sdlGlContext,
        };
        var vdPin = vd.Pin(GCHandleType.Normal);
        mainViewport.PlatformUserData = vdPin.Pointer;
        mainViewport.PlatformHandle = (IntPtr)vd.WindowID;
    }

    static void ShutdownMultiViewportSupport() =>
        ImGui.DestroyPlatformWindows();

}