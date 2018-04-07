// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Infinity
{
    class InfinityPlayer : Player<InfinityMatch>
    {
        public Vector Position;
        public int X = int.MaxValue; // flo
        public int Y = int.MaxValue;
        public int TileIndex;
        public Vector Heading;
        public double HeadingPhi;
        public Vector Velocity;
        public double Rotation = 0;
        public double ZoomOut = 1;

        public double View = 100;

        public double Mass = 1;
        public double Size = 3;
        
        public double BoosterPower = 100;
        
        public List<Vector> LastPositions = new List<Vector> { };

        public List<Segment> Contour = new List<Segment> {
            new Segment( new Vector(-0.1, 1),   new Vector(0.1, 1)),
            new Segment( new Vector(0.1, 1),    new Vector(0.3, 0.5)),
            new Segment( new Vector(0.3, 0.5),  new Vector(0.3, 0.3)),
            new Segment( new Vector(0.3, 0.3),  new Vector(1, -0.1)),
            new Segment( new Vector(1, -0.1),   new Vector(1, -0.5)),
            new Segment( new Vector(1, -0.5),   new Vector(0.3, -0.7)),
            new Segment( new Vector(0.3, -0.7), new Vector(0.3, -1)),
            new Segment( new Vector(0.3, -1),   new Vector(-0.3, -1)),
            new Segment( new Vector(-0.1, 1),   new Vector(-0.3, 0.5)),
            new Segment( new Vector(-0.3, 0.5), new Vector(-0.3, 0.3)),
            new Segment( new Vector(-0.3, 0.3), new Vector(-1, -0.1)),
            new Segment( new Vector(-1, -0.1),  new Vector(-1, -0.5)),
            new Segment( new Vector(-1, -0.5),  new Vector(-0.3, -0.7)),
            new Segment( new Vector(-0.3, -0.7),new Vector(-0.3, -1))
        };

        public List<Segment> Triangle = new List<Segment>
        {
            new Segment(new Vector(0, 2),   new Vector(1, -1)),
            new Segment(new Vector(1, -1),  new Vector(-1, -1)),
            new Segment(new Vector(-1, -1), new Vector(0, 2)),
        };
        
        public void updateXY(double tileSize)
        {
            X = (int)Math.Floor(Position.X / tileSize);
            Y = (int)Math.Floor(Position.Y / tileSize);
        }
    }
}
