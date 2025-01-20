using System.Drawing;

namespace Neko.Sdl;

public static class Extensions {
    public static bool ThrowIfError(this SDLBool result, string message = "") {
        if (!result) throw new SdlException(message);
        return true;
    }

    public static SDL_Rect ToSdl(this Rectangle rectangle) => new(){
        x = rectangle.X,
        y = rectangle.Y,
        h = rectangle.Height,
        w = rectangle.Width,
    };
    
    public static SDL_FRect ToSdl(this RectangleF rectangle) => new(){
        x = rectangle.X,
        y = rectangle.Y,
        h = rectangle.Height,
        w = rectangle.Width,
    };

    public static Rectangle ToClr(this SDL_Rect rectangle) => 
        new(rectangle.x, rectangle.y, rectangle.w, rectangle.h);
    public static RectangleF ToClr(this SDL_FRect rectangle) => 
        new(rectangle.x, rectangle.y, rectangle.w, rectangle.h);
}