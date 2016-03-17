// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace NanoGames.Network
{
    /// <summary>
    /// Helper class to write packets into a stream.
    /// </summary>
    internal sealed class StreamWriter : IDisposable
    {
        private readonly BlockingCollection<byte[]> _outbox = new BlockingCollection<byte[]>();
        private readonly Stream _stream;
        private readonly WorkerThread _writerThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public StreamWriter(Stream stream)
        {
            _stream = stream;
            _writerThread = new WorkerThread(RunWriter);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _writerThread.Dispose();

            try
            {
                _stream.Dispose();
            }
            catch
            {
            }

            _outbox.Dispose();
        }

        /// <summary>
        /// Enqueues the packet for writing.
        /// </summary>
        /// <param name="packet">The packet to write.</param>
        public void Write(byte[] packet)
        {
            if (packet != null && packet.Length > 65535)
            {
                throw new IndexOutOfRangeException();
            }

            _outbox.Add(packet);
        }

        private void RunWriter(CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                byte[] packet = _outbox.Take(cancellation);
                WriteToStream(packet);
                while (_outbox.TryTake(out packet))
                {
                    WriteToStream(packet);
                }

                _stream.Flush();
            }
        }

        private void WriteToStream(byte[] packet)
        {
            int bytesToWrite = packet == null ? 0 : packet.Length;

            _stream.WriteByte((byte)bytesToWrite);
            _stream.WriteByte((byte)(bytesToWrite >> 8));

            if (bytesToWrite > 0)
            {
                _stream.Write(packet, 0, bytesToWrite);
            }
        }
    }
}
