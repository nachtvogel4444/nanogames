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

        public JQuaternion3D()
        {
            X = 1;
            Y = 0;
            Z = 0;
            W = 0;
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
            double normAB = Math.Sqrt(a.SquaredLength * b.SquaredLength);
            double ct = JVertex3D.Dot(a, b) / normAB;
            double hc = Math.Sqrt(0.5 * (1.0 + ct));
            JVertex3D w = JVertex3D.Cross(a, b) / (normAB * 2.0 * hc);

            X = w.X;
            Y = w.Y;
            Z = w.Z;
            W = hc;

            Normalize();

            /*
            float norm_u_norm_v = sqrt(sqlength(u) * sqlength(v));
            float cos_theta = dot(u, v) / norm_u_norm_v;
            float half_cos = sqrt(0.5f * (1.f + cos_theta));
            vec3 w = cross(u, v) / (norm_u_norm_v * 2.f * half_cos);
            return quat(half_cos, w.x, w.y, w.z);
            */
        }
        
        public static JQuaternion3D operator -(JQuaternion3D q)
        {
            q.Normalize();
            return new JQuaternion3D(-q.X, -q.Y, -q.Z, q.W);
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
