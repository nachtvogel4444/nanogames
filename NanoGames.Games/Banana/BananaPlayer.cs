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
        public Methods methods = new Methods();
        public Vector Position;
        public double Angle;
        
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
                
                Graphics.Circle(color, new Vector(player.Position.X, player.Position.Y), Constants.RadiusPlayer);
                
                List<Vector> list = methods.RotateRectangle(
                    new Vector(-Constants.RadiusPlayer, -Constants.ThicknessGun),
                    new Vector(1.2 * Constants.RadiusPlayer, Constants.ThicknessGun),
                    new Vector(0, 0),
                    Angle);

                for (int i = 0; i <= 2; i++)
                {
                    Graphics.Line(color, methods.ShiftPoint(list[i], player.Position), methods.ShiftPoint(list[i+1], player.Position));
                }
            }

            foreach (Bullet bullet in Match.BulletList)
            {
                if (bullet.IsExploded)
                {
                    continue;
                }

                Vector bulletTip = new Vector(bullet.X, bullet.Y);
                Vector bulletTail = new Vector(bullet.X - Constants.LengthBullet * bullet.Vx, 
                    bullet.Y - Constants.LengthBullet * bullet.Vy);
                Graphics.Line(new Color(1, 1, 1), bulletTip, bulletTail);
            }
        }
    }
}
