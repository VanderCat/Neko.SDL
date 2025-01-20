namespace Neko.Sdl;

public class SdlException(string message) : Exception(message+"\n"+SDL_GetError()) {

}