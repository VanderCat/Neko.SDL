using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Events;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl.Video;
/// <summary>
/// <para>
/// SDL's video subsystem is largely interested in abstracting window management from the underlying operating system.
/// You can create windows, manage them in various ways, set them fullscreen, and get events when interesting things
/// happen with them, such as the mouse or keyboard interacting with a window.
/// </para>
/// <para>
/// The video subsystem is also interested in abstracting away some platform-specific differences in OpenGL: context
/// creation, swapping buffers, etc. This may be crucial to your app, but also you are not required to use OpenGL at
/// all. In fact, SDL can provide rendering to those windows as well, either with an easy-to-use 2D API or with a
/// more-powerful GPU API . Of course, it can simply get out of your way and give you the window handles you need to use
/// Vulkan, Direct3D, Metal, or whatever else you like directly, too.
/// </para>
/// <para>
/// The video subsystem covers a lot of functionality, out of necessity, so it is worth perusing the list of functions
/// just to see what's available, but most apps can get by with simply creating a window and listening for events, so
/// start with <see cref="Create"/> and <see cref="Poll"/>.
/// </para>
/// </summary>

public sealed unsafe partial class Window : SdlWrapper<SDL_Window> {
    public Renderer? Renderer;
    protected Pin<Window> _pin;

    public bool AlwaysOnTop {
        get => Flags.HasFlag(WindowFlags.AlwaysOnTop);
        set => SDL_SetWindowAlwaysOnTop(Handle, value);
    }
    
    /// <summary>
    /// Create a window with the specified dimensions and flags.
    /// </summary>
    /// <param name="width">the width of the window.</param>
    /// <param name="height">the height of the window.</param>
    /// <param name="title">the title of the window.</param>
    /// <param name="windowFlags">0, or one or more <see cref="WindowFlags"/> OR'd together.</param>
    /// <returns>Returns the window that was created</returns>
    /// <remarks>
    /// <para>
    /// The window size is a request and may be different than expected based on the desktop layout and window manager
    /// policies. Your application should be prepared to handle a window of any size.
    /// </para>
    /// flags may be any of the following OR'd together:
    /// <ul>
    ///     <li><see cref="WindowFlags.Fullscreen"/>: fullscreen window at desktop resolution</li>
    ///     <li><see cref="WindowFlags.Opengl"/>: window usable with an OpenGL context</li>
    ///     <li><see cref="WindowFlags.Hidden"/>: window is not visible</li>
    ///     <li><see cref="WindowFlags.Borderless"/>: no window decoration</li>
    ///     <li><see cref="WindowFlags.Resizable"/>: window can be resized</li>
    ///     <li><see cref="WindowFlags.Minimized"/>: window is minimized</li>
    ///     <li><see cref="WindowFlags.Maximized"/>: window is maximized</li>
    ///     <li><see cref="WindowFlags.MouseGrabbed"/>: window has grabbed mouse focus</li>
    ///     <li><see cref="WindowFlags.InputFocus"/>: window has input focus</li>
    ///     <li><see cref="WindowFlags.MouseFocus"/>: window has mouse focus</li>
    ///     <li><see cref="WindowFlags.External"/>: window not created by SDL</li>
    ///     <li><see cref="WindowFlags.Modal"/>: window is modal</li>
    ///     <li><see cref="WindowFlags.HighPixelDensity"/>: window uses high pixel density back buffer if possible</li>
    ///     <li><see cref="WindowFlags.MouseCapture"/>: window has mouse captured (unrelated to MOUSE_GRABBED)</li>
    ///     <li><see cref="WindowFlags.AlwaysOnTop"/>: window should always be above others</li>
    ///     <li><see cref="WindowFlags.Utility"/>: window should be treated as a utility window, not showing in the task bar and window list</li>
    ///     <li><see cref="WindowFlags.Tooltip"/>: window should be treated as a tooltip and does not get mouse or keyboard focus, requires a parent window</li>
    ///     <li><see cref="WindowFlags.PopupMenu"/>: window should be treated as a popup menu, requires a parent window</li>
    ///     <li><see cref="WindowFlags.KeyboardGrabbed"/>: window has grabbed keyboard input</li>
    ///     <li><see cref="WindowFlags.Vulkan"/>: window usable with a Vulkan instance</li>
    ///     <li><see cref="WindowFlags.Metal"/>: window usable with a Metal instance</li>
    ///     <li><see cref="WindowFlags.Transparent"/>: window with transparent buffer</li>
    ///     <li><see cref="WindowFlags.NotFocusable"/>: window should not be focusable</li>
    /// </ul>
    /// <para>
    /// The <see cref="Window"/> will be shown if <see cref="WindowFlags.Hidden"/> is not set. If hidden at creation
    /// time, <see cref="Show"/> can be used to show it later.
    /// </para>
    /// <para>
    /// On Apple's macOS, you must set the NSHighResolutionCapable Info.plist property to YES, otherwise you will not
    /// receive a High-DPI OpenGL canvas.
    /// </para>
    /// <para>
    /// The window pixel size may differ from its window coordinate size if the window is on a high pixel density
    /// display. Use <see cref="Size"/> to query the client area's size in window coordinates, and
    /// <see cref="SizeInPixels"/> or (TODO:)<see cref="SDL_GetRenderOutputSize"/> to query the drawable size in pixels.
    /// Note that the drawable size can vary after the window is created and should be queried again if you get an
    /// <see cref="Neko.Sdl.Events.EventType.WindowPixelSizeChanged"/> event.
    /// </para>
    /// <para>
    /// If the window is created with any of the <see cref="WindowFlags.Opengl"/> or <see cref="WindowFlags.Vulkan"/>
    /// flags, then the corresponding LoadLibrary function (<see cref="Gl.LoadLibrary"/> or
    /// <see cref="Vulkan.LoadLibrary()"/>) is called and the corresponding UnloadLibrary function is called by
    /// <see cref="Dispose"/>.
    /// </para>
    /// <para>
    /// If <see cref="WindowFlags.Vulkan"/> is specified and there isn't a working Vulkan driver, <see cref="Create"/>
    /// will fail, because <see cref="Vulkan.LoadLibrary()"/> will fail.
    /// </para>
    /// <para>
    /// If <see cref="WindowFlags.Metal"/> is specified on an OS that does not support Metal, <see cref="Create"/> will
    /// fail.
    /// </para>
    /// <para>
    /// If you intend to use this window with an <see cref="Neko.Sdl.Video.Renderer"/>, you should use
    /// <see cref="CreateWindowAndRenderer"/> instead of this function, to avoid window flicker.
    /// </para>
    /// <para>
    /// On non-Apple devices, SDL requires you to either not link to the Vulkan loader or link to a dynamic library
    /// version. This limitation may be removed in a future version of SDL.
    /// </para>
    /// </remarks>
    public static Window Create(int width, int height, string? title, WindowFlags windowFlags) {
        using var props = new WindowCreateProperties();
        if (!string.IsNullOrEmpty(title))
            props.Title = title;
        props.Width = width;
        props.Height = height;
        props.Flags = (long)windowFlags;
        return Create(props);
    }

    /// <summary>
    /// Create a child popup window of the parent window.
    /// </summary>
    /// <param name="offsetX">the x position of the popup window relative to the origin of the parent.</param>
    /// <param name="offsetY">the y position of the popup window relative to the origin of the parent window.</param>
    /// <param name="w">the width of the window.</param>
    /// <param name="h">the height of the window.</param>
    /// <param name="flags">
    /// <see cref="Neko.Sdl.Video.WindowFlags.Tooltip"/> or <see cref="Neko.Sdl.Video.WindowFlags.PopupMenu"/>, and zero
    /// or more additional <see cref="Neko.Sdl.Video.WindowFlags"/> OR'd together.</param>
    /// <returns>Returns the window that was created</returns>
    /// <exception cref="SdlException"></exception>
    /// <remarks>
    /// <para>
    /// The window size is a request and may be different than expected based on the desktop layout and window manager
    /// policies. Your application should be prepared to handle a window of any size.
    /// </para>
    /// The flags parameter must contain at least one of the following:
    ///<ul>
    ///     <li>
    ///         <see cref="Neko.Sdl.Video.WindowFlags.Tooltip"/>: The popup window is a tooltip and will not pass any
    ///         input events.
    ///     </li>
    ///     <li>
    ///         <see cref="Neko.Sdl.Video.WindowFlags.PopupMenu"/>: The popup window is a popup menu. The topmost popup
    ///         menu will implicitly gain the keyboard focus.
    ///     </li>
    /// </ul>
    /// The following flags are not relevant to popup window creation and will be ignored:
    /// <ul>
    ///     <li><see cref="Neko.Sdl.Video.WindowFlags.Minimized"/></li>
    ///     <li><see cref="Neko.Sdl.Video.WindowFlags.Maximized"/></li>
    ///     <li><see cref="Neko.Sdl.Video.WindowFlags.Fullscreen"/></li>
    ///     <li><see cref="Neko.Sdl.Video.WindowFlags.Borderless"/></li>
    /// </ul>
    /// The following flags are incompatible with popup window creation and will cause it to fail:
    /// <ul>
    ///     <li><see cref="Neko.Sdl.Video.WindowFlags.Utility"/></li>
    ///     <li><see cref="Neko.Sdl.Video.WindowFlags.Modal"/></li>
    /// </ul>
    /// <para>
    /// The parent parameter must be non-null and a valid window. The parent of a popup window can be either a regular,
    /// toplevel window, or another popup window.
    /// </para>
    /// <para>
    /// Popup windows cannot be minimized, maximized, made fullscreen, raised, flash, be made a modal window, be the
    /// parent of a toplevel window, or grab the mouse and/or keyboard. Attempts to do so will fail.
    /// </para>
    /// <para>
    /// Popup windows implicitly do not have a border/decorations and do not appear on the taskbar/dock or in lists of
    /// windows such as alt-tab menus.
    /// </para>
    /// <para>
    /// By default, popup window positions will automatically be constrained to keep the entire window within display
    /// bounds. This can be overridden with the SDL_PROP_WINDOW_CREATE_CONSTRAIN_POPUP_BOOLEAN property.
    /// </para>
    /// <para>
    /// By default, popup menus will automatically grab keyboard focus from the parent when shown. This behavior can be
    /// overridden by setting the <see cref="Neko.Sdl.Video.WindowFlags.NotFocusable"/> flag, setting the
    /// SDL_PROP_WINDOW_CREATE_FOCUSABLE_BOOLEAN property to false, or toggling it after creation via the
    /// <see cref="Focusable"/> property.
    /// </para>
    /// <para>
    /// If a parent window is hidden or destroyed, any child popup windows will be recursively hidden or destroyed as
    /// well. Child popup windows not explicitly hidden will be restored when the parent is shown.
    /// </para>
    /// </remarks>
    public Window CreatePopup(int offsetX, int offsetY, int w, int h, WindowFlags flags = WindowFlags.PopupMenu) {
        using var props = new WindowCreateProperties();

        // Popups must specify either the tooltip or popup menu window flags
        if (flags is not WindowFlags.Tooltip && flags is not WindowFlags.PopupMenu)
            throw new ArgumentException(
                "Popup windows must specify either the 'WindowFlags.Tooltip' or the 'WindowFlags.PopupMenu' flag",
                nameof(flags));
        
        props.Parent = this;
        props.X = offsetX;
        props.Y = offsetY;
        props.Width = w;
        props.Height = h;
        props.Flags = (long)flags;
        var window = Create(props);

        return window;
    }

    internal Window? _parent;
    internal List<Window> _children = [];

    /// <summary>
    /// Create a window and default renderer.
    /// </summary>
    /// <param name="width">the width of the window.</param>
    /// <param name="height">the height of the window.</param>
    /// <param name="title">the title of the window.</param>
    /// <param name="windowFlags">the flags used to create the window (see <see cref="Create"/>).</param>
    /// <param name="renderer">the result renderer.</param>
    public static Window CreateWithRenderer(int width, int height, string title, WindowFlags windowFlags, out Renderer renderer) {
        // Hide the window so if the renderer recreates it, we don't get a visual flash on screen
        var hidden = (windowFlags & WindowFlags.Hidden) != 0;
        windowFlags |= WindowFlags.Hidden;
        var window = Create(width, height, title, windowFlags);

        try {
            renderer = Renderer.Create(window);
        }
        finally {
            window.Dispose();
        }

        if (!hidden)
            window.Show();

        return window;
    }

    public static Window Create(WindowCreateProperties properties) {
        var handle = SDL_CreateWindowWithProperties(properties);
        if (handle is null) throw new SdlException("Failed to open window");
        var window = new Window(handle);
        __windowIdCache[window.Id] = window;
        
        window._parent = properties.Parent;
        properties.Parent?._children.Add(window);
        
        window._pin = window.Pin(GCHandleType.Normal);
        return window;
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
        foreach (var child in _children) {
            child.Destroy();
        }
        _children = null;
        _parent = null;
        Destroy();
        Renderer?.Dispose();
        _pin.Dispose();
    }
    
    internal void Destroy() => SDL_DestroyWindow(Handle);
}