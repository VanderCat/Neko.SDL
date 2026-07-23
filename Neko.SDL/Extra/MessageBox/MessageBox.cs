using Neko.Sdl.Extra.StandardLibrary;
using Neko.Sdl.Video;

namespace Neko.Sdl.Extra.MessageBox;

/// <summary>
/// <para>
/// SDL offers a simple message box API, which is useful for simple alerts, such as informing the user when something
/// fatal happens at startup without the need to build a UI for it (or informing the user before your UI is ready).
/// </para>
/// <para>
/// These message boxes are native system dialogs where possible.
/// </para>
/// <para>
/// There is both a customizable function (<see cref="Show"/>) that offers lots of options for what to display and
/// reports on what choice the user made, and also a much-simplified version (<see cref="ShowSimple(string, string, MessageBoxFlags)"/>), merely takes
/// a text message and title, and waits until the user presses a single "OK" UI button. Often, this is all that is
/// necessary.
/// </para>
/// </summary>
public unsafe class MessageBox {
    public MessageBoxFlags Flags = MessageBoxFlags.Information;
    public Window? Window = null;
    public string Title = "";
    public string Message = "";

    public List<MessageBoxButton> Buttons = [];

    public MessageBoxColorScheme? ColorScheme = null;
    
    /// <inheritdoc cref="ShowSimple(string, string, MessageBoxFlags)"/>
    /// <param name="window">the parent window</param>
    public static void ShowSimple(string title, string message,  Window window, MessageBoxFlags flags = MessageBoxFlags.Information) {
        SDL_ShowSimpleMessageBox((SDL_MessageBoxFlags)flags, title, message, window).ThrowIfError();
    }
    /// <summary>
    /// Display a simple modal message box.
    /// </summary>
    /// <param name="title">title text.</param>
    /// <param name="message">message text.</param>
    /// <param name="flags">an <see cref="MessageBoxFlags"/> value</param>
    /// <remarks>
    /// <para>
    /// If your needs aren't complex, this function is preferred over <see cref="Show"/>.
    /// </para>
    /// <para>
    /// flags may be any of the following:
    /// </para>
    /// <ul>
    /// <li>Error: error dialog</li>
    /// <li>Warning: warning dialog</li>
    /// <li>Information: informational dialog</li>
    /// </ul>
    /// <para>
    /// This function should be called on the thread that created the parent window, or on the main thread if the
    /// messagebox has no parent. It will block execution of that thread until the user clicks a button or closes the
    /// messagebox.
    /// </para>
    /// <para>
    /// This function may be called at any time, even before <see cref="NekoSDL.Init"/>. This makes it useful for reporting errors like
    /// a failure to create a renderer or OpenGL context.
    /// </para>
    /// <para>
    /// On X11, SDL rolls its own dialog box with X11 primitives instead of a formal toolkit like GTK+ or Qt.
    /// </para>
    /// <para>
    /// Note that if <see cref="NekoSDL.Init"/> would fail because there isn't any available video target, this function is likely to
    /// fail for the same reasons. If this is a concern, check the return value from this function and fall back to
    /// writing to stderr if you can.
    /// </para>
    /// </remarks>
    public static void ShowSimple(string title, string message, MessageBoxFlags flags = MessageBoxFlags.Information) {
        SDL_ShowSimpleMessageBox((SDL_MessageBoxFlags)flags, title, message, null).ThrowIfError();
    }

    /// <summary>
    /// Create a modal message box.
    /// </summary>
    /// <returns>the user id of hit button</returns>
    /// <remarks>
    /// <para>
    /// If your needs aren't complex, it might be easier to use <see cref="ShowSimple(string, string, MessageBoxFlags)"/>.
    /// </para>
    /// <para>
    /// This function should be called on the thread that created the parent window, or on the main thread if the
    /// messagebox has no parent. It will block execution of that thread until the user clicks a button or closes the
    /// messagebox.
    /// </para>
    /// <para>
    /// This function may be called at any time, even before <see cref="NekoSDL.Init"/>. This makes it useful for
    /// reporting errors like a failure to create a renderer or OpenGL context.
    /// </para>
    /// <para>
    /// On X11, SDL rolls its own dialog box with X11 primitives instead of a formal toolkit like GTK+ or Qt.
    /// </para>
    /// <para>
    /// Note that if <see cref="NekoSDL.Init"/> would fail because there isn't any available video target, this function
    /// is likely to fail for the same reasons. If this is a concern, check the return value from this function and fall
    /// back to writing to stderr if you can.
    /// </para>
    /// </remarks>
    public int Show() {
        var native = new SDL_MessageBoxData();
        if (Window is not null)
            native.window = Window;
        using var nativeTitle = Title.RentUtf8();
        using var nativeMessage = Message.RentUtf8();
        if (ColorScheme is not null) {
            var colorScheme = new SDL_MessageBoxColorScheme();
            MessageBoxColorScheme.Populate(ref colorScheme, ref ColorScheme);
            native.colorScheme = &colorScheme;
        }

        native.flags = (SDL_MessageBoxFlags)Flags;
        native.numbuttons = Buttons.Count;
        var nativeButtons = Util.RentArray<SDL_MessageBoxButtonData>(native.numbuttons);
        for (var i = 0; i < native.numbuttons; i++) {
            var nativeButton = new SDL_MessageBoxButtonData();
            var button = Buttons[i];
            nativeButton.flags = (SDL_MessageBoxButtonFlags)button.Flags;
            nativeButton.buttonID = button.ButtonId;
            nativeButton.text = button.Text.ToUnmanagedPointer(); //allocates!
            nativeButtons[i] = nativeButton;
        }

        var success = false;
        var result = 0;
        fixed(SDL_MessageBoxButtonData* nativeButtonsPtr = nativeButtons)
        fixed(byte* nativeMessagePtr = nativeMessage)
        fixed (byte* nativeTitlePtr = nativeTitle) {
            native.title = nativeTitlePtr;
            native.message = nativeMessagePtr;
            native.buttons = nativeButtonsPtr;
            success = SDL_ShowMessageBox(&native, &result);
        }
        for (var i = 0; i < native.numbuttons; i++) 
            UnmanagedMemory.Free(nativeButtons[i].text);
        nativeButtons.Dispose();
        if (!success) throw new SdlException();
        return result;
    }
}