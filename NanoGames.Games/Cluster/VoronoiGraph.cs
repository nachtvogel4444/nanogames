// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq
using System.

namespace NanoGames.Games.Cluster
{
    internal class VoronoiGraph
    {
        public int LengthPoints;
        //public List<Vector> CenterPoints;
        public Vector[] CenterPoints = { };
        public List<Tile> Tiles;
        public double RadBoundary;

        public VoronoiGraph(List<Vector> points, double radboundary)
        {
            LengthPoints = points.Count;
            CenterPoints = points;
            RadBoundary = radboundary;
            Tiles = new List<Tile> { };
        }

        public List<Tile> GetAllTiles()
        {
            for (int i = 0; i < LengthPoints; i++)
            {
                Tiles.Add(getOneTile(i));
            }

            return Tiles;
        }

        private Tile getOneTile(int idx_thisCenterPoint)
        {
            /*int idx_nearestNeighbor = getIndexNearestNeighbor(idx_thisPoint);

            Vector m = 0.5 * (Points[idx_nearestNeighbor] - Points[idx_thisPoint]);
            Vector d = (m - Points[idx_thisPoint]).RotatedLeft;
            Vector g = Points[idx_thisPoint] - m;

            //while 
            while ()


            Vector h = Points[idx_nearestNeighbor] - m;

            double k = 0.5 * (g.SquaredLength - h.SquaredLength) / (d.X * (g.X - h.X) + d.Y * (g.Y - g.Y));
            Vector p = m + k * d;

            Tile newTile = new Tile();
            newTile.CenterPoint = Points[idx_thisPoint];
            newTile.Segments.Add(new Segment())*/

            // procedere:
            // a) 
            // b) find all midlines. Midlines have the form m = p + k * d. k is a scalar.
            
            // d) go through list of intersectoins and find the ones. distance to this_point <= distance all other points.


            var thisCenterPoint = CenterPoints[idx_thisCenterPoint];

            // sort all centerPoints by their distance to thisCenterPoint
            var distancesSquared = CenterPoints.Select(x => (x - thisCenterPoint).Length).ToList();
            var sortedCenterPoints = CenterPoints.Zip(distancesSquared, (x, y) => (x: x, y: y)).OrderBy(t => t.y).Select(t => t.x).ToList();

            // find all midLines. Midlines have the form midline = m + k * d. k is a scalar.
            var m = sortedCenterPoints.Select(x => 0.5 * (x - thisCenterPoint)).ToList();
            var d = m.Select(x => (x - thisCenterPoint).RotatedLeft).ToList();
            var midlines = m.Zip(d, (a, b) => (m: a, d: b)).ToList();

            // find all intersections od midlines. In this list are all the points of the final voronoi graph
            var counter = 1;
            var length = CenterPoints.Length;
            List<Vector> intersections = new List<Vector { };
            foreach (var t in midlines)
            {
                for (int idx = counter; idx < length; idx++)
                {
                    Vector m1 = t.m;
                    Vector m2 = midlines[idx].m;
                    Vector d1 = t.d;
                    Vector d2 = midlines[idx].d;

                    double denom = d1.Y * d2.X - d1.X * d2.Y;
                    // check if denom is zero
                    double k1 = (d2.Y * m1.X - d2.X * m1.Y - d2.Y * m2.X + d2.X * m2.Y) / denom;
                    // from wolfram alpha k1 = (d22 m11 - d21 m12 - d22 m21 + d21 m22) / (d12 d21 - d11 d22);
                    Vector intersection = m1 + k1 * d1;
                    intersections.Add(intersection);
                }

                counter++;
            }







        }



        private int getIndexNearestNeighbor(int idx_thisPoint)
        {
            double sqaredDist = double.MaxValue;
            int idx_minDist = -1;

            for (int i = 0; i < LengthPoints; i++)
            {
                if (i != idx_thisPoint)
                {
                    double newSqaredDist = (CenterPoints[i] - CenterPoints[idx_thisPoint]).SquaredLength;

                    if (newSqaredDist < sqaredDist)
                    {
                        idx_minDist = i;
                    }
                }
            }

            return idx_minDist;
        }


    }
}
