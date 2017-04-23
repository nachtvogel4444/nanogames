// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Globalization;

namespace NanoGames.Games.Banana
{
    /// <summary>
    /// A 2D vector with integer coordinates.
    /// </summary>
    public struct VectorInt
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public VectorInt(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Gets the squared length of the vector.
        /// </summary>
        public double SquaredLength => X * X + Y * Y;

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public double Length => Math.Sqrt(SquaredLength);

        /// <summary>
        /// Gets the vector rotated 90 degrees left.
        /// </summary>
        public VectorInt RotatedLeft => new VectorInt(Y, -X);

        /// <summary>
        /// Gets the vector rotated 90 degrees right.
        /// </summary>
        public VectorInt RotatedRight => new VectorInt(-Y, X);

        public static VectorInt operator +(VectorInt a, VectorInt b)
        {
            return new VectorInt(a.X + b.X, a.Y + b.Y);
        }

        public static VectorInt operator -(VectorInt a, VectorInt b)
        {
            return new VectorInt(a.X - b.X, a.Y - b.Y);
        }

        public static VectorInt operator -(VectorInt v)
        {
            return new VectorInt(-v.X, -v.Y);
        }

        public static VectorInt operator *(int f, VectorInt v)
        {
            return new VectorInt(f * v.X, f * v.Y);
        }

        public static VectorInt operator *(VectorInt v, int f)
        {
            return new VectorInt(f * v.X, f * v.Y);
        }

        public static bool operator ==(VectorInt a, VectorInt b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(VectorInt a, VectorInt b)
        {
            return !(a == b);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is VectorInt && this == (VectorInt)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return X.GetHashCode() * unchecked((int)0xf8f5a063) + Y.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1})", X, Y);
        }
    }
}
