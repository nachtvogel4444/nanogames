// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Engine
{
    /// <summary>
    /// Encodes a specific key press, which either has a keycode, or a key.
    /// </summary>
    internal sealed class KeyEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEvent"/> class.
        /// </summary>
        /// <param name="code">The key code.</param>
        public KeyEvent(KeyCode code)
        {
            Code = code;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEvent"/> class.
        /// </summary>
        /// <param name="c">The character entered.</param>
        public KeyEvent(char c)
        {
            Char = c;
        }

        /// <summary>
        /// Gets the key code of this event.
        /// </summary>
        public KeyCode Code { get; }

        /// <summary>
        /// Gets the character of this event.
        /// </summary>
        public char Char { get; }
    }
}
