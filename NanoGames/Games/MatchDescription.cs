// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames.Games
{
    /// <summary>
    /// Describes the players and circumstances of a match.
    /// Used by <see cref="Discipline.CreateMatch(MatchDescription)"/> to create a new match instance.
    /// </summary>
    internal sealed class MatchDescription
    {
        /// <summary>
        /// Gets or sets the list of players.
        /// </summary>
        public List<Player> Players { get; set; } = new List<Player>();

        /// <summary>
        /// Gets or sets the index of the local player, or -1 if there is no local player.
        /// </summary>
        public int LocalPlayerIndex { get; set; } = -1;
    }
}
