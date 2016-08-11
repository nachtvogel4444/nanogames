// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
ToDo:

    Anzeige: near miss, wer dran ist plus reihenfolge
    healthpoints
    verschiedene bullets, flächenschaden, granaten mehrere projektile
    player platzierung
    verschiedene untergründe: reflecktierend, doppelte, halbe geschwindigkeit 
    verschiedene modi: suddenddeath
    töne: letzte 5sec, treffer, eingabe
    bei gedrückter taste schneller werte ändern
    mappool oder random maps
    laufen
 
*/

namespace NanoGames.Games.Banana
{
    class BananaMatch : Match<BananaPlayer>
    {
        public int FrameCounter = 0;
        public int FrameCounterRound = 0;
        public int SecToGoInRound = 0;
        public int RoundCounter = 0;
        public string StateOfGame = "game";
        public BananaPlayer ActivePlayer;
        public int ActivePlayerIdx = 0;
        public List<SimpleBullet> BulletList = new List<SimpleBullet>();
        /*
        public double[] LandX = new double[6] { 0, 100, 200, 220, 300, 320 };
        public double[] LandY = new double[6] { 100, 60, 60, 120, 110, 100};
        public string[] LandType = new string[6] { "exploded", "exploded", "exploded", "exploded", "exploded", "exploded" };
        */
        public double[] LandX = new double[2] { 0, 320 };
        public double[] LandY = new double[2] { 100, 100 };
        public string[] LandType = new string[2] { "exploded", "exploded" };
        public Landscape Land;

        private int finishedPlayers = 0;

        protected override void Initialize()
        {
            Land = new Landscape(LandX, LandY, LandType);

            ActivePlayerIdx = Convert.ToInt32(Math.Floor(Random.NextDouble() * Players.Count));
            ActivePlayer = Players[ActivePlayerIdx];

            for (int i = 0; i < Players.Count; ++i)
            {
                int startX = Convert.ToInt32(Random.NextDouble() * (320 - Constants.offset) + 0.5 * Constants.offset);
                double startAngle = Random.NextDouble() * 2 * Math.PI;
                // double startX = 100;
                // double startAngle = 85 * Math.PI / 180;
                Players[i].Position = new Vector(startX, 100);
                Players[i].Angle = startAngle;
            }
        }

        protected override void Update()
        {
            SecToGoInRound = Convert.ToInt32((Constants.RoundTime - FrameCounterRound) / 60);

            if (FrameCounterRound == Constants.RoundTime)
            {
                ActivePlayerIdx++;
                ActivePlayerIdx = ActivePlayerIdx % Players.Count;
                ActivePlayer = Players[ActivePlayerIdx];
                FrameCounterRound = 0;
            }

            switch (StateOfGame)
            {
                case "game":
                    // Active player can do stuff
                    SetSpeedBullet(ActivePlayer);
                    RotatePlayer(ActivePlayer);
                    ShootGun(ActivePlayer);
                break;

                case "animation":
                    // shoot animation is over -> next player
                    if (BulletList.Count != 0)
                    {
                        List<SimpleBullet> BulletsToKeep = new List<SimpleBullet>();
                        foreach (SimpleBullet bullet in BulletList)
                        {
                            // Move bullet
                            bullet.MoveSimpleBullet();
                            
                            // Check for collsion with players
                            foreach (var player in Players)
                            {
                                if (bullet.CheckCollision(player.Position, Constants.RadiusPlayer))
                                {
                                    bullet.State = "exploded";
                                    player.HasFinished = true;
                                    finishedPlayers++;
                                }
                            }   

                            // Check for collisions with landscape
                            for(int i = 0; i < Land.NPoints; i++)
                            {
                                if (bullet.CheckCollision(new Vector(Land.X[i], Land.Y[i]), 0.6 * Constants.Dx))
                                {
                                    bullet.State = Land.Type[i];
                                }
                            }

                            // Check if bullet is outside screen
                            if (bullet.Position.X < 0 || bullet.Position.X > 320 || bullet.Position.Y > 200)
                            {
                                bullet.State = "exploded";
                            }

                            // move bullet to new list if exploded
                            if (!String.Equals(bullet.State, "exploded"))
                            {
                                BulletsToKeep.Add(bullet);
                            }
                        }

                        BulletList = BulletsToKeep;
                     }

                     else
                     {
                         StateOfGame = "game";
                         ActivePlayerIdx++;
                         ActivePlayerIdx = ActivePlayerIdx % Players.Count;
                         ActivePlayer = Players[ActivePlayerIdx];
                         FrameCounterRound = 0;
                     }
                    
                      break;
            }
            
            foreach (var player in Players)
            {
                player.DrawScreen();
            }

            if (Players.Count == 1)
            {
                /* Practice mode. */
                if (finishedPlayers == 1)
                {
                    IsCompleted = true;
                }
            }
            else
            {
                /* Tournament mode. The match ends when the second-to-last player has reached the goal. */
                if (finishedPlayers >= Players.Count - 1)
                {
                    IsCompleted = true;
                }
            }

            FrameCounter++;
            FrameCounterRound++;
        }

        private void RotatePlayer(BananaPlayer player)
        {
            if (player.Input.Left.WasActivated)
            {
               player.Angle += Constants.StepAngle;
            }

            if (player.Input.Right.WasActivated)
            {
                player.Angle -= Constants.StepAngle;
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

        private void SetSpeedBullet(BananaPlayer player)
        {
            if (player.Input.Up.WasActivated)
            {
                player.SpeedBullet += Constants.StepSpeedBullet;
            }

            if (player.Input.Down.WasActivated)
            {
                player.SpeedBullet -= Constants.StepSpeedBullet;
            }

            if (player.SpeedBullet < 0)
            {
                player.SpeedBullet = 0;
            }

            if (player.SpeedBullet > 10)
            {
                player.SpeedBullet = 10;
            }

        }

        /*
        private void MovePlayer(BananaPlayer player)
        {
            if (player.Input.Left.IsPressed || player.Input.Left.WasActivated)
            {
                player.Position.X -= Constants.StepPlayer;
                player.Direction = 0;
            }

            if (player.Input.Right.IsPressed || player.Input.Right.WasActivated)
            {
                player.Position.X += Constants.StepPlayer;
                player.Direction = 1;
            }
        }
        */

        private void ShootGun(BananaPlayer player)
        {
            if (player.Input.Fire.WasActivated || player.Input.Fire.IsPressed)
            {
                BulletList.Add(new SimpleBullet(new Vector(player.Position.X, player.Position.Y), player.Angle, player.SpeedBullet));
                StateOfGame = "animation";                                  
            }
        }
    }
}
