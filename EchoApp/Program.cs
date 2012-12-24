using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FragLabs.Audio.Engines;
using FragLabs.Audio.Engines.OpenAL;

namespace EchoApp
{
    class Program
    {
        private static byte[] _readBuffer;
        private static Stream _capture;
        private static Stream _playback;

        static void Main(string[] args)
        {
            _readBuffer = new byte[960];
            Console.WriteLine("Opening \"{0}\" for playback", OpenALHelper.PlaybackDevices()[0].DeviceName);
            Console.WriteLine("Opening \"{0}\" for capture", OpenALHelper.CaptureDevices()[0].DeviceName);

            _playback = OpenALHelper.PlaybackDevices()[0].OpenStream(48000, OpenALAudioFormat.Mono16Bit);
            _capture = OpenALHelper.CaptureDevices()[0].OpenStream(48000, OpenALAudioFormat.Mono16Bit, 10);
            _capture.BeginRead(_readBuffer, 0, _readBuffer.Length, Callback, null);

            Console.WriteLine("Press [ENTER] to exit");
            Console.ReadLine();

            _playback.Close();
            _playback.Dispose();
            _capture.Close();
            _capture.Dispose();
        }

        private static void Callback(IAsyncResult ar)
        {
            if (!_capture.CanRead) return;

            var read = _capture.EndRead(ar);
            if (read > 0 && _playback.CanWrite)
            {
                //  if you want to use BeginWrite here instead you need to copy the _readBuffer to avoid race conditions reader/writing the same buffer asynchronously
                //  alternatively you can use multiple buffers to avoid such race conditions
                _playback.Write(_readBuffer, 0, read);
            }
            _capture.BeginRead(_readBuffer, 0, _readBuffer.Length, Callback, null);
        }
    }
}
