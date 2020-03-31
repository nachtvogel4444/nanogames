// Copyright (c) the authors of nanoGames. All rights reserved.
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
        public List<Tile> Tiles = new List<Tile> { };
        public Random Random;
        public List<Vector> CenterPoints = new List<Vector> { };

        public int BuildIdxTiles;

        private List<Color> tileColors = new List<Color> { };
               

        public Planet(Vector pos, double r, Random random)
        {
            Position = pos;
            Radius = r;
            Random = random;
            
            getCenterPoints();
            getColors();

            BuildIdxTiles = 0;
        }
        

        public void Draw(ClusterPlayer observer)
        {
            double m = observer.Magnification;
            double r = Radius * m;

            IGraphics g = observer.Output.Graphics;
            Vector obs = observer.Position;
            
            foreach (Tile tile in Tiles)
            {
                tile.Draw(observer);
            }          
        }


        public bool Build()
        {
            Vector centerpoint = CenterPoints[BuildIdxTiles];
            buildOneTile(centerpoint);
            BuildIdxTiles++;

            if (BuildIdxTiles == CenterPoints.Count)
            {
                // all tiles of this planet have been build
                return true;
            }

            return false;
        }

        private void getCenterPoints()
        {
            int n = (int)(Radius * Radius * Math.PI * Constants.Planet.VoronoiDensity);

            for (int i = 0; i < n; i++)
            {
                double rad = Functions.NextDoubleDist(Random, 0.3) * Radius;
                double angle = Random.NextDouble() * 2 * Math.PI;

                Vector p = new Vector(Math.Cos(angle), Math.Sin(angle)) * rad;

                CenterPoints.Add(p);
            }


            CenterPoints = CenterPoints.OrderBy(x => x.SquaredLength).ToList();
        }
       
        private void getColors()
        {
            Color c1 = new Color(1.0, 1.0, Functions.NextDoubleBtw(Random, 0.0, 0.4));
            Color c2 = new Color(1.0, 0, 0.0);

            tileColors = Functions.ColorGradient(c1, c2, CenterPoints.Count); 
        }

        private void buildOneTile(Vector thisCenterPoint)
        {
            // reduce number of points to the nearest 
            var squaredDistances = CenterPoints.Select(x => (thisCenterPoint - x).Length);
            var nearestCenterPoints = CenterPoints.Zip(squaredDistances, (x, y) => (x: x, y: y)).OrderBy(t => t.y).Select(t => t.x).Select(v => v).ToList();
            nearestCenterPoints = nearestCenterPoints.Take(50).ToList();

            // find all midLines. Midlines have the form midline = m + k * d. k is a scalar.
            // here thisCenterPoint is still in the list. Not needed
            var m = nearestCenterPoints.Select(x => 0.5 * (x + thisCenterPoint)).ToList();
            var d = m.Select(x => (x - thisCenterPoint).RotatedLeft).ToList();
            var midlines = m.Zip(d, (a, b) => (m: a, d: b)).ToList();
            
            // find all intersections of all midlines.
            int idx1 = 0;
            int length = nearestCenterPoints.Count;
            List<Vector> intersections = new List<Vector> { };
            foreach (var t in midlines)
            {
                for (int idx2 = idx1+1; idx2 < length; idx2++)
                {
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
            }
            
            // get intersections with edge of planet
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
            
            // take only valid intersection for tile. this is the main problem. n**3 actions ....
            List<Vector> validIntersections = new List<Vector> { };
            foreach (Vector intersection in intersections)
            {
                var distToCP = (intersection - thisCenterPoint).SquaredLength;
                var isvalid = true;

                foreach (Vector point in nearestCenterPoints)
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

            // add full tile to list
            if (validIntersections.Count > 2)
            {
                Tiles.Add(new Tile(Position + thisCenterPoint, 
                    sortedValidIntersections.Select(x => Position + x).ToList(), 
                    tileColors[BuildIdxTiles]));
            }
            
        }

    }
}
