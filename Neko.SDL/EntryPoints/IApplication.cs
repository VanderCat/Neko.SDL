using Neko.Sdl.Events;

namespace Neko.Sdl.EntryPoints;

/// <summary>
/// Interface for <see cref="NekoSDL.EnterApp"/> apps.
/// </summary>
public interface IApplication {
    /// <summary>
    /// Event entry point
    /// </summary>
    /// <param name="event">the new event for the app to examine</param>
    /// <inheritdoc cref="Iterate"/>
    /// <remarks>
    /// This function is called as needed by SDL after <see cref="Init"/> returns <see cref="AppResult.Continue"/>.
    /// It is called once for each new event.
    /// <br/><br/>
    /// There is (currently) no guarantee about what thread this will be called from; whatever
    /// thread pushes an event onto SDL's queue will trigger this function. SDL is responsible
    /// for pumping the event queue between each call to <see cref="Iterate"/>, so in normal operation
    /// one should only get events in a serial fashion, but be careful if you have a thread
    /// that explicitly calls SDL_PushEvent. SDL itself will push events to the queue on the
    /// main thread.
    /// <br/><br/>
    /// Events sent to this function are not owned by the app; if you need to save the data,
    /// you should copy it.
    /// <br/><br/>
    /// This function should not go into an infinite mainloop; it should handle the provided
    /// event appropriately and return.
    /// <br/><br/>
    /// If this function returns <see cref="AppResult.Continue"/>, the app will continue normal operation,
    /// receiving repeated calls to <see cref="Iterate"/> and <see cref="Event"/> for the life of the
    /// program. If this function returns <see cref="AppResult.Failure"/>,, SDL will call <see cref="Quit"/> and
    /// terminate the process with an exit code that reports an error to the platform. If it
    /// returns <see cref="AppResult.Success"/>, SDL calls SDL_AppQuit and terminates with an exit code that
    /// reports success to the platform.
    /// </remarks>
    public AppResult Event(ref Event @event);
    
    /// <summary>
    /// Initial entry point
    /// </summary>
    /// <param name="args">command line arguments</param>
    /// <inheritdoc cref="Iterate"/>
    /// <remarks>
    /// This function is called by SDL once, at startup. The function should initialize whatever is necessary,
    /// possibly create windows and open audio devices, etc. args parameter work like it would
    /// with a standard "main" function.
    /// <br/><br/>
    /// This function should not go into an infinite mainloop; it should do any one-time setup it requires and
    /// then return.
    /// <br/><br/>
    /// If this function returns <see cref="AppResult.Continue"/>, the app will continue normal operation,
    /// receiving repeated calls to <see cref="Iterate"/> and <see cref="Event"/> for the life of the
    /// program. If this function returns <see cref="AppResult.Failure"/>,, SDL will call <see cref="Quit"/> and
    /// terminate the process with an exit code that reports an error to the platform. If it
    /// returns <see cref="AppResult.Success"/>, SDL calls SDL_AppQuit and terminates with an exit code that
    /// reports success to the platform.
    /// <br/><br/>
    /// This function is called by SDL on the main thread.
    /// </remarks>
    public AppResult Init(string[] args);
    
    /// <summary>
    /// Iteration entry point
    /// </summary>
    /// <returns>
    /// <see cref="AppResult.Failure"/> to terminate with an error,
    /// <see cref="AppResult.Success"/> to terminate with success,
    /// <see cref="AppResult.Continue"/> to continue.
    /// </returns>
    /// <remarks>
    /// This function is called repeatedly by SDL after <see cref="Init"/> returns <see cref="AppResult.Continue"/>. The
    /// function should operate as a single iteration the program's primary loop; it should update
    /// whatever state it needs and draw a new frame of video, usually.
    /// <br/><br/>
    /// On some platforms, this function will be called at the refresh rate of the display (which
    /// might change during the life of your app!). There are no promises made about what frequency
    /// this function might run at. You should use SDL's timer functions if you need to see how much
    /// time has passed since the last iteration.
    /// <br/><br/>
    /// There is no need to process the SDL event queue during this function; SDL will send events
    /// as they arrive in <see cref="Event"/>, and in most cases the event queue will be empty when this
    /// function runs anyhow.
    /// <br/><br/>
    /// This function should not go into an infinite mainloop; it should do one iteration of whatever
    /// the program does and return.
    /// <br/><br/>
    /// If this function returns <see cref="AppResult.Continue"/>, the app will continue normal operation,
    /// receiving repeated calls to <see cref="Iterate"/> and <see cref="Event"/> for the life of the
    /// program. If this function returns <see cref="AppResult.Failure"/>,, SDL will call <see cref="Quit"/> and
    /// terminate the process with an exit code that reports an error to the platform. If it
    /// returns <see cref="AppResult.Success"/>, SDL calls SDL_AppQuit and terminates with an exit code that
    /// reports success to the platform.
    /// <br/><br/>
    /// This function is called by SDL on the main thread.
    /// </remarks>
    public AppResult Iterate();
    
    /// <summary>
    /// Deinit entry point
    /// </summary>
    /// <param name="result">the result code that terminated the app (success or failure)</param>
    /// <remarks>
    /// This function is called once by SDL before terminating the program.
    /// <br/><br/>
    /// This function will be called in all cases, even if SDL_AppInit requests termination at startup.
    /// <br/><br/>
    /// This function should not go into an infinite mainloop; it should deinitialize any resources
    /// necessary, perform whatever shutdown activities, and return.
    /// <br/><br/>
    /// You do not need to call <see cref="Quit"/> in this function, as SDL will call it after this function
    /// returns and before the process terminates, but it is safe to do so.
    /// </remarks>
    public void Quit(AppResult result);
}