// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games
{
    /// <summary>
    /// Describes the properties of a player in a match.
    /// </summary>
    public sealed class PlayerDescription
    {
        /// <summary>
        /// Gets or sets the player's color.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the player's graphics.
        /// </summary>
        public Graphics Graphics { get; set; }

        public IOutput Output { get; set; }

        /// <summary>
        /// Gets or sets the player's input.
        /// </summary>
        public InputState Input { get; set; }

        /// <summary>
        /// Gets or sets the player's name.
        /// </summary>
        public string Name { get; set; }
    }
}
