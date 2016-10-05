// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    class Landscape
    {
        public Vector[] Points;
        public string[] Surface;
        public int N;

        private double[,] points;
        private double[] y;

        public Landscape()
        {
            /*
            X = new double[2] { 0, 320 };
            Y = new double[2] { 100, 100 };
            Surface = new string[2] { "Normal", "Normal" };
            */
            
            points = new double[7, 2] { { 0, 100 }, { 50, 120 }, { 120, 80 }, { 170, 90 }, { 200, 95 }, { 250, 140 }, { 320, 100 } };
            Surface = new string[7-1] { "Normal", "Normal", "Normal", "Normal", "Normal", "Normal" };

            N = points.GetLength(0);

            Points = new Vector[N];
            for (int i = 0; i < N; i++)
            {
                Points[i] = new Vector(points[i, 0], points[i, 1]);
            }
        }
    }
}
