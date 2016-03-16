// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Diagnostics;

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Contains duration constants. All values are in Stopwatch ticks.
    /// </summary>
    internal static class Durations
    {
        /// <summary>
        /// The pre-vote countdown duration in Stopwatch ticks.
        /// </summary>
        public static readonly long VoteCountdown = 3 * Stopwatch.Frequency;

        /// <summary>
        /// The vote duration in Stopwatch ticks.
        /// </summary>
        public static readonly long Vote = 10 * Stopwatch.Frequency;

        /// <summary>
        /// The pre-match transition duration in Stopwatch ticks.
        /// </summary>
        public static readonly long MatchTransition = 4 * Stopwatch.Frequency;

        /// <summary>
        /// The pre-match countdown duration in Stopwatch ticks.
        /// </summary>
        public static readonly long MatchCountdown = 3 * Stopwatch.Frequency;
    }
}
