using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Neko.Sdl;
//
// public static unsafe class Logging {
//     public enum Priority {
//         Invalid = SDL_LogPriority.SDL_LOG_PRIORITY_INVALID,
//         Trace = SDL_LogPriority.SDL_LOG_PRIORITY_TRACE,
//         Verbose = SDL_LogPriority.SDL_LOG_PRIORITY_VERBOSE,
//         Debug = SDL_LogPriority.SDL_LOG_PRIORITY_DEBUG,
//         Info = SDL_LogPriority.SDL_LOG_PRIORITY_INFO,
//         Warn = SDL_LogPriority.SDL_LOG_PRIORITY_WARN,
//         Error = SDL_LogPriority.SDL_LOG_PRIORITY_ERROR,
//         Critical = SDL_LogPriority.SDL_LOG_PRIORITY_CRITICAL,
//     }
//
//     private static Dictionary<int, string> _categories = new Dictionary<int, string> {
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_APPLICATION, "Application"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_ERROR, "Error"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_ASSERT, "Assert"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_SYSTEM, "System"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_AUDIO, "Audio"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_VIDEO, "Video"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_RENDER, "Render"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_INPUT, "Input"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_TEST, "Test"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_GPU, "GPU"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_RESERVED2, "Reserved"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_RESERVED3, "Reserved"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_RESERVED4, "Reserved"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_RESERVED5, "Reserved"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_RESERVED6, "Reserved"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_RESERVED7, "Reserved"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_RESERVED8, "Reserved"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_RESERVED9, "Reserved"},
//         {(int)SDL_LogCategory.SDL_LOG_CATEGORY_RESERVED10, "Reserved"},
//     };
//
//     private static int _lastfreecat = 19;
//
//     public static int AddCategory(string name) {
//         while (_categories.ContainsKey(_lastfreecat)) {
//             _lastfreecat++;
//         }
//         _categories.Add(_lastfreecat, name);
//         return _lastfreecat++;
//     }
//
//     public static void SetCategory(int index, string name) {
//         if (index < 19)
//             throw new ArgumentException("Could not change default category name");
//     }
//
//     public static string? GetCategoryName(int index) {
//         _categories.TryGetValue(index, out var value);
//         return value;
//     } 
//     
//     [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
//     public static void NativeLogFunction(IntPtr userdata, int category, SDL_LogPriority priority, byte* message) {
//         var log = userdata.AsPin<LogFunction>();
//         if (log.TryGetTarget(out var target))
//             target.Invoke(category, (Priority)priority, Marshal.PtrToStringUTF8((IntPtr)message)??"");
//     }
//
//     public delegate void LogFunction(int category, Priority priority, string message);
//
//     private static delegate*unmanaged[Cdecl]<IntPtr, int, SDL_LogPriority, byte*, void>
//         _function = SDL_GetDefaultLogOutputFunction();
//
//     private static Pin<LogFunction>? _outputFunction;
//     public static LogFunction? OutputFunction {
//         get => _outputFunction?.Target;
//         set {
//             _outputFunction?.Dispose();
//             if (value is null) {
//                 _outputFunction = null;
//                 SDL_SetLogOutputFunction(SDL_GetDefaultLogOutputFunction(), 0);
//                 return;
//             }
//             _outputFunction = value.Pin(GCHandleType.Normal);
//             SDL_SetLogOutputFunction(&NativeLogFunction, _outputFunction.Pointer);
//         }
//     }
//
//     public static void SetPriorities(Priority priority) => SDL_SetLogPriorities((SDL_LogPriority)priority);
//     public static void ResetPriorities() => SDL_ResetLogPriorities();
//     
//     public static Priority GetPriority(int category) => (Priority)SDL_GetLogPriority(category);
//     public static void SetPriority(int category, Priority priority) => SDL_SetLogPriority(category, (SDL_LogPriority)priority);
//     
//     public static void SetPriorityPrefix(Priority priority, string prefix) => SDL_SetLogPriorityPrefix((SDL_LogPriority)priority, prefix);
//     
//     [Obsolete("Use C# stuff instead, e.g. provided log function")]
//     public static void Log(string message) {
//         var a = Encoding.UTF8.GetBytes(message.Replace("%", "%%"));
//         fixed(byte* ptr = a)
//             SDL_Log(ptr, __arglist());
//     }
//     
//     [Obsolete("Use C# stuff instead, e.g. provided log function")]
//     public static void LogCritical(int category, string message) {
//         var a = Encoding.UTF8.GetBytes(message.Replace("%", "%%"));
//         fixed(byte* ptr = a)
//             SDL_LogCritical(category, ptr, __arglist());
//     }
//     
//     [Obsolete("Use C# stuff instead, e.g. provided log function")]
//     public static void LogDebug(int category, string message) {
//         var a = Encoding.UTF8.GetBytes(message.Replace("%", "%%"));
//         fixed(byte* ptr = a)
//             SDL_LogDebug(category, ptr, __arglist());
//     }
//     
//     [Obsolete("Use C# stuff instead, e.g. provided log function")]
//     public static void LogError(int category, string message) {
//         var a = Encoding.UTF8.GetBytes(message.Replace("%", "%%"));
//         fixed(byte* ptr = a)
//             SDL_LogError(category, ptr, __arglist());
//     }
//     
//     [Obsolete("Use C# stuff instead, e.g. provided log function")]
//     public static void LogInfo(int category, string message) {
//         var a = Encoding.UTF8.GetBytes(message.Replace("%", "%%"));
//         fixed(byte* ptr = a)
//             SDL_LogInfo(category, ptr, __arglist());
//     }
//     
//     [Obsolete("Use C# stuff instead, e.g. provided log function")]
//     public static void LogTrace(int category, string message) {
//         var a = Encoding.UTF8.GetBytes(message.Replace("%", "%%"));
//         fixed(byte* ptr = a)
//             SDL_LogInfo(category, ptr, __arglist());
//     }
//     
//     [Obsolete("Use C# stuff instead, e.g. provided log function")]
//     public static void LogVerbose(int category, string message) {
//         var a = Encoding.UTF8.GetBytes(message.Replace("%", "%%"));
//         fixed(byte* ptr = a)
//             SDL_LogInfo(category, ptr, __arglist());
//     }
//     
//     [Obsolete("Use C# stuff instead, e.g. provided log function")]
//     public static void LogWarn(int category, string message) {
//         var a = Encoding.UTF8.GetBytes(message.Replace("%", "%%"));
//         fixed(byte* ptr = a)
//             SDL_LogInfo(category, ptr, __arglist());
//     }
//     
//     [Obsolete("Use C# stuff instead, e.g. provided log function")]
//     public static void LogMessage(int category, Priority priority, string message) {
//         var a = Encoding.UTF8.GetBytes(message.Replace("%", "%%"));
//         fixed(byte* ptr = a)
//             SDL_LogInfo(category, ptr, __arglist());
//     }
// }