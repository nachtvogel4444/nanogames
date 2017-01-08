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
        public Vector Position = new Vector(0, 0);
        public Vector PositionBefore = new Vector(0, 0);
        public int[] PositionIndex = new int[2] { 0, 0 };
        public Vector Normal = new Vector(0, 0);
        public double Alpha = 0;

        public bool HasFinished = false;
        public double Radius = 4;
        
        public Vector[] Hitbox = new Vector[10];
        // public AudioSettings PlayerAudioSettings = new AudioSettings();

        public Vector Velocity = new Vector(0, 0);

        public double SpeedProjectile = 0;
        public double Health = 100;
        public bool IsFalling = false;
        
        private double localAiming = 0.25 * Math.PI;
        private double aiming = 0;
        

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
            tmp = "TIME LEFT " + ((int)(Match.FramesLeft / 60.0)).ToString();
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
            numberOfLines = (int)(Match.ActivePlayer.Health * 40 / 100);
            for (int i = 0; i < numberOfLines; i++)
            {
                Output.Graphics.Line(colorActivePlayer, new Vector(30 + i, 20), new Vector(30 + i, 28));
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
            Match.Land.Draw(Output.Graphics);

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
                Match.MatchAudioSettings.NextPlayer = false;
            }

            if (Match.MatchAudioSettings.AngleSet)
            {
                Output.Audio.Play(Sounds.Toc);
                Match.MatchAudioSettings.AngleSet = false;
            }

            if (Match.MatchAudioSettings.LoadingPower)
            {
                Output.Audio.Play(Sound.Chirp(1.7, new Pitch(100), new Pitch(300)));
                Match.MatchAudioSettings.LoadingPower = false;
            }

            if (Match.MatchAudioSettings.PlayerShot)
            {
                Output.Audio.Play(Sound.Sequence(
                            Sound.Noise(0.1, new Pitch(800), new Pitch(1000)),
                            Sound.Noise(0.1, new Pitch(500), new Pitch(700)),
                            Sound.Noise(0.1, new Pitch(200), new Pitch(500))));
                Match.MatchAudioSettings.PlayerShot = false;
            }

            if (Match.MatchAudioSettings.PlayerWalked)
            {
                Output.Audio.Play(Sound.Noise(0.1, new Pitch(400), new Pitch(500)));
                Match.MatchAudioSettings.PlayerWalked = false;
            }
            
            if (Match.MatchAudioSettings.TimerFiveSecondsToGo)
            {
                Output.Audio.Play(Sound.Chirp(0.1, Pitch.C(5), Pitch.C(6)));
                Match.MatchAudioSettings.TimerFiveSecondsToGo = false;
            }

            if (Match.MatchAudioSettings.TimerOneSecondToGo)
            {
                Output.Audio.Play(Sound.Tone(1, Pitch.C(6)));
                Match.MatchAudioSettings.TimerOneSecondToGo = false;
            }

            if (Match.MatchAudioSettings.PlayerHitGround)
            {
                Output.Audio.Play(Sounds.Toc);
                Match.MatchAudioSettings.PlayerHitGround = false;
            }

            if (Match.MatchAudioSettings.BulletExploded)
            {
                Output.Audio.Play(Sounds.Explosion);
                Match.MatchAudioSettings.BulletExploded = false;
            }

            if (Match.MatchAudioSettings.GrenadeExploded)
            {
                Output.Audio.Play(Sounds.Explosion);
                Match.MatchAudioSettings.GrenadeExploded = false;
            }
        }

        public void GetBorn()
        {

            PositionIndex[0] = Convert.ToInt32(Match.Random.NextDouble() * (Match.Land.Border.Count - 1));
            PositionIndex[1] = Convert.ToInt32(Match.Random.NextDouble() * (Match.Land.Border[PositionIndex[0]].Count - 1));

            Position = Match.Land.Border[PositionIndex[0]][PositionIndex[1]];
            Normal = Match.Land.Normal[PositionIndex[0]][PositionIndex[1]];
            Alpha = Math.Atan2(Normal.Y, Normal.X);

            UpdateHitbox();

            IdxWeapon = 0;

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
                    step = -2;
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
                    step = 2;
                }
            }

            if (Input.Left.IsPressed) { countLeft++; }
            else { countLeft = 0; }

            if (Input.Right.IsPressed) { countRight++; }
            else { countRight = 0; }

            if (step != 0)
            {
                PositionIndex[1] = mod(PositionIndex[1] + step, Match.Land.Border[PositionIndex[0]].Count);
                Position = Match.Land.Border[PositionIndex[0]][PositionIndex[1]];
                Normal = Match.Land.Normal[PositionIndex[0]][PositionIndex[1]];
                Alpha = Math.Atan2(Normal.Y, Normal.X);
                aiming += Alpha;
                UpdateHitbox();
                Match.MatchAudioSettings.PlayerWalked = true;
            }
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
                Match.StateOfGame = "ActivePlayerShoot2";
                countFire = 0;
                SpeedProjectile = 0;
                Match.MatchAudioSettings.LoadingPower = true;
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
            Vector velocity = SpeedProjectile * new Vector(Math.Cos(aiming), Math.Sin(aiming));
            //Vector velocity = 1 * new Vector(Math.Cos(aiming), Math.Sin(aiming));
            Vector position = Position + 5 * Normal + 5 * new Vector(Math.Cos(aiming), Math.Sin(aiming));

            Match.MatchAudioSettings.PlayerShot = true;

            if (weapons[IdxWeapon] == "Bazooka")
            {
                Match.Bullet.StartBullet(position, velocity);
            }
            
            if (weapons[IdxWeapon] == "Grenade 1sec")
            {
                Match.Grenade.StartGrenade(position, velocity, 60);
            }

            if (weapons[IdxWeapon] == "Grenade 2sec")
            {
                Match.Grenade.StartGrenade(position, velocity, 120);
            }

            if (weapons[IdxWeapon] == "Grenade 3sec")
            {
                Match.Grenade.StartGrenade(position, velocity, 180);
            }

            Match.StateOfGame = "SomethingFlying";


        }

        public void Fall()
        {
            PositionBefore = Position;
            Position += Velocity + 0.5 * new Vector(0, Constants.Gravity);
            Velocity += new Vector(0, Constants.Gravity);
            UpdateHitbox();
            // audio tbd
        }
        
        public void Draw(IGraphics g, Color c, int i)
        {
            string tmp = "";

            /* Body of the player */
            g.CircleSegment(c, Position + 1 * Normal, 2, Alpha + Math.PI / 2, Alpha - Math.PI / 2);
            g.CircleSegment(c, Position + 1 * Normal, 3, Alpha - Math.PI / 2, Alpha + Math.PI / 2);
            g.Line(c, Position + 1 * Normal + 3 * new Vector(-Normal.Y, Normal.X), Position + 1 * Normal + 3 * new Vector(Normal.Y, -Normal.X));

            /* Acitve Player is filled */
            if (this == Match.ActivePlayer)
            {
                g.CircleSegment(c, Position + 1 * Normal, 0.5, Alpha + Math.PI / 2, Alpha - Math.PI / 2);
                g.CircleSegment(c, Position + 1 * Normal, 1, Alpha + Math.PI / 2, Alpha - Math.PI / 2);
                g.CircleSegment(c, Position + 1 * Normal, 1.5, Alpha + Math.PI / 2, Alpha - Math.PI / 2);
                g.CircleSegment(c, Position + 1 * Normal, 2.5, Alpha + Math.PI / 2, Alpha - Math.PI / 2);
            }

            /* hitbox*//*
            for (int j = 0; j < Hitbox.Length - 1; j++)
            {
                g.Line(c, Hitbox[j], Hitbox[j + 1]);
            }*/

            /* Gun */
            g.Line(c, Position + 5 * Normal, Position + 5 * Normal + 5 * new Vector(Math.Cos(aiming), Math.Sin(aiming)));

            /* crosshair*/
            if (this == Match.ActivePlayer)
            {
                var p = Position + 5 * Normal + 15 * new Vector(Math.Cos(aiming), Math.Sin(aiming));
                g.Line(c, p - 1 * new Vector(Math.Cos(aiming + Math.PI / 4), Math.Sin(aiming + Math.PI / 4)), p + 1 * new Vector(Math.Cos(aiming + Math.PI / 4), Math.Sin(aiming + Math.PI / 4)));
                g.Line(c, p - 1 * new Vector(Math.Cos(aiming - Math.PI / 4), Math.Sin(aiming - Math.PI / 4)), p + 1 * new Vector(Math.Cos(aiming - Math.PI / 4), Math.Sin(aiming - Math.PI / 4)));
            }

            /* Name */
            tmp = ((i + Match.StartPlayerIdx) % Match.Players.Count + 1).ToString().ToUpper() + "." + Name;
            g.Print(c, 3, Position + new Vector(-tmp.Length / 2 * 3, -18), tmp);

            /* Health */
            tmp = ((int)Health).ToString().ToUpper();
            g.Print(c, 3, Position + new Vector(-tmp.Length / 2 * 3, -15), tmp);


        }

        public void UpdateHitbox()
        {
            for (int i = 0; i <= 8; i++)
            {
                var angleA = Alpha - Math.PI / 2 + i * Math.PI / 8;
                Hitbox[i] = Position + Normal + new Vector(3 * Math.Cos(angleA), 3 * Math.Sin(angleA));
            }
            
            Hitbox[9] = Position + Normal + new Vector(3 * Math.Cos(Alpha - Math.PI / 2), 3 * Math.Sin(Alpha - Math.PI / 2));
        }

        private int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        
    }
}
