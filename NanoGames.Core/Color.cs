// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using ProtoBuf;

namespace NanoGames
{
    /// <summary>
    /// A color.
    /// </summary>
    [ProtoContract]
    public struct Color
    {
        /// <summary>
        /// The red value ranging linearly from 0 to 1.
        /// </summary>
        [ProtoMember(1)]
        public double R;

        /// <summary>
        /// The green value ranging linearly from 0 to 1.
        /// </summary>
        [ProtoMember(2)]
        public double G;

        /// <summary>
        /// The blue value ranging linearly from 0 to 1.
        /// </summary>
        [ProtoMember(3)]
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

        public static Color operator *(double f, Color c)
        {
            return new Color(f * c.R, f * c.G, f * c.B);
        }

        public static Color operator *(Color c, double f)
        {
            return f * c;
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B;
        }

        public static bool operator !=(Color a, Color b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Color && (Color)obj == this;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return R.GetHashCode() + 17 * G.GetHashCode() + 251 * B.GetHashCode();
        }
    }
}
