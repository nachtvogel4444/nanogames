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
        public int IdxPosition = 1000;
        public Vector PositionBefore;

        public bool HasFinished = false;
        public double Radius = 2;

        private Vector velocity;

        private Vector jumpLeft;
        private Vector jumpRight;
        private Vector jumpVertical;
        private int frameCountJump;

        private double angle = 85 * Math.PI / 180;
        private double speedBullet = 4;
        private double lengthGun = 4;
        
        private double angleJump = 15 * Math.PI / 180;
        private int wait = 20;
        private int countLeft = 0;
        private int countRight = 0;
        private int countUp = 0;
        private int countDown = 0;
        private int idxAction = 0;
        private string[] action = new string[] { "ActivePlayerMoving", "ActivePlayerAiming" };
        

        public void SetPlayer()
        {
            jumpLeft = new Vector(Math.Cos(0.5 * Math.PI + angleJump), -Math.Sin(0.5 * Math.PI + angleJump));
            jumpRight = new Vector(Math.Cos(0.5 * Math.PI - angleJump), -Math.Sin(0.5 * Math.PI - angleJump));
            jumpVertical = new Vector(0, -1);
            
            int idx = Convert.ToInt32(Math.Floor(Match.Random.NextDouble() * Match.Land.XTracks.GetLength(0)));
            Position = new Vector(Match.Land.XTracks[idx], Match.Land.YTracks[idx]);
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
                Output.Graphics.Line(color, player.Position, new Vector(player.Position.X + lengthGun * Math.Cos(player.angle), player.Position.Y - lengthGun * Math.Sin(player.angle)));
            }

            /* Draw all the bullets. */
            foreach (SimpleBullet bullet in Match.ListBullets)
            {
                Output.Graphics.Line(new Color(1, 1, 1), bullet.Position, bullet.PositionBefore);
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
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 20), "ANGLE: " + (Convert.ToInt32(Match.ActivePlayer.angle * 180 / Math.PI)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 30), "SPEED: " + (Convert.ToInt32(Match.ActivePlayer.speedBullet * 10)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 40), "STATEOFGAME: " + Match.StateOfGame.ToUpper());
            Output.Graphics.Print(new Color(1, 1, 1), 10, new Vector(150, 20), Match.SecToGoInRound.ToString());
        }

        public void SelectAction()
        {
            if (Input.AltFire.WasActivated)
            {
                idxAction++;
                idxAction = idxAction % action.Count(); 
                Match.StateOfGame = action[idxAction];
            }
        }

        public void Move()
        {
            if (Input.Left.WasActivated)
            {
                if (countLeft < wait)
                {
                    IdxPosition -= 1; 
                }

                else
                {
                    IdxPosition -= 2;
                }
            }

            if (Input.Right.WasActivated)
            {
                if (countRight < wait)
                {
                    IdxPosition += 1;
                }

                else
                {
                    IdxPosition += 2;
                }
            }

            Position = new Vector(Match.Land.XTracks[IdxPosition], Match.Land.YTracks[IdxPosition]); 
            
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

        public void Jump1()
        {
            if (Input.Up.WasActivated)
            {
                Match.StateOfGame = "AnimationBeforeJump";
                countUp = 0;
            }
        }

        public void Jump2()
        {
            double speed = 0;

            if(countUp > wait)
            {
                speed = 1;
            }

            else
            {
                if (Input.Up.IsPressed)
                {
                    countUp++;
                }

                else
                {
                    speed = 0.5;
                }
            }

            if (speed != 0)
            {
                if (Input.Left.WasActivated || Input.Left.IsPressed)
                {
                    velocity = speed * jumpLeft;
                    Position += (speed + 1.1 * Match.Land.Tolerance) * jumpLeft;
                }

                else if (Input.Right.WasActivated || Input.Right.IsPressed)
                {
                    velocity = speed * jumpRight;
                    Position += (speed + 1.1 * Match.Land.Tolerance) * jumpRight;
                }

                else
                {
                    velocity = speed * jumpVertical;
                    Position += (speed + 1.1 * Match.Land.Tolerance) * jumpVertical;
                }

                frameCountJump = 1;
                Match.StateOfGame = "AnimationJump";
            }
        }

        public void Jump3()
        {
            PositionBefore = Position;
            Position += velocity + 0.5 * new Vector(0, Constants.Gravity) * (2 * frameCountJump + 1);
            velocity += new Vector(0, Constants.Gravity);
            frameCountJump++;
        }


        public void SetAngle()
        {
            double deg = Math.PI / 180;

            if (Input.Left.WasActivated)
            {

                if (countLeft < wait)
                {
                   angle += 1 * deg;
                }

                else
                {
                    angle += 5 * deg;
                }
             }

             if (Input.Right.WasActivated)
             {
                if (countRight < wait)
                {
                    angle -= 1 * deg;
                }

                else
                {
                    angle -= 5 * deg;
                }
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
              
             if (angle > Math.PI)
             {
                 angle = Math.PI;
             }
              
             if (angle < 0)
             {
                 angle = 0;
             }
        }

        public void SetSpeedBullet()
        {
            if (Input.Up.WasActivated)
            {
                if (countUp < wait)
                {
                    speedBullet += 0.1;
                }

                else
                {
                    speedBullet += 0.5;
                }
            }

            if (Input.Down.WasActivated)
            {
                if (countDown < wait)
                {
                    speedBullet -= 0.1;
                }

                else
                {
                    speedBullet -= 0.5;
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

            if (speedBullet < 0)
            {
                speedBullet = 0;
            }

            if (speedBullet > 10)
            {
                speedBullet = 10;
            }
        }

        public void ShootGun()
        {
            if (Input.Fire.WasActivated || Input.Fire.IsPressed)
            {
                Vector velocity = speedBullet * new Vector(Math.Cos(angle), -Math.Sin(angle));
                Vector position = Position + (Radius + speedBullet + 1.1 * Match.Land.Tolerance) * new Vector(Math.Cos(angle), -Math.Sin(angle));

                Match.ListBullets.Add(new SimpleBullet(position, velocity));
                Match.StateOfGame = "AnimationShoot";
            }
        }

    }
}
