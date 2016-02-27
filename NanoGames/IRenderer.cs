// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// The 2D rendering interface.
    /// </summary>
    internal interface IRenderer
    {
        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="color">The line color.</param>
        /// <param name="vectorA">The line's starting point.</param>
        /// <param name="vectorB">The line's end point.</param>
        void Line(Color color, Vector vectorA, Vector vectorB);
    }
}
