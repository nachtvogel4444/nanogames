// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.CatchMe
{
    internal class FixedCircle
    {
        public Vector Position;
        public double Radius;

        public FixedCircle(Vector pos, double r)
        {
            Position = pos;
            Radius = r;
        }
    }
}
