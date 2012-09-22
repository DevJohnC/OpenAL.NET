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
            API.alGenBuffers(1, new uint[] { 1 });
            API.alBufferData(1, format, samples, samples.Length, frequency);
            API.alSourceQueueBuffers(1, 1, new uint[] { 1 });
        }

        void Open()
        {
            if (device != IntPtr.Zero)
                return;
            device = API.alcOpenDevice(DeviceName);
            context = API.alcCreateContext(device, IntPtr.Zero);
            API.alcMakeContextCurrent(context);
            API.alGenSources(1, new uint[] { 1 });
            API.alSourcePlay(1);
        }

        void Close()
        {
            if (device == IntPtr.Zero)
                return;
            API.alcDestroyContext(context);
            API.alcCloseDevice(device);
        }

        /// <summary>
        /// Gets the device name.
        /// </summary>
        public string DeviceName { get; private set; }

        public void Dispose()
        {
            Close();
        }
    }
}
