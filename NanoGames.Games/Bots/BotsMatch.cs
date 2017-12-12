// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Bots
{
    internal class BotsMatch : Match<BotsPlayer>
    {
        private List<Bot> Bots;
        private List<Bullet> Bullets;
        private Color white;

        protected override void Initialize()
        {
            // add bots
            Bots = new List<Bot> { };
            for (int i = 0; i < Players.Count; i++)
            {
                Bots.Add(new Bot());
            }

            // set positions, aiming of players and bots
            for (int i = 0; i < Players.Count; i++)
            {
                double x = 40 + (i + 1) * 240 / (Players.Count + 1);
                Players[i].Position = new Vector(x, 180);
                Bots[i].Position = new Vector(x, 20);

                Players[i].Aiming = new Vector(0, -1);
                Bots[i].Aiming = new Vector(0, 1);
            }

            // initialzie stuff
            Bullets = new List<Bullet> { };
            white = new Color(1, 1, 1);  
        }

        protected override void Update()
        {
            // Move players
            foreach (var player in Players)
            {
                if (player.Input.Up.WasActivated)
                {
                    player.Position.Y -= player.Stepsize;
                }
                if (player.Input.Down.WasActivated)
                {
                    player.Position.Y += player.Stepsize;
                }
                if (player.Input.Left.WasActivated)
                {
                    player.Position.X -= player.Stepsize;
                }
                if (player.Input.Right.WasActivated)
                {
                    player.Position.X += player.Stepsize;
                }
            }

            // Move bots

            // Move Bullets
            foreach (var bullet in Bullets)
            {
                bullet.Position += bullet.Velocity;
            }

            // players shot
            foreach (var player in Players)
            {
                if (player.Input.Fire.WasActivated)
                {
                    Bullets.Add(new Bullet(player.GunTip, player.GunSpeed * player.Aiming));
                }
            }


            // Draw all players
            foreach (var player in Players)
            {
                Output.Graphics.Circle(player.LocalColor, player.Position, player.Radius);
                Output.Graphics.Line(player.LocalColor, player.Position, player.Position + player.Aiming * player.GunLength);
            }

            // Draw all bots
            foreach (var bots in Bots)
            {
                Output.Graphics.Circle(bots.Color, bots.Position, bots.Radius);
                Output.Graphics.Line(bots.Color, bots.Position, bots.Position + bots.Aiming * bots.GunLength);
            }

            // Draw all bullets
            foreach (var bullet in Bullets)
            {
                Output.Graphics.Point(white, bullet.Position);
            }

        }
        
    }
}
