// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames.Engine
{
    /// <summary>
    /// A container for the input/output interface.
    /// </summary>
    internal sealed class Terminal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Terminal"/> class.
        /// </summary>
        /// <param name="graphics">The player's graphics interface.</param>
        public Terminal(Graphics graphics)
        {
            Graphics = graphics;
        }

        /// <summary>
        /// Gets the player's input state.
        /// </summary>
        public Input Input { get; } = new Input();

        /// <summary>
        /// Gets the key events since the last frame.
        /// </summary>
        public List<KeyEvent> KeyEvents { get; } = new List<KeyEvent>();

        /// <summary>
        /// Gets the player's graphics interface.
        /// </summary>
        public Graphics Graphics { get; }
    }
}
