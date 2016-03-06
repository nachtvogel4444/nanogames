// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

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
        /// <param name="input">The player's input state.</param>
        /// <param name="graphics">The player's graphics interface.</param>
        public Terminal(ExtendedInput input, Graphics graphics)
        {
            Input = input;
            Graphics = graphics;
        }

        /// <summary>
        /// Gets the player's input state.
        /// </summary>
        public ExtendedInput Input { get; }

        /// <summary>
        /// Gets the player's graphics interface.
        /// </summary>
        public Graphics Graphics { get; }
    }
}
