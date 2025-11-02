using System.Runtime.InteropServices;

namespace Neko.Sdl.Diagnostics;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct NativeAssertData {
    /// <summary>
    /// true if app should always continue when assertion is triggered.
    /// </summary>
    public SDLBool AlwaysIgnore;
    /// <summary>
    /// Number of times this assertion has been triggered.
    /// </summary>
    public uint TriggerCount;
    /// <summary>
    /// A string of this assert's test code.
    /// </summary>
    public byte* Condition;
    /// <summary>
    /// The source file where this assert lives.
    /// </summary>
    public byte* Filename;
    /// <summary>
    /// The line in `filename` where this assert lives.
    /// </summary>
    public int Linenum;
    /// <summary>
    /// The name of the function where this assert lives.
    /// </summary>
    public byte* Function;
    /// <summary>
    /// Next item in the linked list.
    /// </summary>
    public NativeAssertData* Next;
}