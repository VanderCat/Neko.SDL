using Neko.Sdl.Input;
using Neko.Sdl.Video;
using SDL;

namespace Neko.Sdl.Tests;

public class MyWindow() : Window(800, 600, "hello", WindowFlags.Resizable | WindowFlags.HighPixelDensity) {

    public bool FlashScreen = false;
    public float Frame = 0f;

    public void Run() {
        while (!ShouldQuit) {
            if (FlashScreen) {
                FlashScreen = false;
                Console.WriteLine("flash!");
            }
            PollEvents();
            Renderer.DrawColorF = new ColorF(MathF.Sin(Frame) / 2 + 0.5f, MathF.Sin(Frame) / 2 + 0.5f, 0.3f);
            Renderer.Clear();
            Renderer.Present();

            Frame += 0.015f;

            Time.Timer.Delay(10);
        }
    }
    override protected void OnKeyDown(SDL_KeyboardEvent e) {
        switch ((Keycode)e.key) {
            case Keycode.R:
                RelativeMouseMode = !RelativeMouseMode;
                break;
            case Keycode.V:
                Console.WriteLine($"clipboard: {Clipboard.Text}");
                break;
            case Keycode.F10:
                Fullscreen = false;
                break;
            case Keycode.F11:
                Fullscreen = true;
                break;
            case Keycode.J: unsafe {
                using var gamepads = SDL3.SDL_GetGamepads(); //TODO: Neko.SDL.Gamepad

                if (gamepads == null || gamepads.Count == 0)
                    break;

                var gamepad = SDL3.SDL_OpenGamepad(gamepads[0]); //TODO: Neko.SDL.Gamepad

                int count;
                var bindings = SDL3.SDL_GetGamepadBindings(gamepad, &count); //TODO: Neko.SDL.Gamepad

                for (int i = 0; i < count; i++) {
                    var binding = *bindings[i];
                    Console.WriteLine(binding.input_type);
                    Console.WriteLine(binding.output_type);
                    Console.WriteLine();
                }

                SDL3.SDL_CloseGamepad(gamepad);//TODO: Neko.SDL.Gamepad
                break;
            }
            case Keycode.F1:
                TextInput.Start(this);
                break;
            case Keycode.F2:
                TextInput.Stop(this);
                break;
            case Keycode.M:
                var mod = (Keymod)e.mod;
                Console.WriteLine(mod);
                break;
            case Keycode.E:
                //Console.WriteLine(SDL3.SDL_GetEventDescription(e)); //TODO: Neko.SDL.Event
                break;
        }
    }

    override protected void OnTextInput(SDL_TextInputEvent e) {
        Console.WriteLine(e.GetText());
    }

    override protected void OnGamepadAdded(SDL_GamepadDeviceEvent e) {
        Console.WriteLine($"gamepad added: {e.which}");
    }

    override protected void OnPenProximityIn(SDL_PenProximityEvent e) {
        Console.WriteLine($"pen proximity in: {e.which}");
    }
}