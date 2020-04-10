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
        public Color Color;

        public List<Vector> Points;
        public Vector CenterOfGraphity = new Vector(0, 0);

        public Tile(Vector centerPoint, List<Vector> points, Color color)
        {
            Segments = new List<Segment> { new Segment(points[points.Count - 1], points[0]) };
            CenterPoint = centerPoint;
            Points = points;
            
            for (int idx = 0; idx < points.Count -1; idx++)
            {
                Segments.Add(new Segment(points[idx], points[idx + 1]));
            }

            foreach (Vector p in points)
            {
                CenterOfGraphity += p;
            }

            CenterOfGraphity = CenterOfGraphity / points.Count - CenterPoint;
            Color = color;
        }


        public void Draw(ClusterPlayer observer)
        {
            double m = observer.Magnification;
            Vector obs = observer.Position;
            Color c = 0.45 * Math.Pow(m, 0.4) * Color;
            IGraphics g = observer.Output.Graphics;

            foreach (Segment part in Segments)
            {
                Segment seg = part.Translated(-obs).Scaled(m).ToOrigin();
                g.LLine(c, seg.Start, seg.End);
            }
            /*
            foreach (Vector point in Points)
            {
                Vector p = point.Translated(-obs).Scaled(m).ToOrigin();
                g.CCircle(Colors.Red, p, 1);
            }
            
            Vector cp = CenterPoint.Translated(-obs).Scaled(m).ToOrigin();
            g.CCircle(Colors.Orange, cp, 0.3*/
            
        }
        
    }
}

