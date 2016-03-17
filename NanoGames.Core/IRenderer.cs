// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// The 2D rendering interface.
    /// The coordinate system has its origin at the bottom left, like OpenGL, and unlike the Graphics interface exposed to the application.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="color">The line color.</param>
        /// <param name="ax">The starting point x coordinate.</param>
        /// <param name="ay">The starting point y coordinate.</param>
        /// <param name="bx">The end point x coordinate.</param>
        /// <param name="by">The end point y coordinate.</param>
        void Line(Color color, float ax, float ay, float bx, float by);

        /// <summary>
        /// Draws a point.
        /// </summary>
        /// <param name="color">The point color.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        void Point(Color color, float x, float y);
    }
}
