// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// A color.
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// The red value ranging linearly from 0 to 1.
        /// </summary>
        public double R;

        /// <summary>
        /// The green value ranging linearly from 0 to 1.
        /// </summary>
        public double G;

        /// <summary>
        /// The blue value ranging linearly from 0 to 1.
        /// </summary>
        public double B;

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="r">The red value ranging linearly from 0 to 1.</param>
        /// <param name="g">The green value ranging linearly from 0 to 1.</param>
        /// <param name="b">The blue value ranging linearly from 0 to 1.</param>
        public Color(double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }
    }
}
