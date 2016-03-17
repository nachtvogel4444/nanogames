// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace NanoGames.Network
{
    /// <summary>
    /// Represents a network connection to a game.
    /// </summary>
    /// <typeparam name="TPacketData">The packet type.</typeparam>
    public abstract class Endpoint<TPacketData> : IDisposable
    {
        private readonly BlockingCollection<PacketData> _inbox = new BlockingCollection<PacketData>();

        private int _isDisposed = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint{TPacket}"/> class.
        /// </summary>
        internal Endpoint()
        {
        }

        /// <summary>
        /// Gets a value indicating whether the endpoint is still connected.
        /// </summary>
        public bool IsConnected => Volatile.Read(ref _isDisposed) == 0 && GetIsConnected();

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sends a packet. Packets are broadcast to all other clients.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        public void Send(TPacketData packet)
        {
            if (Volatile.Read(ref _isDisposed) != 0)
            {
                return;
            }

            SendBytes(SerializationHelper.Serialize(packet));
        }

        /// <summary>
        /// Attempts to receive a packet.
        /// </summary>
        /// <param name="packet">The received packet.</param>
        /// <returns>A value indicating whether we successfully received a packet.</returns>
        public bool TryReceive(out Packet<TPacketData> packet)
        {
            if (Volatile.Read(ref _isDisposed) != 0)
            {
                packet = default(Packet<TPacketData>);
                return false;
            }

            PacketData packetData;
            if (_inbox.TryTake(out packetData))
            {
                packet = default(Packet<TPacketData>);
                packet.Data = SerializationHelper.Deserialize<TPacketData>(packetData.Bytes);
                packet.ArrivalTimestamp = packetData.ArrivalTimestamp;
                return true;
            }
            else
            {
                packet = default(Packet<TPacketData>);
                return false;
            }
        }

        /// <summary>
        /// Queues raw packet bytes for reception.
        /// </summary>
        /// <param name="bytes">The bytes to queue.</param>
        internal void ReceiveBytes(byte[] bytes)
        {
            if (Volatile.Read(ref _isDisposed) != 0)
            {
                return;
            }

            _inbox.Add(new PacketData(Stopwatch.GetTimestamp(), bytes));
        }

        /// <summary>
        /// Gets a value indicating whether the endpoint is still connected.
        /// </summary>
        /// <returns>True if the endpoint is connected, false otherwise.</returns>
        protected abstract bool GetIsConnected();

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="isDisposing">True if this is a dispose call, false if this is a finalizer call.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
            {
                return;
            }

            if (isDisposing)
            {
                _inbox.Dispose();
            }
        }

        /// <summary>
        /// Sends raw packet bytes to all other clients.
        /// </summary>
        /// <param name="bytes">The bytes to send.</param>
        protected abstract void SendBytes(byte[] bytes);

        private struct PacketData
        {
            public long ArrivalTimestamp;

            public byte[] Bytes;

            public PacketData(long arrivalTimestamp, byte[] bytes)
            {
                ArrivalTimestamp = arrivalTimestamp;
                Bytes = bytes;
            }
        }
    }
}
