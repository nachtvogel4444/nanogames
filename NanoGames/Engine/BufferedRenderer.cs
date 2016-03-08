// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Engine
{
    /// <summary>
    /// A renderer implementation that writes to an internal buffer for later rendering.
    /// </summary>
    public sealed class BufferedRenderer : IRenderer
    {
        private readonly List<Tuple<Color, float, float, float, float>> _lines = new List<Tuple<Color, float, float, float, float>>();
        private readonly List<Tuple<Color, float, float>> _points = new List<Tuple<Color, float, float>>();

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Clear()
        {
            _lines.Clear();
            _points.Clear();
        }

        /// <summary>
        /// Renders to contents of the buffer to the specified target renderer.
        /// </summary>
        /// <param name="renderer">The renderer to write to.</param>
        public void RenderTo(IRenderer renderer)
        {
            if (renderer == null)
            {
                return;
            }

            foreach (var line in _lines)
            {
                renderer.Line(line.Item1, line.Item2, line.Item3, line.Item4, line.Item5);
            }

            foreach (var point in _points)
            {
                renderer.Point(point.Item1, point.Item2, point.Item3);
            }
        }

        /// <inheritdoc/>
        public void Line(Color color, float ax, float ay, float bx, float by)
        {
            _lines.Add(Tuple.Create(color, ax, ay, bx, by));
        }

        /// <inheritdoc/>
        public void Point(Color color, float x, float y)
        {
            _points.Add(Tuple.Create(color, x, y));
        }
    }
}
