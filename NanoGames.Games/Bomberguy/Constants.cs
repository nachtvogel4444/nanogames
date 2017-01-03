// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Bomberguy
{

    /// <summary>
    /// Bomberguy game settings
    /// </summary>
    class Constants
    {
        /// <summary>
        /// Settings related to Bombs
        /// </summary>
        public static class Bomb
        {
            /// <summary>
            /// Bomb explosion reach in cells
            /// </summary>
            public const int REACH = 2;
            
            /// <summary>
            /// Bomb size relative to cell size
            /// </summary>
            public const double REL_SIZE = 1;
        }

        /// <summary>
        /// Settings related to BombergGuys
        /// </summary>
        public static class BomberGuy
        {
            /// <summary>
            /// Player speed
            /// </summary>
            public const double SPEED = 9;
            
            /// <summary>
            /// Player size relative to cell size
            /// </summary>
            public const double REL_SIZE = .9;
        }

        /// <summary>
        /// Settings related to BomberMatch
        /// </summary>
        public static class BomberMatch
        {
            /// <summary>
            /// Minimal width and height of the game field in cells
            /// </summary>
            public const int FIELD_MIN_SIZE = 9;
            
            /// <summary>
            /// The probability for an empty cell to initially contain a destroyable obstacle
            /// </summary>
            public const double BOMBSTACLE_PROBABILITY = 0.5;
        }
    }
}
