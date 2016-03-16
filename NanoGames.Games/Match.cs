// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a single match of a discipline.
    /// </summary>
    public abstract class Match
    {
        /// <summary>
        /// Gets or sets a value indicating whether the match is completed.
        /// </summary>
        public bool IsCompleted { get; protected set; }

        /// <summary>
        /// Updates and renders the match.
        /// </summary>
        /// <param name="playerDescriptions">The player descriptions.</param>
        public abstract void Update(List<PlayerDescription> playerDescriptions);
    }
}
