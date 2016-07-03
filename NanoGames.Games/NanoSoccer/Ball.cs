// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.NanoSoccer
{
    internal class Ball : Circle
    {
        public const double BallRadius = 4;
        public const double Tolerance = 1;

        public Vector Position { get; set; }

        public Vector Velocity { get; set; }

        public double Radius
        {
            get
            {
                return BallRadius;
            }
        }

        public double MaximumVelocity
        {
            get
            {
                return NanoSoccerMatch.MaxBallVelocity;
            }
        }

        public double Mass
        {
            get
            {
                return NanoSoccerMatch.BallMass;
            }
        }

        public void Draw(Graphics g)
        {
            g.Circle(Colors.White, Position, Radius);
            g.Circle(Colors.White, Position, Radius * 0.66d);
            g.Circle(Colors.White, Position, Radius * 0.33d);
        }
    }
}
