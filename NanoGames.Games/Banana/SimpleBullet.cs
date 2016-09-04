// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    internal class SimpleBullet
    {
        public Vector Position;
        public Vector PositionBefore;
        private Vector velocity;

        private Methods methods = new Methods();
        private double frameCount = 0;
        public string State = "Normal";

        public SimpleBullet(Vector pos, Vector vel)
        {
            Position = pos;
            PositionBefore = pos - vel;
            velocity = vel;
        }

        public void MoveSimpleBullet()
        {
            PositionBefore = Position; 
            Position += velocity + 0.5 * new Vector(0, Constants.Gravity) * (2 * frameCount + 1);
            velocity += new Vector(0, Constants.Gravity);
            frameCount++;
        }

        public void ChangeState(string state)
        {
            State = state;
        }
        
    }
}
