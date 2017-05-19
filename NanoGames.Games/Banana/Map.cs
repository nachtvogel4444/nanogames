// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;

namespace NanoGames.Games.Banana
{
    internal class Map
    {
        private double epsilon = 0.0001;
        private Pixel[,] PixelMap = new Pixel[320, 200];
        private int xMin = 10;
        private int xMax = 310;
        private int yMin = 40;
        private int yMax = 190;

        private int[] neighborsX = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };
        private int[] neighborsY = new int[8] { -1, -1, -1, 0, 1, 1, 1, 0 };

        private Intersection intersection = new Intersection(false);
        


        public void Initialize()
        {
            // fill Pixels with true
            createBlock(new Vector(50, 80), new Vector(200, 150));
            updateMap();            
        }
        

        private void updateMap()
        {
            updateNumberOfNeighbours();
            removeAllSinglePixels();
            updatePixel();
        }

        private void updateNumberOfNeighbours()
        {
            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    var pixel = PixelMap[i, j];
                    pixel.Neighbors = 0;

                    if (PixelMap[i - 1, j - 1].IsSolid) { pixel.Neighbors++; }
                    if (PixelMap[i, j - 1].IsSolid) { pixel.Neighbors++; }
                    if (PixelMap[i + 1, j - 1].IsSolid) { pixel.Neighbors++; }
                    if (PixelMap[i + 1, j].IsSolid) { pixel.Neighbors++; }
                    if (PixelMap[i + 1, j + 1].IsSolid) { pixel.Neighbors++; }
                    if (PixelMap[i, j + 1].IsSolid) { pixel.Neighbors++; }
                    if (PixelMap[i - 1, j + 1].IsSolid) { pixel.Neighbors++; }
                    if (PixelMap[i - 1, j].IsSolid) { pixel.Neighbors++; }

                }
            }
        }

        private void removeAllSinglePixels()
        {
            bool test = true;

            while (test)
            {
                test = false;

                for (int i = xMin; i < xMax; i++)
                {
                    for (int j = yMin; j < yMax; j++)
                    {
                        var pixel = PixelMap[i, j];

                        if (pixel.Neighbors <= 1 && pixel.IsSolid)
                        {
                            pixel.IsSolid = false;
                            test = true;
                        }
                    }
                }
            }
        }

        private void updatePixel()
        {

            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    var pixel = PixelMap[i, j];

                    int in2out = 0;
                    int out2in = 0;

                    for (int k = 0; k < 8; k++)
                    {
                        int kk = mod(k, 8);

                        if (PixelMap[i + neighborsX[kk], j + neighborsY[kk]].IsSolid != PixelMap[i + neighborsX[kk + 1], j + neighborsY[kk + 1]].IsSolid)
                        {
                            if (PixelMap[i + neighborsX[kk], j + neighborsY[kk]].IsSolid)
                            {
                                in2out = kk;
                            }
                            else
                            {
                                out2in = kk;
                            }
                        }
                    }

                    pixel.Right = new Vector(neighborsX[out2in + 1], neighborsY[out2in + 1]);
                    pixel.Line = new Segment(new Vector(i, j), pixel.Right);
                    pixel.Left = new Vector(neighborsX[in2out], neighborsY[in2out]);

                }
            }
        }


        public Intersection CheckForHit(Segment s, int sizeCaldera)
        {
            checkIntersection(s);
            
            if (intersection.IsTrue)
            {
                makeCaldera(sizeCaldera);
            }
            
            return intersection;
        }

        private void checkIntersection(Segment segmentIn)
        {
            // bounding box
            List<int> bb = segmentIn.BBoxInt;

            // go through all pixels in bbox
            List<Intersection> intersections = new List<Intersection> { };

            for (int i = bb[0]; i <= bb[1]; i++)
            {
                for (int j = bb[2]; j <= bb[3]; j++)
                {
                    Intersection tmpIntersection = new Intersection(segmentIn, PixelMap[i, j].Line);

                    if (tmpIntersection.IsTrue)
                    {
                        intersections.Add(tmpIntersection);
                    }
                   
                }
            }

            // look for nearest intersection to end of segmentIn
            if (intersections.Count != 0)
            {
                double smallestDist = (intersections[0].Point - segmentIn.End).SquaredLength;
                double dist;
                int count = 0;

                for (int i = 1; i < intersections.Count; i++)
                {
                    dist = (intersections[i].Point - segmentIn.End).SquaredLength;

                    if (dist < smallestDist)
                    {
                        smallestDist = dist;
                        count = i;
                    }
                }

                intersection = intersections[count];
            } 

            else
            {
                intersection.IsTrue = false;
            }

        }

        private void makeCaldera(int size)
        {
            Vector pos = intersection.Point;

            int x = (int)Math.Round(pos.X);
            int y = (int)Math.Round(pos.Y);

            for (int i = x - size; i <= x + size; i++)
            {
                for (int j = y - size; j <= y + size; j++)
                {
                    if ((pos - new Vector(i, j)).Length <= size)
                    {
                        if ((i >= xMin) && (i <= xMax) && (j >= yMin) && (j <= yMax))
                        {
                            PixelMap[i, j].IsSolid = false;
                        }
                    }
                }
            }

            updateMap();

        }


        public Vector GoLeft(Vector pos)
        {
            int x = intX(pos);
            int y = intY(pos);
            
            return PixelMap[x,y].Left;
        }

        public Vector GoRight(Vector pos)
        {
            int x = intX(pos);
            int y = intY(pos);

            return PixelMap[x, y].Right;
        }

        public Vector GoLeftLeft(Vector pos)
        {
            Vector left = GoLeft(pos);

            int x = intX(left);
            int y = intY(left);
            
            return PixelMap[x, y].Left;
        }

        public Vector GoRightRight(Vector pos)
        {
            Vector right = GoRight(pos);

            int x = intX(right);
            int y = intY(right);

            return PixelMap[x, y].Left;
        }


        private void createBlock(Vector p1, Vector p2)
        {

            int x1 = clamp((int)p1.X, xMin, xMax);
            int x2 = clamp((int)p2.X, xMin, xMax);
            int y1 = clamp((int)p1.Y, yMin, yMax);
            int y2 = clamp((int)p2.Y, yMin, yMax);

            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    PixelMap[i, j].IsSolid = true;
                }
            }

        }

        public void Draw(IGraphics g, Color c)
        {
            // Draws every line of every pixel

            foreach (Pixel pixel in PixelMap)
            {
                pixel.DrawLine(g, c);
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

        private int intX(Vector v)
        {
            if (Math.Abs(v.X % 1) <= epsilon)
            {
                return Convert.ToInt32(v.X);
            }
            else
            {
                throw new System.ArgumentException("Vector.X has no integer behaviour!");
            }
        }

        private int intY(Vector v)
        {
            if (Math.Abs(v.Y % 1) <= epsilon)
            {
                return Convert.ToInt32(v.Y);
            }
            else
            {
                throw new System.ArgumentException("Vector.Y has no integer behaviour!");
            }
        }

    }
}
