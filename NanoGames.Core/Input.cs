// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// Represents the input state of a player.
    /// </summary>
    public struct Input
    {
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
        /// Copies all values from another instance.
        /// </summary>
        /// <param name="input">The instance to copy from.</param>
        public void CopyFrom(Input input)
        {
            Up = input.Up;
            Down = input.Down;
            Left = input.Left;
            Right = input.Right;
            Fire = input.Fire;
            AltFire = input.AltFire;
        }
    }
}
