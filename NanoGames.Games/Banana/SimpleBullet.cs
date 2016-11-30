// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    internal class Bullet
    {
        public Vector Position;
        public Vector PositionBefore;
        private Vector velocity;
        
        public bool IsExploded = true;

        public void StartBullet(Vector pos, Vector vel)
        {
            Position = pos;
            PositionBefore = pos - vel;
            velocity = vel;
            IsExploded = false;
        }

        public void MoveBullet(Wind w)
        {
            PositionBefore = Position; 
            Position += velocity + 0.5 * new Vector(0, Constants.Gravity);
            // velocity += new Vector(0, Constants.Gravity) + 0.03 * new Vector(w.Speed - velocity.X, -velocity.Y);
            velocity += new Vector(0, Constants.Gravity) + 0.03 * new Vector(w.Speed, 0);
        }       
    }
}
