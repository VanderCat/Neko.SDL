using System.Runtime.InteropServices;
using Neko.Sdl.Extra;

namespace Neko.Sdl.Threading;

// this stuff is would be loaded indefinetly after thread would be started, should i do something whith it or leave as is?
// this also separated from thread impl and hidden, bcs this is only windows specific (windows must die)
internal static unsafe class WindowsThread {
    public static Lazy<NativeSharedObject> msvcrt = new(() => NativeSharedObject.Load("msvcrt.dll"));
    public static Lazy<IntPtr> _beginthreadex = new(() => msvcrt.Value.LoadFunction("_beginthreadex"));
    public static Lazy<IntPtr> _endthreadex = new(() => msvcrt.Value.LoadFunction("_endthreadex"));
    public static Thread Create(ThreadFunction fn, string? name) {
        var fnPin = fn.Pin(GCHandleType.Normal);
        return SDL_CreateThreadRuntime(&Thread.NativeThreadFunc, name, fnPin.Pointer, _beginthreadex.Value, _endthreadex.Value);
    }
    
    public static Thread Create(Properties prop) {
        return SDL_CreateThreadWithPropertiesRuntime(prop, _beginthreadex.Value, _endthreadex.Value);
    }
}