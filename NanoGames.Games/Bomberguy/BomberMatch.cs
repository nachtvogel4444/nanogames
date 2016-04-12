// Copyright (c) the authors of nanoGames. All rights reserved.

// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Bomberguy
{
    internal class BomberMatch : Match<BomberGuy>
    {
        public const int FIELD_SIZE = 15;
        public const double PLAYER_RATIO = .9;
        public const double BOMB_RATIO = 1;
        public const int BOMB_REACH = 3;
        public const double SPEED = 0.35;

        private double _pixelsPerUnit;
        private double _widthOffset;
        private BomberThing[,] _field = new BomberThing[FIELD_SIZE, FIELD_SIZE];
        private int _deadPlayers;

        public int DeadPlayers
        {
            get
            {
                return _deadPlayers;
            }

            set
            {
                _deadPlayers = value;
                CheckCompleted();
            }
        }

        public Vector CellSize
        {
            get { return new Vector(_pixelsPerUnit, _pixelsPerUnit); }
        }

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

        public Vector GetCell(BomberThing thing)
        {
            var c = (thing.Center.X - _widthOffset) / _pixelsPerUnit;
            var r = thing.Center.Y / _pixelsPerUnit;
            return new Vector(Math.Floor(r), Math.Floor(c));
        }

        public Vector GetCell(Vector position)
        {
            var c = (position.X - _widthOffset) / _pixelsPerUnit;
            var r = position.Y / _pixelsPerUnit;
            return new Vector(Math.Floor(r), Math.Floor(c));
        }

        public Vector GetCoordinates(Vector cellCoordinates)
        {
            return new Vector(_widthOffset + cellCoordinates.Y * _pixelsPerUnit, cellCoordinates.X * _pixelsPerUnit);
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
                        _field[r, c] = new Unbombable(this, new Vector(_widthOffset + c * _pixelsPerUnit, r * _pixelsPerUnit), new Vector(_pixelsPerUnit, _pixelsPerUnit));
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
                //var cell = GetCell(p);
                //this[cell] = p;

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

                p.Draw(p.Graphics);

                DropBomb(p);

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

        private void CheckCompleted()
        {
            this.IsCompleted = DeadPlayers == Players.Count;
        }

        private void DropBomb(BomberGuy p)
        {
            if (!p.Input.Fire) return;

            var cell = GetCell(p);

            var bomb = new Bomb(BOMB_REACH, this, GetCoordinates(cell), new Vector(_pixelsPerUnit * BOMB_RATIO, _pixelsPerUnit * BOMB_RATIO));

            this[cell] = bomb;
        }

        private void MovePlayer(BomberGuy p)
        {
            double x = 0, y = 0;

            if (p.Input.Up && !p.Input.Down)
            {
                var neighborLeft = this[GetCell(p.Position + new Vector(0, -SPEED))];
                if (neighborLeft != null)
                {
                    var xDistance = p.Center.X - (neighborLeft.Position + neighborLeft.Size).X;
                    var yDistance = p.Center.Y - (neighborLeft.Position + neighborLeft.Size).Y;

                    if (yDistance < 0 || (yDistance > 0 && (yDistance / p.Size.Y + xDistance / p.Size.X) > 0.55))
                    {
                        neighborLeft = null;
                    }
                }

                var neighborRight = this[GetCell(p.Position + new Vector(p.Size.X, -SPEED))];
                if (neighborRight != null)
                {
                    var xDistance = (neighborRight.Position).X - p.Center.X;
                    var yDistance = p.Center.Y - (neighborRight.Position + neighborRight.Size).Y;

                    if (yDistance < 0 || (yDistance > 0 && (yDistance / p.Size.Y + xDistance / p.Size.X) > 0.55))
                    {
                        neighborRight = null;
                    }
                }
                if ((neighborLeft == null || neighborLeft.Passable) && (neighborRight == null || neighborRight.Passable))
                {
                    y = -1;
                }
            }
            if (p.Input.Down && !p.Input.Up)
            {
                var neighborLeft = this[GetCell(p.Position + p.Size + new Vector(-p.Size.X, SPEED))];
                if (neighborLeft != null)
                {
                    var xDistance = p.Center.X - (neighborLeft.Position + neighborLeft.Size).X;
                    var yDistance = (neighborLeft.Position).Y - p.Center.Y;

                    if (yDistance < 0 || (yDistance > 0 && (yDistance / p.Size.Y + xDistance / p.Size.X) > 0.55))
                    {
                        neighborLeft = null;
                    }
                }

                var neighborRight = this[GetCell(p.Position + p.Size + new Vector(0, SPEED))];
                if (neighborRight != null)
                {
                    var xDistance = (neighborRight.Position).X - p.Center.X;
                    var yDistance = (neighborRight.Position).Y - p.Center.Y;

                    if (yDistance < 0 || (yDistance > 0 && (yDistance / p.Size.Y + xDistance / p.Size.X) > 0.55))
                    {
                        neighborRight = null;
                    }
                }
                if ((neighborLeft == null || neighborLeft.Passable) && (neighborRight == null || neighborRight.Passable))
                {
                    y = 1;
                }
            }
            if (p.Input.Left && !p.Input.Right)
            {
                var neighborAbove = this[GetCell(p.Position + new Vector(-SPEED, 0))];

                if (neighborAbove != null)
                {
                    var xDistance = p.Center.X - (neighborAbove.Position + neighborAbove.Size).X;
                    var yDistance = p.Center.Y - (neighborAbove.Position + neighborAbove.Size).Y;

                    if (xDistance < 0 || (xDistance > 0 && (yDistance / p.Size.Y + xDistance / p.Size.X) > 0.55))
                    {
                        neighborAbove = null;
                    }
                }

                var neighborBelow = this[GetCell(p.Position + new Vector(-SPEED, p.Size.Y))];

                if (neighborBelow != null)
                {
                    var xDistance = p.Center.X - (neighborBelow.Position + neighborBelow.Size).X;
                    var yDistance = (neighborBelow.Position).Y - p.Center.Y;

                    if (xDistance < 0 || (xDistance > 0 && (yDistance / p.Size.Y + xDistance / p.Size.X) > 0.55))
                    {
                        neighborBelow = null;
                    }
                }

                if ((neighborAbove == null || neighborAbove.Passable) && (neighborBelow == null || neighborBelow.Passable))
                {
                    x = -1;
                }
            }
            if (p.Input.Right && !p.Input.Left)
            {
                var neighborAbove = this[GetCell(p.Position + new Vector(p.Size.X + SPEED, 0))];

                if (neighborAbove != null)
                {
                    var xDistance = (neighborAbove.Position).X - p.Center.X;
                    var yDistance = p.Center.Y - (neighborAbove.Position + neighborAbove.Size).Y;

                    if (xDistance < 0 || (xDistance > 0 && (yDistance / p.Size.Y + xDistance / p.Size.X) > 0.55))
                    {
                        neighborAbove = null;
                    }
                }

                var neighborBelow = this[GetCell(p.Position + new Vector(p.Size.X + SPEED, p.Size.Y))];

                if (neighborBelow != null)
                {
                    var xDistance = (neighborBelow.Position).X - p.Center.X;
                    var yDistance = (neighborBelow.Position).Y - p.Center.Y;

                    if (xDistance < 0 || (xDistance > 0 && (yDistance / p.Size.Y + xDistance / p.Size.X) > 0.55))
                    {
                        neighborBelow = null;
                    }
                }
                if ((neighborAbove == null || neighborAbove.Passable) && (neighborBelow == null || neighborBelow.Passable))
                {
                    x = 1;
                }
            }

            if (x == 0 && y == 0) return;

            var direction = new Vector(x, y).Normalized;

            p.Position += direction * SPEED;
        }
    }
}
