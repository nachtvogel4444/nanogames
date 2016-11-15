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
        public List<List<int>> BorderX;
        public List<List<int>> BorderY;

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
            IsBorder = getBorder(IsSolid);
        }

        public void Refresh()
        {
            IsBorder = getBorder(IsSolid);
            refreshBorderList(IsBorder);
        }

        private bool[,] addLayer(bool[,] input, int count)
        {
            if (count <= 0) { return input; }

            bool[,] output = new bool[321, 201];

            for (int x = 1; x < 320; x++)
            {
                for (int y = 1; y < 200; y++)
                {
                    if (input[x, y])
                    {
                        output[x, y - 1] = true;
                        output[x + 1, y - 1] = true;
                        output[x + 1, y] = true;
                        output[x + 1, y + 1] = true;
                        output[x, y + 1] = true;
                        output[x - 1, y + 1] = true;
                        output[x - 1, y] = true;
                        output[x - 1, y - 1] = true;
                        output[x, y] = true;
                    }
                }
            }

            return addLayer(output, count - 1);
        }

        private bool[,] getBorder(bool[,] input)
        {
            int n = input.GetLength(0);
            int m = input.GetLength(1);

            bool[,] output = new bool[n, m];

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < m; y++)
                {
                    if (input[x, y])
                    {
                        output[x, y] = ((x < 1) ||
                                        (x > 319) ||
                                        (y < 1) ||
                                        (y > 199) ||
                                        (input[x - 1, y] == false) ||
                                        (input[x + 1, y] == false) ||
                                        (input[x, y - 1] == false) ||
                                        (input[x, y + 1] == false));
                    }
                }
            }

            return output;
        }

        private void refreshBorderList(bool[,] input)
        {
            BorderX.Clear();
            BorderY.Clear();

            int n = input.GetLength(0);
            int m = input.GetLength(1);
            
            bool[,] tmp = new bool[n, m];

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < m; y++)
                {
                    if (input[x, y] && !tmp[x, y])
                    {
                        int xx = x;
                        int yy = y;
                        BorderX.Add(new List<int>());
                        BorderY.Add(new List<int>());

                        while (!tmp[xx, yy])
                        {
                            BorderX[BorderX.Count - 1].Add(xx);
                            BorderY[BorderX.Count - 1].Add(yy);
                            tmp[xx, yy] = true;

                            if (input[xx + 1, yy] && tmp[xx + 1, yy]) { xx++; continue; }
                            if (input[xx + 1, yy + 1] && tmp[xx + 1, yy + 1]) { xx++; yy++; continue; }
                            if (input[xx, yy + 1] && tmp[xx, yy + 1]) { yy++; continue; }
                            if (input[xx - 1, yy + 1] && tmp[xx - 1, yy + 1]) { xx--; yy++; continue; }
                            if (input[xx - 1, yy] && tmp[xx - 1, yy]) { xx--; continue; }
                            if (input[xx - 1, yy - 1] && tmp[xx - 1, yy - 1]) { xx--; yy--; continue; }
                            if (input[xx, yy - 1] && tmp[xx, yy - 1]) { yy--; continue; }
                            if (input[xx + 1, yy - 1] && tmp[xx + 1, yy - 1]) { xx++; yy--; continue; }
                        }

                        
                    }
                }
            }
            
        } 

        public void Draw(IGraphics g)
        {
            for (int i = 0; i <= 320; i++)
            {
                for (int j = 0; j <= 200; j++)
                {
                    if (IsSolid[i,j])
                    {
                        // g.Point(new Color(0.3, 0.3, 0.3), new Vector(i, j));
                    }
                    
                    if (IsBorder[i, j])
                    {
                        g.Point(new Color(1, 0, 0), new Vector(i, j));
                    }

                    if (IsShell[i, j])
                    {
                        g.Point(new Color(0, 1, 0), new Vector(i, j));
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
