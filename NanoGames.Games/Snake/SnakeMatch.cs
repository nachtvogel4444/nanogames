// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Linq;

namespace NanoGames.Games.Snake
{
    internal class SnakeMatch : Match<SnakePlayer>
    {
        public bool[,] IsOccupied;

        public Position ApplePosition;

        private double _scale, _xOffset, _yOffset;

        private double _movementSpeed;

        private double _movementFrames;

        public int Width { get; private set; }

        public int Height { get; private set; }

        public bool IsMovementFrame { get; set; }

        protected override void Initialize()
        {
            _movementSpeed = Constants.InitalSpeed;


            Width = 65;
            Height = 41;

            IsOccupied = new bool[Width, Height];

            _scale = Math.Min(318.0 / Width, 198.0 / Height);
            _xOffset = 0.5 * (320 - Width * _scale);
            _yOffset = 0.5 * (200 - Height * _scale);

            ApplePosition = new Position((short)(Width / 2), (short)(Height / 2));

            int spawnBorderWidth = Width - 2 * Constants.SpawnDistance - 1;
            int spawnBorderHeight = Height - 2 * Constants.SpawnDistance - 1;
            int spawnBorderSize = 2 * spawnBorderWidth + 2 * spawnBorderHeight;

            int spawnOffset = 0;
            foreach (var player in Players)
            {
                Position spawnPosition;
                Direction spawnDirection;

                if (spawnOffset < spawnBorderWidth)
                {
                    spawnPosition = new Position((short)(Constants.SpawnDistance + spawnOffset), Constants.SpawnDistance);
                    spawnDirection = Direction.Right;
                }
                else if (spawnOffset < spawnBorderWidth + spawnBorderHeight)
                {
                    spawnPosition = new Position((short)(Width - Constants.SpawnDistance), (short)(Constants.SpawnDistance + spawnOffset - spawnBorderWidth));
                    spawnDirection = Direction.Down;
                }
                else if (spawnOffset < 2 * spawnBorderWidth + spawnBorderHeight)
                {
                    spawnPosition = new Position((short)(Width - Constants.SpawnDistance - spawnOffset + spawnBorderWidth + spawnBorderHeight), (short)(Height - Constants.SpawnDistance));
                    spawnDirection = Direction.Left;
                }
                else
                {
                    spawnPosition = new Position(Constants.SpawnDistance, (short)(Height - Constants.SpawnDistance - spawnOffset + 2 * spawnBorderWidth + spawnBorderHeight));
                    spawnDirection = Direction.Up;
                }

                player.AddSegment(spawnPosition);
                player.Direction = spawnDirection;

                spawnOffset += spawnBorderSize / Players.Count;
            }
        }

        protected override void Update()
        {
            _movementSpeed += Constants.Acceleration;
            _movementFrames += _movementSpeed;

            if (_movementFrames < 1)
            {
                IsMovementFrame = false;
            }
            else
            {
                IsMovementFrame = true;
                _movementFrames -= 1;
            }

            if (ApplePosition.X < 0)
            {
                for (int i = 0; i < 100; ++i)
                {
                    var x = Random.Next(Width);
                    var y = Random.Next(Height);
                    if (!IsOccupied[x, y])
                    {
                        ApplePosition = new Position((short)x, (short)y);
                        break;
                    }
                }
            }

            foreach (var player in Players)
            {
                player.Update();
            }

            if ((Players.Count == 1 && !Players[0].IsAlive) || (Players.Count > 1 && Players.Count(p => p.IsAlive) == 1))
            {
                IsCompleted = true;
            }

            Output.Graphics.Line(Constants.WallColor, new Vector(_xOffset, _yOffset), new Vector(_xOffset + Width * _scale, _yOffset));
            Output.Graphics.Line(Constants.WallColor, new Vector(_xOffset + Width * _scale, _yOffset), new Vector(_xOffset + Width * _scale, _yOffset + Height * _scale));
            Output.Graphics.Line(Constants.WallColor, new Vector(_xOffset + Width * _scale, _yOffset + Height * _scale), new Vector(_xOffset, _yOffset + Height * _scale));
            Output.Graphics.Line(Constants.WallColor, new Vector(_xOffset, _yOffset + Height * _scale), new Vector(_xOffset, _yOffset));

            Output.Graphics.Circle(Constants.AppleColor, new Vector(_xOffset + _scale * (ApplePosition.X + 0.5), _yOffset + _scale * (ApplePosition.Y + 0.55)), 0.45 * _scale);
            Output.Graphics.Line(Constants.AppleLeafColor, new Vector(_xOffset + _scale * (ApplePosition.X + 0.5), _yOffset + _scale * (ApplePosition.Y + 0.3)), new Vector(_xOffset + _scale * (ApplePosition.X + 0.85), _yOffset + _scale * (ApplePosition.Y + 0.3)));
            Output.Graphics.Line(Constants.AppleLeafColor, new Vector(_xOffset + _scale * (ApplePosition.X + 0.85), _yOffset + _scale * (ApplePosition.Y + 0.3)), new Vector(_xOffset + _scale * (ApplePosition.X + 1.0), _yOffset + _scale * (ApplePosition.Y + 0.0)));
            Output.Graphics.Line(Constants.AppleLeafColor, new Vector(_xOffset + _scale * (ApplePosition.X + 1.0), _yOffset + _scale * (ApplePosition.Y + 0.0)), new Vector(_xOffset + _scale * (ApplePosition.X + 0.65), _yOffset + _scale * (ApplePosition.Y + 0.0)));
            Output.Graphics.Line(Constants.AppleLeafColor, new Vector(_xOffset + _scale * (ApplePosition.X + 0.65), _yOffset + _scale * (ApplePosition.Y + 0.0)), new Vector(_xOffset + _scale * (ApplePosition.X + 0.5), _yOffset + _scale * (ApplePosition.Y + 0.3)));

            foreach (var player in Players)
            {
                if (player.IsAlive)
                {
                    var segments = player.Segments;
                    for (int i = 0; i < segments.Count; ++i)
                    {
                        var r = i == 0 ? 0.5 : 0.3;
                        var p = segments[i];

                        Output.Graphics.Circle(0.75 * player.LocalColor, new Vector(_xOffset + _scale * (p.X + 0.5), _yOffset + _scale * (p.Y + 0.5)), r * _scale);
                    }
                }
            }
        }
    }
}
