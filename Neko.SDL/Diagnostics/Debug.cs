using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl.Diagnostics;

/// <summary>
/// A helpful assertion macro!
/// </summary>
/// <remarks>
/// SDL assertions operate like your usual assert macro, but with some added features:
/// <ul>
/// <li>
/// It offers a variety of responses when an assertion fails (retry, trigger the debugger,
/// abort the program, ignore the failure once, ignore it for the rest of the program's run).
/// </li>
/// <li>
/// It tries to show the user a dialog by default, if possible, but the app can provide a callback to handle assertion failures however they like.
/// </li>
/// <li>
/// It lets failed assertions be retried. Perhaps you had a network failure and just want to retry the test after plugging your network cable back in? You can.
/// </li>
/// <li>
/// It lets the user ignore an assertion failure, if there's a harmless problem that one can continue past.
/// </li>
/// <li>
/// It lets the user mark an assertion as ignored for the rest of the program's run; if there's a harmless problem that keeps popping up.
/// </li>
/// <li>
/// It provides statistics and data on all failed assertions to the app.
/// </li>
/// <li>
/// It allows the default assertion handler to be controlled with environment variables, in case an automated script needs to control it.
/// </li>
/// </ul>
/// </remarks>
public static unsafe class Debug {
    #region Native Functions
    [StackTraceHidden]
    [DllImport("SDL3")]
    private static extern AssertState SDL_ReportAssertion(NativeAssertData* assertData, byte* func, byte* file, int line);
    [DllImport("SDL3")]
    private static extern delegate*unmanaged[Cdecl]<NativeAssertData*,void*, AssertState> SDL_GetDefaultAssertionHandler();
    [DllImport("SDL3")]
    private static extern delegate*unmanaged[Cdecl]<NativeAssertData*,void*, AssertState> SDL_GetAssertionHandler(void **puserdata);
    [DllImport("SDL3")]
    private static extern void SDL_SetAssertionHandler(delegate*unmanaged[Cdecl]<NativeAssertData*,void*, AssertState> assertionHandler, void* userdata);
    
    [DllImport("SDL3")]
    private static extern NativeAssertData* SDL_GetAssertionReport();
    [DllImport("SDL3")]
    private static extern void SDL_ResetAssertionReport();
    #endregion

    /// <summary>
    /// Get a list of all assertion failures.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This function gets all assertions triggered since the last call to <see cref="ResetAssertionReport"/>, or the start of the program.
    /// </para>
    /// <para>
    /// This function is not thread safe. Other threads calling SDL_ResetAssertionReport() simultaneously, may render the returned pointer invalid.
    /// </para>
    /// </remarks>
    public static AssertData? AssertionReport {
        get {
            var result = SDL_GetAssertionReport();
            if (result is null) return null;
            return new AssertData(*result);
        }
    }

    /// <summary>
    /// Clear the list of all assertion failures.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This function will clear the list of all assertions triggered up to that point. Immediately following this call,
    /// <see cref="AssertionReport"/> will return no items. In addition, any previously-triggered assertions will be
    /// reset to a TriggerCount of zero, and their AlwaysIgnore state will be false.
    /// </para>
    /// <para>
    /// This function is not thread safe. Other threads triggering an assertion, or simultaneously calling this
    /// function may cause memory leaks or crashes.
    /// </para>
    /// </remarks>
    public static void ResetAssertionReport() => SDL_ResetAssertionReport();
    
    
    public static bool AssertionEnabled = true;
    /// <summary>
    /// The default assertion handler.
    /// </summary>
    /// <remarks>
    /// This is the function pointer that is called by default when an assertion is triggered.
    /// This is an internal function provided by SDL, that is used for assertions when <see cref="AssertionHandler"/>
    /// hasn't been used to provide a different function.
    /// </remarks>
    public static delegate*unmanaged[Cdecl]<NativeAssertData*,void*, AssertState> DefaultAssertionHandler => SDL_GetDefaultAssertionHandler();

    [StackTraceHidden]
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static AssertState NativeAssertionHandler(NativeAssertData* assertData, void* userdata) {
        var pin = new Pin<AssertionHandler>(userdata);
        if (pin.TryGetTarget(out var target))
            return target(new AssertData(*assertData));
        return DefaultAssertionHandler(assertData, userdata);
    }
    
    /// <summary>
    /// The current assertion handler.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This function allows an application to show its own assertion UI and/or force the response to an assertion failure.
    /// If the application doesn't provide this, SDL will try to do the right thing, popping up a system-specific GUI dialog,
    /// and probably minimizing any fullscreen windows.
    /// </para>
    /// <para>
    /// This callback may fire from any thread, but it runs wrapped in a mutex, so it will only fire from one thread at a time.
    /// </para>
    /// <para>
    /// This callback is NOT reset to SDL's internal handler upon <see cref="NekoSDL.Quit"/>!
    /// </para>
    /// </remarks>
    public static AssertionHandler? AssertionHandler {
        get {
            void* userdata = null; 
            SDL_GetAssertionHandler(&userdata);
            if (userdata is null)
                return null;
            var pin = new Pin<AssertionHandler>(userdata);
            return pin.Target;
        }
        set {
            void* userdata = null; 
            SDL_GetAssertionHandler(&userdata);
            if (userdata is not null) {
                var pin = new Pin<AssertionHandler>(userdata, true);
                pin.Dispose();
            }
            if (value is not null) {
                SDL_SetAssertionHandler(&NativeAssertionHandler, (void*)value.Pin(GCHandleType.Normal).Pointer);
                return;
            }
            SDL_SetAssertionHandler(DefaultAssertionHandler, null);
        }
    }
    
    /// <summary>
    /// An assertion test that is normally performed only in debug builds.
    /// </summary>
    /// <param name="condition">value to test</param>
    /// <remarks>
    /// <para>
    /// This function is enabled when the DEBUG is true, otherwise it is disabled.
    /// This is meant to only do these tests in debug builds, so they can tend to be more expensive,
    /// and they are meant to bring everything to a halt when they fail, with the programmer there to assess the problem.
    /// </para>
    /// <para>
    /// In short: you can sprinkle these around liberally and assume they will evaporate out of the build when building for end-users.
    /// </para>
    /// <para>
    /// When assertions are disabled, .NET strips this function, which means any function calls and side effects will not run,
    /// but the compiler will not complain about any otherwise-unused variables that are only referenced in the assertion.
    /// </para>
    /// <para>
    /// One can set the environment variable "SDL_ASSERT" to one of several strings ("abort", "break", "retry", "ignore", "always_ignore")
    /// to force a default behavior, which may be desirable for automation purposes.
    /// If your platform requires GUI interfaces to happen on the main thread but you're debugging an assertion in a background thread,
    /// it might be desirable to set this to "break" so that your debugger takes control as soon as assert is triggered,
    /// instead of risking a bad UI interaction (deadlock, etc) in the application.
    /// </para>
    /// </remarks>
    [Conditional("DEBUG")]
    [OverloadResolutionPriority(-1)]
    public static void Assert(bool condition) => Assert(condition, string.Empty, string.Empty, 0, string.Empty);
    
    /// <inheritdoc cref="Assert(bool)"/>
    [Conditional("DEBUG")]
    public static void Assert(bool condition,
        [CallerArgumentExpression("condition")] string? message = "", 
        [CallerMemberName] string? function = "",
        [CallerLineNumber] int? line = null,
        [CallerFilePath] string? filepath = "") => AssertInternal(condition, message, function, line, filepath);
    
    /// <summary>
    /// An assertion test that is performed even in release builds.
    /// </summary>
    /// <param name="condition">value to test</param>
    /// <remarks>
    /// <para>
    /// This function is always enabled. This is meant to be for tests that are cheap to make and extremely unlikely to fail;
    /// generally it is frowned upon to have an assertion failure in a release build, so these assertions generally need to be
    /// of more than life-and-death importance if there's a chance they might trigger. You should almost always consider
    /// handling these cases more gracefully than an assert allows.
    /// </para>
    /// <para>
    /// One can set the environment variable "SDL_ASSERT" to one of several strings ("abort", "break", "retry", "ignore",
    /// "always_ignore") to force a default behavior, which may be desirable for automation purposes. If your platform
    /// requires GUI interfaces to happen on the main thread but you're debugging an assertion in a background thread,
    /// it might be desirable to set this to "break" so that your debugger takes control as soon as assert is triggered,
    /// instead of risking a bad UI interaction (deadlock, etc) in the application. *
    /// </para>
    /// </remarks>
    [OverloadResolutionPriority(-1)]
    public static void AssertRelease(bool condition) => Assert(condition, string.Empty, string.Empty, 0, string.Empty);
    
    /// <inheritdoc cref="AssertRelease(bool)"/>
    public static void AssertRelease(bool condition,
        [CallerArgumentExpression("condition")] string? message = "", 
        [CallerMemberName] string? function = "",
        [CallerLineNumber] int? line = null,
        [CallerFilePath] string? filepath = "") => AssertInternal(condition,  message, function, line, filepath);
    
    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void AssertInternal(bool condition,
        [CallerArgumentExpression("condition")] string? message = "", 
        [CallerMemberName] string? function = "",
        [CallerLineNumber] int? line = null,
        [CallerFilePath] string? filepath = "") {
        if (AssertionEnabled && condition) { EnabledAssert(condition, message, function, line, filepath); }
    }

    [StackTraceHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnabledAssert(
        bool condition, 
        [CallerArgumentExpression("condition")] string? message = "", 
        [CallerMemberName] string? function = "",
        [CallerLineNumber] int? line = null,
        [CallerFilePath] string? filepath = "") {
        var msgPtr = message.ToUnmanagedPointer();
        var fnPtr = function.ToUnmanagedPointer();
        var fpPtr = filepath.ToUnmanagedPointer();
        while (condition) {
            //var data = UnmanagedMemory.Malloc(()sizeof(NativeAssertData)); 
            var data = new NativeAssertData {
                AlwaysIgnore = false,
                TriggerCount = 0,
                Condition = msgPtr,
                Filename = null,
                Linenum = 0,
                Function = null,
                Next = null,
            };
            var assertState = SDL_ReportAssertion(&data, fnPtr, fpPtr, line??-1);
            if (assertState == AssertState.Retry) 
                continue;
            if (assertState == AssertState.Break)
                Debugger.Break();
            break;
        }
    }
}