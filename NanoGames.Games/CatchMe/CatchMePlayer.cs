// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.CatchMe
{
    class CatchMePlayer : Player<CatchMeMatch>
    {

        public Vector Position;
        public double Phi;
        public Vector Velocity;
        public double DPhi;
        public double Mass;
        public double Inertia;
        public double Radius;

        public int BoosterLeft;
        public int BoosterRight;
        public double BoosterPower;

        public bool InputMoveForward;
        public bool InputMoveBackward;
        public bool InputMoveLeft;
        public bool InputMoveRight;
        public bool InputCircleLeft;
        public bool InputCircleRight;
        public bool InputTurbo;

        public double TurboCount;
        public double TurboNotCount;
    }
}
