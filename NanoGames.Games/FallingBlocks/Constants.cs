// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.FallingBlocks
{
    /// <summary>
    /// Constants.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The playing field width, in blocks.
        /// </summary>
        public const int Width = 10;

        /// <summary>
        /// The playeing field height, in blocks.
        /// </summary>
        public const int Height = 20;

        /// <summary>
        /// The height of the extra area of the playing field where the blocks spawn.
        /// </summary>
        public const int ExtraHeight = 2;

        /// <summary>
        /// How many garbage lines to fill initially.
        /// </summary>
        public const int InitialGarbageLines = 2;

        /// <summary>
        /// How much of the garbage is filled statistically.
        /// </summary>
        public const double GarbageFillFactor = 0.5;

        /// <summary>
        /// The size of individual blocks.
        /// </summary>
        public const double BlockSize = 7.5;

        /// <summary>
        /// The size of the border around blocks.
        /// </summary>
        public const double BlockBorder = 0.75;

        /// <summary>
        /// The size of the inner border of the container.
        /// </summary>
        public const double ContainerBorder = 1;

        /// <summary>
        /// The color of the container.
        /// </summary>
        public static readonly Color ContainerColor = new Color(1, 1, 1);

        /// <summary>
        /// The color of falling blocks.
        /// </summary>
        public static readonly Color FallingBlockColor = new Color(1, 1, 1);

        /// <summary>
        /// The bottom left corner of the playing field on the screen.
        /// </summary>
        public static readonly Vector BottomLeft = new Vector(0.5 * (Terminal.Width - BlockSize * Width), 0.5 * (Terminal.Height - BlockSize * (Height + ExtraHeight)));
    }
}
