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
        public Vector StartPosition;
        public Vector Position;
        public Vector PostionTail;
        public Vector StartVelocity;
        public Vector Velocity;
        private Vector Accelaration;
        public double LifeTime;
        public bool IsExploded;

        public SimpleBullet(Vector startPosition, double angle, double velocity)
        {
            StartPosition = startPosition;
            StartVelocity = velocity * new Vector(Math.Cos(angle), -Math.Sin(angle));
            Accelaration = new Vector(0, Constants.Gravity);
            LifeTime = 0;
            IsExploded = false;
        }

        public void MoveSimpleBullet()
        {
            Position = StartPosition + LifeTime * StartVelocity + 0.5 * LifeTime * LifeTime * Accelaration;
            Velocity = StartVelocity + LifeTime * Accelaration;
            PostionTail = Position - Velocity;
            LifeTime++;
        }
    }
}
