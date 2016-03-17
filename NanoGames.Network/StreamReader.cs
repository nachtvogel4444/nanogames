// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.IO;
using System.Threading;

namespace NanoGames.Network
{
    /// <summary>
    /// Helper class to read packets from a stream.
    /// </summary>
    internal sealed class StreamReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly Action<byte[]> _onPacket;
        private readonly Action _onDisconnect;
        private readonly WorkerThread _readerThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamReader"/> class.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="onPacket">The action to invoke for every received packet.</param>
        /// <param name="onClose">The action to invoke when the stream is closed.</param>
        public StreamReader(Stream stream, Action<byte[]> onPacket, Action onClose)
        {
            _stream = stream;
            _onPacket = onPacket;
            _onDisconnect = onClose;
            _readerThread = new WorkerThread(RunReader);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _readerThread.Dispose();

            try
            {
                _stream.Dispose();
            }
            catch
            {
            }
        }

        private void RunReader(CancellationToken cancellation)
        {
            try
            {
                while (!cancellation.IsCancellationRequested)
                {
                    int l0 = _stream.ReadByte();
                    if (l0 < 0)
                    {
                        return;
                    }

                    int l1 = _stream.ReadByte();
                    if (l1 < 0)
                    {
                        return;
                    }

                    int bytesToRead = (l1 << 8) | l0;

                    byte[] packet;
                    if (bytesToRead == 0)
                    {
                        packet = null;
                    }
                    else
                    {
                        packet = new byte[bytesToRead];
                        int bytesRead = 0;

                        while (bytesRead < bytesToRead)
                        {
                            int b = _stream.Read(packet, bytesRead, bytesToRead - bytesRead);
                            if (b <= 0)
                            {
                                return;
                            }

                            bytesRead += b;
                        }
                    }

                    _onPacket(packet);
                }
            }
            finally
            {
                _onDisconnect?.Invoke();
            }
        }
    }
}
