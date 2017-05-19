// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;

namespace NanoGames.Games.Banana
{
    internal class Pixel
    {
        public Segment Line;

        public bool IsSolid;

        //public VectorInt Position;
        public Vector Left;
        public Vector Right;
        public int Neighbors;

        public void DrawLine(IGraphics g, Color c)
        {
            Line.Draw(g, c);
        }

    }
}
