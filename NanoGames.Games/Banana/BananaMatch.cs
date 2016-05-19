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
                Players[i].Angle = 0.0;
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
                RotatePlayer(player);
                ShootGun(player, BulletList);
            }

            foreach (var player in Players)
            {
                player.DrawScreen();
            }

            FrameCounter++;
        }

        private void RotatePlayer(BananaPlayer player)
        {
            /* Skip players that have already finished. */
            if (player.HasFinished)
            {
                return;
            }

            if (player.Input.Left)
            {
                player.Angle -= Constants.StepAngle;
            }

            if (player.Input.Right)
            {
                player.Angle += Constants.StepAngle;
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
                    bulletList.Add(new Bullet());
                    bulletList[bulletList.Count - 1].X0 = player.Position.X;
                    bulletList[bulletList.Count - 1].Y0 = player.Position.Y;
                    bulletList[bulletList.Count - 1].Vx0 = Constants.SpeedBullet * Math.Cos(player.Angle);
                    bulletList[bulletList.Count - 1].Vy0 = Constants.SpeedBullet * Math.Sin(player.Angle);
                }

                else
                {
                    if (bulletList[bulletList.Count - 1].LifeTime > Constants.WaitBullet)
                        {
                            bulletList.Add(new Bullet());
                            bulletList[bulletList.Count - 1].X0 = player.Position.X;
                            bulletList[bulletList.Count - 1].Y0 = player.Position.Y;
                            bulletList[bulletList.Count - 1].Vx0 = Constants.SpeedBullet * Math.Cos(player.Angle);
                            bulletList[bulletList.Count - 1].Vy0 = Constants.SpeedBullet * Math.Sin(player.Angle);
                        }
                }
            }
        }
    }
}
