using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Neko.Sdl.CodeGen;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

[GenEvent]
public unsafe partial struct TextEditingEvent {
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

    /// <summary>
    /// Text editing event
    /// </summary>
    /// <remarks>
    /// The text is not freed when set like this 
    /// </remarks>
    public string? Text {
        get => Marshal.PtrToStringUTF8((IntPtr)_text);
        set {
            if (value is null) {
                _text = null;
                return;
            }
            _text = value.ToUnmanagedPointer();
        }
    }
    public int Start;
    public int Length;
    
}