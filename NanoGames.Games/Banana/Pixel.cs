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

        public int I = 0;
        public int J = 0;
        public Vector Left;
        public Vector Right;
        public Vector Normal;
        public int Neighbors;
        public bool Exists;

        public Pixel(int i, int j)
        {
            I = i;
            J = j;
            IsSolid = false;
            Left = new Vector(0, 0);
            Right = new Vector(0, 0);
            Normal = new Vector(0, 0);
            Neighbors = 0;
            Line = new Segment(new Vector(0, 0), new Vector(1, 1));
            Exists = true;
        }

        public bool IsBorder => (IsSolid && Neighbors < 8);
        
        public void DrawLine(IGraphics g, Color c)
        {
            if (IsBorder)
            {
                Line.Draw(g, c);
                // Line.DrawDebug(g, c);
            }
        }

    }
}
