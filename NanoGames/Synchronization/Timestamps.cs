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
        public static readonly long VoteStart = Durations.Countdown;

        /// <summary>
        /// The timestamp where the vote ends and the pre-match preparation starts.
        /// </summary>
        public static readonly long PreparationStart = VoteStart + Durations.Vote;

        /// <summary>
        /// The timestamp where the match starts.
        /// </summary>
        public static readonly long MatchStart = PreparationStart + Durations.Preparation;
    }
}
