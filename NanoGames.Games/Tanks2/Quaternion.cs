// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Globalization;

namespace NanoGames.Games.Tanks2
{
    /// <summary>
    /// A quaternion q = W + Xi + Yj + Zk.
    /// </summary>
    public struct Quaternion
    {
        /// <summary>
        /// The W component.
        /// </summary>
        public double W;

        /// <summary>
        /// The X component.
        /// </summary>
        public double X;

        /// <summary>
        /// The Y component.
        /// </summary>
        public double Y;

        /// <summary>
        /// The Z component.
        /// </summary>
        public double Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="w">The W component.</param>
        /// <param name="x">The X component.</param>
        /// <param name="y">The Y component.</param>
        /// <param name="z">The Z component.</param>
        public Quaternion(double w, double x, double y, double z)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="axis">The axis of the rotation.</param>
        /// <param name="angle">The angle of the rotation.</param>
        public Quaternion(Vector3 axis, double angle)
        {
            axis = axis.Normalized;

            W = Math.Cos(angle / 2);
            X = axis.X * Math.Sin(angle / 2);
            Y = axis.Y * Math.Sin(angle / 2);
            Z = axis.Z * Math.Sin(angle / 2);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="axis1">The first axis.</param>
        /// <param name="axis2">The second axis.</param>
        public Quaternion(Vector3 axis1, Vector3 axis2)
        {
            Vector3 a1 = axis1.Normalized;
            Vector3 a2 = axis2.Normalized;
            double d = Geometrie.Dot(a1, a2);

            if (d > (1 - 1e-6))
            {
                W = 1;
                X = 0;
                Y = 0;
                Z = 0;
            }

            else if (d < (-1 + 1e-6))
            {
                Vector3 rotAxis = Geometrie.Cross(a1, new Vector3(1, 0, 0));

                if (rotAxis.SquaredLength <= 1e-6)
                {
                    rotAxis = Geometrie.Cross(a1, new Vector3(0, 1, 0));
                }

                rotAxis = rotAxis.Normalized;

                this = new Quaternion(rotAxis, Math.PI);
            }

            else
            {
                double hc = Math.Sqrt(0.5 * (1.0 + d));
                Vector3 w = Geometrie.Cross(a1, a2) / (2.0 * hc);

                W = hc;
                X = w.X;
                Y = w.Y;
                Z = w.Z;

                this = Normalized;
            }

        }

        /// <summary>
        /// Gets the squared length of the quaternion.
        /// </summary>
        public double SquaredLength => W * W + X * X + Y * Y + Z * Z;

        /// <summary>
        /// Gets the length of the quaternion.
        /// </summary>
        public double Length => Math.Sqrt(SquaredLength);

        /// <summary>
        /// Gets the quaternion normalized to length 1.
        /// </summary>
        public Quaternion Normalized => this / Length;

        /// <summary>
        /// Gets the conjugate.
        /// </summary>
        public Quaternion Conjugate => new Quaternion(W, -X, -Y, -Z);

        public Vector3 ToVector3 => new Vector3(X, Y, Z);

        public static Quaternion operator +(Quaternion a, Quaternion b)
        {
            return new Quaternion(a.W + b.W, a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Quaternion operator -(Quaternion a, Quaternion b)
        {
            return new Quaternion(a.W - b.W, a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Quaternion operator -(Quaternion q)
        {
            return new Quaternion(-q.W, -q.X, -q.Y, -q.Z);
        }

        public static Quaternion operator *(double f, Quaternion q)
        {
            return new Quaternion(f * q.W, f * q.X, f * q.Y, f * q.Z);
        }

        public static Quaternion operator *(Quaternion q, double f)
        {
            return new Quaternion(f * q.W, f * q.X, f * q.Y, f * q.Z);
        }

        public static Quaternion operator /(Quaternion q, double f)
        {
            return (1 / f) * q;
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return new Quaternion(a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z,
                                  a.X * b.W + a.W * b.X + a.Y * b.Z - a.Z * b.Y,
                                  a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
                                  a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W);
        }

        public static Vector3 operator *(Vector3 v, Quaternion q)
        {
            Vector3 qv = q.ToVector3;
            Vector3 t = 2 * Geometrie.Cross(qv, v);
            return v + q.W * t + Geometrie.Cross(qv, t);
        }


        public static bool operator ==(Quaternion a, Quaternion b)
        {
            return a.W == b.W && a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator !=(Quaternion a, Quaternion b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Returns the dot product of two quaternions.
        /// </summary>
        /// <param name="a">The first quaternion.</param>
        /// <param name="b">The second quaternion.</param>
        /// <returns>The dot product.</returns>
        public static double Dot(Quaternion a, Quaternion b)
        {
            return a.W * b.W + a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is Quaternion && this == (Quaternion)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return X.GetHashCode() * unchecked((int)0xf8f5a063) + Y.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2}, {3})", W, X, Y, Z);
        }
    }
}

