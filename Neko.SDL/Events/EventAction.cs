using System.ComponentModel;

namespace Neko.Sdl.Events;
public enum EventAction : int {
    [Description("Check but don't remove events from the queue front")]
    Peek = SDL_EventAction.SDL_PEEKEVENT,
    [Description("Add events to the back of the queue")]
    Add = SDL_EventAction.SDL_ADDEVENT,
    [Description("Retrieve/remove events from the front of the queue")]
    Get = SDL_EventAction.SDL_GETEVENT
}