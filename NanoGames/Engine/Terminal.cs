// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine.OutputSystems;
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
        /// <param name="graphics">The graphics interface.</param>
        /// <param name="audio">The audio interface.</param>
        public Terminal(IGraphics graphics, Audio audio)
        {
            Graphics = graphics;
            Audio = audio;
        }

        /// <summary>
        /// Gets or sets the player's input state.
        /// </summary>
        public InputState Input { get; set; }

        /// <summary>
        /// Gets the key events since the last frame.
        /// </summary>
        public List<KeyEvent> KeyEvents { get; } = new List<KeyEvent>();

        /// <summary>
        /// Gets the player's graphics interface.
        /// </summary>
        public IGraphics Graphics { get; }

        /// <summary>
        /// Gets the player's audio interface.
        /// </summary>
        public Audio Audio { get; }
    }
}
