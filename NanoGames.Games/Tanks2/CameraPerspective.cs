// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Tanks2
{
    public class CameraPerspective
    {
        public Vector3 Position;

        public double FOVX = 65 * Math.PI / 180;
        public double FOVY = 40 * Math.PI / 180;

        public double ZFar = 300;
        public double ZNear = 10;

        private Vector3 direction;
        private Quaternion rotation;
        private Vector3 zaxis;


        public CameraPerspective(Vector3 pos, Vector3 dir)
        {
            zaxis = new Vector3(0, 0, 1);
            Position = pos;
            Direction = dir.Normalized;
        }


        public Vector3 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
                rotation = new Quaternion(zaxis, direction);
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
                direction = zaxis * rotation;
            }
        }

        public void CenterToPoint(Vector3 p)
        {
            Direction = (Position - p).Normalized;
        }

    }
}
