using System;
using System.Reflection;
using Avalonia;
using Avalonia.Interactivity;
using Neko.Sdl;
using Neko.Sdl.Video;
using SDL;
using Window = Avalonia.Controls.Window;

namespace Neko.SDL.AvaloniaApp;

public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        AppMetadata.Set(AppDomain.CurrentDomain.FriendlyName, Assembly.GetEntryAssembly()!.GetName().Version!.ToString(), "nekosdl.testapp.avalonia");
        NekoSDL.Init(InitFlags.Video);
        Loaded += WindowLoaded;
    }
    private unsafe void WindowLoaded(object? sender, RoutedEventArgs e) {

        
    }
    
}