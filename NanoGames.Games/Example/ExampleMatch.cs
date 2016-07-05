// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Example
{
    internal class ExampleMatch : Match<ExamplePlayer>
    {
        private const double _radius = 8;
        private const double _tolerance = 2;

        private const int _stepsPerFrame = 10;
        private const double _acceleration = 0.001;
        private const double _maxSpeed = 0.5;

        private static readonly Vector _center = new Vector(160, 100);

        private int _finishedPlayers;

        protected override void Initialize()
        {
            /* This is called by the framework at the start of every match. */

            /* Arrange the players in a circle. */
            double start = Random.NextDouble() * 2 * Math.PI;
            for (int i = 0; i < Players.Count; ++i)
            {
                double angle = start + i * 2 * Math.PI / Players.Count;
                Players[i].Position = new Vector(160 + 88 * Math.Cos(angle), 100 + 88 * Math.Sin(angle));
            }
        }

        protected override void Update()
        {
            /*
             * This is called by the framework once every frame.
             * We have to draw draw onto every player's Graphics interface.
             */

            for (int i = 0; i < _stepsPerFrame; ++i)
            {
                /*
                 * We update in tiny increments to make the physics more robust.
                 */

                foreach (var player in Players)
                {
                    MovePlayer(player);
                }
            }

            if (Players.Count == 1)
            {
                /* Practice mode. */
                if (_finishedPlayers == 1)
                {
                    IsCompleted = true;
                }
            }
            else
            {
                /* Tournament mode. The match ends when the second-to-last player has reached the goal. */
                if (_finishedPlayers >= Players.Count - 1)
                {
                    IsCompleted = true;
                }
            }

            /* Draw the goal for all players. */
            var v = new Vector(_radius + _tolerance, _radius + _tolerance);
            Output.Graphics.Rectangle(new Color(0.25, 0.25, 0.25), _center - v, _center + v);

            DrawScreen();
        }

        private static Vector LimitSpeed(Vector velocity)
        {
            var speed = velocity.Length;
            if (speed > _maxSpeed)
            {
                velocity *= _maxSpeed / speed;
            }

            return velocity;
        }

        private static Vector NormalizeVector(Vector vector)
        {
            if (vector.X < -160)
            {
                vector.X += 320;
            }

            if (vector.X > 160)
            {
                vector.X -= 320;
            }

            if (vector.Y < -100)
            {
                vector.Y += 200;
            }

            if (vector.Y > 100)
            {
                vector.Y -= 200;
            }

            return vector;
        }

        private void DrawScreen()
        {
            /* Draw each player. */
            foreach (var player in Players)
            {
                /* Skip players that have already finished. */
                if (player.HasFinished)
                {
                    continue;
                }

                Color color = player.LocalColor;

                /* Due to the wrap around, a single player can be visible on both edges of the screen. */

                var x1 = player.Position.X;
                var y1 = player.Position.Y;
                var x2 = x1 + (x1 < 160 ? 320 : -320);
                var y2 = y1 + (y1 < 100 ? 200 : -200);

                Output.Graphics.Circle(color, new Vector(x1, y1), _radius);
                Output.Graphics.Circle(color, new Vector(x1, y2), _radius);
                Output.Graphics.Circle(color, new Vector(x2, y1), _radius);
                Output.Graphics.Circle(color, new Vector(x2, y2), _radius);
            }
        }

        private void MovePlayer(ExamplePlayer player)
        {
            /* Skip players that have already finished. */
            if (player.HasFinished)
            {
                return;
            }

            var acceleration = default(Vector);

            if (player.Input.Up.IsPressed)
            {
                acceleration.Y -= 1;
            }

            if (player.Input.Down.IsPressed)
            {
                acceleration.Y += 1;
            }

            if (player.Input.Left.IsPressed)
            {
                acceleration.X -= 1;
            }

            if (player.Input.Right.IsPressed)
            {
                acceleration.X += 1;
            }

            if (acceleration != default(Vector))
            {
                player.Velocity += _acceleration * acceleration.Normalized;
            }

            /* Cap the speed at the maximum. */
            player.Velocity = LimitSpeed(player.Velocity);

            /* Move the player by their velocity. */
            player.Position += player.Velocity;

            /* Wrap around the screen. */

            if (player.Position.X < 0)
            {
                player.Position.X += 320;
            }

            if (player.Position.X > 320)
            {
                player.Position.X -= 320;
            }

            if (player.Position.Y < 0)
            {
                player.Position.Y += 200;
            }

            if (player.Position.Y > 200)
            {
                player.Position.Y -= 200;
            }

            /* Check for collisions. */
            foreach (var otherPlayer in Players)
            {
                var relativePosition = NormalizeVector(otherPlayer.Position - player.Position);

                if (otherPlayer != player
                    && !otherPlayer.HasFinished
                    && relativePosition.Length < 2 * _radius)
                {
                    /*
                     * We overlap with the other player.
                     * This is only a collision if the players are moving towards each other, otherwise, let them move apart naturally.
                     */

                    var relativeVelocity = otherPlayer.Velocity - player.Velocity;

                    /* The dot product tells us if the vectors are oriented towards each other. */
                    if (Vector.Dot(relativePosition, relativeVelocity) < 0)
                    {
                        /*
                         * The other player is moving towards us, relatively.
                         * Compute the result of a perfectly elastic condition.
                         */

                        var velocityExchange = Vector.Dot(relativeVelocity, relativePosition.Normalized) * relativePosition.Normalized;
                        player.Velocity = LimitSpeed(player.Velocity + velocityExchange);
                        otherPlayer.Velocity = LimitSpeed(otherPlayer.Velocity - velocityExchange);
                    }
                }
            }

            /* Check for the victory condition. */
            if ((player.Position - _center).Length < _tolerance)
            {
                ++_finishedPlayers;
                player.HasFinished = true;

                /* Finishing earlier is better. */
                player.Score = Players.Count - _finishedPlayers;
            }
        }
    }
}
