using System.Text;

namespace Neko.Sdl.Extra;

/// <summary>
/// <para>
/// A GUID is a 128-bit value that represents something that is uniquely identifiable by this value: "globally unique."
/// </para>
/// <para>
/// SDL provides functions to convert a GUID to/from a string.
/// </para>
/// </summary>
public static unsafe class GUID {
    /// <summary>
    /// Convert a GUID string into a Guid structure.
    /// </summary>
    /// <param name="pchGUID">string containing an ASCII representation of a GUID.</param>
    /// <returns>Returns a <see cref="Guid"/> structure.</returns>
    /// <remarks>
    /// Performs no error checking. If this function is given a string containing an invalid GUID, the function will
    /// silently succeed, but the GUID generated will not be useful.
    /// </remarks>
    public static Guid FromString(string pchGUID) {
        using var rentedGuid = pchGUID.RentUtf8();
        SDL_GUID meow;
        fixed (byte* guidPtr = rentedGuid)
            meow = SDL_StringToGUID(guidPtr);
        return new Guid(meow.data);
    }

    /// <summary>
    /// Get an ASCII string representation for a given Guid.
    /// </summary>
    /// <param name="guid">guid you wish to convert to string</param>
    /// <returns>converted string</returns>
    public static string ToString(Guid guid) {
        using var arr = Util.RentArray<byte>(33);
        fixed(byte* arrPtr = arr)
            SDL_GUIDToString((SDL_GUID)(object)guid, arrPtr, 33);
        return Encoding.ASCII.GetString(arr);
    }
}