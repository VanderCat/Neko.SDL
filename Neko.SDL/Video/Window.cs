using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Events;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl.Video;

public sealed unsafe partial class Window : SdlWrapper<SDL_Window> {
    public Renderer? Renderer;
    protected Pin<Window> _pin;

    public bool AlwaysOnTop {
        get => Flags.HasFlag(WindowFlags.AlwaysOnTop);
        set => SDL_SetWindowAlwaysOnTop(Handle, value);
    }
    
    public static Window Create(int width, int height, string title, WindowFlags windowFlags) {
        var handle = SDL_CreateWindow(title, width, height, (SDL_WindowFlags)windowFlags);
        if (handle is null) throw new SdlException("Failed to open window");
        var window = new Window(handle);
        __windowIdCache[window.Id] = window;
        
        window._pin = window.Pin(GCHandleType.Normal);
        return window;
    }
    public static Window Create(Properties properties) { //TODO: WindowCreateProperties
        var handle = SDL_CreateWindowWithProperties(properties);
        if (handle is null) throw new SdlException("Failed to open window");
        var window = new Window(handle);
        __windowIdCache[window.Id] = window;
        
        window._pin = window.Pin(GCHandleType.Normal);
        return window;
    }

    public static void CreateWindowAndRenderer(int width, int height, string title, WindowFlags windowFlags, out Window window, out Renderer renderer) {
        window = Create(width, height, title, windowFlags);
        renderer = window.CreateRenderer();
    }

    private static Dictionary<uint, Window> __windowIdCache = new();

    public static Window GetFromPtr(SDL_Window* window) {
        if (window is null) throw new SdlException("Window is NULL");
        var id = (uint)SDL_GetWindowID(window);
        if (id == 0) throw new SdlException("Could not found the id");
        if (__windowIdCache.TryGetValue(id, out var value)) {
            return value;
        }
        return __windowIdCache[id] = window;
    }
    
    public Properties Properties => new (SDL_GetWindowProperties(this)); //todo: dont create object

    public bool RelativeMouseMode {
        get => SDL_GetWindowRelativeMouseMode(this);
        set => SDL_SetWindowRelativeMouseMode(this, value);
    }
    
    public void WarpMouse(float x, float y) =>
        SDL_WarpMouseInWindow(this, x, y);
    
    public void WarpMouse(Vector2 position) =>
        WarpMouse(position.X, position.Y);

    public Renderer CreateRenderer(string? name = null) {
        return Renderer = Renderer.Create(this, name);
    }
    

    //public Renderer Renderer => SDL_GetRenderer(Handle);
    
    public uint Display => (uint)SDL_GetDisplayForWindow(Handle);
    public float DisplayScale => SDL_GetWindowDisplayScale(Handle);

    public void Flash(FlashOperation flashOperation) => SDL_FlashWindow(Handle, (SDL_FlashOperation)(int)flashOperation).ThrowIfError();

    public void GetAspectRatio(out float minAspect, out float maxAspect) {
        minAspect = 0f;
        maxAspect = 0f;
        SDL_GetWindowAspectRatio(Handle, (float*)Unsafe.AsPointer(ref minAspect),
            (float*)Unsafe.AsPointer(ref maxAspect)).ThrowIfError();
    }
    
    public void GetBordersSize(out int top, out int left, out int bottom, out int right) {
        top = left = bottom = right = 0;
        SDL_GetWindowBordersSize(Handle,
            (int*)Unsafe.AsPointer(ref top),
            (int*)Unsafe.AsPointer(ref left),
            (int*)Unsafe.AsPointer(ref bottom),
            (int*)Unsafe.AsPointer(ref right)).ThrowIfError();
    }

    public bool TryGetBordersSize(out int top, out int left, out int bottom, out int right) {
        top = left = bottom = right = 0;
        return SDL_GetWindowBordersSize(Handle,
            (int*)Unsafe.AsPointer(ref top),
            (int*)Unsafe.AsPointer(ref left),
            (int*)Unsafe.AsPointer(ref bottom),
            (int*)Unsafe.AsPointer(ref right));
    }

    public WindowFlags Flags => (WindowFlags)(ulong)SDL_GetWindowFlags(Handle);

    public uint Id => (uint)SDL_GetWindowID(Handle);

    public static Window GetById(uint id) {
        if (!__windowIdCache.TryGetValue(id, out var window)) {
            var windowPtr = SDL_GetWindowFromID((SDL_WindowID)id);
            if (windowPtr is null) throw new SdlException();
            window = __windowIdCache[id] = new Window(windowPtr);
        }

        return window;
    }
    public static Window GetById(SDL_WindowID id) => GetById((uint)id);

    public DisplayMode? FullscreenMode {
        get {
            var fm = SDL_GetWindowFullscreenMode(Handle);
            return fm is null ? null : fm;
        }
        set => SDL_SetWindowFullscreenMode(Handle, value is null ? null : value).ThrowIfError();
    }

    public bool Fullscreen {
        set => SDL_SetWindowFullscreen(Handle, value).ThrowIfError();
    }

    public IntPtr IccProfile => throw new NotImplementedException();

    public bool KeyboardGrab {
        get => SDL_GetWindowKeyboardGrab(Handle);
        set => SDL_SetWindowKeyboardGrab(Handle, value).ThrowIfError();
    }
    
    public bool MouseGrab {
        get => SDL_GetWindowMouseGrab(Handle);
        set => SDL_SetWindowMouseGrab(Handle, value).ThrowIfError();
    }

    public Size MaximumSize {
        get {
            var w = 0;
            var h = 0;
            SDL_GetWindowMaximumSize(Handle,
                (int*)Unsafe.AsPointer(ref w),
                (int*)Unsafe.AsPointer(ref h)).ThrowIfError();
            return new Size(w, h);
        }
        set => SDL_SetWindowMaximumSize(Handle,
                value.Width, value.Height);
    }
    
    public Size MinimumSize {
        get {
            var w = 0;
            var h = 0;
            SDL_GetWindowMinimumSize(Handle,
                (int*)Unsafe.AsPointer(ref w),
                (int*)Unsafe.AsPointer(ref h)).ThrowIfError();
            return new Size(w, h);
        }
        set => SDL_SetWindowMinimumSize(Handle,
            value.Width, value.Height);
    }

    public Rectangle MouseRect {
        get {
            var rect = SDL_GetWindowMouseRect(Handle);
            return new Rectangle(rect->x, rect->y, rect->w, rect->h);
        }
        set {
            var rect = new SDL_Rect {
                x = value.X,
                y = value.Y,
                w = value.Width,
                h = value.Height,
            };
            SDL_SetWindowMouseRect(Handle, (SDL_Rect*)Unsafe.AsPointer(ref rect));
        }
    }

    public float Opacity {
        get => SDL_GetWindowOpacity(Handle);
        set => SDL_SetWindowOpacity(Handle, value);
    }

    public Window Parent {
        get => GetFromPtr(SDL_GetWindowParent(Handle));
        set => SDL_SetWindowParent(Handle, value.Handle);
    }

    public float PixelDensity {
        get => SDL_GetWindowPixelDensity(Handle);
    }

    public PixelFormat PixelFormat {
        get => (PixelFormat)(uint)SDL_GetWindowPixelFormat(Handle);
    }

    public Point Position {
        get {
            var x = 0;
            var y = 0;
            SDL_GetWindowPosition(Handle,
                (int*)Unsafe.AsPointer(ref x),
                (int*)Unsafe.AsPointer(ref y)).ThrowIfError();
            return new Point(x, y);
        }
        set => SDL_SetWindowPosition(Handle,
            value.X, value.Y);
    }

    public static Window[] GetWindows() {
        var count = 0;
        var ptrptr = SDL_GetWindows((int*)Unsafe.AsPointer(ref count));
        if (ptrptr is null) throw new SdlException("");
        var span = new Span<IntPtr>(ptrptr, count);
        var arr = new Window[count];
        int counter = 0;
        foreach (SDL_Window* ptr in span) {
            arr[counter++] = __windowIdCache[(uint)SDL_GetWindowID(ptr)];
        }
        UnmanagedMemory.Free(ptrptr);
        return arr;
    }
    
    public Rectangle SafeArea {
        get {
            var rect = new SDL_Rect();
            SDL_GetWindowSafeArea(Handle, (SDL_Rect*)Unsafe.AsPointer(ref rect)).ThrowIfError();
            return new Rectangle(rect.x, rect.y, rect.w, rect.h);
        }
    }
    
    public Size Size {
        get {
            var w = 0;
            var h = 0;
            SDL_GetWindowSize(Handle,
                (int*)Unsafe.AsPointer(ref w),
                (int*)Unsafe.AsPointer(ref h)).ThrowIfError();
            return new Size(w, h);
        }
        set => SDL_SetWindowSize(Handle,
            value.Width, value.Height);
    }
    
    public Size SizeInPixels {
        get {
            var w = 0;
            var h = 0;
            SDL_GetWindowSizeInPixels(Handle,
                (int*)Unsafe.AsPointer(ref w),
                (int*)Unsafe.AsPointer(ref h)).ThrowIfError();
            return new Size(w, h);
        }
    }

    public SDL_Surface* Surface => throw new NotImplementedException();
    public bool HasSurface => SDL_WindowHasSurface(Handle);

    public int SurfaceVSync {
        get {
            var vsync = 0;
            SDL_GetWindowSurfaceVSync(Handle, (int*)Unsafe.AsPointer(ref vsync)).ThrowIfError();
            return vsync;
        }
        set =>SDL_SetWindowSurfaceVSync(Handle, value).ThrowIfError();
    }

    public string Title {
        get => SDL_GetWindowTitle(Handle);
        set => SDL_SetWindowTitle(Handle, value);
    }

    public void Hide() => SDL_HideWindow(Handle).ThrowIfError();
    public void Maximize() => SDL_MaximizeWindow(Handle).ThrowIfError();
    public void Minimize() => SDL_MinimizeWindow(Handle).ThrowIfError();
    public void Raise() => SDL_RaiseWindow(Handle).ThrowIfError();
    public void Restore() => SDL_RestoreWindow(Handle).ThrowIfError();
    public void Show() => SDL_ShowWindow(Handle).ThrowIfError();
    public void ShowSystemMenu(int x, int y) => SDL_ShowWindowSystemMenu(Handle, x, y).ThrowIfError();
    public bool Sync() => SDL_SyncWindow(Handle);
    public void UpdateWindowSurface() => SDL_UpdateWindowSurface(Handle).ThrowIfError();

    public void UpdateWindowSurfaceRects(Rectangle[] rects) {
        var sdlrects = new SDL_Rect[rects.Length];
        var counter = 0;
        foreach (var rect in rects) 
            sdlrects[counter++] = new SDL_Rect {
                x = rect.X,
                y = rect.Y,
                w = rect.Width,
                h = rect.Height,
            };
        fixed (SDL_Rect* sdlrectsptr = sdlrects)
            SDL_UpdateWindowSurfaceRects(Handle, sdlrectsptr, sdlrects.Length);
    }

    public bool Bordered {
        get => !Flags.HasFlag(WindowFlags.Borderless);
        set => SDL_SetWindowBordered(Handle, value).ThrowIfError();
    }

    public bool Focusable {
        get => !Flags.HasFlag(WindowFlags.NotFocusable);
        set => SDL_SetWindowFocusable(Handle, value).ThrowIfError();
    }
    
    public bool Modal {
        //get => !WindowFlags.HasFlag(WindowFlags.Modal); //flag missing
        set => SDL_SetWindowModal(Handle, value).ThrowIfError();
    }
    
    public bool Resizable {
        get => Flags.HasFlag(WindowFlags.Resizable);
        set => SDL_SetWindowResizable(Handle, value).ThrowIfError();
    }

    public void SetHitTest() => throw new NotImplementedException();

    public void SetIcon(SDL_Surface icon) {
        throw new NotImplementedException();
    }

    public void SetShape(SDL_Surface shape) {
        throw new NotImplementedException();
    }

    // public void Setup() {
    //     SDL_SetEventFilter(&NativeFilter, _pin.Pointer);
    //
    //     if (OperatingSystem.IsWindows())
    //         SDL_SetWindowsMessageHook(&WndProc, _pin.Pointer);
    // }
    //
    // [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    // private static SDLBool WndProc(IntPtr userdata, MSG* message) {
    //     var handle = new Pin<Window>(userdata);
    //
    //     //if (handle.TryGetTarget(out var window)) 
    //     //Log.Debug($"from {window}, message: {message->message}");
    //
    //     return true;
    // }
    
    public bool ShouldQuit { get; protected set; }

    public override void Dispose() {
        base.Dispose();
        __windowIdCache.Remove(Id);
        Destroy();
        Renderer?.Dispose();
        _pin.Dispose();
    }
    
    internal void Destroy() => SDL_DestroyWindow(Handle);
}