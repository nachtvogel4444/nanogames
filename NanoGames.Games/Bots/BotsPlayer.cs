// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bots
{
    internal class BotsPlayer : Player<BotsMatch>
    {
        public Vector Position;
        public Vector Aiming;
        public double Stepsize;

        public double Radius;
        public double GunLength;
        public double GunSpeed;

        public BotsPlayer()
        {
            Radius = 3;
            GunLength = 1.5 * Radius;
            Stepsize = 2;
            GunSpeed = 10;
        }
        
        public Vector GunTip => Position + GunLength * Aiming;
    }
}
