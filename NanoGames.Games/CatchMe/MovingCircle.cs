// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.CatchMe
{
    internal class MovingCircle
    {
        // moving circle / obstacle

        public Vector Position;
        public double Phi;
        public Vector Velocity;
        public double DPhi;
        public Vector Heading;
        public double Mass;
        public double Inertia;
        public double Radius;

    }
}
