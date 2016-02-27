// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// Represents an input/output connection to a player.
    /// </summary>
    internal sealed class Terminal : IRenderer
    {
        /// <summary>
        /// The virtual screen width.
        /// </summary>
        public const double Width = 640;

        /// <summary>
        /// The virtual screen height.
        /// </summary>
        public const double Height = 360;

        private readonly IRenderer _renderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Terminal"/> class.
        /// </summary>
        /// <param name="renderer">The renderer to draw onto.</param>
        public Terminal(IRenderer renderer)
        {
            _renderer = renderer;
        }

        /// <inheritdoc/>
        public void Line(Color color, Vector vectorA, Vector vectorB)
        {
            _renderer?.Line(color, vectorA, vectorB);
        }
    }
}
