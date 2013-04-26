using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static PlaybackStream _playback;

        static void Main(string[] args)
        {
            _readBuffer = new byte[960];
            Console.WriteLine("Opening \"{0}\" for playback", OpenALHelper.PlaybackDevices[0].DeviceName);
            Console.WriteLine("Opening \"{0}\" for capture", OpenALHelper.CaptureDevices[0].DeviceName);

            _playback = OpenALHelper.PlaybackDevices[0].OpenStream(48000, OpenALAudioFormat.Mono16Bit);
            _playback.Listener.Position = new Vector3() { X = 0.0f, Y = 0.0f, Z = 0.0f };
            _playback.Listener.Velocity = new Vector3() { X = 0.0f, Y = 0.0f, Z = 0.0f };
            _playback.Listener.Orientation = new Orientation()
                {
                    At = new Vector3() { X = 0.0f, Y = 0.0f, Z = 1.0f },
                    Up = new Vector3() { X = 0.0f, Y = 1.0f, Z = 0.0f }
                };
            _playback.ALPosition = new Vector3() { X = 0.0f, Y = 0.0f, Z = 0.0f };
            _playback.Velocity = new Vector3() { X = 0.0f, Y = 0.0f, Z = 0.0f };
            _capture = OpenALHelper.CaptureDevices[0].OpenStream(48000, OpenALAudioFormat.Mono16Bit, 10);
            _capture.BeginRead(_readBuffer, 0, _readBuffer.Length, Callback, null);

            while (true)
            {
                PrintUI();
                if (ProcessInput())
                    break;
            }

            _playback.Close();
            _playback.Dispose();
            _capture.Close();
            _capture.Dispose();
        }

        /// <summary>
        /// Processes keyboard events, returns true when the user hits [ENTER].
        /// </summary>
        /// <returns></returns>
        static bool ProcessInput()
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Enter)
                return true;
            switch (key.Key)
            {
                case ConsoleKey.W:
                    {
                        var listenerLocation = _playback.Listener.Position;
                        listenerLocation.Z += 0.5f;
                        _playback.Listener.Position = listenerLocation;
                    }
                    break;
                case ConsoleKey.S:
                    {
                        var listenerLocation = _playback.Listener.Position;
                        listenerLocation.Z -= 0.5f;
                        _playback.Listener.Position = listenerLocation;
                    }
                    break;
                case ConsoleKey.A:
                    {
                        var listenerLocation = _playback.Listener.Position;
                        listenerLocation.X -= 0.5f;
                        _playback.Listener.Position = listenerLocation;
                    }
                    break;
                case ConsoleKey.D:
                    {
                        var listenerLocation = _playback.Listener.Position;
                        listenerLocation.X += 0.5f;
                        _playback.Listener.Position = listenerLocation;
                    }
                    break;
                case ConsoleKey.Q:
                    {
                        var listenerLocation = _playback.Listener.Position;
                        listenerLocation.Y += 0.5f;
                        _playback.Listener.Position = listenerLocation;
                    }
                    break;
                case ConsoleKey.E:
                    {
                        var listenerLocation = _playback.Listener.Position;
                        listenerLocation.Y -= 0.5f;
                        _playback.Listener.Position = listenerLocation;
                    }
                    break;
            }
            return false;
        }

        static void PrintUI()
        {
            Console.Clear();
            Console.WriteLine("Listener location: {0:f2},{1:f2},{2:f2}", _playback.Listener.Position.X,
                _playback.Listener.Position.Y, _playback.Listener.Position.Z);
            Console.WriteLine("Controls: W - Step forward");
            Console.WriteLine("          S - Step backward");
            Console.WriteLine("          A - Step left");
            Console.WriteLine("          D - Step right");
            Console.WriteLine("          Q - Fly up");
            Console.WriteLine("          E - Fly down");
            Console.WriteLine("          [ENTER] - Exit program");
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
