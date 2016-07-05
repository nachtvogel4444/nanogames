// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// Represents the graphics output.
    /// </summary>
    public interface IGraphics
    {
        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="color">The line color.</param>
        /// <param name="start">The line's starting point.</param>
        /// <param name="end">The line's end point.</param>
        void Line(Color color, Vector start, Vector end);

        /// <summary>
        /// Draws a point.
        /// </summary>
        /// <param name="color">The point color.</param>
        /// <param name="vector">The point coordinates.</param>
        void Point(Color color, Vector vector);
    }
}
