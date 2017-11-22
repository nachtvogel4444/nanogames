// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Globalization;

namespace NanoGames.Games.Tanks2
{
    /// <summary>
    /// A 3D vector.
    /// </summary>
    public struct Vector3
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
        /// The Z coordinate.
        /// </summary>
        public double Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Gets the squared length of the vector.
        /// </summary>
        public double SquaredLength => X * X + Y * Y + Z * Z;

        /// <summary>
        /// Gets the length of the vector.
        /// </summary>
        public double Length => Math.Sqrt(SquaredLength);

        /// <summary>
        /// Gets the vector normalized to length 1.
        /// </summary>
        public Vector3 Normalized => this / Length;

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.X, -v.Y, -v.Z);
        }

        public static Vector3 operator *(double f, Vector3 v)
        {
            return new Vector3(f * v.X, f * v.Y, f * v.Z);
        }

        public static Vector3 operator *(Vector3 v, double f)
        {
            return new Vector3(f * v.X, f * v.Y, f * v.Z);
        }

        public static Vector3 operator /(Vector3 v, double f)
        {
            return (1 / f) * v;
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Computes the linear combination of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <param name="ratio">The mix ratio.</param>
        /// <returns>The linear combination of two vectors.</returns>
        public static Vector3 Mix(Vector3 a, Vector3 b, double ratio)
        {
            return a * (1 - ratio) + b * ratio;
        }

        /// <summary>
        /// Returns the dot product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <summary>
        /// Returns the cross product of two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>The dot product.</returns>
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(a.Y * b.Z - a.Z * b.Y,
                               a.Z * b.X - a.X * b.Z,
                               a.X * b.Y - a.Y * b.X);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Vector3 && this == (Vector3)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return X.GetHashCode() * unchecked((int)0xf8f5a063) + Y.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})", X, Y, Z);
        }
    }
}

