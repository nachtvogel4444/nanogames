// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a game discipline.
    /// </summary>
    public abstract class Discipline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Discipline"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Discipline(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new match.
        /// </summary>
        /// <param name="description">The match description.</param>
        /// <returns>The new match.</returns>
        public abstract IMatch CreateMatch(MatchDescription description);
    }
}
