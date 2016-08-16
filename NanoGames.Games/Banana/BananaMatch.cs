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
    /// bei gedrückter taste schneller werte ändern
    mappool oder random maps
    laufen, jump fly in player klasse
    checkcollions aus simple bullet in methods
 
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
        public string StateOfGame = "Game";
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
                Players[i].IdxPosition = 100;
                Players[i].Position = new Vector(Land.XTracksUpper[Players[i].IdxPosition], Land.YTracksUpper[Players[i].IdxPosition]);
                Players[i].Angle = startAngle;
                Players[i].StepMove = (int)Constants.VelocityPlayer;
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
                case "Game":
                    // Active player can do stuff
                    //SetSpeedBullet(ActivePlayer);
                    MovePlayer(ActivePlayer);
                    JumpPlayer(ActivePlayer);
                    ShootGun(ActivePlayer);
                break;

                case "AnimationJump":
                    // player is jumping / flying
                    FlyPlayer(ActivePlayer);
                    ActivePlayer.CheckCollisionLandscape();
                break;

                case "AnimationShoot":
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
                            for(int i = 0; i < Land.NPointsInterpolated; i++)
                            {
                                if (bullet.CheckCollision(new Vector(Land.XInterpolated[i], Land.YInterpolated[i]), 0.6 * Constants.Dx))
                                {
                                    switch (Land.TypeInterpolated[i])
                                    {
                                        case "normal":
                                            bullet.State = "exploded";

                                        break;
                                    }
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
                         StateOfGame = "Game";
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

        /*
        private void RotatePlayer(BananaPlayer player)
        {
            if (player.Input.Left.WasActivated)
            {
               if (CountInputLeftRight < Constants.WaitTimeKey)
                {
                    player.Angle += Constants.StepAngle;
                }

               else
                {
                    player.Angle += Constants.StepAngle * Constants.MultiplierAngle;
                }
            }

            if (player.Input.Right.WasActivated)
            {
                if (CountInputLeftRight < Constants.WaitTimeKey)
                {
                    player.Angle -= Constants.StepAngle;
                }

                else
                {
                    player.Angle -= Constants.StepAngle * Constants.MultiplierAngle;
                }
            }

            if (player.Input.Left.IsPressed || player.Input.Right.IsPressed)
            {
                CountInputLeftRight++;
            }

            if (!player.Input.Left.IsPressed && !player.Input.Right.IsPressed)
            {
                CountInputLeftRight = 0;
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
        */

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
        
        private void MovePlayer(BananaPlayer player)
        {
            if (player.Input.Left.WasActivated)
            {
                if (CountInputLeftRight < Constants.WaitTimeKey)
                {
                    player.IdxPosition -= player.StepMove;
                    player.Position = new Vector(Land.XTracksUpper[player.IdxPosition], Land.YTracksUpper[player.IdxPosition]);
                    player.Direction = -1;
                }

                else
                {
                    player.IdxPosition -= 2 * player.StepMove;
                    player.Position = new Vector(Land.XTracksUpper[player.IdxPosition], Land.YTracksUpper[player.IdxPosition]);
                    player.Direction = -1;
                }
            }

            if (player.Input.Right.WasActivated)
            {
                if (CountInputLeftRight < Constants.WaitTimeKey)
                {
                    player.IdxPosition += player.StepMove;
                    player.Position = new Vector(Land.XTracksUpper[player.IdxPosition], Land.YTracksUpper[player.IdxPosition]);
                    player.Direction = 1;
                }

                else
                {
                    player.IdxPosition += 2 * player.StepMove;
                    player.Position = new Vector(Land.XTracksUpper[player.IdxPosition], Land.YTracksUpper[player.IdxPosition]);
                    player.Direction = 1;
                }
            }

            if (player.Input.Left.IsPressed || player.Input.Right.IsPressed)
            {
                CountInputLeftRight++;
            }

            if (!player.Input.Left.IsPressed && !player.Input.Right.IsPressed)
            {
                CountInputLeftRight = 0;
            }
        }

        private void JumpPlayer(BananaPlayer player)
        {
            if (player.Input.Up.WasActivated)
            {
                double angleLandscape = Land.Alpha[player.IdxPosition];
                     
                if (player.Input.Left.WasActivated || player.Input.Left.IsPressed)
                {
                    Vector n = new Vector(Math.Cos(angleLandscape + Math.PI / 4), -Math.Sin(angleLandscape + Math.PI / 4));
                    player.VelocityStartJump = Constants.JumpVelocity *n;
                    player.PositionStartJump = player.Position + 1.1 * Constants.Dx * n;
                }

                else if (player.Input.Right.WasActivated || player.Input.Right.IsPressed)
                {
                    Vector n = new Vector(Math.Cos(angleLandscape - Math.PI / 4), -Math.Sin(angleLandscape - Math.PI / 4));
                    player.VelocityStartJump = Constants.JumpVelocity * n;
                    player.PositionStartJump = player.Position + 1.1 * Constants.Dx * n;
                }

                else
                {
                    Vector n = new Vector(Math.Cos(angleLandscape), -Math.Sin(angleLandscape));
                    player.VelocityStartJump = Constants.JumpVelocity *n;
                    player.PositionStartJump = player.Position + 1.1 * Constants.Dx * n;
                }

                player.LifeTimeJump = 1;
                StateOfGame = "AnimationJump";
            }
        }

        private void FlyPlayer(BananaPlayer player)
        {
            player.Position = player.PositionStartJump + player.LifeTimeJump * player.VelocityStartJump + 0.5 * player.LifeTimeJump * player.LifeTimeJump * player.Acceleration;
            player.Velocity = player.VelocityStartJump + player.LifeTimeJump * player.Acceleration;
            player.PositionTail = player.Position - player.Velocity;
            player.LifeTimeJump++;
        }

        private void ShootGun(BananaPlayer player)
        {
            if (player.Input.Fire.WasActivated || player.Input.Fire.IsPressed)
            {
                BulletList.Add(new SimpleBullet(new Vector(player.Position.X, player.Position.Y), player.Angle, player.SpeedBullet));
                StateOfGame = "AnimationShoot";                                  
            }
        }
    }
}
