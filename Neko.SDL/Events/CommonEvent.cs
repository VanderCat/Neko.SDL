namespace Neko.Sdl.Events;

public unsafe class CommonEvent : Event {
    public new SDL_CommonEvent* Handle;
    public CommonEvent(SDL_Event* id) : base(id) { }
    public CommonEvent(ref SDL_Event id) : base(ref id) { }
    
}