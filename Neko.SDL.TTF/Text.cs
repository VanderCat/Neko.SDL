namespace Neko.Sdl.Ttf;

public sealed unsafe partial class Text : SdlWrapper<TTF_Text> {
    /// <summary>
    /// Draw text to an SDL renderer.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static void DrawRenderer(Text text, float x, float y) {
        TTF_DrawRendererText(text, x, y).ThrowIfError();
    }
    
    /// <summary>
    /// Get the geometry data needed for drawing the text.
    /// </summary>
    /// <returns></returns>
    // public GPUAtlasDrawSequence* GetGPUDrawData() {
    //     throw new NotImplementedException();
    // }

    /// <summary>
    /// Get the properties associated with a text object.
    /// </summary>
    public Properties Properties { get; }
    /// <summary>
    /// Set the text engine used by a text object.
    /// </summary>
    public TextEngine Engine { get; set; }

    /// <summary>
    /// Get the font used by a text object.
    /// </summary>
    public Font Font { get; set; }
    /// <summary>
    /// Get the direction to be used for text shaping a text object.
    /// </summary>
    /// <returns></returns>
    public FontDirection GetDirection() {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get the script used for text shaping a text object.
    /// </summary>
    /// <returns></returns>
    public uint GetScript() {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Set the color of a text object.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public void SetColor(ColorF color) {
        TTF_SetTextColorFloat(this, color.R, color.G, color.B, color.A).ThrowIfError();   
    }
    /// <summary>
    /// Get the color of a text object.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public bool GetColor(byte *r, byte *g, byte *b, byte *a) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get the position of a text object.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool GetPosition(int *x, int *y) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get whether wrapping is enabled on a text object.
    /// </summary>
    /// <param name="wrap_width"></param>
    /// <returns></returns>
    public bool GetWrapWidth(int *wrap_width) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Set whether whitespace should be visible when wrapping a text object.
    /// </summary>
    /// <param name="visible"></param>
    /// <returns></returns>
    public bool SetWrapWhitespaceVisible(bool visible) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Return whether whitespace is shown when wrapping a text object.
    /// </summary>
    /// <returns></returns>
    public bool WrapWhitespaceVisible() {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Set the UTF-8 text used by a text object.
    /// </summary>
    /// <returns></returns>
    public bool SetString(byte* @string, nint length) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Insert UTF-8 text into a text object.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="???"></param>
    /// <returns></returns>
    public bool InsertString(int offset, byte* @string, nint length) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Append UTF-8 text to a text object.
    /// </summary>
    /// <returns></returns>
    public bool AppendString(byte* @string, nint length) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Delete UTF-8 text from a text object.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public bool DeleteString(int offset, int length) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get the size of a text object.
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    public bool Size(int *w, int *h) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get the substring of a text object that surrounds a text offset.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="substring"></param>
    /// <returns></returns>
    public bool GetSubString(int offset, SubString *substring) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get the substring of a text object that contains the given line.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="substring"></param>
    /// <returns></returns>
    public bool GetSubStringForLine(int line, SubString *substring) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get the substrings of a text object that contain a range of text.
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public SubString ** GetSubStringsForRange(int offset, int length, int *count) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get the portion of a text string that is closest to a point.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="substring"></param>
    /// <returns></returns>
    public bool GetSubStringForPoint(int x, int y, SubString *substring) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get the previous substring in a text object
    /// </summary>
    /// <returns></returns>
    public bool GetPreviousSubString(SubString *substring, SubString *previous) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Get the next substring in a text object
    /// </summary>
    /// <returns></returns>
    public bool GetNextSubString(SubString *substring, SubString *next) {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Update the layout of a text object.
    /// </summary>
    /// <returns></returns>
    public bool Update() {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Destroy a text object created by a text engine.
    /// </summary>
    public void Destroy() {
        throw new NotImplementedException();
    }
}