namespace Neko.Sdl.Audio;


public static class AudioFormatExt {
    /// <summary>
    /// Get the appropriate memset value for silencing an audio format
    /// </summary>
    /// <param name="format">the audio data format to query</param>
    /// <returns>byte value that can be passed to memset</returns>
    /// <remarks>
    /// The value returned by this function can be used as the second argument to memset (or SDL_memset) to set an audio
    /// buffer in a specific format to silence.
    /// </remarks>
    public static int GetSilenceValue(this AudioFormat format) => SDL_GetSilenceValueForFormat((SDL_AudioFormat)format);

    /// <summary>
    /// Determine if an AudioFormat represents floating point data
    /// </summary>
    /// <param name="format">an AudioFormat value</param>
    /// <returns></returns>
    public static bool IsFloat(this AudioFormat format) => ((uint)format & SDL_AUDIO_MASK_FLOAT) > 0;
    
    public static bool IsBigEndian(this AudioFormat format) => ((uint)format & SDL_AUDIO_MASK_BIG_ENDIAN) > 0;
    
    public static bool IsSigned(this AudioFormat format) => ((uint)format & SDL_AUDIO_MASK_SIGNED) > 0;
    
    public static bool IsInt(this AudioFormat format) => !format.IsFloat();
    
    public static bool IsLittleEndian(this AudioFormat format) => !format.IsBigEndian();

    public static bool IsUnsigned(this AudioFormat format) => !format.IsSigned();

    /// <summary>
    /// Retrieve the size, in bits, from an AudioFormat
    /// </summary>
    /// <param name="format">an AudioFormat value</param>
    /// <returns>Data size in bits</returns>
    public static uint BitSize(this AudioFormat format) => (uint)format & SDL_AUDIO_MASK_BITSIZE;
    
    /// <summary>
    /// Retrieve the size, in bytes, from an AudioFormat
    /// </summary>
    /// <param name="format">an AudioFormat value</param>
    /// <returns>Data size in bytes</returns>
    public static uint ByteSize(this AudioFormat format) => format.BitSize()/8;

    /// <summary>
    /// Define an AudioFormat value
    /// </summary>
    /// <param name="signed">1 for signed data, 0 for unsigned data</param>
    /// <param name="bigEndian">1 for bigendian data, 0 for littleendian data</param>
    /// <param name="flt">1 for floating point data, 0 for integer data</param>
    /// <param name="size">number of bits per sample</param>
    /// <returns>AudioFormat</returns>
    /// <remarks>
    /// SDL does not support custom audio formats, so this function is not of much use externally, but it can be
    /// illustrative as to what the various bits of an SDL_AudioFormat mean.
    /// <br/><br/>
    /// For example, <see cref="AudioFormat.S32le"/> looks like this:
    /// <code>
    /// AudioFormatExt.Define(1, 0, 0, 32)
    /// </code>
    /// </remarks>
    public static AudioFormat Define(ushort signed, ushort bigEndian, ushort flt, byte size)
        => (AudioFormat)(((signed & 1) << 15) | ((bigEndian & 1) << 15) | ((flt & 1) << 8) | size);
}