// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Concurrent;

namespace NanoGames.Network
{
    /// <summary>
    /// Represents a network connection to a game.
    /// </summary>
    /// <typeparam name="TPacket">The packet type.</typeparam>
    public abstract class Endpoint<TPacket> : IDisposable
    {
        private readonly BlockingCollection<byte[]> _inbox = new BlockingCollection<byte[]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint{TPacket}"/> class.
        /// </summary>
        internal Endpoint()
        {
        }

        /// <summary>
        /// Gets a value indicating whether the endpoint is still connected.
        /// </summary>
        public abstract bool IsConnected { get; }

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
        public void Send(TPacket packet)
        {
            SendBytes(SerializationHelper.Serialize(packet));
        }

        /// <summary>
        /// Attempts to receive a packet.
        /// </summary>
        /// <param name="packet">The received packet.</param>
        /// <returns>A value indicating whether we successfully received a packet.</returns>
        public bool TryReceive(out TPacket packet)
        {
            byte[] bytes;
            if (_inbox.TryTake(out bytes))
            {
                packet = SerializationHelper.Deserialize<TPacket>(bytes);
                return true;
            }
            else
            {
                packet = default(TPacket);
                return false;
            }
        }

        /// <summary>
        /// Queues raw packet bytes for reception.
        /// </summary>
        /// <param name="bytes">The bytes to queue.</param>
        internal void ReceiveBytes(byte[] bytes)
        {
            _inbox.Add(bytes);
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        /// <param name="isDisposing">True if this is a dispose call, false if this is a finalizer call.</param>
        protected virtual void Dispose(bool isDisposing)
        {
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
    }
}
