// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace NanoGames.Games.Banana
{
    class Landscape
    {
        private bool[,] isSolid = new bool[321, 201];
        private bool[,] isBorder = new bool[321, 201];
        private bool[,] isPoint = new bool[321, 201];
        public List<List<Vector>> Border = new List<List<Vector>>();
        public List<List<Vector>> Normal = new List<List<Vector>>();

        public void InitializePoints(Random r)
        {
            for (int i = 0; i <= 320; i++)
            {
                for (int j = 0; j <= 200; j++)
                {
                    if (isSolid[i, j])
                    {
                        var t = r.NextDouble();

                        if (t < 0.2)
                        {
                            isPoint[i, j] = true;
                        }
                    }

                }
            }
        }

        public void Refresh()
        {
            isBorder = getBorder();
            refreshBorder();
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

        private bool[,] getBorder()
        {
            int n = isSolid.GetLength(0);
            int m = isSolid.GetLength(1);

            bool[,] output = new bool[n, m];

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < m; y++)
                {
                    if (isSolid[x, y])
                    {
                        output[x, y] = ((x < 1) ||
                                        (x > 319) ||
                                        (y < 1) ||
                                        (y > 199) ||
                                        (isSolid[x - 1, y] == false) ||
                                        (isSolid[x + 1, y] == false) ||
                                        (isSolid[x, y - 1] == false) ||
                                        (isSolid[x, y + 1] == false));
                    }
                }
            }

            return output;
        }

        private void refreshBorder()
        {
            Border.Clear();
            Normal.Clear();
            
            int n = isBorder.GetLength(0);
            int m = isBorder.GetLength(1);
            
            bool[,] wasTaken = new bool[n, m];

            for (int x = 0; x < n; x++)
            {
                for (int y = 0; y < m; y++)
                {
                    if (isBorder[x, y] && !wasTaken[x, y])
                    {
                        int xx = x;
                        int yy = y;
                        List<Vector> tmpB = new List<Vector>();
                        
                        int last = 3;
                        int[,] mylist = new int[8, 2] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 } };
                        
                        do
                        {
                            wasTaken[xx, yy] = true;
   
                            tmpB.Add(new Vector(xx, yy));

                            for (int k = 0; k < 8; k++)
                            {
                                int loc = mod(last - 3 + k, 8);

                                if (isBorder[xx + mylist[loc, 0], yy + mylist[loc, 1]])
                                {
                                    xx += mylist[loc, 0];
                                    yy += mylist[loc, 1];
                                    last = loc;
                                    break;
                                }
                            }

                        } while ((xx != x) || (yy != y));

                        // remove duplicates
                        var B = tmpB.Distinct().ToList();

                        // make normals
                        List<Vector> N = new List<Vector>();
                        Vector s;
                        foreach (Vector b in B)
                        {
                            xx = (int)b.X;
                            yy = (int)b.Y;

                            s = new Vector(0, 0);
                            for (int k = -2; k <= 2; k++)
                            {
                                for (int l = -2; l <= 2; l++)
                                {
                                    if (((xx + k) < 320) && ((xx + k) > 0) && ((yy + l) < 200) && ((yy + l) > 0))
                                    {
                                        if (isSolid[xx + k, yy + l])
                                        {
                                            s += new Vector(k, l);
                                        }
                                    }

                                }
                            }

                            N.Add(-s.Normalized);

                        }

                        Normal.Add(N);
                        Border.Add(B);
                                        

                        // add tmpBorder and tmpNormal to Border and Nnormal if they are long enough
                        //if (tmpBorder.Count > 7)
                        //{
                        //    Normal.Add(tmpNormal);
                        //    Border.Add(tmpBorder);
                        //}

                    }
                }
            }
            
        }

        public void makeCaldera(Vector position, int size)
        {
            for (int i = (int)position.X - size + 1; i <= (int)position.X + size + 1; i++)
            {
                for (int j = (int)position.Y - size + 1; j <= (int)position.Y + size + 1; j++)
                {
                    if ((position - new Vector(i, j)).Length <= size)
                    {
                        if ((i >= 0) && (i <= 320) && (j >= 0) && (j <= 200))
                        {
                            isSolid[i, j] = false;
                        }
                    }
                }
            }

            Refresh();
        }

        public bool CheckIsSolid(Vector p)
        {
            int x = (int)p.X;
            int y = (int)p.Y;

            if (isSolid[x, y] ||
                isSolid[x + 1, y] ||
                isSolid[x + 1, y + 1] ||
                isSolid[x, y + 1])
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        public void Draw(IGraphics g)
        {

            //List<Color> colorList = new List<Color> { new Color(0, 0, 1), new Color(0, 1, 1), new Color(1, 1, 1), new Color(1, 0, 1), new Color(1, 1, 0) };

            for (int i = 0; i <= 320; i++)
            {
                for (int j = 0; j <= 200; j++)
                {
                    if (isSolid[i, j] && isPoint[i, j])
                    {
                        g.Point(new Color(1, 1, 1), new Vector(i, j));
                    }
                }
            }

            for (int i = 0; i < Border.Count; i++)
            {
                for (int j = 0; j < Border[i].Count; j++)
                {
                    g.Line(new Color(1, 1, 1), Border[i][j], Border[i][mod(j + 1, Border[i].Count)]);
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

        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public void BuildLandscape(string name)
        {
            /* the first landscape */
            if (name == "first")
            {
                createBlock(new Vector(100, 100), new Vector(130, 110));
                createBlock(new Vector(120, 120), new Vector(190, 300));
                createBlock(new Vector(150, 100), new Vector(300, 130));
            }
            
            if (name == "blocks")
            {
                createBlock(new Vector(30, 60), new Vector(100, 90));
                createBlock(new Vector(150, 100), new Vector(300, 120));
                createBlock(new Vector(110, 160), new Vector(150, 190));
                createBlock(new Vector(230, 160), new Vector(280, 190));
            }

            if (name == "SimpleBlock")
            {
                createBlock(new Vector(100, 100), new Vector(150, 110));
            }

            if (name == "lady")
            {
                readImageToIsSolid(@"H:\Projekte\nanogames\NanoGames.Games\Banana\lady.bmp");
            }

            if (name == "boarder")
            {
                readImageToIsSolid(@"H:\Projekte\nanogames\NanoGames.Games\Banana\boarder.bmp");
            }

            if (name == "skier")
            {
                readImageToIsSolid(@"H:\Projekte\nanogames\NanoGames.Games\Banana\skier.bmp");
            }

            if (name == "airplane")
            {
                readImageToIsSolid(@"H:\Projekte\nanogames\NanoGames.Games\Banana\airplane.bmp");
            }
            if (name == "owls")
            {
                readImageToIsSolid(@"H:\Projekte\nanogames\NanoGames.Games\Banana\owls.bmp");
            }

            Refresh();
        }

        public void BuildLandscapeRandom(Random random)
        {
            List<string> names = new List<string> { "blocks", "lady", "boarder", "skier", "airplane", "owls" };
            int i = Convert.ToInt32(Math.Floor(random.NextDouble() * names.Count));
            BuildLandscape(names[i]);
        }

        private void createBlock(Vector p1, Vector p2)
        {

            int x1 = clamp((int)p1.X, 1, 319);
            int x2 = clamp((int)p2.X, 1, 319);
            int y1 = clamp((int)p1.Y, 1, 199);
            int y2 = clamp((int)p2.Y, 1, 199);

            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    isSolid[i, j] = true;
                }
            }

        }

        private void readImageToIsSolid(string filename)
        {
            Bitmap bmp = new Bitmap(Image.FromFile(filename));

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    var b = bmp.GetPixel(i, j).GetBrightness();

                    if (b < 0.5)
                    {
                        isSolid[i + 10, j + 40] = true;
                    }
                }
            }
        }
    }
}
