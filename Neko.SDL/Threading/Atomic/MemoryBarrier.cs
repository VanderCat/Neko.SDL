namespace Neko.Sdl.Threading.Atomic;

public static class MemoryBarrier {
    /// <summary>
    /// Insert a memory acquire barrier
    /// </summary>
    /// <remarks>
    /// Please see <see cref="Release"/> for the details on what memory barriers are and when to use them.
    /// </remarks>
    /// <seealso cref="Release"/>
    public static void Acquire() => SDL_MemoryBarrierAcquireFunction();
    
    /// <summary>
    /// Insert a memory release barrier
    /// </summary>
    /// <remarks>
    /// Memory barriers are designed to prevent reads and writes from being reordered
    /// by the compiler and being seen out of order on multi-core CPUs.
    /// <br/><br/>
    /// A typical pattern would be for thread A to write some data and a flag, and for
    /// thread B to read the flag and get the data. In this case you would insert a
    /// release barrier between writing the data and the flag, guaranteeing that the
    /// data write completes no later than the flag is written, and you would insert
    /// an acquire barrier between reading the flag and reading the data, to ensure
    /// that all the reads associated with the flag have completed.
    /// <br/><br/>
    /// In this pattern you should always see a release barrier paired with an acquire
    /// barrier and you should gate the data reads/writes with a single flag variable.
    /// <br/><br/>
    /// For more information on these semantics, take a look at the blog post:
    /// http://preshing.com/20120913/acquire-and-release-semantics
    /// </remarks>
    public static void Release() => SDL_MemoryBarrierReleaseFunction();
}