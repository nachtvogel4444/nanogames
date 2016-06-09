// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    class BananaMatch : Match<BananaPlayer>
    {
        public List<Bullet> BulletList = new List<Bullet>();
        public int FrameCounter = 0;

        protected override void Initialize()
        {
            double offset = 20.0;
            int start = Convert.ToInt32(Random.NextDouble() * (320 - offset) + 0.5 * offset);

            for (int i = 0; i < Players.Count; ++i)
            {
                Players[i].Position = new Vector(start, 100);
                Players[i].Angle = 0;
                Players[i].SpeedBullet = Constants.SpeedBullet;
            }
        }

        protected override void Update()
        {
            int y = 100 + Constants.RadiusPlayer;
            Graphics.Line(new Color(1, 1, 1), new Vector(0, y), new Vector(320, y));

            foreach (Bullet bullet in BulletList)
            {
                bullet.MoveBullet();
            }

            foreach (var player in Players)
            {
                MovePlayer(player);
                RotatePlayer(player);
                ShootGun(player, BulletList);
            }

            foreach (var player in Players)
            {
                player.DrawScreen();
            }

            FrameCounter++;
        }

        private void MovePlayer(BananaPlayer player)
        {
            /* Skip players that have already finished. */
            if (player.HasFinished)
            {
                return;
            }

            if (player.Input.Left)
            {
                player.Position -= new Vector(Constants.StepPlayer, 0);
            }

            if (player.Input.Right)
            {
                player.Position += new Vector(Constants.StepPlayer, 0);
            }
        }

        private void RotatePlayer(BananaPlayer player)
        {
            /* Skip players that have already finished. */
            if (player.HasFinished)
            {
                return;
            }

            if (player.Angle < 0)
            {
                player.Angle = 2 * Math.PI + player.Angle;
            }

            if (player.Angle > 2 * Math.PI)
            {
                player.Angle = player.Angle - 2 * Math.PI;
            }
            
            if (player.Input.AltFire)
            {
                player.GunIsRight = !player.GunIsRight;
                player.Angle = Math.PI - player.Angle;
            }

            if (player.Input.Up)
            {
                if (((player.Angle >= 0) && (player.Angle <= 0.5 * Math.PI)) || ((player.Angle >= 1.5 * Math.PI) && (player.Angle <= 2 * Math.PI)))
                {
                    player.Angle += Constants.StepAngle;
                }

                if (player.Angle > 0.5 * Math.PI && player.Angle < 1.5 * Math.PI)
                {
                    player.Angle -= Constants.StepAngle;
                }
            }

            if (player.Input.Down)
            {
                if (((player.Angle >= 0) && (player.Angle <= 0.5 * Math.PI)) || ((player.Angle >= 1.5 * Math.PI) && (player.Angle <= 2 * Math.PI)))
                {
                    player.Angle -= Constants.StepAngle;
                }

                if (player.Angle > 0.5 * Math.PI && player.Angle < 1.5 * Math.PI)
                {
                    player.Angle += Constants.StepAngle;
                }
                
            }
        }

        private void ShootGun(BananaPlayer player, List<Bullet> bulletList)
        {
            /* Skip players that have already finished. */
            if (player.HasFinished)
            {
                return;
            }

            if (player.Input.Fire)
            {
                if (bulletList.Count == 0)
                {
                    bulletList.Add(new Bullet(player.Position, player.Angle, player.SpeedBullet));
                }

                else
                {
                    if (bulletList[bulletList.Count - 1].LifeTime > Constants.WaitToNextBullet)
                        {
                            bulletList.Add(new Bullet(player.Position, player.Angle, player.SpeedBullet));
                        }
                }
            }
        }
    }
}
