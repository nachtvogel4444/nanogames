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
        public int eee = 34;
        /// <summary>
        /// Performs a 2D rotation of a point around the origin through an angle
        /// </summary>
        /// <param name="point"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector RotatePoint(Vector point, double angle)
        {
            var x = point.X * Math.Cos(angle) - point.Y * Math.Sin(angle);
            var y = point.X * Math.Sin(angle) + point.Y * Math.Cos(angle);

            return new Vector(x, y);
        }

        /// <summary>
        /// Performs a 2D shift of a point with shift
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        public Vector ShiftPoint(Vector vector, Vector shift)
        {
            var x = vector.X  + shift.X;
            var y = vector.Y  + shift.Y;

            return new Vector(x, y);
        }


        /// <summary>
        /// Performs a 2D rotation of a rectangle around the origin through an angle
        /// </summary>
        /// <param name="pointTopLeft"></param>
        /// <param name="pointBottomRight"></param>
        /// <param name="origin"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public List<Vector> RotateRectangle(Vector pointTopLeft, Vector pointBottomRight, Vector origin, double angle)
        {
            var ax = pointTopLeft.X - origin.X;
            var ay = pointTopLeft.Y - origin.Y;

            var bx = pointBottomRight.X - origin.X;
            var by = pointTopLeft.Y - origin.Y;

            var cx = pointBottomRight.X - origin.X;
            var cy = pointBottomRight.Y - origin.Y;

            var dx = pointTopLeft.X - origin.X;
            var dy = pointBottomRight.Y - origin.Y;

            Vector aa = RotatePoint(new Vector(ax, ay), angle);
            Vector bb = RotatePoint(new Vector(bx, by), angle);
            Vector cc = RotatePoint(new Vector(cx, cy), angle);
            Vector dd = RotatePoint(new Vector(dx, dy), angle);


            List<Vector> list = new List<Vector> {aa, bb, cc, dd };
            
            return list;
        }

    }
}
