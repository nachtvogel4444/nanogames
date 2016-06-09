// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    class Polygon : List<Vector>
    {
        public bool IsClosed { get; set; }

        public Polygon(Vector[] vectors, bool isClosed)
        {
            foreach (Vector vector in vectors)
            {
                Add(vector);
            }

            IsClosed = isClosed;
        }

        public Polygon RotatePolygon(double angle)
        {
            Polygon newPolygon = new Polygon(new Vector[]{ }, IsClosed); 

            for (int i = 0; i < this.Count(); i++)
            {
                newPolygon.Add(this[i].RotateAngle(angle));
            }

            return newPolygon;
        }

        public Polygon RotatePolygon(double angle, Vector origin)
        {
            Polygon newPolygon = new Polygon(new Vector[] { }, IsClosed);

            for (int i = 0; i < this.Count(); i++)
            {
                newPolygon.Add(this[i].RotateAngle(angle, origin));
            }

            return newPolygon;
        }
    }
}
