// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    public static class Geometry
    {
        

        public static Intersection Intersection(Vector p11, Vector p12, Vector p21, Vector p22)
        {
            double A1 = p11.Y - p12.Y;
            double B1 = p11.X - p12.X;
            double C1 = A1 * p11.X + B1 * p12.Y;
            double A2 = p21.Y - p22.Y;
            double B2 = p21.X - p22.X;
            double C2 = A2 * p21.X + B2 * p22.Y;

            double det = A1 * B2 - A2 * B1; 

            if (det == 0)
            {
                return 
            }
            else
            {
                double x = (B2 * C1 - B1 * C2) / det;
                double y = (A1 * C2 - A2 * C1) / det;
            }

        }

    }
}
