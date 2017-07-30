// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Circles
{
    internal class CirclesMatch : Match<CirclesPlayer>
    {
        private double t;
        private int stepsPerFrame = 10;

        protected override void Initialize()
        {
            /* This is called by the framework at the start of every match. */

            // time
            t = 1.0 / 60 / stepsPerFrame;

            // initialize each player
            for (int i = 0; i < Players.Count; ++i)
            {
                // position
                double x = 320 * Random.NextDouble();
                double y = 200 * Random.NextDouble();
                
                Players[i].Position = new Vector(x, y);
                Players[i].Position = new Vector(160, 100);

                // velocity
                double angle = 2 * Math.PI * Random.NextDouble();

                Players[i].Velocity = 10 * new Vector(Math.Cos(angle), Math.Sin(angle));
                Players[i].Velocity = 10 * new Vector(1, 0);

                // mass
                Players[i].Mass = 1;

                // Radius
                Players[i].Radius = 3;
            }
        }


        protected override void Update()
        {
            /*
             * This is called by the framework once every frame.
             * We have to draw draw onto every player's Graphics interface.
             */

            foreach (CirclesPlayer player in Players)
            {
                // do all the physics ( in increments  to get more robust)
                for (int i = 0; i < stepsPerFrame; i++)
                {
                    movePlayer(player);
                }

                // draw screen
                drawScreen();
            }
            
        }

        private void movePlayer(CirclesPlayer player)
        {
            // calculate forces on player
            Vector force = new Vector(0, 0);

            // input
            double power = 10;
            
            if (player.Input.Left.IsPressed)
            {
                force =  power * player.Velocity.RotatedLeft.Normalized;
            }

            if (player.Input.Right.IsPressed)
            {
                force = power * player.Velocity.RotatedRight.Normalized;
            }

            // calculate acceleration from forces
            player.Acceleration = force / player.Mass;

            // calculate new postion and velocity from acceleration
            player.Position = player.Position + player.Velocity * t + 0.5 * player.Acceleration * t * t;
            player.Velocity = player.Velocity + player.Acceleration * t;
        }

        private void drawScreen()
        {            
            // draw each player
            foreach (var player in Players)
            {
                Output.Graphics.Circle(player.LocalColor, player.Position, player.Radius);
            }
            
        }
    }
}
