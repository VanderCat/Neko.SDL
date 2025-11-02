using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using Neko.Sdl;

namespace Neko.Sdl.Ttf;

public unsafe partial class Font : SdlWrapper<TTF_Font> {
    /// <summary>
    /// This function gets the version of the dynamically linked SDL_ttf library.
    /// </summary>
    public static int Version => TTF_Version();
    /// <summary>
    /// Query the version of the FreeType library in use.
    /// </summary>
    public static Version FreeTypeVersion {
        get {
            int major;
            int minor;
            int patch;
            TTF_GetFreeTypeVersion(&major, &minor, &patch);
            return new Version(major, minor, patch);
        }
    }

    /// <summary>
    /// Query the version of the HarfBuzz library in use.
    /// </summary>
    public static Version HarfBuzzVersion {
        get {
            int major;
            int minor;
            int patch;
            TTF_GetHarfBuzzVersion(&major, &minor, &patch);
            return new Version(major, minor, patch);
        }
    }
    /// <summary>
    /// Initialize SDL_ttf.
    /// </summary>
    /// <remarks>
    /// You must successfully call this function before it is safe to call any other function in this library.
    /// <br/><br/>
    /// It is safe to call this more than once, and each successful <see cref="Init"/> call should be paired with a matching <see cref="Quit"/> call.
    /// </remarks>
    public static void Init() => TTF_Init().ThrowIfError();

    /// <summary>
    /// Create a font from a file, using a specified point size.
    /// </summary>
    /// <param name="file">path to font file</param>
    /// <param name="ptsize">point size to use for the newly-opened font</param>
    /// <returns>a valid Font</returns>
    /// <remarks>
    /// Some .fon fonts will have several sizes embedded in the file, so the point size becomes the index of choosing which size.
    /// If the value is too high, the last indexed size will be the default.
    /// <br/><br/>
    /// When done with the returned <see cref="Font"/>, use <see cref="Dispose"/> to dispose of it.
    /// </remarks>
    public static Font Open(string file, float ptsize) {
        var result = TTF_OpenFont(file, ptsize);
        if (result is null)
            throw new SdlException();
        return result;
    }

    /// <summary>
    /// Create a font from an SDL_IOStream, using a specified point size.
    /// </summary>
    /// <param name="src"></param>
    /// <param name="closeio">true to close src when the font is closed, false to leave it open</param>
    /// <param name="ptsize">point size to use for the newly-opened font</param>
    /// <returns>a valid Font</returns>
    /// <remarks>
    /// Some .fon fonts will have several sizes embedded in the file, so the point size becomes the index of choosing which size.
    /// If the value is too high, the last indexed size will be the default.
    /// <br/><br/>
    /// If closeio is true, src will be automatically closed once the font is closed. Otherwise you should keep src open until the font is closed.
    /// <br/><br/>
    /// When done with the returned <see cref="Font"/>, use <see cref="Dispose"/> to dispose of it.
    /// </remarks>
    public static Font Open(IOStream src, float ptsize, bool closeio = false) {
        try {
            var result = TTF_OpenFontIO(src, closeio, ptsize);
            if (result is null)
                throw new SdlException();
            return result;
        }
        finally {
            if (closeio) src.Dispose();
        }
    }

    /// <summary>
    /// Create a font with the specified properties.
    /// </summary>
    /// <param name="props"></param>
    /// <returns></returns>
    public static Font Open(FontCreateProperties props) => TTF_OpenFontWithProperties(props);

    /// <summary>
    /// Create a copy of an existing font.
    /// </summary>
    /// <returns>font</returns>
    /// <remarks>
    /// The copy will be distinct from the original, but will share the font file and have the same size and style as the original.
    /// <br/><br/>
    /// When done with the returned <see cref="Font"/>, use <see cref="Dispose"/> to dispose of it.
    /// </remarks>
    public Font Copy() => TTF_CopyFont(this);

    private Properties? _properties;
    
    /// <summary>
    /// Properties associated with a font.
    /// </summary>
    public Properties Properties { //TODO: font properties
        get {
            if (_properties is null)
                return _properties = (Properties)TTF_GetFontProperties(this);
            return _properties;
        }
    }

    /// <summary>
    /// Get the font generation.
    /// </summary>
    /// <remarks>
    /// The generation is incremented each time font properties change that require rebuilding glyphs, such as style, size, etc.
    /// </remarks>
    public uint Generation => TTF_GetFontGeneration(this);

    /// <summary>
    /// Add a fallback font.
    /// </summary>
    /// <param name="fallback">the font to add as a fallback</param>
    /// <remarks>
    /// Add a font that will be used for glyphs that are not in the current font. The fallback font should have the same size and style as the current font.
    /// <br/><br/>
    /// If there are multiple fallback fonts, they are used in the order added.
    /// <br/><br/>
    /// This updates any <see cref="Text"/> objects using this font.
    /// </remarks>
    public void AddFallback(Font fallback) => TTF_AddFallbackFont(this, fallback).ThrowIfError();

    /// <summary>
    /// Remove a fallback font.
    /// </summary>
    /// <param name="fallback">the font to remove as a fallback</param>
    /// <remarks>
    /// This updates any <see cref="Text"/> objects using this font.
    /// </remarks>
    public void RemoveFallback(Font fallback) => TTF_RemoveFallbackFont(this, fallback);

    /// <summary>
    /// Remove all fallback fonts.
    /// </summary>
    /// <remarks>
    /// This updates any <see cref="Text"/> objects using this font.
    /// </remarks>
    public void ClearFallback() => TTF_ClearFallbackFonts(this);

    /// <summary>
    /// Set font size dynamically with target resolutions, in dots per inch.
    /// </summary>
    /// <param name="ptsize">the new point size</param>
    /// <param name="hdpi">the target horizontal DPI</param>
    /// <param name="vdpi">the target vertical DPI</param>
    /// <remarks>
    /// This updates any <see cref="Text"/> objects using this font, and clears already-generated glyphs, if any, from the cache.
    /// </remarks>
    public void SetFontSizeDPI(float ptsize, int hdpi, int vdpi) => TTF_SetFontSizeDPI(this, ptsize, hdpi, vdpi);

    /// <summary>
    /// The size of a font.
    /// </summary>
    public float Size {
        get => TTF_GetFontSize(this);
        set => TTF_SetFontSize(this, value);
    }
    
    /// <summary>
    /// Get font target resolutions, in dots per inch.
    /// </summary>
    /// <param name="hdpi"></param>
    /// <param name="vdpi"></param>
    /// <returns></returns>
    public bool GetDPI(int *hdpi, int *vdpi) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Font's current style.
    /// </summary>
    public FontStyleFlags Style {
        get => (FontStyleFlags)TTF_GetFontStyle(this);
        set => TTF_SetFontStyle(this, (TTF_FontStyleFlags)value);
    }
    
    /// <summary>
    /// Query a font's current outline.
    /// </summary>
    public int Outline {
        get => TTF_GetFontOutline(this);
        set => TTF_SetFontOutline(this, value);
    }
    
    /// <summary>
    /// Font's current hinter setting.
    /// </summary>
    public HintingFlags Hinting {
        get => (HintingFlags)TTF_GetFontHinting(this);
        set => TTF_SetFontHinting(this, (TTF_HintingFlags)value);
    }
    
    /// <summary>
    /// The number of faces of a font.
    /// </summary>
    public int NumFaces => TTF_GetNumFontFaces(this);
    
    /// <summary>
    /// Is Signed Distance Field rendering for a font.
    /// </summary>
    public bool SDF {
        get => TTF_GetFontSDF(this);
        set => TTF_SetFontSDF(this, value);
    }
    
    /// <summary>
    /// Font's weight, in terms of the lightness/heaviness of the strokes.
    /// </summary>
    public int Weight => TTF_GetFontWeight(this);
    
    /// <summary>
    /// Query a font's current wrap alignment option.
    /// </summary>
    public HorizontalAlignment WrapAlignment {
        get => (HorizontalAlignment)TTF_GetFontWrapAlignment(this);
        set => TTF_SetFontWrapAlignment(this, (TTF_HorizontalAlignment)value);
    }

    /// <summary>
    /// The total height of a font.
    /// </summary>
    public int Height => TTF_GetFontHeight(this);
    
    /// <summary>
    /// The offset from the baseline to the top of a font.
    /// </summary>
    public int Ascent => TTF_GetFontAscent(this);
    
    /// <summary>
    /// The offset from the baseline to the bottom of a font.
    /// </summary>
    public int Descent => TTF_GetFontDescent(this);
    
    /// <summary>
    /// The spacing between lines of text for a font.
    /// </summary>
    public int LineSkip {
        get => TTF_GetFontLineSkip(this);
        set => TTF_SetFontLineSkip(this, value);
    }
    
    /// <summary>
    /// Set if kerning is enabled for a font.
    /// </summary>
    public bool Kerning {
        get => TTF_GetFontKerning(this);
        set => TTF_SetFontKerning(this, value);
    }
    
    /// <summary>
    /// Whether a font is fixed-width.
    /// </summary>
    public bool IsFixedWidth => TTF_FontIsFixedWidth(this);
    
    /// <summary>
    /// Query whether a font is scalable or not.
    /// </summary>
    public bool IsScalable => TTF_FontIsScalable(this);
    
    /// <summary>
    /// Query a font's family name.
    /// </summary>
    public string FamilyName => TTF_GetFontFamilyName(this)??"";
    
    /// <summary>
    /// Query a font's style name.
    /// </summary>
    public string StyleName => TTF_GetFontStyleName(this)??"";
    
    /// <summary>
    /// Get the direction to be used for text shaping by a font.
    /// </summary>
    public FontDirection Direction {
        get => (FontDirection)TTF_GetFontDirection(this);
        set => TTF_SetFontDirection(this, (TTF_Direction)value).ThrowIfError();
    }

    /// <summary>
    /// Get the additional character spacing in pixels to be applied between any two rendered characters.
    /// </summary>
    public int CharSpacing {
        get => TTF_GetFontCharSpacing(this);
        set => TTF_SetFontCharSpacing(this, value).ThrowIfError();
    }

    /// <summary>
    /// Convert from a 4 character string to a 32-bit tag.
    /// </summary>
    /// <param name="string"></param>
    /// <returns></returns>
    public static uint StringToTag(string @string) => TTF_StringToTag(@string);

    /// <summary>
    /// Convert from a 32-bit tag to a 4 character string.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="string"></param>
    /// <param name="size"></param>
    public static void TagToString(uint tag, out string @string, nuint size) {
        var buf = new byte[4];
        fixed(byte* ptr = buf)
            TTF_TagToString(tag, ptr, size);
        @string = Encoding.UTF8.GetString(buf);
    }

    /// <summary>
    /// The script to be used for text shaping by a font.
    /// </summary>
    public uint Script {
        get => TTF_GetFontScript(this);
        set => TTF_SetFontScript(this, value).ThrowIfError();
    }

    /// <summary>
    /// Get the script used by a 32-bit codepoint.
    /// </summary>
    /// <param name="ch"></param>
    /// <returns></returns>
    public static uint GetGlyphScript(uint ch) => TTF_GetGlyphScript(ch);

    /// <summary>
    /// Language to be used for text shaping by a font.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="language_bcp47"></param>
    /// <returns></returns>
    public string Language {
        set => TTF_SetFontLanguage(this, value).ThrowIfError();
    }

    /// <summary>
    /// Check whether a glyph is provided by the font for a UNICODE codepoint.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="ch"></param>
    /// <returns></returns>
    public bool FontHasGlyph(uint ch) => TTF_FontHasGlyph(this, ch);

    /// <summary>
    /// Get the pixel image for a UNICODE codepoint.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="ch"></param>
    /// <param name="image_type"></param>
    /// <returns></returns>
    public Surface GetGlyphImage(uint ch, out ImageType imageType) {
        TTF_ImageType i;
        var a = TTF_GetGlyphImage(this, ch, &i);
        if (a == null)
            throw new SdlException();
        imageType = (ImageType)i;
        return a;
    }

    /// <summary>
    /// Get the pixel image for a character index.
    /// </summary>
    /// <param name="glyphIndex"></param>
    /// <param name="imageType"></param>
    /// <returns></returns>
    public Surface GetGlyphImageForIndex(uint glyphIndex, out ImageType imageType) {
        TTF_ImageType i;
        var a = TTF_GetGlyphImageForIndex(this, glyphIndex, &i);
        if (a == null)
            throw new SdlException();
        imageType = (ImageType)i;
        return a;
    }

    /// <summary>
    /// Query the metrics (dimensions) of a font's glyph for a UNICODE codepoint.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="ch"></param>
    /// <param name="minx"></param>
    /// <param name="maxx"></param>
    /// <param name="miny"></param>
    /// <param name="maxy"></param>
    /// <param name="advance"></param>
    /// <returns></returns>
    public void GetGlyphMetrics(uint ch, out int minx, out int maxx, out int miny, out int maxy, out int advance) {
        int _minx, _maxx, _miny, _maxy, _advance;
        TTF_GetGlyphMetrics(this, ch, &_minx, &_maxx, &_miny, &_maxy, &_advance).ThrowIfError();
        maxx = _maxx;
        maxy = _maxy;
        minx = _minx;
        miny = _miny;
        advance = _advance;
    }

    /// <summary>
    /// Query the kerning size between the glyphs of two UNICODE codepoints.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="previous_ch"></param>
    /// <param name="ch"></param>
    /// <param name="kerning"></param>
    /// <returns></returns>
    public int GetGlyphKerning(uint previous_ch, uint ch) {
        int kerning;
        TTF_GetGlyphKerning(this, previous_ch, ch, &kerning).ThrowIfError();
        return kerning;
    }

    /// <summary>
    /// Calculate the dimensions of a rendered string of UTF-8 text.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    public Size GetStringSize(string text) {
        int w, h;
        var utf8str = text.RentUtf8();
        fixed(byte* utf8ptr = utf8str.Rented)
            TTF_GetStringSize(this, utf8ptr, (nuint)utf8str.Length, &w, &h).ThrowIfError();
        return new Size(w, h);
    }

    /// <summary>
    /// Calculate the dimensions of a rendered string of UTF-8 text.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="wrapWidth"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    public Size GetStringSize(string text, int wrapWidth) {
        int w, h;
        var utf8str = text.RentUtf8();
        fixed(byte* utf8ptr = utf8str.Rented)
            TTF_GetStringSizeWrapped(this, utf8ptr, (nuint)utf8str.Length, wrapWidth, &w, &h).ThrowIfError();
        return new Size(w, h);
    }

    /// <summary>
    /// Calculate how much of a UTF-8 string will fit in a given width.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="maxWidth"></param>
    /// <param name="measured_width"></param>
    /// <param name="measured_length"></param>
    /// <returns></returns>
    public (int width, nuint length) MeasureString(string text, int maxWidth) {
        int measuredWidth;
        nuint measuredLength;
        var utf8str = text.RentUtf8();
        fixed(byte* utf8ptr = utf8str.Rented)
            TTF_MeasureString(this, utf8ptr, (nuint)utf8str.Length, maxWidth, &measuredWidth, &measuredLength).ThrowIfError();
        return new(measuredWidth, measuredLength);
    }

    /// <summary>
    /// Render UTF-8 text at fast quality to a new 8-bit surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="fg"></param>
    /// <returns></returns>
    public Surface RenderText_Solid(byte* text, nint length, SDL_Color fg) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Render word-wrapped UTF-8 text at fast quality to a new 8-bit surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="fg"></param>
    /// <param name="wrapLength"></param>
    /// <returns></returns>
    public Surface RenderText_Solid(byte* text, nint length, SDL_Color fg, int wrapLength) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Render a single 32-bit glyph at fast quality to a new 8-bit surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="ch"></param>
    /// <param name="fg"></param>
    /// <returns></returns>
    public Surface RenderGlyph_Solid(uint ch, SDL_Color fg) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Render UTF-8 text at high quality to a new 8-bit surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="fg"></param>
    /// <param name="bg"></param>
    /// <returns></returns>
    public Surface RenderText_Shaded(byte* text, nint length, SDL_Color fg, SDL_Color bg) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Render word-wrapped UTF-8 text at high quality to a new 8-bit surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="fg"></param>
    /// <param name="bg"></param>
    /// <param name="wrap_width"></param>
    /// <returns></returns>
    public Surface RenderText_Shaded(byte* text, nint length, SDL_Color fg, SDL_Color bg, int wrap_width) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Render a single UNICODE codepoint at high quality to a new 8-bit surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="ch"></param>
    /// <param name="fg"></param>
    /// <param name="bg"></param>
    /// <returns></returns>
    public Surface RenderGlyph_Shaded(uint ch, SDL_Color fg, SDL_Color bg) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Render UTF-8 text at high quality to a new ARGB surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="fg"></param>
    /// <returns></returns>
    public Surface RenderText_Blended(byte* text, nint length, SDL_Color fg) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Render word-wrapped UTF-8 text at high quality to a new ARGB surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="fg"></param>
    /// <param name="wrap_width"></param>
    /// <returns></returns>
    public Surface RenderText_Blended(byte* text, nint length, SDL_Color fg, int wrap_width) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Render a single UNICODE codepoint at high quality to a new ARGB surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="ch"></param>
    /// <param name="fg"></param>
    /// <returns></returns>
    public Surface RenderGlyph_Blended(uint ch, SDL_Color fg) {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Render UTF-8 text at LCD subpixel quality to a new ARGB surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="fg"></param>
    /// <param name="bg"></param>
    /// <returns></returns>
    public Surface RenderText_LCD(string text, nuint length, Color fg, Color bg) => 
        TTF_RenderText_LCD(this, text, length, Unsafe.As<Color, SDL_Color>(ref fg), Unsafe.As<Color, SDL_Color>(ref bg));

    /// <summary>
    /// Render word-wrapped UTF-8 text at LCD subpixel quality to a new ARGB surface.
    /// </summary>
    /// <param name="font"></param>
    /// <param name="text"></param>
    /// <param name="length"></param>
    /// <param name="fg"></param>
    /// <param name="bg"></param>
    /// <param name="wrapWidth"></param>
    /// <returns></returns>
    public Surface RenderText_LCD(string text, nuint length, Color fg, Color bg, int wrapWidth) => 
        TTF_RenderText_LCD_Wrapped(this, text, length, Unsafe.As<Color, SDL_Color>(ref fg), Unsafe.As<Color, SDL_Color>(ref bg), wrapWidth);

    /// <summary>
    /// Render a single UNICODE codepoint at LCD subpixel quality to a new ARGB surface.
    /// </summary>
    /// <param name="ch"></param>
    /// <param name="fg"></param>
    /// <param name="bg"></param>
    /// <returns></returns>
    public Surface RenderGlyph_LCD(uint ch, Color fg, Color bg) => 
        TTF_RenderGlyph_LCD(this, ch, Unsafe.As<Color, SDL_Color>(ref fg), Unsafe.As<Color, SDL_Color>(ref bg));

    /// <summary>
    /// Dispose of a previously-created font.
    /// </summary>
    public override void Dispose() => TTF_CloseFont(this);
    
    /// <summary>
    /// Deinitialize SDL_ttf.
    /// </summary>
    public static void Quit() => TTF_Quit();
    
    public static int WasInit => TTF_WasInit();
}