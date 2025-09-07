namespace Neko.Sdl;

public interface IClipboardDataProvider {
    /// <summary>
    /// Callback function that will be called when the clipboard is cleared, or when new data is set
    /// </summary>
    public void CleanupClipboardData();
    
    /// <summary>
    /// Callback function that will be called when data for the specified mime-type is requested by the OS
    /// </summary>
    /// <param name="mimeType">the requested mime-type</param>
    /// <returns>the data for the provided mime-type</returns>
    /// <remarks>
    /// The callback function is called with NULL as the mime_type when the clipboard is cleared
    /// or new data is set. The clipboard is automatically cleared in SDL_Quit().
    /// </remarks>
    public byte[] GetClipboardData(string? mimeType);
    
    /// <summary>
    /// A list of mime-types that are being offered
    /// </summary>
    public string[] MimeTypes { get; }
}