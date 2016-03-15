// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Diagnostics;

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Contains constants for the game speed.
    /// </summary>
    internal static class GameSpeed
    {
        /// <summary>
        /// The number of virtual frames per second.
        /// </summary>
        public const int FramesPerSecond = 60;

        /// <summary>
        /// The interval between frames in Stopwatch ticks.
        /// </summary>
        public static readonly long FrameDuration = Stopwatch.Frequency / FramesPerSecond;
    }
}
