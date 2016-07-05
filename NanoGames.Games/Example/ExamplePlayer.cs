// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Example
{
    internal class ExamplePlayer : Player<ExampleMatch>
    {
        public const double Radius = 8;
        public const double Tolerance = 2;

        public bool HasFinished = false;

        public Vector Position;

        public Vector Velocity;

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

                /* Due to the wrap around, a single player can be visible on both edges of the screen. */

                var x1 = player.Position.X;
                var y1 = player.Position.Y;
                var x2 = x1 + (x1 < 160 ? 320 : -320);
                var y2 = y1 + (y1 < 100 ? 200 : -200);

                Output.Graphics.Circle(color, new Vector(x1, y1), Radius);
                Output.Graphics.Circle(color, new Vector(x1, y2), Radius);
                Output.Graphics.Circle(color, new Vector(x2, y1), Radius);
                Output.Graphics.Circle(color, new Vector(x2, y2), Radius);
            }
        }
    }
}
