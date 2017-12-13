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
        private string tmp;
        private List<BotsPlayer> PlayersAlive;
        private int currentScore;

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

                Players[i].Direction = new Vector(0, -1);
                Bots[i].Aiming = new Vector(0, 1);
            }

            // initialzie stuff
            Bullets = new List<Bullet> { };
            white = new Color(1, 1, 1);
            PlayersAlive = new List<BotsPlayer> { };
            foreach (var player in Players)
            {
                PlayersAlive.Add(player);
            }
            currentScore = 0;
        }

        protected override void Update()
        {
            // Move players
            foreach (var player in PlayersAlive)
            {
                Vector newPosition; 

                if (player.Input.Left.WasActivated)
                {
                    player.Direction = player.Direction.Rotated(-player.AngleSize);
                }
                if (player.Input.Right.WasActivated)
                {
                    player.Direction = player.Direction.Rotated(player.AngleSize);
                }
                if (player.Input.Up.WasActivated)
                {
                    newPosition = player.Position + player.StepSize * player.Direction;

                    if (newPosition.X > 0 && newPosition.X < 320 &&
                        newPosition.Y > 0 && newPosition.Y < 200)
                    {
                        player.Position = newPosition;
                    }
                }
                if (player.Input.Down.WasActivated)
                {
                    newPosition = player.Position - player.StepSize * player.Direction;

                    if (newPosition.X > 0 && newPosition.X < 320 &&
                        newPosition.Y > 0 && newPosition.Y < 200)
                    {
                        player.Position = newPosition;
                    }
                }
            }

            // Move bots

            // Move Bullets, check for collision
            foreach (var bullet in Bullets)
            {
                bullet.Position += bullet.Velocity;
                
                foreach (var player in PlayersAlive)
                {
                    if ((bullet.Position - player.Position).Length <= player.Radius)
                    {
                        player.HealthPoints -= bullet.Damage;
                        bullet.IsExploded = true;
                    }

                }

                if (bullet.Position.X < 0 || bullet.Position.X > 320 ||
                    bullet.Position.Y < 0 || bullet.Position.Y > 200)
                {
                    bullet.IsExploded = true;
                }
            }

            // players shot
            foreach (var player in PlayersAlive)
            {
                if (player.Input.Fire.WasActivated && player.GunCounter.AfterFirstEvent(Frame))
                {
                    player.GunCounter.Reset(Frame);
                    Bullets.Add(new Bullet(player.GunTip, player.GunSpeed * player.Direction));
                }
            }

            // remove dead players from game
            var newList = new List<BotsPlayer> { };
            foreach (var player in PlayersAlive)
            {
                if (player.HealthPoints > 0)
                {
                    newList.Add(player);
                }
                else
                {
                    player.Score = currentScore;
                    currentScore++;
                }
            }
            PlayersAlive = newList;

            // remove exploded bullets from game
            var newList2 = new List<Bullet> { };
            foreach (Bullet bullet in Bullets)
            {
                if (!bullet.IsExploded)
                {
                    newList2.Add(bullet);
                }
            }
            Bullets = newList2;

            // Draw all players
            foreach (var player in PlayersAlive)
            {
                Output.Graphics.Circle(player.LocalColor, player.Position, player.Radius);
                Output.Graphics.Line(player.LocalColor, player.Position, player.Position + player.Direction * player.GunLength);

                tmp = player.HealthPoints + "P";
                Output.Graphics.Print(player.LocalColor, 4, player.Position + new Vector(-0.5 * tmp.Length * 4, -10), tmp);
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
                Output.Graphics.Circle(white, bullet.Position, bullet.Radius);
            }

            // draw info
            tmp = (int)(Frame / 60.0) + " SEC";
            Output.Graphics.Print(white, 4, new Vector(160 - 0.5 * tmp.Length * 4, 5), tmp);

        }
        
    }
}
