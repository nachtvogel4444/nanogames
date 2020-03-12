// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


namespace NanoGames.Games.Cluster
{
    internal static class Constants
    {
        public const double Viscosity = 5;
        public const double dt = 1.0 / 60;
        public const double bounceFactor = 0.7;

        public const bool Debug = false;

        public static class World
        {
            public const double SigmaX = 3000.0 / 3.0;
            public const double SigmaRatio = 1.0 / 3.0;

            public const double DensityOfStars = 0.00005;

            public const int NumberOfPlanets = 15;
            public const double MaxR = 100;
            public const double C = 0.6;
            public const double MinD = 20;
            
            public const double SigmaY = SigmaX * SigmaRatio;
        }

        public static class LBeam
        {
            public const double Speed = 500;
            public const double Length = 10;
            public const int FireTime = 10;
        }

        public static class Planet
        {
            public const double VoronoiDensity = 1.0 / (20.0 * 20.0);
            public const double VoronoiError = 0.1;
            public const double VoronoiStep = 0.01;
            public const double epsilon = 0.1;
        }
    }
}
