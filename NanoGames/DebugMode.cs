// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Diagnostics;
using System.Linq;

namespace NanoGames
{
    /// <summary>
    /// Determines if the game is being debugged, which modifies some behaviors.
    /// </summary>
    internal static class DebugMode
    {
        private static bool _isEnabled;

        static DebugMode()
        {
            SetDebugMode();

            if (!_isEnabled && Environment.GetCommandLineArgs().Any(a => a?.ToLowerInvariant() == "/debug"))
            {
                _isEnabled = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the game is in debug mode.
        /// </summary>
        public static bool IsEnabled => _isEnabled;

        private static void SetDebugMode()
        {
            if (Debugger.IsAttached)
            {
                _isEnabled = true;
            }
        }
    }
}
