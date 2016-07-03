// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.NanoSoccer
{
    internal class Field
    {
        private const double _goalPostRadius = 5;
        private static readonly Vector _topLeftEdge = new Vector(30, 10);
        private static readonly Vector _topRightEdge = new Vector(290, 10);
        private static readonly Vector _bottomRightEdge = new Vector(290, 190);
        private static readonly Vector _bottomLeftEdge = new Vector(30, 190);
        private static readonly Vector _leftGoalTop = new Vector(30, 70);
        private static readonly Vector _leftGoalBottom = new Vector(30, 130);
        private static readonly Vector _rightGoalTop = new Vector(290, 70);
        private static readonly Vector _rightGoalBottom = new Vector(290, 130);

        private static readonly Goalpost _rightGoalTopPost = new Goalpost(
            new Vector(_rightGoalTop.X + _goalPostRadius, _rightGoalTop.Y - _goalPostRadius),
            _goalPostRadius, Math.PI / 2d, Math.PI, WallType.RIGHTGOAL);

        private static readonly Goalpost _rightGoalBottomPost = new Goalpost(
            new Vector(_rightGoalBottom.X + _goalPostRadius, _rightGoalBottom.Y + _goalPostRadius),
            _goalPostRadius, Math.PI, 3 * Math.PI / 2d, WallType.RIGHTGOAL);

        private static readonly Goalpost _leftGoalBottomPost = new Goalpost(
            new Vector(_leftGoalBottom.X - _goalPostRadius, _leftGoalBottom.Y + _goalPostRadius),
            _goalPostRadius, 3 * Math.PI / 2d, 2 * Math.PI, WallType.LEFTGOAL);

        private static readonly Goalpost _leftGoalTopPost = new Goalpost(
            new Vector(_leftGoalTop.X - _goalPostRadius, _leftGoalTop.Y - _goalPostRadius),
            _goalPostRadius, 0, Math.PI / 2d, WallType.LEFTGOAL);

        private List<Wall> _walls = new List<Wall>();
        private List<Goalpost> _posts = new List<Goalpost>();

        private NanoSoccerMatch _match;

        public Field(NanoSoccerMatch match)
        {
            _match = match;
            InitializeField();
        }

        public double LeftGoalX { get { return _leftGoalTop.X; } }

        public double RightGoalX { get { return _rightGoalTop.X; } }

        public void CheckCollisions(Circle circle)
        {
            /* Check for collisions with walls. */
            foreach (var w in _walls)
            {
                _match.CalculateWallCollision(w, circle);
            }

            /* Check for collisions with goal edges. */
            foreach (var p in _posts)
            {
                _match.CalculateCircleCollision(p, circle);
            }
        }

        public void Draw(Graphics g)
        {
            /* walls */
            foreach (var w in _walls)
            {
                w.Draw(g);
            }
            foreach (var p in _posts)
            {
                p.Draw(g);
            }
        }

        private void InitializeField()
        {
            /* top wall */
            _walls.Add(new Wall(_topLeftEdge, _topRightEdge - _topLeftEdge, WallType.SIDE));

            /* right wall, upper part */
            _walls.Add(new Wall(_topRightEdge, _rightGoalTop - new Vector(0, _goalPostRadius) - _topRightEdge, WallType.SIDE));

            /* right goal inner part */
            Vector rightGoalTopLeftCorner = _rightGoalTop + new Vector(_goalPostRadius, 0);
            Vector rightGoalTopRightCorner = new Vector(320, rightGoalTopLeftCorner.Y);
            Vector rightGoalBottomRightCorner = rightGoalTopRightCorner + new Vector(0, 60);
            Vector rightGoalBottomLeftCorner = rightGoalTopLeftCorner + new Vector(0, 60);
            _walls.Add(new Wall(rightGoalTopLeftCorner, rightGoalTopRightCorner - rightGoalTopLeftCorner, WallType.RIGHTGOAL));
            _walls.Add(new Wall(rightGoalTopRightCorner, rightGoalBottomRightCorner - rightGoalTopRightCorner, WallType.RIGHTGOAL));
            _walls.Add(new Wall(rightGoalBottomRightCorner, rightGoalBottomLeftCorner - rightGoalBottomRightCorner, WallType.RIGHTGOAL));

            /* right boundary, lower half */
            Vector _rightBoundaryLowerStart = _rightGoalBottom + new Vector(0, _goalPostRadius);
            _walls.Add(new Wall(_rightBoundaryLowerStart, _bottomRightEdge - _rightBoundaryLowerStart, WallType.SIDE));

            /* bottom boundary */
            _walls.Add(new Wall(_bottomRightEdge, _bottomLeftEdge - _bottomRightEdge, WallType.SIDE));

            /* left boundary, lower half */
            _walls.Add(new Wall(_bottomLeftEdge, _leftGoalBottom + new Vector(0, _goalPostRadius) - _bottomLeftEdge, WallType.SIDE));

            /* left goal inner part */
            Vector leftGoalBottomRightCorner = _leftGoalBottom - new Vector(_goalPostRadius, 0);
            Vector leftGoalBottomLeftCorner = new Vector(0, leftGoalBottomRightCorner.Y);
            Vector leftGoalTopLeftCorner = leftGoalBottomLeftCorner - new Vector(0, 60);
            Vector leftGoalTopRightCorner = leftGoalBottomRightCorner - new Vector(0, 60);
            _walls.Add(new Wall(leftGoalBottomRightCorner, leftGoalBottomLeftCorner - leftGoalBottomRightCorner, WallType.LEFTGOAL));
            _walls.Add(new Wall(leftGoalBottomLeftCorner, leftGoalTopLeftCorner - leftGoalBottomLeftCorner, WallType.LEFTGOAL));
            _walls.Add(new Wall(leftGoalTopLeftCorner, leftGoalTopRightCorner - leftGoalTopLeftCorner, WallType.LEFTGOAL));

            /* left boundary, upper half */
            Vector _leftBoundaryUpperStart = _leftGoalTop - new Vector(0, _goalPostRadius);
            _walls.Add(new Wall(_leftBoundaryUpperStart, _topLeftEdge - _leftBoundaryUpperStart, WallType.SIDE));

            _posts.Add(_leftGoalTopPost);
            _posts.Add(_leftGoalBottomPost);
            _posts.Add(_rightGoalTopPost);
            _posts.Add(_rightGoalBottomPost);
        }
    }
}
