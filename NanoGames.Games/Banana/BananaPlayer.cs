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
        public Vector PositionTail;
        public Vector Velocity;
        public Vector PositionStartJump;
        public Vector VelocityStartJump;
        public Vector Acceleration;
        public int LifeTimeJump;
        public int IdxPosition;
        public double Angle;
        public double SpeedBullet = 2; 
        public bool HasFinished = false;
        public int Direction = -1;
        public int StepMove;
        private Methods methods = new Methods();

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
            Output.Graphics.Print(new Color(1, 1, 1), 4, new Vector(10, 80), "N: " + (Convert.ToInt32(Match.Land.NPointsInterpolated)).ToString());
        }

        public void CheckCollisionLandscape()
        {
            for (int i = 0; i < Match.Land.NPointsInterpolated - 1; i++)
            {
                Vector obstacle = new Vector(Match.Land.XTracksUpper[i], Match.Land.YTracksUpper[i]);

                if (methods.CheckCollision(Position, PositionTail, obstacle, 0.6 * Constants.Dx))
                {
                    Position = obstacle;
                    IdxPosition = i;
                    Match.StateOfGame = "Game";
                    break;
                }
            }

        }
    }
}
