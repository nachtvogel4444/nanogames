// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using ProtoBuf;

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Contains a player's input for a specific frame.
    /// </summary>
    [ProtoContract]
    internal class InputEntry
    {
        /// <summary>
        /// The frame index.
        /// </summary>
        [ProtoMember(1)]
        public int Frame;

        /// <summary>
        /// The input.
        /// </summary>
        [ProtoMember(2)]
        public InputState Input;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputEntry"/> class.
        /// </summary>
        public InputEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputEntry"/> class.
        /// </summary>
        /// <param name="frame">The frame index.</param>
        /// <param name="input">The input.</param>
        public InputEntry(int frame, InputState input)
        {
            Frame = frame;
            Input = input;
        }
    }
}
