using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neko.Sdl.Time;

public static unsafe class Timer {
    public delegate uint Callback(uint id, uint interval);
    public delegate ulong CallbackNS(uint id, ulong interval);
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static uint NativeCallback(IntPtr userdata, SDL_TimerID timerId, uint interval) {
        var pin = userdata.AsPin<Callback>(true);
        var result = pin.Target((uint)timerId, interval);
        if (result == 0) {
            pin.Dispose();
        }
        return result;
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static ulong NativeCallbackNS(IntPtr userdata, SDL_TimerID timerId, ulong interval) {
        var pin = userdata.AsPin<CallbackNS>(true);
        var result = pin.Target((uint)timerId, interval);
        if (result == 0) {
            pin.Dispose();
        }
        return result;
    }

    public static uint Add(uint interval, Callback callback) =>
        (uint)SDL_AddTimer(interval, &NativeCallback, callback.Pin(GCHandleType.Normal).Pointer);
    
    public static uint AddNS(ulong interval, CallbackNS callback) =>
        (uint)SDL_AddTimerNS(interval, &NativeCallbackNS, callback.Pin(GCHandleType.Normal).Pointer);

    public static void RemoveTimer(uint id) => SDL_RemoveTimer((SDL_TimerID)id); 
        
    public static void Delay(uint ms) => SDL_Delay(ms);
    public static void DelayNS(ulong ns) => SDL_DelayNS(ns);
    public static void DelayPrecise(ulong ns) => SDL_DelayPrecise(ns);

    public static ulong GetPerformanceCounter() => SDL_GetPerformanceCounter();
    public static ulong GetPerformanceFrequency() => SDL_GetPerformanceFrequency();

    public static ulong GetTicks() => SDL_GetTicks();
    public static ulong GetTicksNS() => SDL_GetTicksNS();
    
}