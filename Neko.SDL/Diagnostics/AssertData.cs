using System.Runtime.InteropServices;
using System.Text;

namespace Neko.Sdl.Diagnostics;

public unsafe class AssertData(NativeAssertData data) {
    /// <summary>
    /// true if app should always continue when assertion is triggered.
    /// </summary>
    public bool AlwaysIgnore = data.AlwaysIgnore;
    /// <summary>
    /// Number of times this assertion has been triggered.
    /// </summary>
    public uint TriggerCount = data.TriggerCount;
    /// <summary>
    /// A string of this assert's test code.
    /// </summary>
    public string Condition = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(data.Condition));
    /// <summary>
    /// The source file where this assert lives.
    /// </summary>
    public string Filename = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(data.Filename));
    /// <summary>
    /// The line in `filename` where this assert lives.
    /// </summary>
    public int Linenum = data.Linenum;
    /// <summary>
    /// The name of the function where this assert lives.
    /// </summary>
    public string Function = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(data.Function));

    /// <summary>
    /// Next item in the linked list.
    /// </summary>
    public AssertData Next => new (*data.Next);
}