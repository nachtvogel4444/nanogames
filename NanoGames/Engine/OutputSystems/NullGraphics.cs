// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Engine.OutputSystems
{
    /// <summary>
    /// A null graphics implementation that renders nowhere.
    /// </summary>
    internal sealed class NullGraphics : IGraphics
    {
        /// <summary>
        /// The null graphics instance.
        /// </summary>
        public static readonly IGraphics Instance = new NullGraphics();

        /// <inheritdoc/>
        public void Line(Color color, Vector start, Vector end)
        {
        }

        /// <inheritdoc/>
        public void Point(Color color, Vector vector)
        {
        }
    }
}
