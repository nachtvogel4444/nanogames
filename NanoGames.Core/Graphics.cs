// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames
{
    /// <summary>
    /// Represents a player's graphics interface.
    /// The screen always has a virtual width of 320 and a height of 200 pixels.
    /// (0, 0) is at the top-left corner.
    /// </summary>
    public sealed class Graphics
    {
        /// <summary>
        /// The virtual screen width.
        /// </summary>
        public const double Width = 320;

        /// <summary>
        /// The virtual screen height.
        /// </summary>
        public const double Height = 200;

        /// <summary>
        /// The virtual screen center point.
        /// </summary>
        public static readonly Vector Center = new Vector(Width * 0.5, Height * 0.5);

        /// <summary>
        /// A terminal with a null renderer.
        /// </summary>
        public static readonly Graphics Null = new Graphics(null);

        private readonly IRenderer _renderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphics"/> class.
        /// </summary>
        /// <param name="renderer">The renderer to draw onto.</param>
        public Graphics(IRenderer renderer)
        {
            _renderer = renderer;
        }

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="color">The line color.</param>
        /// <param name="vectorA">The line's starting point.</param>
        /// <param name="vectorB">The line's end point.</param>
        public void Line(Color color, Vector vectorA, Vector vectorB)
        {
            _renderer?.Line(color, (float)vectorA.X, (float)Height - (float)vectorA.Y, (float)vectorB.X, (float)Height - (float)vectorB.Y);
        }

        /// <summary>
        /// Draws a point.
        /// </summary>
        /// <param name="color">The point color.</param>
        /// <param name="vector">The point coordinates.</param>
        public void Point(Color color, Vector vector)
        {
            _renderer?.Point(color, (float)vector.X, (float)Height - (float)vector.Y);
        }

        /// <summary>
        /// Draws a glyph.
        /// </summary>
        /// <param name="color">The color of the glyph.</param>
        /// <param name="glyph">The glyph to draw.</param>
        /// <param name="topLeft">The top-left corner of the glyph.</param>
        /// <param name="x">The x vector of the glyph to draw.</param>
        /// <param name="y">The y vector of the glyph to draw.</param>
        public void Glyph(Color color, Glyph glyph, Vector topLeft, Vector x, Vector y)
        {
            if (glyph == null)
            {
                return;
            }

            foreach (var stroke in glyph.Strokes)
            {
                Line(color, topLeft + stroke.A.X * x + stroke.A.Y * y, topLeft + stroke.B.X * x + stroke.B.Y * y);
            }

            foreach (var point in glyph.Points)
            {
                Point(color, topLeft + point.X * x + point.Y * y);
            }
        }

        /// <summary>
        /// Writes text.
        /// </summary>
        /// <param name="color">The text color.</param>
        /// <param name="size">The font size. Glyphs are square shaped, so this is both the width and height of each character.</param>
        /// <param name="topLeft">The position of the top-left corner of the text.</param>
        /// <param name="text">The text to write.</param>
        public void Print(Color color, double size, Vector topLeft, string text)
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
                Glyph(color, Font.GetGlyph(c), topLeft, x, y);
                topLeft.X += size;
            }
        }

        /// <summary>
        /// Writes text centered at the given position.
        /// </summary>
        /// <param name="color">The text color.</param>
        /// <param name="size">The font size. Glyphs are square shaped, so this is both the width and height of each character.</param>
        /// <param name="topCenter">The position of the top-center of the text.</param>
        /// <param name="text">The text to write.</param>
        public void PrintCenter(Color color, double size, Vector topCenter, string text)
        {
            if (text == null)
            {
                return;
            }

            Print(color, size, topCenter - new Vector(text.Length * size * 0.5, 0), text);
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="center">The center point.</param>
        /// <param name="radius">The radius.</param>
        public void Circle(Color color, Vector center, double radius)
        {
            if (radius == 0)
            {
                return;
            }

            int steps = Math.Max(8, (int)Math.Ceiling(Math.Abs(radius) * 2 * Math.PI));
            for (int i = 0; i < steps; ++i)
            {
                var angleA = i * 2 * Math.PI / steps;
                var angleB = (i + 1) * 2 * Math.PI / steps;

                var vectorA = center + new Vector(radius * Math.Cos(angleA), radius * Math.Sin(angleA));
                var vectorB = center + new Vector(radius * Math.Cos(angleB), radius * Math.Sin(angleB));

                Line(color, vectorA, vectorB);
            }
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="topLeft">The top-left point.</param>
        /// <param name="bottomRight">The bottom-right point.</param>
        public void Rectangle(Color color, Vector topLeft, Vector bottomRight)
        {
            var topRight = new Vector(bottomRight.X, topLeft.Y);
            var bottomLeft = new Vector(topLeft.X, bottomRight.Y);

            Line(color, topLeft, topRight);
            Line(color, topRight, bottomRight);
            Line(color, bottomRight, bottomLeft);
            Line(color, bottomLeft, topLeft);
        }
    }
}
