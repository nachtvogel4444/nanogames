// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks
{
    public class JCamera
    {
        public JVertex3D Position;
        public JVertex3D Direction;
        public JQuaternion3D Quaternion;

        public double Width = 320; 
        public double Heigth = 200;

        public double Zfar = 300;
        public double Znear = 10;

        public JCamera(JVertex3D pos, JVertex3D dir)
        {
            Position = pos;
            Direction = dir.Normalized;
            Quaternion = new JQuaternion3D(new JVertex3D(0, 0, 1, 0), dir);
        }

        public void CenterToPoint(JVertex3D pos)
        {
            Direction = (pos - Direction).Normalized;
        }
    }
}
