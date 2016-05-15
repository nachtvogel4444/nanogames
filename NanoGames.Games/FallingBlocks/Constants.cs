// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.FallingBlocks
{
    /// <summary>
    /// Constants.
    /// </summary>
    internal static class Constants
    {
        public const double InitialFallSpeed = 1;

        public const double FallAcceleration = 0.05;

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
        /// The top-left corner of the playing field on the screen.
        /// </summary>
        public static readonly Vector TopLeft = new Vector(0.5 * (Graphics.Width - BlockSize * Width), 0.5 * (Graphics.Height - BlockSize * (Height - ExtraHeight)));

        public static readonly byte[][][,] RotatedPieces;

        private static readonly byte[][,] _rawPieces = new byte[][,]
        {
            new byte[,]
            {
                { 1, 0, 0 },
                { 1, 1, 1 },
            },
            new byte[,]
            {
                { 0, 0, 1 },
                { 1, 1, 1 },
            },
            new byte[,]
            {
                { 1, 1 },
                { 1, 1 },
            },
            new byte[,]
            {
                { 1, 1, 1, 1 },
            },
            new byte[,]
            {
                { 0, 1, 1 },
                { 1, 1, 0 },
            },
            new byte[,]
            {
                { 1, 1, 0 },
                { 0, 1, 1 },
            },
            new byte[,]
            {
                { 0, 1, 0 },
                { 1, 1, 1 },
            },
        };

        static Constants()
        {
            RotatedPieces = new byte[_rawPieces.Length][][,];

            for (int i = 0; i < _rawPieces.Length; ++i)
            {
                var rotatedPiece = RotatedPieces[i] = new byte[4][,];
                var piece = rotatedPiece[0] = _rawPieces[i];
                var w = piece.GetLength(0);
                var h = piece.GetLength(1);
                var piece90 = rotatedPiece[1] = new byte[h, w];
                var piece180 = rotatedPiece[2] = new byte[w, h];
                var piece270 = rotatedPiece[3] = new byte[h, w];

                for (int x = 0; x < w; ++x)
                {
                    for (int y = 0; y < h; ++y)
                    {
                        var v = piece[x, y];
                        piece90[h - y - 1, x] = v;
                        piece180[w - x - 1, h - y - 1] = v;
                        piece270[y, w - x - 1] = v;
                    }
                }
            }
        }
    }
}
