namespace Neko.Sdl.Ttf;

public class FontCreateProperties : Properties {
    /// <summary>
    /// The font file to open, if an SDL_IOStream isn't being used.
    /// </summary>
    /// <remarks>
    /// This is required if <see cref="IOStream"/> and <see cref="ExistingFont"/> aren't set
    /// </remarks>
    public string? Filename {
        get => GetString(TTF_PROP_FONT_CREATE_FILENAME_STRING);
        set {
            if (value is null)
                Clear(TTF_PROP_FONT_CREATE_FILENAME_STRING);
            else
                SetString(TTF_PROP_FONT_CREATE_FILENAME_STRING, value);
        }
    }
    
    /// <summary>
    /// an IOStream containing the font to be opened
    /// </summary>
    /// <remarks>
    /// This should not be closed until the font is closed.
    /// <br/><br/>
    /// This is required if <see cref="Filename"/> and <see cref="ExistingFont"/> aren't set.
    /// </remarks>
    public unsafe IOStream? IOStream {
        get {
            var ptr = (SDL_IOStream*)GetPointer(TTF_PROP_FONT_CREATE_IOSTREAM_POINTER, 0);
            if (ptr is null)
                return null;
            return ptr;
        }
        set {
            if (value is null)
                Clear(TTF_PROP_FONT_CREATE_IOSTREAM_POINTER);
            else
                SetPointer(TTF_PROP_FONT_CREATE_IOSTREAM_POINTER, (IntPtr)value.Handle);
        }
    }

    /// <summary>
    /// The offset in the iostream for the beginning of the font, defaults to 0.
    /// </summary>
    public long? IOStreamOffset {
        get {
            var a = GetNumber(TTF_PROP_FONT_CREATE_IOSTREAM_OFFSET_NUMBER, Int64.MaxValue);
            if (a == Int64.MaxValue) return null;
            return a;
        }
        set {
            if (value is null)
                Clear(TTF_PROP_FONT_CREATE_IOSTREAM_OFFSET_NUMBER);
            else
                SetNumber(TTF_PROP_FONT_CREATE_IOSTREAM_OFFSET_NUMBER, (long)value);
        }
    }
    
    /// <summary>
    /// true if closing the font should also close the associated IOStream.
    /// </summary>
    public bool? IOStreamAutoClose {
        get => GetBoolean(TTF_PROP_FONT_CREATE_IOSTREAM_AUTOCLOSE_BOOLEAN);
        set {
            if (value is null)
                Clear(TTF_PROP_FONT_CREATE_IOSTREAM_AUTOCLOSE_BOOLEAN);
            else
                SetBoolean(TTF_PROP_FONT_CREATE_IOSTREAM_AUTOCLOSE_BOOLEAN, (bool)value);
        }
    }

    /// <summary>
    /// The point size of the font
    /// </summary>
    /// <remarks>
    /// Some .fon fonts will have several sizes embedded in the file, so
    /// the point size becomes the index of choosing which size. If the value is too high,
    /// the last indexed size will be the default.
    /// </remarks>
    public float? Size {
        get {
            var a = GetFloat(TTF_PROP_FONT_CREATE_SIZE_FLOAT, -1);
            if (a < 0) return null;
            return a;
        }
        set {
            if (value is null)
                Clear(TTF_PROP_FONT_CREATE_SIZE_FLOAT);
            else
                SetFloat(TTF_PROP_FONT_CREATE_SIZE_FLOAT, (long)value);
        }
    }

    /// <summary>
    /// The face index of the font, if the font contains multiple font faces
    /// </summary>
    public long? Face {
        get {
            var a = GetNumber(TTF_PROP_FONT_CREATE_FACE_NUMBER, -1);
            if (a == -1) return null;
            return a;
        }
        set {
            if (value is null)
                Clear(TTF_PROP_FONT_CREATE_FACE_NUMBER);
            else
                SetNumber(TTF_PROP_FONT_CREATE_FACE_NUMBER, (long)value);
        }
    }
    
    /// <summary>
    /// The horizontal DPI to use for font rendering
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="VerticalDpi"/> if set, or 72 otherwise
    /// </remarks>
    public long? HorizontalDpi {
        get {
            var a = GetNumber(TTF_PROP_FONT_CREATE_HORIZONTAL_DPI_NUMBER, -1);
            if (a == -1) return null;
            return a;
        }
        set {
            if (value is null)
                Clear(TTF_PROP_FONT_CREATE_HORIZONTAL_DPI_NUMBER);
            else
                SetNumber(TTF_PROP_FONT_CREATE_HORIZONTAL_DPI_NUMBER, (long)value);
        }
    }
    
    /// <summary>
    /// The vertical DPI to use for font rendering
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="HorizontalDpi"/> if set, or 72 otherwise
    /// </remarks>
    public long? VerticalDpi {
        get {
            var a = GetNumber(TTF_PROP_FONT_CREATE_VERTICAL_DPI_NUMBER, -1);
            if (a == -1) return null;
            return a;
        }
        set {
            if (value is null)
                Clear(TTF_PROP_FONT_CREATE_VERTICAL_DPI_NUMBER);
            else
                SetNumber(TTF_PROP_FONT_CREATE_VERTICAL_DPI_NUMBER, (long)value);
        }
    }
    
    /// <summary>
    /// An optional Font 
    /// </summary>
    /// <remarks>
    /// If set, will be used as the font data source and the initial size and style of the new font
    /// </remarks>
    public unsafe Font? ExistingFont {
        get {
            var ptr = (TTF_Font*)GetPointer(TTF_PROP_FONT_CREATE_EXISTING_FONT_POINTER, 0);
            if (ptr is null)
                return null;
            return ptr;
        }
        set {
            if (value is null)
                SetPointer(TTF_PROP_FONT_CREATE_EXISTING_FONT_POINTER, 0);
            else
                SetPointer(TTF_PROP_FONT_CREATE_EXISTING_FONT_POINTER, (IntPtr)value.Handle);
        }
    }
}