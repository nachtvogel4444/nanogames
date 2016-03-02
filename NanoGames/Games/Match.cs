// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a single match of a discipline.
    /// </summary>
    internal abstract class Match : IView
    {
        /// <summary>
        /// Gets a random number generator.
        /// </summary>
        public Random Random { get; } = new Random();

        /// <summary>
        /// Initializes the match.
        /// </summary>
        public abstract void Initialize();

        /// <inheritdoc/>
        public abstract void Update(Terminal terminal);
    }
}
