// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Globalization;

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
        /// Gets the length of the vector.
        /// </summary>
        public double LengthBBox => Math.Abs(Math.Min(X, Y));

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

        public static bool operator <(Vector a, Vector b)
        {
            return Math.Abs(a.X) - Math.Abs(b.X) < 0 &&
                   Math.Abs(a.Y) - Math.Abs(b.Y) < 0;
        }

        public static bool operator >(Vector a, Vector b)
        {
            return Math.Abs(a.X) - Math.Abs(b.X) > 0 ||
                   Math.Abs(a.Y) - Math.Abs(b.Y) > 0;
        }
        /// <summary>
        /// Computes the linear combination of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="ratio">The mix ratio.</param>
        /// <returns>The linear combination of two vectors.</returns>
        public static Vector Mix(Vector a, Vector b, double ratio)
        {
            return a * (1 - ratio) + b * ratio;
        }

        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// Computes the unit vector with the given angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <returns>The unit vector with the given angle.</returns>
        public static Vector FromAngle(double angle)
        {
            return new Vector(Math.Cos(angle), Math.Sin(angle));
        }

        /// <summary>
        /// Returns a new Vector, rotated by the given angle.
        /// </summary>
        /// <param name="angle">The angle to rotate.</param>
        /// <returns>The rotated vector.</returns>
        public Vector Rotated(double angle)
        {
            var ca = Math.Cos(angle);
            var sa = Math.Sin(angle);
            return new Vector(ca * X - sa * Y, sa * X + ca * Y);
            //return new Vector(ca * X + sa * Y, -(sa * X - ca * Y));
        }

        /// <summary>
        /// Gets a translated vector.
        /// </summary>
        /// <param name="t">The translation vector.</param>
        /// <returns>"Returns translated Vector"</returns>
        public Vector Translated(Vector t) => this + t;

        /// <summary>
        /// Gets a scaled vector.
        /// </summary>
        /// <param name="s">The scaling factor.</param>
        /// <returns>"Returns the scaled vector"</returns>
        public Vector Scaled(double s) => s * this;

        /// <summary>
        /// Gets a to the origin translated vector.
        /// </summary>
        /// <returns>"Returns the translated Segment"</returns>
        public Vector ToOrigin() => this + new Vector(160, 100);

        /// <summary>
        /// Gets the mid point of this vector and other vector.
        /// </summary>
        /// <param name="otherVector">The other vector.</param>
        /// <returns>"Returns the vector to the mid point"</returns>
        public Vector MidPointTo(Vector otherVector) => 0.5 * (this + otherVector);

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
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1})", X, Y);
        }
    }
}
