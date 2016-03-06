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
        /// <summary>
        /// Gets the list of players.
        /// </summary>
        public IReadOnlyList<TPlayer> Players { get; private set; }

        /// <summary>
        /// Sets the list of players.
        /// </summary>
        /// <param name="localPlayerIndex">The index of the local player, or -1 if there is no local player.</param>
        /// <param name="players">The list of players.</param>
        public void Initialize(List<TPlayer> players)
        {
            if (Players != null)
            {
                throw new InvalidOperationException("The players can only be set once.");
            }

            Players = players.AsReadOnly();

            Initialize();
        }

        /// <inheritdoc/>
        public override sealed void Update(List<PlayerDescription> playerDescriptions)
        {
            for (int i = 0; i < Players.Count; ++i)
            {
                Players[i].Graphics = playerDescriptions[i].Graphics;
                Players[i].Input = playerDescriptions[i].Input;
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
        /// Initializes the match. This is called before <see cref="Player.Initialize"/>.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Updates and renders the match for all players. This is called before <see cref="Player.Update"/>.
        /// </summary>
        protected abstract void Update();
    }
}
