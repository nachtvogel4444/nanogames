// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Example
{
    internal class ExampleMatch : Match<ExamplePlayer>
    {
        private const int _stepsPerFrame = 10;
        private const double _acceleration = 0.002;
        private const double _maxSpeed = 0.5;

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

            /* After this, ExamplePlayer.Initialize is called by the framework for every individual player. */
        }

        protected override void Update()
        {
            /*
             * This is called by the framework once every frame.
             * We can either draw onto every player's graphics interface here, or do that in ExamplePlayer.Update.
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

            /* After this, ExamplePlayer.Update is called by the framework for every individual player. */
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

        private void MovePlayer(ExamplePlayer player)
        {
            /* Skip players that have already finished. */
            if (player.HasFinished)
            {
                return;
            }

            if (player.Input.Up)
            {
                player.Velocity.Y -= _acceleration;
            }

            if (player.Input.Down)
            {
                player.Velocity.Y += _acceleration;
            }

            if (player.Input.Left)
            {
                player.Velocity.X -= _acceleration;
            }

            if (player.Input.Right)
            {
                player.Velocity.X += _acceleration;
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
                if (otherPlayer != player
                    && !otherPlayer.HasFinished
                    && (otherPlayer.Position - player.Position).Length < 2 * ExamplePlayer.Radius)
                {
                    /*
                     * We overlap with the other player.
                     * This is only a collision if the players are moving towards each other, otherwise, let them move apart naturally.
                     */

                    var relativePosition = otherPlayer.Position - player.Position;
                    var relativeVelocity = otherPlayer.Velocity - player.Velocity;

                    /* The dot product tells us if the vectors are oriented towards each other. */
                    if (Vector.Dot(relativePosition, relativeVelocity) < 0)
                    {
                        /*
                         * The other player is moving towards us, relatively.
                         * Compute the result of a perfectly elastic condition.
                         */

                        player.Velocity = LimitSpeed(player.Velocity + relativeVelocity);
                        otherPlayer.Velocity = LimitSpeed(otherPlayer.Velocity - relativeVelocity);
                    }
                }
            }

            /* Check for the victory condition. */
            if ((player.Position - Graphics.Center).Length < ExamplePlayer.Tolerance)
            {
                ++_finishedPlayers;
                player.HasFinished = true;

                /* Finishing earlier is better. */
                player.Score = -_finishedPlayers;
            }
        }
    }
}
