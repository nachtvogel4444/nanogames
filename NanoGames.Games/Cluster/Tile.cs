﻿// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Cluster
{
    internal class Tile
    {
        public List<Segment> Segments;
        public Vector CenterPoint;

        public List<Vector> Points; 
        public List<Vector> AddPoints;

        public Tile(Vector centerPoint, List<Vector> points, List<Vector> addPoints)
        {
            Segments = new List<Segment> { new Segment(points[points.Count - 1], points[0]) };
            CenterPoint = centerPoint;
            Points = points;
            AddPoints = addPoints;
            
            for (int idx = 0; idx < points.Count -1; idx++)
            {
                Segments.Add(new Segment(points[idx], points[idx + 1]));
            }
        }


        public void Draw(ClusterPlayer observer)
        {
            double m = observer.Magnification;
            Vector obs = observer.Position;
            IGraphics g = observer.Output.Graphics;

            foreach (Segment part in Segments)
            {
                Segment seg = part.Translated(-obs).Scaled(m).ToOrigin();
                g.LLine(Colors.White, seg.Start, seg.End);
            }

            /* debug
            foreach (Vector point in Points)
            {
                Vector p = point.Translated(-obs).Scaled(m).ToOrigin();
                g.CCircle(Colors.Red, p, 1);
            }

            foreach (Vector point in AddPoints)
            {
                Vector p = point.Translated(-obs).Scaled(m).ToOrigin();
                g.PPoint(Colors.White, p);
            }

            Vector cp = CenterPoint.Translated(-obs).Scaled(m).ToOrigin();
            g.CCircle(Colors.Orange, cp, 1);
            */
        }
        
    }
}

