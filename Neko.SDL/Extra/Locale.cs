using System.Runtime.InteropServices;

namespace Neko.Sdl.Extra;

public sealed class Locale {
    public Locale(string language, string country) {
        Language = language;
        Country = country;
    }

    private unsafe Locale(SDL_Locale locale) {
        Language = Marshal.PtrToStringUTF8((IntPtr)locale.language);
        Country = Marshal.PtrToStringUTF8((IntPtr)locale.country);
    }

    public readonly string Language;
    public readonly string? Country;

    public override string ToString() {
        return Country + "-" + Language;
    }

    public static Locale[] GetPreferred() {
        using var ptr = SDL_GetPreferredLocales();
        if (ptr is null) throw new SdlException("Failed to get preferred locales");
        var locales = new Locale[ptr.Count];
        for (var i = 0; i < ptr.Count; i++) {
            locales[i] = new(ptr[i]);
        }
        return locales;
    }
}