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

        public static Vector PointPoint(Vector point1, Vector point2)
        {
            return point2 - point1;
        }

        public static Vector Intersection(Line line1, Line line2)
        {
            if (Vector.Dot(line1.M, line2.M) == 1)
            {
                return new Vector(0, 0);
            }

            else
            {

            }
        }

        public static Vector PointLine(Vector point, Line line)
        {


            return ;
        }
    }
}
