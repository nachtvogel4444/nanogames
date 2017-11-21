// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class JCamera
    {
        public JVertex3D Position;
        public JVertex3D Direction;
        public JQuaternion3D Rotation;

        public double Width = 320; 
        public double Heigth = 200;

        public double ZFar = 300;
        public double ZNear = 10;


        public JCamera(JVertex3D pos, JVertex3D dir)
        {
            Position = pos;
            Direction = dir.Normalized;
            Rotation = new JQuaternion3D(new JVertex3D(0, 0, 1, 0), dir);
        }


        public Vector GetImagePoint(JVertex3D v)
        {
            Vector p;

            p.X =  160.0 * v.X + 160.0;
            p.Y = -100.0 * v.Y + 100.0;

            return p;
        }

        public void CenterToPoint(JVertex3D pos)
        {
            Direction = new JVertex3D(pos.X - Direction.X, pos.Y - Direction.Y, pos.Z - Direction.Z, 0).Normalized;
            Rotation = new JQuaternion3D(new JVertex3D(0, 0, 1, 0), Direction);
        }
    }
}
