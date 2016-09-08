// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    class Grenade
    {
        public Vector Position;
        public Vector PositionBefore;
        public Vector Velocity;
        public bool IsExploded = false;
        
        private int frameCount = 0;
        public double Radius = 1;
        private int lifeTime = 3 * 60;

        public Grenade(Vector pos, Vector vel)
        {
            Position = pos;
            PositionBefore = pos - vel;
            Velocity = vel;
        }

        public void MoveGrenade()
        {
            PositionBefore = Position;
            Position += Velocity + 0.5 * new Vector(0, Constants.Gravity);
            Velocity += new Vector(0, Constants.Gravity);
            frameCount++;

            if (frameCount > lifeTime)
            {
                IsExploded = true;
            }
        }
    }
}
