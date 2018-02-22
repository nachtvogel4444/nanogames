// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Infinity
{
    internal class Tile
    {
        // a tile is rectangular region (320x200) of space

        public Vector Position;
        public List<Vector> Stars = new List<Vector> { };

        public List<InfinityPlayer> Players = new List<InfinityPlayer> { };

        public int X;
        public int Y;

    }
}
