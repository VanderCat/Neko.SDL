using System.Numerics;
using ImGuiNET;

namespace Neko.Sdl.ImGuiBackend;

public interface IPlatformBackend : IDisposable  {
  public string? GetClipboardText();

  public void SetClipboardText(string? text);

  public bool OpenInShell(string url);
  public void SetImeData(ImGuiViewportPtr viewport, ImGuiPlatformImeDataPtr data);

  public void NewFrame();

}