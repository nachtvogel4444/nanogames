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
    simple gun to bazooka
    grenade
 
*/

namespace NanoGames.Games.Banana
{
    class BananaMatch : Match<BananaPlayer>
    {
        public int FramesLeft = 0;
        private int framesMax = 1800;
        public string StateOfGame = "ActivePlayerActing";
        public BananaPlayer ActivePlayer;
        public int StartPlayerIdx = 0;
        private int activePlayerIdx = 0;
        public Landscape Land = new Landscape();
        public Bullet Bullet = new Bullet();
        public Grenade Grenade = new Grenade();
        public Wind Wind = new Wind();
        public AudioSettings MatchAudioSettings = new AudioSettings();

        private int finishedPlayers = 0;

        protected override void Initialize()
        {
            Land.BuildLandscape("blocks");

            foreach (var player in Players)
            {
                player.GetBorn();
                player.Name = "TEST";
            }

            StartPlayerIdx = Convert.ToInt32(Math.Floor(Random.NextDouble() * Players.Count));
            activePlayerIdx = StartPlayerIdx;
            ActivePlayer = Players[activePlayerIdx];

            Wind.SetSpeed(Random);

            FramesLeft = framesMax;
        }

        protected override void Update()
        {

            switch (StateOfGame)
            {
                case "NextPlayer":

                    do
                    {
                        activePlayerIdx++;
                        activePlayerIdx = activePlayerIdx % Players.Count;
                        ActivePlayer = Players[activePlayerIdx];

                    } while (ActivePlayer.HasFinished);

                    FramesLeft = framesMax;
                    Wind.SetSpeed(Random);
                    StateOfGame = "ActivePlayerActing";     // StateOfGame -> ActivePlayerActing
                    break;
                    
                case "ActivePlayerActing":

                    ActivePlayer.Move();
                    ActivePlayer.SetAngle();
                    ActivePlayer.SetWeapon();
                    ActivePlayer.Shoot1();                  // StateOfGame -> Shooting2
                    break;

                case "ActivePlayerShoot2":
                    ActivePlayer.Shoot2();                  // StateOfGame -> Shooting3
                    break;

                case "ActivePlayerShoot3":
                    ActivePlayer.Shoot3();                  // StateOfGame -> SomethingFlying, ...
                    break;

                case "SomethingFlying":
                    
                    // player falling/flying
                    bool somethingFlying = false;

                    foreach (var player in Players)
                    {
                        if (player.IsFalling)
                        {
                            player.Fall();
                            CheckCollisionPlayerLand(player);
                            CheckCollisionPlayerScreen(player);
                            somethingFlying = true;
                        }
                    }

                    // bullet flying
                    if (!Bullet.IsExploded)
                    {
                        Bullet.MoveBullet(Wind);
                        CheckCollisionBulletLand();                  
                        CheckCollisionBulletPlayers();              
                        CheckCollisionBulletScreen();                
                        somethingFlying = true;
                    }

                    // grenade flying
                    if (!Grenade.IsExploded && !Grenade.IsDead)
                    {
                        Grenade.MoveGrenade();
                        CheckCollisionGrenadeLand();
                        CheckCollisionGrenadeScreen();
                        CheckGrenadeExplosion();
                        somethingFlying = true;
                    }

                    if (!somethingFlying)
                    {
                        StateOfGame = "NextPlayer";            // StateOfGame -> NextPlayer
                    }

                    break;

            }

            if ((FramesLeft == 300) ||
                (FramesLeft == 240) ||
                (FramesLeft == 180) ||
                (FramesLeft == 120))
            {
                MatchAudioSettings.TimerFiveSecondsToGo = true;
            }

            if (FramesLeft == 60)
            {
                MatchAudioSettings.TimerOneSecondToGo = true;
            }

            FramesLeft--;
            
            if (FramesLeft <= 0)
            {
                StateOfGame = "NextPlayer";
            }

            foreach (var player in Players)
            {
                if (player.Health <= 0)
                {
                    player.HasFinished = true;
                    finishedPlayers++;
                }
            }

            foreach (var player in Players)
            {
                player.DrawScreen();
                player.PlayAudio();
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
        }
        
        private void CheckCollisionBulletPlayers()
        {
            foreach (var player in Players)
            {
                for (int i = 0; i < player.Hitbox.Length - 2; i++)
                {
                    Intersection intersection = new Intersection(Bullet.Position, Bullet.PositionBefore, player.Hitbox[i], player.Hitbox[i + 1]);

                    if (intersection.IsTrue)
                    {
                        MatchAudioSettings.BulletExploded = true;
                        Bullet.IsExploded = true;
                        Land.makeCaldera(intersection.Point, 3);
                        player.Health -= 50;

                        foreach (var playerB in Players)
                        {
                            CheckIfPlayerIsFalling(playerB);
                        }

                        return;
                    }
                }
            }              
        }

        private void CheckCollisionBulletLand()
        {
            foreach (List<Vector> block in Land.Border)
            {
                for (int i = 0; i < block.Count - 1; i++)
                {
                    Intersection intersection = new Intersection(Bullet.Position, Bullet.PositionBefore, block[i], block[i + 1]);

                    if (intersection.IsTrue)
                    {
                        MatchAudioSettings.BulletExploded = true;
                        Bullet.IsExploded = true;
                        Land.makeCaldera(intersection.Point, 3);

                        foreach (var player in Players)
                        {
                            CheckIfPlayerIsFalling(player);
                        }
                        
                        return;
                    }
                }
            }


        }
        
        private void CheckCollisionGrenadeLand()
        {
            for (int i = 0; i < Land.Border.Count - 1; i++)
            {
                var block = Land.Border[i];
                 
                for (int j = 0; j < block.Count - 1; j++)
                {
                    Vector p1 = block[j];
                    Vector p2 = block[j + 1];

                    Intersection intersection = new Intersection(Grenade.Position, Grenade.PositionBefore, p1, p2);

                    if (intersection.IsTrue)
                    {
                        var n = new Vector();
                        if ((p1 - intersection.Point).Length < (p2 - intersection.Point).Length)
                        {
                            n = Land.Normal[i][j];
                        }
                        else
                        {
                            n = Land.Normal[i][j + 1];
                        }

                        Grenade.Bounce(intersection.Point, n);

                        return;
                    }
                }
            }


        }

        private void CheckCollisionBulletScreen()
        {
            if (Bullet.Position.X < 0 || Bullet.Position.X > 320 || Bullet.Position.Y > 200)
            {
                Bullet.IsExploded = true;
            }
            
        }

        private void CheckCollisionGrenadeScreen()
        {
            if (Grenade.Position.X < 0 || Grenade.Position.X > 320 || Grenade.Position.Y > 200)
            {
                Grenade.IsDead = true;
            }

        }

        private void CheckGrenadeExplosion()
        {
            if (Grenade.IsExploded)
            {
                Land.makeCaldera(Grenade.Position, 6);
                MatchAudioSettings.GrenadeExploded = true;

                foreach (var player in Players)
                {
                    double damage = 0;
                    double dist = (player.Position - Grenade.Position).Length;

                    if (dist < 5)
                    {
                        damage = 50;
                    } 
                    if ((dist >= 5) && (dist < 15))
                    {
                        damage = -5 * dist + 75;
                    }

                    player.Health -= damage;

                    CheckIfPlayerIsFalling(player);
                    
                }
            }
        }

        private void CheckCollisionPlayerLand(BananaPlayer player)
        {
            for (int i = 0; i < Land.Border.Count; i++)
            {
                var block = Land.Border[i];

                for (int j = 0; j < block.Count - 1; j++)
                {
                    Intersection intersection = new Intersection(player.Position, player.PositionBefore, block[j], block[j + 1]);

                    if (intersection.IsTrue)
                    {
                        if ((intersection.Point - block[j]).Length < (intersection.Point - block[j + 1]).Length)
                        {
                            player.Position = block[j];
                            player.PositionIndex[0] = i;
                            player.PositionIndex[1] = j;
                        }

                        else
                        {
                            player.Position = block[j + 1];
                            player.PositionIndex[0] = i;
                            player.PositionIndex[1] = j + 1;
                        }

                        player.UpdateHitbox();
                        player.IsFalling = false;
                        player.Health -= player.Velocity.Length * 10;
                        player.Velocity = new Vector(0, 0);
                        MatchAudioSettings.PlayerHitGround = true;

                        return;
                    }
                }
            }
        }

        private void CheckCollisionPlayerScreen(BananaPlayer player)
        {
            if (player.Position.X < 0 || player.Position.X > 320 || player.Position.Y > 200)
            {
                player.Health = 0;
                player.IsFalling = false;
            }

        }

        private void CheckIfPlayerIsFalling(BananaPlayer player)
        {
            for (int i = 0; i < Land.Border.Count; i++)
            {
                for (int j = 0; j < Land.Border[i].Count; j++)
                {
                    if (player.Position == Land.Border[i][j])
                    {
                        return;
                    }
                }
            }

            // player is hovering in air
            player.IsFalling = true;
        }

    }
}
