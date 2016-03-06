// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a player.
    /// </summary>
    internal abstract class Player
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
        public Graphics Graphics { get; set; }

        /// <summary>
        /// Gets or sets the player's input.
        /// </summary>
        public Input Input { get; set; }

        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        public Random Random { get; } = new Random();

        /// <summary>
        /// Initializes the player. This is called after <see cref="Match{TPlayer}.Initialize()"/>.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Updates and optionally renders the player's state. This is runs after calling <see cref="Match{TPlayer}.Update()"/>.
        /// </summary>
        public abstract void Update();
    }
}
