// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Tanks2
{
    public class Face
    {
        public List<Segment> Segments;
        public List<Vector3> Points;

        public Color Color;

        private List<double> s1;
        private List<double> s2;
        private List<double> n1;
        private List<double> n2;
        
        List<Vector> intersects = new List<Vector> { };
        List<Vector> s = new List<Vector> { };

        public Face(List<Segment> segs, Color c)
        {
            Segments = segs;
            Color = c;

            Points = new List<Vector3> { };
            Points.Add(Segments[0].Start);
            for (int i = 0; i < Segments.Count; i++)
            {
                Points.Add(Segments[i].Stop);
                Segments[i].Color = Color;
            }
        }

        public Vector3 Normal => Vector3.Cross(A, B).Normalized;

        public Vector3 A => Points[0] - Points[1];
       
        public Vector3 B => Points[0] - Points[2];

        public double MinZ => minZ();

        public double MaxZ => maxZ();

        public bool IsFront(Face other) => other.MaxZ > MinZ;
        
        public void Cut(Face other)
        {
            foreach (Segment thisseg in Segments)
            {
                foreach (Segment otherseg in other.Segments)
                {
                    
                }
            }
        }


        private double minZ()
        {
            double min =double.MaxValue;

            foreach (Vector3 v in Points)
            {
                min = Math.Min(min, v.Z);
            }

            return min;
        }

        private double maxZ()
        {
            double max = double.MinValue;

            foreach (Vector3 v in Points)
            {
                max = Math.Max(max, v.Z);
            }

            return max;
        }

    }
}

