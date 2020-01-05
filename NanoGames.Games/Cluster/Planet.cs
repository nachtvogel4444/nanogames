// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Cluster
{
    internal class Planet
    {
        public Vector Position;
        public double Radius;
        public List<Tile> VoronoiTiles = new List<Tile> { };
        public Random Random;
        public List<Vector> VoronoiPoints = new List<Vector> { };


        public Planet(Vector pos, double r, Random random)
        {
            Position = pos;
            Radius = r;
            Random = random;

            addTiles();
        }
        

        public void Draw(ClusterPlayer observer)
        {
            double m = observer.Magnification;
            double r = Radius * m;

            IGraphics g = observer.Output.Graphics;
            Vector obs = observer.Position;

            if (r > 0.5)
            {
                Color c = Colors.White;
                Vector p = Position.Translated(-obs).Scaled(m).ToOrigin();
                
                g.CCircle(c, p, r);
            }

            foreach (Vector vp in VoronoiPoints)
            {
                Color c = Colors.Blue;
                Vector p = (Position + vp).Translated(-obs).Scaled(m).ToOrigin();
                g.PPoint(c, p);
            }

            foreach (Tile tile in VoronoiTiles)
            {
                tile.Draw(observer);
            }
        }


        private void addTiles()
        {
            // find random voronoi points in planet
            int n = (int)(Radius * Radius * Math.PI * Constants.Planet.VoronoiDensity);

            for (int i = 0; i < n; i++)
            {
                bool foundplace = false;
                Vector p = new Vector(0, 0);

                while (!foundplace)
                {
                    p = new Vector(Functions.NextDoubleBtw(Random, -Radius, Radius), Functions.NextDoubleBtw(Random, -Radius, Radius));

                    if (p.Length < Radius)
                    {
                        foundplace = true;
                    }

                }

                VoronoiPoints.Add(p);
            }

            // do voronoi stuff
            n = VoronoiPoints.Count;

            foreach (Vector vp in VoronoiPoints)
            {
                // continue here with calculation of voronoipoints
                // pixel based, handwaving
                
                double dist = double.MaxValue;
                Vector vp1 = new Vector(0, 0);
                
                foreach (Vector othervp in VoronoiPoints)
                {
                    if (othervp != vp)
                    {
                        if (dist > (vp - othervp).Length)
                        {
                            vp1 = othervp;
                            dist = (vp - othervp).Length;
                        }
                    }
                }

                Vector p = vp.MidPointTo(vp1);
                Vector dir = (p - vp).RotatedLeft.Normalized;

                // step-by-step search fornext tilepoint
                int foundIt = 0;
                int counter = 0;
               
                while (foundIt == 0 && counter < 10000)
                {
                    foreach (Vector othervp in VoronoiPoints)
                    {
                        if ((othervp != vp) && (othervp != vp1))
                        {
                            if (Math.Abs((p - vp).Length  - (p - othervp).Length) < Constants.Planet.VoronoiError)
                            {
                                foundIt++;
                            }
                        }
                    }

                    p = p + Constants.Planet.VoronoiStep * dir;

                    if (p.Length >= Radius)
                    {
                        foundIt = 1;
                    }

                    counter++;
                }

                if (true)
                {
                    Tile tile = new Tile();
                    tile.Segments.Add(new Segment(Position + vp.MidPointTo(vp1), Position + p));
                    VoronoiTiles.Add(tile);
                }
                
            }
        }
    }
}
