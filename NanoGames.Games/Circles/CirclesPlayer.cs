// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Circles
{
    class CirclesPlayer : Player<CirclesMatch>
    {

        public Vector Position;
        public Vector Velocity;
        public Vector Acceleration;
        public double Mass;
        public double Radius;

    }
}
