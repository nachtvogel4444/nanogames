// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// Helper methods to draw various graphics primitives.
    /// </summary>
    public static class GraphicsExtensions
    {
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="color">The color.</param>
        /// <param name="topLeft">The top-left point.</param>
        /// <param name="bottomRight">The bottom-right point.</param>
        public static void Rectangle(this IGraphics graphics, Color color, Vector topLeft, Vector bottomRight)
        {
            var topRight = new Vector(bottomRight.X, topLeft.Y);
            var bottomLeft = new Vector(topLeft.X, bottomRight.Y);

            graphics.Line(color, topLeft, topRight);
            graphics.Line(color, topRight, bottomRight);
            graphics.Line(color, bottomRight, bottomLeft);
            graphics.Line(color, bottomLeft, topLeft);
        }
    }
}
