// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Example
{
    internal class ExampleMatch : Match<ExamplePlayer>
    {
        private const double _acceleration = 0.1;
        private const double _maxSpeed = 10;

        private int _finishedPlayers;

        protected override void Initialize()
        {
            /* This is called by the framework at the start of every match. */

            /* Arrange the players in a circle. */
            double start = Random.NextDouble() * 2 * Math.PI;
            for (int i = 0; i < Players.Count; ++i)
            {
                double angle = start + i * 2 * Math.PI / Players.Count;
                Players[i].Position = new Vector(160 + 90 * Math.Cos(angle), 90 + 60 * Math.Sin(angle));
            }

            /* After this, ExamplePlayer.Initialize is called by the framework for every individual player. */
        }

        protected override void Update()
        {
            /*
             * This is called by the framework once every frame.
             * We can either draw onto every player's graphics interface here, or do that in ExamplePlayer.Update.
             */

            foreach (var player in Players)
            {
                MovePlayer(player);
            }

            if (_finishedPlayers == Players.Count)
            {
                IsCompleted = true;
            }

            /* After this, ExamplePlayer.Update is called by the framework for every individual player. */
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
                player.Velocity.Y += _acceleration;
            }

            if (player.Input.Down)
            {
                player.Velocity.Y -= _acceleration;
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
            if (player.Velocity.Length > _maxSpeed)
            {
                player.Velocity *= _maxSpeed / player.Velocity.Length;
            }

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
                player.Position.Y += 180;
            }

            if (player.Position.Y > 180)
            {
                player.Position.Y -= 180;
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
