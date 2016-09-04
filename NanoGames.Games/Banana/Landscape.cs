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
        public int NPointsPolygon;
        public int NPointsInterpolated;
        public int NPointsTracks;
        private List<double> alpha = new List<double>();
        private List<double> x = new List<double>();
        private List<double> y = new List<double>();
        private List<double> xTracks = new List<double>();
        private List<double> yTracks = new List<double>();
        private List<string> type = new List<string>();
        public double[] Alpha;
        public double[] XPolygon;
        public double[] YPolygon;
        public string[] TypePolygon;
        public double[] XInterpolated;
        public double[] YInterpolated;
        public string[] TypeInterpolated;
        public double[] XTracks;
        public double[] YTracks;

        // public double[] lineX = new double[2] { 0, 320 };
        // public double[] lineY = new double[2] { 100, 100 };
        // public string[] lineType = new string[2] { "Normal", "Normal" };

        public double[] lineX = new double[7] { 0, 50, 120, 170, 200, 250, 320 };
        public double[] lineY = new double[7] { 100, 120, 80, 90, 95, 140, 100 };
        public string[] lineType = new string[7] { "Normal", "Normal", "Normal", "Normal", "Normal", "Normal", "Normal" };

        public double Tolerance = 0.1;

        public void createLandscape(double[] inX, double[] inY, string[] inType, double radiusPlayer)
        {
            // interpolate input polygon in Arrays, X: x-coordinates, Y: y coordinates, Type, type of wall
            // this is the line which is seen in the game as a landscape

            NPointsPolygon = inX.GetLength(0);

            for (int i = 0; i < NPointsPolygon - 1; i++)
            {
                double dx = inX[i + 1] - inX[i];
                double dy = inY[i + 1] - inY[i];

                double dist = Math.Sqrt(dx * dx + dy * dy);

                double xo = dy / dist;
                double yo = -dx / dist;

                double a = Math.Atan(dx / dy);              

                int n = (int)(dist / Tolerance) + 1;
                dx = dx / n;
                dy = dy / n;

                for (int j = 0; j < n; j++)
                {
                    double xtmp = inX[i] + j * dx;
                    double ytmp = inY[i] + j * dy;

                    x.Add(xtmp);
                    y.Add(ytmp);
                    type.Add(inType[i]);
                    
                    xTracks.Add(xtmp + radiusPlayer * xo);
                    yTracks.Add(ytmp + radiusPlayer * yo);

                    alpha.Add(a);
                }
            }

            x.Add(inX[NPointsPolygon - 1]);
            y.Add(inY[NPointsPolygon - 1]);

            XInterpolated = x.ToArray();
            YInterpolated = y.ToArray();
            TypeInterpolated = type.ToArray();
            NPointsInterpolated = XInterpolated.GetLength(0);

            XPolygon = inX;
            YPolygon = inY;
            TypePolygon = inType;

            XTracks = xTracks.ToArray();
            YTracks = yTracks.ToArray();
            NPointsTracks = XTracks.GetLength(0);

            Alpha = alpha.ToArray();


        }
    }
}
