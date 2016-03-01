// Copyright (c) the authors of NanoGames. All rights reserved.
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

        /// <inheritdoc/>
        public override void Initialize()
        {
            for (int y = 0; y < Constants.Height && y < Constants.InitialGarbageLines; ++y)
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

        /// <inheritdoc/>
        public override void Update()
        {
            var bottomLeft = Constants.BottomLeft;

            Terminal.Line(Constants.ContainerColor, bottomLeft + new Vector(-Constants.ContainerBorder, -Constants.ContainerBorder), bottomLeft + new Vector(-Constants.ContainerBorder, Constants.Height * Constants.BlockSize + Constants.ContainerBorder));
            Terminal.Line(Constants.ContainerColor, bottomLeft + new Vector(Constants.Width * Constants.BlockSize + Constants.ContainerBorder, -Constants.ContainerBorder), bottomLeft + new Vector(Constants.Width * Constants.BlockSize + Constants.ContainerBorder, Constants.Height * Constants.BlockSize + Constants.ContainerBorder));
            Terminal.Line(Constants.ContainerColor, bottomLeft + new Vector(-Constants.ContainerBorder, -Constants.ContainerBorder), bottomLeft + new Vector(Constants.Width * Constants.BlockSize + Constants.ContainerBorder, -Constants.ContainerBorder));

            for (int x = 0; x < Constants.Width; ++x)
            {
                for (int y = 0; y < Constants.Height; ++y)
                {
                    if (_isBlocked[x, y])
                    {
                        var color = _color[x, y];
                        var blockBottomLeft = Constants.BottomLeft + new Vector(x * Constants.BlockSize, y * Constants.BlockSize);

                        var a = blockBottomLeft + new Vector(Constants.BlockBorder, Constants.BlockBorder);
                        var b = blockBottomLeft + new Vector(Constants.BlockSize - Constants.BlockBorder, Constants.BlockBorder);
                        var c = blockBottomLeft + new Vector(Constants.BlockSize - Constants.BlockBorder, Constants.BlockSize - Constants.BlockBorder);
                        var d = blockBottomLeft + new Vector(Constants.BlockBorder, Constants.BlockSize - Constants.BlockBorder);

                        Terminal.Line(color, a, b);
                        Terminal.Line(color, b, c);
                        Terminal.Line(color, c, d);
                        Terminal.Line(color, d, a);
                    }
                }
            }
        }
    }
}
