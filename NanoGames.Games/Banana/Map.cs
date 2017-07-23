// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;

namespace NanoGames.Games.Banana
{
    internal class Map
    {
        private double epsilon = 0.0001;
        public Pixel[,] PixelMap = new Pixel[320, 200];
        private int xMin = 10;
        private int xMax = 310;
        private int yMin = 40;
        private int yMax = 190;

        private int[] neighborsX = new int[8] { -1, 0, 1, 1, 1, 0, -1, -1 };
        private int[] neighborsY = new int[8] { -1, -1, -1, 0, 1, 1, 1, 0 };

        private Intersection intersection = new Intersection(false);
        


        public void Initialize()
        {

            for (int i = 0; i < 320; i++)
            {
                for (int j = 0; j < 200; j++)
                {
                    PixelMap[i, j] = new Pixel(i, j);
                }
            }
            
            createBlock(new Vector(50, 80), new Vector(200, 150));
            updateMap();            
        }
        

        private void updateMap()
        {
            removeAllSinglePixels();
            updatePixel();
        }

        private void removeAllSinglePixels()
        {
            bool keepSearching = true;

            while (keepSearching)
            {
                keepSearching = false;

                for (int i = xMin; i < xMax; i++)
                {
                    for (int j = yMin; j < yMax; j++)
                    {
                        var pixel = PixelMap[i, j];

                        if (pixel.IsSolid)
                        {
                            // get list with IsSolid of neighbors
                            List<bool> neighborIsSolid = new List<bool> { };
                            for (int k = 0; k < 8; k++)
                            {
                                if (PixelMap[i + neighborsX[k], j + neighborsY[k]].IsSolid) { neighborIsSolid.Add(true); }
                                else { neighborIsSolid.Add(false); }
                            }
                            // add first element to end to circumvent the mod shit
                            neighborIsSolid.Add(neighborIsSolid[0]);

                            // test if two consecutive pixels are solid
                            bool solid = false;
                            for (int k = 0; k < 8; k++)
                            {
                                if (neighborIsSolid[k] && neighborIsSolid[k + 1])
                                {
                                    solid = true;
                                }
                            }

                            if (!solid)
                            {
                                pixel.IsSolid = false;
                                pixel.IsBorder = false;
                                keepSearching = true;
                            }

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
                    
                    if (pixel.IsSolid)
                    {
                        // get list with IsSolid of neighbors
                        List<bool> neighborIsSolid = new List<bool> { };
                        for (int k = 0; k < 8; k++)
                        {
                            if (PixelMap[i + neighborsX[k], j + neighborsY[k]].IsSolid)
                            {
                                neighborIsSolid.Add(true);
                            }
                            else
                            {
                                neighborIsSolid.Add(false);
                            }
                        }
                        // add first element to end to circumvent the mod shit
                        neighborIsSolid.Add(neighborIsSolid[0]);

                        // extract transition from solid to not solid
                        int in2out = -1;
                        int out2in = -1;
                        for (int k = 0; k < 8; k++)
                        {
                            if (neighborIsSolid[k] && !neighborIsSolid[k + 1]) { in2out = k; pixel.IsBorder = true; }
                            if (!neighborIsSolid[k] && neighborIsSolid[k + 1]) { out2in = k; pixel.IsBorder = true; }
                            
                        }
                        
                        // if pixel is border do all the stuff which player needs to know
                        if (pixel.IsBorder)
                        {
                            // pixel left and right
                            pixel.Left = PixelMap[i + neighborsX[in2out], j + neighborsY[in2out]];
                            pixel.Right = PixelMap[i + neighborsX[mod(out2in + 1, 8)], j + neighborsY[mod(out2in + 1, 8)]];

                            // connection line between middle of pixel and righthand side pixel
                            pixel.Line = new Segment(new Vector(i + 0.5, j + 0.5), new Vector(pixel.Right.I + 0.5, pixel.Right.J + 0.5));

                            // position for player
                            pixel.Position = pixel.Line.MidPoint;

                            // normal for player
                            pixel.Normal = pixel.Line.Normal;

                            // angle of line
                            pixel.Alpha = pixel.Line.AngleNormal;
                        }
                    }

                    else
                    {
                        // just to be save
                        pixel.IsBorder = false;
                    }             
                }
            }
        }

        public Pixel GetRandomBorderPixel(double r)
        {
            // count all border pixels
            List<Pixel> border = new List<Pixel> { };

            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    if (PixelMap[i, j].IsBorder)
                    {
                        border.Add(PixelMap[i, j]);
                    }
                }
            }

            // get random border position
            return border[(int)(r * border.Count)];

        }


        public Intersection CheckForHit(Segment s, int sizeCaldera)
        {
            Intersection intersection = checkIntersection(s);
            
            if (intersection.IsTrue)
            {
                MakeCaldera(intersection.Point, sizeCaldera);
            }
            
            return intersection;
        }

        private Intersection checkIntersection(Segment segmentIn)
        {
            List<int> bb = segmentIn.BBoxInt;
            Intersection intersection0 = new Intersection(false);
            double dist0 = double.MaxValue;

            // go through all pixels in bounding box
            for (int i = bb[0] - 1; i <= bb[1]; i++)
            {
                for (int j = bb[2] - 1; j <= bb[3]; j++)
                {
                    Pixel pixel = GetPixel(i, j);

                    if (pixel.IsBorder)
                    {
                        var t = 1;
                    }

                    if (pixel.Exists && pixel.IsBorder)
                    {
                        Intersection intersection = new Intersection(segmentIn, pixel.Line);

                        if (intersection.IsTrue)
                        {
                            double dist = (intersection.Point - segmentIn.Start).SquaredLength;

                            if (dist < dist0)
                            {
                                intersection0 = intersection;
                                dist0 = dist;
                            }
                        }
                    }
                }
            }

            return intersection0;
        }

        public void MakeCaldera(Vector pos, int size)
        {

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
                            PixelMap[i, j].IsBorder = false;
                        }
                    }
                }
            }

            updateMap();

        }
       
        public Pixel GetPixel(int i, int j)
        {
            if (i < 0 || i > 319 || j < 0 || j > 199 )
            {
                Pixel pixel = new Pixel(i, j);
                pixel.Exists = false;
                return pixel;
            }
            else
            {
                return PixelMap[i, j];
            }
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
