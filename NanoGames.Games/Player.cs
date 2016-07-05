// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a player.
    /// </summary>
    public abstract class Player
    {
        /// <summary>
        /// Gets or sets the player's index, ranging from 0 to the number of players minus one.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Gets or sets the player's color.
        /// </summary>
        public Color Color { get; internal set; }

        /// <summary>
        /// Gets or sets the player's local color, which is equal to the player's color, except for the local player, where it is white.
        /// </summary>
        public Color LocalColor { get; internal set; }

        public IOutput Output { get; internal set; }

        /// <summary>
        /// Gets or sets the player's input.
        /// </summary>
        public Input Input { get; } = new Input();

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the player's score.
        /// This used to determine the player's ranking.
        /// It's never shown on the screen and can be a completely artificial number.
        /// Higher scores are better.
        /// </summary>
        public double Score { get; internal set; }
    }
}
