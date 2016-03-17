// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames
{
    /// <summary>
    /// A 2D vector.
    /// </summary>
    public struct Vector
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public double X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public double Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public Vector(double x, double y)
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
        /// Gets the vector normalized to length 1.
        /// </summary>
        public Vector Normalized => this / Length;

        /// <summary>
        /// Gets the vector rotated 90 degrees left.
        /// </summary>
        public Vector RotatedLeft => new Vector(Y, -X);

        /// <summary>
        /// Gets the vector rotated 90 degrees right.
        /// </summary>
        public Vector RotatedRight => new Vector(-Y, X);

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator -(Vector v)
        {
            return new Vector(-v.X, -v.Y);
        }

        public static Vector operator *(double f, Vector v)
        {
            return new Vector(f * v.X, f * v.Y);
        }

        public static Vector operator *(Vector v, double f)
        {
            return new Vector(f * v.X, f * v.Y);
        }

        public static Vector operator /(Vector v, double f)
        {
            return (1 / f) * v;
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(Vector a, Vector b)
        {
            return a.X * b.X + a.Y + b.Y;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Vector && this == (Vector)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return X.GetHashCode() * unchecked((int)0xf8f5a063) + Y.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return FormattableString.Invariant($"({X}, {Y})");
        }
    }
}
