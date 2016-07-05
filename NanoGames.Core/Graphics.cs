// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// Represents a player's graphics interface.
    /// The screen always has a virtual width of 320 and a height of 200 pixels.
    /// (0, 0) is at the top-left corner.
    /// </summary>
    public sealed class Graphics : IGraphics
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

        /// <inheritdoc/>
        public void Line(Color color, Vector vectorA, Vector vectorB)
        {
            _renderer?.Line(color, (float)vectorA.X, (float)Height - (float)vectorA.Y, (float)vectorB.X, (float)Height - (float)vectorB.Y);
        }

        /// <inheritdoc/>
        public void Point(Color color, Vector vector)
        {
            _renderer?.Point(color, (float)vector.X, (float)Height - (float)vector.Y);
        }
    }
}
