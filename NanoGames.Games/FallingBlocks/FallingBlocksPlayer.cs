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

        /// <inheritdoc/>
        public override void Update()
        {
            Graphics.Line(Constants.ContainerColor, Constants.TopLeft + new Vector(-Constants.ContainerBorder, -Constants.ContainerBorder), Constants.TopLeft + new Vector(-Constants.ContainerBorder, Constants.Height * Constants.BlockSize + Constants.ContainerBorder));
            Graphics.Line(Constants.ContainerColor, Constants.TopLeft + new Vector(Constants.Width * Constants.BlockSize + Constants.ContainerBorder, -Constants.ContainerBorder), Constants.TopLeft + new Vector(Constants.Width * Constants.BlockSize + Constants.ContainerBorder, Constants.Height * Constants.BlockSize + Constants.ContainerBorder));
            Graphics.Line(Constants.ContainerColor, Constants.TopLeft + new Vector(-Constants.ContainerBorder, Constants.Height * Constants.BlockSize + Constants.ContainerBorder), Constants.TopLeft + new Vector(Constants.Width * Constants.BlockSize + Constants.ContainerBorder, Constants.Height * Constants.BlockSize + Constants.ContainerBorder));

            for (int x = 0; x < Constants.Width; ++x)
            {
                for (int y = 0; y < Constants.Height; ++y)
                {
                    if (_isBlocked[x, y])
                    {
                        Graphics.Rectangle(
                            _color[x, y],
                            Constants.TopLeft + new Vector(x * Constants.BlockSize + Constants.BlockBorder, y * Constants.BlockSize + Constants.BlockBorder),
                            Constants.TopLeft + new Vector((x + 1) * Constants.BlockSize - Constants.BlockBorder, (y + 1) * Constants.BlockSize - Constants.BlockBorder));
                    }
                }
            }
        }
    }
}
