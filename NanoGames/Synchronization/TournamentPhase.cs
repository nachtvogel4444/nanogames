// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Synchronization
{
    /// <summary>
    /// A tournament phase.
    /// </summary>
    internal enum TournamentPhase
    {
        /// <summary>
        /// Unknown/undefined.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Show the lobby.
        /// </summary>
        Lobby = 1,

        /// <summary>
        /// Show the pre-vote countdown.
        /// </summary>
        VoteCountdown = 2,

        /// <summary>
        /// Show the vote.
        /// </summary>
        Vote = 3,

        /// <summary>
        /// Show the pre-match transition.
        /// </summary>
        MatchTransition = 4,

        /// <summary>
        /// Show the pre-match countdown.
        /// </summary>
        MatchCountdown = 5,

        /// <summary>
        /// Show the match.
        /// </summary>
        Match = 6,
    }
}
