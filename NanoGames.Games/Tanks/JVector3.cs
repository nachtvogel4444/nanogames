// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.
using System;

namespace NanoGames.Games.Tanks
{
    /// <summary>
    /// A 3D vector.
    /// </summary>
    public struct JVector3
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
        /// Initializes a new instance of the <see cref="JVertex"/> struct.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="z">The Z coordinate.</param>
        public JVector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double SquaredLength => X * X + Y * Y + Z * Z;
        
        public double Length => Math.Sqrt(SquaredLength);

        public JVector3 Normalized => this / Length;

        public static double Dot(JVector3 a, JVector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static JVector3 Cross(JVector3 a, JVector3 b)
        {
            return new JVector3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        public static JVector3 operator +(JVector3 a, JVector3 b)
        {
            return new JVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static JVector3 operator -(JVector3 a, JVector3 b)
        {
            return new JVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static JVector3 operator *(JVector3 v, double f)
        {
            return new JVector3(f * v.X, f * v.Y, f * v.Z);
        }

        public static JVector3 operator *(double f, JVector3 v)
        {
            return new JVector3(f * v.X, f * v.Y, f * v.Z);
        }

        public static JVector3 operator /(JVector3 v, double f)
        {
            return (1.0 / f) * v;
        }

        public static JVector3 operator -(JVector3 v)
        {
            return new JVector3(-v.X, -v.Y, -v.Z);
        }

        public JVector3 Translate(JVector3 trans)
        {
            return new JVector3(X + trans.X, Y + trans.Y, Z + trans.Z);
        }

        /*
        public JVector3 RotateX(double alpha)
        {
            return new JVector3(X, Math.Cos(alpha) * Y - Math.Sin(alpha) * Z, Math.Sin(alpha) * Y + Math.Cos(alpha) * Z);
        }

        public JVector3 RotateY(double beta)
        {
            return new JVector3(Math.Cos(beta) * X + Math.Sin(beta) * Z, Y, -Math.Sin(beta) * X + Math.Cos(beta) * Z);
        }

        public JVector3 RotateZ(double gamma)
        {
            return new JVector3(Math.Cos(gamma) * X - Math.Sin(gamma) * Y, Math.Sin(gamma) * X + Math.Cos(gamma) * Y, Z);
        }

        public JVector3 RotateQuaternion(JQuaternion qin)
        {
            JQuaternion q = qin.Normalized();

            double x = (1.0 - 2.0 * q.Y * q.Y - 2.0 * q.Z * q.Z) * X + 2.0 * (q.X * q.Y - q.Z * q.W)             * Y + 2.0 * (q.X * q.Z + q.Y * q.W) * Z;
            double y = 2.0 * (q.X * q.Y + q.Z * q.W)             * X + (1.0 - 2.0 * q.X * q.X - 2.0 * q.Z * q.Z) * Y + 2.0 * (q.Y * q.Z - q.X * q.W) * Z;
            double z = 2.0 * (q.X * q.Z - q.Y * q.W)             * X + 2.0 * (q.Y * q.Z + q.X * q.W)             * Y + (1.0 - 2.0 * q.X * q.X - 2.0 * q.Y * q.Y) * Z;

            return new JVector3(x, y, z);
        }

        public JVector3 Scale(double s)
        {
            return new JVector3(X * s, Y * s, Z * s);
        }

        public JVector3 Scale(double sx, double sy, double sz)
        {
            return new JVector3(X * sx, Y * sy, Z * sz);
        }

        public JVector3 TransformObjectToWorldSpace(JVector3 pos, JQuaternion q)
        {
            return RotateQuaternion(q).Translate(pos);
        }

        public JVector3 TransformWorldToViewSpace(JCamera camera)
        {
            return Translate(-camera.Position).RotateQuaternion(camera.Quaternion);
        }

        public JVector3 TransformViewToProjectedSpace(JCamera camera)
        {
            JVector3 scaled = Scale(1.0 / camera.Width, 1.0 / camera.Heigth, -2.0 / (camera.Zfar - camera.Znear));
            JVector3 moved = scaled.Translate(new JVector3(0, 0, -(camera.Zfar + camera.Znear) / (camera.Zfar - camera.Znear)));

            return moved;
        }

        public Vector ClipTo2D()
        {
            return new Vector((X + 1) * 160, 200 - (Y + 1) * 100);
        }
        */
    }
}
