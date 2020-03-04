﻿// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Cluster
{
    internal class VoronoiGraph
    {
        public int LengthPoints;
        public List<Vector> Points;
        public List<Tile> Tiles;
        public double RadBoundary;

        public VoronoiGraph(List<Vector> points, double radboundary)
        {
            LengthPoints = points.Count;
            Points = points;
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

        private Tile getOneTile(int idx_thisPoint)
        {
            int idx_nearestNeighbor = getIndexNearestNeighbor(idx_thisPoint);

            Vector m = 0.5 * (Points[idx_nearestNeighbor] - Points[idx_thisPoint]);
            Vector d = (m - Points[idx_thisPoint]).RotatedLeft;
            Vector g = Points[idx_thisPoint] - m;

            //while 

            Vector h = Points[idx_nearestNeighbor] - m;

            double k = 0.5 * (g.SquaredLength - h.SquaredLength) / (d.X * (g.X - h.X) + d.Y * (g.Y - g.Y));
            Vector p = m + k * d;





        }

        private int getIndexNearestNeighbor(int idx_thisPoint)
        {
            double sqaredDist = double.MaxValue;
            int idx_minDist = -1;

            for (int i = 0; i < LengthPoints; i++)
            {
                if (i != idx_thisPoint)
                {
                    double newSqaredDist = (Points[i] - Points[idx_thisPoint]).SquaredLength;

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
