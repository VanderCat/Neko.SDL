namespace Neko.Sdl.Threading;

/// <summary>
/// A class used for thread-safe initialization and shutdown
/// </summary>
/// <remarks>
/// Note that this doesn't protect any resources created during initialization,
/// or guarantee that nobody is using those resources during cleanup.
/// You should use other mechanisms to protect those, if that's a concern for your code.
/// </remarks>
public unsafe partial class InitState : SdlWrapper<SDL_InitState> {
    /// <summary>
    /// Return whether initialization should be done
    /// </summary>
    /// <remarks>
    /// This function checks the passed in state and if initialization should be done,
    /// sets the status to <see cref="InitStatus.Initializing"/> and returns true. If another thread is
    /// already modifying this state, it will wait until that's done before returning.
    ///
    /// If this function returns true, the calling code must call <see cref="SetInitialized"/> to
    /// complete the initialization.
    /// </remarks>
    public bool ShouldInit => SDL_ShouldInit(this);
    
    /// <summary>
    /// Return whether cleanup should be done
    /// </summary>
    /// <remarks>
    /// This function checks the passed in state and if cleanup should be done, sets the
    /// status to <see cref="InitStatus.Uninitializing"/> and returns true.
    ///
    /// If this function returns true, the calling code must call <see cref="SetInitialized"/> to
    /// complete the cleanup.
    /// </remarks>
    public bool ShouldQuit => SDL_ShouldQuit(this);
    
    /// <summary>
    /// Finish an initialization state transition
    /// </summary>
    /// <remarks>
    /// This function sets the status of the passed in state to <see cref="InitStatus.Initialized"/>
    /// or <see cref="InitStatus.Uninitialized"/> and allows any threads waiting for the status
    /// to proceed.
    /// </remarks>
    public void SetInitialized(bool value) => SDL_SetInitialized(this, value);

    public InitStatus Status => (InitStatus)SDL_GetAtomicInt(&Handle->status);
}