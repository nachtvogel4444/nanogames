// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    public struct Line
    {
        /// <summary>
        /// The origin point.
        /// </summary>
        public Vector P;

        /// <summary>
        /// The directional vector nornmalized.
        /// </summary>
        public Vector M;

        /// <summary>
        /// Initializes a new instance of the <see cref="Line"/> struct.
        /// </summary>
        /// <param name="p">The origin point.</param>
        /// <param name="m">The directional vector nornmalized.</param>
        public Line(Vector p, Vector m)
        {
            P = p;
            M = m;
        }
        
        public bool IsInLine(Vector v)
        {
            if ((v.X - P.X) / M.X == (v.Y - P.Y) / M.Y)
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
