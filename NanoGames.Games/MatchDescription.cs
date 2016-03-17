// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games
{
    /// <summary>
    /// Describes the players and circumstances of a match.
    /// Used by <see cref="Discipline.CreateMatch(MatchDescription)"/> to create a new match instance.
    /// </summary>
    public sealed class MatchDescription
    {
        /// <summary>
        /// Gets or sets the list of players.
        /// </summary>
        public List<PlayerDescription> Players { get; set; } = new List<PlayerDescription>();

        public Random Random { get; set; }
    }
}
