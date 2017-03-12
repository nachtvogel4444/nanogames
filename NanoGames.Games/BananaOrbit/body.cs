// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.BananaOrbit
{
    internal class Body
    {
        private Vector position;
        private int frame = 0;
        private int counter = 0;

        private Segment thigh1;
        private Color c = new Color(1, 1, 1);

        // animation in 8 steps, body walks 5-6 pixels
        // hip
        private Vector[] cycleHip = new Vector[8] {
            new Vector(0, 2.4),   // contact 
            new Vector(0, 2.0),   // recoil
            new Vector(0, 2.2),   // passing
            new Vector(0, 2.8),   // highpoint
            new Vector(0, 2.4),   // contact
            new Vector(0, 2.0),   // recoil
            new Vector(0, 2.2),   // passing
            new Vector(0, 2.8)    // highpoint
        };

        // Knee1
        private Vector[] cycleKnee1 = new Vector[8] {
            new Vector(0.8, 1.4),
            new Vector(1.0, 1.0),
            new Vector(-0.4, 1.2),
            new Vector(-0.8, 1.6),
            new Vector(-0.6, 1.2),
            new Vector(-0.2, 1.4),
            new Vector(0.6, 1.6),
            new Vector(1.0, 2.4)
        };

        public Body(Vector pos)
        {
            position = pos;
            thigh1 = new Segment(cycleHip[counter], cycleKnee1[counter]);
        }

        public void Cycle()
        {
            frame++;
            
            if (mod(frame, 5)==0)
            {
                counter++;
                counter = mod(counter, 8);
                thigh1 = new Segment(position + cycleHip[counter], position + cycleKnee1[counter]);
            }
        }

        public void Draw(IGraphics g)
        {
            thigh1.Draw(g, c);
        }

        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

    }
}
