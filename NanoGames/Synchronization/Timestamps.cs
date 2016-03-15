// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Contains timestamps for various round transition points. All timestamps are Stopwatch timestamp offsets of the round start timestamp.
    /// </summary>
    internal static class Timestamps
    {
        /// <summary>
        /// The timestamp where the vote starts.
        /// </summary>
        public static readonly long VoteStart = Durations.VoteCountdown;

        /// <summary>
        /// The timestamp where the vote ends and the pre-match transition starts.
        /// </summary>
        public static readonly long MatchTransitionStart = VoteStart + Durations.Vote;

        /// <summary>
        /// The timestamp where the pre-match transition ends and the pre-match countdown starts.
        /// </summary>
        public static readonly long MatchCountdownStart = MatchTransitionStart + Durations.MatchTransition;

        /// <summary>
        /// The timestamp where the pre-match countdown ends and the match starts.
        /// </summary>
        public static readonly long MatchStart = MatchCountdownStart + Durations.MatchCountdown;
    }
}
