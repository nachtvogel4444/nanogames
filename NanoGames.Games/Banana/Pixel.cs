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
        public Vector Normal;
        public int Neighbors;

        public Pixel()
        {
            IsSolid = false;
            Left = new Vector(0, 0);
            Right = new Vector(0, 0);
            Normal = new Vector(0, 0);
            Neighbors = 0;
            Line = new Segment(new Vector(0, 0), new Vector(1, 1));
        }

        public bool IsBorder()
        {
            if (IsSolid && Neighbors < 8)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public void DrawLine(IGraphics g, Color c)
        {
            if (IsBorder())
            {
                // Line.Draw(g, c);
                Line.DrawDebug(g, c);
            }
        }

    }
}
