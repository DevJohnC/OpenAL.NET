using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FragLabs.Audio.Engines.OpenAL
{
    /// <summary>
    /// Audio playback device.
    /// </summary>
    public class PlaybackDevice : IDisposable
    {
        IntPtr device = IntPtr.Zero;
        IntPtr context = IntPtr.Zero;
        uint sourceId = 0;
        uint bufferId = 0;

        public PlaybackDevice(string deviceName)
        {
            DeviceName = deviceName;
        }
        ~PlaybackDevice()
        {
            Dispose();
        }

        /// <summary>
        /// Plays the given data using the device.
        /// </summary>
        /// <param name="samples"></param>
        /// <param name="format"></param>
        /// <param name="frequency"></param>
        public void Play(byte[] samples, OpenALAudioFormat format, uint frequency)
        {
            Open();
            if (sourceId == 0)
                CreateSource();
            CreateBuffer();
            API.alBufferData(bufferId, format, samples, samples.Length, frequency);
            API.alSourceQueueBuffers(sourceId, 1, new uint[] { bufferId });
            if (!IsPlaying)
                API.alSourcePlay(sourceId);
            CleanupPlayedBuffers();
        }

        public void Stop()
        {
            if (IsPlaying)
                API.alSourceStop(sourceId);
            CleanupPlayedBuffers();
            DestroySource();
        }

        void CleanupPlayedBuffers()
        {
            if (sourceId != 0)
            {
                int buffers;
                API.alGetSourcei(sourceId, IntSourceProperty.AL_BUFFERS_PROCESSED, out buffers);
                uint[] removedBuffers = new uint[buffers];
                API.alSourceUnqueueBuffers(sourceId, buffers, removedBuffers);
                API.alDeleteBuffers(buffers, removedBuffers);
            }
        }

        void DestroySource()
        {
            uint[] sources = new uint[1];
            API.alDeleteSources(1, sources);
            sourceId = 0;
        }

        void CreateBuffer()
        {
            uint[] buffers = new uint[1];
            API.alGenBuffers(1, buffers);
            bufferId = buffers[0];
        }

        void CreateSource()
        {
            uint[] sources = new uint[1];
            API.alGenSources(1, sources);
            sourceId = sources[0];
        }

        void Open()
        {
            if (device != IntPtr.Zero)
                return;
            device = API.alcOpenDevice(DeviceName);
            context = API.alcCreateContext(device, IntPtr.Zero);
            API.alcMakeContextCurrent(context);
        }

        void Close()
        {
            if (device == IntPtr.Zero)
                return;
            API.alcMakeContextCurrent(IntPtr.Zero);
            API.alcDestroyContext(context);
            API.alcCloseDevice(device);
        }

        /// <summary>
        /// Gets the device name.
        /// </summary>
        public string DeviceName { get; private set; }

        public SourceState State
        {
            get
            {
                if (sourceId == 0)
                    return SourceState.Uninitialized;

                int state;
                API.alGetSourcei(sourceId, IntSourceProperty.AL_SOURCE_STATE, out state);

                return (SourceState)state;
            }
        }

        public bool IsPlaying
        {
            get { return (this.State == SourceState.Playing); }
        }

        public bool IsPaused
        {
            get { return (this.State == SourceState.Paused); }
        }

        public bool IsStopped
        {
            get { return (this.State == SourceState.Stopped); }
        }

        public void Dispose()
        {
            Stop();
            Close();
        }
    }
}
