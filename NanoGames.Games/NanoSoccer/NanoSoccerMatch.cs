// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.NanoSoccer
{
    internal class NanoSoccerMatch : Match<NanoSoccerPlayer>
    {
        public const double MaxPlayerVelocity = 0.2;
        public const double MaxBallVelocity = 1;
        public const double PlayerMass = 1;
        public const double BallMass = 0.01;
        private const int _stepsPerFrame = 10;
        private const double _acceleration = 0.001;

        private const double _frictionFactor = 0.99;
        private const double _ballFrictionFactor = 0.9975;

        private Field _field;
        private Ball _ball;

        public void CalculateCircleCollision(Circle c1, Circle c2)
        {
            if ((c1.Position - c2.Position).Length < c1.Radius + c2.Radius)
            {
                /*
                 * We overlap with the ball.
                 * This is only a collision if the objects are moving towards each other, otherwise, let them move apart naturally.
                 */

                var relativePosition = c1.Position - c2.Position;
                var relativeVelocity = c1.Velocity - c2.Velocity;

                /* The dot product tells us if the vectors are oriented towards each other. */
                if (Vector.Dot(relativePosition, relativeVelocity) < 0)
                {
                    /*
                     * The ball is moving towards us, relatively.
                     * Compute the result of a perfectly elastic condition.
                     */

                    var velocityExchange = Vector.Dot(relativeVelocity, relativePosition.Normalized) * relativePosition.Normalized;
                    var massRelation = c2.Mass / (c1.Mass + c2.Mass);
                    c2.Velocity = LimitSpeed(c2.Velocity + velocityExchange * 2 * (1 - massRelation), c2.MaximumVelocity);
                    c1.Velocity = LimitSpeed(c1.Velocity - velocityExchange * 2 * massRelation, c1.MaximumVelocity);
                }
            }
        }

        public void CalculateWallCollision(Wall wall, Circle circle)
        {
            Vector wallNormal = wall.Length.RotatedRight;
            Vector playerToWallOrigin = circle.Position - wall.Origin;
            Vector playerToWallEnd = circle.Position - (wall.Origin + wall.Length);
            Vector playerToWall = Vector.Dot(wallNormal, playerToWallOrigin) / (wallNormal.Length * wallNormal.Length) * wallNormal;
            double wallDistance = playerToWall.Length;

            if (wallDistance > circle.Radius)
                return;

            if (Vector.Dot(playerToWallOrigin, wall.Length) < 0)
            {
                return;
            }

            if (Vector.Dot(playerToWallEnd, wall.Length) > 0)
            {
                return;
            }

            circle.Position += wallNormal.Normalized * (circle.Radius - wallDistance);

            double angle = Math.Acos(Vector.Dot(wall.Length.Normalized, circle.Velocity.Normalized));
            if (angle < Math.PI)
            {
                circle.Velocity = circle.Velocity.Rotated(-(Math.PI + 2 * (Math.PI / 2d - angle)));
            }
        }

        protected override void Initialize()
        {
            /* This is called by the framework at the start of every match. */

            /* Arrange the players in a circle. */
            double start = Random.NextDouble() * 2 * Math.PI;
            for (int i = 0; i < Players.Count; ++i)
            {
                double angle = start + i * 2 * Math.PI / Players.Count;
                Players[i].Position = new Vector(160 + 66 * Math.Cos(angle), 100 + 66 * Math.Sin(angle));
                Players[i].Team = i % 2;
            }

            _field = new Field(this);
            _ball = new Ball();
            _ball.Position = new Vector(160, 100);
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

                MoveBall();
            }

            /* Draw the field */
            _field.Draw(Graphics);

            /* Draw the ball */
            _ball.Draw(Graphics);

            /* Draw all players */
            foreach (var player in Players)
            {
                player.DrawScreen();
            }

            /* Check victory condition */
            CheckCompleted();
        }

        private static Vector LimitSpeed(Vector velocity, double maxSpeed)
        {
            var speed = velocity.Length;
            if (speed > maxSpeed)
            {
                velocity *= maxSpeed / speed;
            }

            return velocity;
        }

        private void CheckCompleted()
        {
            if (_ball.Position.X + Ball.BallRadius < _field.LeftGoalX)
            {
                /* Blue team (1) has won */
                foreach (var p in Players)
                {
                    if (p.Team == 1) p.Score = 1;
                }
                IsCompleted = true;
            }

            if (_ball.Position.X - Ball.BallRadius > _field.RightGoalX)
            {
                /* Red team (0) has won */
                foreach (var p in Players)
                {
                    if (p.Team == 0) p.Score = 1;
                }
                IsCompleted = true;
            }
        }

        private void MovePlayer(NanoSoccerPlayer player)
        {
            bool accelerating = false;

            if (player.Input.Up.IsPressed)
            {
                accelerating = true;
                player.Velocity -= new Vector(0, _acceleration);
            }

            if (player.Input.Down.IsPressed)
            {
                accelerating = true;
                player.Velocity += new Vector(0, _acceleration);
            }

            if (player.Input.Left.IsPressed)
            {
                accelerating = true;
                player.Velocity -= new Vector(_acceleration, 0);
            }

            if (player.Input.Right.IsPressed)
            {
                accelerating = true;
                player.Velocity += new Vector(_acceleration, 0);
            }

            /* Cap the speed at the maximum. */
            player.Velocity = LimitSpeed(player.Velocity, MaxPlayerVelocity);

            /* Decelerate by friction */
            if (!accelerating)
            {
                player.Velocity *= _frictionFactor;
            }
            /* Move the player by their velocity. */
            player.Position += player.Velocity;

            /* Check for collisions with field boundaries. */
            _field.CheckCollisions(player);

            /* Check for collisions with other players. */
            foreach (var otherPlayer in Players)
            {
                if (otherPlayer != player)
                {
                    CalculateCircleCollision(player, otherPlayer);
                }
            }

            /* Check for collision with ball */
            CalculateCircleCollision(_ball, player);
        }

        private void MoveBall()
        {
            /* Decelerate by friction */

            _ball.Velocity *= _ballFrictionFactor;

            /* Move the ball by their velocity. */
            _ball.Position += _ball.Velocity;

            /* Check for collisions with walls. */
            _field.CheckCollisions(_ball);
        }
    }
}
