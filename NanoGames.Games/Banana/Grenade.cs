// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    internal class Grenade
    {
        // show postion of 1 2 3 secs

        public Vector Position;
        public Vector PositionBefore;
        public Vector Velocity;
        public bool IsExploded = true;
        public bool IsDead = true;
        public double TimeLeft;

        private int frameCount = 0;
        public double Radius = 1;
        private int lifeTime = 0;

        public void StartGrenade(Vector pos, Vector vel, int time)
        {
            Position = pos;
            PositionBefore = pos - vel;
            Velocity = vel;
            IsExploded = false;
            IsDead = false;
            lifeTime = time;
            frameCount = 0;
            TimeLeft = time;
        }

        public void MoveGrenade()
        {
            if (Velocity.Length > 0.001)
            {
                PositionBefore = Position;
                Position += Velocity + 0.5 * new Vector(0, Constants.Gravity);
                Velocity += new Vector(0, Constants.Gravity);
            }
            frameCount++;
            TimeLeft--;

            if (frameCount > lifeTime)
            {
                IsExploded = true;
            }
        }

        public void Bounce(Vector intersection, Vector norm)
        {
            Vector incomming = Velocity.Normalized;
            Vector outgoing;
            Vector p1 = intersection - incomming;
            Vector mid;
            Vector p2;
            double l;

            double A1 = -intersection.Y + (intersection.Y + norm.Y);
            double B1 = intersection.X + norm.X - intersection.X;
            double C1 = A1 * intersection.X - B1 * intersection.Y;

            double A2 = -B1;
            double B2 = A1;
            double C2 = A2 * p1.X - B2 * p1.Y;

            double det = A1 * B2 - A2 * B1;

            if (det != 0)
            {
                mid.X = (B2 * C1 - B1 * C2) / det;
                mid.Y = (A1 * C2 - A2 * C1) / det;

                mid.Y = -mid.Y;
                p2 = mid - (p1 - mid);
            }
            else
            {
                p2 = p1;
            }
            

            outgoing = (p2 - intersection).Normalized;

            l = Velocity.Length - (PositionBefore - intersection).Length;
            
            Position = intersection + l * outgoing;

            Velocity = 0.6 * Velocity.Length * outgoing;

        }
    }
}
