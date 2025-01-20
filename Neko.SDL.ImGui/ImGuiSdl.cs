using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImGuiNET;
using Neko.Sdl;
using Neko.Sdl.Events;
using Neko.Sdl.Extra;
using Neko.Sdl.Input;
using Neko.Sdl.Video;
using Rectangle = Neko.Sdl.Rectangle;
using Timer = Neko.Sdl.Time.Timer;

namespace Neko.Sdl.ImGuiBackend;

public static unsafe class ImGuiSdl {
    // dear imgui: Platform Backend for SDL3 (*EXPERIMENTAL*)
    // This needs to be used along with a Renderer (e.g. DirectX11, OpenGL3, Vulkan..)
    // (Info: SDL3 is a cross-platform general purpose library for handling windows, inputs, graphics context creation, etc.)

    // (**IMPORTANT: SDL 3.0.0 is NOT YET RELEASED AND CURRENTLY HAS A FAST CHANGING API. THIS CODE BREAKS OFTEN AS SDL3 CHANGES.**)

    // Implemented features:
    //  [X] Platform: Clipboard support.
    //  [X] Platform: Mouse support. Can discriminate Mouse/TouchScreen.
    //  [X] Platform: Keyboard support. Since 1.87 we are using the io.AddKeyEvent() function. Pass ImGuiKey values to all key functions e.g. ImGui::IsKeyPressed(ImGuiKey.Space). [Legacy SDL_SCANCODE_* values are obsolete since 1.87 and not supported since 1.91.5]
    //  [X] Platform: Gamepad support. Enabled with 'io.ConfigFlags |= ImGuiConfigFlags_NavEnableGamepad'.
    //  [X] Platform: Mouse cursor shape and visibility (ImGuiBackendFlags.HasMouseCursors). Disable with 'io.ConfigFlags |= ImGuiConfigFlags_NoMouseCursorChange'.
    //  [x] Platform: Multi-viewport support (multiple windows). Enable with 'io.ConfigFlags |= ImGuiConfigFlags_ViewportsEnable' -> the OS animation effect when window gets created/destroyed is problematic. SDL2 backend doesn't have issue.
    // Missing features or Issues:
    //  [ ] Platform: Multi-viewport: Minimized windows seems to break mouse wheel events (at least under Windows).
    //  [x] Platform: IME support. Position somehow broken in SDL3 + app needs to call 'SDL_SetHint(SDL_HINT_IME_SHOW_UI, "1");' before SDL_CreateWindow()!.

    // You can use unmodified imgui_impl_* files in your project. See examples/ folder for examples of using this.
    // Prefer including the entire imgui/ repository into your project (either as a copy or as a submodule), and only build the backends you need.
    // Learn about Dear ImGui:
    // - FAQ                  https://dearimgui.com/faq
    // - Getting Started      https://dearimgui.com/getting-started
    // - Documentation        https://dearimgui.com/docs (same as your local docs/ folder).
    // - Introduction, links and more at the top of imgui.cpp

    // CHANGELOG
    // (minor and older changes stripped away, please see git history for details)
    //  2025-XX-XX: Platform: Added support for multiple windows via the ImGuiPlatformIO interface.
    //  2024-09-11: (Docking) Added support for viewport.ParentViewportId field to support parenting at OS level. (#797
    //  2024-10-24: Emscripten: EventType.MOUSE_WHEEL event doesn't require dividing by 100.0f on Emscripten.
    //  2024-09-03: Update for SDL3 api changes: SDL_GetGamepads() memory ownership revert. (#7918, #7898, #7807)
    //  2024-08-22: moved some OS/backend related function pointers from ImGuiIO to ImGuiPlatformIO:
    //               - io.GetClipboardTextFn    -> platform_io.Platform_GetClipboardTextFn
    //               - io.SetClipboardTextFn    -> platform_io.Platform_SetClipboardTextFn
    //               - io.PlatformSetImeDataFn  -> platform_io.Platform_SetImeDataFn
    //  2024-08-19: Storing SDL_WindowID inside ImGuiViewport::PlatformHandle instead of Window.
    //  2024-08-19: ProcessEvent() now ignores events intended for other SDL windows. (#7853)
    //  2024-07-22: Update for SDL3 api changes: SDL_GetGamepads() memory ownership change. (#7807)
    //  2024-07-18: Update for SDL3 api changes: SDL_GetClipboardText() memory ownership change. (#7801)
    //  2024-07-15: Update for SDL3 api changes: SDL_GetProperty() change to SDL_GetPointerProperty(). (#7794)
    //  2024-07-02: Update for SDL3 api changes: Keycode.x renames and Keycode.KP_x removals (#7761, #7762).
    //  2024-07-01: Update for SDL3 api changes: SDL_SetTextInputRect() changed to SDL_SetTextInputArea().
    //  2024-06-26: Update for SDL3 api changes: SDL_StartTextInput()/SDL_StopTextInput()/SDL_SetTextInputRect() functions signatures.
    //  2024-06-24: Update for SDL3 api changes: EventType.KEY_DOWN/EventType.KEY_UP contents.
    //  2024-06-03; Update for SDL3 api changes: SystemCursor. renames.
    //  2024-05-15: Update for SDL3 api changes: Keycode. renames.
    //  2024-04-15: Inputs: Re-enable calling SDL_StartTextInput()/SDL_StopTextInput() as SDL3 no longer enables it by default and should play nicer with IME.
    //  2024-02-13: Inputs: Fixed gamepad support. Handle gamepad disconnection. Added SetGamepadMode().
    //  2023-11-13: Updated for recent SDL3 API changes.
    //  2023-10-05: Inputs: Added support for extra ImGuiKey values: F13 to F24 function keys, app back/forward keys.
    //  2023-05-04: Fixed build on Emscripten/iOS/Android. (#6391)
    //  2023-04-06: Inputs: Avoid calling SDL_StartTextInput()/SDL_StopTextInput() as they don't only pertain to IME. It's unclear exactly what their relation is to IME. (#6306)
    //  2023-04-04: Inputs: Added support for io.AddMouseSourceEvent() to discriminate ImGuiMouseSource.Mouse/ImGuiMouseSource.TouchScreen. (#2702)
    //  2023-02-23: Accept SDL_GetPerformanceCounter() not returning a monotonically increasing value. (#6189, #6114, #3644)
    //  2023-02-07: Forked "imgui_impl_sdl2" into "imgui_impl_sdl3". Removed version checks for old feature. Refer to imgui_impl_sdl2.cpp for older changelog.


    // SDL Data
    public enum GamepadMode { AutoFirst, AutoAll, Manual };
    class Data {
        public Window Window;
        public Renderer? Renderer;
        public ulong Time;
        public char* ClipboardTextData;
        public bool UseVulkan;
        public bool WantUpdateMonitors;

        // IME handling
        public Window? ImeWindow;

        // Mouse handling
        public uint MouseWindowID;
        public int MouseButtonsDown;
        public Dictionary<ImGuiMouseCursor, Cursor> MouseCursors = new();
        public Cursor MouseLastCursor;
        public int MousePendingLeaveFrame;
        public bool MouseCanUseGlobalState;
        public bool MouseCanReportHoveredViewport; // This is hard to use/unreliable on SDL so we'll set ImGuiBackendFlags_HasMouseHoveredViewport dynamically based on state.

        // Gamepad handling
        public List<IntPtr> Gamepads = new();
        public GamepadMode  GamepadMode;
        public bool WantUpdateGamepadsList;
    };

    // Backend data stored in io.BackendPlatformUserData to allow support for multiple Dear ImGui contexts
    // It is STRONGLY preferred that you use docking branch with multi-viewports (== single Dear ImGui context + multiple windows) instead of multiple Dear ImGui contexts.
    // FIXME: multi-context support is not well tested and probably dysfunctional in this backend.
    // FIXME: some shared resources (mouse cursor shape, gamepad) are mishandled when using multi-context.
    private static Data _backend;
    private static Pin<Data> _pin;

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static char* GetClipboardText(void* context) {
        if (_backend.ClipboardTextData is not null)
            Marshal.FreeHGlobal((IntPtr)_backend.ClipboardTextData);
        var sdlClipboardText = Clipboard.Text;
        var ptr = Marshal.StringToHGlobalAnsi(sdlClipboardText);
        _backend.ClipboardTextData = sdlClipboardText is not null ? (char*)ptr : null;
        return _backend.ClipboardTextData;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetClipboardText(void* context, byte* text) => 
        Clipboard.Text = Marshal.PtrToStringUTF8((IntPtr)text);
    

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void PlatformSetImeData(void* context, ImGuiViewport* viewport, ImGuiPlatformImeData* data) {
        var windowId = (uint)viewport->PlatformHandle;
        var window = Window.GetById(windowId);
        if ((data->WantVisible == 0 || _backend.ImeWindow != window) && _backend.ImeWindow is not null) {
            TextInput.Stop(_backend.ImeWindow);
            _backend.ImeWindow = null;
        }

        if (data->WantVisible != 1) return;

        Rectangle r = new() {
            X = (int)(data->InputPos.X - viewport->Pos.X),
            Y = (int)(data->InputPos.Y - viewport->Pos.Y + data->InputLineHeight),
            Width = 1,
            Height = (int)data->InputLineHeight,
        };
        TextInput.SetArea(window, r, 0);
        TextInput.Start(window);
        _backend.ImeWindow = window;
    }

    public static ImGuiKey KeyEventToImGuiKey(Keycode keycode, Scancode scancode)
    {
        // Keypad doesn't have individual key values in SDL3
        switch (scancode)
        {
            case Scancode.Kp0: return ImGuiKey.Keypad0;
            case Scancode.Kp1: return ImGuiKey.Keypad1;
            case Scancode.Kp2: return ImGuiKey.Keypad2;
            case Scancode.Kp3: return ImGuiKey.Keypad3;
            case Scancode.Kp4: return ImGuiKey.Keypad4;
            case Scancode.Kp5: return ImGuiKey.Keypad5;
            case Scancode.Kp6: return ImGuiKey.Keypad6;
            case Scancode.Kp7: return ImGuiKey.Keypad7;
            case Scancode.Kp8: return ImGuiKey.Keypad8;
            case Scancode.Kp9: return ImGuiKey.Keypad9;
            case Scancode.KpPeriod: return ImGuiKey.KeypadDecimal;
            case Scancode.KpDivide: return ImGuiKey.KeypadDivide;
            case Scancode.KpMultiply: return ImGuiKey.KeypadMultiply;
            case Scancode.KpMinus: return ImGuiKey.KeypadSubtract;
            case Scancode.KpPlus: return ImGuiKey.KeypadAdd;
            case Scancode.KpEnter: return ImGuiKey.KeypadEnter;
            case Scancode.KpEquals: return ImGuiKey.KeypadEqual;
        }
        switch (keycode)
        {
            case Keycode.Tab: return ImGuiKey.Tab;
            case Keycode.Left: return ImGuiKey.LeftArrow;
            case Keycode.Right: return ImGuiKey.RightArrow;
            case Keycode.Up: return ImGuiKey.UpArrow;
            case Keycode.Down: return ImGuiKey.DownArrow;
            case Keycode.Pageup: return ImGuiKey.PageUp;
            case Keycode.Pagedown: return ImGuiKey.PageDown;
            case Keycode.Home: return ImGuiKey.Home;
            case Keycode.End: return ImGuiKey.End;
            case Keycode.Insert: return ImGuiKey.Insert;
            case Keycode.Delete: return ImGuiKey.Delete;
            case Keycode.Backspace: return ImGuiKey.Backspace;
            case Keycode.Space: return ImGuiKey.Space;
            case Keycode.Return: return ImGuiKey.Enter;
            case Keycode.Escape: return ImGuiKey.Escape;
            case Keycode.Apostrophe: return ImGuiKey.Apostrophe;
            case Keycode.Comma: return ImGuiKey.Comma;
            case Keycode.Minus: return ImGuiKey.Minus;
            case Keycode.Period: return ImGuiKey.Period;
            case Keycode.Slash: return ImGuiKey.Slash;
            case Keycode.Semicolon: return ImGuiKey.Semicolon;
            case Keycode.Equals: return ImGuiKey.Equal;
            case Keycode.Leftbracket: return ImGuiKey.LeftBracket;
            case Keycode.Backslash: return ImGuiKey.Backslash;
            case Keycode.Rightbracket: return ImGuiKey.RightBracket;
            case Keycode.Grave: return ImGuiKey.GraveAccent;
            case Keycode.Capslock: return ImGuiKey.CapsLock;
            case Keycode.Scrolllock: return ImGuiKey.ScrollLock;
            case Keycode.Numlockclear: return ImGuiKey.NumLock;
            case Keycode.Printscreen: return ImGuiKey.PrintScreen;
            case Keycode.Pause: return ImGuiKey.Pause;
            case Keycode.Lctrl: return ImGuiKey.LeftCtrl;
            case Keycode.Lshift: return ImGuiKey.LeftShift;
            case Keycode.Lalt: return ImGuiKey.LeftAlt;
            case Keycode.Lgui: return ImGuiKey.LeftSuper;
            case Keycode.Rctrl: return ImGuiKey.RightCtrl;
            case Keycode.Rshift: return ImGuiKey.RightShift;
            case Keycode.Ralt: return ImGuiKey.RightAlt;
            case Keycode.Rgui: return ImGuiKey.RightSuper;
            case Keycode.Application: return ImGuiKey.Menu;
            case Keycode._0: return ImGuiKey._0;
            case Keycode._1: return ImGuiKey._1;
            case Keycode._2: return ImGuiKey._2;
            case Keycode._3: return ImGuiKey._3;
            case Keycode._4: return ImGuiKey._4;
            case Keycode._5: return ImGuiKey._5;
            case Keycode._6: return ImGuiKey._6;
            case Keycode._7: return ImGuiKey._7;
            case Keycode._8: return ImGuiKey._8;
            case Keycode._9: return ImGuiKey._9;
            case Keycode.A: return ImGuiKey.A;
            case Keycode.B: return ImGuiKey.B;
            case Keycode.C: return ImGuiKey.C;
            case Keycode.D: return ImGuiKey.D;
            case Keycode.E: return ImGuiKey.E;
            case Keycode.F: return ImGuiKey.F;
            case Keycode.G: return ImGuiKey.G;
            case Keycode.H: return ImGuiKey.H;
            case Keycode.I: return ImGuiKey.I;
            case Keycode.J: return ImGuiKey.J;
            case Keycode.K: return ImGuiKey.K;
            case Keycode.L: return ImGuiKey.L;
            case Keycode.M: return ImGuiKey.M;
            case Keycode.N: return ImGuiKey.N;
            case Keycode.O: return ImGuiKey.O;
            case Keycode.P: return ImGuiKey.P;
            case Keycode.Q: return ImGuiKey.Q;
            case Keycode.R: return ImGuiKey.R;
            case Keycode.S: return ImGuiKey.S;
            case Keycode.T: return ImGuiKey.T;
            case Keycode.U: return ImGuiKey.U;
            case Keycode.V: return ImGuiKey.V;
            case Keycode.W: return ImGuiKey.W;
            case Keycode.X: return ImGuiKey.X;
            case Keycode.Y: return ImGuiKey.Y;
            case Keycode.Z: return ImGuiKey.Z;
            case Keycode.F1: return ImGuiKey.F1;
            case Keycode.F2: return ImGuiKey.F2;
            case Keycode.F3: return ImGuiKey.F3;
            case Keycode.F4: return ImGuiKey.F4;
            case Keycode.F5: return ImGuiKey.F5;
            case Keycode.F6: return ImGuiKey.F6;
            case Keycode.F7: return ImGuiKey.F7;
            case Keycode.F8: return ImGuiKey.F8;
            case Keycode.F9: return ImGuiKey.F9;
            case Keycode.F10: return ImGuiKey.F10;
            case Keycode.F11: return ImGuiKey.F11;
            case Keycode.F12: return ImGuiKey.F12;
            case Keycode.F13: return ImGuiKey.F13;
            case Keycode.F14: return ImGuiKey.F14;
            case Keycode.F15: return ImGuiKey.F15;
            case Keycode.F16: return ImGuiKey.F16;
            case Keycode.F17: return ImGuiKey.F17;
            case Keycode.F18: return ImGuiKey.F18;
            case Keycode.F19: return ImGuiKey.F19;
            case Keycode.F20: return ImGuiKey.F20;
            case Keycode.F21: return ImGuiKey.F21;
            case Keycode.F22: return ImGuiKey.F22;
            case Keycode.F23: return ImGuiKey.F23;
            case Keycode.F24: return ImGuiKey.F24;
            case Keycode.AcBack: return ImGuiKey.AppBack;
            case Keycode.AcForward: return ImGuiKey.AppForward;
        }
        return ImGuiKey.None;
    }

    static void UpdateKeyModifiers(Keymod sdlKeyMods)
    {
        var io = ImGui.GetIO();
        io.AddKeyEvent(ImGuiKey.ModCtrl, (sdlKeyMods & Keymod.Ctrl) != 0);
        io.AddKeyEvent(ImGuiKey.ModShift, (sdlKeyMods & Keymod.Shift) != 0);
        io.AddKeyEvent(ImGuiKey.ModAlt, (sdlKeyMods & Keymod.Alt) != 0);
        io.AddKeyEvent(ImGuiKey.ModSuper, (sdlKeyMods & Keymod.Gui) != 0);
    }


    public static ImGuiViewportPtr GetViewportForWindowID(uint windowId) =>
        ImGui.FindViewportByPlatformHandle((nint)windowId);
        
    public static ImGuiViewportPtr GetImGuiViewport(this Window window) =>
        GetViewportForWindowID(window.Id);

    // You can read the io.WantCaptureMouse, io.WantCaptureKeyboard flags to tell if dear imgui wants to use your inputs.
    // - When io.WantCaptureMouse is true, do not dispatch mouse input data to your main application, or clear/overwrite your copy of the mouse data.
    // - When io.WantCaptureKeyboard is true, do not dispatch keyboard input data to your main application, or clear/overwrite your copy of the keyboard data.
    // Generally you may always pass all inputs to dear imgui, and hide them from your application based on those two flags.
    // If you have multiple SDL events and some of them are not meant to be used by dear imgui, you may need to filter events based on their windowID field.
    public static bool ProcessEvent(SDL_Event* e) {
        if (_backend is null) 
            throw new Exception("Context or backend not initialized! Did you call Init()?");
        
        var io = ImGui.GetIO();
        var type = (EventType)e->type;
        ImGuiViewportPtr viewport;
        switch (type) {
            case EventType.MouseMotion:
                viewport = GetViewportForWindowID((uint)e->motion.windowID);
                if (viewport.NativePtr is null)
                    return false;
                var mousePos = new Vector2(e->motion.x, e->motion.y);
                if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable)) {
                    var windowPos = Window.GetById((uint)e->motion.windowID).Position;
                    mousePos = mousePos with { X = mousePos.X + windowPos.X, Y = mousePos.Y + windowPos.Y };
                }
                io.AddMouseSourceEvent(e->motion.which == SDL_TOUCH_MOUSEID ? ImGuiMouseSource.TouchScreen : ImGuiMouseSource.Mouse);
                io.AddMousePosEvent(mousePos.X, mousePos.Y);
                return true;
            case EventType.MouseWheel:
                viewport = GetViewportForWindowID((uint)e->wheel.windowID);
                if (viewport.NativePtr is null)
                    return false;
                var wheelX = -e->wheel.x;
                var wheelY = e->wheel.y;
                io.AddMouseSourceEvent(e->wheel.which == SDL_TOUCH_MOUSEID ? ImGuiMouseSource.TouchScreen : ImGuiMouseSource.Mouse);
                io.AddMouseWheelEvent(wheelX, wheelY);
                return true;
            case EventType.MouseButtonDown:
            case EventType.MouseButtonUp:
                viewport = GetViewportForWindowID((uint)e->button.windowID);
                if (viewport.NativePtr is null)
                    return false;
                int mouseButton = -1;
                if (e->button.button == SDL_BUTTON_LEFT) { mouseButton = 0; }
                if (e->button.button == SDL_BUTTON_RIGHT) { mouseButton = 1; }
                if (e->button.button == SDL_BUTTON_MIDDLE) { mouseButton = 2; }
                if (e->button.button == SDL_BUTTON_X1) { mouseButton = 3; }
                if (e->button.button == SDL_BUTTON_X2) { mouseButton = 4; }
                if (mouseButton == -1)
                    break;
                io.AddMouseSourceEvent(e->button.which == SDL_TOUCH_MOUSEID ? ImGuiMouseSource.TouchScreen : ImGuiMouseSource.Mouse);
                io.AddMouseButtonEvent(mouseButton, e->Type == (SDL_EventType)EventType.MouseButtonDown);
                _backend.MouseButtonsDown = (e->type == (uint)EventType.MouseButtonDown) ? (_backend.MouseButtonsDown | (1 << mouseButton)) : (_backend.MouseButtonsDown & ~(1 << mouseButton));
                return true;
            case EventType.TextInput:
                viewport = GetViewportForWindowID((uint)e->text.windowID);
                if (viewport.NativePtr is null)
                    return false;
                io.AddInputCharactersUTF8(new ReadOnlySpan<char>(e->text.text, (int)SDL_strlen(e->text.text)));
                return true;
            case EventType.KeyDown:
            case EventType.KeyUp:
                viewport = GetViewportForWindowID((uint)e->key.windowID);
                if (viewport.NativePtr is null)
                    return false;
                UpdateKeyModifiers((Keymod)e->key.mod);
                var key = KeyEventToImGuiKey((Keycode)e->key.key, (Scancode)e->key.scancode);
                io.AddKeyEvent(key, type is EventType.KeyDown);
                io.SetKeyEventNativeData(key, (int)e->key.key, (int)e->key.scancode, (int)e->key.scancode); // To support legacy indexing (<1.87 user code). Legacy backend uses Keycode.*** as indices to IsKeyXXX() functions.
                return true;
            case EventType.DisplayOrientation:
            case EventType.DisplayAdded:
            case EventType.DisplayRemoved:
            case EventType.DisplayMoved:
            case EventType.DisplayContentScaleChanged:
                _backend.WantUpdateMonitors = true;
                return true;
            case EventType.WindowMouseEnter:
                _backend.MouseWindowID = (uint)e->window.windowID;
                _backend.MousePendingLeaveFrame = 0;
                return true;
            // - In some cases, when detaching a window from main viewport SDL may send SDL_WINDOWEVENT_ENTER one frame too late,
            //   causing SDL_WINDOWEVENT_LEAVE on previous frame to interrupt drag operation by clear mouse position. This is why
            //   we delay process the SDL_WINDOWEVENT_LEAVE events by one frame. See issue #5012 for details.
            // FIXME: Unconfirmed whether this is still needed with SDL3.
            case EventType.WindowMouseLeave:
                _backend.MousePendingLeaveFrame = ImGui.GetFrameCount() + 1;
                return true;
            case EventType.WindowFocusGained:
            case EventType.WindowFocusLost:
                io.AddFocusEvent(type is EventType.WindowFocusGained);
                return true;
            case EventType.WindowCloseRequested:
                viewport = GetViewportForWindowID((uint)e->window.windowID);
                if (viewport.NativePtr is null)
                    return false;
                viewport.PlatformRequestClose = true;
                return true;
            case EventType.WindowMoved:
                viewport = GetViewportForWindowID((uint)e->window.windowID);
                if (viewport.NativePtr is null)
                    return false;
                viewport.PlatformRequestMove = true;
                return true;
            case EventType.WindowResized:
                viewport = GetViewportForWindowID((uint)e->window.windowID);
                if (viewport.NativePtr is null)
                    return false;
                viewport.PlatformRequestResize = true;
                return true;
            case EventType.GamepadAdded:
            case EventType.GamepadRemoved:
                _backend.WantUpdateGamepadsList = true;
                return true;
        }
        return false;
    }

    public static void SetupPlatformHandles(ImGuiViewportPtr viewport, Window window) {
        viewport.PlatformHandle = (IntPtr)window.Id;
        viewport.PlatformHandleRaw = 0;
        if (OperatingSystem.IsWindows())
            viewport.PlatformHandleRaw = 
                window.Properties.GetPointer(SDL_PROP_WINDOW_WIN32_HWND_POINTER, 0);
        if (OperatingSystem.IsMacOS())
            viewport.PlatformHandleRaw = 
                window.Properties.GetPointer(SDL_PROP_WINDOW_COCOA_WINDOW_POINTER, 0);
    }

    static void Init(Window window, Renderer? renderer = null, IntPtr sdlGlСontext = 0)
    {
        var io = ImGui.GetIO();
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        if (io.BackendPlatformUserData != 0)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            throw new InvalidOperationException("Already initialized a platform backend!");

        // Check and store if we are on a SDL backend that supports global mouse position
        // ("wayland" and "rpi" don't support it, but we chose to use a white-list instead of a black-list)
        bool mouseCanUseGlobalState = false;

        if (HasCaptureAndGlobalMouse) {
            var sdlBackend = VideoDriver.Current;
            if (sdlBackend is null) 
                throw new InvalidOperationException("Video driver have not been initialized yet!");
            string[] globalMouseWhitelist = [
                "windows", "cocoa", "x11", "DIVE", "VMAN"
            ];
            mouseCanUseGlobalState = globalMouseWhitelist.Any(whitelist => sdlBackend == whitelist);
        }

        // Setup backend capabilities flags
        _backend = new Data();
        _pin = _backend.Pin(GCHandleType.Normal);
        io.BackendPlatformUserData = _pin.Pointer;

        ((ImGuiIO*)io)->BackendPlatformName = (byte*)Marshal.StringToHGlobalAnsi("imgui_impl_nekosdl");
        
        io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;           // We can honor GetMouseCursor() values (optional)
        io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;            // We can honor io.WantSetMousePos requests (optional, rarely used)
        if (mouseCanUseGlobalState)
            io.BackendFlags |= ImGuiBackendFlags.PlatformHasViewports;  // We can create multi-viewports on the Platform side (optional)

        _backend.Window = window;
        _backend.Renderer = renderer;
        
        // SDL on Linux/OSX doesn't report events for unfocused windows (see https://github.com/ocornut/imgui/issues/4960)
        // We will use 'MouseCanReportHoveredViewport' to set 'ImGuiBackendFlags_HasMouseHoveredViewport' dynamically each frame.
        _backend.MouseCanUseGlobalState = mouseCanUseGlobalState;
        if (IsApple)
            _backend.MouseCanReportHoveredViewport = _backend.MouseCanUseGlobalState;
        else
            _backend.MouseCanReportHoveredViewport = false;

        var platformIo = (ImGuiPlatformIO*)ImGui.GetPlatformIO();
        delegate*unmanaged[Cdecl]<void*, byte*, void> setClipboardText = &SetClipboardText;
        delegate*unmanaged[Cdecl]<void*, char*> getClipboardText = &GetClipboardText;
        delegate*unmanaged[Cdecl]<void*, ImGuiViewport*, ImGuiPlatformImeData*, void> setImeData = &PlatformSetImeData;
        platformIo->Platform_SetClipboardTextFn = (IntPtr)setClipboardText;
        platformIo->Platform_GetClipboardTextFn = (IntPtr)getClipboardText;
        platformIo->Platform_SetImeDataFn = (IntPtr)setImeData;
        
        // Update monitor a first time during init
        UpdateMonitors();

        // Gamepad handling
        _backend.GamepadMode = GamepadMode.AutoFirst;
        _backend.WantUpdateGamepadsList = true;

        // Load mouse cursors
        _backend.MouseCursors[ImGuiMouseCursor.Arrow] = Cursor.CreateSystem(SystemCursor.Default);
        _backend.MouseCursors[ImGuiMouseCursor.TextInput] = Cursor.CreateSystem(SystemCursor.Text);
        _backend.MouseCursors[ImGuiMouseCursor.ResizeAll] = Cursor.CreateSystem(SystemCursor.Move);
        _backend.MouseCursors[ImGuiMouseCursor.ResizeNS] = Cursor.CreateSystem(SystemCursor.NsResize);
        _backend.MouseCursors[ImGuiMouseCursor.ResizeEW] = Cursor.CreateSystem(SystemCursor.EwResize);
        _backend.MouseCursors[ImGuiMouseCursor.ResizeNESW] = Cursor.CreateSystem(SystemCursor.NeswResize);
        _backend.MouseCursors[ImGuiMouseCursor.ResizeNWSE] = Cursor.CreateSystem(SystemCursor.NwseResize);
        _backend.MouseCursors[ImGuiMouseCursor.Hand] = Cursor.CreateSystem(SystemCursor.Pointer);
        _backend.MouseCursors[ImGuiMouseCursor.NotAllowed] = Cursor.CreateSystem(SystemCursor.NotAllowed);

        // Set platform dependent data in viewport
        // Our mouse update function expect PlatformHandle to be filled for the main viewport
        var mainViewport = ImGui.GetMainViewport();
        SetupPlatformHandles(mainViewport, window);

        // From 2.0.5: Set SDL hint to receive mouse click events on window focus, otherwise SDL doesn't emit the event.
        // Without this, when clicking to gain focus, our widgets wouldn't activate even though they showed as hovered.
        // (This is unfortunately a global SDL setting, so enabling it might have a side-effect on your application.
        // It is unlikely to make a difference, but if your app absolutely needs to ignore the initial on-focus click:
        // you can ignore EventType.MOUSE_BUTTON_DOWN events coming right after a SDL_WINDOWEVENT_FOCUS_GAINED)
        Hints.MouseFocusClickthrough.SetValue("1");

        // From 2.0.22: Disable auto-capture, this is preventing drag and drop across multiple windows (see #5710)
        Hints.MouseAutoCapture.SetValue("0");
        
        // SDL 3.x : see https://github.com/libsdl-org/SDL/issues/6659
        SDL_SetHint("SDL_BORDERLESS_WINDOWED_STYLE", "0");

        // We need SDL_CaptureMouse(), SDL_GetGlobalMouseState() from SDL 2.0.4+ to support multiple viewports.
        if (io.BackendFlags.HasFlag(ImGuiBackendFlags.PlatformHasViewports))
            InitMultiViewportSupport(window, sdlGlСontext);
    }

    public static bool HasCaptureAndGlobalMouse => // no amiga in .NET
        !OperatingSystem.IsBrowser() && !OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS();

    public static bool IsApple => OperatingSystem.IsMacOS() | OperatingSystem.IsMacCatalyst() |
                                  OperatingSystem.IsIOS() | OperatingSystem.IsTvOS();

    public static void InitForOpenGL(Window window, IntPtr glContext) => 
        Init(window, null, glContext);
    public static void InitForSDLRenderer(Window window, Renderer renderer) =>
        Init(window, renderer);

    public static void InitForOther(Window window) => 
        Init(window);

    public static void Shutdown() {
        if (_backend == null)
            throw new InvalidOperationException("No platform backend to shutdown, or already shutdown?");
        
        var io = ImGui.GetIO();

        if (_backend.ClipboardTextData is not null)
            UnmanagedMemory.Free(_backend.ClipboardTextData);
        
        foreach (var pair in _backend.MouseCursors) {
            pair.Value.Dispose();
        }
        
        CloseGamepads();

        ((ImGuiIO*)io)->BackendPlatformName = null;
        io.BackendPlatformUserData = 0;
        io.BackendFlags &= ~(ImGuiBackendFlags.HasMouseCursors | ImGuiBackendFlags.HasSetMousePos | ImGuiBackendFlags.HasGamepad);
        _pin.Dispose();
    }

    static void UpdateMouseData() {
        var io = ImGui.GetIO();

        bool isAppFocused;
        Window? focusedWindow;
        // We forward mouse input when hovered or captured (via EventType.MOUSE_MOTION) or when focused (below)
        if (HasCaptureAndGlobalMouse) {
            // SDL_CaptureMouse() let the OS know e.g. that our imgui drag outside the SDL window boundaries shouldn't e.g. trigger other operations outside
            try {
                Mouse.Capture = _backend.MouseButtonsDown != 0;
            } catch (SdlException e) {}
            focusedWindow = Keyboard.GetFocusedWindow();
            isAppFocused = focusedWindow is not null && 
                            (_backend.Window == focusedWindow || GetViewportForWindowID(focusedWindow.Id).PlatformHandle != 0);
        }
        else {
            focusedWindow = _backend.Window;
            //TODO: Window.IsFocused
            isAppFocused = _backend.Window.Flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS);
            
            // SDL 2.0.3 and non-windowed systems: single-viewport only
        }

        if (!isAppFocused) return;
        
        // (Optional) Set OS mouse position from Dear ImGui if requested (rarely used, only when io.ConfigNavMoveSetMousePos is enabled by user)
        if (io.WantSetMousePos){
            if (HasCaptureAndGlobalMouse)
                if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
                    Mouse.Warp(io.MousePos);
                else
                    _backend.Window.WarpMouse(io.MousePos);
        }

        // (Optional) Fallback to provide mouse position when focused (EventType.MOUSE_MOTION already provides this when hovered or captured)
        if (_backend.MouseCanUseGlobalState && _backend.MouseButtonsDown == 0) {
            // Single-viewport mode: mouse position in client window coordinates (io.MousePos is (0,0) when the mouse is on the upper-left corner of the app window)
            // Multi-viewport mode: mouse position in OS absolute coordinates (io.MousePos is (0,0) when the mouse is on the upper-left of the primary monitor)
            var mousePos = Mouse.GlobalState.Position;
            if (!io.ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable)) {
                var windowPos = focusedWindow.Position;
                mousePos.X -= windowPos.X;
                mousePos.Y -= windowPos.Y;
            }
            io.AddMousePosEvent(mousePos.X, mousePos.Y);
        }
        // (Optional) When using multiple viewports: call io.AddMouseViewportEvent() with the viewport the OS mouse cursor is hovering.
        // If ImGuiBackendFlags_HasMouseHoveredViewport is not set by the backend, Dear imGui will ignore this field and infer the information using its flawed heuristic.
        // - [!] SDL backend does NOT correctly ignore viewports with the _NoInputs flag.
        //       Some backend are not able to handle that correctly. If a backend report an hovered viewport that has the _NoInputs flag (e.g. when dragging a window
        //       for docking, the viewport has the _NoInputs flag in order to allow us to find the viewport under), then Dear ImGui is forced to ignore the value reported
        //       by the backend, and use its flawed heuristic to guess the viewport behind.
        // - [X] SDL backend correctly reports this regardless of another viewport behind focused and dragged from (we need this to find a useful drag and drop target).
        if (io.BackendFlags.HasFlag(ImGuiBackendFlags.HasMouseHoveredViewport)) {
            uint mouseViewportId = 0;
            var mouseViewport = GetViewportForWindowID(_backend.MouseWindowID);
            if (mouseViewport.PlatformHandle != 0)
                mouseViewportId = mouseViewport.ID;
            io.AddMouseViewportEvent(mouseViewportId);
        }
    }

    static void UpdateMouseCursor() {
        var io = ImGui.GetIO();
        if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.NoMouseCursorChange))
            return;
        

        var imguiCursor = ImGui.GetMouseCursor();
        if (io.MouseDrawCursor || imguiCursor == ImGuiMouseCursor.None)
            // Hide OS mouse cursor if imgui is drawing it or if it wants no cursor
            Cursor.Hide();
        else {
            // Show OS mouse cursor
            if (!_backend.MouseCursors.TryGetValue(imguiCursor, out var expectedCursor))
                expectedCursor = _backend.MouseCursors[ImGuiMouseCursor.Arrow];
            if (_backend.MouseLastCursor != expectedCursor) {
                expectedCursor.SetActive(); // SDL function doesn't have an early out (see #6113)
                _backend.MouseLastCursor = expectedCursor;
            }
            Cursor.Show();
        }
    }

    static void CloseGamepads() {
        
        if (_backend.GamepadMode == GamepadMode.Manual) return;
        foreach (SDL_Gamepad* gamepad in _backend.Gamepads)
            SDL_CloseGamepad(gamepad);
    }

    public static void SetGamepadMode(GamepadMode mode, IEnumerable<IntPtr>? gamepads) {
        
        CloseGamepads();

        if (mode is GamepadMode.Manual) {
            if (gamepads is null)
                throw new Exception();
            _backend.Gamepads.AddRange(gamepads);
        }
        else 
            _backend.WantUpdateGamepadsList = true;
        _backend.GamepadMode = mode;
    }

    static void UpdateGamepadButton(Data bd, ImGuiIOPtr io, ImGuiKey key, GamepadButton buttonNo) {
        var mergedValue = false;
        foreach (SDL_Gamepad* gamepad in bd.Gamepads)
            mergedValue |= SDL_GetGamepadButton(gamepad, (SDL_GamepadButton)buttonNo);
        io.AddKeyEvent(key, mergedValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static float Saturate(float v) => v < 0.0f ? 0.0f : v  > 1.0f ? 1.0f : v;
    
    static void UpdateGamepadAnalog(Data bd, ImGuiIOPtr io, ImGuiKey key, GamepadAxis axisNo, float v0, float v1)
    {
        var mergedValue = 0.0f;
        foreach (SDL_Gamepad* gamepad in bd.Gamepads) {
            var vn = Saturate((SDL_GetGamepadAxis(gamepad, (SDL_GamepadAxis)axisNo) - v0) / (v1 - v0));
            if (mergedValue < vn)
                mergedValue = vn;
        }
        io.AddKeyAnalogEvent(key, mergedValue > 0.1f, mergedValue);
    }

    static void UpdateGamepads() {
        var io = ImGui.GetIO();
        

        // Update list of gamepads to use
        if (_backend.WantUpdateGamepadsList && _backend.GamepadMode != GamepadMode.Manual) {
            CloseGamepads();
            var sdlGamepadsCount = 0;
            var sdlGamepads = SDL_GetGamepads(&sdlGamepadsCount);
            for (int n = 0; n < sdlGamepadsCount; n++) {
                var gamepad = SDL_OpenGamepad(sdlGamepads[n]);
                if (gamepad is null) continue;
                _backend.Gamepads.Add((IntPtr)gamepad);
                if (_backend.GamepadMode == GamepadMode.AutoFirst)
                    break;
            }
            
            _backend.WantUpdateGamepadsList = false;
            UnmanagedMemory.Free(sdlGamepads);
        }

        // FIXME: Technically feeding gamepad shouldn't depend on this now that they are regular inputs.
        if ((io.ConfigFlags & ImGuiConfigFlags.NavEnableGamepad) == 0)
            return;
        io.BackendFlags &= ~ImGuiBackendFlags.HasGamepad;
        if (_backend.Gamepads.Count == 0)
            return;
        io.BackendFlags |= ImGuiBackendFlags.HasGamepad;

        // Update gamepad inputs
        const int thumbDeadZone = 8000;           // SDL_gamepad.h suggests using this value.
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadStart,       GamepadButton.Start);
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadBack,        GamepadButton.Back);
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadFaceLeft,    GamepadButton.West);           // Xbox X, PS Square
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadFaceRight,   GamepadButton.East);           // Xbox B, PS Circle
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadFaceUp,      GamepadButton.North);          // Xbox Y, PS Triangle
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadFaceDown,    GamepadButton.South);          // Xbox A, PS Cross
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadDpadLeft,    GamepadButton.DpadLeft);
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadDpadRight,   GamepadButton.DpadRight);
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadDpadUp,      GamepadButton.DpadUp);
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadDpadDown,    GamepadButton.DpadDown);
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadL1,          GamepadButton.LeftShoulder);
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadR1,          GamepadButton.RightShoulder);
        UpdateGamepadAnalog(_backend, io, ImGuiKey.GamepadL2,          GamepadAxis.LeftTrigger,  0.0f, 32767);
        UpdateGamepadAnalog(_backend, io, ImGuiKey.GamepadR2,          GamepadAxis.RightTrigger, 0.0f, 32767);
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadL3,          GamepadButton.LeftStick);
        UpdateGamepadButton(_backend, io, ImGuiKey.GamepadR3,          GamepadButton.RightStick);
        UpdateGamepadAnalog(_backend, io, ImGuiKey.GamepadLStickLeft,  GamepadAxis.Leftx,  -thumbDeadZone, -32768);
        UpdateGamepadAnalog(_backend, io, ImGuiKey.GamepadLStickRight, GamepadAxis.Leftx,  +thumbDeadZone, +32767);
        UpdateGamepadAnalog(_backend, io, ImGuiKey.GamepadLStickUp,    GamepadAxis.Lefty,  -thumbDeadZone, -32768);
        UpdateGamepadAnalog(_backend, io, ImGuiKey.GamepadLStickDown,  GamepadAxis.Lefty,  +thumbDeadZone, +32767);
        UpdateGamepadAnalog(_backend, io, ImGuiKey.GamepadRStickLeft,  GamepadAxis.Rightx, -thumbDeadZone, -32768);
        UpdateGamepadAnalog(_backend, io, ImGuiKey.GamepadRStickRight, GamepadAxis.Rightx, +thumbDeadZone, +32767);
        UpdateGamepadAnalog(_backend, io, ImGuiKey.GamepadRStickUp,    GamepadAxis.Righty, -thumbDeadZone, -32768);
        UpdateGamepadAnalog(_backend, io, ImGuiKey.GamepadRStickDown,  GamepadAxis.Righty, +thumbDeadZone, +32767);
    }
    
    static void UpdateMonitors() {
        
        ImGuiPlatformIO* platformIo = ImGui.GetPlatformIO();
        _backend.WantUpdateMonitors = false;
        
        var displays = Display.GetIds();
        var monitors = new List<ImGuiPlatformMonitor>();
        foreach (var displayId in displays) {
            // Warning: the validity of monitor DPI information on Windows depends on the application DPI awareness settings, which generally needs to be set in the manifest or at runtime.
            ImGuiPlatformMonitor monitor;
            var r = Display.GetBounds(displayId);
            monitor.MainPos = monitor.WorkPos = new Vector2(r.X, r.Y);
            monitor.MainSize = monitor.WorkSize = new Vector2(r.Width, r.Height);
            r = Display.GetUsableBounds(displayId);
            monitor.WorkPos = new Vector2(r.X, r.Y);
            monitor.WorkSize = new Vector2(r.Width, r.Height);
            // FIXME-VIEWPORT: On macOS SDL reports actual monitor DPI scale, ignoring OS configuration. We may want to set
            //  DpiScale to cocoa_window.backingScaleFactor here.
            monitor.DpiScale = Display.GetContentScale(displayId);
            monitor.PlatformHandle = (void*)displayId;
            if (monitor.DpiScale <= 0.0f)
                continue; // Some accessibility applications are declaring virtual monitors with a DPI of 0, see #7902.
            monitors.Add(monitor);
        }

        var monitorsArray = new Span<ImGuiPlatformMonitor>(monitors.ToArray());
        var monitorPtr = UnmanagedMemory.Calloc((nuint)monitors.Count, (nuint)Marshal.SizeOf<ImGuiPlatformMonitor>());
        var monitorPtrSpan = new Span<ImGuiPlatformMonitor>((void*)monitorPtr, monitorsArray.Length);
        monitorsArray.CopyTo(monitorPtrSpan);
        UnmanagedMemory.Free(platformIo->Monitors.Data);
        platformIo->Monitors = new ImVector(monitors.Count, monitors.Count, monitorPtr);
    }

    public static void NewFrame() {
        if (_backend == null)
            throw new InvalidOperationException(
                "Context or backend not initialized! Did you call Init()?");
        var io = ImGui.GetIO();

        // Setup display size (every frame to accommodate for window resizing)
        int displayW, displayH;
        var size = _backend.Window.Size;
        if (SDL_GetWindowFlags(_backend.Window).HasFlag(SDL_WindowFlags.SDL_WINDOW_MINIMIZED))
            size = new Size(0, 0);
        SDL_GetWindowSizeInPixels(_backend.Window, &displayW, &displayH);
        io.DisplaySize = new Vector2(size.Width, size.Height);
        if (size is { Width: > 0, Height: > 0 })
            io.DisplayFramebufferScale = new Vector2((float)displayW / size.Width, (float)displayH / size.Height);
        
        // Update monitors
        if (_backend.WantUpdateMonitors)
            UpdateMonitors();

        // Setup time step (we don't use SDL_GetTicks() because it is using millisecond resolution)
        // (Accept SDL_GetPerformanceCounter() not returning a monotonically increasing value. Happens in VMs and Emscripten, see #6189, #6114, #3644)
        var frequency = Timer.GetPerformanceFrequency();
        var currentTime = Timer.GetPerformanceCounter();
        if (currentTime <= _backend.Time)
            currentTime = _backend.Time + 1;
        io.DeltaTime = _backend.Time > 0 ? (float)((double)(currentTime - _backend.Time) / frequency) : (1.0f / 60.0f);
        _backend.Time = currentTime;

        if (_backend.MousePendingLeaveFrame != 0 && _backend.MousePendingLeaveFrame >= ImGui.GetFrameCount() && _backend.MouseButtonsDown == 0)
        {
            _backend.MouseWindowID = 0;
            _backend.MousePendingLeaveFrame = 0;
            io.AddMousePosEvent(-float.MinValue, -float.MaxValue);
        }
        
        // Our io.AddMouseViewportEvent() calls will only be valid when not capturing.
        // Technically speaking testing for 'bd->MouseButtonsDown == 0' would be more rigorous, but testing for payload reduces noise and potential side-effects.
        if (_backend.MouseCanReportHoveredViewport && ImGui.GetDragDropPayload().NativePtr is null)
            io.BackendFlags |= ImGuiBackendFlags.HasMouseHoveredViewport;
        else
            io.BackendFlags &= ~ImGuiBackendFlags.HasMouseHoveredViewport;

        UpdateMouseData();
        UpdateMouseCursor();

        // Update game controllers (if enabled and available)
        UpdateGamepads();
    }
    
    //--------------------------------------------------------------------------------------------------------
    // MULTI-VIEWPORT / PLATFORM INTERFACE SUPPORT
    // This is an _advanced_ and _optional_ feature, allowing the backend to create and handle multiple viewports simultaneously.
    // If you are new to dear imgui or creating a new binding for dear imgui, it is recommended that you completely ignore this section first..
    //--------------------------------------------------------------------------------------------------------

    // Helper structure we store in the void* RendererUserData field of each ImGuiViewport to easily retrieve our backend data.
    class ViewportData
    {
        public Window?     Window;
        public Window?     ParentWindow;
        public UInt32          WindowID;
        public bool            WindowOwned;
        public IntPtr GLContext;

        public ViewportData() {
            Window = ParentWindow = null; 
            WindowID = 0; 
            WindowOwned = false; 
            GLContext = 0;
        }

        //TODO: do we really need that?
        // ~ViewportData() {
        //     IM_ASSERT(Window == null && GLContext == null);
        // }
    }

    static Window GetSDLWindowFromViewportID(uint viewportId) {
        if (viewportId == 0) return null;
        var viewport = ImGui.FindViewportByID(viewportId);
        
        if (viewport.NativePtr is null) return null;
        
        var windowId = viewport.PlatformHandle;
        return Window.GetById((uint)windowId);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void CreateWindow(ImGuiViewport* viewport) {
        var vd = new ViewportData();
        var pin = vd.Pin(GCHandleType.Normal);
        viewport->PlatformUserData = (void*)pin.Pointer;

        vd.ParentWindow = GetSDLWindowFromViewportID(viewport->ParentViewportId);

        var mainViewport = ImGui.GetMainViewport();
        var mainViewportDataPin = new Pin<ViewportData>(mainViewport.PlatformUserData);
        if (!mainViewportDataPin.TryGetTarget(out var mainViewportData)) 
            throw new Exception("Failed to get viewport platform data");
        
        // Share GL resources with main context
        var useOpengl = mainViewportData.GLContext != 0;
        IntPtr backupContext = 0;
        if (useOpengl) {
            backupContext = Gl.CurrentContext;
            Gl.Attributes[GlAttr.ShareWithCurrentContext] = 1;
            mainViewportData.Window.MakeGlCurrent(mainViewportData.GLContext);
        }

        WindowFlags flags = 0;
        flags |= useOpengl ? WindowFlags.Opengl : (_backend.UseVulkan ? WindowFlags.Vulkan : 0);
        flags |= _backend.Window.Flags.HasFlag(WindowFlags.HighPixelDensity) ? WindowFlags.HighPixelDensity : 0;
        flags |= viewport->Flags.HasFlag(ImGuiViewportFlags.NoDecoration) ? WindowFlags.Borderless : 0;
        flags |= viewport->Flags.HasFlag(ImGuiViewportFlags.NoDecoration) ? 0 : WindowFlags.Resizable;
        flags |= viewport->Flags.HasFlag(ImGuiViewportFlags.NoTaskBarIcon) ? WindowFlags.Utility : 0;
        flags |= viewport->Flags.HasFlag(ImGuiViewportFlags.TopMost) ? WindowFlags.AlwaysOnTop : 0;
        vd.Window = new Window((int)viewport->Size.X, (int)viewport->Size.Y, "Untitled", flags);
        vd.Window.Parent = vd.ParentWindow; 
        vd.Window.Position = new Point((int)viewport->Pos.X, (int)viewport->Pos.Y);
        vd.WindowOwned = true;
        if (useOpengl) {
            vd.GLContext = Gl.CreateContext(vd.Window);
            Gl.SwapInterval = 0;
        }
        if (useOpengl && backupContext != 0)
            vd.Window.MakeGlCurrent(backupContext);

        SetupPlatformHandles(viewport, vd.Window);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void DestroyWindow(ImGuiViewport* viewport) {
        var vdPin = new Pin<ViewportData>(viewport->PlatformUserData, true);
        if (!vdPin.TryGetTarget(out var vd)) 
            throw new Exception("Failed to get viewport platform data");
        
        if (vd.GLContext != 0 && vd.WindowOwned)
            Gl.DestroyContext(vd.GLContext);
        if (vd.Window is not null && vd.WindowOwned)
            vd.Window.Dispose();
        vd.GLContext = 0;
        vd.Window = null;
        vdPin.Dispose();
        
        viewport->PlatformUserData = viewport->PlatformHandle = null;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void ShowWindow(ImGuiViewport* viewport) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        
        // if (OperatingSystem.IsWindows()) {
        //     var hwnd = viewport->PlatformHandleRaw;
        //     // SDL hack: Show icon in task bar (#7989)
        //     // Note: SDL_WINDOW_UTILITY can be used to control task bar visibility, but on Windows, it does not affect child windows.
        //     if (!viewport->Flags.HasFlag(ImGuiViewportFlags.NoTaskBarIcon)) {
        //         //  TODO:
        //         // LONG ex_style =  ::GetWindowLong(hwnd, GWL_EXSTYLE);
        //         // ex_style |= WS_EX_APPWINDOW;
        //         // ex_style &= ~WS_EX_TOOLWINDOW;
        //         // ::ShowWindow(hwnd, SW_HIDE);
        //         // ::SetWindowLong(hwnd, GWL_EXSTYLE, ex_style);
        //     }
        // }
        Hints.WindowActivateWhenShown.SetValue((viewport->Flags.HasFlag(ImGuiViewportFlags.NoFocusOnAppearing)) ? "0" : "1");
        vd.Window.Show();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void UpdateWindow(ImGuiViewport* viewport) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();

        // Update SDL3 parent if it changed _after_ creation.
        // This is for advanced apps that are manipulating ParentViewportID manually.
        var newParent = GetSDLWindowFromViewportID(viewport->ParentViewportId);
        if (newParent != vd.ParentWindow) {
            vd.ParentWindow = newParent;
            vd.Window.Parent = vd.ParentWindow;
        }
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static Vector2 GetWindowPos(ImGuiViewport* viewport) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();

        var p = vd.Window.Position;
        return new Vector2(p.X, p.Y);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetWindowPos(ImGuiViewport* viewport, Vector2 pos) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        
        vd.Window.Position = new Point((int)pos.X, (int)pos.Y);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static Vector2 GetWindowSize(ImGuiViewport* viewport) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        var s = vd.Window.Size;
        return new Vector2(s.Width, s.Height);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetWindowSize(ImGuiViewport* viewport, Vector2 size) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        
        vd.Window.Size = new Size((int)size.X, (int)size.Y);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetWindowTitle(ImGuiViewport* viewport, byte* title) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        
        SDL_SetWindowTitle(vd.Window, title); //i dont want to bother with pointers here
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetWindowAlpha(ImGuiViewport* viewport, float alpha) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();

        vd.Window.Opacity = alpha;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SetWindowFocus(ImGuiViewport* viewport) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        
        vd.Window.Raise();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static bool GetWindowFocus(ImGuiViewport* viewport) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        
        return vd.Window.Flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static bool GetWindowMinimized(ImGuiViewport* viewport) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        return vd.Window.Flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_MINIMIZED);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void RenderWindow(ImGuiViewport* viewport, void* unused) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        if (vd.GLContext != 0) return;
        
        vd.Window.MakeGlCurrent(vd.GLContext);
    }

    private static ViewportData GetViewportData(this ImGuiViewportPtr viewport) {
        var vdPin = new Pin<ViewportData>(viewport.PlatformUserData, true);
        if (!vdPin.TryGetTarget(out var vd)) 
            throw new Exception("Failed to get viewport platform data");
        return vd;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    static void SwapBuffers(ImGuiViewport* viewport, void* unused) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();

        if (vd.GLContext == 0) return;
        
        vd.Window.MakeGlCurrent(vd.GLContext);
        vd.Window.GlSwap();
    }

    // Vulkan support (the Vulkan renderer needs to call a platform-side support function to create the surface)
    // SDL is graceful enough to _not_ need <vulkan/vulkan.h> so we can safely include this.
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int CreateVkSurface(ImGuiViewport* viewport, IntPtr vk_instance, IntPtr vk_allocator, IntPtr* out_vk_surface)
    {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        
        bool ret = SDL_Vulkan_CreateSurface(vd.Window, (VkInstance_T*)vk_instance, (VkAllocationCallbacks*)vk_allocator, (VkSurfaceKHR_T**)out_vk_surface);
        return ret ? 0 : 1; // ret ? VK_SUCCESS : VK_NOT_READY
    }

    public static void InitMultiViewportSupport(Window window, IntPtr sdlGlContext) {
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
        delegate*unmanaged[Cdecl]<ImGuiViewport*, void> setWindowFocus = &SetWindowFocus;
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
            WindowID = (uint)SDL_GetWindowID(window),
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