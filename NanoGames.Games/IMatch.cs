// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a single match of a discipline.
    /// </summary>
    public interface IMatch
    {
        /// <summary>
        /// Gets or sets a value indicating whether the match is completed.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets the current scores for all players.
        /// </summary>
        IEnumerable<double> PlayerScores { get; }

        IReadOnlyList<Player> Players { get; }

        /// <summary>
        /// Updates and renders the match.
        /// </summary>
        /// <param name="matchGraphics">A Graphics instance used to draw onto the screen of all players.</param>
        /// <param name="playerDescriptions">The player descriptions.</param>
        void Update(Graphics matchGraphics, List<PlayerDescription> playerDescriptions);
    }
}
