// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Snake
{
    internal static class Constants
    {
        public const int SpawnDistance = 7;

        public const int InitialLength = 3;

        public const int LengthGrowth = 3;

        public const double InitalSpeed = 0.15;

        public const double Acceleration = 0.00001;

        public static readonly Color WallColor = new Color(0.25, 0.25, 0.25);

        public static readonly Color AppleColor = new Color(0.35, 0.05, 0.0);

        public static readonly Color AppleLeafColor = new Color(0, 0.25, 0.0);
    }
}
