// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.FallingBlocks
{
    /// <summary>
    /// The player state of a falling blocks game.
    /// </summary>
    internal sealed class FallingBlocksPlayer : Player<FallingBlocksMatch>
    {
        private readonly bool[,] _isBlocked = new bool[Constants.Width, Constants.Height];

        private readonly Color[,] _color = new Color[Constants.Width, Constants.Height];

        public FallingBlocksPlayer LeftPlayer { get; set; }

        public FallingBlocksPlayer RightPlayer { get; set; }

        public void Initialize()
        {
            for (int y = Constants.Height - Constants.InitialGarbageLines; y < Constants.Height; ++y)
            {
                for (int x = 0; x < Constants.Width; ++x)
                {
                    if (Match.Random.NextDouble() < Constants.GarbageFillFactor)
                    {
                        _isBlocked[x, y] = true;
                        _color[x, y] = Color;
                    }
                }
            }
        }

        public void DrawScreen()
        {
            Draw(Graphics, default(Vector));
            LeftPlayer?.Draw(Graphics, new Vector(-100, 0));
            RightPlayer?.Draw(Graphics, new Vector(100, 0));
        }

        private void Draw(Graphics graphics, Vector offset)
        {
            if (graphics != Graphics)
            {
                graphics.PrintCenter(Color, 8, new Vector(160, 16) + offset, Name);
            }

            graphics.Line(Constants.ContainerColor, Constants.TopLeft + offset + new Vector(-Constants.ContainerBorder, -Constants.ContainerBorder), Constants.TopLeft + offset + new Vector(-Constants.ContainerBorder, Constants.Height * Constants.BlockSize + Constants.ContainerBorder));
            graphics.Line(Constants.ContainerColor, Constants.TopLeft + offset + new Vector(Constants.Width * Constants.BlockSize + Constants.ContainerBorder, -Constants.ContainerBorder), Constants.TopLeft + offset + new Vector(Constants.Width * Constants.BlockSize + Constants.ContainerBorder, Constants.Height * Constants.BlockSize + Constants.ContainerBorder));
            graphics.Line(Constants.ContainerColor, Constants.TopLeft + offset + new Vector(-Constants.ContainerBorder, Constants.Height * Constants.BlockSize + Constants.ContainerBorder), Constants.TopLeft + offset + new Vector(Constants.Width * Constants.BlockSize + Constants.ContainerBorder, Constants.Height * Constants.BlockSize + Constants.ContainerBorder));

            for (int x = 0; x < Constants.Width; ++x)
            {
                for (int y = 0; y < Constants.Height; ++y)
                {
                    if (_isBlocked[x, y])
                    {
                        graphics.Rectangle(
                            _color[x, y],
                            Constants.TopLeft + offset + new Vector(x * Constants.BlockSize + Constants.BlockBorder, y * Constants.BlockSize + Constants.BlockBorder),
                            Constants.TopLeft + offset + new Vector((x + 1) * Constants.BlockSize - Constants.BlockBorder, (y + 1) * Constants.BlockSize - Constants.BlockBorder));
                    }
                }
            }
        }
    }
}
