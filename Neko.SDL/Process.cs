using System.Runtime.CompilerServices;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl;

/// <summary>
/// Process control support.
/// <br/><br/>
/// These functions provide a cross-platform way to spawn and manage OS-level processes.
/// <br/><br/>
/// You can create a new subprocess with <see cref="Create(IEnumerable{string},bool)"/> and optionally
/// read and write to it using <see cref="Read"/> or <see cref="GetInput"/> and <see cref="GetOutput"/>.
/// If more advanced functionality like chaining input between processes is necessary, you can use <see cref="Create(Properties)"/>.
/// <br/><br/>
/// You can get the status of a created process with <see cref="Wait"/>, or terminate the process with <see cref="Kill"/>.
/// <br/><br/>
/// Don't forget to call <see cref="Dispose"/> to clean up, whether the process was killed, terminated on its own, or is still running!
/// </summary>
public unsafe partial class Process : SdlWrapper<SDL_Process> {
    /// <summary>
    /// Create a new process.
    /// </summary>
    /// <param name="args">the path and arguments for the new process</param>
    /// <param name="pipeStdio">true to create pipes to the process's standard input and from the process's standard output, false for the process to have no input and inherit the application's standard output.</param>
    /// <returns>Returns the newly created and running process, or null if the process couldn't be created</returns>
    /// <remarks>
    /// The path to the executable is supplied in args[0]. args[1..N] are additional arguments passed on the command line of the new process.
    /// <code>
    /// const char *args[] = { "myprogram", "argument", NULL };
    /// </code>
    /// Setting pipeStdio to true is equivalent to setting SDL_PROP_PROCESS_CREATE_STDIN_NUMBER and SDL_PROP_PROCESS_CREATE_STDOUT_NUMBER to SDL_PROCESS_STDIO_APP, and will allow the use of <see cref="Read"/> or <see cref="GetInput"/> and <see cref="Output"/>.
    /// </remarks>
    /// <seealso cref="Create()"/>
    public static Process? Create(IEnumerable<string> args, bool pipeStdio = false) {
        using var array = args.MarshalToBytePtrPtr();
        return SDL_CreateProcess(array, pipeStdio);
    }
    /// <summary>
    /// Create a new process with the specified properties
    /// </summary>
    /// <param name="properties">the properties to use</param>
    /// <returns>Returns the newly created and running process, or null if the process couldn't be created</returns>
    /// <remarks>
    /// These are the supported properties: <br/>
    /// <br/>
    /// SDL_PROP_PROCESS_CREATE_ARGS_POINTER: an array of strings containing the program to run, any arguments, and a NULL pointer, e.g. const char *args[] = { "myprogram", "argument", NULL }. This is a required property.
    ///<br/>
    /// SDL_PROP_PROCESS_CREATE_ENVIRONMENT_POINTER: an SDL_Environment pointer. If this property is set, it will be the entire environment for the process, otherwise the current environment is used.
    /// <br/>
    /// SDL_PROP_PROCESS_CREATE_WORKING_DIRECTORY_STRING: a UTF-8 encoded string representing the working directory for the process, defaults to the current working directory.
    ///<br/>
    /// SDL_PROP_PROCESS_CREATE_STDIN_NUMBER: an SDL_ProcessIO value describing where standard input for the process comes from, defaults to SDL_PROCESS_STDIO_NULL.
    ///<br/>
    /// SDL_PROP_PROCESS_CREATE_STDIN_POINTER: an SDL_IOStream pointer used for standard input when SDL_PROP_PROCESS_CREATE_STDIN_NUMBER is set to SDL_PROCESS_STDIO_REDIRECT.
    ///<br/>
    /// SDL_PROP_PROCESS_CREATE_STDOUT_NUMBER: an SDL_ProcessIO value describing where standard output for the process goes to, defaults to SDL_PROCESS_STDIO_INHERITED.
    ///<br/>
    /// SDL_PROP_PROCESS_CREATE_STDOUT_POINTER: an SDL_IOStream pointer used for standard output when SDL_PROP_PROCESS_CREATE_STDOUT_NUMBER is set to SDL_PROCESS_STDIO_REDIRECT.
    ///<br/>
    /// SDL_PROP_PROCESS_CREATE_STDERR_NUMBER: an SDL_ProcessIO value describing where standard error for the process goes to, defaults to SDL_PROCESS_STDIO_INHERITED.
    ///<br/>
    /// SDL_PROP_PROCESS_CREATE_STDERR_POINTER: an SDL_IOStream pointer used for standard error when SDL_PROP_PROCESS_CREATE_STDERR_NUMBER is set to SDL_PROCESS_STDIO_REDIRECT.
    ///<br/>
    /// SDL_PROP_PROCESS_CREATE_STDERR_TO_STDOUT_BOOLEAN: true if the error output of the process should be redirected into the standard output of the process. This property has no effect if SDL_PROP_PROCESS_CREATE_STDERR_NUMBER is set.
    ///<br/>
    /// SDL_PROP_PROCESS_CREATE_BACKGROUND_BOOLEAN: true if the process should run in the background. In this case the default input and output is SDL_PROCESS_STDIO_NULL and the exitcode of the process is not available, and will always be 0.
    ///<br/>
    /// SDL_PROP_PROCESS_CREATE_CMDLINE_STRING: a string containing the program to run and any parameters. This string is passed directly to CreateProcess on Windows, and does nothing on other platforms. This property is only important if you want to start programs that does non-standard command-line processing, and in most cases using SDL_PROP_PROCESS_CREATE_ARGS_POINTER is sufficient.
    ///<br/><br/>
    /// On POSIX platforms, wait() and waitpid(-1, ...) should not be called, and SIGCHLD should not be ignored or handled because those would prevent SDL from properly tracking the lifetime of the underlying process. You should use SDL_WaitProcess() instead.
    /// </remarks>
    public static Process? Create(Properties properties) {
        return SDL_CreateProcessWithProperties(properties);
    }

    /// <summary>
    /// Destroy a previously created process object
    /// </summary>
    /// <remarks>
    /// Note that this does not stop the process, just destroys the SDL object used to track it. If you want to stop the process you should use SDL_KillProcess().
    /// </remarks>
    public override void Dispose() {
        SDL_DestroyProcess(this);
        base.Dispose();
    }

    /// <summary>
    /// Get the <see cref="IOStream"/> associated with process standard input.
    /// </summary>
    /// <returns>the input stream</returns>
    /// <remarks>
    /// The process must have been created with <see cref="Create(System.Collections.Generic.IEnumerable{string},bool)"/> and pipeStdio set to true,
    /// or with <see cref="Create(Properties)"/> and SDL_PROP_PROCESS_CREATE_STDIN_NUMBER set to SDL_PROCESS_STDIO_APP.
    /// <br/><br/>
    /// Writing to this stream can return less data than expected if the process hasn't read its input.
    /// It may be blocked waiting for its output to be read, if so you may need to call <see cref="GetOutput"/>
    /// and read the output in parallel with writing input.
    /// </remarks>
    public IOStream Input {
        get {
            var result = SDL_GetProcessInput(this);
            if (result == null)
                throw new SdlException();
            return result;
        }
    }

    /// <summary>
    /// Get the <see cref="IOStream"/> associated with process standard output
    /// </summary>
    /// <returns>the output stream</returns>
    /// <remarks>
    /// The process must have been created with <see cref="Create(System.Collections.Generic.IEnumerable{string},bool)"/> and pipeStdio set to true,
    /// or with <see cref="Create(Properties)"/> and SDL_PROP_PROCESS_CREATE_STDOUT_NUMBER set to SDL_PROCESS_STDOUT_APP.
    /// <br/><br/>
    /// Reading from this stream can return 0 with <see cref="IOStream.Status"/> returning <see cref="IOStatus.NotReady"/> if no output is available yet.
    /// </remarks>
    public IOStream Output {
        get {
            var result = SDL_GetProcessOutput(this);
            if (result == null)
                throw new SdlException();
            return result;
        }
    }

    //TODO: Cache
    /// <summary>
    /// Get the properties associated with a process
    /// </summary>
    /// <remarks>
    /// The following read-only properties are provided by SDL:
    /// <ul>
    /// <li>SDL_PROP_PROCESS_PID_NUMBER: the process ID of the process.</li>
    /// <li>SDL_PROP_PROCESS_STDIN_POINTER: an SDL_IOStream that can be used to write input to the process,
    ///     if it was created with SDL_PROP_PROCESS_CREATE_STDIN_NUMBER set to SDL_PROCESS_STDIO_APP.</li>
    /// <li>SDL_PROP_PROCESS_STDOUT_POINTER: a non-blocking SDL_IOStream that can be used to read output from the process,
    ///     if it was created with SDL_PROP_PROCESS_CREATE_STDOUT_NUMBER set to SDL_PROCESS_STDIO_APP.</li>
    /// <li>SDL_PROP_PROCESS_STDERR_POINTER: a non-blocking SDL_IOStream that can be used to read error output from the process,
    ///     if it was created with SDL_PROP_PROCESS_CREATE_STDERR_NUMBER set to SDL_PROCESS_STDIO_APP. </li>
    /// <li>SDL_PROP_PROCESS_BACKGROUND_BOOLEAN: true if the process is running in the background. </li>
    /// </ul>
    /// </remarks>
    public Properties Properties {
        get {
            var result = SDL_GetProcessProperties(this);
            if (result == 0)
                throw new SdlException();
            return (Properties)result;
        }
    }

    /// <summary>
    /// Stop a process
    /// </summary>
    /// <param name="force">
    /// true to terminate the process immediately, false to try to stop the process gracefully.
    /// In general you should try to stop the process
    /// gracefully first as terminating a process may leave it with half-written data or in some other unstable state.
    /// </param>
    public void Kill(bool force = false) {
        if (!SDL_KillProcess(this, force))
            throw new SdlException();
    }

    /// <summary>
    /// Read all the output from a process
    /// </summary>
    /// <param name="dataSize">number of bytes read</param>
    /// <param name="exitCode">exit code if the process has exited</param>
    /// <remarks>
    /// If a process was created with I/O enabled, you can use this function to read the output. This function blocks until the process is complete, capturing all output, and providing the process exit code.
    /// <br/><br/>
    /// The data is allocated with a zero byte at the end (null terminated) for convenience. This extra byte is not included in the value reported via datasize.
    /// <br/><br/>
    /// The data should be freed with <see cref="UnmanagedMemory.Free(nint)"/>.
    /// </remarks>
    public IntPtr Read(out nuint dataSize, out int exitCode) {
        dataSize = 0;
        exitCode = -255;
        var result = SDL_ReadProcess(this, (nuint*)Unsafe.AsPointer(ref dataSize), (int*)Unsafe.AsPointer(ref exitCode));
        if (result == 0)
            throw new SdlException();
        return result;
    }
    
    public byte[] Read(out int exitCode) {
        var result = Read(out var dataSize, out exitCode);
        var array = new Span<byte>((void*)result, (int)dataSize).ToArray();
        UnmanagedMemory.Free(result);
        return array;
    }

    /// <summary>
    /// Wait for a process to finish
    /// </summary>
    /// <param name="exitCode">process exit code if the process has exited</param>
    /// <param name="block">If true, block until the process finishes; otherwise, report on the process' status</param>
    /// <returns>true if the process exited, false otherwise</returns>
    /// <remarks>
    /// This can be called multiple times to get the status of a process.
    /// <br/><br/>
    /// The exit code will be the exit code of the process if it terminates normally,
    /// a negative signal if it terminated due to a signal, or -255 otherwise. It will not be changed if the process is still running.
    /// <br/><br/>
    /// If you create a process with standard output piped to the application (pipe_stdio being true)
    /// then you should read all of the process output before calling <see cref="Wait"/>. If you don't
    /// do this the process might be blocked indefinitely waiting for output to be read and <see cref="Wait"/>
    /// will never return true;
    /// </remarks>
    public bool Wait(out int exitCode, bool block = true) {
        exitCode = -255;
        return SDL_WaitProcess(this, block, (int*)Unsafe.AsPointer(ref exitCode));
    }
}