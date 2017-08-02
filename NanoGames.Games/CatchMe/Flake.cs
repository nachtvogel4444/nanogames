// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.CatchMe
{
    internal class Flake
    {
        public Vector Position;
        public Vector Velocity;
        public Color Color;

        public double Time;

        public Flake(Vector pos, Vector vel, Color col)
        {
            Position = pos;
            Velocity = vel;
            Time = 0;
            Color = col;
        }
        
    }
}
