// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using System.Text;
using Neko.Sdl;
using Neko.Sdl.Events;
using Neko.Sdl.Input;
using Neko.Sdl.Video;
using SDL;

namespace Neko.Sdl.Tests;

public static class Program {
    public static void Main1() {
        if (OperatingSystem.IsWindows())
            Console.OutputEncoding = Encoding.UTF8;

        Hints.WindowsCloseOnAltF4.SetValue("null byte \0 in string");
        Debug.Assert(Hints.WindowsCloseOnAltF4.GetValue() == "null byte ");

        Hints.WindowsCloseOnAltF4.SetValue("1");
        
        NekoSDL.InitSubSystem(InitFlags.Video | InitFlags.Gamepad);

        // Check if satellite libraries exist.
        Console.WriteLine($"SDL revision: {NekoSDL.Revision}, IMG IMG_Version(), TTF TTF_Version(), Mixer MIX_Version()");
        PrintDisplays();
        using (var window = new MyWindow()) {
            window.Setup();
            window.CreateRenderer();
            PrintWindows();

            const Keymod state = Keymod.Caps | Keymod.Alt;
            Keyboard.ModState = state;
            Debug.Assert(Keyboard.ModState == state);

            window.Run();
        }
        NekoSDL.QuitSubSystem(InitFlags.Video | InitFlags.Gamepad);

        NekoSDL.Quit();
    }

    private static void PrintDisplays() {
        var displays = Display.GetIds(); ;
        foreach (var id in displays) {
            Console.WriteLine(id);
            var modes = Display.GetFullscreenModes(id);
            foreach (var mode in modes) 
                Console.WriteLine($"{mode.Width}x{mode.Height}@{mode.RefreshRate}");
        }
    }

    private static void PrintWindows() {
        var windows = Window.GetWindows();
        foreach (var window in windows) {
            Console.WriteLine($"Window title: {window.Title}");
        }
    }
}