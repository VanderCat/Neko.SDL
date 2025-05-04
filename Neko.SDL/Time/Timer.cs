using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neko.Sdl.Time;

/// <summary>
/// SDL provides time management functionality. It is useful for dealing with (usually) small durations of time.
/// <br/><br/>
/// This is not to be confused with calendar time management, which is provided by CategoryTime.
/// <br/><br/>
/// This category covers measuring time elapsed (<see cref="GetTicks"/>, <see cref="GetPerformanceCounter"/>), putting a thread
/// to sleep for a certain amount of time (<see cref="Delay"/>, <see cref="DelayNS"/>, <see cref="DelayPrecise"/>), and firing a callback
/// function after a certain amount of time has elasped (<see cref="Add"/>, etc).
/// </summary>
public static unsafe class Timer {
    /// <summary>
    /// Delegate for the millisecond timer callback function
    /// </summary>
    /// <param name="id">the current timer being processed</param>
    /// <param name="interval">the current callback time interval</param>
    /// <returns>Returns the new callback time interval, or 0 to disable further runs of the callback</returns>
    /// <remarks>
    /// The callback function is passed the current timer interval and returns the next timer interval, in milliseconds.
    /// If the returned value is the same as the one passed in, the periodic alarm continues, otherwise a new alarm is
    /// scheduled. If the callback returns 0, the periodic alarm is canceled and will be removed.
    /// <br/><br/>
    /// SDL may call this callback at any time from a background thread; the application is responsible for locking
    /// resources the callback touches that need to be protected.
    /// </remarks>
    public delegate uint Callback(uint id, uint interval);
    /// <summary>
    /// Delegate for the nanosecond timer callback function.
    /// </summary>
    /// <param name="id">the current timer being processed</param>
    /// <param name="interval">the current callback time interval</param>
    /// <returns>Returns the new callback time interval, or 0 to disable further runs of the callback</returns>
    /// <remarks>
    /// The callback function is passed the current timer interval and returns the next timer interval, in nanoseconds.
    /// If the returned value is the same as the one passed in, the periodic alarm continues, otherwise a new alarm is
    /// scheduled. If the callback returns 0, the periodic alarm is canceled and will be removed.
    /// <br/><br/>
    /// SDL may call this callback at any time from a background thread; the application is responsible for locking
    /// resources the callback touches that need to be protected.
    /// </remarks>
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

    /// <summary>
    /// Call a callback function at a future time.
    /// </summary>
    /// <param name="interval">the timer delay, passed to callback</param>
    /// <param name="callback">function to call when the specified interval elapses</param>
    /// <returns>timer ID</returns>
    /// <remarks>
    /// The callback function is passed the current timer interval and the user supplied parameter from the
    /// <see cref="Add"/> call and should return the next timer interval. If the value returned from the callback is 0,
    /// the timer is canceled and will be removed.
    /// <br/><br/>
    /// The callback is run on a separate thread, and for short timeouts can potentially be called before this function
    /// returns.
    /// <br/><br/>
    /// Timers take into account the amount of time it took to execute the callback. For example, if the callback took
    /// 250 ms to execute and returned 1000 (ms), the timer would only wait another 750 ms before its next iteration.
    /// <br/><br/>
    /// Timing may be inexact due to OS scheduling. Be sure to note the current time with <see cref="GetTicksNS"/> or
    /// <see cref="GetPerformanceCounter"/> in case your callback needs to adjust for variances.
    /// <br/><br/>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static uint Add(uint interval, Callback callback) {
        var t = SDL_AddTimer(interval, &NativeCallback, callback.Pin(GCHandleType.Normal).Pointer);
        if (t == 0) throw new SdlException();
        return (uint)t;
    }
    
    /// <inheritdoc cref="Add"/>
    /// <remarks>
    /// The callback function is passed the current timer interval and the user supplied parameter from the
    /// Timer.AddNS() call and should return the next timer interval. If the value returned from the callback is 0,
    /// the timer is canceled and will be removed.
    /// <br/><br/>
    /// The callback is run on a separate thread, and for short timeouts can potentially be called before this function
    /// returns.
    /// <br/><br/>
    /// Timers take into account the amount of time it took to execute the callback. For example, if the callback took
    /// 250 ns to execute and returned 1000 (ns), the timer would only wait another 750 ns before its next iteration.
    /// <br/><br/>
    /// Timing may be inexact due to OS scheduling. Be sure to note the current time with <see cref="GetTicksNS"/> or
    /// T<see cref="GetPerformanceCounter"/> in case your callback needs to adjust for variances.
    /// <br/><br/>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static uint AddNS(ulong interval, CallbackNS callback) {
        var t = SDL_AddTimerNS(interval, &NativeCallbackNS, callback.Pin(GCHandleType.Normal).Pointer);
        if (t == 0) throw new SdlException();
        return (uint)t;
    }

    /// <summary>
    /// Remove a timer created with <see cref="Add"/>
    /// </summary>
    /// <param name="id">the ID of the timer to remove</param>
    /// <remarks>It is safe to call this function from any thread</remarks>
    public static void RemoveTimer(uint id) => SDL_RemoveTimer((SDL_TimerID)id).ThrowIfError(); 
    
    /// <summary>
    /// Wait a specified number of milliseconds before returning
    /// </summary>
    /// <param name="ms">the number of milliseconds to delay</param>
    /// <remarks>
    /// This function waits a specified number of milliseconds before returning. It waits at least the specified time,
    /// but possibly longer due to OS scheduling.
    /// <br/><br/>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static void Delay(uint ms) => SDL_Delay(ms);
    
    /// <summary>
    /// Wait a specified number of nanoseconds before returning
    /// </summary>
    /// <param name="ns">the number of nanoseconds to delay</param>
    /// <remarks>
    /// This function waits a specified number of nanoseconds before returning. It waits at least the specified time,
    /// but possibly longer due to OS scheduling.
    /// <br/><br/>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static void DelayNS(ulong ns) => SDL_DelayNS(ns);
    
    /// <summary>
    /// Wait a specified number of nanoseconds before returning
    /// </summary>
    /// <param name="ns">the number of nanoseconds to delay</param>
    /// <remarks>
    /// This function waits a specified number of nanoseconds before returning. It will attempt to wait as close to the
    /// requested time as possible, busy waiting if necessary, but could return later due to OS scheduling.
    /// <br/><br/>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static void DelayPrecise(ulong ns) => SDL_DelayPrecise(ns);

    /// <summary>
    /// Get the current value of the high resolution counter.
    /// </summary>
    /// <returns>current counter value</returns>
    /// <remarks>
    /// This function is typically used for profiling.
    /// <br/><br/>
    /// The counter values are only meaningful relative to each other. Differences between values can be converted to
    /// times by using <see cref="GetPerformanceFrequency"/>.
    /// <br/><br/>
    /// It is safe to call this function from any thread.
    /// </remarks>
    public static ulong GetPerformanceCounter() => SDL_GetPerformanceCounter();
    
    /// <summary>
    /// Get the count per second of the high resolution counter.
    /// </summary>
    /// <returns>platform-specific count per second</returns>
    /// <remarks>It is safe to call this function from any thread.</remarks>
    public static ulong GetPerformanceFrequency() => SDL_GetPerformanceFrequency();

    /// <summary>
    /// Get the number of milliseconds that have elapsed since the SDL library initialization
    /// </summary>
    /// <returns>
    /// unsigned 64‑bit integer that represents the number of milliseconds that have elapsed since the SDL library was
    /// initialized (typically via a call to <see cref="NekoSDL.Init"/>).
    /// </returns>
    /// <remarks>It is safe to call this function from any thread.</remarks>
    public static ulong GetTicks() => SDL_GetTicks();
    
    /// <summary>
    /// Get the number of nanoseconds since SDL library initialization
    /// </summary>
    /// <returns>an unsigned 64-bit value representing the number of nanoseconds since the SDL library initialized</returns>
    public static ulong GetTicksNS() => SDL_GetTicksNS();
    
}