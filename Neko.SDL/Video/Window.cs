using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Events;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl.Video;

public unsafe partial class Window : SdlWrapper<SDL_Window> {
    public Renderer? Renderer;
    private readonly bool Initialized;
    protected Pin<Window> _pin;

    public bool AlwaysOnTop {
        get => Flags.HasFlag(WindowFlags.AlwaysOnTop);
        set => SDL_SetWindowAlwaysOnTop(Handle, value);
    }
    
    public Window(int width, int height, string title, WindowFlags windowFlags) {
        Create(width, height, title, windowFlags);
        __windowIdCache[Id] = this;
        
        Initialized = true;
        _pin = this.Pin(GCHandleType.Normal);
    }

    public static void CreateWindowAndRenderer(int width, int height, string title, WindowFlags windowFlags, out Window window, out Renderer renderer) {
        SDL_Window* sdlWindow;
        SDL_Renderer* sdlRenderer;
        SDL_CreateWindowAndRenderer(title, width, height, (SDL_WindowFlags)windowFlags, &sdlWindow, &sdlRenderer)
            .ThrowIfError("Failed to create window and renderer");
        window = sdlWindow;
        renderer = sdlRenderer;
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

    protected virtual void Create(int width, int height, string title, WindowFlags windowFlags) {
        Handle = SDL_CreateWindow(title, width, height, (SDL_WindowFlags)windowFlags);
        if (Handle is null) throw new SdlException("Failed to open window:");
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
        if (name is null)
            return Renderer = new Renderer(this, null);
        return Renderer = new Renderer(this, name);
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

    public static Window GetById(uint id) => __windowIdCache[id];

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

    public void Setup() {
        SDL_SetEventFilter(&NativeFilter, _pin.Pointer);

        if (OperatingSystem.IsWindows())
            SDL_SetWindowsMessageHook(&WndProc, _pin.Pointer);
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static SDLBool WndProc(IntPtr userdata, MSG* message) {
        var handle = new Pin<Window>(userdata);

        //if (handle.TryGetTarget(out var window)) 
            //Log.Debug($"from {window}, message: {message->message}");
        
        return true;
    }

    // ReSharper disable once UseCollectionExpression
    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    private static SDLBool NativeFilter(IntPtr userdata, SDL_Event* e) {
        var handle = new Pin<Window>(userdata);
        if (handle.TryGetTarget(out var window))
            return window.HandleEventFromFilter(e);

        return true;
    }

    public Action<Event>? EventFilter;

    private bool HandleEventFromFilter(SDL_Event* e) {
        switch (e->Type) {
            case SDL_EventType.SDL_EVENT_QUIT:
                return OnQuit(e->quit);
            default:
                EventFilter?.Invoke(e);
                break;
        }

        return true;
    }
    
    public bool ShouldQuit { get; protected set; }

    #region Events
    protected virtual bool OnQuit(SDL_QuitEvent e) {
        ShouldQuit = true;
        return true;
    }
    protected virtual void OnKeyDown(SDL_KeyboardEvent e) { }
    protected virtual void OnTextInput(SDL_TextInputEvent e) { }
    protected virtual void OnPenProximityIn(SDL_PenProximityEvent e) { }
    protected virtual void OnTerminating(SDL_CommonEvent e) { }
    protected virtual void OnLowMemory(SDL_CommonEvent e) { }
    protected virtual void OnWillEnterBackground(SDL_CommonEvent e) { }
    protected virtual void OnEnterBackground(SDL_CommonEvent e) { }
    protected virtual void OnWillEnterForeground(SDL_CommonEvent e) { }
    protected virtual void OnEnterForeground(SDL_CommonEvent e) { }
    protected virtual void OnLocaleChanged(SDL_CommonEvent e) { }
    protected virtual void OnSystemThemeChanged(SDL_CommonEvent e) {}
    protected virtual void OnAudioDeviceAdded(SDL_AudioDeviceEvent e) { }
    protected virtual void OnAudioDeviceRemoved(SDL_AudioDeviceEvent e) { }
    protected virtual void OnAudioDeviceFormatChanged(SDL_AudioDeviceEvent e) { }

    protected virtual void OnCameraDeviceAdded(SDL_CameraDeviceEvent e) { }
    protected virtual void OnCameraDeviceRemoved(SDL_CameraDeviceEvent e) { }
    protected virtual void OnCameraDeviceApproved(SDL_CameraDeviceEvent e) { }
    protected virtual void OnCameraDeviceDenied(SDL_CameraDeviceEvent e) { }

    protected virtual void OnClipboardUpdate(SDL_ClipboardEvent e) { }

    protected virtual void OnDisplayAdded(SDL_DisplayEvent e) { }
    protected virtual void OnDisplayRemoved(SDL_DisplayEvent e) { }
    protected virtual void OnDisplayOrientation(SDL_DisplayEvent e) { }
    protected virtual void OnDisplayDesktopModeChanged(SDL_DisplayEvent e) { }
    protected virtual void OnDisplayCurrentModeChanged(SDL_DisplayEvent e) { }
    protected virtual void OnDisplayContentScaleChanged(SDL_DisplayEvent e) { }

    protected virtual void OnGamepadAdded(SDL_GamepadDeviceEvent e) { }
    protected virtual void OnGamepadRemoved(SDL_GamepadDeviceEvent e) { }
    protected virtual void OnGamepadRemapped(SDL_GamepadDeviceEvent e) { }
    protected virtual void OnGamepadAxisMotion(SDL_GamepadAxisEvent e) { }
    protected virtual void OnGamepadButtonDown(SDL_GamepadButtonEvent e) { }
    protected virtual void OnGamepadButtonUp(SDL_GamepadButtonEvent e) { }
    protected virtual void OnGamepadTouchpadDown(SDL_GamepadTouchpadEvent e) { }
    protected virtual void OnGamepadTouchpadMotion(SDL_GamepadTouchpadEvent e) { }
    protected virtual void OnGamepadTouchpadUp(SDL_GamepadTouchpadEvent e) { }
    protected virtual void OnGamepadSensorUpdate(SDL_GamepadSensorEvent e) { }

    protected virtual void OnDropBegin(SDL_DropEvent e) { }
    protected virtual void OnDropFile(SDL_DropEvent e) { }
    protected virtual void OnDropText(SDL_DropEvent e) { }
    protected virtual void OnDropComplete(SDL_DropEvent e) { }
    protected virtual void OnDropPosition(SDL_DropEvent e) { }

    protected virtual void OnFingerMotion(SDL_TouchFingerEvent e) { }
    protected virtual void OnFingerDown(SDL_TouchFingerEvent e) { }
    protected virtual void OnFingerUp(SDL_TouchFingerEvent e) { }

    protected virtual void OnKeyboardAdded(SDL_KeyboardDeviceEvent e) { }
    protected virtual void OnKeyboardRemoved(SDL_KeyboardDeviceEvent e) { }

    protected virtual void OnKeyUp(SDL_KeyboardEvent e) { }
    protected virtual void OnTextEditing(SDL_TextEditingEvent e) { }
    protected virtual void OnTextEditingCandidates(SDL_TextEditingCandidatesEvent e) { }

    protected virtual void OnJoystickAdded(SDL_JoyDeviceEvent e) { }
    protected virtual void OnJoystickRemoved(SDL_JoyDeviceEvent e) { }
    protected virtual void OnJoystickUpdateComplete(SDL_JoyDeviceEvent e) { }
    protected virtual void OnJoystickAxisMotion(SDL_JoyAxisEvent e) { }
    protected virtual void OnJoystickBallMotion(SDL_JoyBallEvent e) { }
    protected virtual void OnJoystickHatMotion(SDL_JoyHatEvent e) { }
    protected virtual void OnJoystickButtonDown(SDL_JoyButtonEvent e) { }
    protected virtual void OnJoystickButtonUp(SDL_JoyButtonEvent e) { }
    protected virtual void OnJoystickBatteryUpdated(SDL_JoyBatteryEvent e) { }

    protected virtual void OnMouseAdded(SDL_MouseDeviceEvent e) { }
    protected virtual void OnMouseRemoved(SDL_MouseDeviceEvent e) { }
    protected virtual void OnMouseMotion(SDL_MouseMotionEvent e) { }
    protected virtual void OnMouseButtonDown(SDL_MouseButtonEvent e) { }
    protected virtual void OnMouseButtonUp(SDL_MouseButtonEvent e) { }
    protected virtual void OnMouseWheel(SDL_MouseWheelEvent e) { }

    protected virtual void OnPenProximityOut(SDL_PenProximityEvent e) { }
    protected virtual void OnPenDown(SDL_PenTouchEvent e) { }
    protected virtual void OnPenUp(SDL_PenTouchEvent e) { }
    protected virtual void OnPenButtonDown(SDL_PenButtonEvent e) { }
    protected virtual void OnPenButtonUp(SDL_PenButtonEvent e) { }
    protected virtual void OnPenMotion(SDL_PenMotionEvent e) { }
    protected virtual void OnPenAxis(SDL_PenAxisEvent e) { }

    protected virtual void OnSensorUpdate(SDL_SensorEvent e) { }

    protected virtual void OnWindowShown(SDL_WindowEvent e) { }
    protected virtual void OnWindowHidden(SDL_WindowEvent e) { }
    protected virtual void OnWindowExposed(SDL_WindowEvent e) { }
    protected virtual void OnWindowMoved(SDL_WindowEvent e) { }
    protected virtual void OnWindowResized(SDL_WindowEvent e) { }
    protected virtual void OnWindowPixelSizeChanged(SDL_WindowEvent e) { }
    protected virtual void OnWindowMetalViewResized(SDL_WindowEvent e) { }
    protected virtual void OnWindowMinimized(SDL_WindowEvent e) { }
    protected virtual void OnWindowMaximized(SDL_WindowEvent e) { }
    protected virtual void OnWindowRestored(SDL_WindowEvent e) { }
    protected virtual void OnWindowMouseEnter(SDL_WindowEvent e) { }
    protected virtual void OnWindowMouseLeave(SDL_WindowEvent e) { }
    protected virtual void OnWindowFocusGained(SDL_WindowEvent e) { }
    protected virtual void OnWindowFocusLost(SDL_WindowEvent e) { }
    protected virtual void OnWindowCloseRequested(SDL_WindowEvent e) { }
    protected virtual void OnWindowHitTest(SDL_WindowEvent e) { }
    protected virtual void OnWindowICCProfChanged(SDL_WindowEvent e) { }
    protected virtual void OnWindowDisplayChanged(SDL_WindowEvent e) { }
    protected virtual void OnWindowDisplayScaleChanged(SDL_WindowEvent e) { }
    protected virtual void OnWindowSafeAreaChanged(SDL_WindowEvent e) { }
    protected virtual void OnWindowOccluded(SDL_WindowEvent e) { }
    protected virtual void OnWindowEnterFullscreen(SDL_WindowEvent e) { }
    protected virtual void OnWindowLeaveFullscreen(SDL_WindowEvent e) { }
    protected virtual void OnWindowDestroyed(SDL_WindowEvent e) { }
    protected virtual void OnWindowHDRStateChanged(SDL_WindowEvent e) { }
    protected virtual void OnGamepadSteamHandleUpdated(SDL_GamepadDeviceEvent e) { }
    protected virtual void OnGamepadUpdateComplete(SDL_GamepadDeviceEvent e) { }
    protected virtual void OnDisplayMoved(SDL_DisplayEvent e) { }
    protected virtual void OnRenderDeviceLost(SDL_RenderEvent e) { }
    protected virtual void OnRenderDeviceReset(SDL_RenderEvent e) { }
    protected virtual void OnRenderTargetsReset(SDL_RenderEvent e) { }
    
    protected virtual void HandleEvent(SDL_Event e) {
        switch (e.Type) {
            case SDL_EventType.SDL_EVENT_QUIT:
                OnQuit(e.quit);
                break;
            case SDL_EventType.SDL_EVENT_KEY_DOWN:
                OnKeyDown(e.key);
                break;
            case SDL_EventType.SDL_EVENT_KEY_UP:
                OnKeyUp(e.key);
                break;
            case SDL_EventType.SDL_EVENT_TEXT_INPUT:
                OnTextInput(e.text);
                break;
            case SDL_EventType.SDL_EVENT_TEXT_EDITING:
                OnTextEditing(e.edit);
                break;
            case SDL_EventType.SDL_EVENT_TEXT_EDITING_CANDIDATES:
                OnTextEditingCandidates(e.edit_candidates);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
                OnGamepadAdded(e.gdevice);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
                OnGamepadRemoved(e.gdevice);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_REMAPPED:
                OnGamepadRemapped(e.gdevice);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
                OnGamepadAxisMotion(e.gaxis);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN:
                OnGamepadButtonDown(e.gbutton);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP:
                OnGamepadButtonUp(e.gbutton);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_DOWN:
                OnGamepadTouchpadDown(e.gtouchpad);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_MOTION:
                OnGamepadTouchpadMotion(e.gtouchpad);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_TOUCHPAD_UP:
                OnGamepadTouchpadUp(e.gtouchpad);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_SENSOR_UPDATE:
                OnGamepadSensorUpdate(e.gsensor);
                break;
            case SDL_EventType.SDL_EVENT_PEN_PROXIMITY_IN:
                OnPenProximityIn(e.pproximity);
                break;
            case SDL_EventType.SDL_EVENT_PEN_PROXIMITY_OUT:
                OnPenProximityOut(e.pproximity);
                break;
            case SDL_EventType.SDL_EVENT_PEN_DOWN:
                OnPenDown(e.ptouch);
                break;
            case SDL_EventType.SDL_EVENT_PEN_UP:
                OnPenUp(e.ptouch);
                break;
            case SDL_EventType.SDL_EVENT_PEN_BUTTON_DOWN:
                OnPenButtonDown(e.pbutton);
                break;
            case SDL_EventType.SDL_EVENT_PEN_BUTTON_UP:
                OnPenButtonUp(e.pbutton);
                break;
            case SDL_EventType.SDL_EVENT_PEN_MOTION:
                OnPenMotion(e.pmotion);
                break;
            case SDL_EventType.SDL_EVENT_PEN_AXIS:
                OnPenAxis(e.paxis);
                break;
            case SDL_EventType.SDL_EVENT_TERMINATING:
                OnTerminating(e.common);
                break;
            case SDL_EventType.SDL_EVENT_LOW_MEMORY:
                OnLowMemory(e.common);
                break;
            case SDL_EventType.SDL_EVENT_WILL_ENTER_BACKGROUND:
                OnWillEnterBackground(e.common);
                break;
            case SDL_EventType.SDL_EVENT_DID_ENTER_BACKGROUND:
                OnEnterBackground(e.common);
                break;
            case SDL_EventType.SDL_EVENT_WILL_ENTER_FOREGROUND:
                OnWillEnterForeground(e.common);
                break;
            case SDL_EventType.SDL_EVENT_DID_ENTER_FOREGROUND:
                OnEnterForeground(e.common);
                break;
            case SDL_EventType.SDL_EVENT_LOCALE_CHANGED:
                OnLocaleChanged(e.common);
                break;
            case SDL_EventType.SDL_EVENT_SYSTEM_THEME_CHANGED:
                OnSystemThemeChanged(e.common);
                break;
            case SDL_EventType.SDL_EVENT_DISPLAY_ORIENTATION:
                OnDisplayOrientation(e.display);
                break;
            case SDL_EventType.SDL_EVENT_DISPLAY_ADDED:
                OnDisplayAdded(e.display);
                break;
            case SDL_EventType.SDL_EVENT_DISPLAY_REMOVED:
                OnDisplayRemoved(e.display);
                break;
            case SDL_EventType.SDL_EVENT_DISPLAY_DESKTOP_MODE_CHANGED:
                OnDisplayDesktopModeChanged(e.display);
                break;
            case SDL_EventType.SDL_EVENT_DISPLAY_CURRENT_MODE_CHANGED:
                OnDisplayCurrentModeChanged(e.display);
                break;
            case SDL_EventType.SDL_EVENT_DISPLAY_CONTENT_SCALE_CHANGED:
                OnDisplayContentScaleChanged(e.display);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_SHOWN:
                OnWindowShown(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_HIDDEN:
                OnWindowHidden(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_EXPOSED:
                OnWindowExposed(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_MOVED:
                OnWindowMoved(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_RESIZED:
                OnWindowResized(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_PIXEL_SIZE_CHANGED:
                OnWindowPixelSizeChanged(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_METAL_VIEW_RESIZED:
                OnWindowMetalViewResized(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_MINIMIZED:
                OnWindowMinimized(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_MAXIMIZED:
                OnWindowMaximized(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_RESTORED:
                OnWindowRestored(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_MOUSE_ENTER:
                OnWindowMouseEnter(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_MOUSE_LEAVE:
                OnWindowMouseLeave(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_GAINED:
                OnWindowFocusGained(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_LOST:
                OnWindowFocusLost(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                OnWindowCloseRequested(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_HIT_TEST:
                OnWindowHitTest(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_ICCPROF_CHANGED:
                OnWindowICCProfChanged(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_DISPLAY_CHANGED:
                OnWindowDisplayChanged(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_DISPLAY_SCALE_CHANGED:
                OnWindowDisplayScaleChanged(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_SAFE_AREA_CHANGED:
                OnWindowSafeAreaChanged(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_OCCLUDED:
                OnWindowOccluded(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_ENTER_FULLSCREEN:
                OnWindowEnterFullscreen(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_LEAVE_FULLSCREEN:
                OnWindowLeaveFullscreen(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_DESTROYED:
                OnWindowDestroyed(e.window);
                break;
            case SDL_EventType.SDL_EVENT_WINDOW_HDR_STATE_CHANGED:
                OnWindowHDRStateChanged(e.window);
                break;
            case SDL_EventType.SDL_EVENT_KEYBOARD_ADDED:
                OnKeyboardAdded(e.kdevice);
                break;
            case SDL_EventType.SDL_EVENT_KEYBOARD_REMOVED:
                OnKeyboardRemoved(e.kdevice);
                break;
            case SDL_EventType.SDL_EVENT_KEYMAP_CHANGED:
                // Handle keymap change event
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
                OnMouseMotion(e.motion);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
                OnMouseButtonDown(e.button);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
                OnMouseButtonUp(e.button);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_WHEEL:
                OnMouseWheel(e.wheel);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_ADDED:
                OnMouseAdded(e.mdevice);
                break;
            case SDL_EventType.SDL_EVENT_MOUSE_REMOVED:
                OnMouseRemoved(e.mdevice);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_AXIS_MOTION:
                OnJoystickAxisMotion(e.jaxis);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_BALL_MOTION:
                OnJoystickBallMotion(e.jball);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_HAT_MOTION:
                OnJoystickHatMotion(e.jhat);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_BUTTON_DOWN:
                OnJoystickButtonDown(e.jbutton);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_BUTTON_UP:
                OnJoystickButtonUp(e.jbutton);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_ADDED:
                OnJoystickAdded(e.jdevice);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_REMOVED:
                OnJoystickRemoved(e.jdevice);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_BATTERY_UPDATED:
                OnJoystickBatteryUpdated(e.jbattery);
                break;
            case SDL_EventType.SDL_EVENT_JOYSTICK_UPDATE_COMPLETE:
                OnJoystickUpdateComplete(e.jdevice);
                break;
            case SDL_EventType.SDL_EVENT_AUDIO_DEVICE_ADDED:
                OnAudioDeviceAdded(e.adevice);
                break;
            case SDL_EventType.SDL_EVENT_AUDIO_DEVICE_REMOVED:
                OnAudioDeviceRemoved(e.adevice);
                break;
            case SDL_EventType.SDL_EVENT_AUDIO_DEVICE_FORMAT_CHANGED:
                OnAudioDeviceFormatChanged(e.adevice);
                break;
            case SDL_EventType.SDL_EVENT_SENSOR_UPDATE:
                OnSensorUpdate(e.sensor);
                break;
            case SDL_EventType.SDL_EVENT_CLIPBOARD_UPDATE:
                OnClipboardUpdate(e.clipboard);
                break;
            case SDL_EventType.SDL_EVENT_DROP_BEGIN:
                OnDropBegin(e.drop);
                break;
            case SDL_EventType.SDL_EVENT_DROP_FILE:
                OnDropFile(e.drop);
                break;
            case SDL_EventType.SDL_EVENT_DROP_TEXT:
                OnDropText(e.drop);
                break;
            case SDL_EventType.SDL_EVENT_DROP_COMPLETE:
                OnDropComplete(e.drop);
                break;
            case SDL_EventType.SDL_EVENT_DROP_POSITION:
                OnDropPosition(e.drop);
                break;
            case SDL_EventType.SDL_EVENT_FINGER_DOWN:
                OnFingerDown(e.tfinger);
                break;
            case SDL_EventType.SDL_EVENT_FINGER_UP:
                OnFingerUp(e.tfinger);
                break;
            case SDL_EventType.SDL_EVENT_FINGER_MOTION:
                OnFingerMotion(e.tfinger);
                break;
            case SDL_EventType.SDL_EVENT_CAMERA_DEVICE_ADDED:
                OnCameraDeviceAdded(e.cdevice);
                break;
            case SDL_EventType.SDL_EVENT_CAMERA_DEVICE_REMOVED:
                OnCameraDeviceRemoved(e.cdevice);
                break;
            case SDL_EventType.SDL_EVENT_CAMERA_DEVICE_APPROVED:
                OnCameraDeviceApproved(e.cdevice);
                break;
            case SDL_EventType.SDL_EVENT_CAMERA_DEVICE_DENIED:
                OnCameraDeviceDenied(e.cdevice);
                break;
            case SDL_EventType.SDL_EVENT_RENDER_TARGETS_RESET:
                OnRenderTargetsReset(e.render);
                break;
            case SDL_EventType.SDL_EVENT_RENDER_DEVICE_RESET:
                OnRenderDeviceReset(e.render);
                break;
            case SDL_EventType.SDL_EVENT_RENDER_DEVICE_LOST:
                OnRenderDeviceLost(e.render);
                break;
            case SDL_EventType.SDL_EVENT_DISPLAY_MOVED:
                OnDisplayMoved(e.display);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_UPDATE_COMPLETE:
                OnGamepadUpdateComplete(e.gdevice);
                break;
            case SDL_EventType.SDL_EVENT_GAMEPAD_STEAM_HANDLE_UPDATED:
                OnGamepadSteamHandleUpdated(e.gdevice);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    #endregion

    private const int eventsPerPeep = 64;
    private readonly SDL_Event[] events = new SDL_Event[eventsPerPeep];

    public void PollEvents() {
        SDL_PumpEvents();
        int eventsRead;
        do {
            eventsRead = SDL_PeepEvents(events, SDL_EventAction.SDL_GETEVENT, SDL_EventType.SDL_EVENT_FIRST, SDL_EventType.SDL_EVENT_LAST);
            for (int i = 0; i < eventsRead; i++)
                HandleEvent(events[i]);
        } while (eventsRead == eventsPerPeep);
    }

    public override void Dispose() {
        base.Dispose();
        __windowIdCache.Remove(Id);
        if (Initialized) {
            Destroy();
            Renderer?.Dispose();
        }
        _pin.Dispose();
    }
    
    internal void Destroy() => SDL_DestroyWindow(Handle);
}