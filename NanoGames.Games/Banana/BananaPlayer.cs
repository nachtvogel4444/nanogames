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
        public double Angle;
        public double SpeedBullet;
        public bool GunIsRight = true;
        private Polygon StartPolygonGun = new Polygon(new Vector[] {
            new Vector(-Constants.RadiusPlayer, -Constants.ThicknessGun),
            new Vector(1.2 * Constants.RadiusPlayer, -Constants.ThicknessGun),
            new Vector(1.2 * Constants.RadiusPlayer, Constants.ThicknessGun),
            new Vector(-Constants.RadiusPlayer, Constants.ThicknessGun),
        }, false);
        private int endIndex = 0;    
        public bool HasFinished = false;

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
                Graphics.Circle(color, new Vector(player.Position.X, player.Position.Y), Constants.RadiusPlayer);

                /* Draw the Gun of the player. */
                Polygon newPolygonGun = StartPolygonGun.RotatePolygon(player.Angle);
                if (StartPolygonGun.IsClosed)
                {
                    endIndex = StartPolygonGun.Count() - 1;
                }
                if (!StartPolygonGun.IsClosed)
                {
                    endIndex = StartPolygonGun.Count() - 2;
                }
                
                for (int i = 0; i <= endIndex ; i++)
                {
                    Graphics.Line(color, player.Position + newPolygonGun[i], player.Position + newPolygonGun[i + 1]);
                }
            }

            /* Draw all the bullets. */
            foreach (Bullet bullet in Match.BulletList)
            {
                if (bullet.IsExploded)
                {
                    continue;
                }
                
                Graphics.Line(new Color(1, 1, 1), bullet.Position, bullet.PostionTail);
            }
        }
    }
}
