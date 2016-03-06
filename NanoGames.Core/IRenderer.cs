// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// The 2D rendering interface.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="color">The line color.</param>
        /// <param name="vectorA">The line's starting point.</param>
        /// <param name="vectorB">The line's end point.</param>
        void Line(Color color, Vector vectorA, Vector vectorB);

        /// <summary>
        /// Draws a point.
        /// </summary>
        /// <param name="color">The point color.</param>
        /// <param name="vector">The point coordinates.</param>
        void Point(Color color, Vector vector);
    }
}
