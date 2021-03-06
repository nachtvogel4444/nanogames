﻿// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


namespace NanoGames.Games.Bots
{
    internal class Bullet
    {
        public Vector Position;
        public Vector Velocity;
        public double Radius;
        public int Damage;
        
        public bool IsExploded = true;

        public Bullet(Vector pos, Vector vel)
        {
            Position = pos;
            Velocity = vel;
            Radius = 1;
            Damage = 10;
            IsExploded = false;
        }    
    }
}
