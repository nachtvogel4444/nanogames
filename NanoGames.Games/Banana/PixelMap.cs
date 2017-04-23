// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;

namespace NanoGames.Games.Banana
{
    internal class PixelMap
    {
        private Pixel[,] Pixels = new Pixel[320, 200];
        private int xMin = 10;
        private int xMax = 310;
        private int yMin = 40;
        private int yMax = 190;

    
        public void InitializePixels()
        {
            // fill Pixels with true
            createBlock(new Vector(50, 80), new Vector(200, 150));
            UpdatePixelLines();            
        }

        public void UpdatePixelLines()
        {
            // updates all lines from each pixel by looking at the 8 neighbouring pixels

            for (int i = xMin; i < xMax; i++)
            {
                for (int j = yMin; j < yMax; j++)
                {
                    var pixel = Pixels[i, j];

                    // remove all lines
                    pixel.Lines.Clear();

                    // add lines according to neighbours

                    // left vertical line
                    if (pixel.IsSolid &&
                        !Pixels[i - 1, j - 1].IsSolid &&
                        !Pixels[i - 1, j].IsSolid &&
                        !Pixels[i - 1, j + 1].IsSolid)
                    {
                        pixel.Lines.Add(new Segment(new Vector(i, j), new Vector(i, j + 1)));
                    }

                    // right vertical line
                    if (pixel.IsSolid &&
                        !Pixels[i + 1, j - 1].IsSolid &&
                        !Pixels[i + 1, j].IsSolid &&
                        !Pixels[i + 1, j + 1].IsSolid)
                    {
                        pixel.Lines.Add(new Segment(new Vector(i + 1, j), new Vector(i + 1, j + 1)));
                    }

                    // top horizontal line
                    if (pixel.IsSolid &&
                        !Pixels[i - 1, j - 1].IsSolid &&
                        !Pixels[i, j - 1].IsSolid &&
                        !Pixels[i + 1, j - 1].IsSolid)
                    {
                        pixel.Lines.Add(new Segment(new Vector(i, j), new Vector(i + 1, j)));
                    }

                    // right vertical line
                    if (pixel.IsSolid &&
                        !Pixels[i - 1, j + 1].IsSolid &&
                        !Pixels[i, j + 1].IsSolid &&
                        !Pixels[i + 1, j + 1].IsSolid)
                    {
                        pixel.Lines.Add(new Segment(new Vector(i, j + 1), new Vector(i + 1, j + 1)));
                    }

                    // diagonals
                    bool slashTop = Pixels[i - 1, j].IsSolid && Pixels[i, j - 1].IsSolid;
                    bool backslashTop = Pixels[i, j - 1].IsSolid && Pixels[i + 1, j].IsSolid;
                    bool slashBot = Pixels[i + 1, j].IsSolid && Pixels[i, j + 1].IsSolid;
                    bool backslashBot = Pixels[i - 1, j].IsSolid && Pixels[i, j + 1].IsSolid;

                    // slash
                    if (!pixel.IsSolid &&
                        (!slashTop && slashBot) ||
                        (slashTop && !slashBot))
                    {
                        pixel.Lines.Add(new Segment(new Vector(i, j + 1), new Vector(i + 1, j)));
                    }

                    // backslash
                    if (!pixel.IsSolid &&
                        (backslashTop && !backslashBot) ||
                        (!backslashTop && backslashBot))
                    {
                        pixel.Lines.Add(new Segment(new Vector(i, j), new Vector(i + 1, j + 1)));
                    }

                    // one pixel cavity

                    // top side open
                    if (!pixel.IsSolid &&
                        !Pixels[i, j - 1].IsSolid &&
                        Pixels[i - 1, j].IsSolid &&
                        Pixels[i + 1, j].IsSolid &&
                        Pixels[i, j + 1].IsSolid)
                    {
                        pixel.Lines.Add(new Segment(new Vector(i, j), new Vector(i + 0.5, j + 0.5)));
                        pixel.Lines.Add(new Segment(new Vector(i + 0.5, j + 0.5), new Vector(i + 1, j)));
                    }

                    // bottom side open
                    if (!pixel.IsSolid &&
                        !Pixels[i, j + 1].IsSolid &&
                        Pixels[i - 1, j].IsSolid &&
                        Pixels[i + 1, j].IsSolid &&
                        Pixels[i, j - 1].IsSolid)
                    {
                        pixel.Lines.Add(new Segment(new Vector(i, j + 1), new Vector(i + 0.5, j + 0.5)));
                        pixel.Lines.Add(new Segment(new Vector(i + 0.5, j + 0.5), new Vector(i + 1, j + 1)));
                    }

                    // left side open
                    if (!pixel.IsSolid &&
                        !Pixels[i - 1, j].IsSolid &&
                        Pixels[i, j - 1].IsSolid &&
                        Pixels[i, j + 1].IsSolid &&
                        Pixels[i + 1, j].IsSolid)
                    {
                        pixel.Lines.Add(new Segment(new Vector(i, j), new Vector(i + 0.5, j + 0.5)));
                        pixel.Lines.Add(new Segment(new Vector(i + 0.5, j + 0.5), new Vector(i, j + 1)));
                    }

                    // right side open
                    if (!pixel.IsSolid &&
                        !Pixels[i + 1, j].IsSolid &&
                        Pixels[i, j - 1].IsSolid &&
                        Pixels[i, j + 1].IsSolid &&
                        Pixels[i - 1, j].IsSolid)
                    {
                        pixel.Lines.Add(new Segment(new Vector(i + 1, j), new Vector(i + 0.5, j + 0.5)));
                        pixel.Lines.Add(new Segment(new Vector(i + 0.5, j + 0.5), new Vector(i + 1, j + 1)));
                    }

                    // hole
                    if (!pixel.IsSolid &&
                        Pixels[i - 1, j].IsSolid &&
                        Pixels[i + 1, j].IsSolid &&
                        Pixels[i, j - 1].IsSolid &&
                        Pixels[i, j + 1].IsSolid)
                    {
                        // maybe insert some little hole in here
                    }

                }
            }
        }

        public Intersection CheckCollision(Segment segmentIn)
        {
            // bounding box
            List<int> bb = segmentIn.BBoxInt;

            // search for intersections
            List<Intersection> intersections = new List<Intersection> { };

            // go through all pixels in bbox
            for (int i = bb[0]; i <= bb[1]; i++)
            {
                for (int j = bb[2]; j <= bb[3]; j++)
                {
                    // go through all lines in pixel
                    foreach (Segment line in Pixels[i, j].Lines)
                    {
                        Intersection intersection = new Intersection(segmentIn, line);

                        if (intersection.IsTrue)
                        {
                            intersections.Add(intersection);
                        }
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

                return intersections[count];
            } 

            else
            {
                Intersection intersection = new Intersection();
                return intersection;
            }

        }

        public void MakeCaldera(Vector position, int size)
        {
            for (int i = (int)position.X - size + 1; i <= (int)position.X + size + 1; i++)
            {
                for (int j = (int)position.Y - size + 1; j <= (int)position.Y + size + 1; j++)
                {
                    if ((position - new Vector(i, j)).Length <= size)
                    {
                        if ((i >= 0) && (i <= 320) && (j >= 0) && (j <= 200))
                        {
                            Pixels[i, j].IsSolid = false;
                        }
                    }
                }
            }

            UpdatePixelLines();

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
                    Pixels[i, j].IsSolid = true;
                }
            }

        }

        public void Draw(IGraphics g, Color c)
        {
            // Draws every line of every pixel

            foreach (Pixel pixel in Pixels)
            {
                foreach (Segment line in pixel.Lines)
                {
                    line.Draw(g, c);
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
