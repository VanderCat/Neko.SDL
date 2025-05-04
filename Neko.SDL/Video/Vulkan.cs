using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Extra;

namespace Neko.Sdl.Video;

/// <summary>
/// Functions for creating Vulkan surfaces on SDL windows.<br/><br/>
/// For the most part, Vulkan operates independent of SDL, but it benefits from a little support during setup.<br/><br/>
/// Use GetInstanceExtensions() to get platform-specific bits for creating a VkInstance, then
/// GetVkGetInstanceProcAddr() to get the appropriate function for querying Vulkan entry points.
/// Then CreateSurface() will get you the final pieces you need to prepare for rendering into an SDL_Window with Vulkan.
/// <br/><br/>
/// Unlike OpenGL, most of the details of "context" creation and window buffer swapping are handled by the Vulkan
/// API directly, so SDL doesn't provide Vulkan equivalents of Gl.SwapWindow(), etc; they aren't necessary.
/// </summary>
public static unsafe class Vulkan {
    /// <summary>
    /// Dynamically load the Vulkan loader library.
    /// </summary>
    /// <remarks>
    /// This should be called after initializing the video driver, but before creating any Vulkan windows. If no Vulkan
    /// loader library is loaded, the default library will be loaded upon creation of the first Vulkan window.
    /// <br/><br/>
    /// SDL keeps a counter of how many times this function has been successfully called, so it is safe to call this
    /// function multiple times, so long as it is eventually paired with an equivalent number of calls to
    /// Vulkan.UnloadLibrary. The path argument is ignored unless there is no library currently loaded, and the
    /// library isn't actually unloaded until there have been an equivalent number of calls to Vulkan.UnloadLibrary.
    /// <br/><br/>
    /// If you specify a path, an application should retrieve all of the Vulkan functions it uses from the
    /// dynamic library using SDL_Vulkan_GetVkGetInstanceProcAddr unless you can guarantee path points to the same
    /// vulkan loader library the application linked to.
    /// <br/><br/>
    /// On Apple devices, if path is NULL, SDL will attempt to find the vkGetInstanceProcAddr address within all the
    /// Mach-O images of the current process. This is because it is fairly common for Vulkan applications to link with
    /// libvulkan (and historically MoltenVK was provided as a static library). If it is not found, on macOS, SDL will
    /// attempt to load vulkan.framework/vulkan, libvulkan.1.dylib, MoltenVK.framework/MoltenVK, and libMoltenVK.dylib,
    /// in that order. On iOS, SDL will attempt to load libMoltenVK.dylib. Applications using a dynamic framework or
    /// .dylib must ensure it is included in its application bundle.
    /// <br/><br/>
    /// This function is not thread safe
    /// </remarks>
    public static void LoadLibrary() => SDL_Vulkan_LoadLibrary((byte*)null).ThrowIfError();
    
    /// <inheritdoc cref="LoadLibrary()"/>
    /// <param name="path">the platform dependent Vulkan loader library name</param>
    public static void LoadLibrary(string path) => SDL_Vulkan_LoadLibrary(path).ThrowIfError();
    
    /// <summary>
    /// Unload the Vulkan library previously loaded by SDL_Vulkan_LoadLibrary()
    /// </summary>
    /// <remarks>
    /// SDL keeps a counter of how many times this function has been called, so it is safe to call this function
    /// multiple times, so long as it is paired with an equivalent number of calls to Vulkan.LoadLibrary. The
    /// library isn't actually unloaded until there have been an equivalent number of calls to Vulkan.UnloadLibrary.
    /// <br/><br/>
    /// Once the library has actually been unloaded, if any Vulkan instances remain, they will likely crash the program.
    /// Clean up any existing Vulkan resources, and destroy appropriate windows, renderers and GPU devices before
    /// calling this function.
    /// </remarks>
    public static void UnloadLibrary() => SDL_Vulkan_UnloadLibrary();

    /// <summary>
    /// Get the address of the vkGetInstanceProcAddr function.
    /// </summary>
    /// <returns>the function pointer for vkGetInstanceProcAddr</returns>
    /// <exception cref="SdlException">Failed to get address</exception>
    public static IntPtr GetVkGetInstanceProcAddr() {
        var ptr = SDL_Vulkan_GetVkGetInstanceProcAddr();
        if (ptr is 0) throw new SdlException("");
        return ptr;
    }

    /// <summary>
    /// Get the Vulkan instance extensions needed for vkCreateInstance
    /// </summary>
    /// <returns>An array of extension name strings on success</returns>
    /// <exception cref="SdlException">Failed to get extensions</exception>
    /// <remarks>
    /// This should be called after either calling Vulkan.LoadLibrary() or creating a Window with the WindowFlags.Vulkan flag.
    /// <br/><br/>
    /// On return, the variable pointed to by count will be set to the number of elements returned, suitable for using
    /// with VkInstanceCreateInfo.enabledExtensionCount, and the returned array can be used with
    /// VkInstanceCreateInfo.ppEnabledExtensionNames, for calling Vulkan's vkCreateInstance API.
    /// </remarks>
    public static string[] GetInstanceExtension() {
        uint size = 0;
        var ptr = SDL_Vulkan_GetInstanceExtensions((uint*)Unsafe.AsPointer(ref size));
        if (ptr is null) throw new SdlException();
        var arr = new string[size];
        var span = new Span<IntPtr>(ptr, (int)size);
        for (var i = 0; i < span.Length; i++) {
            arr[i] = Marshal.PtrToStringUTF8(span[i]) ?? string.Empty;
        }
        return arr;
    }
    
    /// <summary>
    /// Query support for presentation via a given physical device and queue family
    /// </summary>
    /// <param name="vkInsance">the Vulkan instance handle</param>
    /// <param name="vkDevice">a valid Vulkan physical device handle</param>
    /// <param name="queueFamilyIndex">a valid queue family index for the given physical device</param>
    /// <returns>true if supported, false if unsupported or an error occurred.</returns>
    /// <remarks>
    /// The instance must have been created with extensions returned by Vulkan.GetInstanceExtensions() enabled.
    /// </remarks>
    public static bool GetPresentationSupport(IntPtr vkInsance, IntPtr vkDevice, uint queueFamilyIndex) =>
        SDL_Vulkan_GetPresentationSupport((VkInstance_T*)vkInsance, (VkPhysicalDevice_T*)vkDevice, queueFamilyIndex);

    /// <summary>
    /// Create a Vulkan rendering surface for a window
    /// </summary>
    /// <param name="window">the window to which to attach the Vulkan surface</param>
    /// <param name="vkInstance">the Vulkan instance handle</param>
    /// <param name="vkSurfaceKhr">a pointer to a VkSurfaceKHR handle to output the newly created surface</param>
    /// <param name="vkAllocationCallbacks">
    /// a VkAllocationCallbacks struct, which lets the app set the allocator that creates the surface. Can be NULL.
    /// </param>
    /// <remarks>
    /// The window must have been created with the SDL_WINDOW_VULKAN flag and instance must have been created with
    /// extensions returned by Vulkan.GetInstanceExtensions() enabled.
    /// <br/><br/>
    /// If allocator is NULL, Vulkan will use the system default allocator. This argument is passed directly to Vulkan
    /// and isn't used by SDL itself.
    /// </remarks>
    public static void CreateVkSurface(this Window window, IntPtr vkInstance, ref IntPtr vkSurfaceKhr, IntPtr vkAllocationCallbacks = 0) => 
        SDL_Vulkan_CreateSurface(window, (VkInstance_T*)vkInstance,
        (VkAllocationCallbacks*)vkAllocationCallbacks, (VkSurfaceKHR_T**)Unsafe.AsPointer(ref vkSurfaceKhr)).ThrowIfError();
    
    /// <summary>
    /// Destroy the Vulkan rendering surface of a window
    /// </summary>
    /// <param name="vkInstance">the Vulkan instance handle</param>
    /// <param name="vkSurfaceKhr">vkSurfaceKHR handle to destroy</param>
    /// <param name="vkAllocationCallbacks">
    /// a VkAllocationCallbacks struct, which lets the app set the allocator that destroys the surface. Optional
    /// </param>
    /// <remarks>
    /// This should be called before Window.Destroy(), if Vulkan.CreateVkSurface() was called after new Window()
    /// <br/><br/>
    /// The instance must have been created with extensions returned by Vulkan.GetInstanceExtensions() enabled and
    /// surface must have been created successfully by an Vulkan.CreateSurface() call.
    /// <br/><br/>
    /// If allocator is NULL, Vulkan will use the system default allocator. This argument is passed directly to Vulkan
    /// and isn't used by SDL itself.
    /// </remarks>
    public static void DestroySurface(IntPtr vkInstance, IntPtr vkSurfaceKhr, IntPtr vkAllocationCallbacks = 0) => 
        SDL_Vulkan_DestroySurface((VkInstance_T*)vkInstance,
            (VkSurfaceKHR_T*)vkSurfaceKhr, (VkAllocationCallbacks*)vkAllocationCallbacks);
}