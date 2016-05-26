// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.NanoSoccer
{
    internal class NanoSoccerPlayer : Player<NanoSoccerMatch>, Circle
    {
        public const double PlayerRadius = 8;
        public const double Tolerance = 2;

        public int Team;

        public double Radius
        {
            get { return PlayerRadius; }
        }

        public Vector Position
        {
            get; set;
        }

        public Vector Velocity
        {
            get; set;
        }

        public void DrawScreen()
        {
            /* Draw each player. */
            foreach (var player in Match.Players)
            {
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

                Graphics.Circle(color, player.Position, Radius);

                Graphics.Circle(Team == 0 ? new Color(1, 0, 0) : new Color(0, 0, 1), player.Position, Radius * 0.8d);
            }
        }
    }
}
