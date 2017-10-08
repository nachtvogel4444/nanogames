// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

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

        /// <summary>
        /// Draws a glyph.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="color">The color of the glyph.</param>
        /// <param name="glyph">The glyph to draw.</param>
        /// <param name="topLeft">The top-left corner of the glyph.</param>
        /// <param name="x">The x vector of the glyph to draw.</param>
        /// <param name="y">The y vector of the glyph to draw.</param>
        public static void Glyph(this IGraphics graphics, Color color, Glyph glyph, Vector topLeft, Vector x, Vector y)
        {
            if (glyph == null)
            {
                return;
            }

            foreach (var stroke in glyph.Strokes)
            {
                graphics.Line(color, topLeft + stroke.A.X * x + stroke.A.Y * y, topLeft + stroke.B.X * x + stroke.B.Y * y);
            }

            foreach (var point in glyph.Points)
            {
                graphics.Point(color, topLeft + point.X * x + point.Y * y);
            }
        }

        /// <summary>
        /// Writes text.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="color">The text color.</param>
        /// <param name="size">The font size. Glyphs are square shaped, so this is both the width and height of each character.</param>
        /// <param name="topLeft">The position of the top-left corner of the text.</param>
        /// <param name="text">The text to write.</param>
        public static void Print(this IGraphics graphics, Color color, double size, Vector topLeft, string text)
        {
            if (text == null)
            {
                return;
            }

            var x = new Vector(size, 0);
            var y = new Vector(0, size);

            foreach (var c in text)
            {
                var glyph = Font.GetGlyph(c);
                graphics.Glyph(color, Font.GetGlyph(c), topLeft, x, y);
                topLeft.X += size;
            }
        }

        /// <summary>
        /// Writes text centered at the given position.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="color">The text color.</param>
        /// <param name="size">The font size. Glyphs are square shaped, so this is both the width and height of each character.</param>
        /// <param name="topCenter">The position of the top-center of the text.</param>
        /// <param name="text">The text to write.</param>
        public static void PrintCenter(this IGraphics graphics, Color color, double size, Vector topCenter, string text)
        {
            if (text == null)
            {
                return;
            }

            graphics.Print(color, size, topCenter - new Vector(text.Length * size * 0.5, 0), text);
        }

        /// <summary>
        /// Draws a circle segment.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="color">The color.</param>
        /// <param name="center">The center point.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="startAngle">The start angle of the segment.</param>
        /// <param name="endAngle">The end angle of the segment.</param>
        public static void CircleSegment(this IGraphics graphics, Color color, Vector center, double radius, double startAngle, double endAngle)
        {
            if (radius == 0)
            {
                return;
            }

            double angle = endAngle - startAngle;

            int steps = Math.Max(8, (int)Math.Ceiling(Math.Abs(radius) * 2 * Math.PI));
            steps = Math.Min(steps, 50);
            for (int i = 0; i < steps; ++i)
            {
                var angleA = startAngle + i * angle / steps;
                var angleB = startAngle + (i + 1) * angle / steps;

                var vectorA = center + new Vector(radius * Math.Cos(angleA), radius * Math.Sin(angleA));
                var vectorB = center + new Vector(radius * Math.Cos(angleB), radius * Math.Sin(angleB));

                graphics.Line(color, vectorA, vectorB);
            }
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="color">The color.</param>
        /// <param name="center">The center point.</param>
        /// <param name="radius">The radius.</param>
        public static void Circle(this IGraphics graphics, Color color, Vector center, double radius)
        {
            graphics.CircleSegment(color, center, radius, 0, 2 * Math.PI);
        }
    }
}
