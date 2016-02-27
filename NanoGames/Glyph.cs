// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections;
using System.Collections.Generic;

namespace NanoGames
{
    /// <summary>
    /// Represents a reusable single-colored shape.
    /// </summary>
    internal sealed class Glyph : IEnumerable<Glyph.Stroke>
    {
        private readonly double _width;
        private readonly double _height;

        private readonly List<Stroke> _strokes = new List<Stroke>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Glyph"/> class.
        /// </summary>
        /// <param name="width">The width of the glyph's inner coordinate system.</param>
        /// <param name="height">The height of the glyph's inner coordinate system.</param>
        public Glyph(double width, double height)
        {
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Adds a new stroke to the glyph.
        /// </summary>
        /// <param name="points">The points to draw.</param>
        public void Add(params double[] points)
        {
            for (int i = 0; i + 3 < points.Length; i += 2)
            {
                _strokes.Add(new Stroke(new Vector(points[i] / _width, points[i + 1] / _height), new Vector(points[i + 2] / _width, points[i + 3] / _height)));
            }
        }

        /// <inheritdoc/>
        public IEnumerator<Stroke> GetEnumerator()
        {
            return _strokes.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _strokes.GetEnumerator();
        }

        /// <summary>
        /// Represents a single stroke of a glyph.
        /// </summary>
        public sealed class Stroke
        {
            /// <summary>
            /// The stroke's starting point.
            /// </summary>
            public readonly Vector A;

            /// <summary>
            /// The stroke's end point.
            /// </summary>
            public readonly Vector B;

            /// <summary>
            /// Initializes a new instance of the <see cref="Stroke"/> class.
            /// </summary>
            /// <param name="a">The starting point.</param>
            /// <param name="b">The end point.</param>
            public Stroke(Vector a, Vector b)
            {
                A = a;
                B = b;
            }
        }
    }
}
