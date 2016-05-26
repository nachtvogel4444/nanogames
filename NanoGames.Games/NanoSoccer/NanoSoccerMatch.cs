// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.NanoSoccer
{
    internal class NanoSoccerMatch : Match<NanoSoccerPlayer>
    {
        private const int _stepsPerFrame = 10;
        private const double _acceleration = 0.001;
        private const double _maxSpeed = 0.2;
        private const double _maxBallSpeed = 0.6;
        private const double _goalEdgeRadius = 5;
        private const double _frictionFactor = 0.99;
        private const double _ballFrictionFactor = 0.997;
        private const int _leftGoalX = 30;
        private const int _rightGoalX = 290;
        private static readonly Vector _topLeftEdge = new Vector(30, 10);
        private static readonly Vector _topRightEdge = new Vector(290, 10);
        private static readonly Vector _rightGoalTopEdge = new Vector(_rightGoalX, 70);
        private static readonly Vector _rightGoalBottomEdge = new Vector(_rightGoalX, 130);
        private static readonly Vector _bottomRightEdge = new Vector(290, 190);
        private static readonly Vector _bottomLeftEdge = new Vector(30, 190);
        private static readonly Vector _leftGoalBottomEdge = new Vector(_leftGoalX, 130);
        private static readonly Vector _leftGoalTopEdge = new Vector(_leftGoalX, 70);

        //private static readonly GoalEdge _rightGoalTopEdge = new GoalEdge(new Vector(295, 70), 5, Math.PI / 2d, Math.PI);
        //private static readonly GoalEdge _rightGoalBottomEdge = new GoalEdge(new Vector(295, 130), 5, Math.PI, 3 * Math.PI / 2d);
        //private static readonly GoalEdge _leftGoalBottomEdge = new GoalEdge(new Vector(25, 130), 5, 3 * Math.PI / 2d, 2 * Math.PI);
        //private static readonly GoalEdge _leftGoalTopEdge = new GoalEdge(new Vector(25, 70), 5, 0, Math.PI / 2d);

        private List<Wall> _walls = new List<Wall>();
        private Ball _ball;

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

            InitializeWalls();
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

            /* Draw the field boundaries */
            /* walls */
            foreach (var w in _walls)
            {
                w.Draw(Graphics);
            }
            ///* right goal top edge */
            //_rightGoalTopEdge.Draw(Graphics);
            ///* right goal bottom edge */
            //_rightGoalBottomEdge.Draw(Graphics);
            ///* left goal bottom edge */
            //_leftGoalBottomEdge.Draw(Graphics);
            ///* left goal top edge */
            //_leftGoalTopEdge.Draw(Graphics);

            foreach (var player in Players)
            {
                player.DrawScreen();
                _ball.Draw(player.Graphics);
            }

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
            if (_ball.Position.X + Ball.BallRadius < _leftGoalX)
            {
                /* Blue team (1) has won */
                foreach (var p in Players)
                {
                    if (p.Team == 1) p.Score = 1;
                }
                IsCompleted = true;
            }

            if (_ball.Position.X - Ball.BallRadius > _rightGoalX)
            {
                /* Red team (0) has won */
                foreach (var p in Players)
                {
                    if (p.Team == 0) p.Score = 1;
                }
                IsCompleted = true;
            }
        }

        private void InitializeWalls()
        {
            /* top wall */
            _walls.Add(new Wall(_topLeftEdge, _topRightEdge - _topLeftEdge, WallType.SIDE));
            /* right wall, upper part */
            //_walls.Add(new Wall(_topRightEdge, _rightGoalTopEdge.Position - new Vector(_rightGoalTopEdge.Radius, 0) - _topRightEdge));
            _walls.Add(new Wall(_topRightEdge, _rightGoalTopEdge - _topRightEdge, WallType.SIDE));

            /* right goal inner part */
            //Vector rightGoalTopLeftCorner = _rightGoalTopEdge.Position + new Vector(0, _rightGoalTopEdge.Radius);
            Vector rightGoalTopLeftCorner = _rightGoalTopEdge;
            Vector rightGoalTopRightCorner = new Vector(320, rightGoalTopLeftCorner.Y);
            Vector rightGoalBottomRightCorner = rightGoalTopRightCorner + new Vector(0, 60);
            Vector rightGoalBottomLeftCorner = rightGoalTopLeftCorner + new Vector(0, 60);
            _walls.Add(new Wall(rightGoalTopLeftCorner, rightGoalTopRightCorner - rightGoalTopLeftCorner, WallType.RIGHTGOAL));
            _walls.Add(new Wall(rightGoalTopRightCorner, rightGoalBottomRightCorner - rightGoalTopRightCorner, WallType.RIGHTGOAL));
            _walls.Add(new Wall(rightGoalBottomRightCorner, rightGoalBottomLeftCorner - rightGoalBottomRightCorner, WallType.RIGHTGOAL));
            /* right boundary, lower half */
            //Vector rightLowerBoundaryStart = _rightGoalBottomEdge.Position - new Vector(_rightGoalTopEdge.Radius, 0);
            //_walls.Add(new Wall(rightLowerBoundaryStart, _bottomRightEdge - rightLowerBoundaryStart));
            _walls.Add(new Wall(_rightGoalBottomEdge, _bottomRightEdge - _rightGoalBottomEdge, WallType.SIDE));
            /* bottom boundary */
            _walls.Add(new Wall(_bottomRightEdge, _bottomLeftEdge - _bottomRightEdge, WallType.SIDE));
            /* left boundary, lower half */
            //Vector leftLowerBoundaryEnd = _leftGoalBottomEdge.Position + new Vector(_leftGoalBottomEdge.Radius, 0);
            //_walls.Add(new Wall(_bottomLeftEdge, leftLowerBoundaryEnd - _bottomLeftEdge));
            _walls.Add(new Wall(_bottomLeftEdge, _leftGoalBottomEdge - _bottomLeftEdge, WallType.SIDE));
            /* left goal inner part */
            //Vector leftGoalBottomRightCorner = _leftGoalBottomEdge.Position - new Vector(0, _rightGoalTopEdge.Radius);
            Vector leftGoalBottomRightCorner = _leftGoalBottomEdge;
            Vector leftGoalBottomLeftCorner = new Vector(0, leftGoalBottomRightCorner.Y);
            Vector leftGoalTopLeftCorner = leftGoalBottomLeftCorner - new Vector(0, 60);
            Vector leftGoalTopRightCorner = leftGoalBottomRightCorner - new Vector(0, 60);
            _walls.Add(new Wall(leftGoalBottomRightCorner, leftGoalBottomLeftCorner - leftGoalBottomRightCorner, WallType.LEFTGOAL));
            _walls.Add(new Wall(leftGoalBottomLeftCorner, leftGoalTopLeftCorner - leftGoalBottomLeftCorner, WallType.LEFTGOAL));
            _walls.Add(new Wall(leftGoalTopLeftCorner, leftGoalTopRightCorner - leftGoalTopLeftCorner, WallType.LEFTGOAL));
            /* right boundary, upper half */
            //Vector leftUpperBoundaryStart = _leftGoalTopEdge.Position + new Vector(_leftGoalTopEdge.Radius, 0);
            //_walls.Add(new Wall(leftUpperBoundaryStart, _topLeftEdge - leftUpperBoundaryStart));
            _walls.Add(new Wall(_leftGoalTopEdge, _topLeftEdge - _leftGoalTopEdge, WallType.SIDE));
        }

        private void MovePlayer(NanoSoccerPlayer player)
        {
            bool accelerating = false;

            if (player.Input.Up)
            {
                accelerating = true;
                player.Velocity -= new Vector(0, _acceleration);
            }

            if (player.Input.Down)
            {
                accelerating = true;
                player.Velocity += new Vector(0, _acceleration);
            }

            if (player.Input.Left)
            {
                accelerating = true;
                player.Velocity -= new Vector(_acceleration, 0);
            }

            if (player.Input.Right)
            {
                accelerating = true;
                player.Velocity += new Vector(_acceleration, 0);
            }

            /* Cap the speed at the maximum. */
            player.Velocity = LimitSpeed(player.Velocity, _maxSpeed);

            /* Decelerate by friction */
            if (!accelerating)
            {
                player.Velocity *= _frictionFactor;
            }
            /* Move the player by their velocity. */
            player.Position += player.Velocity;

            /* Check for collisions with walls. */
            foreach (var w in _walls)
            {
                CalculateWallCollision(w, player);
            }

            /* Check for collisions with goal edges. */
            //if ((_rightGoalTopEdge.Position - player.Position).Length < NanoSoccerPlayer.Radius + _rightGoalTopEdge.Radius)
            //{
            //    var relativePosition = _rightGoalTopEdge.Position - player.Position;
            //    var relativeVelocity = -player.Velocity;

            //    /* The dot product tells us if the vectors are oriented towards each other. */
            //    if (Vector.Dot(relativePosition, relativeVelocity) < 0)
            //    {
            //        /*
            //         * The other player is moving towards us, relatively.
            //         * Compute the result of a perfectly elastic condition.
            //         */

            //        player.Velocity = LimitSpeed(player.Velocity + 2 * relativeVelocity);
            //    }
            //}

            /* Check for collisions with other players. */
            foreach (var otherPlayer in Players)
            {
                if (otherPlayer != player
                    && (otherPlayer.Position - player.Position).Length < 2 * NanoSoccerPlayer.PlayerRadius)
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

                        player.Velocity = LimitSpeed(player.Velocity + relativeVelocity, _maxSpeed);
                        otherPlayer.Velocity = LimitSpeed(otherPlayer.Velocity - relativeVelocity, _maxSpeed);
                    }
                }
            }

            /* Check for collision with ball */

            if ((_ball.Position - player.Position).Length < Ball.BallRadius + NanoSoccerPlayer.PlayerRadius)
            {
                /*
                 * We overlap with the ball.
                 * This is only a collision if the objects are moving towards each other, otherwise, let them move apart naturally.
                 */

                var relativePosition = _ball.Position - player.Position;
                var relativeVelocity = _ball.Velocity - player.Velocity;

                /* The dot product tells us if the vectors are oriented towards each other. */
                if (Vector.Dot(relativePosition, relativeVelocity) < 0)
                {
                    /*
                     * The ball is moving towards us, relatively.
                     * Compute the result of a perfectly elastic condition.
                     */

                    player.Velocity = LimitSpeed(player.Velocity + relativeVelocity, _maxSpeed);
                    _ball.Velocity = LimitSpeed(_ball.Velocity - 2 * relativeVelocity, _maxBallSpeed);
                }
            }
        }

        private void MoveBall()
        {
            /* Decelerate by friction */

            _ball.Velocity *= _ballFrictionFactor;

            /* Move the ball by their velocity. */
            _ball.Position += _ball.Velocity;

            /* Check for collisions with walls. */
            foreach (var w in _walls)
            {
                CalculateWallCollision(w, _ball);
            }
        }

        private void CalculateWallCollision(Wall wall, Circle circle)
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
    }
}
