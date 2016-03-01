// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a single match.
    /// </summary>
    /// <typeparam name="TPlayer">The player type associated with the match.</typeparam>
    internal abstract class Match<TPlayer> : Match
        where TPlayer : Player
    {
        /// <summary>
        /// Gets or sets the list of players.
        /// </summary>
        public List<TPlayer> Players { get; set; }

        /// <inheritdoc/>
        public override sealed void Update()
        {
            UpdateMatch();
            foreach (var player in Players)
            {
                player.Update();
            }
        }

        /// <summary>
        /// Updates and renders the match for all players. This is called before <see cref="Player.Update"/>.
        /// </summary>
        protected abstract void UpdateMatch();
    }
}
