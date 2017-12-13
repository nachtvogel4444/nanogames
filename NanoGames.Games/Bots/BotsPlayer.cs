// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Bots
{
    internal class BotsPlayer : Player<BotsMatch>
    {
        public Vector Position;
        public Vector Direction;
        public double StepSize;
        public double AngleSize;
        public int HealthPoints;
        public bool IsDead;

        public double Radius;
        public double GunLength;
        public double GunSpeed;
        public Counter GunCounter;

        public BotsPlayer()
        {
            Radius = 3;
            GunLength = 1.5 * Radius;
            StepSize = 1.5;
            AngleSize = 15 * Math.PI / 180;
            GunSpeed = 3 * StepSize;
            GunCounter = new Counter(0.1, 0);
            HealthPoints = 100;
            IsDead = false;
        }
        
        public Vector GunTip => Position + GunLength * Direction;
        
    }
}
