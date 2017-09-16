// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


namespace NanoGames.Games.CatchMe
{
    internal class Ring
    {
        public Vector Position;
        public Vector Velocity;
        public Color Color;

        public double Time;

        public Ring(Vector pos, Color col)
        {
            Position = pos;
            Time = 0;
            Color = col;
        }
        
    }
}
