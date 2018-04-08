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
        public Vector Velocity;

        public bool IsExploded = true;

        public void StartBullet(Vector pos, Vector vel)
        {
            Position = pos;
            PositionBefore = pos - vel;
            Velocity = vel;
            IsExploded = false;
        }

        public void MoveBullet(Wind w)
        {
            PositionBefore = Position;
            Position += Velocity + 0.5 * new Vector(0, Constants.Gravity);
            Velocity += new Vector(0, Constants.Gravity) + 0.008 * new Vector(w.Speed, 0);
        }
    }
}
