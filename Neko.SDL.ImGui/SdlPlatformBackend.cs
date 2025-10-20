using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImGuiNET;
using Neko.Sdl.Events;
using Neko.Sdl.Extra.StandardLibrary;
using Neko.Sdl.Input;
using Neko.Sdl.Video;

namespace Neko.Sdl.ImGuiBackend;

// dear imgui: Platform Backend for SDL3 (*EXPERIMENTAL*)
// This needs to be used along with a Renderer (e.g. DirectX11, OpenGL3, Vulkan..)
// (Info: SDL3 is a cross-platform general purpose library for handling windows, inputs, graphics context creation, etc.)

// (**IMPORTANT: SDL 3.0.0 is NOT YET RELEASED AND CURRENTLY HAS A FAST CHANGING API. THIS CODE BREAKS OFTEN AS SDL3 CHANGES.**)

// Implemented features:
//  [X] Platform: Clipboard support.
//  [X] Platform: Mouse support. Can discriminate Mouse/TouchScreen.
//  [X] Platform: Keyboard support. Since 1.87 we are using the io.AddKeyEvent() function. Pass ImGuiKey values to all key functions e.g. ImGui::IsKeyPressed(ImGuiKey.Space). [Legacy SDL_SCANCODE_* values are obsolete since 1.87 and not supported since 1.91.5]
//  [X] Platform: Gamepad support.
//  [X] Platform: Mouse cursor shape and visibility (ImGuiBackendFlags.HasMouseCursors). Disable with 'io.ConfigFlags |= ImGuiConfigFlags_NoMouseCursorChange'.
//  [x] Platform: Multi-viewport support (multiple windows). Enable with 'io.ConfigFlags |= ImGuiConfigFlags_ViewportsEnable' . the OS animation effect when window gets created/destroyed is problematic. SDL2 backend doesn't have issue.
// Missing features or Issues:
//  [ ] Platform: Multi-viewport: Minimized windows seems to break mouse wheel events (at least under Windows).
//  [x] Platform: IME support. Position somehow broken in SDL3 + app needs to call 'SDL_SetHint(SDL_HINT_IME_SHOW_UI, "1");' before SDL_CreateWindow()!.
public unsafe class SdlPlatformBackend : IPlatformBackend, IViewportBackend {
    public Window Window;
    public Renderer? Renderer;
    public ulong Time;
    public string? ClipboardTextData;
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
    public bool MouseCanUseCapture;
    public bool MouseCanReportHoveredViewport; // This is hard to use/unreliable on SDL so we'll set ImGuiBackendFlags_HasMouseHoveredViewport dynamically based on state.

    // Gamepad handling
    public List<IntPtr> Gamepads = new();
    public ImGuiSdl.GamepadMode  GamepadMode;
    public bool WantUpdateGamepadsList;

    public SdlPlatformBackend(Window window, Renderer? renderer = null, IntPtr sdlGlСontext = 0) {
        var io = ImGui.GetIO();
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
        if (io.BackendPlatformUserData != 0)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            throw new InvalidOperationException("Already initialized a platform backend!");
        var verLinked = NekoSDL.Version;

        // Setup backend capabilities flags


        ((ImGuiIO*)io)->BackendPlatformName = (byte*)Marshal.StringToHGlobalAnsi("imgui_impl_nekosdl");
        
        io.BackendFlags |= ImGuiBackendFlags.HasMouseCursors;           // We can honor GetMouseCursor() values (optional)
        io.BackendFlags |= ImGuiBackendFlags.HasSetMousePos;            // We can honor io.WantSetMousePos requests (optional, rarely used)

        Window = window;
        Renderer = renderer;
        
        // SDL on Linux/OSX doesn't report events for unfocused windows (see https://github.com/ocornut/imgui/issues/4960)
        // We will use 'MouseCanReportHoveredViewport' to set 'ImGuiBackendFlags_HasMouseHoveredViewport' dynamically each frame.
        if (ImGuiSdl.IsApple)
            MouseCanReportHoveredViewport = MouseCanUseGlobalState;
        else
            MouseCanReportHoveredViewport = false;

        MouseCanUseGlobalState = false;
        MouseCanUseCapture = false;
        // Check and store if we are on a SDL backend that supports global mouse position
        // ("wayland" and "rpi" don't support it, but we chose to use a white-list instead of a black-list)
        if (ImGuiSdl.HasCaptureAndGlobalMouse) {
            var sdlBackend = VideoDriver.Current;
            if (sdlBackend is null) 
                throw new InvalidOperationException("Video driver have not been initialized yet!");
            string[] globalMouseWhitelist = [
                "windows", "cocoa", "x11", "DIVE", "VMAN"
            ];
            MouseCanUseGlobalState = MouseCanUseGlobalState = globalMouseWhitelist.Any(whitelist => sdlBackend == whitelist);
        }
        if (MouseCanUseGlobalState) {
            io.BackendFlags |= ImGuiBackendFlags.PlatformHasViewports; // We can create multi-viewports on the Platform side (optional)
        }
        
        // Update monitor a first time during init
        UpdateMonitors();

        // Gamepad handling
        GamepadMode = ImGuiSdl.GamepadMode.AutoFirst;
        WantUpdateGamepadsList = true;

        // Load mouse cursors
        MouseCursors[ImGuiMouseCursor.Arrow] = Cursor.CreateSystem(SystemCursor.Default);
        MouseCursors[ImGuiMouseCursor.TextInput] = Cursor.CreateSystem(SystemCursor.Text);
        MouseCursors[ImGuiMouseCursor.ResizeAll] = Cursor.CreateSystem(SystemCursor.Move);
        MouseCursors[ImGuiMouseCursor.ResizeNS] = Cursor.CreateSystem(SystemCursor.NsResize);
        MouseCursors[ImGuiMouseCursor.ResizeEW] = Cursor.CreateSystem(SystemCursor.EwResize);
        MouseCursors[ImGuiMouseCursor.ResizeNESW] = Cursor.CreateSystem(SystemCursor.NeswResize);
        MouseCursors[ImGuiMouseCursor.ResizeNWSE] = Cursor.CreateSystem(SystemCursor.NwseResize);
        MouseCursors[ImGuiMouseCursor.Hand] = Cursor.CreateSystem(SystemCursor.Pointer);
        // MouseCursors[ImGuiMouseCursor.Wait] = Cursor.CreateSystem(SystemCursor.Wait);
        // MouseCursors[ImGuiMouseCursor.Progress] = Cursor.CreateSystem(SystemCursor.Progress);
        MouseCursors[ImGuiMouseCursor.Hand] = Cursor.CreateSystem(SystemCursor.Pointer);
        MouseCursors[ImGuiMouseCursor.NotAllowed] = Cursor.CreateSystem(SystemCursor.NotAllowed);

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
    }
    
    public string? GetClipboardText() {
        return Clipboard.Text;
    }

    public void SetClipboardText(string? text) {
        Clipboard.Text = text;
    }

    public bool OpenInShell(string url) {
        NekoSDL.OpenUrl(url);
        return true;
    }

    public void SetImeData(ImGuiViewportPtr viewport, ImGuiPlatformImeDataPtr data) {
        var windowId = (uint)viewport.PlatformHandle;
        var window = Window.GetById(windowId);
        if (ImeWindow is not null && (data.WantVisible || ImeWindow != window)) {
            TextInput.Stop(ImeWindow);
            ImeWindow = null;
        }

        if (!data.WantVisible) return;

        Rectangle r = new() {
            X = (int)(data.InputPos.X - viewport.Pos.X),
            Y = (int)(data.InputPos.Y - viewport.Pos.Y + data.InputLineHeight),
            Width = 1,
            Height = (int)data.InputLineHeight,
        };
        TextInput.SetArea(window, r, 0);
        TextInput.Start(window);
        ImeWindow = window;
        if (SDL_TextInputActive(window) && data.WantVisible)
            SDL_StartTextInput(window);
    }
    
    private void UpdateGamepadButton(ImGuiIOPtr io, ImGuiKey key, GamepadButton buttonNo) {
        var mergedValue = false;
        foreach (SDL_Gamepad* gamepad in Gamepads)
            mergedValue |= SDL_GetGamepadButton(gamepad, (SDL_GamepadButton)buttonNo);
        io.AddKeyEvent(key, mergedValue);
    }
    
    public void SetGamepadMode(ImGuiSdl.GamepadMode mode, IEnumerable<IntPtr>? gamepads) {
        CloseGamepads();

        if (mode is ImGuiSdl.GamepadMode.Manual) {
            if (gamepads is null)
                throw new Exception();
            Gamepads.AddRange(gamepads);
        }
        else 
            WantUpdateGamepadsList = true;
        GamepadMode = mode;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Saturate(float v) => v < 0.0f ? 0.0f : v  > 1.0f ? 1.0f : v;
    
    private void UpdateGamepadAnalog(ImGuiIOPtr io, ImGuiKey key, GamepadAxis axisNo, float v0, float v1) {
        var mergedValue = 0.0f;
        foreach (SDL_Gamepad* gamepad in Gamepads) {
            var vn = Saturate((SDL_GetGamepadAxis(gamepad, (SDL_GamepadAxis)axisNo) - v0) / (v1 - v0));
            if (mergedValue < vn)
                mergedValue = vn;
        }
        io.AddKeyAnalogEvent(key, mergedValue > 0.1f, mergedValue);
    }

    private void UpdateGamepads() {
        var io = ImGui.GetIO();
        
        // Update list of gamepads to use
        if (WantUpdateGamepadsList && GamepadMode != ImGuiSdl.GamepadMode.Manual) {
            CloseGamepads();
            var sdlGamepadsCount = 0;
            var sdlGamepads = SDL_GetGamepads(&sdlGamepadsCount);
            for (int n = 0; n < sdlGamepadsCount; n++) {
                var gamepad = SDL_OpenGamepad(sdlGamepads[n]);
                if (gamepad is null) continue;
                Gamepads.Add((IntPtr)gamepad);
                if (GamepadMode == ImGuiSdl.GamepadMode.AutoFirst)
                    break;
            }
            
            WantUpdateGamepadsList = false;
            UnmanagedMemory.Free(sdlGamepads);
        }

        // FIXME: Technically feeding gamepad shouldn't depend on this now that they are regular inputs.
        if ((io.ConfigFlags & ImGuiConfigFlags.NavEnableGamepad) == 0)
            return;
        io.BackendFlags &= ~ImGuiBackendFlags.HasGamepad;
        if (Gamepads.Count == 0)
            return;
        io.BackendFlags |= ImGuiBackendFlags.HasGamepad;

        // Update gamepad inputs
        const int thumbDeadZone = 8000;           // SDL_gamepad.h suggests using this value.
        UpdateGamepadButton(io, ImGuiKey.GamepadStart,       GamepadButton.Start);
        UpdateGamepadButton(io, ImGuiKey.GamepadBack,        GamepadButton.Back);
        UpdateGamepadButton(io, ImGuiKey.GamepadFaceLeft,    GamepadButton.West);           // Xbox X, PS Square
        UpdateGamepadButton(io, ImGuiKey.GamepadFaceRight,   GamepadButton.East);           // Xbox B, PS Circle
        UpdateGamepadButton(io, ImGuiKey.GamepadFaceUp,      GamepadButton.North);          // Xbox Y, PS Triangle
        UpdateGamepadButton(io, ImGuiKey.GamepadFaceDown,    GamepadButton.South);          // Xbox A, PS Cross
        UpdateGamepadButton(io, ImGuiKey.GamepadDpadLeft,    GamepadButton.DpadLeft);
        UpdateGamepadButton(io, ImGuiKey.GamepadDpadRight,   GamepadButton.DpadRight);
        UpdateGamepadButton(io, ImGuiKey.GamepadDpadUp,      GamepadButton.DpadUp);
        UpdateGamepadButton(io, ImGuiKey.GamepadDpadDown,    GamepadButton.DpadDown);
        UpdateGamepadButton(io, ImGuiKey.GamepadL1,          GamepadButton.LeftShoulder);
        UpdateGamepadButton(io, ImGuiKey.GamepadR1,          GamepadButton.RightShoulder);
        UpdateGamepadAnalog(io, ImGuiKey.GamepadL2,          GamepadAxis.LeftTrigger,  0.0f, 32767);
        UpdateGamepadAnalog(io, ImGuiKey.GamepadR2,          GamepadAxis.RightTrigger, 0.0f, 32767);
        UpdateGamepadButton(io, ImGuiKey.GamepadL3,          GamepadButton.LeftStick);
        UpdateGamepadButton(io, ImGuiKey.GamepadR3,          GamepadButton.RightStick);
        UpdateGamepadAnalog(io, ImGuiKey.GamepadLStickLeft,  GamepadAxis.Leftx,  -thumbDeadZone, -32768);
        UpdateGamepadAnalog(io, ImGuiKey.GamepadLStickRight, GamepadAxis.Leftx,  +thumbDeadZone, +32767);
        UpdateGamepadAnalog(io, ImGuiKey.GamepadLStickUp,    GamepadAxis.Lefty,  -thumbDeadZone, -32768);
        UpdateGamepadAnalog(io, ImGuiKey.GamepadLStickDown,  GamepadAxis.Lefty,  +thumbDeadZone, +32767);
        UpdateGamepadAnalog(io, ImGuiKey.GamepadRStickLeft,  GamepadAxis.Rightx, -thumbDeadZone, -32768);
        UpdateGamepadAnalog(io, ImGuiKey.GamepadRStickRight, GamepadAxis.Rightx, +thumbDeadZone, +32767);
        UpdateGamepadAnalog(io, ImGuiKey.GamepadRStickUp,    GamepadAxis.Righty, -thumbDeadZone, -32768);
        UpdateGamepadAnalog(io, ImGuiKey.GamepadRStickDown,  GamepadAxis.Righty, +thumbDeadZone, +32767);
    }
    
    private void UpdateMonitors() {
        ImGuiPlatformIO* platformIo = ImGui.GetPlatformIO();
        WantUpdateMonitors = false;
        
        var displays = Display.GetIds();
        var monitors = new List<ImGuiPlatformMonitor>();
        foreach (var displayId in displays) {
            // Warning: the validity of monitor DPI information on Windows depends on the application DPI awareness settings, which generally needs to be set in the manifest or at runtime.
            ImGuiPlatformMonitor monitor;
            var r = Display.GetBounds(displayId);
            monitor.MainPos = monitor.WorkPos = new Vector2(r.X, r.Y);
            monitor.MainSize = monitor.WorkSize = new Vector2(r.Width, r.Height);
            r = Display.GetUsableBounds(displayId);
            if (r.Width > 0 && r.Height > 0) {
                monitor.WorkPos = new Vector2(r.X, r.Y);
                monitor.WorkSize = new Vector2(r.Width, r.Height);
            }
            monitor.DpiScale = Display.GetContentScale(displayId); // See https://wiki.libsdl.org/SDL3/README-highdpi for details.
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
    
    static void GetWindowSizeAndFramebufferScale(Window window, out Vector2 outSize, out Vector2 outFramebufferScale) {
        var size = window.Size;
        if (window.Flags.HasFlag(WindowFlags.Minimized))
            size.Width = size.Height = 0;

        float fb_scale_x;
        float fb_scale_y;
        if (ImGuiSdl.IsApple) {
            fb_scale_x = window.DisplayScale; // Seems more reliable during resolution change (#8703)
            fb_scale_y = fb_scale_x;
        }
        else {
            var displaySize = window.SizeInPixels;
            fb_scale_x = (size.Width > 0) ? (float)displaySize.Width / size.Width : 1.0f;
            fb_scale_y = (size.Height > 0) ? (float)displaySize.Height / size.Height : 1.0f;
        }
        
        outSize = new Vector2(size.Width, size.Height);
        outFramebufferScale = new Vector2(fb_scale_x, fb_scale_y);
    }
    
    private void CloseGamepads() {
        if (GamepadMode == ImGuiSdl.GamepadMode.Manual) return;
        foreach (SDL_Gamepad* gamepad in Gamepads)
            SDL_CloseGamepad(gamepad);
    }

    public void NewFrame() {
        var io = ImGui.GetIO();

        // Setup display size (every frame to accommodate for window resizing)
        // Setup main viewport size (every frame to accommodate for window resizing)
        GetWindowSizeAndFramebufferScale(Window, out io.DisplaySize, out io.DisplayFramebufferScale);
        var size = Window.Size;
        if (Window.Flags.HasFlag(WindowFlags.Minimized))
            size = new Size(0, 0);
        var display = Window.SizeInPixels;
        io.DisplaySize = new Vector2(size.Width, size.Height);
        if (size is { Width: > 0, Height: > 0 })
            io.DisplayFramebufferScale = new Vector2((float)display.Width / size.Width, (float)display.Height / size.Height);
        
        // Update monitors
        if (OperatingSystem.IsWindows())
            WantUpdateMonitors = true; // Keep polling under Windows to handle changes of work area when resizing task-bar (#8415)
        if (WantUpdateMonitors)
            UpdateMonitors();

        // Setup time step (we could also use SDL_GetTicksNS() available since SDL3)
        // (Accept SDL_GetPerformanceCounter() not returning a monotonically increasing value. Happens in VMs and Emscripten, see #6189, #6114, #3644)
        var frequency = Neko.Sdl.Time.Timer.GetPerformanceFrequency();
        var currentTime = Neko.Sdl.Time.Timer.GetPerformanceCounter();
        if (currentTime <= Time)
            currentTime = Time + 1;
        io.DeltaTime = Time > 0 ? (float)((double)(currentTime - Time) / frequency) : (1.0f / 60.0f);
        Time = currentTime;

        if (MousePendingLeaveFrame != 0 && MousePendingLeaveFrame >= ImGui.GetFrameCount() && MouseButtonsDown == 0)
        {
            MouseWindowID = 0;
            MousePendingLeaveFrame = 0;
            io.AddMousePosEvent(-float.MinValue, -float.MaxValue);
        }
        
        // Our io.AddMouseViewportEvent() calls will only be valid when not capturing.
        // Technically speaking testing for 'bd->MouseButtonsDown == 0' would be more rigorous, but testing for payload reduces noise and potential side-effects.
        if (MouseCanReportHoveredViewport && ImGui.GetDragDropPayload().NativePtr is null)
            io.BackendFlags |= ImGuiBackendFlags.HasMouseHoveredViewport;
        else
            io.BackendFlags &= ~ImGuiBackendFlags.HasMouseHoveredViewport;

        UpdateMouseData();
        UpdateMouseCursor();

        // Update game controllers (if enabled and available)
        UpdateGamepads();
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
            //case Keycode.Apostrophe: return ImGuiKey.Apostrophe;
            case Keycode.Comma: return ImGuiKey.Comma;
            //case Keycode.Minus: return ImGuiKey.Minus;
            case Keycode.Period: return ImGuiKey.Period;
            //case Keycode.Slash: return ImGuiKey.Slash;
            case Keycode.Semicolon: return ImGuiKey.Semicolon;
            //case Keycode.Equals: return ImGuiKey.Equal;
            //case Keycode.Leftbracket: return ImGuiKey.LeftBracket;
            //case Keycode.Backslash: return ImGuiKey.Backslash;
            //case Keycode.Rightbracket: return ImGuiKey.RightBracket;
            //case Keycode.Grave: return ImGuiKey.GraveAccent;
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
        // Fallback to scancode
        switch (scancode) {
            case Scancode.Grave: return ImGuiKey.GraveAccent;
            case Scancode.Minus: return ImGuiKey.Minus;
            case Scancode.Equals: return ImGuiKey.Equal;
            case Scancode.Leftbracket: return ImGuiKey.LeftBracket;
            case Scancode.Rightbracket: return ImGuiKey.RightBracket;
            //case Scancode.Nonusbackslash: return ImGuiKey.Oem102;
            case Scancode.Backslash: return ImGuiKey.Backslash;
            case Scancode.Semicolon: return ImGuiKey.Semicolon;
            case Scancode.Apostrophe: return ImGuiKey.Apostrophe;
            case Scancode.Comma: return ImGuiKey.Comma;
            case Scancode.Period: return ImGuiKey.Period;
            case Scancode.Slash: return ImGuiKey.Slash;
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
        
    public static ImGuiViewportPtr GetImGuiViewport(Window window) =>
        GetViewportForWindowID(window.Id);

    public bool ProcessEvent(ref Event ev) {
        var io = ImGui.GetIO();
        var type = ev.Type;
        var e = (SDL_Event*)Unsafe.AsPointer(ref ev); //TODO:
        ImGuiViewportPtr viewport;
        switch (type) {
            case EventType.MouseMotion:
                viewport = GetViewportForWindowID(ev.Motion.WindowId);
                if (viewport.NativePtr is null)
                    return false;
                var mousePos = new Vector2(ev.Motion.X, ev.Motion.Y);
                if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable)) {
                    var windowPos = ev.Motion.Window.Position;
                    mousePos = mousePos with { X = mousePos.X + windowPos.X, Y = mousePos.Y + windowPos.Y };
                }
                io.AddMouseSourceEvent(ev.Motion.Which == (uint)SDL_TOUCH_MOUSEID ? ImGuiMouseSource.TouchScreen : ImGuiMouseSource.Mouse);
                io.AddMousePosEvent(mousePos.X, mousePos.Y);
                return true;
            case EventType.MouseWheel:
                viewport = GetViewportForWindowID(ev.Wheel.WindowId);
                if (viewport.NativePtr is null)
                    return false;
                var wheelX = -ev.Wheel.X;
                var wheelY = ev.Wheel.Y;
                io.AddMouseSourceEvent(ev.Wheel.Which == (uint)SDL_TOUCH_MOUSEID ? ImGuiMouseSource.TouchScreen : ImGuiMouseSource.Mouse);
                io.AddMouseWheelEvent(wheelX, wheelY);
                return true;
            case EventType.MouseButtonDown:
            case EventType.MouseButtonUp:
                viewport = GetViewportForWindowID(ev.Button.WindowId);
                if (viewport.NativePtr is null)
                    return false;
                int mouseButton = -1;
                if (ev.Button.Button == MouseButtonFlags.Lmask) { mouseButton = 0; }
                if (ev.Button.Button == MouseButtonFlags.Rmask) { mouseButton = 1; }
                if (ev.Button.Button == MouseButtonFlags.Mmask) { mouseButton = 2; }
                if (ev.Button.Button == MouseButtonFlags.X1mask) { mouseButton = 3; }
                if (ev.Button.Button == MouseButtonFlags.X2mask) { mouseButton = 4; }
                if (mouseButton == -1)
                    break;
                io.AddMouseSourceEvent(ev.Button.Which == (uint)SDL_TOUCH_MOUSEID ? ImGuiMouseSource.TouchScreen : ImGuiMouseSource.Mouse);
                io.AddMouseButtonEvent(mouseButton, ev.Type == EventType.MouseButtonDown);
                MouseButtonsDown = (ev.Type == EventType.MouseButtonDown) ? (MouseButtonsDown | (1 << mouseButton)) : (MouseButtonsDown & ~(1 << mouseButton));
                return true;
            case EventType.TextInput:
                viewport = GetViewportForWindowID(ev.Text.WindowId);
                if (viewport.NativePtr is null)
                    return false;
                io.AddInputCharactersUTF8(ev.Text.Text);
                return true;
            case EventType.KeyDown:
            case EventType.KeyUp:
                viewport = GetViewportForWindowID(ev.Key.WindowId);
                if (viewport.NativePtr is null)
                    return false;
                UpdateKeyModifiers(ev.Key.Mod);
                var key = KeyEventToImGuiKey(ev.Key.Key, ev.Key.Scancode);
                io.AddKeyEvent(key, type is EventType.KeyDown);
                io.SetKeyEventNativeData(key, (int)ev.Key.Key, (int)ev.Key.Scancode, (int)ev.Key.Scancode); // To support legacy indexing (<1.87 user code). Legacy backend uses Keycode.*** as indices to IsKeyXXX() functions.
                return true;
            case EventType.DisplayOrientation:
            case EventType.DisplayAdded:
            case EventType.DisplayRemoved:
            case EventType.DisplayMoved:
            case EventType.DisplayContentScaleChanged:
                WantUpdateMonitors = true;
                return true;
            case EventType.WindowMouseEnter:
                MouseWindowID = ev.Window.WindowId;
                MousePendingLeaveFrame = 0;
                return true;
            // - In some cases, when detaching a window from main viewport SDL may send SDL_WINDOWEVENT_ENTER one frame too late,
            //   causing SDL_WINDOWEVENT_LEAVE on previous frame to interrupt drag operation by clear mouse position. This is why
            //   we delay process the SDL_WINDOWEVENT_LEAVE events by one frame. See issue #5012 for details.
            // FIXME: Unconfirmed whether this is still needed with SDL3.
            case EventType.WindowMouseLeave:
                MousePendingLeaveFrame = ImGui.GetFrameCount() + 1;
                return true;
            case EventType.WindowFocusGained:
            case EventType.WindowFocusLost:
                viewport = GetViewportForWindowID(ev.Text.WindowId);
                if (viewport.NativePtr is null)
                    return false;
                io.AddFocusEvent(type is EventType.WindowFocusGained);
                return true;
            case EventType.WindowCloseRequested:
                viewport = GetViewportForWindowID(ev.Window.WindowId);
                if (viewport.NativePtr is null)
                    return false;
                viewport.PlatformRequestClose = true;
                return true;
            case EventType.WindowMoved:
                viewport = GetViewportForWindowID(ev.Window.WindowId);
                if (viewport.NativePtr is null)
                    return false;
                viewport.PlatformRequestMove = true;
                return true;
            case EventType.WindowResized:
                viewport = GetViewportForWindowID(ev.Window.WindowId);
                if (viewport.NativePtr is null)
                    return false;
                viewport.PlatformRequestResize = true;
                return true;
            case EventType.GamepadAdded:
            case EventType.GamepadRemoved:
                WantUpdateGamepadsList = true;
                return true;
        }
        return false;
    }
    
    private void UpdateMouseData() {
        var io = ImGui.GetIO();

        bool isAppFocused;
        Window? focusedWindow;
        // We forward mouse input when hovered or captured (via EventType.MOUSE_MOTION) or when focused (below)
        if (ImGuiSdl.HasCaptureAndGlobalMouse) {
            // - SDL_CaptureMouse() let the OS know e.g. that our drags can extend outside of parent boundaries (we want updated position) and shouldn't trigger other operations outside.
            // - Debuggers under Linux tends to leave captured mouse on break, which may be very inconvenient, so to mitigate the issue we wait until mouse has moved to begin capture.
            if (MouseCanUseCapture) {
                for (var i = 0; i < (int)ImGuiMouseButton.COUNT; i++)
                    if (ImGui.IsMouseDragging((ImGuiMouseButton)i, 1.0f)) {
                        Mouse.Capture = true;
                        break;
                    }
            }
            focusedWindow = Keyboard.GetFocusedWindow();
            isAppFocused = focusedWindow is not null && 
                            (Window == focusedWindow || GetViewportForWindowID(focusedWindow.Id).PlatformHandle != 0);
        }
        else {
            focusedWindow = Window;
            //TODO: Window.IsFocused
            isAppFocused = Window.Flags.HasFlag(WindowFlags.InputFocus);
            
            // SDL 2.0.3 and non-windowed systems: single-viewport only
        }

        if (!isAppFocused) return;
        
        // (Optional) Set OS mouse position from Dear ImGui if requested (rarely used, only when io.ConfigNavMoveSetMousePos is enabled by user)
        if (io.WantSetMousePos){
            if (ImGuiSdl.HasCaptureAndGlobalMouse)
                if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.ViewportsEnable))
                    Mouse.Warp(io.MousePos);
                else
                    Window.WarpMouse(io.MousePos);
        }

        // (Optional) Fallback to provide unclamped mouse position when focused but not hovered (SDL_EVENT_MOUSE_MOTION already provides this when hovered or captured)
        // Note that SDL_GetGlobalMouseState() is in theory slow on X11, but this only runs on rather specific cases. If a problem we may provide a way to opt-out this feature.
        var hoveredWindow = Mouse.Focus;
        var relativeMouseMode = hoveredWindow?.RelativeMouseMode??false;
        if (hoveredWindow is not null && MouseCanUseGlobalState && MouseButtonsDown == 0 && !relativeMouseMode) {
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
            var mouseViewport = GetViewportForWindowID(MouseWindowID);
            if (mouseViewport.PlatformHandle != 0)
                mouseViewportId = mouseViewport.ID;
            io.AddMouseViewportEvent(mouseViewportId);
        }
    }

    private void UpdateMouseCursor() {
        var io = ImGui.GetIO();
        if (io.ConfigFlags.HasFlag(ImGuiConfigFlags.NoMouseCursorChange))
            return;
        

        var imguiCursor = ImGui.GetMouseCursor();
        if (io.MouseDrawCursor || imguiCursor == ImGuiMouseCursor.None)
            // Hide OS mouse cursor if imgui is drawing it or if it wants no cursor
            Cursor.Hide();
        else {
            // Show OS mouse cursor
            if (!MouseCursors.TryGetValue(imguiCursor, out var expectedCursor))
                expectedCursor = MouseCursors[ImGuiMouseCursor.Arrow];
            if (MouseLastCursor != expectedCursor) {
                expectedCursor.SetActive(); // SDL function doesn't have an early out (see #6113)
                MouseLastCursor = expectedCursor;
            }
            Cursor.Show();
        }
    }
    
    //--------------------------------------------------------------------------------------------------------
    // MULTI-VIEWPORT / PLATFORM INTERFACE SUPPORT
    // This is an _advanced_ and _optional_ feature, allowing the backend to create and handle multiple viewports simultaneously.
    // If you are new to dear imgui or creating a new binding for dear imgui, it is recommended that you completely ignore this section first..
    //--------------------------------------------------------------------------------------------------------

    public void CreateWindow(ImGuiViewportPtr viewport) {
        var vd = new ViewportData();
        var pin = vd.Pin(GCHandleType.Normal);
        viewport.PlatformUserData = pin.Pointer;

        vd.ParentWindow = GetSDLWindowFromViewportID(viewport.ParentViewportId);

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
        flags |= useOpengl ? WindowFlags.Opengl : (UseVulkan ? WindowFlags.Vulkan : 0);
        flags |= Window.Flags.HasFlag(WindowFlags.HighPixelDensity) ? WindowFlags.HighPixelDensity : 0;
        flags |= viewport.Flags.HasFlag(ImGuiViewportFlags.NoDecoration) ? WindowFlags.Borderless : 0;
        flags |= viewport.Flags.HasFlag(ImGuiViewportFlags.NoDecoration) ? 0 : WindowFlags.Resizable;
        flags |= viewport.Flags.HasFlag(ImGuiViewportFlags.NoTaskBarIcon) ? WindowFlags.Utility : 0;
        flags |= viewport.Flags.HasFlag(ImGuiViewportFlags.TopMost) ? WindowFlags.AlwaysOnTop : 0;
        vd.Window = Window.Create((int)viewport.Size.X, (int)viewport.Size.Y, "Untitled", flags);
        if (!ImGuiSdl.IsApple) // On Mac, SDL3 Parenting appears to prevent viewport from appearing in another monitor
            vd.Window.Parent = vd.ParentWindow; 
        vd.Window.Position = new Point((int)viewport.Pos.X, (int)viewport.Pos.Y);
        vd.WindowOwned = true;
        if (useOpengl) {
            vd.GLContext = Gl.CreateContext(vd.Window);
            Gl.SwapInterval = 0;
        }
        if (useOpengl && backupContext != 0)
            vd.Window.MakeGlCurrent(backupContext);

        SetupPlatformHandles(viewport, vd.Window);
    }

    public void DestroyWindow(ImGuiViewportPtr viewport) {
        var vdPin = new Pin<ViewportData>(viewport.PlatformUserData, true);
        if (!vdPin.TryGetTarget(out var vd)) 
            throw new Exception("Failed to get viewport platform data");
        
        if (vd.GLContext != 0 && vd.WindowOwned)
            Gl.DestroyContext(vd.GLContext);
        if (vd.Window is not null && vd.WindowOwned)
            vd.Window.Dispose();
        vd.GLContext = 0;
        vd.Window = null;
        vdPin.Dispose();
        
        viewport.PlatformUserData = viewport.PlatformHandle = 0;
    }

    public void ShowWindow(ImGuiViewportPtr viewport) {
        var vd = ((ImGuiViewportPtr)viewport).GetViewportData();
        
        // if (OperatingSystem.IsWindows()) {
        //     var hwnd = viewport.PlatformHandleRaw;
        //     // SDL hack: Show icon in task bar (#7989)
        //     // Note: SDL_WINDOW_UTILITY can be used to control task bar visibility, but on Windows, it does not affect child windows.
        //     if (!viewport.Flags.HasFlag(ImGuiViewportFlags.NoTaskBarIcon)) {
        //         //  TODO:
        //         // LONG ex_style =  ::GetWindowLong(hwnd, GWL_EXSTYLE);
        //         // ex_style |= WS_EX_APPWINDOW;
        //         // ex_style &= ~WS_EX_TOOLWINDOW;
        //         // ::ShowWindow(hwnd, SW_HIDE);
        //         // ::SetWindowLong(hwnd, GWL_EXSTYLE, ex_style);
        //     }
        // }
        if (ImGuiSdl.IsApple)
            Hints.WindowActivateWhenShown.SetValue("1");
        else
            Hints.WindowActivateWhenShown.SetValue((viewport.Flags.HasFlag(ImGuiViewportFlags.NoFocusOnAppearing)) ? "0" : "1");
        vd.Window?.Show();
    }

    public void UpdateWindow(ImGuiViewportPtr viewport) {
        var vd = viewport.GetViewportData();

        if (!ImGuiSdl.IsApple) { // On Mac, SDL3 Parenting appears to prevent viewport from appearing in another monitor
            // Update SDL3 parent if it changed _after_ creation.
            // This is for advanced apps that are manipulating ParentViewportID manually.
            var newParent = GetSDLWindowFromViewportID(viewport.ParentViewportId);
            if (vd.ParentWindow is null) return;
            if (newParent.Handle != vd.ParentWindow.Handle) {
                vd.ParentWindow = newParent;
                vd.Window.Parent = vd.ParentWindow;
            }
        }
    }

    public void SetWindowPos(ImGuiViewportPtr viewport, Vector2 pos) {
        var vd = viewport.GetViewportData();
        
        vd.Window.Position = new Point((int)pos.X, (int)pos.Y);
    }

    public Vector2 GetWindowPos(ImGuiViewportPtr viewport) {
        var vd = viewport.GetViewportData();

        var p = vd.Window.Position;
        return new Vector2(p.X, p.Y);
    }

    public void SetWindowSize(ImGuiViewportPtr viewport, Vector2 size) {
        var vd = viewport.GetViewportData();
        
        vd.Window.Size = new Size((int)size.X, (int)size.Y);
    }

    public Vector2 GetWindowSize(ImGuiViewportPtr viewport) {
        var vd = viewport.GetViewportData();
        var s = vd.Window.Size;
        return new Vector2(s.Width, s.Height);
    }

    public void SetWindowFocus(ImGuiViewportPtr viewport, bool focus) {
        var vd = viewport.GetViewportData();
        
        vd.Window.Raise();
    }

    public bool GetWindowFocus(ImGuiViewportPtr viewport) {
        var vd = viewport.GetViewportData();
        
        return vd.Window.Flags.HasFlag(WindowFlags.InputFocus);
    }

    public bool GetWindowMinimized(ImGuiViewportPtr viewport) {
        var vd = viewport.GetViewportData();
        return vd.Window.Flags.HasFlag(WindowFlags.Minimized);
    }

    public void SetWindowTitle(ImGuiViewportPtr viewport, string title) {
        var vd = viewport.GetViewportData();
        
        SDL_SetWindowTitle(vd.Window, title); //i dont want to bother with pointers here
    }

    public void SetWindowAlpha(ImGuiViewportPtr viewport, float alpha) {
        var vd = viewport.GetViewportData();

        vd.Window.Opacity = alpha;
    }

    public void RenderWindow(ImGuiViewportPtr viewport, IntPtr unused) {
        var vd = viewport.GetViewportData();
        if (vd.GLContext != 0) return;
        
        vd.Window.MakeGlCurrent(vd.GLContext);
    }

    public void SwapBuffers(ImGuiViewportPtr viewport, IntPtr unused) {
        var vd = viewport.GetViewportData();

        if (vd.GLContext == 0) return;
        
        vd.Window.MakeGlCurrent(vd.GLContext);
        vd.Window.GlSwap();
    }

    [SkipImplementation]
    public float GetWindowDpiScale(ImGuiViewportPtr viewport) {
        throw new NotImplementedException();
    }
    
    [SkipImplementation]
    public void OnChangedViewport(ImGuiViewportPtr viewport) {
        throw new NotImplementedException();
    }

    [SkipImplementation]
    public Vector2 GetWindowWorkAreaInsets(ImGuiViewportPtr viewport) {
        throw new NotImplementedException();
    }

    public int CreateVkSurface(ImGuiViewportPtr viewport, IntPtr vkInstance, IntPtr vkAllocator, ref IntPtr outVkSurface) {
        var vd = viewport.GetViewportData();
        
        bool ret = SDL_Vulkan_CreateSurface(vd.Window, (VkInstance_T*)vkInstance, (VkAllocationCallbacks*)vkAllocator, (VkSurfaceKHR_T**)outVkSurface);
        return ret ? 0 : 1; // ret ? VK_SUCCESS : VK_NOT_READY
    }
    
    static Window GetSDLWindowFromViewportID(uint viewportId) {
        if (viewportId == 0) return null;
        var viewport = ImGui.FindViewportByID(viewportId);
        
        if (viewport.NativePtr is null) return null;
        
        var windowId = viewport.PlatformHandle;
        return Window.GetById((uint)windowId);
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

    public void Dispose() {
        var io = ImGui.GetIO();

        // if (ClipboardTextData is not null)
        //     UnmanagedMemory.Free(ClipboardTextData);

        foreach (var pair in MouseCursors) {
            pair.Value.Dispose();
        }

        CloseGamepads();

        ((ImGuiIO*)io)->BackendPlatformName = null;
        io.BackendPlatformUserData = 0;
        io.BackendFlags &= ~(ImGuiBackendFlags.HasMouseCursors
                             | ImGuiBackendFlags.HasSetMousePos
                             | ImGuiBackendFlags.HasGamepad
                             | ImGuiBackendFlags.PlatformHasViewports
                             | ImGuiBackendFlags.HasMouseHoveredViewport);
    }
}