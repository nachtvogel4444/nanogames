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
        private int frameCountMove = 0;
        private int frameCountMoveMax = 1800;
        public int CountInputLeftRight = 0;
        public int SecToGoInRound = 0;
        public string StateOfGame = "ActivePlayerMoving";
        public BananaPlayer ActivePlayer;
        private int activePlayerIdx = 0;
        public Landscape Land = new Landscape();
        private List<BananaPlayer> listPlayers = new List<BananaPlayer>();
        public List<SimpleBullet> ListBullets = new List<SimpleBullet>();
        public List<Grenade> ListGrenades = new List<Grenade>();
        public Wind Wind = new Wind();

        private int finishedPlayers = 0;

        protected override void Initialize()
        {
            Land.CreateBlock(new Vector(100, 100), new Vector(103, 104));
            //Land.CreateBlock(new Vector(160, 120), new Vector(190, 200));
            //Land.CreateBlock(new Vector(200, 50), new Vector(300, 70));
            Land.Make();

            for (int i = 0; i < Players.Count; ++i)
            {
                listPlayers.Add(Players[i]);
                Players[i].SetPlayer();
            }

            activePlayerIdx = Convert.ToInt32(Math.Floor(Random.NextDouble() * Players.Count));
            ActivePlayer = Players[activePlayerIdx];

            Wind.SetSpeed(Random);
        }

        protected override void Update()
        {
            SecToGoInRound = Convert.ToInt32((frameCountMoveMax - frameCountMove) / 60);
            
            if (frameCountMove >= frameCountMoveMax)
            {
                activePlayerIdx++;
                activePlayerIdx = activePlayerIdx % Players.Count;
                ActivePlayer = Players[activePlayerIdx];
                frameCountMove = 0;
                Wind.SetSpeed(Random);
                StateOfGame = "ActivePlayerMoving";
            }

            switch (StateOfGame)
            {
                case "ActivePlayerMoving":
                    
                    ActivePlayer.SelectWeapon();
                    ActivePlayer.Move();
                    ActivePlayer.SetAngle();
                    ActivePlayer.Shoot1();
                    CheckCollisionActivePlayerScreen();
                    break;

                case "AnimationBeforeShoot":
                    ActivePlayer.Shoot2();
                    break;

                case "AnimationShoot":
                    ActivePlayer.Shoot3();
                    break;

                case "AnimationProjectileFly":

                    if (ListGrenades.Count != 0)
                    {
                        List<Grenade> grenadesToKeep = new List<Grenade>();

                        foreach (Grenade grenade in ListGrenades)
                        {
                            grenade.MoveGrenade();
                        }

                        CheckCollisionGrenadesLand();
                        CheckCollisionGrenadesScreen();

                        foreach (Grenade grenade in ListGrenades)
                        {
                            if (!grenade.IsExploded)
                            {
                                grenadesToKeep.Add(grenade);
                            }
                        }
                        
                        ListGrenades = grenadesToKeep;
                    }

                    if (ListBullets.Count != 0)
                    {
                        List<SimpleBullet> bulletsToKeep = new List<SimpleBullet>();

                        foreach (SimpleBullet bullet in ListBullets)
                        {
                            bullet.MoveSimpleBullet(Wind);
                        }

                        CheckCollisionBulletsLand();
                        CheckCollisionBulletsPlayers();
                        CheckCollisionBulletsScreen();

                        foreach (SimpleBullet bullet in ListBullets)
                        {
                            if (!bullet.IsExploded)
                            {
                                bulletsToKeep.Add(bullet);
                            }
                        }

                        ListBullets = bulletsToKeep;
                    }

                    if (ListGrenades.Count == 0 && ListBullets.Count == 0)
                    {
                        frameCountMove = frameCountMoveMax;
                    }

                    break;
            }

            frameCountMove++;
  
            CheckPlayers();

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
        
        
        private bool CheckCollision(Vector start, Vector stop, Vector obstacle, double minDist)
        {
            // bc: from postion to tail
            // ba: from position to obstacle 
            double bcx = stop.X - start.X;
            double bcy = -stop.Y + start.Y;
            double bax = obstacle.X - start.X;
            double bay = -obstacle.Y + start.Y;
            double cax = obstacle.X - stop.X;
            double cay = -obstacle.Y + stop.Y;
            double distsq;

            double m = (bax * bcx + bay * bcy) / (bcx * bcx + bcy * bcy);

            if (m < 0)
            {
                return false;
            }

            else if (m > 1)
            {
                distsq = cax * cax + cay * cay;
            }

            else
            {
                double dx = obstacle.X - (start.X + m * bcx);
                double dy = -obstacle.Y - (-start.Y + m * bcy);
                distsq = dx * dx + dy * dy;
            }

            if (distsq < (minDist * minDist))
            {
                return true;
            }

            else
            {
                return false;
            }

        }

        private void CheckCollisionBulletsPlayers()
        {
            foreach (SimpleBullet bullet in ListBullets)
            {
                foreach (var player in listPlayers)
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
            }
        }

        private void CheckCollisionBulletsLand()
        {
            foreach (SimpleBullet bullet in ListBullets)
            {
                
                
            }
        }

        private void CheckCollisionBulletsScreen()
        {
            foreach (SimpleBullet bullet in ListBullets)
            {
                if (bullet.Position.X < 0 || bullet.Position.X > 320 || bullet.Position.Y > 200)
                {
                    bullet.IsExploded = true;
                }
            }
        }

        private void CheckCollisionGrenadesPlayers()
        {
            foreach (Grenade grenade in ListGrenades)
            {
                foreach (var player in listPlayers)
                {
                    if (CheckCollision(grenade.Position, grenade.PositionBefore, player.Position, player.Radius + grenade.Radius))
                    {
                        

                        break;
                    }
                }
            }
        }

        private void CheckCollisionGrenadesLand()
        {
            foreach (Grenade grenade in ListGrenades)
            {

            }
        }

        private void CheckCollisionGrenadesScreen()
        {
            foreach (Grenade grenade in ListGrenades)
            {
                if (grenade.Position.X < 0 || grenade.Position.X > 320 || grenade.Position.Y > 200)
                {
                    grenade.IsExploded = true;
                }
            }
        }

        private void CheckCollisionActivePlayerScreen()
        {
            if (ActivePlayer.Position.X < 0 || ActivePlayer.Position.X > 320 || ActivePlayer.Position.Y > 200)
            {
                ActivePlayer.HasFinished = true;
            }
             
        }

        private void CheckPlayers()
        {
            List<BananaPlayer> playersToKeep = new List<BananaPlayer>();

            foreach (var player in listPlayers)
            {
                if (!player.HasFinished)
                {
                    playersToKeep.Add(player);
                }
            }

            listPlayers = playersToKeep;
        }
        
    }
}
