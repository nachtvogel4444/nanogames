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
        public bool[,] IsSolid = new bool[321, 201];
        public bool[,] IsBorder = new bool[321, 201];
        public List<List<int[]>> Borders = new List<List<int[]>>();

        public void CreateBlock(Vector p1, Vector p2)
        {
            
            int x1 = clamp((int)p1.X, 0, 320);
            int x2 = clamp((int)p2.X, 0, 320);
            int y1 = clamp((int)p1.Y, 0, 200);
            int y2 = clamp((int)p2.Y, 0, 200);

            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    IsSolid[i, j] = true;
                }
            }

        }

        public void Make()
        {
            List<int[]> tmp1 = new List<int[]>();
            List<int[]> tmp2 = new List<int[]>();

            /* Fill IsBorder */
            for (int x = 0; x <= 320; x++)
            {
                for (int y = 0; y <= 200; y++)
                {
                    if (IsSolid[x,y])
                    {
                        if (checkForBorder(x, y))
                        {
                            IsBorder[x, y] = true;
                            tmp1.Add(new int[2] { x, y });
                        }
                    }
                }
            }

            /* Fill Lines */
            Borders.Add(new List<int[]>());
            foreach (int[] bp in tmp1)
            {
                if (!tmp2.Contains(bp))
                {
                    int[] p = bp;
                    Borders.Last().Add(p);

                    while (p != findNewNeighbour(p, tmp2))
                    {
                        p = findNewNeighbour(p, tmp2);
                        Borders.Last().Add(p);
                        tmp2.Add(p);
                    }
                }
                
               
            }
            
        }

        private bool checkForBorder(int x, int y)
        {
            if ((x < 1) || (x > 319) ||
                (y < 1) || (y > 199))
            {
                return true;
            }

            return ((IsSolid[x - 1, y] == false) ||
                (IsSolid[x + 1, y] == false) ||
                (IsSolid[x, y - 1] == false) ||
                (IsSolid[x, y + 1] == false));
        }

        private int[] findNewNeighbour(int[] p, List<int[]> l)
        {
            int x = p[0];
            int y = p[1];

            if (IsBorder[x + 1, y] && !l.Contains(new int[2] { x + 1, y })) { return new int[2] { x + 1, y }; }
            if (IsBorder[x + 1, y + 1] && !l.Contains(new int[2] { x + 1, y + 1 })) { return new int[2] { x + 1, y + 1 }; }
            if (IsBorder[x, y + 1] && !l.Contains(new int[2] { x, y + 1 })) { return new int[2] { x, y + 1 }; }
            if (IsBorder[x - 1, y + 1] && !l.Contains(new int[2] { x - 1, y + 1 })) { return new int[2] { x - 1, y + 1 }; }
            if (IsBorder[x - 1, y] && !l.Contains(new int[2] { x - 1, y })) { return new int[2] { x - 1, y }; }
            if (IsBorder[x - 1, y - 1] && !l.Contains(new int[2] { x - 1, y - 1 })) { return new int[2] { x - 1, y - 1 }; }
            if (IsBorder[x, y - 1] && !l.Contains(new int[2] { x, y - 1 })) { return new int[2] { x, y - 1 }; }
            if (IsBorder[x + 1, y - 1] && !l.Contains(new int[2] { x + 1, y - 1 })) { return new int[2] { x + 1, y - 1 }; }

            return p;
        }

        public void Draw(IGraphics g)
        {
            for (int i = 0; i <= 320; i++)
            {
                for (int j = 0; j <= 200; j++)
                {
                    if (IsSolid[i,j])
                    {
                        // g.Point(new Color(1, 1, 1), new Vector(i, j));
                    }
                    
                    if (IsBorder[i, j])
                    {
                        g.Point(new Color(1, 0, 0), new Vector(i, j));
                    }
                }
            }
        }

        private T clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;

            return value;
        }

    }
}
