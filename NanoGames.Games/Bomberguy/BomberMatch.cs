// Copyright (c) the authors of nanoGames. All rights reserved.

// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Bomberguy
{
    internal class BomberMatch : Match<BomberGuy>
    {
        public const int FIELD_SIZE = 15;
        public const double PLAYER_RATIO = 4d / 5d;
        public const double SPEED = 0.4;

        private double _pixelsPerUnit;
        private double _widthOffset;
        private BomberThing[,] _field = new BomberThing[FIELD_SIZE, FIELD_SIZE];

        public BomberThing this[Vector v]
        {
            get
            {
                return this[(int)v.X, (int)v.Y];
            }

            set
            {
                this[(int)v.X, (int)v.Y] = value;
            }
        }

        public BomberThing this[int x, int y]
        {
            get
            {
                return _field[x, y];
            }

            set
            {
                _field[x, y] = value;
            }
        }

        protected override void Initialize()
        {
            _pixelsPerUnit = Graphics.Height / FIELD_SIZE;
            _widthOffset = (Graphics.Width - Graphics.Height) / 2d;

            // initialize static obstacles
            for (int r = 0; r < FIELD_SIZE; r++)
            {
                for (int c = 0; c < FIELD_SIZE; c++)
                {
                    if (r == 0 || r == FIELD_SIZE - 1 || c == 0 || c == FIELD_SIZE - 1 || r % 2 == 0 && c % 2 == 0)
                    {
                        _field[r, c] = new Unbombable(new Vector(_widthOffset + c * _pixelsPerUnit, r * _pixelsPerUnit), new Vector(_pixelsPerUnit, _pixelsPerUnit));
                    }
                }
            }

            // initialize players
            double distance = FIELD_SIZE - 2;
            int count = 0;
            Vector pos = new Vector(_widthOffset + Math.Ceiling(_pixelsPerUnit), Math.Ceiling(_pixelsPerUnit));
            Vector direction = new Vector(1, 0);
            foreach (BomberGuy p in this.Players)
            {
                p.Size = new Vector(_pixelsPerUnit * PLAYER_RATIO, _pixelsPerUnit * PLAYER_RATIO);
                p.Position = pos;
                var cell = GetCell(p);
                this[cell] = p;

                direction = direction.RotatedRight;

                pos = pos + direction * (distance * _pixelsPerUnit);

                count++;
                if (count % 4 == 0)
                    distance = System.Math.Floor(distance / 2d);
            }
        }

        protected override void Update()
        {
            foreach (var p in this.Players)
            {
                MovePlayer(p);

                for (int r = 0; r < FIELD_SIZE; r++)
                {
                    for (int c = 0; c < FIELD_SIZE; c++)
                    {
                        BomberThing thing = _field[r, c];

                        if (thing != null)
                            thing.Draw(p.Graphics);
                    }
                }
            }
        }

        private void MovePlayer(BomberGuy p)
        {
            double x = 0, y = 0;

            if (p.Input.Up && !p.Input.Down)
            {
                var neighbor1 = this[GetCell(p.Position + new Vector(0, -SPEED))];
                var neighbor2 = this[GetCell(p.Position + new Vector(p.Size.X, -SPEED))];
                if ((neighbor1 == null || neighbor1.Passable) && (neighbor2 == null || neighbor2.Passable))
                {
                    y = -1;
                }
            }
            if (p.Input.Down && !p.Input.Up)
            {
                var neighbor1 = this[GetCell(p.Position + p.Size + new Vector(-p.Size.X, SPEED))];
                var neighbor2 = this[GetCell(p.Position + p.Size + new Vector(0, SPEED))];
                if ((neighbor1 == null || neighbor1.Passable) && (neighbor2 == null || neighbor2.Passable))
                {
                    y = 1;
                }
            }
            if (p.Input.Left && !p.Input.Right)
            {
                var neighbor1 = this[GetCell(p.Position + new Vector(-SPEED, 0))];

                if (neighbor1 != null)
                {
                    var yDistance = p.Center.Y - (neighbor1.Position + neighbor1.Size).Y;
                    var xDistance = p.Center.X - (neighbor1.Position + neighbor1.Size).X;

                    if (yDistance > 0 && (yDistance / p.Size.Y + xDistance / p.Size.X) > 0.5)
                    {
                        neighbor1 = null;
                    }
                }

                var neighbor2 = this[GetCell(p.Position + new Vector(-SPEED, p.Size.Y))];

                if (neighbor2 != null)
                {
                    var yDistance2 = (neighbor2.Position + neighbor2.Size).Y - p.Center.Y;
                    var xDistance2 = (neighbor2.Position + neighbor2.Size).X - p.Center.X;

                    if (yDistance2 / p.Size.Y < xDistance2 / p.Size.X)
                    {
                        neighbor2 = null;
                    }
                }

                if ((neighbor1 == null || neighbor1.Passable) && (neighbor2 == null || neighbor2.Passable))
                {
                    x = -1;
                }
            }
            if (p.Input.Right && !p.Input.Left)
            {
                var neighbor1 = this[GetCell(p.Position + p.Size + new Vector(SPEED, -p.Size.Y))];
                var neighbor2 = this[GetCell(p.Position + p.Size + new Vector(SPEED, 0))];
                if ((neighbor1 == null || neighbor1.Passable) && (neighbor2 == null || neighbor2.Passable))
                {
                    x = 1;
                }
            }

            if (x == 0 && y == 0) return;

            var direction = new Vector(x, y).Normalized;

            p.Position += direction * SPEED;
        }

        private Vector GetCell(BomberThing thing)
        {
            var c = (thing.Center.X - _widthOffset) / _pixelsPerUnit;
            var r = thing.Center.Y / _pixelsPerUnit;
            return new Vector(Math.Floor(c), Math.Floor(r));
        }

        private Vector GetCell(Vector position)
        {
            var c = (position.X - _widthOffset) / _pixelsPerUnit;
            var r = position.Y / _pixelsPerUnit;
            return new Vector(Math.Floor(c), Math.Floor(r));
        }
    }
}
