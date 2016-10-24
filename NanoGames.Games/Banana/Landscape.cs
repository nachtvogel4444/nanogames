// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    class Landscape
    {
        public int[,] Area = new int[320, 240];
        public List<List<Vector>> Lines = new List<List<Vector>>();

        public void CreateBlock(Vector p1, Vector p2)
        {
            int x1 = (int)p1.X;
            int x2 = (int)p2.X;
            int y1 = (int)p1.Y;
            int y2 = (int)p2.Y;

            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    Area[i, j] = 1;
                }
            }

        }

        public void Make()
        {
            for (int x = 0; x < 320; x++)
            {
                for (int y = 0; y < 240; y++)
                {
                    if (Area[x,y] == 1)
                    {
                        if (isBorder(x, y))
                        {
                            Area[x, y] = 2;
                        }
                    }
                }
            }
        }

        private bool isBorder(int x, int y)
        {
            if ((Area[x - 1, y] == 0) ||
                (Area[x + 1, y] == 0) ||
                (Area[x, y - 1] == 0) || 
                (Area[x, y + 1] == 0))
            {
                return true;
            }
            return false;
        }

        public void Draw(IGraphics g)
        {
            for (int i = 0; i < 320; i++)
            {
                for (int j = 0; j < 240; j++)
                {/*
                    if (Area[i,j] == 1)
                    {
                        g.Point(new Color(1, 1, 1), new Vector(i, j));
                    }
                    */
                    if (Area[i, j] == 2)
                    {
                        g.Point(new Color(1, 0, 0), new Vector(i, j));
                    }
                }
            }
        }

    }
}
