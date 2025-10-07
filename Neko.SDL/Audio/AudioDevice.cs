using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neko.Sdl.Extra;
using Neko.Sdl.Extra.StandardLibrary;

namespace Neko.Sdl.Audio;

public unsafe class AudioDevice : IDisposable {
    private SDL_AudioDeviceID _id;
    public uint Id => (uint)_id; 
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator AudioDevice(SDL_AudioDeviceID o) => new(o);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SDL_AudioDeviceID(AudioDevice o) => o._id;

    public static AudioDevice DefaultPlayback = new (SDL_AUDIO_DEVICE_DEFAULT_PLAYBACK);
    public static AudioDevice DefaultRecording = new (SDL_AUDIO_DEVICE_DEFAULT_RECORDING);
    public AudioDevice(SDL_AudioDeviceID id) {
        _id = id;
    }
    public bool IsPaused => SDL_AudioDevicePaused(this);

    /// <summary>
    /// Close a previously-opened audio device
    /// </summary>
    /// <remarks>
    /// The application should close open audio devices once they are no longer needed.
    /// <br/><br/>
    /// This function may block briefly while pending audio data is played by the hardware, so that applications don't
    /// drop the last buffer of data they supplied if terminating immediately afterwards.
    /// </remarks>
    public void Close() => Dispose();

    public void Dispose() => SDL_CloseAudioDevice(this);

    /// <summary>
    /// Get the current channel map of an audio device.
    /// </summary>
    /// <returns>
    /// Returns an array of the current channel mapping, with as many elements as the current output spec's channels, or
    /// null if default
    /// </returns>
    /// <remarks>
    /// Channel maps are optional; most things do not need them, instead passing data in the order that SDL expects.
    /// <br/><br/>
    /// Audio devices usually have no remapping applied. This is represented by returning NULL, and does not signify an
    /// error.
    /// </remarks>
    public int[]? ChannelMap {
        get {
            int count = 0;
            var a = SDL_GetAudioDeviceChannelMap(this, &count);
            if (a is null)
                return null;
            return Util.ConvertSdlArrayToManaged(a, (uint)count);
        }
    }

    /// <summary>
    /// Get the current audio format of a specific audio device
    /// </summary>
    /// <param name="spec">Device details</param>
    /// <param name="sampleFrames">device buffer size, in sample frames</param>
    /// <remarks>
    /// For an opened device, this will report the format the device is currently using. If the device isn't yet opened,
    /// this will report the device's preferred format (or a reasonable default if this can't be determined).
    /// <br/><br/>
    /// Using <see cref="DefaultPlayback"/> and <see cref="DefaultRecording"/> useful for getting a reasonable
    /// recommendation before opening the system-recommended default device.
    /// <br/><br/>
    /// You can also use this to request the current device buffer size. This is specified in sample frames and
    /// represents the amount of data SDL will feed to the physical hardware in each chunk. This can be converted to
    /// milliseconds of audio with the following equation:
    /// <code>
    /// ms = (int) ((((Sint64) frames) * 1000) / spec.freq);
    /// </code>
    /// Buffer size is only important if you need low-level control over the audio playback timing. Most apps do not need this.
    /// </remarks>
    public void GetFormat(out AudioSpec spec, out int sampleFrames) {
        spec = new AudioSpec();
        sampleFrames = 0;
        SDL_GetAudioDeviceFormat(this, (SDL_AudioSpec*)Unsafe.AsPointer(ref spec),
            (int*)Unsafe.AsRef(ref sampleFrames)).ThrowIfError();
    }

    /// <summary>
    /// Gain of an audio device
    /// </summary>
    /// <remarks>
    /// The gain of a device is its volume; a larger gain means a louder output, with a gain of zero being silence.
    /// <br/><br/>
    /// Audio devices default to a gain of 1.0f (no change in output).
    /// <br/><br/>
    /// Physical devices may not have their gain changed, only logical devices, and get returns -1.0f
    /// when used on physical devices. Set throws an Exception. While it might seem attractive to adjust several logical
    /// devices at once in this way, it would allow an app or library to interfere with another portion of the program's
    /// otherwise-isolated devices.
    /// <br/><br/>
    /// This is applied, along with any per-audiostream gain, during playback to the hardware, and can be continuously
    /// changed to create various effects. On recording devices, this will adjust the gain before passing the data into
    /// an audiostream; that recording audiostream can then adjust its gain further when outputting the data elsewhere,
    /// if it likes, but that second gain is not applied until the data leaves the audiostream again.
    /// </remarks>
    public float Gain {
        get {
            var result = SDL_GetAudioDeviceGain(this);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (result == -1.0f && !IsPhysical)
                throw new SdlException();
            return Gain;
        }
        set => SDL_SetAudioDeviceGain(this, value).ThrowIfError();
    }

    public string? Name {
        get => SDL_GetAudioDeviceName(this);
        
    }

    /// <summary>
    /// Determine if an audio device is physical (instead of logical)
    /// </summary>
    /// <returns>
    /// An AudioDevice that represents physical hardware is a physical device; there is one for each piece of
    /// hardware that SDL can see. Logical devices are created by calling <see cref="Open"/> or
    /// <see cref="OpenStream"/>, and while each is associated with a physical device, there can be any number of
    /// logical devices on one physical device.
    /// <br/><br/>
    /// For the most part, logical and physical IDs are interchangeable--if you try to open a logical device, SDL
    /// understands to assign that effort to the underlying physical device, etc. However, it might be useful to know if
    /// an arbitrary device ID is physical or logical. This function reports which.
    /// <br/><br/>
    /// This function may return either true or false for invalid device IDs.
    /// </returns>
    public bool IsPhysical => SDL_IsAudioDevicePhysical(this);

    /// <summary>
    /// Determine if an audio device is a playback device (instead of recording)
    /// </summary>
    /// <returns>
    /// This function may return either true or false for invalid device IDs
    /// </returns>
    public bool IsPlayback => SDL_IsAudioDevicePlayback(this);

    public AudioDevice Open(ref AudioSpec spec) {
        var result = SDL_OpenAudioDevice(this, (SDL_AudioSpec*)Unsafe.AsPointer(ref spec));
        if (result == 0)
            throw new SdlException();
        return (AudioDevice)result;
    }

    /// <summary>
    /// Open a specific audio device
    /// </summary>
    /// <returns>the device</returns>
    /// <remarks>
    /// You can open both playback and recording devices through this function. Playback devices will take data from
    /// bound audio streams, mix it, and send it to the hardware. Recording devices will feed any bound audio streams
    /// with a copy of any incoming data.
    /// <br/><br/>
    /// An opened audio device starts out with no audio streams bound. To start audio playing, bind a stream and supply
    /// audio data to it. Unlike SDL2, there is no audio callback; you only bind audio streams and make sure they have
    /// data flowing into them (however, you can simulate SDL2's semantics fairly closely by using
    /// <see cref="OpenStream"/> instead of this function).
    /// <br/><br/>
    /// If you don't care about opening a specific device, use either <see cref="DefaultPlayback"/> or
    /// <see cref="DefaultRecording"/>. In this case, SDL will try to pick the most reasonable default, and may also
    /// switch between physical devices seamlessly later, if the most reasonable default changes during the lifetime of
    /// this opened device (user changed the default in the OS's system preferences, the default got unplugged so the
    /// system jumped to a new default, the user plugged in headphones on a mobile device, etc). Unless you have a good
    /// reason to choose a specific device, this is probably what you want.
    /// <br/><br/>
    /// You may request a specific format for the audio device, but there is no promise the device will honor that
    /// request for several reasons. As such, it's only meant to be a hint as to what data your app will provide. Audio
    /// streams will accept data in whatever format you specify and manage conversion for you as appropriate.
    /// <see cref="GetFormat"/> can tell you the preferred format for the device before opening and the actual format
    /// the device is using after opening.
    /// <br/><br/>
    /// It's legal to open the same device more than once; each successful open will generate a new logical
    /// AudioDevice that is managed separately from others on the same physical device. This allows libraries to
    /// open a device separately from the main app and bind its own streams without conflicting.
    /// <br/><br/>
    /// It is also legal to open a device returned by a previous call to this function; doing so just creates another
    /// logical device on the same physical device. This may be useful for making logical groupings of audio streams.
    /// <br/><br/>
    /// This function returns the opened device on success. This is a new, unique AudioDevice that represents a
    /// logical device.
    /// <br/><br/>
    /// Some backends might offer arbitrary devices (for example, a networked audio protocol that can connect to an
    /// arbitrary server). For these, as a change from SDL2, you should open a default device ID and use an SDL hint to
    /// specify the target if you care, or otherwise let the backend figure out a reasonable default. Most backends
    /// don't offer anything like this, and often this would be an end user setting an environment variable for their
    /// custom need, and not something an application should specifically manage.
    /// <br/><br/>
    /// When done with an audio device, possibly at the end of the app's life, one should call <see cref="Close"/> on
    /// the returned device.
    /// </remarks>
    public AudioDevice Open() {
        var result = SDL_OpenAudioDevice(this, null);
        if (result == 0)
            throw new SdlException();
        return (AudioDevice)result;
    }
    
    /// <inheritdoc cref="OpenStream(StreamCallback?)"/>
    /// <param name="spec">the audio stream's data format</param>
    public AudioStream OpenStream(ref AudioSpec spec, AudioStream.Callback? callback = null) {
        SDL_AudioStream* stream = null;
        if (callback is null)
            stream = SDL_OpenAudioDeviceStream(this, (SDL_AudioSpec*)Unsafe.AsPointer(ref spec), null, 0);
        else {
            var streamObj = new AudioStream(null);
            var pin = streamObj.Pin(GCHandleType.Normal);
            stream = SDL_OpenAudioDeviceStream(this, (SDL_AudioSpec*)Unsafe.AsPointer(ref spec), &AudioStream.CallbackNative,
                pin.Pointer);
            if (stream is null) {
                pin.Dispose();
                throw new SdlException();
            }
            streamObj.Handle = stream;
            return streamObj;
        }
        if (stream is null)
            throw new SdlException();
        return new AudioStream(stream);
    }
    
    /// <summary>
    /// Convenience function for straightforward audio init for the common case
    /// </summary>
    /// <param name="callback">
    /// a callback where the app will provide new data for playback, or receive new data for recording.
    /// Can be null, in which case the app will need to call SDL_PutAudioStreamData or SDL_GetAudioStreamData as necessary.
    /// </param>
    /// <returns></returns>
    /// <remarks>
    /// If all your app intends to do is provide a single source of PCM audio, this function allows you to do all your
    /// audio setup in a single call.
    /// <br/><br/>
    /// This is also intended to be a clean means to migrate apps from SDL2.
    /// <br/><br/>
    /// This function will open an audio device, create a stream and bind it. Unlike other methods of setup, the audio
    /// device will be closed when this stream is destroyed, so the app can treat the returned AudioStream as the
    /// only object needed to manage audio playback.
    /// <br/><br/>
    /// Also unlike other functions, the audio device begins paused. This is to map more closely to SDL2-style behavior,
    /// since there is no extra step here to bind a stream to begin audio flowing. The audio device should be resumed
    /// with <see cref="AudioStream.ResumeDevice"/>.
    /// <br/><br/>
    /// This function works with both playback and recording devices.
    /// <br/><br/>
    /// The spec parameter represents the app's side of the audio stream. That is, for recording audio, this will be the
    /// output format, and for playing audio, this will be the input format. If spec is null, the system will choose the
    /// format, and the app can use <see cref="AudioStream.Format"/> to obtain this information later.
    /// <br/><br/>
    /// If you don't care about opening a specific audio device, you can (and probably should), use
    /// <see cref="DefaultPlayback"/> for playback and <see cref="DefaultRecording"/> for recording.
    /// <br/><br/>
    /// One can optionally provide a callback function; if null, the app is expected to queue audio data for playback
    /// (or unqueue audio data if capturing). Otherwise, the callback will begin to fire once the device is unpaused.
    /// <br/><br/>
    /// Destroying the returned stream with <see cref="AudioStream.Dispose"/> will also close the audio device associated
    /// with this stream.
    /// </remarks>
    public AudioStream OpenStream(AudioStream.Callback? callback = null) {
        SDL_AudioStream* stream = null;
        if (callback is null)
            stream = SDL_OpenAudioDeviceStream(this, null, null, 0);
        else {
            //TODO: pin leaking here, handle never released, same as above
            var streamObj = new AudioStream(null);
            var pin = streamObj.Pin(GCHandleType.Normal);
            stream = SDL_OpenAudioDeviceStream(this, null, &AudioStream.CallbackNative,
                pin.Pointer);
            if (stream is null) {
                pin.Dispose();
                throw new SdlException();
            }
            streamObj.Handle = stream;
            return streamObj;
        }
        if (stream is null)
            throw new SdlException();
        return new AudioStream(stream);
    }

    /// <summary>
    /// Use this function to pause audio playback on a specified device
    /// </summary>
    /// <remarks>
    /// This function pauses audio processing for a given device. Any bound audio streams will not progress, and no
    /// audio will be generated. Pausing one device does not prevent other unpaused devices from running.
    /// <br/><br/>
    /// Unlike in SDL2, audio devices start in an unpaused state, since an app has to bind a stream before any audio
    /// will flow. Pausing a paused device is a legal no-op.
    /// <br/><br/>
    /// Pausing a device can be useful to halt all audio without unbinding all the audio streams. This might be useful
    /// while a game is paused, or a level is loading, etc.
    /// <br/><br/>
    /// Physical devices can not be paused or unpaused, only logical devices created through <see cref="Open"/> can
    /// be.
    /// </remarks>
    public void Pause() => SDL_PauseAudioDevice(this).ThrowIfError();

    /// <summary>
    /// Use this function to unpause audio playback on a specified device
    /// </summary>
    /// <remarks>
    /// This function unpauses audio processing for a given device that has previously been paused with
    /// <see cref="Pause"/>. Once unpaused, any bound audio streams will begin to progress again, and audio can be
    /// generated.
    /// <br/><br/>
    /// Unlike in SDL2, audio devices start in an unpaused state, since an app has to bind a stream before any audio
    /// will flow. Unpausing an unpaused device is a legal no-op.
    /// <br/><br/>
    /// Physical devices can not be paused or unpaused, only logical devices created through <see cref="Open"/> can
    /// be.
    /// </remarks>
    public void Resume() => SDL_ResumeAudioDevice(this).ThrowIfError();

    /// <summary>
    /// Bind a single audio stream to an audio device
    /// </summary>
    /// <param name="stream">an audio stream to bind to a device</param>
    public void Bind(AudioStream stream) => SDL_BindAudioStream(this, stream).ThrowIfError();

    /// <summary>
    /// Bind a collection of audio streams to an audio device
    /// </summary>
    /// <param name="streams"></param>
    /// <remarks>
    /// Audio data will flow through any bound streams. For a playback device, data for all bound streams will be mixed
    /// together and fed to the device. For a recording device, a copy of recorded data will be provided to each bound
    /// stream.
    /// <br/><br/>
    /// Audio streams can only be bound to an open device. This operation is atomic--all streams bound in the same call
    /// will start processing at the same time, so they can stay in sync. Also: either all streams will be bound or none
    /// of them will be.
    /// <br/><br/>
    /// It is an error to bind an already-bound stream; it must be explicitly unbound first.
    /// <br/><br/>
    /// Binding a stream to a device will set its output format for playback devices, and its input format for recording
    /// devices, so they match the device's settings. The caller is welcome to change the other end of the stream's
    /// format at any time with SDL_SetAudioStreamFormat(). If the other end of the stream's format has never been set
    /// (the audio stream was created with a NULL audio spec), this function will set it to match the device end's
    /// format.
    /// </remarks>
    public void Bind(IEnumerable<AudioStream> streams) {
        var ptrs = streams.Select(stream => stream.Pointer).ToArray();
        fixed (IntPtr* ptrs1 = ptrs)
            SDL_BindAudioStreams(this, (SDL_AudioStream**)ptrs1, ptrs.Length).ThrowIfError();
    }

    private static AudioDevice[]? _playback = null;
    private static AudioDevice[]? _recording = null;

    /// <summary>
    /// List of currently-connected audio playback devices
    /// </summary>
    /// <remarks>
    /// This returns of list of available devices that play sound, perhaps to speakers or headphones
    /// ("playback" devices). If you want devices that record audio, like a microphone ("recording" devices), use
    /// <see cref="Recording"/> instead.
    /// <br/><br/>
    /// This only returns a list of physical devices; it will not have any device IDs returned by <see cref="Open()"/>.
    /// <br/><br/>
    /// This is cached to reduce GC strain, use <see cref="Refresh"/> to clear the cache.
    /// </remarks>
    public static AudioDevice[] Playback {
        get {
            if (_playback is not null)
                return _playback;
            int count;
            var ptr = SDL_GetAudioPlaybackDevices(&count);
            if (ptr is null)
                throw new SdlException();
            var result = new AudioDevice[count];
            for (int i = 0; i < result.Length; i++) {
                result[i] = new AudioDevice(0);
            }
            UnmanagedMemory.Free(ptr);
            return _playback = result;
        }
    }
    
    /// <summary>
    /// List of currently-connected audio recording devices
    /// </summary>
    /// <remarks>
    /// This returns of list of available devices that record audio, like a microphone ("recording" devices). If you
    /// want devices that play sound, perhaps to speakers or headphones ("playback" devices), use
    /// <see cref="Playback"/> instead.
    /// <br/><br/>
    /// This only returns a list of physical devices; it will not have any device IDs returned by <see cref="Open()"/>.
    /// <br/><br/>
    /// This is cached to reduce GC strain, use <see cref="Refresh"/> to clear the cache.
    /// </remarks>
    public static AudioDevice[] Recording {
        get {
            if (_recording is not null)
                return _recording;
            int count;
            var ptr = SDL_GetAudioRecordingDevices(&count);
            if (ptr is null)
                throw new SdlException();
            var result = new AudioDevice[count];
            for (int i = 0; i < result.Length; i++) {
                result[i] = new AudioDevice(0);
            }
            UnmanagedMemory.Free(ptr);
            return _recording = result;
        }
    }
    

    /// <summary>
    /// Refresh AudioDevice cache
    /// </summary>
    public static void Refresh() {
        // if (_playback is not null) foreach (var device in _playback)
        //     device.Dispose();
        _playback = null;
        
        // if (_recording is not null) foreach (var device in _recording)
        //     device.Dispose();
        _recording = null;
    }

    /// <summary>
    /// A callback that fires around an audio device's processing work
    /// </summary>
    /// <param name="device">the audio device this callback is running for</param>
    /// <param name="start">true if the start of iteration, false if the end</param>
    /// <remarks>
    /// This callback fires when a logical audio device is about to start accessing its bound audio streams, and fires
    /// again when it has finished accessing them. It covers the range of one "iteration" of the audio device.
    /// <br/><br/>
    /// It can be useful to use this callback to update state that must apply to all bound audio streams atomically: to
    /// make sure state changes don't happen while half of the streams are already processed for the latest audio buffer.
    /// <br/><br/>
    /// This callback should run as quickly as possible and not block for any significant time, as this callback delays
    /// submission of data to the audio device, which can cause audio playback problems. This callback delays all audio
    /// processing across a single physical audio device: all its logical devices and all bound audio streams. Use it
    /// carefully.
    /// </remarks>
    public delegate void IterationCallback(AudioDevice device, bool start);

    private static void IterationCallbackNative(IntPtr userdata, SDL_AudioDeviceID id, bool start) {
        
    }

    // missing in bindings:
    // public void SetIterationCallbacks(IterationCallback start, IterationCallback end) {
    //     
    // }
    
    /// <summary>
    /// A callback that fires when data is about to be fed to an audio device
    /// </summary>
    /// <param name="spec">the current format of audio that is to be submitted to the audio device</param>
    /// <param name="buffer">the buffer of audio samples to be submitted. The callback can inspect and/or modify this data</param>
    /// <remarks>
    /// This is useful for accessing the final mix, perhaps for writing a visualizer or applying a final effect to the
    /// audio data before playback.
    /// <br/><br/>
    /// This callback should run as quickly as possible and not block for any significant time, as this callback delays
    /// submission of data to the audio device, which can cause audio playback problems.
    /// <br/><br/>
    /// The postmix callback must be able to handle any audio data format specified in spec, which can change between
    /// callbacks if the audio device changed. However, this only covers frequency and channel count; data is always
    /// provided here in SDL_AUDIO_F32 format.
    /// <br/><br/>
    /// The postmix callback runs after logical device gain and audiostream gain have been applied, which is to say you
    /// can make the output data louder at this point than the gain settings would suggest.
    /// </remarks>
    public delegate void AudioPostmixCallback(ref AudioSpec spec, Span<float> buffer);
    
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void AudioPostmixCallbackNative(IntPtr userdata, SDL_AudioSpec* spec, float* buffer, int buflen) {
        var span = new Span<float>(buffer, buflen);
        var pin = userdata.AsPin<AudioPostmixCallback>();
        if (pin.TryGetTarget(out var callback))
            callback(ref Unsafe.AsRef<AudioSpec>(spec), span);
    }

    private Pin<AudioPostmixCallback>? _audioPostmixCallbackPin;
    
    /// <summary>
    /// A callback that fires when data is about to be fed to an audio device
    /// </summary>
    /// <remarks>
    /// This is useful for accessing the final mix, perhaps for writing a visualizer or applying a final effect to the
    /// audio data before playback.
    /// <br/><br/>
    /// The buffer is the final mix of all bound audio streams on an opened device; this callback will fire regularly
    /// for any device that is both opened and unpaused. If there is no new data to mix, either because no streams are
    /// bound to the device or all the streams are empty, this callback will still fire with the entire buffer set to
    /// silence.
    /// <br/><br/>
    /// This callback is allowed to make changes to the data; the contents of the buffer after this call is what is
    /// ultimately passed along to the hardware.
    /// <br/><br/>
    /// The callback is always provided the data in float format (values from -1.0f to 1.0f), but the number of channels
    /// or sample rate may be different than the format the app requested when opening the device; SDL might have had to
    /// manage a conversion behind the scenes, or the playback might have jumped to new physical hardware when a system
    /// default changed, etc. These details may change between calls. Accordingly, the size of the buffer might change
    /// between calls as well.
    /// <br/><br/>
    /// This callback can run at any time, and from any thread; if you need to serialize access to your app's data, you
    /// should provide and use a mutex or other synchronization device.
    /// <br/><br/>
    /// All of this to say: there are specific needs this callback can fulfill, but it is not the simplest interface.
    /// Apps should generally provide audio in their preferred format through an SDL_AudioStream and let SDL handle the
    /// difference.
    /// <br/><br/>
    /// This function is extremely time-sensitive; the callback should do the least amount of work possible and return
    /// as quickly as it can. The longer the callback runs, the higher the risk of audio dropouts or other problems.
    /// <br/><br/>
    /// This function will block until the audio device is in between iterations, so any existing callback that might be
    /// running will finish before this function sets the new callback and returns.
    /// <br/><br/>
    /// Setting a null callback function disables any previously-set callback.
    /// </remarks>
    public AudioPostmixCallback? PostmixCallback {
        get => _audioPostmixCallbackPin?.Target;
        set {
            _audioPostmixCallbackPin?.Dispose();
            if (value is null) {
                SDL_SetAudioPostmixCallback(this, null, 0).ThrowIfError();
                return;
            }
            _audioPostmixCallbackPin = value.Pin(GCHandleType.Normal);
            SDL_SetAudioPostmixCallback(this, &AudioPostmixCallbackNative, _audioPostmixCallbackPin.Pointer).ThrowIfError();
        }
    }
    
    
}
