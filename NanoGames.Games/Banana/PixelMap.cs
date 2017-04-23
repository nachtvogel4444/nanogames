// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;

namespace NanoGames.Games.Banana
{
    internal class PixelMap
    {
        private Pixel[,] Pixels;
        private int xMin = 10;
        private int xMax = 310;
        private int yMin = 40;
        private int yMax = 190;

        public PixelMap()
        {
            Pixels = new Pixel[320, 200];
        }
    
        public void InitializePixels()
        {
            // fill Pixels with true
            // keep sicherheitsrand from border
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
            // bounding box, ... etprehcnede pixel testen, intersections in liste speichern. näheste zu segment.End ausgeben
            Intersection intersection = new Intersection(segmentIn, segmentPixel);
        }

        public void MakeCaldera()
        {

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

    }
}
