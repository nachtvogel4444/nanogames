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
        public int FrameCounter = 0;
        public int StartRound = 0;
        public BananaPlayer ActivePlayer;
        public int PartOfRound = 0;
        public List<object> bulletList = new List<object>();

        protected override void Initialize()
        {
            double offset = 20.0;
            int start = Convert.ToInt32(Random.NextDouble() * (320 - offset) + 0.5 * offset);

            ActivePlayer = Players[(int) System.Math.Floor(Random.NextDouble() * Players.Count)];

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

            if (PartOfRound == 0)
            {
                RotatePlayer(ActivePlayer);
                //ChangePowerBar(player);
                ShootGun(ActivePlayer, bulletList);

                foreach (var player in Players)
                {
                    player.DrawScreen();
                }
            }

            if (PartOfRound == 1)
            {
                if (StartRound == 0)
                {
                    StartRound = 1;

                }

                foreach (var player in Players)
                {
                    player.DrawScreen();
                }
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
                player.Angle += player.Angle;
            }

            if (player.Input.Right)
            {
                player.Angle -= player.Angle;
            }
            if (player.Angle < 0)
            {
                player.Angle = 0;
            }

            if (player.Angle > Math.PI)
            {
                player.Angle = Math.PI;
            }
        }

        private void ShootGun(BananaPlayer player)
        {
            /* Skip players that have already finished. */
            if (player.HasFinished)
            {
                return;
            }

            if ((player.Input.Fire) && (PartOfRound == 0))
            {
                PartOfRound = 1;
            }
        }
    }
}
