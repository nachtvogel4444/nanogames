// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Network
{
    /// <summary>
    /// A network packet.
    /// </summary>
    /// <typeparam name="TPacketData">The packet data type.</typeparam>
    public struct Packet<TPacketData>
    {
        /// <summary>
        /// Gets or sets a timestamp (as returned by Stopwatch.GetTimestamp()) indicating when the packet arrived.
        /// </summary>
        public long ArrivalTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the packet data.
        /// </summary>
        public TPacketData Data { get; set; }
    }
}
