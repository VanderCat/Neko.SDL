namespace Neko.Sdl.Diagnostics;

/// <summary>
/// Possible outcomes from a triggered assertion
/// </summary>
/// <remarks>
/// <para>
/// When an enabled assertion triggers, it may call the assertion handler
/// (possibly one provided by the app via <see cref="Debug.AssertionHandler"/>), which will
/// return one of these values, possibly after asking the user.
/// </para>
/// <para>
/// Then SDL will respond based on this outcome (loop around to retry the condition,
/// try to break in a debugger, kill the program, or ignore the problem).
/// </para>
/// </remarks>
public enum AssertState {
    /// <summary>
    /// Retry the assert immediately.
    /// </summary>
    Retry,
    /// <summary>
    /// Make the debugger trigger a breakpoint.
    /// </summary>
    Break,
    /// <summary>
    /// Terminate the program.
    /// </summary>
    Abort,
    /// <summary>
    /// Ignore the assert.
    /// </summary>
    Ignore,
    /// <summary>
    /// Ignore the assert from now on.
    /// </summary>
    AlwaysIgnore
} 