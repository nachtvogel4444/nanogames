// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.CatchMe
{
    internal class CatchMeMatch : Match<CatchMePlayer>
    {
        private double dt;
        private double dT;
        private double maxSpeed;
        private string stateOfGame;
        private int frameCounter;
        private int frameCounterStart;
        private int frameCounterEnd;
        private CatchMePlayer prey;
        private Vector CatchPosisition;

        private int stepsPerFrame = 10;
        private double minDistance = 1;
        private List<Flake> flakes = new List<Flake> {};

        private const double viscosity = 5;

        protected override void Initialize()
        {
            /* This is called by the framework at the start of every match. */

            // stateOfGame, frameCounter
            stateOfGame = "Start";
            frameCounter = 0;
            frameCounterStart = 0;
            frameCounterEnd = 0;

            // time
            dt = 1.0 / 60 / stepsPerFrame;
            dT = 1.0 / 60;

            // max speed
            maxSpeed = minDistance / dt;

            // prey
            prey = Players[(int)(Players.Count * Random.NextDouble())];

            // initialize each player
            for (int i = 0; i < Players.Count; ++i)
            {
                var player = Players[i];
                
                // Radius
                player.Radius = 3;
                
                // Turbo
                player.TurboCount = 0;
                player.TurboNotCount = 0;

                // Booster
                player.BoosterPower = 20;

                // position
                double x = 270 * Random.NextDouble();
                double y = 150 * Random.NextDouble();
                
                player.Position = new Vector(x, y);
                // player.Position = new Vector(160, 100);

                // velocity
                double angle = 2 * Math.PI * Random.NextDouble();

                player.Velocity = speedCheck(4 * new Vector(Math.Cos(angle), Math.Sin(angle)));

                // phi and dphi 
                player.Phi = Math.Sin(player.Velocity.Normalized.Y);
                player.DPhi = 0;

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

            // match has severeal states

            switch (stateOfGame)
            {
                // start state
                // prey is able to move
                case "Start":
                    {
                        // check player input, only for prey
                        checkPlayerInput(prey);

                        // do all the physics ( in increments to get more robust)
                        for (int i = 0; i < stepsPerFrame; i++)
                        {
                            movePlayer(prey);
                        }

                        if (frameCounterStart > 120)
                        {
                            stateOfGame = "Game";
                        }

                        frameCounterStart++;

                        break;
                    }

                // game state
                // everyone is able to move
                case "Game":
                    {
                        // check input
                        foreach (CatchMePlayer player in Players)
                        {
                            checkPlayerInput(player);
                        }

                        // do all the physics ( in increments to get more robust)
                        for (int i = 0; i < stepsPerFrame; i++)
                        {

                            // move each player
                            foreach (CatchMePlayer player in Players)
                            {
                                movePlayer(player);
                            }

                            // check for collsion with prey
                            foreach (CatchMePlayer player in Players)
                            {
                                if (player != prey)
                                {
                                    // get dist of player to prey
                                    double dist = (prey.Position - player.Position).Length;

                                    if (dist < 10)
                                    {
                                        // get postion of catch
                                        CatchPosisition = (prey.Position + player.Position) / 2;
                                        
                                        // end game
                                        stateOfGame = "End";
                                    }
                                }
                            }

                        }

                        break;
                    }

                // end state
                // show end animation
                case "End":
                    {
                        // make an explosion with flakes
                        Vector pos = CatchPosisition;
                        Color col = new Color(1, 1, 1);
                        double alpha;
                        double delta;
                        double fade;
                        Vector direction;

                        for (int j = 0; j < 100; j++)
                        {
                            alpha = 2 * Math.PI * Random.NextDouble();
                            delta = 5 * Random.NextDouble();
                            fade = Math.Pow(0.99, frameCounterEnd + 1);
                            direction.X = Math.Cos(alpha);
                            direction.Y = Math.Sin(alpha);
                            flakes.Add(new Flake(pos + delta * direction, direction * 50, col * fade));
                        }

                        // end game if x seconds passed
                        if (frameCounterEnd > 300)
                        {
                            IsCompleted = true;
                        }
                        
                        frameCounterEnd++;

                        break;
                    }
            }

            // draw screen
            drawScreen();

            // framecounter
            frameCounter++;
        }

        private void movePlayer(CatchMePlayer player)
        {
            // The effects force and torque are calculated. These effects take place at the end of each time step
            // => velocity changes, position changes and after that the player rotates
            // This rotation of the previous timestep is implemented at the beginning of the next timestep

            // rotation of last time step is forced on velocity vector
            player.Velocity = player.Velocity.Length * new Vector(Math.Cos(player.Phi), Math.Sin(player.Phi));

            // calculate forces and torque on player

            // set inintial values to zero
            Vector force = new Vector(0, 0);
            double torque = 0;

            double power;

            // calculate power of one booster
            if (!player.InputTurbo)
            {
                power = player.BoosterPower;
            }
            else
            {
                power = 3 * player.BoosterPower;
            }

            // player moves forward
            if (player.InputMoveForward)
            {
                force += 2 * power * player.Velocity.Normalized;
            }

            // players moves right
            if (player.InputMoveRight)
            {
                force += 1 * power * player.Velocity.Normalized;
                torque += 0.08 * (player.Radius + 1.5) * power;
            }

            // player moves left
            if (player.InputMoveLeft)
            {
                force += 1 * power * player.Velocity.Normalized;
                torque -= 0.08 * (player.Radius + 1.5) * power;
            }

            // players circles right
            if (player.InputCircleRight)
            {
                torque += 0.05 * (player.Radius + 1.5) * power;
            }

            // player cirlces left
            if (player.InputCircleLeft)
            {
                torque -= 0.05 * (player.Radius + 1.5) * power;
            }

            // if force is zero add "standgas"
            if (force.Length == 0)
            {
                force = 4 * player.Velocity.Normalized;
            }

            // viscosity
            force -= 0.05 * player.Radius * viscosity * player.Velocity;
            torque -= 0.04 * player.Radius * viscosity * player.Radius * player.DPhi;

            // calculate acceleration and angular acceleration from forces and torque
            Vector acceleration = force / player.Mass;
            double angularAcc = torque / player.Inertia;

            // calculate new postion and velocity from acceleration
            player.Position = player.Position + player.Velocity * dt + 0.5 * acceleration * dt * dt;
            player.Velocity = speedCheck(player.Velocity + acceleration * dt);

            // calculate new phi and dphi
            player.Phi = player.Phi + player.DPhi * dt + 0.5 * angularAcc * dt * dt;
            player.DPhi = speedCheck(player.DPhi + angularAcc * dt, player.Radius);
        }

        private void checkPlayerInput(CatchMePlayer player)
        {
            // set public variables of player according to input

            // reset all 
            player.InputMoveForward = false;
            player.InputMoveBackward = false;
            player.InputMoveLeft = false;
            player.InputMoveRight = false;
            player.InputCircleLeft = false;
            player.InputCircleRight = false;
            player.InputTurbo = false;
            player.BoosterLeft = 0;
            player.BoosterRight = 0;

            // player moves forward
            if (player.Input.Left.IsPressed && player.Input.Right.IsPressed)
            {
                player.InputMoveForward = true;
                player.BoosterRight = 1;
                player.BoosterLeft = 1;
            }

            // player gets turbo if possible
            if (player.Input.AltFire.IsPressed && (player.TurboCount < 40) && (player.TurboNotCount > 120))
            {
                player.InputTurbo = true;

                player.TurboCount++;

                // turbonotcount is reset
                if (player.TurboCount == 40)
                {
                    player.TurboNotCount = 0;
                }
            }
            else
            {
                player.TurboNotCount++;

                // turbocount is reset
                if (player.TurboNotCount == 120)
                {
                    player.TurboCount = 0;
                }
            }

            // players moves right
            if (player.Input.Right.IsPressed && !player.Input.Left.IsPressed)
            {
                player.InputMoveRight = true;
                player.BoosterRight = 1;
            }

            // player moves left
            if (player.Input.Left.IsPressed && !player.Input.Right.IsPressed)
            {
                player.InputMoveLeft = true;
                player.BoosterLeft = 1;
            }

            // players circles right
            if (player.Input.Right.IsPressed && !player.Input.Left.IsPressed && player.Input.Fire.IsPressed)
            {
                player.InputCircleRight = true;
                player.BoosterLeft = 1;
                player.BoosterRight = -1;
            }

            // player cirlces left
            if (player.Input.Left.IsPressed && !player.Input.Right.IsPressed && player.Input.Fire.IsPressed)
            {
                player.InputCircleRight = true;
                player.BoosterLeft = -1;
                player.BoosterRight = 1;
            }
        }

        private void drawScreen()
        {
            var g = Output.Graphics;
            var boostCol = new Color(1, 0, 0);
                   
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

                // add flakes
                double alpha;
                double delta;
                Vector direction;

                if (!player.InputTurbo)
                {
                    for (int i = 0; i < 5; i++)
                    {   
                        // booster right
                        alpha = Math.PI / 4 * Random.NextDouble() - Math.PI / 8;
                        delta = 100 * dT * Random.NextDouble();
                        direction.X = Math.Cos(alpha) * dir.X + Math.Sin(alpha) * dir.Y;
                        direction.Y = +Math.Sin(alpha) * dir.X + Math.Cos(alpha) * dir.Y;
                        flakes.Add(new Flake(pos1 - (3 + delta) * dir + 0.5 * ortho, -direction * 100 * player.BoosterRight, col));

                        // booster left
                        alpha = Math.PI / 4 * Random.NextDouble() - Math.PI / 8;
                        delta = 100 * dT * Random.NextDouble();
                        direction.X = Math.Cos(alpha) * dir.X + Math.Sin(alpha) * dir.Y;
                        direction.Y = +Math.Sin(alpha) * dir.X + Math.Cos(alpha) * dir.Y;
                        flakes.Add(new Flake(pos2 - (3 + delta) * dir - 0.5 * ortho, -direction * 100 * player.BoosterLeft, col));
                    }

                        
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    { 
                        // booster right
                        alpha = Math.PI / 3 * Random.NextDouble() - Math.PI / 6;
                        delta = 100 * 2 * dT * Random.NextDouble();
                        direction.X = Math.Cos(alpha) * dir.X + Math.Sin(alpha) * dir.Y;
                        direction.Y = +Math.Sin(alpha) * dir.X + Math.Cos(alpha) * dir.Y;
                        flakes.Add(new Flake(pos1 - (3 + delta) * dir + 0.5 * ortho, -direction * 100 * player.BoosterRight * 2, boostCol));

                        // booster left
                        alpha = Math.PI / 3 * Random.NextDouble() - Math.PI / 6;
                        delta = 100 * 2 * dT * Random.NextDouble();
                        direction.X = Math.Cos(alpha) * dir.X + Math.Sin(alpha) * dir.Y;
                        direction.Y = +Math.Sin(alpha) * dir.X + Math.Cos(alpha) * dir.Y;
                        flakes.Add(new Flake(pos2 - (3 + delta) * dir - 0.5 * ortho, -direction * 100 * player.BoosterLeft * 2, boostCol));
                    }
                       
                }
            }

            // draw output from boosters
            
            // draw all flakes
            foreach (Flake flake in flakes)
            {
                g.Point(flake.Color, flake.Position);
            }

            // move flakes, set time and fade out
            foreach (Flake flake in flakes)
            {
                // velocity and postition
                flake.Position = flake.Position + flake.Velocity * dT - 0.5 * 0.001 * viscosity * flake.Velocity.Normalized * dT * dT;
                flake.Velocity = flake.Velocity + 0.001 * viscosity * flake.Velocity.Normalized * dT;

                // fade
                flake.Color = flake.Color * 0.85;

                // time
                flake.Time++;
            }

            // remove old flakes
            List<Flake> tmp = new List<Flake> { };
            for (int i = 0; i < flakes.Count; i++)
            {
                if (flakes[i].Time < 20)
                {
                    tmp.Add(flakes[i]);
                }
            }
            flakes = tmp;

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
