// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;


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
        private List<Flake> flakes = new List<Flake> { };
        private List<MovingCircle> movingCircles = new List<MovingCircle> { };

        private double xMax;
        private double yMax;

        private const int stepsPerFrame = 10;
        private const double minDistance = 1;
        private const double viscosity = 5;

        private Color white = new Color(1, 1, 1);
        private Color red = new Color(1, 1, 1);
        private Color orange = new Color(1, 0.647, 0);
        private Color green = new Color(0, 1, 0);

        private Vector shift;
        private Vector screenPosition;

        private List<Vector> mapRandomPoints =  new List<Vector> {};


        protected override void Initialize()
        {
            /* This is called by the framework at the start of every match. */

            // stateOfGame, frameCounter
            stateOfGame = "Start";
            frameCounter = 0;
            frameCounterStart = 0;
            frameCounterEnd = 0;

            // map
            xMax = 400;
            yMax = 300;
            
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
                player.TurboWait = 360;
                player.TurboLength = 150;

                // Booster
                player.BoosterPower = 20;

                // position
                double x = (xMax - 3 * player.Radius) * Random.NextDouble() + 1.5 * player.Radius;
                double y = (yMax - 3 * player.Radius) * Random.NextDouble() + 1.5 * player.Radius;
                
                player.Position = new Vector(x, y);
                // player.Position = new Vector(160, 100);

                // phi, heading
                player.Phi = 2 * Math.PI * Random.NextDouble();
                player.Heading.X = Math.Cos(player.Phi);
                player.Heading.Y = Math.Sin(player.Phi);
                
                // mass and inertia
                player.Mass = 1.0 / 27 * player.Radius * player.Radius * player.Radius;
                player.Inertia = 1.0 / 9 * player.Radius * player.Radius;
            }

            // fill map with random points
            for (int i = 0; i < 250; i++)
            {
                mapRandomPoints.Add(randomPosition(new Vector(0, 0), new Vector(xMax, yMax)));                
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
            foreach (CatchMePlayer player in Players)
            {
                drawScreenPlayer(player);
            }

            // framecounter
            frameCounter++;
        }

        private void movePlayer(CatchMePlayer player)
        {
            //  - initial parameters are calculated
            //
            // - force and torque from input is calculted
            // - force if player hits boundary of map
            // - force and torque from viscosity
            // - acceleration, velocity and postion is calculated
            // - angular acceleration, dPhi and phi/heading is calculated

            
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
            
            // calculate forces and torque from input

            // standgas of boosters
            force = 2 * power / 10 * player.Heading;

            // player moves forward
            if (player.InputMoveForward)
            {
                force += 2 * power * player.Heading;
            }

            // players moves right
            if (player.InputMoveRight)
            {
                force += 1 * power * player.Heading;
                torque += 0.12 * (player.Radius + 1.5) * power;
            }

            // player moves left
            if (player.InputMoveLeft)
            {
                force += 1 * power * player.Heading;
                torque -= 0.12 * (player.Radius + 1.5) * power;
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

            // calculate force if player hits boundary of map

            // player hits left border
            if (player.Position.X < (player.Radius))
            {
                force += 1000 * (100 * Math.Atan((player.Radius) - player.Position.X) / Math.PI + 0.5) * new Vector(1, 0);
                torque += 30 * player.Radius * player.Velocity.Y / Math.Abs(player.Velocity.Y);
            }

            // player hits right border
            if (player.Position.X > (xMax - player.Radius))
            {
                force -= 1000 * (100 * Math.Atan(player.Position.X - (xMax - player.Radius)) / Math.PI + 0.5) * new Vector(1, 0);
                torque -= 30 * player.Radius * player.Velocity.Y / Math.Abs(player.Velocity.Y);
            }

            // player hits top border
            if (player.Position.Y < (player.Radius))
            {
                force += 1000 * (100 * Math.Atan((player.Radius) - player.Position.Y)  / Math.PI + 0.5) * new Vector(0, 1);
                torque -= 30 * player.Radius * player.Velocity.X / Math.Abs(player.Velocity.X);
            }

            // player hits bottom border
            if (player.Position.Y > (yMax - player.Radius))
            {
                force -= 1000 * (100 * Math.Atan(player.Position.Y - (yMax - player.Radius)) / Math.PI + 0.5) * new Vector(0, 1);
                torque += 30 * player.Radius * player.Velocity.X / Math.Abs(player.Velocity.X);
            }

            // if force is zero add "standgas"
            if (force.Length == 0)
            {
                force = 4 * player.Velocity.Normalized;
            }

            // viscosity
            force -= 0.05 * player.Radius * viscosity * player.Velocity;
            torque -= 0.04 * player.Radius * viscosity * player.Radius * player.DPhi;

            // for debug plot force vector in middle of the screen
            // Output.Graphics.Line(new Color(1, 1, 0), new Vector(160, 110), new Vector(160, 110) + force);
            // Output.Graphics.Circle(new Color(1, 1, 0), new Vector(160, 110), 1);

            // calculate acceleration and angular acceleration from forces and torque
            Vector acceleration = force / player.Mass;
            double angularAcc = torque / player.Inertia;

            // calculate new postion and velocity from acceleration
            player.Position = player.Position + player.Velocity * dt + 0.5 * acceleration * dt * dt;
            player.Velocity = speedCheck(player.Velocity + acceleration * dt);

            // calculate new phi and dphi
            player.Phi = player.Phi + player.DPhi * dt + 0.5 * angularAcc * dt * dt;
            player.DPhi = speedCheck(player.DPhi + angularAcc * dt, player.Radius);

            // calcualte heading from phi
            player.Heading.X = Math.Cos(player.Phi);
            player.Heading.Y = Math.Sin(player.Phi);
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
            if (player.Input.AltFire.IsPressed && (player.TurboCount < player.TurboLength) && (player.TurboNotCount > player.TurboWait))
            {
                player.InputTurbo = true;

                player.TurboCount++;

                // turbonotcount is reset
                if (player.TurboCount == player.TurboLength)
                {
                    player.TurboNotCount = 0;
                }
            }
            else
            {
                player.TurboNotCount++;

                // turbocount is reset
                if (player.TurboNotCount == player.TurboWait)
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

        private void drawScreenPlayer(CatchMePlayer p)
        {
            // draw the screen on the monitor of the player

            var g = p.Output.Graphics;
            var boostCol = new Color(1, 0, 0);

            // get shift and screenpostion
            screenPosition = p.Position;
            
            if (screenPosition.X < 150) { screenPosition.X = 150; }
            if (screenPosition.X > (xMax - 150)) { screenPosition.X = (xMax - 150); }
            if (screenPosition.Y < 90) { screenPosition.Y = 90; }
            if (screenPosition.Y > (yMax - 90)) { screenPosition.Y = (yMax - 90); }
            
            shift = new Vector(160, 100) - screenPosition;

            // draw each player
            foreach (var player in Players)
            {
                var pos = player.Position;
                var radius = player.Radius;
                var dir = player.Heading;
                var ortho = player.Heading.RotatedLeft;
                var col = player.LocalColor;
                
                // Name
                string name = player.Name.ToUpper();
                Vector namePosition = player.Position + new Vector(-name.Length / 2 * 3, -18);
                g.Print(col, 3, doShift(namePosition), name);

                // body
                g.Circle(col, doShift(pos), radius);

                // booster
                var pos1 = pos + (radius + 1) * ortho + dir;
                var pos2 = pos - (radius + 1) * ortho + dir;
                
                g.Line(col, doShift(pos1), doShift(pos1 - 3 * dir));
                g.Line(col, doShift(pos1) + ortho, doShift(pos1 + ortho - 3 * dir));
                g.Line(col, doShift(pos1), doShift(pos1 + ortho));
                g.Line(col, doShift(pos1 - 3 * dir), doShift(pos1 + ortho - 3 * dir));

                g.Line(col, doShift(pos2), doShift(pos2 - 3 * dir));
                g.Line(col, doShift(pos2 - ortho), doShift(pos2 - ortho - 3 * dir));
                g.Line(col, doShift(pos2), doShift(pos2 - ortho));
                g.Line(col, doShift(pos2 - 3 * dir), doShift(pos2 - ortho - 3 * dir));

                // indication of booster power
                Color barColor;
                int barNumber;

                if (player.InputTurbo)
                {
                    barColor = green;
                    barNumber = 6;

                    if (player.TurboCount < (player.TurboLength * 5.0 / 6))
                    {
                        barNumber = 5;

                        if (player.TurboCount < (player.TurboLength * 5.0 / 6))
                        {
                            barNumber = 4;
                            barColor = orange;

                            if (player.TurboCount < (player.TurboLength * 4.0 / 6))
                            {
                                barNumber = 3;

                                if (player.TurboCount < (player.TurboLength * 3.0 / 6))
                                {
                                    barNumber = 2;
                                    barColor = red;

                                    if (player.TurboCount < (player.TurboLength * 2.0 / 6))
                                    {
                                        barNumber = 1;
                                        barColor = red;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    barColor = red;
                    barNumber = 1;

                    if (player.TurboNotCount > (player.TurboWait * 1.0 / 3))
                    {
                        barColor = orange;
                        barNumber = 2;

                        if (player.TurboNotCount > player.TurboWait * 2.0 / 3)
                        {
                            barColor = green;
                            barNumber = 3;
                        }
                    }
                }

                // draw bars
                for (int i = 0; i < barNumber; i++)
                {
                    g.Line(barColor, doShift(namePosition - new Vector(-10, i)), doShift(namePosition - new Vector(-7, i)));
                }

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
                g.Point(flake.Color, doShift(flake.Position));
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

            // draw border of map
            g.Rectangle(white, doShift(new Vector(0, 0)), doShift(new Vector(xMax, yMax)));

            // draw random points of map
            foreach (Vector v in mapRandomPoints)
            {
                g.Point(white, doShift(v));
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

        private Vector randomPosition(Vector topLeft, Vector bottomRight)
        {
            // returns a random vector in the rectangle from topleft to bottomright
            double x = (bottomRight.X - topLeft.X) * Random.NextDouble();
            double y = (bottomRight.Y - topLeft.Y) * Random.NextDouble();

            return new Vector(x, y);
        }

        private Vector doShift(Vector input)
        {
            return input + shift;
        }

    }
}
