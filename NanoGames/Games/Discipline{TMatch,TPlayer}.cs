// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a concrete game discipline.
    /// </summary>
    /// <typeparam name="TMatch">The match type.</typeparam>
    /// <typeparam name="TPlayer">The player type.</typeparam>
    internal sealed class Discipline<TMatch, TPlayer> : Discipline
        where TMatch : Match<TPlayer>, new()
        where TPlayer : Player<TMatch>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Discipline{TMatch, TPlayer}"/> class.
        /// </summary>
        /// <param name="name">The name of the discipline.</param>
        public Discipline(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Creates a new match for the discipline.
        /// </summary>
        /// <param name="description">The match description.</param>
        /// <returns>The new match.</returns>
        public override Match CreateMatch(MatchDescription description)
        {
            var match = new TMatch();

            var players = new List<TPlayer>();
            foreach (var playerDescription in description.Players)
            {
                var player = new TPlayer();
                player.Match = match;
                CopyPlayerValues(playerDescription, player);
                players.Add(player);
            }

            match.SetPlayers(description.LocalPlayerIndex, players);

            match.Initialize();
            foreach (var player in players)
            {
                player.Initialize();
            }

            return match;
        }

        private void CopyPlayerValues(Player from, Player to)
        {
            to.Index = from.Index;
            to.Color = from.Color;
        }
    }
}
