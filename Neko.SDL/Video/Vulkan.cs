namespace Neko.Sdl.Video;

[Obsolete("Unfinished")]
public unsafe class Vulkan {
    public static void LoadLibrary(string path) => SDL_Vulkan_LoadLibrary(path).ThrowIfError();
    public static void UnloadLibrary() => SDL_Vulkan_UnloadLibrary();

    public static IntPtr GetInstanceProcAddr() {
        var ptr = SDL_Vulkan_GetVkGetInstanceProcAddr();
        if (ptr is 0) throw new SdlException("");
        return ptr;
    }

    public static bool GetPresentationSupport(IntPtr vkInsance, IntPtr vkDevice, uint queueFamilyIndex) =>
        SDL_Vulkan_GetPresentationSupport((VkInstance_T*)vkInsance, (VkPhysicalDevice_T*)vkDevice, queueFamilyIndex);
}