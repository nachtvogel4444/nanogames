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
        /// <param name="renderer">The renderer.</param>
        public Terminal(IRenderer renderer)
        {
            Renderer = renderer;
            Graphics = renderer == null ? Graphics.Null : new Graphics(renderer);
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
        /// Gets the backend renderer.
        /// </summary>
        public IRenderer Renderer { get; }

        /// <summary>
        /// Gets the player's graphics interface.
        /// </summary>
        public Graphics Graphics { get; }
    }
}
