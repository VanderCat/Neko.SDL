using System;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using Neko.Sdl;
using Neko.Sdl.Events;
using Neko.Sdl.Threading;
using Neko.Sdl.Video;
using SDL;
using Window = Neko.Sdl.Video.Window;

namespace Neko.SDL.AvaloniaApp;

public unsafe class SdlOpenGlControl : NativeControlHost {
    private IntPtr _context;
    private Window _window;
    private bool _closing;
    private readonly DispatcherTimer _timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60) };
    public IntPtr Handle { get; private set; }
    private static class Fn {
        public static delegate*unmanaged[Cdecl]<int, int, int, int, void> Viewport;
        public static delegate*unmanaged[Cdecl]<float, float, float, float, void> ClearColor;
        public static delegate*unmanaged[Cdecl]<int, void> Clear;
    }

    override protected IPlatformHandle CreateNativeControlCore(IPlatformHandle parent) {
        var handle = base.CreateNativeControlCore(parent);
        Handle = handle.Handle;
        var properties = new Properties();
        properties.SetNumber(SDL3.SDL_PROP_WINDOW_CREATE_X11_WINDOW_NUMBER, Handle);
        Console.WriteLine(Handle);
        properties.SetBoolean(SDL3.SDL_PROP_WINDOW_CREATE_OPENGL_BOOLEAN, true);
        _window = Window.Create(properties);
        _context = Gl.CreateContext(_window);
        Fn.Viewport = (delegate*unmanaged[Cdecl]<int,int,int,int,void>) Gl.GetProcAddr("glViewport");
        Fn.ClearColor = (delegate* unmanaged[Cdecl]<float,float,float,float, void>) Gl.GetProcAddr("glClearColor");
        Fn.Clear = (delegate* unmanaged[Cdecl]<int, void>) Gl.GetProcAddr("glClear");
        Fn.Viewport(0, 0, (int)Width, (int)Height);
        Fn.ClearColor(1f, 1f, 1f, 1f);
        Fn.Clear(0x00004000);
        _timer.Tick += delegate { Draw(); };
        _timer.IsEnabled = true;
        return handle;
    }

    protected void Draw() {
        if (EventQueue.Poll(out var @event)) {
            Console.WriteLine(@event);
        }
        Fn.ClearColor((float)Random.Shared.NextDouble(), (float)Random.Shared.NextDouble(), (float)Random.Shared.NextDouble(), 1f);
        Fn.Clear(0x00004000);
        _window.GlSwap();
    }

    override protected void OnSizeChanged(SizeChangedEventArgs e) {
        base.OnSizeChanged(e);
        Fn.Viewport(0, 0, (int)e.NewSize.Width, (int)e.NewSize.Height);
    }

    override protected void DestroyNativeControlCore(IPlatformHandle control) {
        base.DestroyNativeControlCore(control);
        Gl.DestroyContext(_context);
        _window.Dispose();
    }
}