using System;
using System.Collections.Generic;
using System.IO;

namespace FragLabs.Audio.Engines.OpenAL
{
    public class PlaybackStream : Stream
    {
        private readonly uint _sampleRate;
        private readonly OpenALAudioFormat _format;
        private readonly PlaybackDevice _device;
        private IntPtr _context;
        private uint _sourceId;
        private readonly List<uint> _bufferIds = new List<uint>();

        internal PlaybackStream(uint sampleRate, OpenALAudioFormat format, PlaybackDevice device, IntPtr context)
        {
            _sampleRate = sampleRate;
            _format = format;
            _device = device;
            _context = context;
            CreateSource();
        }

        void CreateSource()
        {
            lock (typeof (PlaybackStream))
            {
                API.alcMakeContextCurrent(_context);
                var sources = new uint[1];
                API.alGenSources(1, sources);
                _sourceId = sources[0];
            }
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get
            {
                return _sourceId != 0;
            }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            lock (typeof (PlaybackStream))
            {
                API.alcMakeContextCurrent(_context);
                var bufferId = CreateBuffer();
                if (offset == 0)
                    API.alBufferData(bufferId, _format, buffer, count, _sampleRate);
                else
                {
                    var tmpBuffer = new byte[count];
                    Buffer.BlockCopy(buffer, offset, tmpBuffer, 0, count);
                    API.alBufferData(bufferId, _format, tmpBuffer, count, _sampleRate);
                }
                API.alSourceQueueBuffers(_sourceId, 1, new[] {bufferId});
                if (!IsPlaying)
                    API.alSourcePlay(_sourceId);
                CleanupPlayedBuffers();
            }
        }

        uint CreateBuffer()
        {
            var buffers = new uint[1];
            API.alGenBuffers(1, buffers);
            var bufferId = buffers[0];
            lock (_bufferIds)
                _bufferIds.Add(bufferId);
            return bufferId;
        }

        void CleanupPlayedBuffers()
        {
            if (_sourceId == 0) return;
            int buffers;
            API.alGetSourcei(_sourceId, IntSourceProperty.AL_BUFFERS_PROCESSED, out buffers);
            var removedBuffers = new uint[buffers];
            API.alSourceUnqueueBuffers(_sourceId, buffers, removedBuffers);
            API.alDeleteBuffers(buffers, removedBuffers);
            lock (_bufferIds)
            {
                foreach (var bufferId in removedBuffers)
                {
                    _bufferIds.Remove(bufferId);
                }
            }
        }

        public override void Close()
        {
            base.Close();
            Cleanup();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Cleanup();
        }

        void Cleanup()
        {
            if (_sourceId == 0) return;

            lock (typeof (PlaybackStream))
            {
                if (IsPlaying)
                    API.alSourceStop(_sourceId);
                CleanupPlayedBuffers();

                var buffers = _bufferIds.Count;
                if (buffers > 0)
                {
                    _bufferIds.Clear();
                    var removedBuffers = new uint[buffers];
                    API.alSourceUnqueueBuffers(_sourceId, buffers, removedBuffers);
                    API.alDeleteBuffers(buffers, removedBuffers);
                }

                DestroySource();
                _device.ClosedStream(this);
                _context = IntPtr.Zero;
            }
        }

        void DestroySource()
        {
            if (_sourceId == 0) return;

            var sources = new uint[1];
            API.alDeleteSources(1, sources);
            _sourceId = 0;
        }

        public SourceState State
        {
            get
            {
                if (_sourceId == 0)
                    return SourceState.Uninitialized;

                int state;
                API.alGetSourcei(_sourceId, IntSourceProperty.AL_SOURCE_STATE, out state);

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
    }
}
