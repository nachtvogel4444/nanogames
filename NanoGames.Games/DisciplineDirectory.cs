// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames.Games
{
    /// <summary>
    /// A directory of game disciplines.
    /// </summary>
    public static class DisciplineDirectory
    {
        private static readonly List<Discipline> _disciplines = new List<Discipline>();

        static DisciplineDirectory()
        {
            /* Note: names must be in ALL-CAPS to render correctly. */
            Add<Example.ExampleMatch, Example.ExamplePlayer>("INTO THE SQUARE");
            Add<FallingBlocks.FallingBlocksMatch, FallingBlocks.FallingBlocksPlayer>("FALLING BLOCKS");
            Add<Bomberguy.BomberMatch, Bomberguy.BomberGuy>("BOMBERGUY");
            Add<NanoSoccer.NanoSoccerMatch, NanoSoccer.NanoSoccerPlayer>("NANO SOCCER");
        }

        /// <summary>
        /// Gets the list of disciplines.
        /// </summary>
        public static IReadOnlyList<Discipline> Disciplines => _disciplines.AsReadOnly();

        private static void Add<TMatch, TPlayer>(string name)
            where TMatch : Match<TPlayer>, new()
            where TPlayer : Player<TMatch>, new()
        {
            _disciplines.Add(new Discipline<TMatch, TPlayer>(name));
        }
    }
}
