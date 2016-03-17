// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using ProtoBuf;

namespace NanoGames
{
    /// <summary>
    /// Represents the input state of a player.
    /// </summary>
    [ProtoContract]
    public struct Input
    {
        /// <summary>
        /// Gets or sets a value indicating whether the up button is pressed.
        /// </summary>
        [ProtoMember(1)]
        public bool Up { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the down button is pressed.
        /// </summary>
        [ProtoMember(2)]
        public bool Down { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the left button is pressed.
        /// </summary>
        [ProtoMember(3)]
        public bool Left { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the right button is pressed.
        /// </summary>
        [ProtoMember(4)]
        public bool Right { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the fire button is pressed.
        /// </summary>
        [ProtoMember(5)]
        public bool Fire { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the alt fire button is pressed.
        /// </summary>
        [ProtoMember(6)]
        public bool AltFire { get; set; }
    }
}
