// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using ProtoBuf;

namespace NanoGames.Application
{
    /// <summary>
    /// Represents a packet sent over the network.
    /// </summary>
    [ProtoContract]
    internal sealed class Packet
    {
        /// <summary>
        /// The sending player id.
        /// </summary>
        [ProtoMember(1)]
        public PlayerId PlayerId;

        /// <summary>
        /// The sending player name. Null means the player name is unchanged.
        /// </summary>
        [ProtoMember(2)]
        public string PlayerName;
    }
}
