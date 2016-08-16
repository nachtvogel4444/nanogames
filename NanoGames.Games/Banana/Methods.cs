// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    public class Methods
    {
        public bool CheckCollision(Vector start, Vector stop, Vector obstacle, double minDist)
        {
            // bc: from postion to tail
            // ba: from position to obstacle 
            double bcx = stop.X - start.X;
            double bcy = -stop.Y + start.Y;
            double bax = obstacle.X - start.X;
            double bay = -obstacle.Y + start.Y;
            double cax = obstacle.X - stop.X;
            double cay = -obstacle.Y + stop.Y;
            double distsq;

            double m = (bax * bcx + bay * bcy) / (bcx * bcx + bcy * bcy);

            if (m < 0)
            {
                return false;
            }

            else if (m > 1)
            {
                distsq = cax * cax + cay * cay;
            }

            else
            {
                double dx = obstacle.X - (start.X + m * bcx);
                double dy = -obstacle.Y - (-start.Y + m * bcy);
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
