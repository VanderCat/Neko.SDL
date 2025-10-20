using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Neko.Sdl.Events;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl.EntryPoints;

internal static unsafe class EntryPointImpl {
    private static IApplication? _application;
    private static AppMainFunction? _mainFunction;
    private static string[] _args = [];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AppMainFunction CallbackFunctionFactory(IApplication application) => 
        args => {
            Enter(application, args);
            return -1;
        };
    public static void Run(string[] args, AppMainFunction mainFunction) {
        if (_mainFunction is not null) 
            throw new InvalidOperationException("Application already initialized");
        _mainFunction = mainFunction;
        _args = args;
        var argv = (byte**)UnmanagedMemory.Calloc((nuint)IntPtr.Size, (nuint)args.Length+1);
        var argc = 0;
        foreach (var arg in args) {
            argc += arg.Length + 1;
        }
        var arglist = (byte*)UnmanagedMemory.Calloc((nuint)IntPtr.Size, (nuint)argc);
        var argCounter = arglist;
        foreach (var arg in args) {
            var argUtf8 = Encoding.UTF8.GetBytes(arg);
            fixed(byte* argPtr = argUtf8)
                Unsafe.CopyBlock(argCounter, argPtr, (uint)argUtf8.Length);
            argCounter += arg.Length + 1;
        }
        SDL_RunApp(argc, argv, &AppMain, 0);
        UnmanagedMemory.Free(arglist);
        UnmanagedMemory.Free(argv);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RunCallback(string[] args, IApplication application) => 
        Run(args, CallbackFunctionFactory(application));
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int AppMain(int argc, byte** argv) {
        return _mainFunction?.Invoke(_args)??-1;
    }
    
    public static void Enter(IApplication application, string[] args) {
        if (_application is not null) throw new InvalidOperationException("Application already initialized");
        _args = args;
        var argv = (byte**)UnmanagedMemory.Calloc((nuint)IntPtr.Size, (nuint)args.Length+1);
        var argc = 0;
        foreach (var arg in args) {
            argc += arg.Length + 1;
        }
        var arglist = (byte*)UnmanagedMemory.Calloc((nuint)IntPtr.Size, (nuint)argc);
        var argCounter = arglist;
        foreach (var arg in args) {
            var argUtf8 = Encoding.UTF8.GetBytes(arg);
            fixed(byte* argPtr = argUtf8)
                Unsafe.CopyBlock(argCounter, argPtr, (uint)argUtf8.Length);
            argCounter += arg.Length + 1;
        }
        _application = application;
        SDL_EnterAppMainCallbacks(args.Length, argv, &AppInit, &AppIterate, &AppEvent, &AppQuit);
        UnmanagedMemory.Free(arglist);
        UnmanagedMemory.Free(argv);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDL_AppResult AppEvent(IntPtr apppointer, SDL_Event* @event) {
        try {
            return (SDL_AppResult)(_application?.Event(ref Unsafe.AsRef<Event>(@event)) ?? AppResult.Failure);
        } catch (Exception e) {
            Log.Critical(0, e.ToString());
            return SDL_AppResult.SDL_APP_FAILURE;
        }
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDL_AppResult AppInit(IntPtr* apppointer, int argc, byte** argv) {
        try {
            return (SDL_AppResult)(_application?.Init(_args) ?? AppResult.Failure);
        } catch (Exception e) {
            Log.Critical(0, e.ToString());
            return SDL_AppResult.SDL_APP_FAILURE;
        }
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static SDL_AppResult AppIterate(IntPtr apppointer) {
        try {
            return (SDL_AppResult)(_application?.Iterate() ?? AppResult.Failure);
        } catch (Exception e) {
            Log.Critical(0, e.ToString());
            return SDL_AppResult.SDL_APP_FAILURE;
        }
    }
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void AppQuit(IntPtr apppointer, SDL_AppResult result) {
        try {
            _application?.Quit((AppResult)result);
        } catch (Exception e) {
            Log.Critical(0, e.ToString());
        }
    }
}