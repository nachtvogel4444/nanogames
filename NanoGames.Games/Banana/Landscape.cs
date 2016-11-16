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
        public List<List<Vector>> Border = new List<List<Vector>>();

        public void CreateBlock(Vector p1, Vector p2)
        {
            
            int x1 = clamp((int)p1.X, 1, 319);
            int x2 = clamp((int)p2.X, 1, 319);
            int y1 = clamp((int)p1.Y, 1, 199);
            int y2 = clamp((int)p2.Y, 1, 199);

            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    IsSolid[i, j] = true;
                }
            }

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
            Border.Clear();

            int n = input.GetLength(0);
            int m = input.GetLength(1);
            
            bool[,] wasTaken = new bool[n, m];

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < m; y++)
                {
                    if (input[x, y] && !wasTaken[x, y])
                    {
                        int xx = x;
                        int yy = y;
                        Border.Add(new List<Vector>());

                        while (!wasTaken[xx, yy])
                        {
                            Border[Border.Count - 1].Add(new Vector(xx, yy));

                            wasTaken[xx, yy] = true;

                            if (input[xx + 1, yy] && !wasTaken[xx + 1, yy]) { xx++; continue; }
                            if (input[xx + 1, yy + 1] && !wasTaken[xx + 1, yy + 1]) { xx++; yy++; continue; }
                            if (input[xx, yy + 1] && !wasTaken[xx, yy + 1]) { yy++; continue; }
                            if (input[xx - 1, yy + 1] && !wasTaken[xx - 1, yy + 1]) { xx--; yy++; continue; }
                            if (input[xx - 1, yy] && !wasTaken[xx - 1, yy]) { xx--; continue; }
                            if (input[xx - 1, yy - 1] && !wasTaken[xx - 1, yy - 1]) { xx--; yy--; continue; }
                            if (input[xx, yy - 1] && !wasTaken[xx, yy - 1]) { yy--; continue; }
                            if (input[xx + 1, yy - 1] && !wasTaken[xx + 1, yy - 1]) { xx++; yy--; continue; }
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
                        // g.Point(new Color(1, 0, 0), new Vector(i, j));
                    }
                }
            }

            foreach (List<Vector> piece in Border)
            {
                for (int i = 0; i < piece.Count - 1; i++)
                {
                    g.Line(new Color(1, 1, 1), piece[i], piece[i + 1]);
                }
                
                g.Line(new Color(1, 1, 1), piece[piece.Count - 1], piece[0]);
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
