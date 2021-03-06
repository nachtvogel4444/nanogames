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
        public Pixel Pixel = new Pixel(0, 0);
        
        public bool HasFinished = false;
        public double Radius = 4;
        
        public Vector[] Hitbox = new Vector[10];
        // public AudioSettings PlayerAudioSettings = new AudioSettings();
        
        public double SpeedProjectile = 0;
        public double Health = 100;
        public bool IsFalling = false;
        
        private double localAiming = 0.25 * Math.PI;
        public double Aiming = 0;
        

        private int wait = 20;
        private int countLeft = 0;
        private int countRight = 0;
        private int countUp = 0;
        private int countDown = 0;
        private int countFire = 0;
        public int IdxWeapon = 0;
        private string[] weapons = new string[] { "Bazooka", "Grenade 1sec", "Grenade 2sec", "Grenade 3sec" };
        private bool looksRight;
        
        public void DrawScreen()
        {
            string tmp;
            Color colorActivePlayer = Match.ActivePlayer.Color;

            if (this == Match.ActivePlayer)
            {
                colorActivePlayer = new Color(1, 1, 1);
            }

            /* draw time left */
            if (Match.FramesLeft < 0)
            {
                tmp = "TIME LEFT 0";
            }
            else
            {
                tmp = "TIME LEFT " + ((int)(Match.FramesLeft / 60.0)).ToString();
            }
            Output.Graphics.Print(new Color(1, 1, 1), 6, new Vector(160 - tmp.Length * 6 / 2, 2),  tmp);

            /* draw active player name*/
            tmp = Match.ActivePlayer.Name;
            Output.Graphics.Print(new Color(1, 1, 1), 6, new Vector(50 - tmp.Length / 2 * 6, 2), tmp);

            /* draw active weapon*/
            tmp = Match.ActivePlayer.weapons[Match.ActivePlayer.IdxWeapon].ToUpper();
            Output.Graphics.Print(new Color(1, 1, 1), 6, new Vector(270 - tmp.Length / 2 * 6, 2), tmp);

            // draw speedprojectile / power
            int numberOfLines = (int)(Match.ActivePlayer.SpeedProjectile * 100 / 5);
            for (int i = 0; i < numberOfLines; i++)
            {
                Output.Graphics.Line(colorActivePlayer, new Vector(110 + i, 20), new Vector(110 + i, 28));
            }
            Output.Graphics.Line(colorActivePlayer, new Vector(110, 20), new Vector(110, 28));
            Output.Graphics.Line(colorActivePlayer, new Vector(110, 20), new Vector(210, 20));
            Output.Graphics.Line(colorActivePlayer, new Vector(110, 28), new Vector(210, 28));
            Output.Graphics.Line(colorActivePlayer, new Vector(210, 20), new Vector(210, 28));
            tmp = "POWER " + (Convert.ToInt32(SpeedProjectile * 20)).ToString();
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(160 - tmp.Length * 4 / 2, 15), tmp);

            // Draw Health
            numberOfLines = (int)(Match.ActivePlayer.Health * 60 / 100);
            for (int i = 0; i < numberOfLines; i++)
            {
                Output.Graphics.Line(colorActivePlayer, new Vector(30.0 + 2.0 / 3 * i, 20), new Vector(30.0 + 2.0 / 3 * i, 28));
            }
            Output.Graphics.Line(colorActivePlayer, new Vector(30, 20), new Vector(30, 28));
            Output.Graphics.Line(colorActivePlayer, new Vector(30, 20), new Vector(70, 20));
            Output.Graphics.Line(colorActivePlayer, new Vector(30, 28), new Vector(70, 28));
            Output.Graphics.Line(colorActivePlayer, new Vector(70, 20), new Vector(70, 28));
            tmp = "HEALTH " + (Convert.ToInt32(Match.ActivePlayer.Health)).ToString().ToUpper();
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(50 - tmp.Length * 4 / 2, 15), tmp);
            
            // draw wind
            numberOfLines = (int)(Math.Abs(Match.Wind.Speed) * 40 / 5);
            for (int i = 0; i < numberOfLines; i++)
            {
                if (Match.Wind.Speed < 0)
                {
                    Output.Graphics.Line(new Color(1, 1, 1), new Vector(270 - i, 20), new Vector(268 - i, 24));
                    Output.Graphics.Line(new Color(1, 1, 1), new Vector(268 - i, 24), new Vector(270 - i, 28));
                }
                if (Match.Wind.Speed > 0)
                {
                    Output.Graphics.Line(new Color(1, 1, 1), new Vector(270 + i, 20), new Vector(272 + i, 24));
                    Output.Graphics.Line(new Color(1, 1, 1), new Vector(272 + i, 24), new Vector(270 + i, 28));
                }
            }

            Output.Graphics.Line(new Color(1, 1, 1), new Vector(270, 20), new Vector(270, 28));
            Output.Graphics.Line(new Color(1, 1, 1), new Vector(250, 20), new Vector(250, 28));
            Output.Graphics.Line(new Color(1, 1, 1), new Vector(250, 20), new Vector(290, 20));
            Output.Graphics.Line(new Color(1, 1, 1), new Vector(250, 28), new Vector(290, 28));
            Output.Graphics.Line(new Color(1, 1, 1), new Vector(290, 20), new Vector(290, 28));
            tmp = "WIND " + (Convert.ToInt32(Math.Abs(Match.Wind.Speed) * 10)).ToString();
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(270 - tmp.Length / 2 * 4, 15), tmp);

            /* Draw landscape */
            Match.Map.Draw(Output.Graphics, new Color(1, 1, 1));
            
            /* Draw each player. */
            for (int i = 0; i < Match.Players.Count; i++)
            {
                var player = Match.Players[i];

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
                player.Draw(Output.Graphics, color, i);

            }

            /* Draw bullet. */
            if (!Match.Bullet.IsExploded)
            {
                Output.Graphics.Line(new Color(1, 1, 1), Match.Bullet.Position, Match.Bullet.PositionBefore);
            }

            /* Draw grenade. */
            if (!Match.Grenade.IsExploded && !Match.Grenade.IsDead)
            {
                Output.Graphics.Circle(new Color(1, 1, 1), Match.Grenade.Position, 1);
                double t = Match.Grenade.TimeLeft / 60.0;
                int s = (int)(t);
                int d = (int)((t - s) * 10.0);
                Output.Graphics.Print(new Color(1, 1, 1), 3, Match.Grenade.Position + new Vector(0, -7), s.ToString() + "." + d.ToString());
            }

            /*
            // Draw Information on Screen
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 10), "ACTIVEPLAYER: " + Match.ActivePlayer.Name);
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 20), "AIMING: " + (Convert.ToInt32(Match.ActivePlayer.aiming * 180 / Math.PI)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 30), "LOCALAIMING: " + (Convert.ToInt32(Match.ActivePlayer.localAiming * 180 / Math.PI)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 40), "POWER: " + (Convert.ToInt32(speedProjectile)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 50), "STATEOFGAME: " + Match.StateOfGame.ToUpper());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 60), "WEAPON: " + weapons[idxWeapon].ToUpper());
            Output.Graphics.Print(new Color(1, 1, 1), 10, new Vector(150, 20), ((int)(Match.FramesLeft / 60.0)).ToString());
            */
        }

        public void PlayAudio()
        {
            if (Match.MatchAudioSettings.NextPlayer)
            {
                Output.Audio.Play(Sound.Tone(1, Pitch.C(6)));
            }

            if (Match.MatchAudioSettings.AngleSet)
            {
                Output.Audio.Play(Sounds.Toc);
            }

            if (Match.MatchAudioSettings.LoadingPower)
            {
                Output.Audio.Play(Sound.Chirp(1.7, new Pitch(100), new Pitch(300)));
            }

            if (Match.MatchAudioSettings.PlayerShot)
            {
                Output.Audio.Play(Sound.Sequence(
                            Sound.Noise(0.1, new Pitch(800), new Pitch(1000)),
                            Sound.Noise(0.1, new Pitch(500), new Pitch(700)),
                            Sound.Noise(0.1, new Pitch(200), new Pitch(500))));
            }

            if (Match.MatchAudioSettings.PlayerWalked)
            {
                Output.Audio.Play(Sound.Noise(0.1, new Pitch(400), new Pitch(500)));
            }

            if (Match.MatchAudioSettings.CannotShot)
            {
                Output.Audio.Play(Sounds.Toc);
            }

            if (Match.MatchAudioSettings.TimerFiveSecondsToGo)
            {
                Output.Audio.Play(Sound.Chirp(0.1, Pitch.C(5), Pitch.C(6)));
            }

            if (Match.MatchAudioSettings.TimerOneSecondToGo)
            {
                Output.Audio.Play(Sound.Tone(1, Pitch.C(6)));
            }

            if (Match.MatchAudioSettings.PlayerHitGround)
            {
                Output.Audio.Play(Sounds.Toc);
            }

            if (Match.MatchAudioSettings.BulletExploded)
            {
                Output.Audio.Play(Sounds.Explosion);
            }

            if (Match.MatchAudioSettings.GrenadeExploded)
            {
                Output.Audio.Play(Sounds.Explosion);
            }
        }

        public void GetBorn(Pixel pixel)
        {
            Score = int.MaxValue;

            Pixel = pixel;

            IdxWeapon = 0;

            if (Match.Random.Next(0, 1) == 0)
            {
                looksRight = false;
                Aiming = pixel.Alpha - localAiming;
            }
            else
            {
                looksRight = true;
                Aiming = pixel.Alpha + localAiming;
            }
        }

        public void SetWeapon()
        {
            if (Input.AltFire.WasActivated)
            {
                IdxWeapon++;
                IdxWeapon = IdxWeapon % weapons.Count();
                Match.MatchAudioSettings.WeaponSelected = true;
            }
        }

        public void Move()
        {
            // player can only move in steps of 1
            // player moves with velocity 2 as standard
            // single tap on arrow key is one step
            
        if (Input.Left.WasActivated)
            {
                looksRight = false;

                if (countLeft < wait)
                {
                    Pixel = Pixel.Left;
                }
                else
                {
                    Pixel = Pixel.Left;
                    Pixel = Pixel.Left;
                }
            }

            if (Input.Right.WasActivated)
            {
                looksRight = true;

                if (countRight < wait)
                {
                    Pixel = Pixel.Right;
                }
                else
                {
                    Pixel = Pixel.Right;
                    Pixel = Pixel.Right;
                }
            }

            if (Input.Left.IsPressed) { countLeft++; }
            else { countLeft = 0; }

            if (Input.Right.IsPressed) { countRight++; }
            else { countRight = 0; }
            
        }

        public void SetAngle()
        {
            double angle = 0;

            if (Input.Up.WasActivated)
            {
                if (countUp < wait)
                {
                    angle = -1 * Math.PI / 180;
                }
                else
                {
                    angle = -5 * Math.PI / 180;
                }
            }

            if (Input.Down.WasActivated)
            {
                if (countDown < wait)
                {
                    angle = 1 * Math.PI / 180;
                }
                else
                {
                    angle = 5 * Math.PI / 180;
                }
            }

            if (angle != 0)
            {
                localAiming += angle;
                Match.MatchAudioSettings.AngleSet = true;
            }

            if (Input.Up.IsPressed) { countUp++; }
            else { countUp = 0; }

            if (Input.Down.IsPressed) { countDown++; }
            else { countDown = 0; }

            if (localAiming < 5.0 / 180 * Math.PI)
            {
                localAiming = 5.0 / 180 * Math.PI;
            }

            if (localAiming > (130.0 / 180 * Math.PI))
            {
                localAiming = 130.0 / 180 * Math.PI;
            }

            if (looksRight)
            {
                Aiming = Pixel.Alpha + localAiming;
            }
            else
            {
                Aiming = Pixel.Alpha - localAiming;
            }
        }
        
        public void Shoot1()
        {
            if (Input.Fire.WasActivated)
            {
                Vector guntip = Pixel.Position + 5 * Pixel.Normal + 5 * new Vector(Math.Cos(Aiming), Math.Sin(Aiming));
                
                if (Match.Land.CheckIsSolid(guntip))
                {
                    Match.MatchAudioSettings.CannotShot = true;
                }

                else
                {
                    Match.StateOfGame = "ActivePlayerShoot2";
                    countFire = 0;
                    SpeedProjectile = 0;
                    Match.MatchAudioSettings.LoadingPower = true;
                }
            }
        }

        public void Shoot2()
        {
            SpeedProjectile = 0.5 + 4.5 * countFire / 98;
            
            if (!Input.Fire.IsPressed || countFire > 98)
            {
                Match.StateOfGame = "ActivePlayerShoot3";
            }

            countFire++;
        }

        public void Shoot3()
        {
            Vector velocity = SpeedProjectile * new Vector(Math.Cos(Aiming), Math.Sin(Aiming));
            Vector guntip = Pixel.Position + 5 * Pixel.Normal + 5 * new Vector(Math.Cos(Aiming), Math.Sin(Aiming));

            Match.MatchAudioSettings.PlayerShot = true;

            if (weapons[IdxWeapon] == "Bazooka")
            {
                Match.Bullet.StartBullet(guntip + velocity, velocity);
            }
            
            if (weapons[IdxWeapon] == "Grenade 1sec")
            {
                Match.Grenade.StartGrenade(guntip + velocity, velocity, 60);
            }

            if (weapons[IdxWeapon] == "Grenade 2sec")
            {
                Match.Grenade.StartGrenade(guntip + velocity, velocity, 120);
            }

            if (weapons[IdxWeapon] == "Grenade 3sec")
            {
                Match.Grenade.StartGrenade(guntip + velocity, velocity, 180);
            }

            Match.StateOfGame = "SomethingFlying";
            
        }
        
        public void Draw(IGraphics g, Color c, int i)
        {
            string tmp = "";

            Vector refpoint = Pixel.Position + 1 * Pixel.Normal;

            /* Body of the player */
            g.CircleSegment(c, refpoint, 2, Pixel.Alpha + Math.PI / 2, Pixel.Alpha - Math.PI / 2);
            g.CircleSegment(c, refpoint, 3, Pixel.Alpha - Math.PI / 2, Pixel.Alpha + Math.PI / 2);
            g.Line(c, refpoint + 3 * new Vector(-Pixel.Normal.Y, Pixel.Normal.X), Pixel.Position + 1 * Pixel.Normal + 3 * new Vector(Pixel.Normal.Y, -Pixel.Normal.X));

            /* Acitve Player is filled */
            if (this == Match.ActivePlayer)
            {
                g.CircleSegment(c, refpoint, 0.5, Pixel.Alpha + Math.PI / 2, Pixel.Alpha - Math.PI / 2);
                g.CircleSegment(c, refpoint, 1.0, Pixel.Alpha + Math.PI / 2, Pixel.Alpha - Math.PI / 2);
                g.CircleSegment(c, refpoint, 1.5, Pixel.Alpha + Math.PI / 2, Pixel.Alpha - Math.PI / 2);
                g.CircleSegment(c, refpoint, 2.5, Pixel.Alpha + Math.PI / 2, Pixel.Alpha - Math.PI / 2);
            }

            /* hitbox*//*
            for (int j = 0; j < Hitbox.Length - 1; j++)
            {
                g.Line(c, Hitbox[j], Hitbox[j + 1]);
            }*/

            /* Gun */
            g.Line(c, refpoint + 3 * Pixel.Normal, refpoint + 3 * Pixel.Normal + 5 * new Vector(Math.Cos(Aiming), Math.Sin(Aiming)));

            /* crosshair*/
            if (this == Match.ActivePlayer)
            {
                var p = refpoint + 2 * Pixel.Normal + 15 * new Vector(Math.Cos(Aiming), Math.Sin(Aiming));
                g.Line(c, p - 1 * new Vector(Math.Cos(Aiming + Math.PI / 4), Math.Sin(Aiming + Math.PI / 4)), p + 1 * new Vector(Math.Cos(Aiming + Math.PI / 4), Math.Sin(Aiming + Math.PI / 4)));
                g.Line(c, p - 1 * new Vector(Math.Cos(Aiming - Math.PI / 4), Math.Sin(Aiming - Math.PI / 4)), p + 1 * new Vector(Math.Cos(Aiming - Math.PI / 4), Math.Sin(Aiming - Math.PI / 4)));
            }

            /* Name */
            tmp = ((i + Match.StartPlayerIdx) % Match.Players.Count + 1).ToString().ToUpper() + "." + Name;
            g.Print(c, 3, Pixel.Position + new Vector(-tmp.Length / 2 * 3, -18), tmp);

            /* Health */
            tmp = ((int)Health).ToString().ToUpper();
            g.Print(c, 3, Pixel.Position + new Vector(-tmp.Length / 2 * 3, -15), tmp);
            
        }

        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
        
    }
}
