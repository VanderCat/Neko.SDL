using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl.Filesystem;

/// <summary>
/// SDL offers an API for examining and manipulating the system's filesystem. This covers most
/// things one would need to do with directories, except for actual file I/O
/// (which is covered by <see cref="IOStream"/> and AsyncIO instead).
/// <br/><br/>
/// There are functions to answer necessary path questions:
/// <ul>
/// <li>Where is my app's data? <see cref="BasePath"/>.</li>
/// <li>Where can I safely write files? <see cref="GetPrefPath"/>.</li>
/// <li>Where are paths like Downloads, Desktop, Music? <see cref="GetUserFolder"/>.</li>
/// <li>What is this thing at this location? <see cref="GetPathInfo"/>.</li>
/// <li>What items live in this folder? <see cref="EnumerateDirectory"/>.</li>
/// <li>What items live in this folder by wildcard? <see cref="GlobDirectory"/>.</li>
/// <li>What is my current working directory? <see cref="CurrentDirectory"/>.</li>
/// </ul>
/// SDL also offers functions to manipulate the directory tree: renaming, removing, copying files.
/// </summary>
public class Filesystem {
    /// <summary>
    /// Copy a file
    /// </summary>
    /// <param name="oldpath">the old path</param>
    /// <param name="newpath">the new path</param>
    /// <remarks>
    /// If the file at newpath already exists, it will be overwritten with the contents of the
    /// file at oldpath.
    /// <br/><br/>
    /// This function will block until the copy is complete, which might be a significant time
    /// for large files on slow disks. On some platforms, the copy can be handed off to the OS
    /// itself, but on others SDL might just open both paths, and read from one and write to
    /// the other.
    /// <br/><br/>
    /// Note that this is not an atomic operation! If something tries to read from newpath
    /// while the copy is in progress, it will see an incomplete copy of the data, and if the
    /// calling thread terminates (or the power goes out) during the copy, newpath's previous
    /// contents will be gone, replaced with an incomplete copy of the data. To avoid this
    /// risk, it is recommended that the app copy to a temporary file in the same directory as
    /// newpath, and if the copy is successful, use SDL_RenamePath() to replace newpath with
    /// the temporary file. This will ensure that reads of newpath will either see a complete
    /// copy of the data, or it will see the pre-copy state of newpath.
    /// <br/><br/>
    /// This function attempts to synchronize the newly-copied data to disk before returning,
    /// if the platform allows it, so that the renaming trick will not have a problem in a
    /// system crash or power failure, where the file could be renamed but the contents never
    /// made it from the system file cache to the physical disk.
    /// <br/><br/>
    /// If the copy fails for any reason, the state of newpath is undefined. It might be half a
    /// copy, it might be the untouched data of what was already there, or it might be a
    /// zero-byte file, etc.
    /// </remarks>
    public static void CopyFile(string oldpath, string newpath) =>
        SDL_CopyFile(oldpath, newpath).ThrowIfError();

    /// <summary>
    /// Create a directory, and any missing parent directories
    /// </summary>
    /// <param name="path">the path of the directory to create</param>
    /// <remarks>
    /// This reports success if path already exists as a directory.
    /// <br/><br/>
    /// If parent directories are missing, it will also create them. Note that if this fails,
    /// it will not remove any parent directories it already made.
    /// </remarks>
    public static void CreateDirectory(string path) =>
        SDL_CreateDirectory(path);
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static unsafe SDL_EnumerationResult EnumerateDirectoryNative(IntPtr userdata, byte* dirname, byte* fname) {
        var meow = userdata.AsPin<EnumerateDirectoryCallback>();
        return (SDL_EnumerationResult)meow.Target(Marshal.PtrToStringUTF8((IntPtr)dirname)??"", Marshal.PtrToStringUTF8((IntPtr)fname)??"");
    }

    /// <summary>
    /// Callback for directory enumeration
    /// </summary>
    /// <param name="dirname">the directory that is being enumerated</param>
    /// <param name="fname">the next entry in the enumeration</param>
    /// <returns></returns>
    /// <remarks>
    /// Enumeration of directory entries will continue until either all entries have been provided
    /// to the callback, or the callback has requested a stop through its return value.
    /// <br/><br/>
    /// Returning <see cref="EnumerationResult.Continue"/> will let enumeration proceed, calling the callback with further
    /// entries. <see cref="EnumerationResult.Success"/> and <see cref="EnumerationResult.Failure"/> will terminate the enumeration early, and
    /// dictate the return value of the enumeration function itself.
    /// <br/><br/>
    /// dirname is guaranteed to end with a path separator ('\' on Windows, '/' on most other
    /// platforms).
    /// </remarks>
    public delegate EnumerationResult EnumerateDirectoryCallback(string dirname, string fname);

    /// <summary>
    /// Enumerate a directory through a callback function
    /// </summary>
    /// <param name="path">the path of the directory to enumerate</param>
    /// <param name="callback">a function that is called for each entry in the directory</param>
    /// <remarks>
    /// This function provides every directory entry through an app-provided callback, called once for each
    /// directory entry, until all results have been provided or the callback returns either <see cref="EnumerationResult.Success"/>
    /// or <see cref="EnumerationResult.Failure"/>.
    /// <br/><br/>
    /// This will throw Exception if there was a system problem in general, or if a callback returns
    /// <see cref="EnumerationResult.Error"/>. A successful return means a callback returned <see cref="EnumerationResult.Success"/> to halt
    /// enumeration, or all directory entries were enumerated.
    /// </remarks>
    public static unsafe void EnumerateDirectory(string path, EnumerateDirectoryCallback callback) {
        using var pin = callback.Pin(GCHandleType.Normal);
        SDL_EnumerateDirectory(path, &EnumerateDirectoryNative, pin.Pointer);
    }

    /// <summary>
    /// An absolute path to the application data directory
    /// </summary>
    /// <remarks>
    /// SDL caches the result of this call internally, but the first call to this function is not
    /// necessarily fast, so plan accordingly.
    /// <br/><br/>
    /// <b>macOS and iOS Specific Functionality</b>: If the application is in a ".app" bundle,
    /// this function returns the Resource directory (e.g. MyApp.app/Contents/Resources/). This
    /// behaviour can be overridden by adding a property to the Info.plist file. Adding a string
    /// key with the name SDL_FILESYSTEM_BASE_DIR_TYPE with a supported value will change the
    /// behaviour.
    /// <br/><br/>
    /// Supported values for the SDL_FILESYSTEM_BASE_DIR_TYPE property (Given an application in
    /// /Applications/SDLApp/MyApp.app):
    /// <ul>
    /// <li>resource: bundle resource directory (the default). For example: /Applications/SDLApp/MyApp.app/Contents/Resources</li>
    /// <li>bundle: the Bundle directory. For example: /Applications/SDLApp/MyApp.app/</li>
    /// <li>parent: the containing directory of the bundle. For example: /Applications/SDLApp/</li>
    /// </ul>
    /// <b>Nintendo 3DS Specific Functionality</b>: This function returns "romfs" directory of
    /// the application as it is uncommon to store resources outside the executable. As such it
    /// is not a writable directory.
    /// <br/><br/>
    /// The returned path is guaranteed to end with a path separator ('\' on Windows, '/' on
    /// most other platforms).
    /// </remarks>
    public static string BasePath {get {
        var result = SDL_GetBasePath();
        if (result is null)
            throw new SdlException();
        return result;
    }}

    /// <summary>
    /// Get what the system believes is the "current working directory"
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// For systems without a concept of a current working directory, this will still attempt
    /// to provide something reasonable.
    /// <br/><br/>
    /// SDL does not provide a means to change the current working directory; for platforms
    /// without this concept, this would cause surprises with file access outside of SDL.
    /// <br/><br/>
    /// The returned path is guaranteed to end with a path separator ('\' on Windows, '/' on
    /// most other platforms).
    /// </remarks>
    public static string CurrentDirectory {get {
        var result = SDL_GetCurrentDirectory();
        if (result is null)
            throw new SdlException();
        return result;
    }}

    /// <summary>
    /// Get information about a filesystem path
    /// </summary>
    /// <param name="path">the path to query</param>
    /// <returns>information about the path</returns>
    public static unsafe PathInfo GetPathInfo(string path) {
        var pathInfo = new PathInfo();
        SDL_GetPathInfo(path, (SDL_PathInfo*)Unsafe.AsPointer(ref pathInfo)).ThrowIfError();
        return pathInfo;
    }

    /// <summary>
    /// Get the user-and-app-specific path where files can be written.
    /// </summary>
    /// <returns>a string of the user directory in platform-dependent notation</returns>
    /// <remarks>
    /// Get the "pref dir". This is meant to be where users can write personal files (preferences
    /// and save games, etc) that are specific to your application. This directory is unique per
    /// user, per application.
    /// <br/><br/>
    /// This function will decide the appropriate location in the native filesystem, create the
    /// directory if necessary, and return a string of the absolute path to the directory in
    /// UTF-8 encoding.
    /// <br/><br/>
    /// On Windows, the string might look like:
    /// <code>
    /// C:\\Users\\bob\\AppData\\Roaming\\My Company\\My Program Name\\
    /// </code>
    /// On Linux, the string might look like:
    /// <code>
    /// /home/bob/.local/share/My Program Name/
    /// </code>
    /// On macOS, the string might look like:
    /// <code>
    /// /Users/bob/Library/Application Support/My Program Name/
    /// </code>
    /// You should assume the path returned by this function is the only safe place to write files
    /// (and that SDL_GetBasePath(), while it might be writable, or even the parent of the returned
    /// path, isn't where you should be writing things).
    /// <br/><br/>
    /// Both the org and app strings may become part of a directory name, so please follow these rules:
    /// <ul>
    /// <li>Try to use the same org string (including case-sensitivity) for all your applications that use this function.</li>
    /// <li>Always use a unique app string for each one, and make sure it never changes for an app once you've decided on it.</li>
    /// <li>Unicode characters are legal, as long as they are UTF-8 encoded, but...</li>
    /// <li>...only use letters, numbers, and spaces. Avoid punctuation like "Game Name 2: Bad Guy's Revenge!" ... "Game Name 2" is sufficient.</li>
    /// </ul>
    /// Due to historical mistakes, org is allowed to be NULL or "". In such cases, SDL will omit the org subdirectory, including on platforms where it shouldn't, and including on platforms where this would make your app fail certification for an app store. New apps should definitely specify a real string for org.
    /// <br/><br/>
    /// The returned path is guaranteed to end with a path separator ('\' on Windows, '/' on most other platforms).
    /// </remarks>
    public static string GetPrefPath(string org, string app) {
        var result = SDL_GetPrefPath(org, app);
        if (result is null)
            throw new SdlException();
        return result;
    }

    /// <summary>
    /// Finds the most suitable user folder for a specific purpose
    /// </summary>
    /// <param name="folder"></param>
    /// <returns>string containing the full path to the folder</returns>
    /// <remarks>
    /// Many OSes provide certain standard folders for certain purposes, such as storing pictures,
    /// music or videos for a certain user. This function gives the path for many of those special
    /// locations.
    /// <br/><br/>
    /// This function is specifically for user folders, which are meant for the user to access
    /// and manage. For application-specific folders, meant to hold data for the application to
    /// manage, see <see cref="BasePath"/> and <see cref="GetPrefPath"/>.
    /// <br/><br/>
    /// The returned path is guaranteed to end with a path separator ('\' on Windows, '/' on
    /// most other platforms).
    /// </remarks>
    public static string GetUserFolder(Folder folder) {
        var result = SDL_GetUserFolder((SDL_Folder)folder);
        if (result is null)
            throw new SdlException();
        return result;
    }

    /// <summary>
    /// Enumerate a directory tree, filtered by pattern, and return a list
    /// </summary>
    /// <param name="path">the path of the directory to enumerate</param>
    /// <param name="pattern">the pattern that files in the directory must match</param>
    /// <param name="caseInsensitive">weather the pattern matching case-insensitive</param>
    /// <returns>an array of strings</returns>
    /// <remarks>
    /// Files are filtered out if they don't match the string in pattern, which may contain wildcard
    /// characters * (match everything) and ? (match one character). If pattern is NULL, no filtering
    /// is done and all results are returned. Subdirectories are permitted, and are specified with a
    /// path separator of /. Wildcard characters * and ? never match a path separator.
    /// </remarks>
    public static unsafe string[] GlobDirectory(string path, string? pattern, bool caseInsensitive = false) {
        var count = 0;
        var dirPtrPtr = SDL_GlobDirectory(path, pattern, (SDL_GlobFlags)(caseInsensitive ? 1 : 0), &count);
        var dir = new string[count];
        for (var i = 0; i < count; i++)
            dir[0] = Marshal.PtrToStringUTF8((IntPtr)dirPtrPtr[i]) ?? "";
        UnmanagedMemory.Free(dirPtrPtr);
        return dir;
    }

    /// <summary>
    /// Remove a file or an empty directory
    /// </summary>
    /// <param name="path">the path to remove from the filesystem</param>
    /// <remarks>
    /// Directories that are not empty will fail; this function will not recursely delete directory trees.
    /// </remarks>
    public static void RemovePath(string path) => SDL_RemovePath(path).ThrowIfError();

    /// <summary>
    /// Rename a file or directory
    /// </summary>
    /// <param name="oldpath">the old path</param>
    /// <param name="newpath">the new path</param>
    /// <remarks>
    /// If the file at newpath already exists, it will be replaced.
    /// <br/><br/>
    /// Note that this will not copy files across filesystems/drives/volumes, as that is
    /// a much more complicated (and possibly time-consuming) operation.
    /// <br/><br/>
    /// Which is to say, if this function fails, <see cref="CopyFile"/> to a temporary file in the
    /// same directory as newpath, then <see cref="RenamePath"/> from the temporary file to newpath
    /// and <see cref="RemovePath"/> on oldpath might work for files. Renaming a non-empty directory
    /// across filesystems is dramatically more complex, however.
    /// </remarks>
    public static void RenamePath(string oldpath, string newpath) => SDL_RenamePath(oldpath, newpath).ThrowIfError();
    
    /// <summary>
    /// Save all the data into a file path
    /// </summary>
    /// <param name="file"> the path to write all available data into</param>
    /// <param name="data">the data to be written№</param>
    public static unsafe void SaveFile(string file, byte[] data) {
        fixed (byte* ptr = data)
            SDL_SaveFile(file, (IntPtr)ptr, (nuint)data.Length).ThrowIfError();
    }
    
    /// <summary>
    /// Load all the data from a file path
    /// </summary>
    /// <param name="file">the path to read all available data from</param>
    /// <returns>the data</returns>
    public static unsafe byte[] LoadFile(string file) {
        nuint nya = 0;
        var meow = SDL_LoadFile(file, &nya);
        var arr = new byte[nya];
        fixed(byte* ptr = arr)
            Unsafe.CopyBlock(ptr, (void*)meow, (uint)nya);
        UnmanagedMemory.Free(meow);
        return arr;
    }
}