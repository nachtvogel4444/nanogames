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
        public List<Tile> VoronoiTiles;
        public Random Random;
        public List<Vector> VoronoiPoints;


        public Planet(Vector pos, double r, int n, Random random)
        {
            Position = pos;
            Radius = r;
            Random = random;

            addTiles(n);
        }
        

        public void Draw(ClusterPlayer observer)
        {
            double m = observer.Magnification;
            double r = Radius * m;

            if (r > 0.5)
            {
                Vector obs = observer.Position;
                Color c = Colors.White;
                IGraphics g = observer.Output.Graphics;
                Vector p = Position.Translated(-obs).Scaled(m).ToOrigin();
                
                g.CCircle(c, p, r);
            }
        }

        private void addTiles(int n)
        {
            // find n random points in planet
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
            foreach (Vector voronoiPoint in VoronoiPoints)
            {

            }
        }
    }
}
