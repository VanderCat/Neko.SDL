using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Neko.Sdl.CodeGen;
using Neko.Sdl.Video;

namespace Neko.Sdl.Events;

[GenEvent]
public unsafe partial struct TextEditingCandidatesEvent {
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
    private byte** _candidates;
    public int NumCandidates;
    public int SelectedCandidateIndex;
    private byte* SelectedCandidatePtr => _candidates[SelectedCandidateIndex];
    public string? SelectedCandidate => Marshal.PtrToStringUTF8((IntPtr)SelectedCandidatePtr);
    private SDLBool _horizontal;
    public bool Horizontal {
        get => _horizontal;
        set => _horizontal = value;
    }
    private byte _padding1;
    private byte _padding2;
    private byte _padding3;
}