// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Example
{
    internal class ExamplePlayer : Player<ExampleMatch>
    {
        public const double Radius = 4;
        public const double Tolerance = 1;

        public bool HasFinished = false;

        public Vector Position;

        public Vector Velocity;

        internal override void Initialize()
        {
            /* This is called once for every player after ExampleMatch.Initialize. */
        }

        internal override void Update()
        {
            /* This is called by the framework once every frame for every player. */

            /* Draw the goal. */
            var v = new Vector(Radius + Tolerance, Radius + Tolerance);
            Graphics.Rectangle(new Color(0.25, 0.25, 0.25), Graphics.Center - v, Graphics.Center + v);

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

                Graphics.Circle(color, player.Position, Radius);
            }
        }
    }
}
