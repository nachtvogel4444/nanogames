// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Engine
{
    /// <summary>
    /// Represents a player's extended input state.
    /// This includes the regular game input, but also parts of the input only available to the UI.
    /// </summary>
    internal class ExtendedInput : Input
    {
        /// <summary>
        /// Gets or sets a value indicating whether the back button is pressed.
        /// </summary>
        public bool Escape { get; set; }

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
