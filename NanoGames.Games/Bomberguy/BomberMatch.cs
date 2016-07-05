// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Bomberguy
{
    internal class BomberMatch : Match<BomberGuy>
    {
        public const int FIELD_MIN_SIZE = 9;
        public const double PLAYER_RATIO = .9;
        public const double BOMB_RATIO = 1;
        public const int BOMB_REACH = 2;
        public const double PLAYER_SPEED = 9;
        public const double BOMBSTACLE_PROBABILITY = 0.5;

        private int _fieldSize;
        private double _playerSpeed;
        private double _pixelsPerUnit;
        private double _widthOffset;
        private RectbombularThing[,] _field;

        public Vector CellSize
        {
            get { return new Vector(_pixelsPerUnit, _pixelsPerUnit); }
        }

        public RectbombularThing this[Vector v]
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

        public RectbombularThing this[int x, int y]
        {
            get
            {
                return _field[x, y];
            }

            set
            {
                _field[x, y] = value;
                if (value != null) value.Position = GetCoordinates(new Vector(x, y));
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

        public void CheckAllDeaths()
        {
            foreach (var p in Players)
            {
                CheckDeath(p);
            }
        }

        protected override void Initialize()
        {
            _fieldSize = FIELD_MIN_SIZE + ((int)(Players.Count / 4)) * 2;

            _field = new RectbombularThing[_fieldSize, _fieldSize];

            _playerSpeed = PLAYER_SPEED / _fieldSize;

            _pixelsPerUnit = GraphicsConstants.Height / _fieldSize;
            _widthOffset = (GraphicsConstants.Width - GraphicsConstants.Height) / 2d;

            // initialize all obstacles
            InitializeField();

            // initialize players
            InitializePlayers();
        }

        protected override void Update()
        {
            CheckCompleted();

            foreach (var p in this.Players)
            {
                DrawField(p);

                p.Draw(p.Output.Graphics);

                /* Skip players that have already finished. */
                if (!p.Alive) continue;

                p.Score = Frame;

                MovePlayer(p);

                DropBomb(p);

                CheckDeath(p);
            }
        }

        private void InitializeField()
        {
            for (int r = 0; r < _fieldSize; r++)
            {
                for (int c = 0; c < _fieldSize; c++)
                {
                    if (r == 0 || r == _fieldSize - 1 || c == 0 || c == _fieldSize - 1 || r % 2 == 0 && c % 2 == 0)
                    {
                        //_field[r, c] = new Unbombable(this, new Vector(_widthOffset + c * _pixelsPerUnit, r * _pixelsPerUnit), new Vector(_pixelsPerUnit, _pixelsPerUnit));
                        this[r, c] = new Unbombable(this, CellSize);
                    }
                    else
                    {
                        if (this.Random.NextDouble() <= BOMBSTACLE_PROBABILITY)
                        {
                            this[r, c] = new Bombstacle(this, CellSize);
                        }
                    }
                }
            }
        }

        private void InitializePlayers()
        {
            int playersPerSide = (int)Math.Ceiling(Players.Count / 4d);
            BomberGuy[,] playerArray = new BomberGuy[4, playersPerSide];
            int side = 0;
            int playerCount = 0;
            foreach (BomberGuy p in Players)
            {
                playerArray[side, playerCount] = p;
                if (++side > 3)
                {
                    side = 0;
                    playerCount++;
                }
            }

            Vector direction = new Vector(1, 0);
            int distance = (int)Math.Floor((double)(_fieldSize - 2) / playersPerSide);
            Vector currPos = new Vector(1, 1);
            for (int i = 0; i < 4; i++)
            {
                if (i == 1) currPos = new Vector(_fieldSize - 2, 1);
                if (i == 2) currPos = new Vector(_fieldSize - 2, _fieldSize - 2);
                if (i == 3) currPos = new Vector(1, _fieldSize - 2);

                for (int j = 0; j < playersPerSide; j++)
                {
                    var p = playerArray[i, j];
                    if (p == null) continue;
                    p.Size = new Vector(_pixelsPerUnit * PLAYER_RATIO, _pixelsPerUnit * PLAYER_RATIO);

                    var cellCoordinates = new Vector(currPos.Y, currPos.X);

                    MakeSpaceAroundPlayer(cellCoordinates);

                    p.Position = GetCoordinates(cellCoordinates);
                    currPos += direction * distance;
                }

                direction = direction.RotatedRight;
            }
        }

        private void MakeSpaceAroundPlayer(Vector cellCoordinates)
        {
            var cellContent = this[cellCoordinates];
            if (cellContent != null && cellContent.Destroyable) cellContent.Destroy();

            var directionVector = new Vector(1, 0);

            for (int i = 0; i < 4; i++)
            {
                cellContent = this[cellCoordinates + directionVector];
                if (cellContent != null && cellContent.Destroyable) cellContent.Destroy();
                directionVector = directionVector.RotatedRight;
            }
        }

        private void DrawField(BomberGuy p)
        {
            for (int r = 0; r < _fieldSize; r++)
            {
                for (int c = 0; c < _fieldSize; c++)
                {
                    BomberThing thing = _field[r, c];

                    if (thing != null)
                        thing.Draw(p.Output.Graphics);
                }
            }
        }

        private void MovePlayer(BomberGuy p)
        {
            double x = 0, y = 0;

            Vector inputVector = GetInputVector(p.Input);
            inputVector = inputVector.Normalized;

            if (p.Input.Up.IsPressed && !p.Input.Down.IsPressed)
            {
                var neighborLeft = this[GetCell(p.Position + new Vector(0, _playerSpeed * inputVector.Y))];
                bool neighborLeftPassable = neighborLeft == null;
                bool slideToRightAllowed = false;
                if (neighborLeft != null && !(neighborLeftPassable = neighborLeft.Passable))
                {
                    var xDistance = p.Center.X - neighborLeft.BottomRight.X;
                    var yDistance = p.Center.Y - neighborLeft.BottomRight.Y;

                    DetermineMovement(out neighborLeftPassable, out slideToRightAllowed, yDistance, xDistance, p.Size.Y, p.Size.X);
                }

                var neighborRight = this[GetCell(p.Position + new Vector(p.Size.X, _playerSpeed * inputVector.Y))];
                bool neighborRightPassable = neighborRight == null;
                bool slideToLeftAllowed = false;
                if (neighborRight != null && !(neighborRightPassable = neighborRight.Passable))
                {
                    neighborRightPassable = neighborRight.Passable;
                    var xDistance = neighborRight.BottomLeft.X - p.Center.X;
                    var yDistance = p.Center.Y - neighborRight.BottomRight.Y;

                    DetermineMovement(out neighborRightPassable, out slideToLeftAllowed, yDistance, xDistance, p.Size.Y, p.Size.X);
                }
                if (neighborLeftPassable && neighborRightPassable)
                {
                    y = -1;
                    x = slideToRightAllowed ? 1 : slideToLeftAllowed ? -1 : x;
                }
            }
            if (p.Input.Down.IsPressed && !p.Input.Up.IsPressed)
            {
                var neighborLeft = this[GetCell(p.Position + p.Size + new Vector(-p.Size.X, _playerSpeed * inputVector.Y))];
                bool neighborLeftPassable = neighborLeft == null;
                bool slideToRightAllowed = false;
                if (neighborLeft != null && !(neighborLeftPassable = neighborLeft.Passable))
                {
                    neighborLeftPassable = neighborLeft.Passable;
                    var xDistance = p.Center.X - neighborLeft.TopRight.X;
                    var yDistance = neighborLeft.TopRight.Y - p.Center.Y;

                    DetermineMovement(out neighborLeftPassable, out slideToRightAllowed, yDistance, xDistance, p.Size.Y, p.Size.X);
                }

                var neighborRight = this[GetCell(p.Position + p.Size + new Vector(0, _playerSpeed * inputVector.Y))];
                bool neighborRightPassable = neighborRight == null;
                bool slideToLeftAllowed = false;
                if (neighborRight != null && !(neighborRightPassable = neighborRight.Passable))
                {
                    var xDistance = neighborRight.TopLeft.X - p.Center.X;
                    var yDistance = neighborRight.TopLeft.Y - p.Center.Y;

                    DetermineMovement(out neighborRightPassable, out slideToLeftAllowed, yDistance, xDistance, p.Size.Y, p.Size.X);
                }
                if (neighborLeftPassable && neighborRightPassable)
                {
                    y = 1;
                    x = slideToRightAllowed ? 1 : slideToLeftAllowed ? -1 : x;
                }
            }
            if (p.Input.Left.IsPressed && !p.Input.Right.IsPressed)
            {
                var neighborAbove = this[GetCell(p.Position + new Vector(_playerSpeed * inputVector.X, 0))];
                bool neighborAbovePassable = neighborAbove == null;
                bool slideToDownAllowed = false;
                if (neighborAbove != null && !(neighborAbovePassable = neighborAbove.Passable))
                {
                    var xDistance = p.Center.X - neighborAbove.BottomRight.X;
                    var yDistance = p.Center.Y - neighborAbove.BottomRight.Y;

                    DetermineMovement(out neighborAbovePassable, out slideToDownAllowed, xDistance, yDistance, p.Size.X, p.Size.Y);
                }

                var neighborBelow = this[GetCell(p.Position + new Vector(_playerSpeed * inputVector.X, p.Size.Y))];
                bool neighborBelowPassable = neighborBelow == null;
                bool slideToUpAllowed = false;
                if (neighborBelow != null && !(neighborBelowPassable = neighborBelow.Passable))
                {
                    var xDistance = p.Center.X - neighborBelow.TopRight.X;
                    var yDistance = neighborBelow.TopRight.Y - p.Center.Y;

                    DetermineMovement(out neighborBelowPassable, out slideToUpAllowed, xDistance, yDistance, p.Size.X, p.Size.Y);
                }

                if (neighborAbovePassable && neighborBelowPassable)
                {
                    x = -1;
                    y = slideToDownAllowed ? 1 : slideToUpAllowed ? -1 : y;
                }
            }
            if (p.Input.Right.IsPressed && !p.Input.Left.IsPressed)
            {
                var neighborAbove = this[GetCell(p.Position + new Vector(p.Size.X + _playerSpeed * inputVector.X, 0))];
                bool neighborAbovePassable = neighborAbove == null;
                bool slideToDownAllowed = false;
                if (neighborAbove != null && !(neighborAbovePassable = neighborAbove.Passable))
                {
                    var xDistance = neighborAbove.BottomLeft.X - p.Center.X;
                    var yDistance = p.Center.Y - neighborAbove.BottomLeft.Y;

                    DetermineMovement(out neighborAbovePassable, out slideToDownAllowed, xDistance, yDistance, p.Size.X, p.Size.Y);
                }

                var neighborBelow = this[GetCell(p.Position + new Vector(p.Size.X + _playerSpeed * inputVector.X, p.Size.Y))];
                bool neighborBelowPassable = neighborBelow == null;
                bool slideToUpAllowed = false;
                if (neighborBelow != null && !(neighborBelowPassable = neighborBelow.Passable))
                {
                    var xDistance = neighborBelow.TopLeft.X - p.Center.X;
                    var yDistance = neighborBelow.TopLeft.Y - p.Center.Y;

                    DetermineMovement(out neighborBelowPassable, out slideToUpAllowed, xDistance, yDistance, p.Size.X, p.Size.Y);
                }
                if (neighborAbovePassable && neighborBelowPassable)
                {
                    x = 1;
                    y = slideToDownAllowed ? 1 : slideToUpAllowed ? -1 : y;
                }
            }

            if (x == 0 && y == 0) return;

            var direction = new Vector(x, y).Normalized;

            p.Position += direction * _playerSpeed;
        }

        private Vector GetInputVector(Input input)
        {
            Vector v = new Vector();
            if (input.Up.IsPressed && !input.Down.IsPressed) v.Y = -1;
            if (input.Down.IsPressed && !input.Up.IsPressed) v.Y = 1;
            if (input.Left.IsPressed && !input.Right.IsPressed) v.X = -1;
            if (input.Right.IsPressed && !input.Left.IsPressed) v.X = 1;

            return v;
        }

        private void DetermineMovement(out bool neighborPassable, out bool slideAllowed, double movementDirectionDistance, double sideDistance, double movementDirectionSize, double sideSize)
        {
            neighborPassable = false;
            slideAllowed = false;
            if (movementDirectionDistance < 0 || (movementDirectionDistance > 0 && (movementDirectionDistance / movementDirectionSize + sideDistance / sideSize) > 0.55))
            {
                neighborPassable = true;
                if (sideSize / 2d - sideDistance < 3d)
                {
                    slideAllowed = true;
                }
            }
        }

        private void DropBomb(BomberGuy p)
        {
            if (!p.Input.Fire.WasActivated) return;

            var cell = GetCell(p);

            var bomb = new Bomb(BOMB_REACH, p, this, GetCoordinates(cell), new Vector(_pixelsPerUnit * BOMB_RATIO, _pixelsPerUnit * BOMB_RATIO));

            this[cell] = bomb;
        }

        private void CheckDeath(BomberGuy p)
        {
            var cell = this[GetCell(p)];

            if (cell != null && cell.Deadly) p.Destroy();
        }

        private void CheckCompleted()
        {
            int deadPlayers = 0;
            foreach (var p in Players)
            {
                if (p.Dead)
                {
                    deadPlayers++;
                }
            }

            this.IsCompleted = deadPlayers == Players.Count || (Players.Count >= 2 && deadPlayers >= Players.Count - 1);
        }
    }
}
