// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Cluster
{
    static class Functions
    {
        static public double NextDoubleNormal(Random rand, double mu, double sigma)
        {
            double u1 = 1 - rand.NextDouble();
            double u2 = 1 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);

            return mu + sigma * randStdNormal;
        }

        static public double NextDoubleBtw(Random ran, double a, double b)
        {
            return a + (b - a) * ran.NextDouble();
        }
    }
}
