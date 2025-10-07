namespace Neko.Sdl.Extra.StandardLibrary;

public static class Convert {
    // public unsafe class Context : IDisposable {
    //     internal SDL_iconv_data_t* _data;
    //
    //     public Context(string tocode, string fromcode) {
    //         _data = SDL_iconv_open(tocode, fromcode);
    //         if ((nuint)_data == SDL_ICONV_ERROR)
    //             throw new SdlException();
    //     }
    //
    //     public void Dispose() {
    //         if (SDL_iconv_close(_data) == -1)
    //             throw new SdlException();
    //     }
    // }
    //
    // public static double ToEncoding(Context context, Span<byte> inbuf) {
    //     
    // };
    //
    public static double ToDouble(string str) => SDL_atof(str);
    public static double ToInteger(string str) => SDL_atoi(str);
    //public static double ToString(int value, int radix = 10) => SDL_itoa()
    
}