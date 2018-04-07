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
        /// Draws a Point, only if it is on screen.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="color">The color.</param>
        /// <param name="position">The position.</param>
        public static void PPoint(this IGraphics graphics, Color color, Vector position)
        {
            if (position.X > 0 && position.X < 320 && position.Y > 0 && position.Y < 200)
            {
                graphics.Point(color, position);
            }
        }

        /// <summary>
        /// Draws a Line, only if it is on screen.
        /// <summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="color">The line color.</param>
        /// <param name="start">The line's starting point.</param>
        /// <param name="end">The line's end point.</param>
        public static void LLine(this IGraphics graphics, Color color, Vector start, Vector end)
        {
            if ((start.X > 0 && start.X < 320 && start.Y > 0 && start.Y < 200) ||
                    (end.X > 0 && end.X < 320 && end.Y > 0 && end.Y < 200))
            {
                graphics.Line(color, start, end);
            }
        }

        /// <summary>
        /// Draws a circle segment only on screen.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="color">The color.</param>
        /// <param name="center">The center point.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="startAngle">The start angle of the segment.</param>
        /// <param name="endAngle">The end angle of the segment.</param>
        public static void CCircleSegment(this IGraphics graphics, Color color, Vector center, double radius, double startAngle, double endAngle)
        {
            if (radius == 0)
            {
                return;
            }

            double angle = endAngle - startAngle;
            double ratio = angle / (2 * Math.PI);

            // old version
            // int steps = Math.Max(8, (int)Math.Ceiling(Math.Abs(radius) * 2 * Math.PI));


            // new version: steps = pi * sqrt(r/(2*h), h is the max distance between Circle and Line
            int steps = Math.Max(8, (int)Math.Ceiling(ratio * Math.Sqrt(Math.Abs(radius) * 100))); // h = 0.05
            steps = Math.Min(steps, (int)Math.Ceiling(ratio * 320)); // 316:1000, 222:500, 160:250, 100:100
            for (int i = 0; i < steps; ++i)
            {
                var angleA = startAngle + i * angle / steps;
                var angleB = startAngle + (i + 1) * angle / steps;

                var vectorA = center + new Vector(radius * Math.Cos(angleA), radius * Math.Sin(angleA));
                var vectorB = center + new Vector(radius * Math.Cos(angleB), radius * Math.Sin(angleB));

                if ((vectorA.X > 0 && vectorA.X < 320 && vectorA.Y > 0 && vectorA.Y < 200) ||
                    (vectorB.X > 0 && vectorB.X < 320 && vectorB.Y > 0 && vectorB.Y < 200))
                {
                    graphics.Line(color, vectorA, vectorB);
                }
            }
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
        /// Draws a circle only on screen.
        /// </summary>
        /// <param name="graphics">The graphics object.</param>
        /// <param name="color">The color.</param>
        /// <param name="center">The center point.</param>
        /// <param name="radius">The radius.</param>
        public static void CCircle(this IGraphics graphics, Color color, Vector center, double radius)
        {
            graphics.CCircleSegment(color, center, radius, 0, 2 * Math.PI);
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
