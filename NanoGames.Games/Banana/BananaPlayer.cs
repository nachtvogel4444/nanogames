// Copyright (c) the authors of nanoGames. All rights reserved.
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
        public Vector Position;
        public int IdxPosition;
        public Vector PositionBefore;

        public bool HasFinished = false;
        public double Radius = 2;

        private Vector velocity;

        private Vector jumpLeft;
        private Vector jumpRight;
        private Vector jumpVertical;
        private int frameCountJump;

        private double angle;
        private double realAngle;
        private double speedProjectile = 0;
        private double lengthGun = 4;
        
        private double angleJump = 15 * Math.PI / 180;
        private int wait = 20;
        private int waitFire = 72;
        private int countLeft = 0;
        private int countRight = 0;
        private int countUp = 0;
        private int countDown = 0;
        private int countFire = 0;
        private int idxWeapon = 0;
        private string[] weapons = new string[] { "Gun", "Grenade" };
        private bool looksRight;

        public void SetPlayer()
        {
            jumpLeft = new Vector(Math.Cos(0.5 * Math.PI + angleJump), -Math.Sin(0.5 * Math.PI + angleJump));
            jumpRight = new Vector(Math.Cos(0.5 * Math.PI - angleJump), -Math.Sin(0.5 * Math.PI - angleJump));
            jumpVertical = new Vector(0, -1);
            
            IdxPosition = Convert.ToInt32(Math.Floor(Match.Random.NextDouble() * Match.Land.XTracks.GetLength(0)));
            Position = new Vector(Match.Land.XTracks[IdxPosition], Match.Land.YTracks[IdxPosition]);

            angle = Match.Random.NextDouble() * Math.PI - Math.PI / 2;

            if (Match.Random.Next(0, 1) == 0)
            {
                looksRight = false;
                realAngle = -angle + Math.PI;
            }
            else
            {
                looksRight = true;
                realAngle = angle;
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
                Output.Graphics.Circle(color, new Vector(player.Position.X, player.Position.Y), Radius);
                if (player == Match.ActivePlayer)
                {
                    Output.Graphics.Circle(color, new Vector(player.Position.X, player.Position.Y), 0.9 * Radius);
                    Output.Graphics.Circle(color, new Vector(player.Position.X, player.Position.Y), 0.8 * Radius);
                }

                /* Draw the Gun of the player. */
                Output.Graphics.Line(color, player.Position, new Vector(player.Position.X + lengthGun * Math.Cos(player.realAngle), player.Position.Y - lengthGun * Math.Sin(player.realAngle)));
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

            for (int i = 0; i < Match.Land.NPointsPolygon - 1; i++)
            {
                Output.Graphics.Line(new Color(1, 1, 1), new Vector(Match.Land.XPolygon[i], Match.Land.YPolygon[i]), new Vector(Match.Land.XPolygon[i + 1], Match.Land.YPolygon[i + 1]));
            }

            /*
            for (int i = 0; i < Match.Land.NPointsInterpolated - 1; i++)
            {
                Output.Graphics.Point(new Color(1, 1, 1), new Vector(Match.Land.XTracksUpper[i], Match.Land.YTracksUpper[i]));
            }
            */

            // Draw Information on Screen
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 10), "ACTIVEPLAYER: " + Match.ActivePlayer.Name);
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 20), "REALANGLE: " + (Convert.ToInt32(Match.ActivePlayer.realAngle * 180 / Math.PI)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 30), "SPEED: " + Convert.ToInt32((Match.ActivePlayer.speedProjectile) * 10).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 40), "STATEOFGAME: " + Match.StateOfGame.ToUpper());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 50), "WEAPON: " + weapons[idxWeapon].ToUpper());
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

        public void PlaySound()
        {
            Output.Audio.Play(Sounds.Explosion);
        }

        public void SelectWeapon()
        {
            if (Input.AltFire.WasActivated)
            {
                idxWeapon++;
                idxWeapon = idxWeapon % weapons.Count();
            }
        }

        public void Move()
        {
            int idx = 0;

            if (Input.Left.WasActivated)
            {
                if (countLeft < wait)
                {
                    idx = -2; 
                }

                else
                {
                    idx = -4;
                }
            }

            if (Input.Right.WasActivated)
            {
                if (countRight < wait)
                {
                    idx = 2;
                }

                else
                {
                    idx = 4;
                }
            }

            if (idx != 0)
            {
                IdxPosition += idx;

                if (IdxPosition >= 0 || IdxPosition < Match.Land.XTracks.GetLength(0))
                {
                    Position = new Vector(Match.Land.XTracks[IdxPosition], Match.Land.YTracks[IdxPosition]);
                }

                else
                {
                    HasFinished = true;
                }

                if (idx < 0)
                {
                    looksRight = false;
                }
                else
                {
                    looksRight = true;
                }

                Output.Audio.Play(Sounds.Walk);
            }
            
            if (Input.Left.IsPressed)
            {
                countLeft++;
            }
            else
            {
                countLeft = 0;
            }

            if (Input.Right.IsPressed)
            {
                countRight++;
            }
            else
            {
                countRight = 0;
            }
        }

        public void SetAngle()
        {
            if (Input.Up.WasActivated)
            {

                if (countUp < wait)
                {
                    angle += 1 * Math.PI / 180;
                }

                else
                {
                    angle += 5 * Math.PI / 180;
                }
             }

             if (Input.Down.WasActivated)
             {
                if (countDown < wait)
                {
                    angle -= 1 * Math.PI / 180;
                }

                else
                {
                    angle -= 5 * Math.PI / 180;
                }
             }

             if (Input.Up.IsPressed)
             {
                 countUp++;
             }
             else
             {
                 countUp = 0;
             }
              
             if (Input.Down.IsPressed)
             {
                 countDown++;
             }
             else
             {
                 countDown = 0;
             }

            if (angle > Math.PI / 2)
            {
                angle = Math.PI / 2;
            }

            if (angle < - Math.PI / 2)
            {
                angle = - Math.PI / 2;
            }

            if (looksRight)
            {
                realAngle = angle;
            }
            else
            {
                realAngle = -angle + Math.PI;
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
            speedProjectile = 2;// 1 + countFire * 9.0 / waitFire;
            
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
                Vector velocity = speedProjectile * new Vector(Math.Cos(realAngle), -Math.Sin(realAngle));
                Vector position = Position + (Radius + speedProjectile + 1.1 * Match.Land.Tolerance) * new Vector(Math.Cos(realAngle), -Math.Sin(realAngle));

                Output.Audio.Play(Sounds.GunFire);

                Match.ListBullets.Add(new SimpleBullet(position, velocity));
            }

            if (weapons[idxWeapon] == "Grenade")
            {
                Vector velocity = speedProjectile * new Vector(Math.Cos(realAngle), -Math.Sin(realAngle));
                Vector position = Position + (Radius + speedProjectile + 1.1 * Match.Land.Tolerance+4) * new Vector(Math.Cos(realAngle), -Math.Sin(realAngle));

                Output.Audio.Play(Sounds.GrenadeFire);

                Match.ListGrenades.Add(new Grenade(position, velocity));

            }

            Match.StateOfGame = "AnimationProjectileFly";
        }

    }
}
