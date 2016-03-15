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
        /// The round countdown duration in Stopwatch ticks.
        /// </summary>
        public static readonly long Countdown = 5 * Stopwatch.Frequency;

        /// <summary>
        /// The vote duration in Stopwatch ticks.
        /// </summary>
        public static readonly long Vote = 15 * Stopwatch.Frequency;

        /// <summary>
        /// The pre-match preparation duration in Stopwatch ticks.
        /// </summary>
        public static readonly long Preparation = 5 * Stopwatch.Frequency;
    }
}
