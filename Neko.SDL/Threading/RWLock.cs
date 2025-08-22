namespace Neko.Sdl.Threading;

/// <summary>
/// A mutex that allows read-only threads to run in parallel
/// </summary>
/// <remarks>
/// A rwlock is roughly the same concept as SDL_Mutex, but allows
/// threads that request read-only access to all hold the lock at
/// the same time. If a thread requests write access, it will block
/// until all read-only threads have released the lock, and no one
/// else can hold the thread (for reading or writing) at the same
/// time as the writing thread.
/// <br/><br/>
/// This can be more efficient in cases where several threads need
/// to access data frequently, but changes to that data are rare.
/// <br/><br/>
/// There are other rules that apply to rwlocks that don't apply
/// to mutexes, about how threads are scheduled and when they can
/// be recursively locked. These are documented in the other
/// rwlock functions.
/// </remarks>
public unsafe partial class RWLock : SdlWrapper<SDL_RWLock> {
    /// <summary>
    /// Create a new read/write lock.
    /// </summary>
    /// <remarks>
    /// A read/write lock is useful for situations where you have
    /// multiple threads trying to access a resource that is rarely
    /// updated. All threads requesting a read-only lock will be
    /// allowed to run in parallel; if a thread requests a write
    /// lock, it will be provided exclusive access. This makes it
    /// safe for multiple threads to use a resource at the same
    /// time if they promise not to change it, and when it has to
    /// be changed, the rwlock will serve as a gateway to make
    /// sure those changes can be made safely.
    /// <br/><br/>
    /// In the right situation, a rwlock can be more efficient than
    /// a mutex, which only lets a single thread proceed at a time,
    /// even if it won't be modifying the data.
    /// <br/><br/>
    /// All newly-created read/write locks begin in the unlocked state.
    /// <br/><br/>
    /// Calls to <see cref="LockForReading"/> and <see cref="LockForWriting"/>
    /// will not return while the rwlock is locked for writing by another
    /// thread. See <see cref="TryLockForReading"/> and
    /// <see cref="TryLockForWriting"/> to attempt to lock without blocking.
    /// <br/><br/>
    /// SDL read/write locks are only recursive for read-only locks! They
    /// are not guaranteed to be fair, or provide access in a FIFO manner!
    /// They are not guaranteed to favor writers. You may not lock a rwlock
    /// for both read-only and write access at the same time from the same
    /// thread (so you can't promote your read-only lock to a write lock
    /// without unlocking first).
    /// </remarks>
    public static RWLock Create() {
        var result = SDL_CreateRWLock();
        if (result is null)
            throw new SdlException();
        return result;
    }
    
    /// <summary>
    /// Destroy a read/write lock
    /// </summary>
    /// <remarks>
    /// This function must be called on any read/write lock that is no longer
    /// needed. Failure to destroy a rwlock will result in a system memory or
    /// resource leak. While it is safe to destroy a rwlock that is unlocked,
    /// it is not safe to attempt to destroy a locked rwlock, and may result
    /// in undefined behavior depending on the platform.
    /// </remarks>
    public override void Dispose() {
        SDL_DestroyRWLock(this);
        base.Dispose();
    }
    
    /// <summary>
    /// Lock the read/write lock for read only operations
    /// </summary>
    /// <remarks>
    /// This will block until the rwlock is available, which is to say it is not
    /// locked for writing by any other thread. Of all threads waiting to lock
    /// the rwlock, all may do so at the same time as long as they are requesting
    /// read-only access; if a thread wants to lock for writing, only one may do
    /// so at a time, and no other threads, read-only or not, may hold the lock
    /// at the same time.
    /// <br/><br/>
    /// It is legal for the owning thread to lock an already-locked rwlock for
    /// reading. It must unlock it the same number of times before it is actually
    /// made available for other threads in the system (this is known as a
    /// "recursive rwlock").
    /// <br/><br/>
    /// Note that locking for writing is not recursive (this is only available to
    /// read-only locks).
    /// <br/><br/>
    /// It is illegal to request a read-only lock from a thread that already holds
    /// the write lock. Doing so results in undefined behavior. Unlock the write
    /// lock before requesting a read-only lock. (But, of course, if you have the
    /// write lock, you don't need further locks to read in any case.)
    /// <br/><br/>
    /// This function will always block until it can lock the mutex, and return
    /// with it locked.
    /// </remarks>
    public void LockForReading() => SDL_LockRWLockForReading(this);
    
    /// <summary>
    /// Lock the read/write lock for write operations
    /// </summary>
    /// <remarks>
    /// This will block until the rwlock is available, which is to say it is not
    /// locked for reading or writing by any other thread. Only one thread may hold
    /// the lock when it requests write access; all other threads, whether they
    /// also want to write or only want read-only access, must wait until the writer
    /// thread has released the lock.
    /// <br/><br/>
    /// It is illegal for the owning thread to lock an already-locked rwlock for
    /// writing (read-only may be locked recursively, writing can not). Doing so
    /// results in undefined behavior.
    /// <br/><br/>
    /// It is illegal to request a write lock from a thread that already holds a
    /// read-only lock. Doing so results in undefined behavior. Unlock the read-only
    /// lock before requesting a write lock.
    /// <br/><br/>
    /// This function will always block until it can lock the mutex, and return with
    /// it locked.
    /// </remarks>
    public void LockForWriting() => SDL_LockRWLockForWriting(this);
    
    /// <summary>
    /// Try to lock a read/write lock for reading without blocking
    /// </summary>
    /// <returns>
    /// This works just like SDL_LockRWLockForReading(), but if the rwlock is
    /// not available, then this function returns false immediately.
    /// <br/><br/>
    /// This technique is useful if you need access to a resource but don't want
    /// to wait for it, and will return to it to try again later.
    /// <br/><br/>
    /// Trying to lock for read-only access can succeed if other threads are
    /// holding read-only locks, as this won't prevent access.
    /// </returns>
    public bool TryLockForReading() => SDL_TryLockRWLockForReading(this);
    
    /// <summary>
    /// Try to lock a read/write lock for writing without blocking
    /// </summary>
    /// <remarks>
    /// This works just like <see cref="LockForWriting"/>, but if the rwlock is not
    /// available, then this function returns false immediately.
    /// <br/><br/>
    /// This technique is useful if you need exclusive access to a resource but
    /// don't want to wait for it, and will return to it to try again later.
    /// <br/><br/>
    /// It is illegal for the owning thread to lock an already-locked rwlock for
    /// writing (read-only may be locked recursively, writing can not). Doing
    /// so results in undefined behavior.
    /// <br/><br/>
    /// It is illegal to request a write lock from a thread that already holds a
    /// read-only lock. Doing so results in undefined behavior. Unlock the read-only
    /// lock before requesting a write lock.
    /// </remarks>
    public bool TryLockForWriting() => SDL_TryLockRWLockForWriting(this);
    
    /// <summary>
    /// Unlock the read/write lock
    /// </summary>
    /// <remarks>
    /// Use this function to unlock the rwlock, whether it was locked for read-only
    /// or write operations.
    /// <br/><br/>
    /// It is legal for the owning thread to lock an already-locked read-only lock.
    /// It must unlock it the same number of times before it is actually made
    /// available for other threads in the system (this is known as a "recursive
    /// rwlock").
    /// <br/><br/>
    /// It is illegal to unlock a rwlock that has not been locked by the current
    /// thread, and doing so results in undefined behavior.
    /// </remarks>
    public void Unlock() => SDL_UnlockRWLock(this);
}