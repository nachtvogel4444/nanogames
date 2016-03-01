// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a player.
    /// </summary>
    internal class Player
    {
        /// <summary>
        /// Gets or sets the player's index, ranging from 0 to the number of players minus one.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the player's color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the player's terminal.
        /// </summary>
        public Terminal Terminal { get; set; }

        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        public Random Random { get; } = new Random();

        /// <summary>
        /// Copies all values from another player object.
        /// </summary>
        /// <param name="player">The player object to copy.</param>
        public virtual void CopyFrom(Player player)
        {
            Index = player.Index;
            Color = player.Color;
            Terminal = player.Terminal;
        }

        /// <summary>
        /// Updates and optionally renders the player's state. This is runs after calling <see cref="Match{TPlayer}.UpdateMatch"/>.
        /// </summary>
        public virtual void Update()
        {
        }
    }
}
