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
        private double dt;
        private double maxSpeed;

        private int stepsPerFrame = 10;
        private double minDistance = 1;

        private const double viscosity = 5;

        protected override void Initialize()
        {
            /* This is called by the framework at the start of every match. */

            // time
            dt = 1.0 / 60 / stepsPerFrame;

            // max speed
            maxSpeed = minDistance / dt;

            // initialize each player
            for (int i = 0; i < Players.Count; ++i)
            {
                var player = Players[i];
                
                // Radius
                player.Radius = 3;

                // position
                double x = 320 * Random.NextDouble();
                double y = 200 * Random.NextDouble();
                
                player.Position = new Vector(x, y);
                player.Position = new Vector(160, 100);

                // velocity
                double angle = 2 * Math.PI * Random.NextDouble();

                player.Velocity = speedCheck(10 * new Vector(Math.Cos(angle), Math.Sin(angle)));
                player.Velocity = speedCheck(20 * new Vector(1, 0));

                // phi and dphi 
                player.Phi = 0;
                player.dPhi = 0;

                // mass and inertia
                player.Mass = 1.0 / 27 * player.Radius * player.Radius * player.Radius;
                player.Inertia = 1.0 / 9 * player.Radius * player.Radius;

                
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
            // The effects force and torque are calculated. These effects take place at the end of each time step
            // => velocity changes, position changes and after that the player rotates
            // This rotation of the previous timestep is implemented at the beginning of the next timestep

            // rotation of last time step is forced on velocity vector
            player.Velocity = player.Velocity.Length * new Vector(Math.Cos(player.Phi), Math.Sin(player.Phi));

            // calculate forces and torque on player
            Vector force = new Vector(0, 0);
            double torque = 0;

            // input
            double power = 20;
            
            // player moves forward
            if (player.Input.Left.IsPressed && player.Input.Right.IsPressed)
            {
                force += 2 * power * player.Velocity.Normalized;
            }

            // player moves forward with boost
            if (player.Input.Left.IsPressed && player.Input.Right.IsPressed && player.Input.Fire.IsPressed)
            {
                force += 2 * power * player.Velocity.Normalized;
            }

            // players moves right
            if (player.Input.Right.IsPressed && !player.Input.Left.IsPressed)
            {
                force += 1 * power * player.Velocity.Normalized;
                torque += 0.08 * (player.Radius + 2) * power;
            }

            // player moves left
            if (player.Input.Left.IsPressed && !player.Input.Right.IsPressed)
            {
                force += 1 * power * player.Velocity.Normalized;
                torque -= 0.08 * (player.Radius + 2) * power;
            }

            // players circles right
            if (player.Input.Right.IsPressed && !player.Input.Left.IsPressed && player.Input.Fire.IsPressed)
            {
                torque += 0.16 * (player.Radius + 2) * power;
            }

            // player cirlces left
            if (player.Input.Left.IsPressed && !player.Input.Right.IsPressed && player.Input.Fire.IsPressed)
            {
                torque -= 0.16 * (player.Radius + 2) * power;
            }

            // viscosity
            force -= 0.05 * player.Radius * viscosity * player.Velocity;
            torque -= 0.2 * player.Radius * player.Radius * player.dPhi;

            // calculate acceleration and angular acceleration from forces and torque
            Vector acceleration = force / player.Mass;
            double angularAcc = torque / player.Inertia;

            // calculate new postion and velocity from acceleration
            player.Position = player.Position + player.Velocity * dt + 0.5 * acceleration * dt * dt;
            player.Velocity = speedCheck(player.Velocity + acceleration * dt);

            // calculate new phi and dphi
            player.Phi = player.Phi + player.dPhi * dt + 0.5 * angularAcc * dt * dt;
            player.dPhi = speedCheck(player.dPhi + angularAcc * dt, player.Radius);

        }

        private void drawScreen()
        {
            var g = Output.Graphics;
                   
            // draw each player
            foreach (var player in Players)
            {
                var pos = player.Position;
                var radius = player.Radius;
                var dir = player.Velocity.Normalized;
                var ortho = player.Velocity.Normalized.RotatedLeft;
                var col = player.LocalColor;

                // body
                g.Circle(col, pos, radius);
                g.Line(col, pos - radius * dir, pos + radius * dir);

                // booster
                var pos1 = pos + (radius + 1) * ortho + dir;
                var pos2 = pos - (radius + 1) * ortho + dir;
                
                g.Line(col, pos1, pos1 - 3 * dir);
                g.Line(col, pos1 + ortho, pos1 + ortho - 3 * dir);
                g.Line(col, pos1, pos1 + ortho);
                g.Line(col, pos1 - 3 * dir, pos1 + ortho - 3 * dir);

                g.Line(col, pos2, pos2 - 3 * dir);
                g.Line(col, pos2 - ortho, pos2 - ortho - 3 * dir);
                g.Line(col, pos2, pos2 - ortho);
                g.Line(col, pos2 - 3 * dir, pos2 - ortho - 3 * dir);
            }
            
        }

        private Vector speedCheck(Vector input)
        {
            if (input.Length > maxSpeed)
            {
                return input.Normalized * maxSpeed;
            }
            else
            {
                return input;
            }
        }

        private double speedCheck(double dphi, double radius)
        {
            if ((radius * dphi) > maxSpeed)
            {
                return maxSpeed / radius;
            }
            else
            {
                return dphi;
            }
        }
    }
}
