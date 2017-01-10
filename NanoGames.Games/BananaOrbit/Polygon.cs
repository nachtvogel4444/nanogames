// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.BananaOrbit
{
    /// <summary>
    /// A closed convex polygon, orientated clockwise.
    /// </summary>
    internal class Polygon
    {
        /// <summary>
        /// The list of all points.
        /// </summary>
        public List<Vector> Points;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">A list of points.</param>
        public Polygon(List<Vector> points)
        {
            Points = points;
            // check for equal points
        }

        /// <summary>
        /// Gets the first point in the polygon.
        /// </summary>
        public Vector FirstPoint => Points[0];

        /// <summary>
        /// Gets the last point in the polygon.
        /// </summary>
        public Vector LastPoint => Points[Points.Count - 1];

        /// <summary>
        /// Gets the edges of the polygon.
        /// </summary>
        public List<Segment> Edges => GetEdges();

        /// <summary>
        /// Gets the list of unit normals to the edges of the polygon, pointing outward.
        /// </summary>
        public List<Vector> Normals => GetNormals();

        /// <summary>
        /// Returns the list of the edges of the polygon.
        /// </summary>
        /// <returns>The edges.</returns>
        public List<Segment> GetEdges()
        {
            int k;
            List<Segment> edges = new List<Segment> { };

            for (int i = 0; i < Points.Count - 1; i++)
            {
                k = (i + 1) % Points.Count;
                edges.Add(new Segment(Points[k], Points[i]));
            }

            return edges;
        }

        /// <summary>
        /// Returns the list of the normals to the edges of the polygon.
        /// </summary>
        /// <returns>The edges.</returns>
        public List<Vector> GetNormals()
        {
            List<Vector> normals = new List<Vector> { };

            for (int i = 0; i < Edges.Count; i++)
            {
                normals.Add(Edges[i].Normal);
            }

            return normals;
        }

        /// <summary>
        /// Returns the area inside the polygon.
        /// </summary>
        /// <returns>The area.</returns>
        public double Area()
        {
            int k;
            double area = 0;

            for (int i = 0; i < Points.Count - 1; i++)
            {
                k = (i + 1) % Points.Count;
                area += (Points[k].X - Points[i].X) * (Points[k].Y - Points[i].Y) / 2;
            }

            return Math.Abs(area);
        }

    }
}
