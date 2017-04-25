// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;

namespace NanoGames.Games.Banana
{
    internal class Pixel
    {
        public List<Segment> Lines;

        public bool IsSolid;

        //public VectorInt Position;
        public VectorInt Left;
        public VectorInt Right;
        public int Neighbors;

    }
}
