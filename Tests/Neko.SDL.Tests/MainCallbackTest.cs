// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.EntryPoints;
using Neko.Sdl.Events;
using SDL;
using Timer = Neko.Sdl.Time.Timer;

namespace Neko.Sdl.Tests;

/// <summary>
/// Base class for tests that use SDL3 main callbacks.
/// See https://wiki.libsdl.org/SDL3/README/main-functions#how-to-use-main-callbacks-in-sdl3.
/// </summary>
[TestFixture]
[Apartment(ApartmentState.STA)]
public abstract unsafe class MainCallbacksTest : IApplication {
    [Test]
    public void TestEnterMainCallbacks() {
        NekoSDL.EnterApp([], this);
    }

    public AppResult Init(string[] args) {
        Log.Priorities.Set(LogPriority.Verbose);
        Log.OutputFunction = (category, priority, message) => Console.WriteLine(message);
        return AppResult.Continue;
    }

    public AppResult Iterate() {
        Timer.Delay(10);
        return AppResult.Continue;
    }

    public AppResult Event(Event e) {
        switch (e.Type) {
            case EventType.Quit:
            case EventType.WindowCloseRequested:
            case EventType.Terminating:
            case EventType.KeyDown when e.Key.Key == SDL_Keycode.SDLK_ESCAPE: //TODO: Neko.SDL.Event
                return AppResult.Success;
        }

        return AppResult.Continue;
    }

    public void Quit(AppResult result) { }
}