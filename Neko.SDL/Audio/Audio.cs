using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Extra;

namespace Neko.Sdl.Audio;

public static unsafe class Audio {
    /// <summary>
    /// Convert some audio data of one format to another format
    /// </summary>
    /// <param name="srcSpec">the format details of the input audio</param>
    /// <param name="srcData">the audio data to be converted</param>
    /// <param name="dstSpec">the format details of the output audio</param>
    /// <returns>converted audio data</returns>
    /// <remarks>
    /// Please note that this function is for convenience, but should not be used to resample audio in blocks, as it
    /// will introduce audio artifacts on the boundaries. You should only use this function if you are converting audio
    /// data in its entirety in one call. If you want to convert audio in smaller chunks, use an AudioStream, which
    /// is designed for this situation.
    /// <br/><br/>
    /// Internally, this function creates and destroys an AudioStream on each use, so it's also less efficient than
    /// using one directly, if you need to convert multiple times.
    /// </remarks>
    public static byte[] ConvertSamples(ref AudioSpec srcSpec, ReadOnlySpan<byte> srcData, ref AudioSpec dstSpec) {
        byte* ptr;
        int len;
        fixed (byte* srcPtr = srcData)
            SDL_ConvertAudioSamples(
                (SDL_AudioSpec*)Unsafe.AsPointer(ref srcSpec), srcPtr, srcData.Length,
                (SDL_AudioSpec*)Unsafe.AsPointer(ref dstSpec), &ptr, &len).ThrowIfError();
        return Util.ConvertSdlArrayToManaged(ptr, (uint)len);
    }

    public class DriverAccessor {
        internal DriverAccessor() { }

        /// <summary>
        /// The number of built-in audio drivers
        /// </summary>
        /// <remarks>
        /// This function returns a hardcoded number. This never returns a negative value; if there are no drivers
        /// compiled into this build of SDL, this function returns zero. The presence of a driver in this list does not
        /// mean it will function, it just means SDL is capable of interacting with that interface. For example, a build
        /// of SDL might have esound support, but if there's no esound server available, SDL's esound driver would fail
        /// if used.
        /// <br/><br/>
        /// By default, SDL tries all drivers, in its preferred order, until one is found to be usable
        /// </remarks>
        public int Length => SDL_GetNumAudioDrivers();

        /// <summary>
        /// Use this function to get the name of a built in audio driver
        /// </summary>
        /// <param name="index">the index of the audio driver; the value ranges from 0 to <see cref="Length"/> - 1.</param>
        /// <returns>The name of the audio driver at the requested index</returns>
        /// <exception cref="ArgumentOutOfRangeException">an invalid index was specified</exception>
        /// <exception cref="ArgumentException">a result name is null</exception>
        /// <remarks>
        /// The list of audio drivers is given in the order that they are normally initialized by default; the drivers
        /// that seem more reasonable to choose first (as far as the SDL developers believe) are earlier in the list.
        /// <br/><br/>
        /// The names of drivers are all simple, low-ASCII identifiers, like "alsa", "coreaudio" or "wasapi". These
        /// never have Unicode characters, and are not meant to be proper names.
        /// </remarks>
        public string Get(int index) {
            if (index >= Length || index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            var result = SDL_GetAudioDriver(index);
            if (result is null)
                throw new ArgumentException("The provided index is invalid", nameof(index));
            return result;
        }
        
        /// <inheritdoc cref="Get"/>
        public string this[int i] => Get(i);

        /// <summary>
        /// Name of the current audio driver
        /// </summary>
        /// <remarks>
        /// The names of drivers are all simple, low-ASCII identifiers, like "alsa", "coreaudio" or "wasapi".
        /// These never have Unicode characters, and are not meant to be proper names.
        /// <br/><br/>
        /// Returns null if no driver has been initialized.
        /// </remarks>
        public string? Current => SDL_GetCurrentAudioDriver();
    }

    public static readonly DriverAccessor Driver = new();

    /// <summary>
    /// Loads a WAV from a file path
    /// </summary>
    /// <param name="path">the file path of the WAV file to open</param>
    /// <param name="spec">an AudioSpec that will be set to the WAVE data's format details on successful return</param>
    /// <returns>audio data</returns>
    /// <exception cref="SdlException">the .WAV file cannot be opened, uses an unknown data format, or is corrupt</exception>
    /// <remarks>
    /// This is a convenience function that is effectively the same as:
    /// <code>
    /// Audio.LoadWAV(IOStream.FromFile(path, "rb"), true, ref spec, audio_buf, audio_len);
    /// </code>
    /// </remarks>
    public static byte[] LoadWAV(string path, ref AudioSpec spec) {
        uint len;
        byte* buf;
        SDL_LoadWAV(path, (SDL_AudioSpec*)Unsafe.AsPointer(ref spec), &buf, &len).ThrowIfError();
        return Util.ConvertSdlArrayToManaged(buf, len);
    }

    /// <summary>
    /// Load the audio data of a WAVE file into memory
    /// </summary>
    /// <param name="src">the data source for the WAVE data</param>
    /// <param name="closeio">
    /// if true, calls <see cref="IOStream.Dispose"/> on src before returning, even in the case of an error
    /// </param>
    /// <param name="spec">an AudioSpec that will be set to the WAVE data's format details on successful return</param>
    /// <returns>audio data</returns>
    /// <remarks>
    /// Loading a WAVE file requires src, spec, audio_buf and audio_len to be valid pointers. The entire data portion of
    /// the file is then loaded into memory and decoded if necessary.
    /// <br/><br/>
    /// Supported formats are RIFF WAVE files with the formats PCM (8, 16, 24, and 32 bits), IEEE Float (32 bits),
    /// Microsoft ADPCM and IMA ADPCM (4 bits), and A-law and mu-law (8 bits). Other formats are currently unsupported
    /// and cause an error.
    /// <br/><br/>
    /// If this function succeeds, the return value is the audio data allocated by the function. The AudioSpec
    /// members Frequency, Channels, and Format are set to the values of the audio data in the buffer.
    /// <br/><br/>
    /// Because of the underspecification of the .WAV format, there are many problematic files in the wild that cause
    /// issues with strict decoders. To provide compatibility with these files, this decoder is lenient in regards to
    /// the truncation of the file, the fact chunk, and the size of the RIFF chunk. The hints
    /// <see cref="Hints.WaveRiffChunkSize"/>, <see cref="Hints.WaveTruncation"/>, and <see cref="Hints.WaveFactChunk"/>
    /// can be used to tune the behavior of the loading process.
    /// <br/><br/>
    /// Any file that is invalid (due to truncation, corruption, or wrong values in the headers), too big, or
    /// unsupported causes an error. Additionally, any critical I/O error from the data source will terminate the
    /// loading process with an error. The function thorws on error and in all cases an appropriate error message will
    /// be sent.
    /// <br/><br/>
    /// It is required that the data source supports seeking.
    /// <br/><br/>
    /// Example:
    /// <code>
    /// Audio.LoadWAV(IOStream.FromFile("sample.wav", "rb"), true, ref spec);
    /// </code>
    /// Note that the <see cref="LoadWAV(string, ref AudioSpec)"/> function does this same thing for you, but in a less
    /// messy way:
    /// <code>
    /// Audio.LoadWAV("sample.wav", ref spec);
    /// </code>
    /// </remarks>
    public static byte[] LoadWAV(IOStream src, bool closeio, ref AudioSpec spec) {
        uint len;
        byte* buf;
        try {
            SDL_LoadWAV_IO(src, false, (SDL_AudioSpec*)Unsafe.AsPointer(ref spec), &buf, &len).ThrowIfError();
        }
        finally {
            if (closeio) src.Dispose();
        }
        return Util.ConvertSdlArrayToManaged(buf, len);
    }

    /// <summary>
    /// Mix audio data in a specified format
    /// </summary>
    /// <param name="dst">the destination for the mixed audio</param>
    /// <param name="src">the source audio buffer to be mixed</param>
    /// <param name="volume">ranges from 0.0 - 1.0, and should be set to 1.0 for full audio volume</param>
    /// <param name="audio">the desired audio format</param>
    /// <exception cref="ArgumentException">the destination and source have different lengths</exception>
    public static void Mix(Span<byte> dst, ReadOnlySpan<byte> src, AudioFormat audio, float volume = 1.0f) {
        if (dst.Length != src.Length) throw new ArgumentException("The provided spans are unequal");
        fixed (byte* srcPtr = src)
        fixed (byte* dstPtr = dst)
            SDL_MixAudio(dstPtr, srcPtr, (SDL_AudioFormat)audio, (uint)dst.Length, volume).ThrowIfError();
    }

    /// <summary>
    /// Get AudioFormat for TypeCode
    /// </summary>
    public static AudioFormat GetAudioFormat(this TypeCode typeCode) => typeCode switch {
        TypeCode.SByte => AudioFormat.S8,
        TypeCode.Byte => AudioFormat.U8,
        TypeCode.Int16 => BitConverter.IsLittleEndian ? AudioFormat.S16le : AudioFormat.S16be,
        TypeCode.Int32 => BitConverter.IsLittleEndian ? AudioFormat.S32le : AudioFormat.S32be,
        TypeCode.Single => BitConverter.IsLittleEndian ? AudioFormat.F32le : AudioFormat.F32be,
        _ => throw new ArgumentOutOfRangeException(nameof(typeCode))
    };
    
    /// <summary>
    /// Mix audio data in a specified format
    /// </summary>
    /// <param name="dst">the destination for the mixed audio</param>
    /// <param name="src">the source audio buffer to be mixed</param>
    /// <param name="volume">ranges from 0.0 - 1.0, and should be set to 1.0 for full audio volume</param>
    /// <typeparam name="T">Audio format</typeparam>
    /// <exception cref="ArgumentException">the destination and source have different lengths</exception>
    /// <remarks>
    /// This takes an audio buffer src of len bytes of format data and mixes it into dst, performing addition, volume
    /// adjustment, and overflow clipping. The buffer pointed to by dst must also be len bytes of format data.
    /// <br/><br/>
    /// This is provided for convenience -- you can mix your own audio data.
    /// <br/><br/>
    /// Do not use this function for mixing together more than two streams of sample data. The output from repeated
    /// application of this function may be distorted by clipping, because there is no accumulator with greater range
    /// than the input (not to mention this being an inefficient way of doing it).
    /// <br/><br/>
    /// It is a common misconception that this function is required to write audio data to an output stream in an audio
    /// callback. While you can do that, SDL_MixAudio() is really only needed when you're mixing a single audio stream
    /// with a volume adjustment.
    /// </remarks>
    public static void Mix<T>(Span<T> dst, ReadOnlySpan<T> src, float volume = 1.0f) where T : unmanaged {
        if (dst.Length != src.Length) throw new ArgumentException("The provided spans are unequal");
        var type = typeof(T);
        var format = Type.GetTypeCode(type).GetAudioFormat();
        fixed (T* srcPtr = src)
        fixed (T* dstPtr = dst)
            SDL_MixAudio((byte*)dstPtr, (byte*)srcPtr, (SDL_AudioFormat)format, (uint)dst.Length, volume).ThrowIfError();
    }

    /// <summary>
    /// Get the human readable name of an audio format
    /// </summary>
    /// <param name="format">the audio format to query</param>
    /// <returns>Returns the human readable name of the specified audio format or "SDL_AUDIO_UNKNOWN" if the format isn't recognized</returns>
    public static string GetName(this AudioFormat format) {
        var result = SDL_GetAudioFormatName((SDL_AudioFormat)format);
        if (result is null)
            throw new SdlException();
        return result;
    }
}