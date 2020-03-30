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
            
            foreach (Tile tile in VoronoiTiles)
            {
                tile.Draw(observer);
            }          
        }


        private void getVoronoipoints()
        {
            int n = (int)(Radius * Radius * Math.PI * Constants.Planet.VoronoiDensity);

            for (int i = 0; i < n; i++)
            {
                double rad = Functions.NextDoubleDist(Random, 0.3) * Radius;
                double angle = Random.NextDouble() * 2 * Math.PI;

                Vector p = new Vector(Math.Cos(angle), Math.Sin(angle)) * rad;

                VoronoiPoints.Add(p);
            }


            VoronoiPoints = VoronoiPoints.OrderBy(x => x.SquaredLength).ToList();
        }
        
        private void addTiles()
        {
            getVoronoipoints();
            getAllTiles();
            addColors();
        }

        private void addColors()
        {
            int n = VoronoiTiles.Count;
            
            var c1 = new Color(1.0, 1.0, Functions.NextDoubleBtw(Random, 0.0, 0.4));
            var c2 = new Color(1.0, 0, 0.0);

            var colors = Functions.ColorGradient(c1, c2, n); 

            int idx = 0;
            foreach (Tile tile in VoronoiTiles)
            {
                tile.Color = colors[idx];
                idx++;
            }
        }

        private void getAllTiles()
        {
            for (int i = 0; i < VoronoiPoints.Count; i++)
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
            // here this center point ius stil in the list. not needed
            var m = VoronoiPoints.Select(x => 0.5 * (x + thisCenterPoint)).ToList();
            var d = m.Select(x => (x - thisCenterPoint).RotatedLeft).ToList();
            var midlines = m.Zip(d, (a, b) => (m: a, d: b)).ToList();
            
            // find all intersections od midlines.
            var counter = 1;
            var idx1 = 0;
            var c2 = 0;
            var length = VoronoiPoints.Count;
            List<Vector> intersections = new List<Vector> { };
            foreach (var t in midlines)
            {
                for (int idx2 = counter; idx2 < length; idx2++)
                {
                    c2++;

                    Vector m1 = t.m;
                    Vector m2 = midlines[idx2].m;
                    Vector d1 = t.d;
                    Vector d2 = midlines[idx2].d;

                    double denom = d1.Y * d2.X - d1.X * d2.Y;

                    if (denom == 0.0)
                    {
                        continue;
                    }

                    // from wolfram alpha k1 = (d22 m11 - d21 m12 - d22 m21 + d21 m22) / (d12 d21 - d11 d22);
                    double k1 = (d2.Y * m1.X - d2.X * m1.Y - d2.Y * m2.X + d2.X * m2.Y) / denom;
                    Vector intersection = m1 + k1 * d1;

                    if (intersection.Length <= Radius)
                    {
                        intersections.Add(intersection);
                    }
                }
                idx1++;
                counter++;
            }

            // get intersecitons with radius of planet
            foreach (var midline in midlines)
            {
                double dx = midline.d.X;
                double dy = midline.d.Y;

                double dxsq = dx * dx;
                double dysq = dy * dy;

                if (dxsq + dysq == 0)
                {
                    continue;
                }

                double mx = midline.m.X;
                double my = midline.m.Y;

                double rsq = Radius * Radius;

                // from wolfram alpha: (-d1 m1 - d2 m2 + sqrt(2 d1 d2 m1 m2 + d2^2 (-m1^2 + r^2) + d1^2 (-m2^2 + r^2)))/(d1^2 + d2^2)
                double k1 = (-dx * mx - dy * my + Math.Sqrt(-(dy * mx - dx * my) * (dy * mx - dx * my) + (dxsq + dysq) * rsq)) / (dxsq + dysq);
                double k2 = (-dx * mx - dy * my - Math.Sqrt(-(dy * mx - dx * my) * (dy * mx - dx * my) + (dxsq + dysq) * rsq)) / (dxsq + dysq);

                intersections.Add(midline.m + k1 * midline.d);
                intersections.Add(midline.m + k2 * midline.d);
                
            }
            
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

            // sort valid intersections
            var angles = validIntersections.Select(x => Math.Atan2(x.Y - thisCenterPoint.Y, x.X - thisCenterPoint.X));
            var sortedValidIntersections = validIntersections.Zip(angles, (x, y) => (x: x, y: y)).OrderBy(t => t.y).Select(t => t.x).ToList();

            if (validIntersections.Count > 2)
            {
                VoronoiTiles.Add(new Tile(Position + thisCenterPoint, 
                    sortedValidIntersections.Select(x => Position + x + new Vector(0*Random.NextDouble(), 0)).ToList(), 
                    intersections.Select(x => Position + x).ToList()));
            }

        }

    }
}
