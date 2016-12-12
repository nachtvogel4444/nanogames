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

            Wind.SetSpeed(0);

            FramesLeft = framesMax;
        }

        protected override void Update()
        {

            if (FramesLeft < 0)
            {
                activePlayerIdx++;
                activePlayerIdx = activePlayerIdx % Players.Count;
                ActivePlayer = Players[activePlayerIdx];
                FramesLeft = framesMax;
                // Wind.SetSpeed(Random);
                StateOfGame = "AnimationNextPLayer";
            }

            switch (StateOfGame)
            {
                case "ActivePlayerActing":

                    ActivePlayer.Move();
                    ActivePlayer.SetAngle();
                    ActivePlayer.SetWeapon();
                    ActivePlayer.Shoot1();                  // StateOfGame -> Shooting2
                    break;
                    
                case "ActivePlayerLastMove":

                    ActivePlayer.Move();
                    ActivePlayer.SetAngle();
                    ActivePlayer.SetWeapon();
                    break;

                case "ActivePlayerShooting2":
                    ActivePlayer.Shoot2();                  // StateOfGame -> Shooting3
                    break;

                case "ActivePlayerShooting3":
                    ActivePlayer.Shoot3();                  // StateOfGame -> BulletFlying, ...
                    break;

                case "PlayerFalling":

                    bool someOneFalls = false;

                    foreach (var player in Players)
                    {
                        if (player.IsFalling)
                        {
                            player.Fall();
                            CheckCollisionPlayerLand(player);
                            someOneFalls = true;
                            FramesLeft++;
                        }
                    }

                    if (!someOneFalls)
                    {
                        StateOfGame = "ActivePlayerLastMove";
                        FramesLeft = 60;
                    }

                    break;

                case "BulletFlying":

                    Bullet.MoveBullet(Wind);

                    CheckCollisionBulletsLand();                  // StateOfGame -> ActivePlayerLastMove
                    // CheckCollisionBulletsPlayers();               // StateOfGame -> ActivePlayerLastMove
                    CheckCollisionBulletsScreen();                // StateOfGame -> ActivePlayerLastMove

                    break;

                case "AnimationNextPlayer":
                    StateOfGame = "ActivePlayerMoving";
                    break;

            }

            FramesLeft--;

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
        /*
        private void CheckCollisionBulletsPlayers()
        {
            foreach (var player in Players)
            {
                if (CheckCollision(bullet.Position, bullet.PositionBefore, player.Position, player.Radius))
                {
                    bullet.IsExploded = true;
                    player.HasFinished = true;
                    finishedPlayers++;

                    Output.Audio.Play(Sounds.Explosion);

                    break;
                }
            }
            
        }*/

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
                        StateOfGame = "ActivePlayerLastMove";
                        FramesLeft = 60;

                        Bullet.IsExploded = true;

                        Land.makeCaldera(intersection.Point);

                        foreach (var player in Players)
                        {
                            CheckPlayerPosition(player);
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

                StateOfGame = "ActivePlayerLastMove";
                FramesLeft = 60 * 5;
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
                player.HasFinished = true;
                player.IsFalling = false;
            }

        }

        private void CheckPlayerPosition(BananaPlayer player)
        {
            for (int i = 0; i < Land.Border.Count; i++)
            {
                for (int j = 0; j < Land.Border[i].Count; j++)
                {
                    if (player.Position == Land.Border[i][j])
                    {
                        player.PositionIndex[0] = i;
                        player.PositionIndex[1] = j;

                        return;
                    }
                }
                
                // player is hovering in air
                player.IsFalling = true;
                StateOfGame = "PlayerFalling";
                FramesLeft = 10;
            }
        }
    }
}
