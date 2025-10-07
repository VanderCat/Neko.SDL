namespace Neko.Sdl.Extra.StandardLibrary;

public static class StringQuery {
    /// <summary>
    /// Query if a character is alphabetic (a letter) or a number
    /// </summary>
    /// <param name="x">character value to check</param>
    /// <returns>true if x falls within the character class, false otherwise</returns>
    /// <remarks>
    /// WARNING: Regardless of system locale, this will only treat ASCII values for English 'a-z', 'A-Z', and '0-9'
    /// as true.
    /// </remarks>
    public static bool IsAlphaNumeric(this char x) => SDL_isalnum(x) != 0;
    public static bool IsAlphabetic(this char x) => SDL_isalpha(x) != 0;
    public static bool IsBlank(this char x) => SDL_isblank(x) != 0;
    public static bool IsControl(this char x) => SDL_iscntrl(x) != 0;
    public static bool IsDigit(this char x) => SDL_isdigit(x) != 0;
    public static bool IsGraph(this char x) => SDL_isgraph(x) != 0;
    public static bool IsInf(this double x) => SDL_isinf(x) != 0;
    public static bool IsInf(this float x) => SDL_isinff(x) != 0;
    public static bool IsLower(this char x) => SDL_islower(x) != 0;
    public static bool IsNan(this double x) => SDL_isnan(x) != 0;
    public static bool IsNan(this float x) => SDL_isnanf(x) != 0;
    public static bool IsPrint(this char x) => SDL_isprint(x) != 0;
    // public static bool IsInf(this double x) => SDL_isinf(x) != 0;
    public static bool IsPunctuation(this char x) => SDL_ispunct(x) != 0;
    public static bool IsSpace(this char x) => SDL_isspace(x) != 0;
    // public static bool IsInf(this double x) => SDL_isinf(x) != 0;
    public static bool IsUpper(this char x) => SDL_isupper(x) != 0;
    public static bool IsXdigit(this char x) => SDL_isxdigit(x) != 0;
}