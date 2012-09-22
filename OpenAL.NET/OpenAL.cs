using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FragLabs.Audio.Engines.OpenAL;

namespace FragLabs.Audio.Engines
{
    /// <summary>
    /// Helper class for working with OpenAL devices.
    /// </summary>
    public class OpenALHelper
    {
        public static CaptureDevice[] CaptureDevices()
        {
            var strings = ReadStringsFromMemory(API.alcGetString(IntPtr.Zero, (int)ALCStrings.ALC_CAPTURE_DEVICE_SPECIFIER));
            var ret = new CaptureDevice[strings.Length];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = new CaptureDevice(strings[i]);
            }
            return ret;
        }

        public static PlaybackDevice[] PlaybackDevices()
        {
            string[] strings = new string[0];
            if (GetIsExtensionPresent("ALC_ENUMERATE_ALL_EXT"))
            {
                strings = ReadStringsFromMemory(API.alcGetString(IntPtr.Zero, (int)ALCStrings.ALC_ALL_DEVICES_SPECIFIER));
            }
            else if (GetIsExtensionPresent("ALC_ENUMERATION_EXT"))
            {
                strings = ReadStringsFromMemory(API.alcGetString(IntPtr.Zero, (int)ALCStrings.ALC_DEVICE_SPECIFIER));
            }
            var ret = new PlaybackDevice[strings.Length];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = new PlaybackDevice(strings[i]);
            }
            return ret;
        }

        internal static string[] ReadStringsFromMemory(IntPtr location)
        {
            List<string> strings = new List<string>();

            bool lastNull = false;
            int i = -1;
            byte c;
            while (!((c = Marshal.ReadByte(location, ++i)) == '\0' && lastNull))
            {
                if (c == '\0')
                {
                    lastNull = true;

                    strings.Add(Marshal.PtrToStringAnsi(location, i));
                    location = new IntPtr((long)location + i + 1);
                    i = -1;
                }
                else
                    lastNull = false;
            }

            return strings.ToArray();
        }

        internal static bool GetIsExtensionPresent(string extension)
        {
            sbyte result;
            if (extension.StartsWith("ALC"))
            {
                result = API.alcIsExtensionPresent(IntPtr.Zero, extension);
            }
            else
            {
                result = API.alIsExtensionPresent(extension);
                //  todo: check for errors here
            }

            return (result == 1);
        }
    }
}
