// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a single match of a discipline.
    /// </summary>
    internal abstract class Match
    {
        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        public Random Random { get; } = new Random();

        /// <summary>
        /// Updates and renders the match for all players.
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Initializes the match.
        /// </summary>
        public abstract void Initialize();
    }
}
