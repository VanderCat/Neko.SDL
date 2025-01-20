namespace Neko.Sdl;

[Flags]
public enum InitFlags : uint {
    /// <summary>
    /// Implies Events
    /// </summary>
    Audio = SDL_INIT_AUDIO,
    /// <summary>
    /// Implies Events, should be initialized on the main thread
    /// </summary>
    Video = SDL_INIT_VIDEO,
    /// <summary>
    /// Implies Events, should be initialized on the same thread as Video on Windows if you don't set Hint.JoystickThread
    /// </summary>
    Joystick = SDL_INIT_JOYSTICK,
    Haptic = SDL_INIT_HAPTIC,
    /// <summary>
    /// Implies Joystick
    /// </summary>
    Gamepad = SDL_INIT_GAMEPAD,
    Events = SDL_INIT_EVENTS,
    /// <summary>
    /// Implies Events
    /// </summary>
    Sensor = SDL_INIT_SENSOR,
    /// <summary>
    /// Implies Events
    /// </summary>
    Camera = SDL_INIT_CAMERA
}