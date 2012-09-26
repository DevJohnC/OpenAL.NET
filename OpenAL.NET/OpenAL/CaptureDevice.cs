using System;
using System.Collections.Generic;
using System.Threading;

namespace FragLabs.Audio.Engines.OpenAL
{
    /// <summary>
    /// Audio capture device.
    /// </summary>
    public class CaptureDevice : IDisposable
    {
        public delegate void CapturedSamplesAvailableHandler(CaptureDevice sender, byte[] sampleData);
        public event CapturedSamplesAvailableHandler CapturedSamplesAvailable;

        IntPtr device = IntPtr.Zero;
        bool stopCaptureThread = false;
        Thread pollingThread;

        public CaptureDevice(string deviceName)
        {
            DeviceName = deviceName;
        }
        ~CaptureDevice()
        {
            Dispose();
        }

        /// <summary>
        /// Starts capturing audio packets.
        /// </summary>
        public void StartCapturing(int sampleRate, OpenALAudioFormat format)
        {
            if (device != IntPtr.Zero)
                return;
            Format = format;
            SampleRate = sampleRate;
            Open();
            API.alcCaptureStart(device);
            stopCaptureThread = false;
            pollingThread = new Thread(new ThreadStart(PollingThread));
            pollingThread.Start();
        }

        /// <summary>
        /// Stops capturing audio packets.
        /// </summary>
        public void StopCapturing()
        {
            if (device == IntPtr.Zero)
                return;
            if (pollingThread != null)
            {
                stopCaptureThread = true;
                API.alcCaptureStop(device);
                pollingThread = null;
                Close();
            }
        }

        void Open()
        {
            if (device != IntPtr.Zero)
                return;
            //  buffer big enough to hold 1/10th of a second
            int bufferSize = SampleRate / 10;
            if (Format == OpenALAudioFormat.Stereo8Bit || Format == OpenALAudioFormat.Stereo16Bit)
                bufferSize *= 2;
            device = API.alcCaptureOpenDevice(DeviceName, (uint)SampleRate, Format, bufferSize);
        }

        void Close()
        {
            if (device == IntPtr.Zero)
                return;
            API.alcCaptureCloseDevice(device);
        }

        unsafe void PollingThread()
        {
            while (!stopCaptureThread)
            {
                int samples = GetSamplesAvailable();

                if (samples > 0)
                {
                    int bitDepth = 8;
                    int channels = 1;
                    if (Format == OpenALAudioFormat.Mono16Bit)
                        bitDepth = 16;
                    if (Format == OpenALAudioFormat.Stereo8Bit)
                        channels = 2;
                    if (Format == OpenALAudioFormat.Stereo16Bit)
                    {
                        channels = 2;
                        bitDepth = 16;
                    }

                    int bytesPerSample = channels * (bitDepth / 8);
                    int bufferSize = samples * bytesPerSample;
                    IntPtr buffPtr;
                    byte[] buffer = new byte[bufferSize];
                    fixed (byte* bbuff = buffer)
                    {
                        buffPtr = new IntPtr((void*)bbuff);
                        API.alcCaptureSamples(device, buffPtr, samples);
                    }
                    if (CapturedSamplesAvailable != null)
                        CapturedSamplesAvailable(this, buffer);
                }
                Thread.Sleep(TimeSpan.FromSeconds(0.05));
            }
        }

        int GetSamplesAvailable()
        {
            if (device == IntPtr.Zero)
                return 0;

            int samples;
            API.alcGetIntegerv(device, ALCEnum.ALC_CAPTURE_SAMPLES, 4, out samples);
            //  todo: error checking
            return samples;
        }

        /// <summary>
        /// Gets the format.
        /// </summary>
        public OpenALAudioFormat Format { get; private set; }

        /// <summary>
        /// Gets the sample rate.
        /// </summary>
        public int SampleRate { get; private set; }

        /// <summary>
        /// Gets the device name.
        /// </summary>
        public string DeviceName { get; private set; }

        public void Dispose()
        {
            StopCapturing();
        }
    }
}
