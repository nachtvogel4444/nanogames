// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Tanks2
{
    public class IntersectionSegmentTriangle
    {
        public Vector3 Position;
        public bool Exists;
        public double S;
        public bool FrontToBack;


        public IntersectionSegmentTriangle(Segment segment, Triangle triangle)
        {
            Vector3 u = segment.ProjectionStop - segment.ProjectionStart;
            Vector3 w = segment.ProjectionStop - triangle.ProjectionA;
            Vector3 np = Vector3.Cross(triangle.ProjectionA - triangle.ProjectionB, triangle.ProjectionA - triangle.ProjectionC);

            double d = Vector3.Dot(np, u);
            double n = Vector3.Dot(np, w);

            if (Math.Abs(d) < 1e-9)
            {
                // line is parallel to triangle
                if (Math.Abs(n) < 1e-9)
                {
                    // segement is in triangle
                    Exists = false;
                }
                else
                {
                    // segement has no intersection
                    Exists = false;
                }
            }
            else
            {
                S = n / d;

                if (S >= 0 && S <= 1)
                {
                    // segment has intersection with triangle
                    Position = segment.ProjectionStart + S * u;

                    if (Position.Z < segment.ProjectionStart.Z)
                    {
                        FrontToBack = false;
                    }
                    else
                    {
                        FrontToBack = true;
                    }
                    Exists = true;
                }
                else
                {
                    Exists = false;
                }

            }
            
        }
    }
}
