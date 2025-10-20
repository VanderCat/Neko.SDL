using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Neko.Sdl.CodeGen;
using Neko.Sdl.Extra.StandardLibrary;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

[GenEvent]
public unsafe partial struct TextInputEvent {
    /// <summary>
    /// Event type, shared with all events, Uint32 to cover user events which are not in the SDL_EventType enumeration
    /// </summary>
    public uint Type;
    public uint Reserved;
    /// <summary>
    /// In nanoseconds, populated using <see cref="Neko.Sdl.Time.Timer.GetTicksNS"/>
    /// </summary>
    public ulong Timestamp;
    
    public uint WindowId;
    public Window Window => Window.GetById(WindowId);
    private byte* _text;
    public string? Text {
        get => Marshal.PtrToStringUTF8((IntPtr)_text);
        set {
            UnmanagedMemory.Free(_text);
            if (value is null) {
                _text = null;
                return;
            }
            _text = value.ToUnmanagedPointer();
        }
    }
}