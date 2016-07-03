// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.NanoSoccer
{
    internal class Goalpost : Circle
    {
        private double _startAngle;
        private double _endAngle;
        private WallType _wallType;

        private Vector _position;

        public Goalpost(Vector position, double radius, double startAngle, double endAngle, WallType type)
        {
            _position = position;
            Radius = radius;
            _startAngle = startAngle;
            _endAngle = endAngle;
            _wallType = type;
        }

        public Vector Position
        {
            get { return _position; }

            set
            {
                // do nothing;
            }
        }

        public Vector Velocity
        {
            get
            {
                return new Vector(0, 0);
            }

            set
            {
                // do nothing;
            }
        }

        public double Radius
        {
            get; private set;
        }

        public double MaximumVelocity
        {
            get
            {
                return 0;
            }
        }

        public double Mass
        {
            get
            {
                return double.PositiveInfinity;
            }
        }

        public void Draw(Graphics g)
        {
            Color color = Colors.White;
            switch (_wallType)
            {
                case WallType.SIDE:
                    color = new Color(0, 0.5, 0);
                    break;

                case WallType.LEFTGOAL:
                    color = new Color(1, 0, 0);
                    break;

                case WallType.RIGHTGOAL:
                    color = new Color(0, 0, 1);
                    break;
            }

            g.CircleSegment(color, Position, Radius, _startAngle, _endAngle);
        }
    }
}
