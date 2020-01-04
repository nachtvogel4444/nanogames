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
            foreach (Vector voronoiPoint in VoronoiPoints)
            {
                // continue here with calculation of voronoipoints
                // pixel based, handwaving

            }
        }
    }
}
