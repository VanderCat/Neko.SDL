using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;

namespace Neko.Sdl.Audio;

public unsafe partial class AudioStream : SdlWrapper<SDL_AudioStream> {
    /// <summary>
    /// Clear any pending data in the stream
    /// </summary>
    /// <remarks>
    /// This drops any queued data, so there will be nothing to read from the stream until more is added
    /// </remarks>
    public void Clear() => SDL_ClearAudioStream(this);

    public static AudioStream Create(ref AudioSpec src, ref AudioSpec dst) {
        var result = SDL_CreateAudioStream( (SDL_AudioSpec*)Unsafe.AsPointer(ref src), (SDL_AudioSpec*)Unsafe.AsPointer(ref dst));
        if (result is null)
            throw new SdlException();
        return result;
    }

    /// <summary>
    /// Free an audio stream
    /// </summary>
    /// <remarks>
    /// This will release all allocated data, including any audio that is still queued. You do not need to manually
    /// clear the stream first.
    ///
    /// If this stream was bound to an audio device, it is unbound during this call. If this stream was created with
    /// <see cref="AudioDevice.OpenStream(StreamCallback)"/>, the audio device that was opened alongside this stream's
    /// creation will be closed, too.
    /// </remarks>
    public override void Dispose() {
        SDL_DestroyAudioStream(this);
        base.Dispose();
    }

    /// <summary>
    /// Tell the stream that you're done sending data, and anything being buffered should be converted/resampled and
    /// made available immediately
    /// </summary>
    /// <remarks>
    /// It is legal to add more data to a stream after flushing, but there may be audio gaps in the output. Generally
    /// this is intended to signal the end of input, so the complete output becomes available.
    /// </remarks>
    public void Flush() => SDL_FlushAudioStream(this).ThrowIfError();

    /// <summary>
    /// Lock an audio stream for serialized access
    /// </summary>
    /// <remarks>
    /// Each SDL_AudioStream has an internal mutex it uses to protect its data structures from threading conflicts. This
    /// function allows an app to lock that mutex, which could be useful if registering callbacks on this stream.
    ///
    /// One does not need to lock a stream to use in it most cases, as the stream manages this lock internally. However,
    /// this lock is held during callbacks, which may run from arbitrary threads at any time, so if an app needs to
    /// protect shared data during those callbacks, locking the stream guarantees that the callback is not running while
    /// the lock is held.
    ///
    /// As this is just a wrapper over SDL_LockMutex for an internal lock; it has all the same attributes (recursive
    /// locks are allowed, etc).
    /// </remarks>
    public void Lock() => SDL_LockAudioStream(this).ThrowIfError();

    /// <summary>
    /// Unlock an audio stream for serialized access
    /// </summary>
    /// <remarks>
    /// This unlocks an audio stream after a call to <see cref="Lock"/>.
    /// </remarks>
    public void Unlock() => SDL_UnlockAudioStream(this).ThrowIfError();
    
    /// <summary>
    /// Lock the mutex and unlock automatically on scope end
    /// </summary>
    /// <example>
    /// <code>
    /// using (stream.ScopeLock()) {
    ///     //do stuff
    /// }
    /// </code>
    /// </example>
    public Scope ScopeLock() {
        Lock();
        return new Scope(this);
    }
    
    
    public ref struct Scope {
        private AudioStream _stream;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Scope(AudioStream stream) {
            _stream = stream;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() {
            _stream.Unlock();
        }
    }

    /// <summary>
    /// Use this function to pause audio playback on the audio device associated with an audio stream
    /// </summary>
    /// <remarks>
    /// This function pauses audio processing for a given device. Any bound audio streams will not progress, and no
    /// audio will be generated. Pausing one device does not prevent other unpaused devices from running.
    /// <br/><br/>
    /// Pausing a device can be useful to halt all audio without unbinding all the audio streams. This might be useful
    /// while a game is paused, or a level is loading, etc.
    /// </remarks>
    public void PauseDevice() => SDL_PauseAudioStreamDevice(this);

    /// <summary>
    /// The number of converted/resampled bytes available
    /// </summary>
    /// <remarks>
    /// The stream may be buffering data behind the scenes until it has enough to resample correctly, so this number
    /// might be lower than what you expect, or even be zero. Add more data or flush the stream if you need the data
    /// now.
    /// <br/><br/>
    /// If the stream has so much data that it would overflow an int, the return value is clamped to a maximum value,
    /// but no queued data is lost; if there are gigabytes of data queued, the app might need to read some of it with
    /// <see cref="GetData"/> before this function's return value is no longer clamped.
    /// </remarks>
    public int Available {
        get {
            var result = SDL_GetAudioStreamAvailable(this);
            if (result == -1) throw new SdlException();
            return result;
        }
    }

    /// <summary>
    /// Get converted/resampled data from the stream
    /// </summary>
    /// <param name="buffer">a buffer to fill with audio data</param>
    /// <returns>he number of bytes read from the stream</returns>
    /// <remarks>
    /// The input/output data format/channels/samplerate is specified when creating the stream, and can be changed after
    /// creation by setting <see cref="Format"/>.
    /// <br/><br/>
    /// Note that any conversion and resampling necessary is done during this call, and <see cref="PutData"/> simply
    /// queues unconverted data for later. This is different than SDL2, where that work was done while inputting new
    /// data to the stream and requesting the output just copied the converted data.
    /// </remarks>
    public int GetData(Span<byte> buffer) {
        int nya;
        fixed (byte* ptr = buffer)
            nya = SDL_GetAudioStreamData(this, (IntPtr)ptr, buffer.Length);
        if (nya == -1)
            throw new SdlException();
        return nya;
    }

    private AudioDevice? _device;

    public AudioDevice? Device {
        get {
            if (_device is not null)
                return _device;
            var result = SDL_GetAudioStreamDevice(this);
            if (result == 0)
                return null;
            return _device = (AudioDevice)result;
        }
    }

    /// <summary>
    /// Input audio format
    /// </summary>
    /// <remarks>
    /// Future calls to and <see cref="Available"/> and <see cref="GetData"/> will reflect the new format, and
    /// future calls to <see cref="PutData{T}(ReadOnlySpan{T})"/> must provide data in the new input formats.
    /// <br/><br/>
    /// Data that was previously queued in the stream will still be operated on in the format that was current when it
    /// was added, which is to say you can put the end of a sound file in one format to a stream, change formats for the
    /// next sound file, and start putting that new data while the previous sound file is still queued, and everything
    /// will still play back correctly.
    /// <br/><br/>
    /// If a stream is bound to a device, then the format of the side of the stream bound to a device cannot be changed
    /// Attempts to make a change to this side will be ignored, but this will not report an error. The other side's
    /// format can be changed.
    /// </remarks>
    public AudioSpec SourceFormat {
        get {
            var foramt = new AudioSpec();
            SDL_GetAudioStreamFormat(this, (SDL_AudioSpec*)&foramt, null).ThrowIfError();
            return foramt;
        }
        set => SDL_SetAudioStreamFormat(this, (SDL_AudioSpec*)Unsafe.AsPointer(ref value), null).ThrowIfError();
    } 
    
    /// <summary>
    /// Output audio format
    /// </summary>
    /// <remarks>
    /// Future calls to and <see cref="Available"/> and <see cref="GetData"/> will reflect the new format, and
    /// future calls to <see cref="PutData{T}(ReadOnlySpan{T})"/> must provide data in the new input formats.
    /// <br/><br/>
    /// Data that was previously queued in the stream will still be operated on in the format that was current when it
    /// was added, which is to say you can put the end of a sound file in one format to a stream, change formats for the
    /// next sound file, and start putting that new data while the previous sound file is still queued, and everything
    /// will still play back correctly.
    /// <br/><br/>
    /// If a stream is bound to a device, then the format of the side of the stream bound to a device cannot be changed
    /// Attempts to make a change to this side will be ignored, but this will not report an error. The other side's
    /// format can be changed.
    /// </remarks>
    public AudioSpec DestinationFormat {
        get {
            var foramt = new AudioSpec();
            SDL_GetAudioStreamFormat(this, null, (SDL_AudioSpec*)&foramt).ThrowIfError();
            return foramt;
        }
        set => SDL_SetAudioStreamFormat(this, null, (SDL_AudioSpec*)Unsafe.AsPointer(ref value)).ThrowIfError();
    }

    /// <summary>
    /// The frequency ratio of the stream
    /// </summary>
    /// <remarks>
    /// The frequency ratio is used to adjust the rate at which input data is consumed. Changing this effectively
    /// modifies the speed and pitch of the audio. A value greater than 1.0f will play the audio faster, and at a higher
    /// pitch. A value less than 1.0f will play the audio slower, and at a lower pitch. 1.0f means play at normal speed.
    /// <br/><br/>
    /// This is applied during <see cref="GetData"/>, and can be continuously changed to create various effects.
    /// </remarks>
    public float FrequencyRatio {
        get {
            var result = SDL_GetAudioStreamFrequencyRatio(this);
            if (result == 0.0f)
                throw new SdlException();
            return result;
        }
        set => SDL_SetAudioStreamFrequencyRatio(this, value).ThrowIfError();
    }

    /// <summary>
    /// The gain of an audio stream
    /// </summary>
    /// <remarks>
    /// The gain of a stream is its volume; a larger gain means a louder output, with a gain of zero being silence.
    /// <br/><br/>
    /// Audio streams default to a gain of 1.0f (no change in output).
    /// <br/><br/>
    /// This is applied during <see cref="GetData"/>, and can be continuously changed to create various effects.
    /// </remarks>
    public float Gain {
        get {
            var result = SDL_GetAudioStreamGain(this);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (result == -1.0f)
                throw new SdlException();
            return result;
        }
        set => SDL_SetAudioStreamGain(this, value).ThrowIfError();
    }

    //TODO: im not sure is those channel map impl is correct
    
    /// <summary>
    /// Array of the current channel mapping, with as many elements as the current output spec's channels, or NULL if default
    /// </summary>
    /// <remarks>
    /// Channel maps are optional; most things do not need them, instead passing data in the order that SDL expects.
    /// <br/><br/>
    /// Audio streams default to no remapping applied. This is represented by returning NULL, and does not signify an error.
    /// </remarks>
    public int[]? InputChannelMap {
        get {
            int count;
            var ptr = SDL_GetAudioStreamInputChannelMap(this, &count);
            if (ptr is null)
                return null;
            return Util.ConvertSdlArrayToManaged(ptr, (uint)count);
        }
    }

    /// <summary>
    /// Array of the current channel mapping, with as many elements as the current output spec's channels, or NULL if default.
    /// </summary>
    /// <remarks>
    /// Channel maps are optional; most things do not need them, instead passing data in the order that SDL expects.
    /// <br/><br/>
    /// Audio streams default to no remapping applied. This is represented by returning NULL, and does not signify an error.
    /// </remarks>
    public int[]? OutputChannelMap {
        get {
            int count;
            var ptr = SDL_GetAudioStreamOutputChannelMap(this, &count);
            if (ptr is null)
                return null;
            return Util.ConvertSdlArrayToManaged(ptr, (uint)count);
        }
    }

    public Properties Properties {
        get {
            var result = SDL_GetAudioStreamProperties(this);
            if (result == 0)
                throw new SdlException();
            return (Properties)result;
        }
    }

    /// <summary>
    /// The number of bytes queued
    /// </summary>
    /// <remarks>
    /// This is the number of bytes put into a stream as input, not the number that can be retrieved as output. Because
    /// of several details, it's not possible to calculate one number directly from the other. If you need to know how
    /// much usable data can be retrieved right now, you should use <see cref="Available"/> and not this property.
    /// <br/><br/>
    /// Note that audio streams can change their input format at any time, even if there is still data queued in a
    /// different format, so the returned byte count will not necessarily match the number of sample frames available.
    /// Users of this API should be aware of format changes they make when feeding a stream and plan accordingly.
    /// <br/><br/>
    /// Queued data is not converted until it is consumed by <see cref="GetData"/>, so this value should be
    /// representative of the exact data that was put into the stream.
    /// <br/><br/>
    /// If the stream has so much data that it would overflow an int, the return value is clamped to a maximum value,
    /// but no queued data is lost; if there are gigabytes of data queued, the app might need to read some of it with
    /// <see cref="GetData"/> before this property's value is no longer clamped.
    /// </remarks>
    public int Queued {
        get {
            var result = SDL_GetAudioStreamQueued(this);
            if (result == -1)
                throw new SdlException();
            return result;
        }
    }

    /// <summary>
    /// Add data to the stream
    /// </summary>
    /// <param name="buf">a span of the audio data to add</param>
    /// <remarks>
    /// This data must match the format/channels/samplerate specified in the latest call to
    /// <see cref="DestinationFormat"/>, or the format specified when creating the stream if it hasn't been changed.
    /// <br/><br/>
    /// Note that this call simply copies the unconverted data for later. This is different than SDL2, where data was
    /// converted during the Put call and the Get call would just dequeue the previously-converted data.
    /// </remarks>
    public void PutData<T>(ReadOnlySpan<T> buf) where T : unmanaged {
        fixed (T* bufPtr = buf)
            SDL_PutAudioStreamData(this, (IntPtr)bufPtr, buf.Length).ThrowIfError();
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void AudioStreamDataCompleteCallbackNative(IntPtr userdata, IntPtr buf, int buflen) {
        var handle = GCHandle.FromIntPtr(userdata);
        handle.Free();
    }

    /// <summary>
    /// Add external data to an audio stream without copying it
    /// </summary>
    /// <param name="buf">The audio data to add</param>
    /// <remarks>
    /// Unlike <see cref="PutData{T}"/>, this function does not make a copy of the provided data, instead storing the
    /// provided pointer. This means that the put operation does not need to allocate and copy the data, but the
    /// original data must remain available until the stream is done with it, either by being read from the stream in
    /// its entirety, or a call to <see cref="Clear"/> or <see cref="Dispose"/>. This also means the entire buffer is
    /// pinned and making Garbage Collector to perform considerably worse.
    /// <br/><br/>
    /// The data must match the format/channels/samplerate specified in the latest call to SDL_SetAudioStreamFormat, or
    /// the format specified when creating the stream if it hasn't been changed.
    /// <br/><br/>
    /// Note that there is still an allocation to store tracking information, so this function is more efficient for
    /// larger blocks of data. If you're planning to put a few samples at a time, it will be more efficient to use
    /// <see cref="PutData{T}"/>, which allocates and buffers in blocks.
    /// </remarks>
    [Obsolete("You should not use this, as it hinders GC. This provided for the sake of completeness")]
    public void PutDataNoCopy<T>(ref T[] buf) {
        var handle = GCHandle.Alloc(buf, GCHandleType.Pinned);
        SDL_PutAudioStreamDataNoCopy(this, handle.AddrOfPinnedObject(), buf.Length,
            &AudioStreamDataCompleteCallbackNative, GCHandle.ToIntPtr(handle)).ThrowIfError();
    }

    /// <summary>
    /// Add data to the stream with each channel in a separate array
    /// </summary>
    /// <param name="channelBuffers">An array of arrays, one array per channel</param>
    /// <param name="numSamples">the number of samples per array to write to the stream</param>
    /// <remarks>
    /// This data must match the format/channels/samplerate specified in the latest call to SDL_SetAudioStreamFormat, or
    /// the format specified when creating the stream if it hasn't been changed.
    /// <br/><br/>
    /// The data will be interleaved and queued. Note that AudioStream only operates on interleaved data, so this is
    /// simply a convenience function for easily queueing data from sources that provide separate arrays. There is no
    /// equivalent function to retrieve planar data.
    /// <br/><br/>
    /// The arrays in channelBuffers are ordered as they are to be interleaved; the first array will be the first
    /// sample in the interleaved data.
    /// <br/><br/>
    /// Note that numSamples is the number of samples per array. This can also be thought of as the number of sample
    /// frames to be queued. A value of 1 with stereo arrays will queue two samples to the stream. This is different
    /// than SDL_PutAudioStreamData, which wants the size of a single array in bytes.
    /// </remarks>
    public void PutData<T>(T[][] channelBuffers, int numSamples) where T : unmanaged {
        // var ptrptr = stackalloc T[channelBuffers.Rank];
        // foreach (var channelBuffer in channelBuffers) {
        //     
        // }
        // SDL_PutAudioStreamPlanarData(this, (IntPtr*)bufPtr, channelBuffers.Rank, numSamples).ThrowIfError();
        throw new NotImplementedException();
    }

    /// <summary>
    /// Use this function to unpause audio playback on the audio device associated with an audio stream
    /// </summary>
    /// <remarks>
    /// This function unpauses audio processing for a given device that has previously been paused. Once unpaused, any
    /// bound audio streams will begin to progress again, and audio can be generated.
    /// <br/><br/>
    /// <see cref="AudioDevice.Open()"/> opens audio devices in a paused state, so this function call is required for audio
    /// playback to begin on such devices.
    /// </remarks>
    public void ResumeDevice() => SDL_ResumeAudioStreamDevice(this).ThrowIfError();

    /// <summary>
    /// A callback that fires when data passes through an AudioStream
    /// </summary>
    /// <param name="stream">the SDL audio stream associated with this callback</param>
    /// <param name="additionalAmmount">the amount of data, in bytes, that is needed right now</param>
    /// <param name="totalAmmount">the total amount of data requested, in bytes, that is requested or available</param>
    /// <remarks>
    /// Apps can (optionally) register a callback with an audio stream that is called when data is added with
    /// <see cref="PutData{T}(ReadOnlySpan{T})"/>, or requested with <see cref="GetData"/>.
    /// <br/><br/>
    /// Two values are offered here: one is the amount of additional data needed to satisfy the immediate request
    /// (which might be zero if the stream already has enough data queued) and the other is the total amount being
    /// requested. In a Get call triggering a Put callback, these values can be different. In a Put call triggering a
    /// Get callback, these values are always the same.
    /// <br/><br/>
    /// Byte counts might be slightly overestimated due to buffering or resampling, and may change from call to call.
    /// <br/><br/>
    /// This callback is not required to do anything. Generally this is useful for adding/reading data on demand, and
    /// the app will often put/get data as appropriate, but the system goes on with the data currently available to it
    /// if this callback does nothing.
    /// </remarks>
    public delegate void Callback(AudioStream stream, int additionalAmmount, int totalAmmount);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void CallbackNative(IntPtr userdata, SDL_AudioStream* stream, int additional_amount, int total_amount) {
        var pin = new Pin<AudioStream>(userdata);
        if (pin.TryGetTarget(out var target))
            target._getCallback?.Invoke(target, additional_amount, total_amount);
    }
    
    internal Callback? _getCallback;
    internal Pin<AudioStream>? _selfPin;
    
    public Callback? GetCallback {
        set {
            if (value is null) {
                _getCallback = null;
                _selfPin?.Dispose();
                _selfPin = null;
                SDL_SetAudioStreamGetCallback(this, null, 0).ThrowIfError();
                return;
            }
            _getCallback = value;
            _selfPin = this.Pin(GCHandleType.Normal);
            SDL_SetAudioStreamGetCallback(this, &CallbackNative, _selfPin.Pointer).ThrowIfError();
        }
    }
}