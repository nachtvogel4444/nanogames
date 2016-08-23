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
        private Vector velocity;
        public Vector Acceleration;
        private int frameCountJump;
        public int IdxPosition;
        public double Angle;
        public double SpeedBullet = 4; 
        public bool HasFinished = false;
        public int Direction = -1;
        private Methods methods = new Methods();
        private int countLeft = 0;
        private int countRight = 0;
        private int countUp = 0;
        private int countDown = 0;
        private int idxAction = 0;
        private string[] action = new string[] { "ActivePlayerMoving", "ActivePlayerAiming" };
        private double angleJump = 30 * Math.PI / 180;
        private Vector jumpLeft;
        private Vector jumpRight;
        private Vector jumpVertical;

        public BananaPlayer()
        {
            jumpLeft = new Vector(Math.Cos(0.5 * Math.PI + angleJump), -Math.Sin(0.5 * Math.PI + angleJump));
            jumpRight = new Vector(Math.Cos(0.5 * Math.PI - angleJump), -Math.Sin(0.5 * Math.PI - angleJump));
            jumpVertical = new Vector(0, -1);
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
                Output.Graphics.Circle(color, new Vector(player.Position.X, player.Position.Y), Constants.RadiusPlayer);
                if (player == Match.ActivePlayer)
                {
                    Output.Graphics.Circle(color, new Vector(player.Position.X, player.Position.Y), 0.7 * Constants.RadiusPlayer);
                }

                /* Draw the Gun of the player. */
                Output.Graphics.Line(color, player.Position, new Vector(player.Position.X + Constants.LengthGun * Math.Cos(player.Angle), player.Position.Y - Constants.LengthGun * Math.Sin(player.Angle)));
            }

            /* Draw all the bullets. */
            foreach (SimpleBullet bullet in Match.BulletList)
            {
                Output.Graphics.Line(new Color(1, 1, 1), bullet.Position, bullet.PositionTail);
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
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 50), "ACTIVEPLAYER: " + Match.ActivePlayer.Name + Match.SecToGoInRound.ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 60), "ANGLE: " + (Convert.ToInt32(Match.ActivePlayer.Angle * 180 / Math.PI)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 70), "SPEED: " + (Convert.ToInt32(Match.ActivePlayer.SpeedBullet * 10)).ToString());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 80), "STATEOFGAME: " + Match.StateOfGame.ToUpper());
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 90), "COUNTUP: " + countUp.ToString().ToUpper());
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
                if (countLeft < Constants.WaitTimeKey)
                {
                    IdxPosition -= Constants.StepMove;
                    Position = new Vector(Match.Land.XTracksUpper[IdxPosition], Match.Land.YTracksUpper[IdxPosition]);
                }

                else
                {
                    IdxPosition -= Constants.StepMove * Constants.MultiplierStepMove;
                    Position = new Vector(Match.Land.XTracksUpper[IdxPosition], Match.Land.YTracksUpper[IdxPosition]);
                }
            }

            if (Input.Right.WasActivated)
            {
                if (countRight< Constants.WaitTimeKey)
                {
                    IdxPosition += Constants.StepMove;
                    Position = new Vector(Match.Land.XTracksUpper[IdxPosition], Match.Land.YTracksUpper[IdxPosition]);
                    Direction = 1;
                }

                else
                {
                    IdxPosition += Constants.StepMove * Constants.MultiplierStepMove;
                    Position = new Vector(Match.Land.XTracksUpper[IdxPosition], Match.Land.YTracksUpper[IdxPosition]);
                    Direction = 1;
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

            if(countUp > 15)
            {
                speed = 1.2;
            }

            else
            {
                if (Input.Up.IsPressed)
                {
                    countUp++;
                }

                else
                {
                    speed = 0.7;
                }
            }

            if (speed > 0)
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

                frameCountJump = 0;
                Match.StateOfGame = "AnimationJump";
            }
        }

        public void Jump3()
        {
            Position += velocity + 0.5 * new Vector(0, Constants.Gravity) * (2 * frameCountJump + 1);
            velocity += new Vector(0, Constants.Gravity);
        }

        public void CheckCollisionLandscape()
        {
            Vector positionBefore = Position - velocity;

            for (int i = 0; i < Match.Land.NPointsInterpolated - 1; i++)
            {
                Vector obstacle = new Vector(Match.Land.XTracksUpper[i], Match.Land.YTracksUpper[i]);

                if (methods.CheckCollision(Position, positionBefore, obstacle, 0.6 * Match.Land.Tolerance))
                {
                    Position = obstacle;
                    IdxPosition = i;
                    Match.StateOfGame = "ActivePlayerMoving";
                    break;
                }
            }

        }

        public void SetAngle()
        {
            if (Input.Left.WasActivated)
            {
               if (countLeft < Constants.WaitTimeKey)
               {
                   Angle += Constants.StepAngle;
               }

               else
               {
                   Angle += Constants.StepAngle * Constants.MultiplierAngle;
               }
            }

            if (Input.Right.WasActivated)
            {
               if (countRight< Constants.WaitTimeKey)
               {
                   Angle -= Constants.StepAngle;
               }

               else
               {
                   Angle -= Constants.StepAngle * Constants.MultiplierAngle;
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

            if (Angle > Math.PI)
            {
                Angle = Math.PI;
            }

            if (Angle < 0)
            {
                Angle = 0;
            }


        }

        public void SetSpeedBullet()
        {
            if (Input.Up.WasActivated)
            {
                if (countUp < Constants.WaitTimeKey)
                {
                    SpeedBullet += Constants.StepSpeedBullet;
                }

                else
                {
                    SpeedBullet += 5 * Constants.StepSpeedBullet;
                }
            }

            if (Input.Down.WasActivated)
            {
                if (countDown < Constants.WaitTimeKey)
                {
                    SpeedBullet -= Constants.StepSpeedBullet;
                }

                else
                {
                    SpeedBullet -= 5 * Constants.StepSpeedBullet;
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

            if (SpeedBullet < 0)
            {
                SpeedBullet = 0;
            }

            if (SpeedBullet > 10)
            {
                SpeedBullet = 10;
            }
        }

        public void ShootGun()
        {
            if (Input.Fire.WasActivated || Input.Fire.IsPressed)
            {
                Vector speed = SpeedBullet * new Vector(Math.Cos(Angle), -Math.Sin(Angle));
                Match.BulletList.Add(new SimpleBullet(Position + speed + Constants.LengthGun * new Vector(Math.Cos(Angle), -Math.Sin(Angle)), speed));
                Match.StateOfGame = "AnimationShoot";
            }
        }

    }
}
