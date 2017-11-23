// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Tanks2
{
    public class Geometrie
    {
        public static void Intersection(Triangle triangle, Segment segment)
        {
            Vector3 u = segment.ProjectionStop - segment.ProjectionStart;
            Vector3 w = segment.ProjectionStop - triangle.A;
            Vector3 np = Cross(triangle.A - triangle.B, triangle.A - triangle.C).Normalized;

            double d = Dot(np, u);
            double n = -Dot(np, w);

            if (Math.Abs(d) < 1e-9)
            {
                // line is parallel to triangle
                if (Math.Abs(n) < 1e-9)
                {
                    // segement is in triangle
                }
                else
                {
                    // segement has no intersection
                }
            }

            double s = n / d;

            if (s >= 0 && s <= 1)
            {
                // 
            }
        }

        /// <summary>
        /// Computes the linear combination of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="ratio">The mix ratio.</param>
        /// <returns>The linear combination of two vectors.</returns>
        public static Vector3 Mix(Vector3 a, Vector3 b, double ratio)
        {
            return a * (1 - ratio) + b * ratio;
        }

        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Returns the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(a.Y * b.Z - a.Z * b.Y,
                               a.Z * b.X - a.X * b.Z,
                               a.X * b.Y - a.Y * b.X);
        }
    }
}
