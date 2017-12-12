// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bots
{
    internal class Bot
    {
        public Vector Position;
        public Vector Aiming;
        public double Stepsize;

        public double Radius;
        public double GunLength;
        public double GunSpeed;
        public Color Color;

        public Bot()
        {
            Radius = 3;
            GunLength = 1.5 * Radius;
            GunSpeed = 10;
            Color = new Color(0.2, 0.2, 0.2);
            Stepsize = 2;
        }

        public Vector GunTip() => Position + GunLength * Aiming;
    }
}
