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

        static public double NextDoubleLinDist(Random ran)
        {
            return Math.Sqrt(ran.NextDouble());
        }

        static public double NextDoubleDist(Random ran, double e)
        {
            return Math.Pow(ran.NextDouble(), e);
        }

        static public double NextDoubleQuadDist(Random ran)
        {
            return Math.Pow(ran.NextDouble(), 1.0 / 3.0);
        }

        static public double NextDoubleCubeDist(Random ran)
        {
            return Math.Pow(ran.NextDouble(), 0.25);
        }

        static public double NextDoubleSqrtDist(Random ran)
        {
            return Math.Pow(ran.NextDouble(), 2.0 / 3.0);
        }

        static public List<Color> ColorGradient(Color start, Color stop, int n)
        {
            double dr = -(start.R - stop.R) / n;
            double dg = -(start.G - stop.G) / n;
            double db = -(start.B - stop.B) / n;

            List<Color> colors = new List<Color> { };
            for (int idx=0; idx < n; idx++)
            {
                colors.Add(new Color(start.R + idx * dr, start.G + idx * dg, start.B + idx * db));
            }

            return colors;
        }
    }
}
