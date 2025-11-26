using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Neko.Sdl.Events;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl.EntryPoints;

internal static unsafe class EntryPointImpl {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AppMainFunction2 CallbackFunctionFactory(IApplication application) =>
        (argc, argv) => {
            Enter(application, argv, argc);
            return -1;
        };
    
    private ref struct Arguments : IDisposable {
        public static byte*[] Argv; 
        public Arguments(string[] args) {
            Argv = new byte*[args.Length + 2];
            var i1 = 2;
            foreach (var arg in args) {
                using var argUtf8 = arg.RentUtf8();
                var ptr = (byte*)UnmanagedMemory.Malloc((nuint)argUtf8.Length);
                fixed(byte* argPtr = argUtf8)
                    Unsafe.CopyBlock(ptr, argPtr, (uint)argUtf8.Length);
                Argv[i1++] = ptr;
            }
        }

        public ref byte* GetPinnableReference() => ref Argv[0];

        public void Dispose() {
            for (var i = 2; i < Argv.Length; i++) {
                UnmanagedMemory.Free(Argv[i]);
            }
        }
    }
    
    public static void Run(string[] args, AppMainFunction mainFunction) {
        using var argv = new Arguments(args);
        fixed(byte** argptr = argv)
            Run(mainFunction, args.Length, argptr);
    }
    
    private static void Run(string[] args, AppMainFunction2 mainFunction) {
        using var argv = new Arguments(args);
        fixed(byte** argptr = argv)
            Run(mainFunction, args.Length, &argptr[2]);
    }
    
    private static void Run(AppMainFunction mainFunction, int argc, byte** argv) {
        using var mainFunctionPin = mainFunction.Pin(GCHandleType.Normal);
        argv[-1] = (byte*)mainFunctionPin.Pointer;
        SDL_RunApp(argc, argv, &AppMain, 0);
    }

    private static void Run(AppMainFunction2 mainFunction, int argc, byte** argv) {
        using var mainFunctionPin = mainFunction.Pin(GCHandleType.Normal);
        argv[-1] = (byte*)mainFunctionPin.Pointer;
        SDL_RunApp(argc, argv, &AppMain2, 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RunCallback(string[] args, IApplication application) => 
        Run(args, CallbackFunctionFactory(application));
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int AppMain(int argc, byte** argv) {
        var pin = new Pin<AppMainFunction>(argv[-1]);
        if (pin.TryGetTarget(out var main)) {
            var args = new string[argc];
            for (int i = 0; i < args.Length; i++) 
                args[i] = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(argv[i]));
            return main(args);
        }
        return -1;
    }
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int AppMain2(int argc, byte** argv) {
        var pin = new Pin<AppMainFunction2>(argv[-1]);
        if (pin.TryGetTarget(out var main))
            return main(argc, argv);
        return -1;
    }
    private static void Enter(IApplication application, byte** argv, int argc) {
        using var applicationPin = application.Pin(GCHandleType.Normal);
        argv[-2] = (byte*)applicationPin.Pointer;
        SDL_EnterAppMainCallbacks(argc, argv, &AppInit, &AppIterate, &AppEvent, &AppQuit);
    }

    public static void Enter(IApplication application, string[] args) {
        using var argv = new Arguments(args);
        fixed(byte** argptr = argv)
            Enter(application, argptr, args.Length);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDL_AppResult AppEvent(IntPtr apppointer, SDL_Event* @event) {
        try {
            var pin = new Pin<IApplication>(apppointer);
            if (pin.TryGetTarget(out var application))
                return (SDL_AppResult)application.Event(Event.Create(@event));
        } catch (Exception e) {
            SdlException.Error = e.ToString();
            Log.Critical(0, e.ToString());
        }
        return SDL_AppResult.SDL_APP_FAILURE;
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDL_AppResult AppInit(IntPtr* apppointer, int argc, byte** argv) {
        try {
            var pin = new Pin<IApplication>(argv[-2]);
            if (pin.TryGetTarget(out var application)) {
                *apppointer = (nint)argv[-2];
                var args = new string[argc];
                for (int i = 0; i < args.Length; i++) 
                    args[i] = Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(argv[i]));
                return (SDL_AppResult)application.Init(args);
            }
        } catch (Exception e) {
            SdlException.Error = e.ToString();
            Log.Critical(0, e.ToString());
        }
        return SDL_AppResult.SDL_APP_FAILURE;
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDL_AppResult AppIterate(IntPtr apppointer) {
        try {
            var pin = new Pin<IApplication>(apppointer);
            if (pin.TryGetTarget(out var application))
                return (SDL_AppResult)application.Iterate();
        } catch (Exception e) {
            SdlException.Error = e.ToString();
            Log.Critical(0, e.ToString());
        }
        return SDL_AppResult.SDL_APP_FAILURE;
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void AppQuit(IntPtr apppointer, SDL_AppResult result) {
        try {
            var pin = new Pin<IApplication>(apppointer);
            if (pin.TryGetTarget(out var application))
                application.Quit((AppResult)result);
        } catch (Exception e) {
            SdlException.Error = e.ToString();
            Log.Critical(0, e.ToString());
        }
    }
}