// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;

namespace NanoGames.Games.Banana
{
    internal class Map
    {
        public bool[,] IsPixel = new bool[320, 200];
        public List<PixelMap> ListPixel = new List<PixelMap> { };
        public List<PixelMap> ListOutlinePixel = new List<PixelMap> { };

        private List<Vector> cw = new List<Vector> { new Vector(-1, 1), new Vector(0, 1), new Vector(1, 1), new Vector(1, 0), new Vector(1, -1), new Vector(0, -1), new Vector(-1, -1), new Vector(-1, 0), };


        public void Refresh()
        {
            refreshNeighbors();
            refreshOutlinePixels();
        }

        private bool checkPixel(Vector p)
        {
            int x = (int)p.X;
            int y = (int)p.Y;

            if (IsPixel[x, y])
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        private void refreshNeighbors()
        {
            foreach (PixelMap pixel in ListPixel)
            {
                pixel.Neighbors.Clear();

                foreach (Vector p in cw)
                {
                    if (checkPixel(pixel.Position + p))
                    {
                        pixel.Neighbors.Add(p);
                    }
                }
            }

        }

        private void refreshOutlinePixels()
        {

        }

    }
}
