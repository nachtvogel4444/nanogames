// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.CatchMe
{
    class CatchMePlayer : Player<CatchMeMatch>
    {
        public Vector Position;
        public double Phi;
        public Vector Velocity;
        public double DPhi;
        public Vector Heading;
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
        public bool InputBlast;

        public int NumberOfBlast;

        public double TurboCount;
        public double TurboNotCount;
        public double TurboLength;
        public double TurboWait;

        public Vector ScreenPosition;

        public bool AddonJump;
        public bool AddonPush;
        public bool Addon2;
        public int AddonMarker;

        public double IntegratedDistance;

        public int Sound; // 0; silence, 1: noise, 2: toc, 3: explosion

        public List<Vector> LastPositions = new List<Vector> { };
    }
}
