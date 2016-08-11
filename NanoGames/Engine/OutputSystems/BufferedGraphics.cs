// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Synchronization;
using System;
using System.Collections.Generic;

namespace NanoGames.Engine.OutputSystems
{
    /// <summary>
    /// A buffered graphics implementation that can be used to draw the same screen multiple times.
    /// </summary>
    [NonClonable]
    internal sealed class BufferedGraphics : IGraphics
    {
        private readonly List<Tuple<Color, Vector, Vector>> _lines = new List<Tuple<Color, Vector, Vector>>();
        private readonly List<Tuple<Color, Vector>> _points = new List<Tuple<Color, Vector>>();

        /// <summary>
        /// Clears the graphics buffer.
        /// </summary>
        public void Clear()
        {
            _lines.Clear();
            _points.Clear();
        }

        /// <summary>
        /// Renders to contents of the buffer to the specified target renderer.
        /// </summary>
        /// <param name="targetGraphics">The graphics to render to.</param>
        public void Render(IGraphics targetGraphics)
        {
            if (targetGraphics == null)
            {
                return;
            }

            foreach (var line in _lines)
            {
                targetGraphics.Line(line.Item1, line.Item2, line.Item3);
            }

            foreach (var point in _points)
            {
                targetGraphics.Point(point.Item1, point.Item2);
            }
        }

        /// <inheritdoc/>
        public void Line(Color color, Vector start, Vector end)
        {
            _lines.Add(Tuple.Create(color, start, end));
        }

        /// <inheritdoc/>
        public void Point(Color color, Vector vector)
        {
            _points.Add(Tuple.Create(color, vector));
        }
    }
}
