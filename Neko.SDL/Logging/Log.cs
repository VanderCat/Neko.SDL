using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Neko.Sdl;

/// <summary>
/// Simple log messages with priorities and categories. A message's <see cref="LogPriority"/> signifies how important the message is.
/// A message's category signifies from what domain it belongs to. Every category has a minimum priority specified:
/// when a message belongs to that category, it will only be sent out if it has that minimum priority or higher.
/// <br/><br/>
/// SDL's own logs are sent below the default priority threshold, so they are quiet by default.
/// <br/><br/>
/// You can change the log verbosity programmatically using <see cref="Priorities"/> or with
/// <see cref="Hints.Logging"/>, or with the "SDL_LOGGING" environment variable.
/// This variable is a comma separated set of category=level tokens that define the default
/// logging levels for SDL applications.
/// <br/><br/>
/// The category can be a numeric category, one of "app", "error", "assert",
/// "system", "audio", "video", "render", "input", "test", or * for any unspecified category.
/// <br/><br/>
/// The level can be a numeric level, one of "trace", "verbose",
/// "debug", "info", "warn", "error", "critical", or "quiet" to disable that category.
/// <br/><br/>
/// You can omit the category if you want to set the logging level for all categories.
/// <br/><br/>
/// If this hint isn't set, the default log levels are equivalent to:
/// <code>
/// app=info,assert=warn,test=verbose,*=error
/// </code>
/// Here's where the messages go on different platforms:
/// <ul>
/// <li>Windows: debug output stream</li>
/// <li>Android: log output</li>
/// <li>Others: standard error output (stderr)</li>
/// </ul>
/// You don't need to have a newline (\n) on the end of messages, the functions will do that for you.
/// For consistent behavior cross-platform, you shouldn't have any newlines in messages,
/// such as to log multiple lines in one call; unusual platform-specific behavior can be observed in such usage.
/// Do one log call per line instead, with no newlines in messages.
/// <br/><br/>
/// Each log call is atomic, so you won't see log messages cut off one another when logging from multiple threads.
/// </summary>
public static class Log {
    
    /// <summary>
    /// Log a message with the specified category and priority
    /// </summary>
    /// <param name="category">the category of the message</param>
    /// <param name="priority">the priority of the message</param>
    /// <param name="text">a text of the message</param>
    public static unsafe void Message(int category, LogPriority priority, string text) {
        SDL_LogMessageV(category, (SDL_LogPriority)priority, text.Replace("%", "%%"), null);
    }

    /// <summary>
    /// Log a message with <see cref="Neko.Sdl.LogPriority.Critical"/>
    /// </summary>
    /// <param name="category">the category of the message</param>
    /// <param name="text">a text of the message</param>
    public static void Critical(int category, string text) =>
        Message(category, LogPriority.Critical, text);
    /// <summary>
    /// Log a message with <see cref="Neko.Sdl.LogPriority.Debug"/>
    /// </summary>
    /// <inheritdoc cref="Critical"/>
    public static void Debug(int category, string text) =>
        Message(category, LogPriority.Debug, text);
    /// <summary>
    /// Log a message with <see cref="Neko.Sdl.LogPriority.Error"/>
    /// </summary>
    /// <inheritdoc cref="Critical"/>
    public static void Error(int category, string text) =>
        Message(category, LogPriority.Error, text);
    /// <summary>
    /// Log a message with <see cref="Neko.Sdl.LogPriority.Info"/>
    /// </summary>
    /// <inheritdoc cref="Critical"/>
    public static void Info(int category, string text) =>
        Message(category, LogPriority.Info, text);
    /// <summary>
    /// Log a message with <see cref="Neko.Sdl.LogPriority.Trace"/>
    /// </summary>
    /// <inheritdoc cref="Critical"/>
    public static void Trace(int category, string text) =>
        Message(category, LogPriority.Trace, text);
    /// <summary>
    /// Log a message with <see cref="Neko.Sdl.LogPriority.Verbose"/>
    /// </summary>
    /// <inheritdoc cref="Critical"/>
    public static void Verbose(int category, string text) =>
        Message(category, LogPriority.Verbose, text);
    /// <summary>
    /// Log a message with <see cref="Neko.Sdl.LogPriority.Warn"/>
    /// </summary>
    /// <inheritdoc cref="Critical"/>
    public static void Warn(int category, string text) =>
        Message(category, LogPriority.Warn, text);

    /// <summary>
    /// Log a message with Application category and <see cref="Neko.Sdl.LogPriority.Info"/>.
    /// </summary>
    /// <param name="text">a text of the message</param>
    public static void LogApp(string text) =>
        Info((int)SDL_LogCategory.SDL_LOG_CATEGORY_APPLICATION, text);

    /// <summary>
    /// Set or get the priority of a particular log category
    /// </summary>
    public sealed class PriorityAccessor {
        internal PriorityAccessor() {}
        /// <summary>
        /// Set or get the priority of a particular log category
        /// </summary>
        /// <param name="category">the category to query or assign</param>
        public LogPriority this[int category] {
            get => (LogPriority)SDL_GetLogPriority(category);
            set => SDL_SetLogPriority(category, (SDL_LogPriority)value);
        }
        /// <summary>
        /// Set or get the priority of a particular log category
        /// </summary>
        /// <param name="category">the category to query or assign</param>
        public LogPriority this[SDL_LogCategory category] {
            get => (LogPriority)SDL_GetLogPriority(category);
            set => SDL_SetLogPriority(category, (SDL_LogPriority)value);
        }

        /// <summary>
        /// Reset all priorities to default
        /// </summary>
        /// <remarks>This is called by <see cref="NekoSDL.Quit"/></remarks>
        public void Reset() {
            SDL_ResetLogPriorities();
        }

        /// <summary>
        /// Set the priority of all log categories
        /// </summary>
        /// <param name="priority">the <see cref="Neko.Sdl.LogPriority"/> to assign</param>
        public void Set(LogPriority priority) {
            SDL_SetLogPriorities((SDL_LogPriority)priority);
        }
    }
    
    /// <summary>
    /// Set the text prepended to log messages of a given priority
    /// </summary>
    /// <param name="priority">priority to modify</param>
    /// <param name="prefix">the prefix to use for that log priority, or null to use no prefix</param>
    /// <remarks>
    /// By default <see cref="Neko.Sdl.LogPriority.Info"/> and below have no prefix, and <see cref="Neko.Sdl.LogPriority.Warn"/> and higher
    /// have a prefix showing their priority, e.g. "WARNING: ".
    /// </remarks>
    public static void SetPriorityPrefix(LogPriority priority, string? prefix = null) =>
        SDL_SetLogPriorityPrefix((SDL_LogPriority)priority, prefix).ThrowIfError();

    /// <inheritdoc cref="PriorityAccessor"/>
    public static readonly PriorityAccessor Priorities = new();
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static unsafe void NativeLogFunction(IntPtr userdata, int category, SDL_LogPriority priority, byte* message) {
        var log = userdata.AsPin<LogFunction>();
        if (log.TryGetTarget(out var target))
            target(category, (LogPriority)priority, Marshal.PtrToStringUTF8((IntPtr)message)??"");
    }

    /// <summary>
    /// The prototype for the log output callback function
    /// </summary>
    /// <remarks>This function is called by SDL when there is new text to be logged. A mutex is held so that this function is never called by more than one thread at once.</remarks>
    public delegate void LogFunction(int category, LogPriority priority, string message);

    private static Pin<LogFunction>? _outputFunction;
    /// <summary>
    /// Replace the default log output function with one of your own
    /// </summary>
    public static unsafe LogFunction? OutputFunction {
        get => _outputFunction?.Target;
        set {
            _outputFunction?.Dispose();
            if (value is null) {
                _outputFunction = null;
                SDL_SetLogOutputFunction(SDL_GetDefaultLogOutputFunction(), 0);
                return;
            }
            _outputFunction = value.Pin(GCHandleType.Normal);
            SDL_SetLogOutputFunction(&NativeLogFunction, _outputFunction.Pointer);
        }
    }

}