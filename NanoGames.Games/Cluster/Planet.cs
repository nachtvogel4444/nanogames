﻿// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoGames.Games.Cluster
{
    internal class Planet
    {
        public Vector Position;
        public double Radius;
        public List<Tile> VoronoiTiles = new List<Tile> { };
        public Random Random;
        public List<Vector> VoronoiPoints = new List<Vector> { };

        public List<Vector> PointsToPlot1 = new List<Vector> { };
        public List<Vector> PointsToPlot2 = new List<Vector> { };
        public List<Vector> PointsToPlot3 = new List<Vector> { };
        public List<Vector> PointsToPlot4 = new List<Vector> { };


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
                g.CCircle(c, p, 0.2 );
            }
            /*
            for (int idx = 0; idx < PointsToPlot1.Count; idx++)
            {
                Color c = Colors.Red;
                Vector p1 = (Position + PointsToPlot1[idx]).Translated(-obs).Scaled(m).ToOrigin();
                Vector p2 = (Position + PointsToPlot2[idx]).Translated(-obs).Scaled(m).ToOrigin();
                g.LLine(c, p1, p2);
            }
            foreach (Vector pp in PointsToPlot3)
            {
                Color c = Colors.White;
                Vector p = (Position + pp).Translated(-obs).Scaled(m).ToOrigin();
                g.PPoint(c, p);

            }
            foreach (Vector pp in PointsToPlot4)
            {
                Color c = Colors.Green;
                Vector p = (Position + pp).Translated(-obs).Scaled(m).ToOrigin();
                g.CCircle(c, p, 2);

            }
            g.CCircle(Colors.Red, (Position + VoronoiPoints[0]).Translated(-obs).Scaled(m).ToOrigin(), 2);*/


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

            getAllTiles();
            
        }

        private void getAllTiles()
        {
            for (int i = 0; i < VoronoiPoints.Count; i++)
            // for (int i = 0; i < 1; i++)
            {
                addOneTile(i);
            }
        }

        private void addOneTile(int idx_thisCenterPoint)
        {
            var thisCenterPoint = VoronoiPoints[idx_thisCenterPoint];

            // sort all centerPoints by their distance to thisCenterPoint
            // var distancesSquared = VoronoiPoints.Select(x => (x - thisCenterPoint).Length).ToList();
            // var sortedCenterPoints = VoronoiPoints.Zip(distancesSquared, (x, y) => (x: x, y: y)).OrderBy(t => t.y).Select(t => t.x).ToList();

            // find all midLines. Midlines have the form midline = m + k * d. k is a scalar.
            var m = VoronoiPoints.Select(x => 0.5 * (x + thisCenterPoint)).ToList();
            var d = m.Select(x => (x - thisCenterPoint).RotatedLeft).ToList();
            var midlines = m.Zip(d, (a, b) => (m: a, d: b)).ToList();

            PointsToPlot1 = m.Zip(d, (a, b) => a + 50 * b.Normalized).ToList();
            PointsToPlot2 = m.Zip(d, (a, b) => a - 50 * b.Normalized).ToList();

            // find all intersections od midlines.
            var counter = 1;
            var length = VoronoiPoints.Count;
            List<Vector> intersections = new List<Vector> { };
            foreach (var t in midlines)
            {
                for (int idx = counter; idx < length; idx++)
                {
                    Vector m1 = t.m;
                    Vector m2 = midlines[idx].m;
                    Vector d1 = t.d;
                    Vector d2 = midlines[idx].d;

                    double denom = d1.Y * d2.X - d1.X * d2.Y;

                    if (denom == 0.0)
                    {
                        break;
                    }

                    // from wolfram alpha k1 = (d22 m11 - d21 m12 - d22 m21 + d21 m22) / (d12 d21 - d11 d22);
                    double k1 = (d2.Y * m1.X - d2.X * m1.Y - d2.Y * m2.X + d2.X * m2.Y) / denom;
                    Vector intersection = m1 + k1 * d1;

                    if (intersection.Length <= Radius)
                    {
                        intersections.Add(intersection);
                    }
                }

                counter++;
            }

            PointsToPlot3 = intersections;

            // take only valid intersection for tile
            List<Vector> validIntersections = new List<Vector> { };
            foreach (Vector intersection in intersections)
            {
                var distToCP = (intersection - thisCenterPoint).SquaredLength;
                var isvalid = true;

                foreach (Vector point in VoronoiPoints)
                {
                    if (point == thisCenterPoint)
                    {
                        continue;
                    }

                    var distToOCP = (intersection - point).SquaredLength;

                    if (distToCP > (distToOCP + Constants.Planet.epsilon))
                    {
                        isvalid = false;
                        break;
                    }
                }

                if (isvalid)
                {
                    validIntersections.Add(intersection);
                }
            }

            PointsToPlot4 = validIntersections;

            // sort valid intersections
            var angles = validIntersections.Select(x => Math.Atan2(x.Y - thisCenterPoint.Y, x.X - thisCenterPoint.X));
            var sortedValidIntersections = validIntersections.Zip(angles, (x, y) => (x: x, y: y)).OrderBy(t => t.y).Select(t => t.x).ToList();

            if (validIntersections.Count > 2)
            {
                VoronoiTiles.Add(new Tile(Position + thisCenterPoint, sortedValidIntersections.Select(x => Position + x).ToList()));
            }

        }

    }
}
