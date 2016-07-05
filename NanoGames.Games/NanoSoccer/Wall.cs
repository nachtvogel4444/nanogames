// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.NanoSoccer
{
    internal enum WallType
    {
        SIDE,
        LEFTGOAL,
        RIGHTGOAL
    }

    internal class Wall
    {
        private WallType _type;

        public Wall(Vector origin, Vector length, WallType type)
        {
            Length = length;
            Origin = origin;
            _type = type;
        }

        public Vector Origin
        {
            get; set;
        }

        public Vector Length
        {
            get; set;
        }

        public void Draw(IGraphics g)
        {
            Color color = Colors.White;
            switch (_type)
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

            g.Line(color, Origin, Origin + Length);
        }
    }
}
