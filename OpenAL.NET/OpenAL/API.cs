using System;
using System.Runtime.InteropServices;

namespace FragLabs.Audio.Engines.OpenAL
{
    internal class API
    {
        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr alGetString(int name);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr alcGetString([In] IntPtr device, int name);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern sbyte alcIsExtensionPresent([In] IntPtr device, string extensionName);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern sbyte alIsExtensionPresent(string extensionName);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alcCaptureStart(IntPtr device);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alcCaptureStop(IntPtr device);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alcCaptureSamples(IntPtr device, IntPtr buffer, int numSamples);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr alcCaptureOpenDevice(string deviceName, uint frequency, OpenALAudioFormat format, int bufferSize);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alcCaptureCloseDevice(IntPtr device);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alcGetIntegerv(IntPtr device, ALCEnum param, int size, out int data);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr alcOpenDevice(string deviceName);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr alcCloseDevice(IntPtr handle);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr alcCreateContext(IntPtr device, IntPtr attrlist);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal extern static void alcMakeContextCurrent(IntPtr context);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr alcGetContextsDevice(IntPtr context);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal extern static IntPtr alcGetCurrentContext();

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal extern static void alcDestroyContext(IntPtr context);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGetSourcei(uint sourceID, IntSourceProperty property, out int value);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourcePlay(uint sourceID);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourcePause(uint sourceID);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourceStop(uint sourceID);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourceQueueBuffers(uint sourceID, int number, uint[] bufferIDs);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourceUnqueueBuffers(uint sourceID, int buffers, uint[] buffersDequeued);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGenSources(int count, uint[] sources);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alDeleteSources(int count, uint[] sources);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGetSourcef(uint sourceID, FloatSourceProperty property, out float value);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alSourcef(uint sourceID, FloatSourceProperty property, float value);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGetBufferi(uint bufferID, ALEnum property, out int value);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alGenBuffers(int count, uint[] bufferIDs);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alBufferData(uint bufferID, OpenALAudioFormat format, byte[] data, int byteSize, uint frequency);

        [DllImport("OpenAL32.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void alDeleteBuffers(int numBuffers, uint[] bufferIDs);
    }

    internal enum ALCStrings : int
    {
        AL_VERSION = 0xB002,
		ALC_DEFAULT_DEVICE_SPECIFIER = 0x1004,
		ALC_DEVICE_SPECIFIER = 0x1005,
		ALC_CAPTURE_DEVICE_SPECIFIER = 0x310,
		ALC_CAPTURE_DEFAULT_DEVICE_SPECIFIER = 0x311,
		ALC_ALL_DEVICES_SPECIFIER = 0x1013,
		ALC_DEFAULT_ALL_DEVICES_SPECIFIER = 0x1012
    }

    public enum OpenALAudioFormat
    {
        Unknown = 0,
        Mono8Bit = 0x1100,
        Mono16Bit = 0x1101,
        Stereo8Bit = 0x1102,
        Stereo16Bit = 0x1103
    }

    internal enum ALEnum
    {
        AL_GAIN = 0x100A,
        AL_FREQUENCY = 0x2001,
        AL_BITS = 0x2002,
        AL_CHANNELS = 0x2003,
        AL_SIZE = 0x2004,
    }

    internal enum ALCEnum
    {
        ALC_MAJOR_VERSION = 0x1000,
        ALC_MINOR_VERSION = 0x1001,
        ALC_ATTRIBUTES_SIZE = 0x1002,
        ALC_ALL_ATTRIBUTES = 0x1003,
        ALC_CAPTURE_SAMPLES = 0x312,
        ALC_FREQUENCY = 0x1007,
        ALC_REFRESH = 0x1008,
        ALC_SYNC = 0x1009,
        ALC_MONO_SOURCES = 0x1010,
        ALC_STEREO_SOURCES = 0x1011,
    }

    internal enum IntSourceProperty
    {
        AL_SOURCE_STATE = 0x1010,
        AL_BUFFERS_QUEUED = 0x1015,
        AL_BUFFERS_PROCESSED = 0x1016
    }

    internal enum FloatSourceProperty
    {
        AL_PITCH = 0x1003,
        AL_GAIN = 0x100A,
        AL_MIN_GAIN = 0x100D,
        AL_MAX_GAIN = 0x100E,
        AL_MAX_DISTANCE = 0x1023,
        AL_ROLLOFF_FACTOR = 0x1021,
        AL_CONE_OUTER_GAIN = 0x1022,
        AL_CONE_INNER_ANGLE = 0x1001,
        AL_CONE_OUTER_ANGLE = 0x1002,
        AL_REFERENCE_DISTANCE = 0x1020
    }
}
