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
    mappool oder random maps
    checkcollions aus simple bullet in methods
    stop player when track ends or is too steep
    fix jump
 
*/

namespace NanoGames.Games.Banana
{
    class BananaMatch : Match<BananaPlayer>
    {
        public int FrameCounter = 0;
        public int FrameCounterRound = 0;
        public int CountInputLeftRight = 0;
        public int SecToGoInRound = 0;
        public int RoundCounter = 0;
        public string StateOfGame = "ActivePlayerMoving";
        public BananaPlayer ActivePlayer;
        public int ActivePlayerIdx = 0;
        public List<SimpleBullet> BulletList = new List<SimpleBullet>();
        public Landscape Land = new Landscape();

        private int finishedPlayers = 0;

        protected override void Initialize()
        {
            Land.createLandscape(Land.lineX, Land.lineY, Land.lineType);

            ActivePlayerIdx = Convert.ToInt32(Math.Floor(Random.NextDouble() * Players.Count));
            ActivePlayer = Players[ActivePlayerIdx];

            for (int i = 0; i < Players.Count; ++i)
            {
                double startAngle = 85 * Math.PI / 180;
                Players[i].IdxPosition = 1000;
                Players[i].Position = new Vector(Land.XTracksUpper[Players[i].IdxPosition], Land.YTracksUpper[Players[i].IdxPosition]);
                Players[i].Angle = startAngle;
                Players[i].Acceleration = new Vector(0, Constants.Gravity);
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
                case "ActivePlayerMoving":

                    ActivePlayer.Move();
                    ActivePlayer.Jump();
                    ActivePlayer.SelectAction();
                    break;

                case "AnimationJump":

                    ActivePlayer.Fly();
                    ActivePlayer.CheckCollisionLandscape();
                    break;

                case "ActivePlayerAiming":

                    ActivePlayer.Rotate();
                    ActivePlayer.SetSpeedBullet();
                    ActivePlayer.ShootGun();
                    ActivePlayer.SelectAction();
                    break;
                    
                case "AnimationShoot":

                    if (BulletList.Count != 0)
                    {
                        List<SimpleBullet> BulletsToKeep = new List<SimpleBullet>();
                        foreach (SimpleBullet bullet in BulletList)
                        {
                            bullet.MoveSimpleBullet();
                            
                            foreach (var player in Players)
                            {
                                if (bullet.CheckCollision(player.Position, Constants.RadiusPlayer))
                                {
                                    bullet.State = "Exploded";
                                    player.HasFinished = true;
                                    finishedPlayers++;
                                }
                            }   
                            
                            for(int i = 0; i < Land.NPointsInterpolated; i++)
                            {
                                if (bullet.CheckCollision(new Vector(Land.XInterpolated[i], Land.YInterpolated[i]), 0.6 * Constants.Dx))
                                {
                                    switch (Land.TypeInterpolated[i])
                                    {
                                        case "Normal":
                                            bullet.State = "Exploded";
                                            break;
                                    }
                                }
                            }
                            
                            if (bullet.Position.X < 0 || bullet.Position.X > 320 || bullet.Position.Y > 200)
                            {
                                bullet.State = "Exploded";
                            }
                            
                            if (!String.Equals(bullet.State, "Exploded"))
                            {
                                BulletsToKeep.Add(bullet);
                            }
                        }

                        BulletList = BulletsToKeep;
                     }

                     else
                     {
                         StateOfGame = "ActivePlayerMoving";
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
    }
}
