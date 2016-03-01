// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// Represents the input state of a player.
    /// </summary>
    internal sealed class Input
    {
        /// <summary>
        /// Gets or sets a value indicating whether the back button is pressed.
        /// </summary>
        public bool Back { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the up button is pressed.
        /// </summary>
        public bool Up { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the down button is pressed.
        /// </summary>
        public bool Down { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the left button is pressed.
        /// </summary>
        public bool Left { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the right button is pressed.
        /// </summary>
        public bool Right { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the fire button is pressed.
        /// </summary>
        public bool Fire { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the alt fire button is pressed.
        /// </summary>
        public bool AltFire { get; set; }

        /// <summary>
        /// Gets or sets the text entered by the user since the last frame.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the backspace key is pressed.
        /// </summary>
        public bool Backspace { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the delete key is pressed.
        /// </summary>
        public bool Delete { get; set; }
    }
}
