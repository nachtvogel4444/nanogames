// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


namespace NanoGames.Games.Infinity
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
