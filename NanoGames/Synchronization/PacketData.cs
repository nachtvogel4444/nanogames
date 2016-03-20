// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using ProtoBuf;

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Represents a packet sent over the network.
    /// </summary>
    [ProtoContract]
    internal sealed class PacketData
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

        /// <summary>
        /// The player's current tournament score.
        /// </summary>
        [ProtoMember(3)]
        public int TournamentScore;

        /// <summary>
        /// A value indicating whether the player is ready for the next game.
        /// </summary>
        [ProtoMember(4)]
        public bool IsReady;

        /// <summary>
        /// A random priority value for the current round.
        /// In case multiple clients call a new round at nearly the same time, this is used to break the tie.
        /// </summary>
        [ProtoMember(5)]
        public int RoundPriority;

        /// <summary>
        /// The time since the start of the round in thousandths of a frame.
        /// </summary>
        [ProtoMember(6)]
        public long RoundMilliFrames;

        /// <summary>
        /// The random number generation seed of the current round.
        /// </summary>
        [ProtoMember(7)]
        public int RoundSeed;

        /// <summary>
        /// The option the current player votes for.
        /// </summary>
        [ProtoMember(8)]
        public int VoteOption;

        /// <summary>
        /// The index of the player in the current round.
        /// </summary>
        [ProtoMember(9)]
        public int RoundPlayerIndex;

        /// <summary>
        /// The input entries for the player.
        /// </summary>
        [ProtoMember(10)]
        public InputEntry[] InputEntries;

        /// <summary>
        /// The player color.
        /// </summary>
        [ProtoMember(11)]
        public Color PlayerColor;
    }
}
