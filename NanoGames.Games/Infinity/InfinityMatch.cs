// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;


namespace NanoGames.Games.Infinity
{
    internal class InfinityMatch : Match<InfinityPlayer>
    {
        private bool debug = false;

        private int[,] elems2 = new int[16, 2] {
                    { -2, -2 },
                    { -1, -2 },
                    { 0, -2 },
                    { 1, -2 },
                    { 2, -2 },
                    { 2, -1 },
                    { 2, -0 },
                    { 2, 1 },
                    { 2, 2 },
                    { 1, 2 },
                    { 0, 2 },
                    { -1, 2 },
                    { -2, 2 },
                    { -2, 1 },
                    { -2, 0 },
                    { -2, -1 } };

        private Vector screen = new Vector(320, 200);

        private double dt = 1.0 / 60;
        private int frameCounter = 0;
        private double tileSize = 100;
        private MyColors myColor = new MyColors();
        
        private double viscosity = 5;
        private double maxSpeed = 100;

        private int numberOfStarsPerTile = 10;

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

            // Rotation
            foreach (InfinityPlayer player in Players)
            {
                player.Rotation = 0;
            }

            // Booster
            foreach (InfinityPlayer player in Players)
            {
                player.BoosterPower = 100;
            }

            // tiles
            int[,] elems = new int[9, 2] {{ 0, 0 }, { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 } };

            for (int i = 0; i < 9; i++)
            {
                var newTile = new Tile();
                newTile.X = elems[i, 0];
                newTile.Y = elems[i, 1];
                newTile.Position = tileSize * new Vector(newTile.X, newTile.Y);
                addStarsToTile(newTile, numberOfStarsPerTile);
                Tiles.Add(newTile);
            }
            exploreNewTiles();
            foreach (InfinityPlayer player in Players)
            {
                player.updateXY(tileSize);
                player.TileIndex = Tiles.FindIndex(x => x.X == player.X && x.Y == player.Y);
                Tiles[player.TileIndex].Players.Add(player);
            }
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
                    dx = dx - new Vector(tileSize, tileSize) / observer.Magnification;
                    observer.Output.Graphics.Line(myColor.Red, dx, new Vector(320 - dx.X, dx.Y));
                    observer.Output.Graphics.Line(myColor.Red, new Vector(320 - dx.X, dx.Y), new Vector(320 - dx.X, 200 - dx.Y));
                    observer.Output.Graphics.Line(myColor.Red, new Vector(320 - dx.X, 200 - dx.Y), new Vector(dx.X, 200 - dx.Y));
                    observer.Output.Graphics.Line(myColor.Red, new Vector(dx.X, 200 - dx.Y), new Vector(dx.X, dx.Y));
                }
                Output.Graphics.Print(myColor.White, 3, new Vector(2, 2), Tiles.Count + " TILES");
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
                player.Magnification /= 1.5;
                countUp = 0;
            }

            if (player.Input.Down.WasActivated && countDown > wait)
            {
                player.Magnification *= 1.5;
                countDown = 0;
            }
        }

        private void drawShip(InfinityPlayer player, InfinityPlayer observer)
        {
            var g = observer.Output.Graphics;
            var c = player.LocalColor;
            var p = player.Position - observer.Position;
            var a = player.HeadingPhi;
            var s = player.Size;
            var m = 1.0 / observer.Magnification;

            // ship contour
            foreach (Segment seg in player.Contour)
            {
                var td = seg.Scaled(s).Rotated(a).Translate(p).Scaled(m).ToOrigin();
                g.Line(c, td.Start, td.End);
            }
        }

        private double getIntensity(Vector obj, InfinityPlayer observer)
        {
            var d = (obj - observer.Position).Length;

            if (d < observer.ViewRadius) {return 1; }
            else { return 0.2; }
        }

        private void drawStars(Vector tilePos, List<Vector> stars, InfinityPlayer observer, double intensity)
        {
            var g = observer.Output.Graphics;
            var p = tilePos - observer.Position;
            var m = 1.0 / observer.Magnification;

            foreach (Vector star in stars)
            {
                var td = star.Translate(p).Scaled(m).ToOrigin();
                var c = intensity * myColor.White; // * getIntensity(tilePos + star, observer);

                g.Point(c, td);
            }
        }

        private void drawEverythingInTile(Tile tile, InfinityPlayer observer)
        {
            var g = observer.Output.Graphics;

            // Check if tile should be drawn
            double intensity;

            if (tile.Position + 0.5 * new Vector(tileSize, tileSize) - observer.Position > 1.5 * new Vector(tileSize, tileSize) + 0.5 * observer.Magnification * screen)
            {

                intensity = 0.2;
                if (!debug) { return; }   
            }
            else
            {
                intensity = 1;
            }

            // Stars
            drawStars(tile.Position, tile.Stars, observer, intensity);
            
            if (debug)
            {
                var r = myColor.Blue;
                var m = 1.0 / observer.Magnification;
                var p = -observer.Position + tile.Position;
                g.Point(r, new Vector(0, 0).Translate(p).Scaled(m).ToOrigin());
                g.Point(r, new Vector(tileSize, 0).Translate(p).Scaled(m).ToOrigin());
                g.Point(r, new Vector(tileSize, tileSize).Translate(p).Scaled(m).ToOrigin());
                g.Point(r, new Vector(0, tileSize).Translate(p).Scaled(m).ToOrigin());
            }

            // Players
            foreach (InfinityPlayer player in tile.Players)
            {
                drawShip(player, observer);
            }

            // draw debug stuff 
            if (false)
            {
                g.Line(intensity * myColor.Red, tile.Position - observer.ScreenPosition, tile.Position + new Vector(tileSize, 0) - observer.ScreenPosition);
                g.Line(intensity * myColor.Red, tile.Position + new Vector(tileSize, 0) - observer.ScreenPosition, tile.Position + new Vector(tileSize, tileSize) - observer.ScreenPosition);
                g.Line(intensity * myColor.Red, tile.Position + new Vector(tileSize, tileSize) - observer.ScreenPosition, tile.Position + new Vector(0, tileSize) - observer.ScreenPosition);
                g.Line(intensity * myColor.Red, tile.Position + new Vector(0, tileSize) - observer.ScreenPosition, tile.Position - observer.ScreenPosition);
                var mystr = "TILE (" + tile.X + " " + tile.Y + ")";
                g.Print(intensity * myColor.White, 3, tile.Position + 0.5 * new Vector(tileSize, tileSize) - observer.ScreenPosition, mystr);
            }
            
        }

        private void addStarsToTile(Tile tile, int n)
        {
            for (int i = 0; i < n; i++)
            {
                tile.Stars.Add(new Vector(Random.NextDouble() * tileSize, Random.NextDouble() * tileSize));
            }
        }

        private void exploreNewTiles()
        {
            foreach (InfinityPlayer player in Players)
            {
                int n = 5; // (int)Math.Ceiling(player.ViewRadius / tileSize);
                var px = player.X;
                var py = player.Y;
                bool needNewTile;

                for (int x = px - n; x < px + n; x++)
                {
                    for (int y = py - n; y < py + n; y++)
                    {
                        if ((px - x) * (px - x) + (py - y) * (py - y) < n * n)
                        {
                            needNewTile = true;

                            foreach (Tile tile in Tiles)
                            {
                                if (tile.X == x && tile.Y == y)
                                {
                                    needNewTile = false;
                                    break;
                                }
                            }

                            if (needNewTile)
                            {
                                Tile newTile = new Tile();
                                newTile.X = x;
                                newTile.Y = y;
                                newTile.Position = tileSize * new Vector(x, y);
                                addStarsToTile(newTile, numberOfStarsPerTile);

                                Tiles.Add(newTile);
                            }
                        }
                    }
                }
            }
        }
    }
}
