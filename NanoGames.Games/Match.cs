// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a single match of a discipline.
    /// </summary>
    public abstract class Match
    {
        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        public Random Random { get; } = new Random();

        /// <summary>
        /// Updates and renders the match.
        /// </summary>
        /// <param name="terminal"></param>
        public abstract void Update(List<PlayerDescription> players);
    }
}
