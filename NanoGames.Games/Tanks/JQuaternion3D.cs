// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;

namespace NanoGames.Games.Tanks
{
    public class JQuaternion3D
    {
        public double X;
        public double Y;
        public double Z;
        public double W;


        public JQuaternion3D(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;

            Normalize();
        }

        public JQuaternion3D(JVertex3D axis, double angle)
        {
            axis = axis.Normalized; // this provides that the quaternion is normalized

            X = axis.X * Math.Sin(angle / 2);
            Y = axis.Y * Math.Sin(angle / 2);
            Z = axis.Z * Math.Sin(angle / 2);
            W = Math.Cos(angle / 2);            
        }

        public JQuaternion3D(JVertex3D a, JVertex3D b) // rotation from a to b
        {
            JVertex3D v = JVertex3D.Cross(a, b).Normalized;

            X = v.X;
            Y = v.Y;
            Z = v.Z;
            W = Math.Sqrt(a.SquaredLength * b.SquaredLength) + JVertex3D.Dot(a, b);
        }


        public JQuaternion3D Normalized()
        {
            double length = Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

            if (length == 0)
            {
                return this;
            }
            else
            {
                return new JQuaternion3D(X / length, Y / length, Z / length, W / length);
            }
        }

        public void Normalize()
        {
            double length = Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
            
            if (length != 0)
            {
                X /= length;
                Y /= length;
                Z /= length;
                W /= length;
            }
        }
    }
}
