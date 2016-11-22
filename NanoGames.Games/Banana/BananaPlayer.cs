﻿// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Banana
{
    class BananaPlayer : Player<BananaMatch>
    {
        public Vector Position = new Vector(0, 0);
        public int[] PositionIndex = new int[2] { 0, 0 };
        public Vector Normal = new Vector(0, 0);
        public double Alpha = 0;

        public bool HasFinished = false;
        public double Radius = 4;
        
        private Vector velocity = new Vector(0, 0);

        private double speedProjectile;

        private double lengthGun = 2;

        private double localAiming = 0.25 * Math.PI;
        private double aiming = 0;
        private double power = 0;
        private double powerMax = 100;

        private int wait = 20;
        private int countLeft = 0;
        private int countRight = 0;
        private int countUp = 0;
        private int countDown = 0;
        private int countFire = 0;
        private int waitFire = 120;
        private int idxWeapon = 0;
        private string[] weapons = new string[] { "Gun", "Grenade" };
        private bool looksRight;

        public void GetBorn()
        {
            PositionIndex[0] = Convert.ToInt32(Match.Random.NextDouble() * (Match.Land.Border.Count - 1));
            PositionIndex[1] = Convert.ToInt32(Match.Random.NextDouble() * (Match.Land.Border[PositionIndex[0]].Count -1));
            
            Position = Match.Land.Border[PositionIndex[0]][PositionIndex[1]];
            Normal = Match.Land.Normal[PositionIndex[0]][PositionIndex[1]];
            Alpha = Math.Atan2(Normal.Y, Normal.X);

            power = 1;
            idxWeapon = 0;
            
            
            if (Match.Random.Next(0, 1) == 0)
            {
                looksRight = false;
                aiming = Alpha - localAiming;
            }
            else
            {
                looksRight = true;
                aiming = Alpha + localAiming;
            }
        }
        
        public void DrawScreen()
        {
            /* Draw each player. */
            foreach (var player in Match.Players)
            {
                /* Skip players that have already finished. */
                if (player.HasFinished)
                {
                    continue;
                }

                Color color;
                if (player == this)
                {
                    /* Always show the current player in white. */
                    color = new Color(1, 1, 1);
                }
                else
                {
                    color = player.Color;
                }

                /* Draw the body of the player. */
                player.Draw(Output.Graphics, color);
                if (player == Match.ActivePlayer)
                {
                    
                }
            }

            /* Draw all the bullets. */
            foreach (SimpleBullet bullet in Match.ListBullets)
            {
                Output.Graphics.Line(new Color(1, 1, 1), bullet.Position, bullet.PositionBefore);
            }

            /* Draw all the grenades. */
            foreach (Grenade grenade in Match.ListGrenades)
            {
                Output.Graphics.Circle(new Color(1, 1, 1), grenade.Position, grenade.Radius);
            }

            /* Draw landscape */
            Match.Land.Draw(Output.Graphics);
            
            // Draw Information on Screen
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 10), "ACTIVEPLAYER: " + Match.ActivePlayer.Name);
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 20), "AIMING: " + (Convert.ToInt32(Match.ActivePlayer.aiming * 180 / Math.PI)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 30), "LOCALAIMING: " + (Convert.ToInt32(Match.ActivePlayer.localAiming * 180 / Math.PI)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 40), "ALPHA: " + (Convert.ToInt32(Alpha * 180 / Math.PI)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 50), "STATEOFGAME: " + Match.StateOfGame.ToUpper());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 60), "WEAPON: " + weapons[idxWeapon].ToUpper());
            Output.Graphics.Print(new Color(1, 1, 1), 10, new Vector(150, 20), Match.SecToGoInRound.ToString());

            if (Match.Wind.Speed >= 0)
            {
                Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 60), "Wind: P " + (Convert.ToInt32(Match.Wind.Speed * 10)).ToString());
            }
            else
            {
                Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 60), "Wind: M " + (Convert.ToInt32(Match.Wind.Speed * 10)).ToString());
            }
        }

        public void SetWeapon()
        {
            if (Input.AltFire.WasActivated)
            {
                idxWeapon++;
                idxWeapon = idxWeapon % weapons.Count();
            }
        }

        public void Move()
        {
            int step = 0;

            if (Input.Left.WasActivated)
            {
                looksRight = false;

                if (countLeft < wait)
                {
                    step = -1;
                }
                else
                {
                    step = -3;
                }
            }

            if (Input.Right.WasActivated)
            {
                looksRight = true;

                if (countRight < wait)
                {
                    step = 1;
                }
                else
                {
                    step = 3;
                }
            }

            if (Input.Left.IsPressed) { countLeft++; }
            else { countLeft = 0; }

            if (Input.Right.IsPressed) { countRight++; }
            else { countRight = 0; }

            if (step != 0)
            {
                PositionIndex[1] += step;
                PositionIndex[1] = mod(PositionIndex[1], Match.Land.Border[PositionIndex[0]].Count);
                Position = Match.Land.Border[PositionIndex[0]][PositionIndex[1]];
                Normal = Match.Land.Normal[PositionIndex[0]][PositionIndex[1]];
                Alpha = Math.Atan2(Normal.Y, Normal.X);
                aiming += Alpha; 
               
                Output.Audio.Play(Sounds.Walk);
            }
        }
        
        public void SetAngle()
        {
            if (Input.Up.WasActivated)
            {
                if (countUp < wait)
                {
                    localAiming -= 1 * Math.PI / 180;
                }
                else
                {
                    localAiming -= 5 * Math.PI / 180;
                }
            }

            if (Input.Down.WasActivated)
            {
                if (countDown < wait)
                {
                    localAiming += 1 * Math.PI / 180;
                }
                else
                {
                    localAiming += 5 * Math.PI / 180;
                }
            }

            if (Input.Up.IsPressed) { countUp++; }
            else { countUp = 0; }

            if (Input.Down.IsPressed) { countDown++; }
            else { countDown = 0; }

            if (localAiming < 10.0 / 180 * Math.PI)
            {
                localAiming = 10.0 / 180 * Math.PI;
            }

            if (localAiming > (115.0 / 180 * Math.PI))
            {
                localAiming = 115.0 / 180 * Math.PI;
            }

            if (looksRight)
            {
                aiming = Alpha + localAiming;
            }
            else
            {
                aiming = Alpha - localAiming;
            }
        }
        
        public void Shoot1()
        {
            if (Input.Fire.WasActivated)
            {
                Match.StateOfGame = "AnimationBeforeShoot";
                countFire = 0;
            }
        }

        public void Shoot2()
        {
            speedProjectile = 1 + 9.0 * countFire / waitFire;
            
            if (!Input.Fire.IsPressed || countFire > waitFire)
            {
                Match.StateOfGame = "AnimationShoot";
            }

            countFire++;
        }

        public void Shoot3()
        {
            if (weapons[idxWeapon] == "Gun")
            {
                Vector velocity = speedProjectile * new Vector(Math.Cos(aiming), Math.Sin(aiming));
                Vector position = Position + 5 * Normal + 5 * new Vector(Math.Cos(aiming), Math.Sin(aiming));

                Output.Audio.Play(Sounds.GunFire);

                Match.ListBullets.Add(new SimpleBullet(position, velocity));
            }

            if (weapons[idxWeapon] == "Grenade")
            {
                Vector velocity = speedProjectile * new Vector(Math.Cos(aiming), -Math.Sin(aiming));
                Vector position = Position + 5 * Normal + 5 * new Vector(Math.Cos(aiming), Math.Sin(aiming));

                Output.Audio.Play(Sounds.GrenadeFire);

                Match.ListGrenades.Add(new Grenade(position, velocity));

            }

            Match.StateOfGame = "AnimationProjectileFly";
        }

        public void Fall()
        {

        }
        
        public void Draw(IGraphics g, Color c)
        {
            /* Body of the player */
            g.CircleSegment(c, Position + 1 * Normal, 2, Alpha + Math.PI / 2, Alpha - Math.PI / 2);
            g.CircleSegment(c, Position + 1 * Normal, 3, Alpha - Math.PI / 2, Alpha + Math.PI / 2);
            g.Line(c, Position + 1 * Normal + 3 * new Vector(-Normal.Y, Normal.X), Position + 1 * Normal + 3 * new Vector(Normal.Y, -Normal.X));

            /* Gun */
            g.Line(c, Position + 5 * Normal, Position + 5 * Normal + 5 * new Vector(Math.Cos(aiming), Math.Sin(aiming)));

        }

        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }


    }
}
