// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;


namespace NanoGames.Games.Infinity
{
    internal class InfinityMatch : Match<InfinityPlayer>
    {
        private bool debug = true;

        private Vector screen = new Vector(320, 200);
        private double tileSize = 500;
        private int nospt = 250;

        private double dt = 1.0 / 60;
        private int frameCounter = 0;
        private MyColors myColor = new MyColors();
        
        private double viscosity = 5;
        private double maxSpeed = 1000;
        
        private List<Tile> Tiles = new List<Tile> { };

        private int countUp;
        private int countDown;
        private int wait = 20;


        protected override void Initialize()
        {
            /* This is called by the framework at the start of every match. */

            // game stuff
            countDown = wait;
            countUp = wait;
            if (debug) { screen = new Vector(64, 40); }

            // Position
            foreach (InfinityPlayer player in Players)
            {
                player.Position = 0.5 * new Vector(tileSize, tileSize);
            }
            
            // Heading and Velocity
            foreach (InfinityPlayer player in Players)
            {
                player.HeadingPhi = Random.NextDouble() * 2 * Math.PI;
                player.Heading = new Vector(Math.Sin(player.HeadingPhi), Math.Cos(player.HeadingPhi));
                player.Velocity = 1 * Random.NextDouble() * player.Heading;
            }

            // tiles

            var newTile = new Tile(new Vector(0, 0), tileSize);
            newTile.AddStars(Random, nospt, Players[0]);
            Tiles.Add(newTile);
            foreach (InfinityPlayer player in Players)
            {
                player.updateXY(tileSize);
                player.TileIndex = Tiles.FindIndex(x => x.X == player.X && x.Y == player.Y);
                Tiles[player.TileIndex].Players.Add(player);
            }
            exploreNewTiles();
        }

        protected override void Update()
        {
            /*
             * This is called by the framework once every frame.
             * We have to draw draw onto every player's Graphics interface.
             */

            // Move players/objects
            // Fire players/objects
            // Collisions
            // New tiles
            // Magnification players
            // Draw stuff
            
            // Move players
            foreach (InfinityPlayer player in Players)
            {
                movePlayer(player);
            }

            // Move objects

            // Fire player

            // Fire objects

            // Collisions

            // Magnification players
            foreach (InfinityPlayer player in Players)
            {
                magnificationPlayer(player);
            }

            // New Tiles
            exploreNewTiles(); // Create new tiles if necessary

            // update what can be seen in tiles
            foreach (InfinityPlayer player in Players)
            {
                var tile = Tiles[player.TileIndex];

                tile.DiscoverStars(player);
                tile.DiscoverPlanets(player);
            }
            
            // Draw world for each observer
            foreach (InfinityPlayer observer in Players)
            {
                // Everything is stored in tiles. Only the Tiles which can be seen by the observer are drawn
                foreach (Tile tile in Tiles)
                {
                    drawEverythingInTile(tile, observer);
                }

                
                // screen debug
                if (debug)
                {
                    var dx = new Vector(0.5 * (320 - screen.X), 0.5 * (200 - screen.Y));
                    observer.Output.Graphics.Line(myColor.White, dx, new Vector(320 - dx.X, dx.Y));
                    observer.Output.Graphics.Line(myColor.White, new Vector(320 - dx.X, dx.Y), new Vector(320 - dx.X, 200 - dx.Y));
                    observer.Output.Graphics.Line(myColor.White, new Vector(320 - dx.X, 200 - dx.Y), new Vector(dx.X, 200 - dx.Y));
                    observer.Output.Graphics.Line(myColor.White, new Vector(dx.X, 200 - dx.Y), new Vector(dx.X, dx.Y));
                    dx = dx - new Vector(tileSize, tileSize) / observer.ZoomOut;
                    observer.Output.Graphics.Line(myColor.Red, dx, new Vector(320 - dx.X, dx.Y));
                    observer.Output.Graphics.Line(myColor.Red, new Vector(320 - dx.X, dx.Y), new Vector(320 - dx.X, 200 - dx.Y));
                    observer.Output.Graphics.Line(myColor.Red, new Vector(320 - dx.X, 200 - dx.Y), new Vector(dx.X, 200 - dx.Y));
                    observer.Output.Graphics.Line(myColor.Red, new Vector(dx.X, 200 - dx.Y), new Vector(dx.X, dx.Y));
                }
            }

            // draw observerinfo
            foreach (InfinityPlayer observer in Players)
            {
                drawObserverInfo(observer);
            }

            // stuff
            frameCounter++;
        }

        private void movePlayer(InfinityPlayer player)
        {
            Vector force = new Vector(0, 0);
            double torque = 0;

            // check input

            // player moves forward
            if (player.Input.Left.IsPressed && player.Input.Right.IsPressed)
            {
                force += player.BoosterPower * player.Heading;
            }
            // players moves right
            if (player.Input.Right.IsPressed && !player.Input.Left.IsPressed)
            {
                force += 0.5 * player.BoosterPower * player.Heading;
                torque -= 10;
            }

            // player moves left
            if (player.Input.Left.IsPressed && !player.Input.Right.IsPressed)
            {
                force += 0.5 * player.BoosterPower * player.Heading;
                torque += 10;
            }

            // players circles right
            if (player.Input.Right.IsPressed && !player.Input.Left.IsPressed && player.Input.Fire.IsPressed)
            {
                torque += 10;
            }

            // player cirlces left
            if (player.Input.Left.IsPressed && !player.Input.Right.IsPressed && player.Input.Fire.IsPressed)
            {
                torque -= 10;
            }

            // viscosity
            force -= 0.05 * player.Size * viscosity * player.Velocity;
            torque -= 0.04 * player.Size * viscosity * player.Size * player.Rotation;
            
            // calculate acceleration and angular acceleration from forces and torque
            Vector acceleration = force / player.Mass;
            double angularAcc = torque / player.Mass;

            // calculate new position and velocity from acceleration
            Vector pos = player.Position + player.Velocity * dt + 0.5 * acceleration * dt * dt;
            Vector vel = player.Velocity + acceleration * dt;

            // check for collsisions (old)
            
            Tiles[player.TileIndex].Players.Remove(player);
            player.Position = pos;
            player.updateXY(tileSize);
            player.TileIndex = Tiles.FindIndex(x => x.X == player.X && x.Y == player.Y);
            Tiles[player.TileIndex].Players.Add(player);

            player.Velocity = vel;
            player.Velocity = Math.Min(maxSpeed, player.Velocity.Length) * player.Velocity.Normalized;

            // calculate new phi and dphi
            player.HeadingPhi += player.Rotation * dt + 0.5 * angularAcc * dt * dt;
            player.Rotation += angularAcc * dt;

            // calcualte heading from phi
            player.Heading = new Vector(Math.Sin(player.HeadingPhi), Math.Cos(player.HeadingPhi));
        }

        private void magnificationPlayer(InfinityPlayer player)
        {
            int wait = 20;
            countUp++;
            countDown++;

            if (player.Input.Up.WasActivated && countUp > wait)
            {
                player.ZoomOut /= 1.5;
                player.ZoomOut = Math.Max(player.ZoomOut, 0.04);
                countUp = 0;
            }

            if (player.Input.Down.WasActivated && countDown > wait)
            {
                player.ZoomOut *= 1.5;
                player.ZoomOut = Math.Min(player.ZoomOut, 100);
                countDown = 0;
            }
        }

        private void drawObserverInfo(InfinityPlayer observer)
        {
            var g = observer.Output.Graphics;
            var c = myColor.White;
            var p = new Vector(5, 150);

            g.Print(c, 3, p, "POS " + observer.Position.X + " " + observer.Position.Y);
            g.Print(c, 3, p + new Vector(0, 5), "SPEED " + observer.Velocity.Length);
            g.Print(c, 3, p + new Vector(0, 10), "ZOOMOUT " + observer.ZoomOut);
            g.Print(c, 3, p + new Vector(0, 15), "TILES " + Tiles.Count);
            int tmp1 = 0;
            int tmp2 = 0;
            int tmp3 = 0;
            foreach (var tile in Tiles)
            {
                tmp3 += tile.Planets.Count;
                foreach (Planet planet in tile.Planets)
                {
                    if (planet.Alphas.Count > 0) { tmp1++; }
                    if (planet.IsFullyDiscovered) { tmp2++; }
                }
            }
            g.Print(c, 3, p + new Vector(0, 20), "PLANETS " + tmp1 + "/" + tmp2 + "/" + tmp3);
        }

        private void drawPlayer(InfinityPlayer player, InfinityPlayer observer)
        {
            var g = observer.Output.Graphics;
            var c = player.LocalColor;
            var p = player.Position - observer.Position;
            var a = player.HeadingPhi;
            var s = player.Size;
            var m = 1 / observer.ZoomOut;

            if (observer.ZoomOut < 10)
            {
                // ship contour
                foreach (Segment seg in player.Contour)
                {
                    var td = seg.Scaled(s).Rotated(a).Translated(p).Scaled(m).ToOrigin();
                    g.Line(c, td.Start, td.End);
                }
            }
            if (observer.ZoomOut >= 10)
            {
                // triangle
                foreach (Segment seg in player.Triangle)
                {
                    var td = seg.Scaled(1.0 / m).Rotated(a).Translated(p).Scaled(m).ToOrigin();
                    g.Line(c, td.Start, td.End);
                }
            }

            if (debug)
            {
                g.Circle(player.LocalColor, p.Scaled(m).ToOrigin(), player.View * m);
            }
        }
        
        private void drawStars(Tile tile, InfinityPlayer observer)
        {
            if (observer.ZoomOut < 10)
            {
                var g = observer.Output.Graphics;
                var p = tile.Position - observer.Position;
                var m = 1 / observer.ZoomOut;

                foreach (Star star in tile.Stars)
                {
                    if (!debug)
                    {
                        if (star.IsDiscovered)
                        { /*
                            var td = star.Position.Translate(p).Scaled(m).ToOrigin();
                            var c = m * myColor.White;

                            g.Point(c, td);
                            */
                        }
                    }

                    else
                    {
                        if (star.IsDiscovered)
                        {/*
                            var td = star.Position.Translate(p).Scaled(m).ToOrigin();
                            var c = myColor.White;

                            g.Point(c, td);
                            */
                        }
                        else
                        { /*
                            var td = star.Position.Translate(p).Scaled(m).ToOrigin();
                            var c = myColor.Green;

                            g.Point(c, td);
                            */
                        }
                    }
                }
            }
        }

        private void drawPlanets(Tile tile, InfinityPlayer observer)
        {
            var g = observer.Output.Graphics;
            var p = tile.Position - observer.Position;
            var m = 1 / observer.ZoomOut;

            foreach (Planet planet in tile.Planets)
            {
                /*
                // flo
                var td = planet.Position.Translate(p).Scaled(m).ToOrigin();
                var c = myColor.White;
                var r =  m * planet.Radius;

                foreach (Interval alpha in planet.Alphas)
                {
                    g.CircleSegment(c, td, r, alpha.A, alpha.B);
                }
                */
            }
        }

        private void drawEverythingInTile(Tile tile, InfinityPlayer observer)
        {
            var g = observer.Output.Graphics;

            // Check if tile should be drawn
            double intensity;

            if (tile.Position + 0.5 * new Vector(tileSize, tileSize) - observer.Position > 1.5 * new Vector(tileSize, tileSize) + 0.5 * observer.ZoomOut * screen)
            {
                if (!debug) { return; }   
            }
            
            // Stars
            drawStars(tile, observer);

            // Planets
            drawPlanets(tile, observer);
            
            if (debug)
            {
                var r = myColor.Blue;
                var m = 1 / observer.ZoomOut;
                var p = -observer.Position + tile.Position;
                g.Point(r, new Vector(0, 0).Translated(p).Scaled(m).ToOrigin());
                g.Point(r, new Vector(tileSize, 0).Translated(p).Scaled(m).ToOrigin());
                g.Point(r, new Vector(tileSize, tileSize).Translated(p).Scaled(m).ToOrigin());
                g.Point(r, new Vector(0, tileSize).Translated(p).Scaled(m).ToOrigin());
            }

            // Players
            foreach (InfinityPlayer player in tile.Players)
            {
                drawPlayer(player, observer);
            }

            // draw debug stuff 
            if (false)
            {
                var osp = observer.Position - new Vector(160, 100);
                g.Line(intensity * myColor.Red, tile.Position - osp, tile.Position + new Vector(tileSize, 0) - osp);
                g.Line(intensity * myColor.Red, tile.Position + new Vector(tileSize, 0) - osp, tile.Position + new Vector(tileSize, tileSize) - osp);
                g.Line(intensity * myColor.Red, tile.Position + new Vector(tileSize, tileSize) - osp, tile.Position + new Vector(0, tileSize) - osp);
                g.Line(intensity * myColor.Red, tile.Position + new Vector(0, tileSize) - osp, tile.Position - osp);
                var mystr = "TILE (" + tile.X + " " + tile.Y + ")";
                g.Print(intensity * myColor.White, 3, tile.Position + 0.5 * new Vector(tileSize, tileSize) - osp, mystr);
            }
            
        }
        
        private void exploreNewTiles()
        {
            foreach (InfinityPlayer player in Players)
            {
                var px = player.X;
                var py = player.Y;
                var pn = Tiles[player.TileIndex].Neighbors;

                if (true)
                {
                    // there are too few neighbors, we must add the missing ones
                    for (int x = px - 1; x <= px + 1; x++)
                    {
                        for (int y = py - 1; y <= py + 1; y++)
                        {
                            bool exists = pn.Exists(t => t.X == x && t.Y == y);

                            if (!exists)
                            {
                                var newTile = new Tile(tileSize * new Vector(x, y), tileSize);
                                newTile.X = x;
                                newTile.Y = y;
                                newTile.AddNeighbors(Tiles);
                                newTile.AddStars(Random, nospt, player);
                                newTile.AddPlanets(Random, 0.2, 500);
                                newTile.AddPlanets(Random, 0.9, 100);
                                newTile.AddPlanets(Random, 0.5, 200);

                                Tiles.Add(newTile);
                            }
                        }
                    }
                }
            }
        }
        
    }
}
