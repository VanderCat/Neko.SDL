namespace Neko.Sdl.Audio;

/// <summary>
/// Format specifier for audio data
/// </summary>
public struct AudioSpec {
    /// <summary>
    /// Audio data format
    /// </summary>
    public AudioFormat Format;
    /// <summary>
    /// Number of channels: 1 mono, 2 stereo, etc
    /// </summary>
    public int Channels;
    /// <summary>
    /// sample rate: sample frames per second
    /// </summary>
    public int Freq;

    /// <summary>
    /// Size of each audio frame (in bytes)
    /// </summary>
    /// <remarks>
    /// This reports on the size of an audio sample frame: stereo Sint16 data (2 channels of 2 bytes each) would be 4
    /// bytes per frame, for example.
    /// </remarks>
    public long FrameSize => Format.ByteSize() * Channels;
    
}