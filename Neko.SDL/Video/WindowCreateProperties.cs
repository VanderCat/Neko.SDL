namespace Neko.Sdl.Video;

public class WindowCreateProperties : Properties {
    /// <summary>
    /// Should the window should be always on top?
    /// </summary>
    public bool? AlwaysOnTop {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_ALWAYS_ON_TOP_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_ALWAYS_ON_TOP_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_ALWAYS_ON_TOP_BOOLEAN, (bool)value);
        }
    }
    
    /// <summary>
    /// Should the window have no decoration?
    /// </summary>
    public bool? Borderless {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_BORDERLESS_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_BORDERLESS_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_BORDERLESS_BOOLEAN, (bool)value);
        }
    }
    /// <summary>
    /// true if the "tooltip" and "menu" window types should be automatically constrained to be entirely within display
    /// bounds (default), false if no constraints on the position are desired.
    /// </summary>
    public bool? ConstrainPopup {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_CONSTRAIN_POPUP_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_CONSTRAIN_POPUP_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_CONSTRAIN_POPUP_BOOLEAN, (bool)value);
        }
    }
    
    /// <summary>
    /// Will window be used with an externally managed graphics context?
    /// </summary>
    public bool? ExternalGraphicsContext {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_EXTERNAL_GRAPHICS_CONTEXT_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_EXTERNAL_GRAPHICS_CONTEXT_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_EXTERNAL_GRAPHICS_CONTEXT_BOOLEAN, (bool)value);
        }
    }
    
    /// <summary>
    /// Should the window should accept keyboard input? (defaults true)
    /// </summary>
    public bool? Focusable {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_FOCUSABLE_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_FOCUSABLE_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_FOCUSABLE_BOOLEAN, (bool)value);
        }
    }
    
    /// <summary>
    /// Should the window should start in fullscreen mode at desktop resolution?
    /// </summary>
    public bool? Fullscreen {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_FULLSCREEN_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_FULLSCREEN_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_FULLSCREEN_BOOLEAN, (bool)value);
        }
    }
    
    /// <summary>
    /// the height of the window
    /// </summary>
    public long Height {
        get => GetNumber(SDL_PROP_WINDOW_CREATE_HEIGHT_NUMBER, 0);
        set => SetNumber(SDL_PROP_WINDOW_CREATE_HEIGHT_NUMBER, value); // fixed: was FULLSCREEN_BOOLEAN
    }

    /// <summary>
    /// Should the window should start hidden?
    /// </summary>
    public bool? Hidden {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_HIDDEN_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_HIDDEN_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_HIDDEN_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Should the window use a high pixel density buffer if possible?
    /// </summary>
    public bool? HighPixelDensity {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_HIGH_PIXEL_DENSITY_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_HIGH_PIXEL_DENSITY_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_HIGH_PIXEL_DENSITY_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Should the window should start maximized?
    /// </summary>
    public bool? Maximized {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_MAXIMIZED_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_MAXIMIZED_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_MAXIMIZED_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Is the window a popup menu?
    /// </summary>
    public bool? Menu {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_MENU_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_MENU_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_MENU_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Will the window be used with Metal rendering?
    /// </summary>
    public bool? Metal {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_METAL_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_METAL_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_METAL_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Should the window should start minimized?
    /// </summary>
    public bool? Minimized {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_MINIMIZED_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_MINIMIZED_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_MINIMIZED_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Is the window modal to its parent?
    /// </summary>
    public bool? Modal {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_MODAL_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_MODAL_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_MODAL_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Does the window start with grabbed mouse focus?
    /// </summary>
    public bool? MouseGrabbed {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_MOUSE_GRABBED_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_MOUSE_GRABBED_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_MOUSE_GRABBED_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Will the window be used with OpenGL rendering?
    /// </summary>
    public bool? OpenGL {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_OPENGL_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_OPENGL_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_OPENGL_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// A window that will be the parent of this window, required for windows with the
    /// "tooltip", "menu", and "modal" properties.
    /// </summary>
    public unsafe Window? Parent {
        get {
            var ptr = GetPointer(SDL_PROP_WINDOW_CREATE_PARENT_POINTER, IntPtr.Zero);
            if (ptr == 0) return null;
            return Window.GetById(SDL_GetWindowID((SDL_Window*)ptr));
        }
        set {
            if (value == null)
                Clear(SDL_PROP_WINDOW_CREATE_PARENT_POINTER);
            else
                SetPointer(SDL_PROP_WINDOW_CREATE_PARENT_POINTER, (IntPtr)value.Handle);
        }
    }

    /// <summary>
    /// Should the window be resizable?
    /// </summary>
    public bool? Resizable {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_RESIZABLE_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_RESIZABLE_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_RESIZABLE_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// The title of the window, in UTF-8 encoding.
    /// </summary>
    public string? Title {
        get => GetString(SDL_PROP_WINDOW_CREATE_TITLE_STRING);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_TITLE_STRING);
            else
                SetString(SDL_PROP_WINDOW_CREATE_TITLE_STRING, value);
        }
    }

    /// <summary>
    /// Should the window show transparent in the areas with alpha of 0?
    /// </summary>
    public bool? Transparent {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_TRANSPARENT_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_TRANSPARENT_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_TRANSPARENT_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Is the window a tooltip?
    /// </summary>
    public bool? Tooltip {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_TOOLTIP_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_TOOLTIP_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_TOOLTIP_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Is the window a utility window, not showing in the task bar and window list?
    /// </summary>
    public bool? Utility {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_UTILITY_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_UTILITY_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_UTILITY_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// Will the window be used with Vulkan rendering?
    /// </summary>
    public bool? Vulkan {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_VULKAN_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_VULKAN_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_VULKAN_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// the width of the window
    /// </summary>
    public long Width {
        get => GetNumber(SDL_PROP_WINDOW_CREATE_WIDTH_NUMBER, 0);
        set => SetNumber(SDL_PROP_WINDOW_CREATE_WIDTH_NUMBER, value);
    }

    /// <summary>
    /// The x position of the window, or <c>SDL_WINDOWPOS_CENTERED</c>, defaults to <c>SDL_WINDOWPOS_UNDEFINED</c>.
    /// This is relative to the parent for windows with the "tooltip" or "menu" property set.
    /// </summary>
    public long X {
        get => GetNumber(SDL_PROP_WINDOW_CREATE_X_NUMBER, unchecked((long)SDL_WINDOWPOS_UNDEFINED));
        set => SetNumber(SDL_PROP_WINDOW_CREATE_X_NUMBER, value);
    }

    /// <summary>
    /// The y position of the window, or <c>SDL_WINDOWPOS_CENTERED</c>, defaults to <c>SDL_WINDOWPOS_UNDEFINED</c>.
    /// This is relative to the parent for windows with the "tooltip" or "menu" property set.
    /// </summary>
    public long Y {
        get => GetNumber(SDL_PROP_WINDOW_CREATE_Y_NUMBER, unchecked((long)SDL_WINDOWPOS_UNDEFINED));
        set => SetNumber(SDL_PROP_WINDOW_CREATE_Y_NUMBER, value);
    }

    #region Platform-specific properties

    // macOS
    public IntPtr CocoaWindow {
        get => GetPointer(SDL_PROP_WINDOW_CREATE_COCOA_WINDOW_POINTER, IntPtr.Zero);
        set {
            if (value == IntPtr.Zero)
                Clear(SDL_PROP_WINDOW_CREATE_COCOA_WINDOW_POINTER);
            else
                SetPointer(SDL_PROP_WINDOW_CREATE_COCOA_WINDOW_POINTER, value);
        }
    }

    public IntPtr CocoaView {
        get => GetPointer(SDL_PROP_WINDOW_CREATE_COCOA_VIEW_POINTER, IntPtr.Zero);
        set {
            if (value == IntPtr.Zero)
                Clear(SDL_PROP_WINDOW_CREATE_COCOA_VIEW_POINTER);
            else
                SetPointer(SDL_PROP_WINDOW_CREATE_COCOA_VIEW_POINTER, value);
        }
    }

    // iOS / tvOS / visionOS
    public IntPtr WindowScene {
        get => GetPointer(SDL_PROP_WINDOW_CREATE_WINDOWSCENE_POINTER, IntPtr.Zero);
        set {
            if (value == IntPtr.Zero)
                Clear(SDL_PROP_WINDOW_CREATE_WINDOWSCENE_POINTER);
            else
                SetPointer(SDL_PROP_WINDOW_CREATE_WINDOWSCENE_POINTER, value);
        }
    }

    // Wayland
    public bool? WaylandSurfaceRoleCustom {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_WAYLAND_SURFACE_ROLE_CUSTOM_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_WAYLAND_SURFACE_ROLE_CUSTOM_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_WAYLAND_SURFACE_ROLE_CUSTOM_BOOLEAN, (bool)value);
        }
    }

    public bool? WaylandCreateEglWindow {
        get => GetBoolean(SDL_PROP_WINDOW_CREATE_WAYLAND_CREATE_EGL_WINDOW_BOOLEAN);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_WAYLAND_CREATE_EGL_WINDOW_BOOLEAN);
            else
                SetBoolean(SDL_PROP_WINDOW_CREATE_WAYLAND_CREATE_EGL_WINDOW_BOOLEAN, (bool)value);
        }
    }

    // TODO: 3.6.0
    // public string? WaylandWindowId {
    //     get => GetString(SDL_PROP_WINDOW_CREATE_WAYLAND_WINDOW_ID_STRING);
    //     set {
    //         if (value is null)
    //             Clear(SDL_PROP_WINDOW_CREATE_WAYLAND_WINDOW_ID_STRING);
    //         else
    //             SetString(SDL_PROP_WINDOW_CREATE_WAYLAND_WINDOW_ID_STRING, value);
    //     }
    // }

    public IntPtr WaylandSurface {
        get => GetPointer(SDL_PROP_WINDOW_CREATE_WAYLAND_WL_SURFACE_POINTER, IntPtr.Zero);
        set {
            if (value == IntPtr.Zero)
                Clear(SDL_PROP_WINDOW_CREATE_WAYLAND_WL_SURFACE_POINTER);
            else
                SetPointer(SDL_PROP_WINDOW_CREATE_WAYLAND_WL_SURFACE_POINTER, value);
        }
    }

    // Windows
    public IntPtr Win32Hwnd {
        get => GetPointer(SDL_PROP_WINDOW_CREATE_WIN32_HWND_POINTER, IntPtr.Zero);
        set {
            if (value == IntPtr.Zero)
                Clear(SDL_PROP_WINDOW_CREATE_WIN32_HWND_POINTER);
            else
                SetPointer(SDL_PROP_WINDOW_CREATE_WIN32_HWND_POINTER, value);
        }
    }

    public IntPtr Win32PixelFormatHwnd {
        get => GetPointer(SDL_PROP_WINDOW_CREATE_WIN32_PIXEL_FORMAT_HWND_POINTER, IntPtr.Zero);
        set {
            if (value == IntPtr.Zero)
                Clear(SDL_PROP_WINDOW_CREATE_WIN32_PIXEL_FORMAT_HWND_POINTER);
            else
                SetPointer(SDL_PROP_WINDOW_CREATE_WIN32_PIXEL_FORMAT_HWND_POINTER, value);
        }
    }

    // X11
    public long X11WindowNumber {
        get => GetNumber(SDL_PROP_WINDOW_CREATE_X11_WINDOW_NUMBER, 0);
        set => SetNumber(SDL_PROP_WINDOW_CREATE_X11_WINDOW_NUMBER, value);
    }

    // Emscripten
    public string? EmscriptenCanvasId {
        get => GetString(SDL_PROP_WINDOW_CREATE_EMSCRIPTEN_CANVAS_ID_STRING);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_EMSCRIPTEN_CANVAS_ID_STRING);
            else
                SetString(SDL_PROP_WINDOW_CREATE_EMSCRIPTEN_CANVAS_ID_STRING, value);
        }
    }

    public string? EmscriptenKeyboardElement {
        get => GetString(SDL_PROP_WINDOW_CREATE_EMSCRIPTEN_KEYBOARD_ELEMENT_STRING);
        set {
            if (value is null)
                Clear(SDL_PROP_WINDOW_CREATE_EMSCRIPTEN_KEYBOARD_ELEMENT_STRING);
            else
                SetString(SDL_PROP_WINDOW_CREATE_EMSCRIPTEN_KEYBOARD_ELEMENT_STRING, value);
        }
    }

    // visionOS
    // TODO: 3.6.0
    // public string? VisionOsSettings {
    //     get => GetString(SDL_PROP_WINDOW_CREATE_VISIONOS_SETTINGS_STRING);
    //     set {
    //         if (value is null)
    //             Clear(SDL_PROP_WINDOW_CREATE_VISIONOS_SETTINGS_STRING);
    //         else
    //             SetString(SDL_PROP_WINDOW_CREATE_VISIONOS_SETTINGS_STRING, value);
    //     }
    // }

    #endregion

    #region Internal

    internal long? Flags {
        get {
            var result = GetNumber(SDL_PROP_WINDOW_CREATE_FLAGS_NUMBER, long.MinValue);
            if (result == long.MinValue) return null;
            return result;
        }
        set {
            if (value is null or long.MinValue) Clear(SDL_PROP_WINDOW_CREATE_FLAGS_NUMBER);
            else SetNumber(SDL_PROP_WINDOW_CREATE_FLAGS_NUMBER, (long)value);
        }
    }

    #endregion
}