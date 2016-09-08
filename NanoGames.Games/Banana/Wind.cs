// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    class Wind
    {
        private double mean = 0;
        private double sigma = 1;
        public double Speed = 0;

        public void SetSpeed(Random r)
        {
            double u1, u2, q, p;

            do
            {
                u1 = 2 * r.NextDouble() - 1;
                u2 = 2 * r.NextDouble() - 1;
                q = u1 * u1 + u2 * u2;
            } while (q == 0 || q >= 1);

            p = Math.Sqrt(-2 * Math.Log(q) / q);

            Speed = u1 * p * sigma + mean;
            
        }
    }
}
