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
        public Pixel Right;
        public Pixel Left;
        public Vector Position;
        public Vector Normal;
        public double Alpha;
        public bool Exists;
        public bool IsBorder;

        public Pixel(int i, int j)
        {
            I = i;
            J = j;
            IsSolid = false;
            IsBorder = false;
            Position = new Vector(0, 0);
            Normal = new Vector(0, 0);
            Alpha = 0;
            Line = new Segment(new Vector(0, 0), new Vector(1, 1));
            Exists = true;
        }
        
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
