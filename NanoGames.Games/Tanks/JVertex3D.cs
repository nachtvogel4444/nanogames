// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Tanks
{
    public struct JVertex3D
    {
        public double X;
        public double Y;
        public double Z;
        public double W; // is zero for direction, one for point in space


        public JVertex3D(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }


        public double SquaredLength => X * X + Y * Y + Z * Z;

        public double Length => Math.Sqrt(SquaredLength);

        public JVertex3D Normalized => this / Length;

        public static double Dot(JVertex3D a, JVertex3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static JVertex3D Cross(JVertex3D a, JVertex3D b)
        {
            return new JVertex3D(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X,
                0);
        }

        public static JVertex3D operator *(JVertex3D v, double f)
        {
            return new JVertex3D(f * v.X, f * v.Y, f * v.Z, v.W);
        }

        public static JVertex3D operator *(double f, JVertex3D v)
        {
            return new JVertex3D(f * v.X, f * v.Y, f * v.Z, v.W);
        }

        public static JVertex3D operator /(JVertex3D v, double f)
        {
            return new JVertex3D(v.X / f, v.Y / f, v.Z / f, v.W);
        }

        public static JVertex3D operator -(JVertex3D v)
        {
            return new JVertex3D(-v.X, -v.Y, -v.Z, v.W);
        }
        
        public static bool operator ==(JVertex3D a, JVertex3D b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        public static bool operator !=(JVertex3D a, JVertex3D b)
        {
            return !(a == b);
        }


        /* old methods
        public static JVertex3D operator +(JVertex3D a, JVertex3D b)
        {
            return new JVertex3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z, 0);            
        }

        public static JVertex3D operator -(JVertex3D a, JVertex3D b)
        {
            return new JVertex3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z, 0);
            
        }
         */
    }
}
