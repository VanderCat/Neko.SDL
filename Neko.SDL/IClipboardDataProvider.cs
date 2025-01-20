namespace Neko.Sdl;

public interface IClipboardDataProvider {
    public void CleanupClipboardData();
    public byte[] GetClipboardData(string? mimeType);
    public string[] MimeTypes { get; }
}