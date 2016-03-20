// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Represents the current state of a player in the tournament.
    /// </summary>
    internal sealed class PlayerState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerState"/> class.
        /// </summary>
        /// <param name="id">The player's id.</param>
        public PlayerState(PlayerId id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the player's id.
        /// </summary>
        public PlayerId Id { get; }

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the player's tournament score.
        /// </summary>
        public int TournamentScore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is ready for the next match.
        /// </summary>
        public bool IsReady { get; set; }

        /// <summary>
        /// Gets or sets the option the current player votes for.
        /// </summary>
        public int VoteOption { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is currently in a match.
        /// </summary>
        public bool IsInMatch { get; set; }

        /// <summary>
        /// Gets or sets the index of the player in the current round.
        /// </summary>
        public int RoundPlayerIndex { get; set; }

        /// <summary>
        /// Gets or sets the arrival timestamp of the last packet we received from this player.
        /// </summary>
        public long LastPacketArrivalTimestamp { get; set; }
    }
}
