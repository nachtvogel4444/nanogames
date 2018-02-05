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
        private int frameCounterEnd;
        private int frameMax;
        private int huntersWaitTime;
        private CatchMePlayer prey;
        private List<CatchMePlayer> hunters = new List<CatchMePlayer> { };
        private Vector CatchPosisition;
        private List<Flake> flakes = new List<Flake> { };
        private List<Ring> ringsEnd = new List<Ring> { };
        private List<Ring> ringsBlast = new List<Ring> { };
        private List<FixedCircle> fixedCircles = new List<FixedCircle> { };

        private double xMap;
        private double yMap;
        private double xMiniMap;
        private double yMiniMap;

        private const int stepsPerFrame = 10;
        private const double minDistance = 1;
        private const double viscosity = 5;

        private Color white = new Color(1, 1, 1);
        private Color red = new Color(1, 0, 0);
        private Color orange = new Color(1, 0.647, 0);
        private Color green = new Color(0, 1, 0);
        private Color blue = new Color(0, 0, 1);
        private Color grey = new Color(0.3, 0.3, 0.3);

        private Vector screenPosition;
        private Vector shift;

        private List<Vector> mapRandomPoints =  new List<Vector> {};

        private bool preyIsSeen;
        private bool preyWasCatched = false;
        private Vector preyPosForHunters;
        private List<Vector> preyLastPosForHunters = new List<Vector> { };
        private Color preyColForHunters;
        private List<CatchMePlayer> sortedHunters;
        private CatchMePlayer catcher;

        FrameCounter tenSeconds = new FrameCounter(10);
        FrameCounter halfSecond = new FrameCounter(0.4);

        private double minDist = double.MaxValue;

        private CompareHunterDistance chd = new CompareHunterDistance();


        protected override void Initialize()
        {
            /* This is called by the framework at the start of every match. */

            // stateOfGame, frameCounter
            stateOfGame = "Start";
            frameCounter = 0;
            frameCounterEnd = 0;
            frameMax = 60 * 60 * 2;
            huntersWaitTime = 300;

            // map
            double aspectRatio = 2.5;
            double areaPerPlayer = 1.5 * 320 * 200;
            xMap = Math.Sqrt(aspectRatio * areaPerPlayer * Players.Count);
            yMap = Math.Sqrt(1 / aspectRatio * areaPerPlayer * Players.Count);
            xMiniMap = 50;
            yMiniMap = 50 * yMap / xMap;
            
            // time
            dt = 1.0 / 60 / stepsPerFrame;
            dT = 1.0 / 60;

            // max speed
            maxSpeed = minDistance / dt;

            // map

            // fixed circles
            double dMin = 10;
            double rMin = 8;
            double rMax = 60;
            double numberOfCircles = 0.02 * Math.Sqrt(areaPerPlayer * Players.Count);
            while (fixedCircles.Count <  numberOfCircles)
            {
                // get random circle
                double r = (rMax - rMin) * Random.NextDouble() + rMin;
                double x = (xMap - 2 * (dMin + r)) * Random.NextDouble() + r + dMin;
                double y = (yMap - 2 * (dMin + r)) * Random.NextDouble() + r + dMin;
                Vector pos = new Vector(x, y);

                // add random circle if standing free
                if (isOutsideCircles(pos, r + dMin))
                {
                    fixedCircles.Add(new FixedCircle(pos, r));
                }
            }

            // fill map with random points
            for (int i = 0; i < (0.001 * areaPerPlayer * Players.Count); i++)
            {
                Vector pos = randomPosition(new Vector(0, 0), new Vector(xMap, yMap));

                bool isInCircle = false;
                foreach (FixedCircle fixedCircle in fixedCircles)
                {
                    if (fixedCircle.Radius >= (fixedCircle.Position - pos).Length)
                    {
                        isInCircle = true;
                    }
                }

                if (!isInCircle)
                {
                    mapRandomPoints.Add(pos);
                }
            }

            // players

            // prey and hunter
            prey = Players[(int)(Players.Count * Random.NextDouble())];
            foreach (var player in Players)
            {
                if (player != prey)
                {
                    hunters.Add(player);
                }
            }

            // initialize each player
            for (int i = 0; i < Players.Count; i++)
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

                //Addons
                player.AddonJump = false;
                player.Addon2 = false;
                player.AddonPush = false;
                player.AddonMarker = 0;

                // position
                Vector pos = new Vector(0, 0); 
                bool notDone = true;
                while (notDone)
                {
                    // get new position of player
                    double x = (xMap - 3 * player.Radius) * Random.NextDouble() + 1.5 * player.Radius;
                    double y = (yMap - 3 * player.Radius) * Random.NextDouble() + 1.5 * player.Radius;

                    pos = new Vector(x, y);

                    // check if postion is in a circle
                    if (isOutsideCircles(pos, 3* player.Radius))
                    {
                        notDone = false;
                    }                    
                }

                player.Position = pos;
                // player.Position = new Vector(160, 100);

                // Last postion
                for (int j = 0; j < 30; j++)
                {
                    player.LastPositions.Add(pos);
                    preyLastPosForHunters.Add(prey.Position);
                }

                // phi, heading
                player.Phi = 2 * Math.PI * Random.NextDouble();
                player.Heading.X = Math.Cos(player.Phi);
                player.Heading.Y = Math.Sin(player.Phi);
                
                // mass and inertia
                player.Mass = 1.0 / 27 * player.Radius * player.Radius * player.Radius;
                player.Inertia = 1.0 / 9 * player.Radius * player.Radius;

                // sound
                player.Sound = 0;
            }

            // prey stuff            
            preyPosForHunters = prey.Position;
            preyColForHunters = prey.Color;
            prey.NumberOfBlast = Players.Count / 2;
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

                        // add flakes to every booster of every player
                        addFlakes();
                        updateFlakes();

                        // update last postion
                        if (halfSecond.Tock())
                        {
                            prey.LastPositions.Add(prey.Position);
                            prey.LastPositions.RemoveAt(0);
                        }

                        if (frameCounter > huntersWaitTime)
                        {
                            stateOfGame = "Game";
                        }

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
                            foreach (CatchMePlayer player in hunters)
                            {
                                // get dist of player to prey
                                double dist = (prey.Position - player.Position).Length;

                                if (dist < 6)
                                {
                                    // get postion of catch
                                    CatchPosisition = (prey.Position + player.Position) / 2;

                                    // prey was catched
                                    preyWasCatched = true;
                                    catcher = player;
                                        
                                    // end game
                                    stateOfGame = "End";
                                }
                            }

                        }

                        // add flakes to every booster of every player
                        addFlakes();
                        updateFlakes();

                        // update last postion
                        if (halfSecond.Tock())
                        {
                            foreach (var player in Players)
                            {
                                player.LastPositions.Add(player.Position);
                                player.LastPositions.RemoveAt(0);
                            }
                        }

                        // update Preyposition and color for hunters every 10 sec
                        if (preyIsSeen || tenSeconds.Tock())
                        {
                            preyPosForHunters = prey.Position;
                            preyColForHunters = prey.Color;
                            preyLastPosForHunters = new List<Vector>(prey.LastPositions);
                        }
                        else
                        {
                            preyColForHunters = prey.Color * (1 - tenSeconds.RatioExpGrowth());
                            preyLastPosForHunters.Add(preyPosForHunters);
                            preyLastPosForHunters.RemoveAt(0);
                        }

                        if ((frameCounter - huntersWaitTime) > frameMax)
                        {
                            stateOfGame = "End";
                            CatchPosisition = prey.Position;
                        }


                        break;
                    }

                // end state
                // show end animation
                case "End":
                    {
                        // do all the stuff which is only needed to do once in end
                        if (frameCounterEnd == 0)
                        {

                            // sort hunters
                            sortedHunters = hunters;
                            sortedHunters.Sort(chd);

                            // give points to players
                            if (preyWasCatched)
                            {
                                for (int i = 0; i < sortedHunters.Count; i++)
                                {
                                    sortedHunters[i].Score = 1000 - i;
                                }

                                // player who catches prey is always first
                                catcher.Score = 2000;

                                // prey is always last if catched
                                prey.Score = 0;
                            }
                            else
                            {
                                // prey is always first if it survived
                                prey.Score = 2000;

                                for (int i = 0; i < sortedHunters.Count; i++)
                                {
                                    sortedHunters[i].Score = 1000 - i;
                                }
                            }

                        }

                        // make an explosion with rings
                        Vector pos = CatchPosisition;
                        Color col = new Color(1, 1, 1);
                        
                        if ((frameCounterEnd % 60) == 0)
                        {
                            ringsEnd.Add(new Ring(pos, blue));
                        }

                        // show order of hunters
                        for (int i = 0; i < sortedHunters.Count; i++)
                        {
                            string s = (i+1) + ". " + sortedHunters[i].Name;
                            Output.Graphics.Print(white, 4, new Vector(210, 100 + i * 5), s);

                            if (preyWasCatched)
                            {
                                Output.Graphics.Print(white, 5, new Vector(210, 94), "RABBIT WAS CATCHED");
                            }
                            else
                            {
                                Output.Graphics.Print(white, 5, new Vector(210, 94), "RABBIT HAS WON");
                            }
                        }

                        // end game if x seconds passed
                        if (frameCounterEnd > 600)
                        {
                            IsCompleted = true;
                        }
                        
                        frameCounterEnd++;

                        break;
                    }
            }
            
            // sounds
            if (0==1) //((frameCounter % 3) == 0)
            {
                foreach (var player in Players)
                {
                    makeSounds(player);
                }
            }

            // update stuff
            updateScreenPosition();
            updatePreyIsSeen();
            updateIntegratedDistance();
            updateRings();
            tenSeconds.Tick();
            halfSecond.Tick();

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
                force -= 20 * player.Velocity;
                torque += 30 * player.Radius * player.Velocity.Y / Math.Abs(player.Velocity.Y);
                player.Sound = 2;
            }

            // player hits right border
            if (player.Position.X > (xMap - player.Radius))
            {
                force -= 20 * player.Velocity;
                force -= 1000 * (100 * Math.Atan(player.Position.X - (xMap - player.Radius)) / Math.PI + 0.5) * new Vector(1, 0);
                torque -= 30 * player.Radius * player.Velocity.Y / Math.Abs(player.Velocity.Y);
                player.Sound = 2;
            }

            // player hits top border
            if (player.Position.Y < (player.Radius))
            {
                force += 1000 * (100 * Math.Atan((player.Radius) - player.Position.Y)  / Math.PI + 0.5) * new Vector(0, 1);
                force -= 20 * player.Velocity;
                torque -= 30 * player.Radius * player.Velocity.X / Math.Abs(player.Velocity.X);
                player.Sound = 2;
            }

            // player hits bottom border
            if (player.Position.Y > (yMap - player.Radius))
            {
                force -= 20 * player.Velocity;
                force -= 1000 * (100 * Math.Atan(player.Position.Y - (yMap - player.Radius)) / Math.PI + 0.5) * new Vector(0, 1);
                torque += 30 * player.Radius * player.Velocity.X / Math.Abs(player.Velocity.X);
                player.Sound = 2;
            }

            // check if player hits fixed circle
            foreach (FixedCircle fixedCircle in fixedCircles)
            {
                if ((player.Position - fixedCircle.Position).Length < (player.Radius + fixedCircle.Radius))
                {
                    force += 1000 * (100 * Math.Atan((player.Position - fixedCircle.Position).Length - (player.Radius + fixedCircle.Radius)) / Math.PI + 0.5) * (-player.Position + fixedCircle.Position).Normalized;
                    force -= 20 * player.Velocity;
                    player.Sound = 2;
                }

            }

            // add "standgas"
            if (force.Length == 0)
            {
                force = 4 * player.Velocity.Normalized;

            }

            // blast
            if (prey.InputBlast && player != prey)
            {
                if ((player.Position - prey.Position).Length <= 60)
                {
                    force += (player.Position - prey.Position).Normalized * 8000;
                }
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

            // marker for addon
            if (player.Input.AltFire.WasActivated)
            {
                player.AddonMarker++;

                if (player.AddonMarker > 2) { player.AddonMarker = 0; };
            }

            // set everything related to movement
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
                player.Sound = 1;
            }

            /*
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
            */

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

            //  blast of prey
            if (player == prey)
            {
                if (player.Input.Fire.WasActivated && player.NumberOfBlast > 0)
                {
                    player.InputBlast = true;
                    player.NumberOfBlast--;
                    ringsBlast.Add(new Ring(prey.Position, blue));
                    ringsBlast.Add(new Ring(prey.Position, blue, 3));
                }
                else
                {
                    player.InputBlast = false;
                }
            }
        }

        private void drawScreenPlayer(CatchMePlayer player)
        {
            // draw the screen on the monitor of the player

            var g = player.Output.Graphics;
            var boostCol = new Color(1, 0, 0);
            string s;
            Color colAddon;
            Vector posAddon;

            // get shift            
            shift = new Vector(160, 100) - player.ScreenPosition;

            if (stateOfGame == "End" && player != prey)
            {
                Vector shiftToGo = new Vector(160, 100) - prey.ScreenPosition;
                Vector dir = (shiftToGo - shift).Normalized;
                double dist = (shiftToGo - shift).Length;

                shift = shift + (1 - Math.Exp(-10.0 * frameCounterEnd / 600)) * (shiftToGo - shift);
            }

            // info
            if (player == prey)
            {
                // generall instruction
                s = "YOU ARE THE RABBIT - RUN FROM EVERYONE";
                g.Print(blue, 5, new Vector(160 - 2.5 * s.Length, 2.5), s);

                // info
                if (stateOfGame == "Start")
                {
                    s = "ONLY YOU CAN MOVE, RUN " + ((huntersWaitTime - frameCounter) / 60).ToString(); ;
                    g.Print(blue, 5, new Vector(160 - 2.5 * s.Length, 12.5), s);
                }
                if (stateOfGame == "Game")
                {
                    s = "TIME TO SURVIVE: " + ((frameMax - (frameCounter - huntersWaitTime)) / 60).ToString();
                    g.Print(blue, 5, new Vector(160 - 2.5 * s.Length, 12.5), s);
                }
                if (stateOfGame == "End")
                {
                    if (preyWasCatched)
                    {
                        s = "YOU LOST, YOU WERE CATCHED BY " + catcher.Name.ToUpper();
                        g.Print(blue, 5, new Vector(160 - 2.5 * s.Length, 12.5), s);
                    }
                    else
                    {
                        s = "YOU WON, NOBODY WAS FAST ENOUGH";
                        g.Print(blue, 5, new Vector(160 - 2.5 * s.Length, 12.5), s);
                    }
                }
            }
            else
            {
                // generall instruction
                s = "YOU ARE A FOX - CATCH THE RABBIT " + prey.Name.ToUpper();
                g.Print(blue, 5, new Vector(160 - 2.5 * s.Length, 2.5), s);

                // info
                if (stateOfGame == "Start")
                {
                    s = "BUT WAIT, ONLY RABBIT CAN MOVE NOW " + ((huntersWaitTime - frameCounter) / 60).ToString(); ;
                    g.Print(blue, 5, new Vector(160 - 2.5 * s.Length, 12.5), s);
                }
                if (stateOfGame == "Game")
                {
                    s = "GO GO GO, TIME LEFT: " + ((frameMax - (frameCounter - huntersWaitTime)) / 60).ToString();
                    g.Print(blue, 5, new Vector(160 - 2.5 * s.Length, 12.5), s);
                }
                if (stateOfGame == "End")
                {
                    if (preyWasCatched)
                    {
                        s = "YOU WON, RABBIT WAS CATCHED BY " + catcher.Name.ToUpper();
                        g.Print(blue, 5, new Vector(160 - 2.5 * s.Length, 12.5), s);
                    }
                    else
                    {
                        s = "YOU LOST, RABBIT ESCAPED ";
                        g.Print(blue, 5, new Vector(160 - 2.5 * s.Length, 12.5), s);
                    }
                }
            }

            // draw addon info

            // draw addon jump
            if (player.AddonJump) { colAddon = red; }
            else { colAddon = grey; }
            posAddon = new Vector(320 - 19 - xMiniMap, 200 - 23 - yMiniMap);
            g.Line(colAddon, posAddon, posAddon + new Vector(3, 0));
            g.Line(colAddon, posAddon, posAddon + new Vector(1.5, -3));
            g.Line(colAddon, posAddon + new Vector(3, 0), posAddon + new Vector(1.5, -3));

            // draw addon 2
            if (player.Addon2) { colAddon = blue; }
            else { colAddon = grey; }
            posAddon = new Vector(320 - 13 - xMiniMap, 200 - 26 - yMiniMap);
            g.Rectangle(colAddon, posAddon, posAddon + new Vector(3, 3));
            
            // draw addon push
            if (player.AddonPush) { colAddon = green; }
            else { colAddon = grey; }
            posAddon = new Vector(320 - 5.5 - xMiniMap, 200 - 24.5 - yMiniMap);
            g.Circle(colAddon, posAddon, 1.5);

            // draw addon marker
            colAddon = grey;
            posAddon = new Vector(320 - 20 - xMiniMap + player.AddonMarker * 6, 200 - 27 - yMiniMap);
            g.Rectangle(colAddon, posAddon, posAddon + new Vector(5, 5));
           
            // draw each player
            foreach (var otherPlayer in Players)
            {
                var pos = otherPlayer.Position;
                var radius = otherPlayer.Radius;
                var dir = otherPlayer.Heading;
                var ortho = otherPlayer.Heading.RotatedLeft;
                var col = otherPlayer.LocalColor;
                var lastPositions = otherPlayer.LastPositions;

                // Name
                if (otherPlayer != player)
                {
                    s = otherPlayer.Name.ToUpper();
                    g.Print(col, 3, doShift(otherPlayer.Position + new Vector(-s.Length / 2 * 3, -18)), s);
                }

                // body
                g.Circle(col, doShift(pos), radius);

                // point in minimap
                if (otherPlayer != prey)
                {
                    //g.Circle(col, convertToMiniMap(pos), 0.5);

                    for (int i = 0; i < lastPositions.Count - 1; i++)
                    {
                        g.Point(0.4 * i / lastPositions.Count * col, convertToMiniMap(lastPositions[i]));
                    }
                }

                // add lines if player is prey
                if (otherPlayer == prey)
                {
                    // drawScreenPlayer on map
                    g.Line(col, doShift(pos + new Vector(0, 3 * radius)), doShift(pos + new Vector(0, 5 * radius)));
                    g.Line(col, doShift(pos - new Vector(0, 3 * radius)), doShift(pos - new Vector(0, 5 * radius)));
                    g.Line(col, doShift(pos + new Vector(3 * radius, 0)), doShift(pos + new Vector(5 * radius, 0)));
                    g.Line(col, doShift(pos - new Vector(3 * radius, 0)), doShift(pos - new Vector(5 * radius, 0)));
                    g.Circle(col, doShift(pos), 0.7 * radius);
                    g.Circle(col, doShift(pos), 0.4 * radius);
                    
                    // draw prey on minimap for hunters 
                    if (player != prey)
                    {
                        g.Line(preyColForHunters, convertToMiniMap(preyPosForHunters) + new Vector(0, 1), convertToMiniMap(preyPosForHunters) + new Vector(0, 2.5));
                        g.Line(preyColForHunters, convertToMiniMap(preyPosForHunters) - new Vector(0, 1), convertToMiniMap(preyPosForHunters) - new Vector(0, 2.5));
                        g.Line(preyColForHunters, convertToMiniMap(preyPosForHunters) + new Vector(1, 0), convertToMiniMap(preyPosForHunters) + new Vector(2.5, 0));
                        g.Line(preyColForHunters, convertToMiniMap(preyPosForHunters) - new Vector(1, 0), convertToMiniMap(preyPosForHunters) - new Vector(2.5, 0));

                        g.Circle(preyColForHunters, convertToMiniMap(preyPosForHunters), 0.5);

                        for (int i = 0; i < preyLastPosForHunters.Count - 1; i++)
                        {
                            g.Point(0.4 * i / preyLastPosForHunters.Count * preyColForHunters, convertToMiniMap(preyLastPosForHunters[i]));
                        }
                    }

                    // draw prey on minimap for prey plus blast info
                    if (player == prey)
                    {
                        g.Circle(col, convertToMiniMap(pos), 0.5);

                        for (int i = 0; i < lastPositions.Count - 1; i++)
                        {
                            g.Point(0.4 * i / lastPositions.Count * col, convertToMiniMap(lastPositions[i]));
                        }
                        
                        string tmp = "BLAST: " + prey.NumberOfBlast.ToString().ToUpper();
                        g.Print(grey, 3, new Vector(320 - 40 - tmp.Length, 200 - 24 - yMiniMap), tmp);
                    }
                }

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

            }

            // draw output from boosters
            
            // draw all flakes
            foreach (Flake flake in flakes)
            {
                g.Point(flake.Color, doShift(flake.Position));
            }

            // draw all rings
            foreach (Ring ring in ringsEnd)
            {
                g.Circle(ring.Color, doShift(ring.Position), ring.Time * 0.7);
                g.Circle(ring.Color, convertToMiniMap(ring.Position), convertToMiniMap(ring.Time * 0.7));
            }

            foreach (Ring ring in ringsBlast)
            {
                g.Circle(ring.Color, doShift(ring.Position), ring.Time * 2.0);
                g.Circle(ring.Color, convertToMiniMap(ring.Position), convertToMiniMap(ring.Time * 2.0));
            }

            // draw border of map
            g.Rectangle(white, doShift(new Vector(0, 0)), doShift(new Vector(xMap, yMap)));

            // draw fixed circles
            foreach (FixedCircle fixedCircle in fixedCircles)
            {
                // draw on map
                g.Circle(white, doShift(fixedCircle.Position), fixedCircle.Radius);

                // draw on minimap
                g.Circle(grey, convertToMiniMap(fixedCircle.Position), convertToMiniMap(fixedCircle.Radius));
            }

            // draw random points of map
            foreach (Vector v in mapRandomPoints)
            {
                g.Point(white, doShift(v));
            }

            // draw minimap
            g.Rectangle(grey, convertToMiniMap(new Vector(0, 0)), convertToMiniMap(new Vector(xMap, yMap)));

            // draw mindist
            foreach (var hunter in hunters)
            {
                double dist = (hunter.Position - prey.Position).Length;

                if (dist < minDist)
                {
                    minDist = dist;
                }
                
            }
            string tmp2 = "MIN " + (minDist - 5.9).ToString("F1").ToUpper();
            g.Print(grey, 3, new Vector(320 - 20 - xMiniMap, 200 - 33 - yMiniMap), tmp2);

            // draw speed
            tmp2 = "SPEED " + (player.Velocity.Length * 1.709).ToString("F0").ToUpper();
            g.Print(grey, 3, new Vector(320 - 20 - xMiniMap, 200 - 37 - yMiniMap), tmp2);


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

        private bool isOutsideCircles(Vector pos, double distance)
        {
            bool isOut = true;

            foreach (FixedCircle fixedCircle in fixedCircles)
            {
                if (((pos - fixedCircle.Position).Length - distance) < fixedCircle.Radius)
                {
                    isOut = false;
                    return isOut;
                }
            }

            return isOut;
        }

        private Vector convertToMiniMap(Vector old)
        {
            double x = (320 - 20 - xMiniMap) + old.X * xMiniMap / xMap;
            double y = (200 - 20 - yMiniMap) + old.Y * yMiniMap / yMap;

            return new Vector(x, y);
        }

        private double convertToMiniMap(double old)
        {
            return old * xMiniMap / xMap;
        }

        private void updateScreenPosition()
        {
            foreach (var player in Players)
            {
                screenPosition = player.Position;

                if (screenPosition.X < 150) { screenPosition.X = 150; }
                if (screenPosition.X > (xMap - 150)) { screenPosition.X = (xMap - 150); }
                if (screenPosition.Y < 90) { screenPosition.Y = 90; }
                if (screenPosition.Y > (yMap - 90)) { screenPosition.Y = (yMap - 90); }

                player.ScreenPosition = screenPosition;
            }

        }

        private void updatePreyIsSeen()
        {
            preyIsSeen = false;

            foreach (var player in hunters)
            {
                if (Math.Abs(prey.Position.X - player.ScreenPosition.X) < 160 && Math.Abs(prey.Position.Y - player.ScreenPosition.Y) < 100)
                {
                    preyIsSeen = true;
                }
            }

        }

        private void updateIntegratedDistance()
        {
            foreach (CatchMePlayer hunter in hunters)
            {
                double dist = (hunter.Position - prey.Position).Length;
                hunter.IntegratedDistance += dist;
            }
        }

        private void addFlakes()
        {
            foreach (CatchMePlayer player in Players)
            {
                // add flakes
                double alpha;
                double delta;
                Vector direction;

                var pos = player.Position;
                var radius = player.Radius;
                var dir = player.Heading;
                var ortho = player.Heading.RotatedLeft;
                var col = player.LocalColor;

                var pos1 = pos + (radius + 1) * ortho + dir;
                var pos2 = pos - (radius + 1) * ortho + dir;

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
        }

        private void updateFlakes()
        {
            if (stateOfGame != "End")
            {
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
                List<Flake> tmpFlake = new List<Flake> { };
                for (int i = 0; i < flakes.Count; i++)
                {
                    if (flakes[i].Time < 20)
                    {
                        tmpFlake.Add(flakes[i]);
                    }
                }

                flakes = tmpFlake;
            }
        }

        private void updateRings()
        {
            // ringsEnd

            // set time and fade out 
            foreach (Ring ring in ringsEnd)
            {
                // fade
                ring.Color = ring.Color * 0.995;

                // time
                ring.Time++;
            }

            // remove old rings
            if (ringsEnd.Count > 10)
            {
                ringsEnd.RemoveAt(0);
            }

            // ringsblast

            // remove old rings
            List<Ring> tmp = new List<Ring> { }; 
            foreach (Ring ring in ringsBlast)
            {
                if (ring.Time < 10)
                {
                    tmp.Add(ring);
                }
            }
            ringsBlast = tmp;

            foreach (Ring ring in ringsBlast)
            {
                // fade
                ring.Color = ring.Color * 0.99;

                // time
                ring.Time++;
            }
        }

        private void makeSounds(CatchMePlayer player)
        {

            if (player.Sound == 1)
            {
                player.Output.Audio.Play(Sounds.LowNoise);
                player.Sound = 0;
            }

            if (player.Sound == 2)
            {
                player.Output.Audio.Play(Sounds.LowBeep);
                player.Sound = 0;
            }
        }
    }
}
