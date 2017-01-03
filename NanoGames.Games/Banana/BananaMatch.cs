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
        private int activePlayerIdx = 0;
        public Landscape Land = new Landscape();
        public Bullet Bullet = new Bullet();
        public Wind Wind = new Wind();

        private int finishedPlayers = 0;

        protected override void Initialize()
        {
            // Land.CreateBlock(new Vector(100, 100), new Vector(104, 102));
            Land.CreateBlock(new Vector(100, 100), new Vector(130, 110));
            Land.CreateBlock(new Vector(120, 120), new Vector(190, 300));
            Land.CreateBlock(new Vector(150, 100), new Vector(300, 130));
            Land.Refresh();

            foreach (var player in Players)
            {
                player.GetBorn();
            }

            activePlayerIdx = Convert.ToInt32(Math.Floor(Random.NextDouble() * Players.Count));
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
                    StateOfGame = "ActivePlayerActing";
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
                            somethingFlying = true;
                        }
                    }

                    // bullet flying
                    if (!Bullet.IsExploded)
                    {
                        Bullet.MoveBullet(Wind);
                        CheckCollisionBulletsLand();                  
                        CheckCollisionBulletsPlayers();              
                        CheckCollisionBulletsScreen();                
                        somethingFlying = true;
                    }

                    if (!somethingFlying)
                    {
                        StateOfGame = "NextPlayer";
                    }

                    break;

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
        
        private void CheckCollisionBulletsPlayers()
        {
            foreach (var player in Players)
            {
                for (int i = 0; i < player.Hitbox.Count - 2; i++)
                {
                    Intersection intersection = new Intersection(Bullet.Position, Bullet.PositionBefore, player.Position + player.Hitbox[i], player.Position + player.Hitbox[i + 1]);

                    if (intersection.IsTrue)
                    {
                        Output.Audio.Play(Sounds.Explosion);
                        Bullet.IsExploded = true;
                        Land.makeCaldera(intersection.Point);
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

        private void CheckCollisionBulletsLand()
        {
            foreach (List<Vector> block in Land.Border)
            {
                for (int i = 0; i < block.Count - 1; i++)
                {
                    Intersection intersection = new Intersection(Bullet.Position, Bullet.PositionBefore, block[i], block[i + 1]);

                    if (intersection.IsTrue)
                    {
                        Output.Audio.Play(Sounds.Explosion);
                        Bullet.IsExploded = true;
                        Land.makeCaldera(intersection.Point);

                        foreach (var player in Players)
                        {
                            CheckIfPlayerIsFalling(player);
                        }
                        
                        return;
                    }
                }
            }


        }

        private void CheckCollisionBulletsScreen()
        {
            if (Bullet.Position.X < 0 || Bullet.Position.X > 320 || Bullet.Position.Y > 200)
            {
                Bullet.IsExploded = true;
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

                        player.IsFalling = false;
                        player.Health -= player.Velocity.Length * 10;
                        player.Velocity = new Vector(0, 0);
                        Output.Audio.Play(Sounds.Toc);

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
