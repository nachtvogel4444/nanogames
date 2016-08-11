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
        public int NLines;
        public int NPoints;
        private List<double> x = new List<double>();
        private List<double> y = new List<double>();
        private List<string> type = new List<string>();
        public double[] X;
        public double[] Y;
        public string[] Type;

        public Landscape(double[] inX, double[] inY, string[] inType)
        {
            Type = inType;

            NLines = inX.GetLength(0);

            for (int i = 0; i < NLines - 1; i++)
            {
                double dx = inX[i + 1] - inX[i];
                double dy = inY[i + 1] - inY[i];

                double dist = Math.Sqrt(dx * dx + dy * dy);

                int n = (int)(dist / Constants.Dx) + 1;
                dx = dx / n;
                dy = dy / n;

                for (int j = 0; j < n; j++)
                {
                    x.Add(inX[i] + j * dx);
                    y.Add(inY[i] + j * dy);
                    type.Add(inType[i]);
                }
            }

            x.Add(inX[NLines - 1]);
            y.Add(inY[NLines - 1]);

            X = x.ToArray();
            Y = y.ToArray();
            Type = type.ToArray();
            NPoints = X.GetLength(0);
        }
    }
}
