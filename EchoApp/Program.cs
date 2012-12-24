using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FragLabs.Audio.Engines;
using FragLabs.Audio.Engines.OpenAL;

namespace EchoApp
{
    class Program
    {
        private static byte[] _readBuffer;
        private static PlaybackDevice _playback;

        static void Main(string[] args)
        {
            _readBuffer = new byte[960];
            Console.WriteLine("Opening \"{0}\" for playback", OpenALHelper.PlaybackDevices()[0].DeviceName);
            Console.WriteLine("Opening \"{0}\" for capture", OpenALHelper.CaptureDevices()[0].DeviceName);

            _playback = OpenALHelper.PlaybackDevices()[0];

            var readerStream = OpenALHelper.CaptureDevices()[0].OpenStream(48000, OpenALAudioFormat.Mono16Bit, 10);
            readerStream.BeginRead(_readBuffer, 0, _readBuffer.Length, Callback, readerStream);

            Console.WriteLine("Press [ENTER] to exit");
            Console.ReadLine();
            readerStream.Close();
            readerStream.Dispose();
        }

        private static void Callback(IAsyncResult ar)
        {
            var readerStream = (CaptureStream)ar.AsyncState;
            if (!readerStream.CanRead) return;

            var read = readerStream.EndRead(ar);
            if (read > 0)
            {
                _playback.Play(_readBuffer, OpenALAudioFormat.Mono16Bit, 48000);
            }
            readerStream.BeginRead(_readBuffer, 0, _readBuffer.Length, Callback, readerStream);
        }
    }
}
