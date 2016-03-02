// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
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
        private int _localPlayerIndex;

        /// <summary>
        /// Gets the list of players.
        /// </summary>
        public IReadOnlyList<TPlayer> Players { get; private set; }

        /// <summary>
        /// Sets the list of players.
        /// </summary>
        /// <param name="localPlayerIndex">The index of the local player, or -1 if there is no local player.</param>
        /// <param name="players">The list of players.</param>
        public void SetPlayers(int localPlayerIndex, List<TPlayer> players)
        {
            if (Players != null)
            {
                throw new InvalidOperationException("The players can only be set once.");
            }

            _localPlayerIndex = localPlayerIndex;
            Players = players.AsReadOnly();
        }

        /// <inheritdoc/>
        public override sealed void Update(Terminal terminal)
        {
            for (int i = 0; i < Players.Count; ++i)
            {
                if (i == _localPlayerIndex)
                {
                    Players[i].Terminal = terminal;
                }
                else
                {
                    Players[i].Terminal = Terminal.Null;
                }
            }

            /* Update the match. */
            Update();

            /* Update each individual player. */
            foreach (var player in Players)
            {
                player.Update();
            }
        }

        /// <summary>
        /// Updates and renders the match for all players. This is called before <see cref="Player.Update"/>.
        /// </summary>
        protected abstract void Update();
    }
}
