// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Snake
{
    internal class SnakePlayer : Player<SnakeMatch>
    {
        private Direction _nextDirection = Direction.None;

        public SegmentBuffer Segments { get; } = new SegmentBuffer();

        public int DesiredLength { get; set; } = Constants.InitialLength;

        public Position HeadPosition => Segments[0];

        public Position TailPosition => Segments[Segments.Count - 1];

        public Direction Direction { get; set; } = Direction.Right;

        public bool IsAlive { get; private set; } = true;

        public void Update()
        {
            if (!IsAlive)
            {
                return;
            }

            Score = Match.Frame;

            var direction = GetInputDirection();
            if (direction != Direction.None && (int)direction != -(int)Direction)
            {
                _nextDirection = direction;
            }

            if (Match.IsMovementFrame)
            {
                if (_nextDirection != Direction.None)
                {
                    Direction = _nextDirection;
                    _nextDirection = Direction.None;
                }

                var headPosition = HeadPosition;
                switch (Direction)
                {
                    case Direction.Up:
                        headPosition.Y -= 1;
                        break;

                    case Direction.Down:
                        headPosition.Y += 1;
                        break;

                    case Direction.Left:
                        headPosition.X -= 1;
                        break;

                    case Direction.Right:
                        headPosition.X += 1;
                        break;
                }

                if (headPosition.X < 0
                    || headPosition.X >= Match.Width
                    || headPosition.Y < 0
                    || headPosition.Y >= Match.Height
                    || Match.IsOccupied[headPosition.X, headPosition.Y])
                {
                    for (int i = 0; i < Segments.Count; ++i)
                    {
                        var position = Segments[i];
                        Match.IsOccupied[position.X, position.Y] = false;
                    }

                    IsAlive = false;
                    return;
                }
                else
                {
                    AddSegment(headPosition);

                    if (headPosition.X == Match.ApplePosition.X && headPosition.Y == Match.ApplePosition.Y)
                    {
                        Match.ApplePosition = new Position(-1, -1);
                        DesiredLength += Constants.LengthGrowth;
                    }
                }
            }
        }

        public void AddSegment(Position position)
        {
            if (Segments.Count < DesiredLength)
            {
                Segments.Add(position);
            }
            else
            {
                var tailPosition = TailPosition;
                Match.IsOccupied[tailPosition.X, tailPosition.Y] = false;
                Segments.Rotate(position);
            }

            Match.IsOccupied[position.X, position.Y] = true;
        }

        private Direction GetInputDirection()
        {
            if (Input.Up.IsPressed)
            {
                if (!Input.Down.IsPressed && !Input.Left.IsPressed && !Input.Right.IsPressed)
                {
                    return Direction.Up;
                }
            }
            else if (Input.Down.IsPressed)
            {
                if (!Input.Left.IsPressed && !Input.Right.IsPressed)
                {
                    return Direction.Down;
                }
            }
            else if (Input.Left.IsPressed)
            {
                if (!Input.Right.IsPressed)
                {
                    return Direction.Left;
                }
            }
            else if (Input.Right.IsPressed)
            {
                return Direction.Right;
            }

            return Direction.None;
        }
    }
}
