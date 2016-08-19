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
        public Vector PositionTail;
        public Vector StartVelocity;
        public Vector Velocity;
        private Vector Accelaration;
        public double LifeTime;
        public string State = "Normal";

        public SimpleBullet(Vector startPosition, Vector startVelocity)
        {
            StartPosition = startPosition;
            StartVelocity = startVelocity;
            Accelaration = new Vector(0, Constants.Gravity);
            LifeTime = 0;
        }

        public void MoveSimpleBullet()
        {
            Position = StartPosition + LifeTime * StartVelocity + 0.5 * LifeTime * LifeTime * Accelaration;
            Velocity = StartVelocity + LifeTime * Accelaration;
            PositionTail = Position - Velocity;
            LifeTime++;
        }

        public void ChangeState(string state)
        {
            State = state;
        }

        public bool CheckCollision(Vector obstacle, double minDist)
        {
            // bc: from postion to tail
            // ba: from position to obstacle 
            double bcx = PositionTail.X - Position.X;
            double bcy = -PositionTail.Y + Position.Y;
            double bax = obstacle.X - Position.X;
            double bay = -obstacle.Y + Position.Y;
            double cax = obstacle.X - PositionTail.X;
            double cay = -obstacle.Y + PositionTail.Y;
            double distsq;

            double m = (bax * bcx + bay * bcy) / (bcx * bcx + bcy * bcy);

            if (m < 0)
            {
                distsq = bax * bax + bay * bay;
            }

            else if (m > 1)
            {
                distsq = cax * cax + cay * cay; 
            }

            else
            {
                double dx = obstacle.X - (Position.X + m * bcx);
                double dy = -obstacle.Y - (-Position.Y + m * bcy);
                distsq = dx * dx + dy * dy;
            }



            if (distsq < (minDist * minDist))
            {
                return true;
            }

            else
            {
                return false;
            }
  
        }
    }
}
