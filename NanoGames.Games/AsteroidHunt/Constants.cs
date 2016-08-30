using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.AsteroidHunt
{
    class Constants
    {
        public static class Match
        {
            public static readonly double InitialAsteroidRatePerPlayer = 1;
            public static readonly double AsteroidRateIncreasePerPlayer = 1d / 600d;
            public static readonly double ScoreFontSize = 6;
        }

        public static class Simploid
        {
            public static readonly double Probability = 0.7;
            public static readonly double MinVelocity = 0.2;
            public static readonly double MaxVelocity = 0.8;
            public static readonly double MinSize = 5;
            public static readonly double MaxSize = 15;
            public static readonly int BaseScore = 1;
            public static readonly int MinHitpoints = 1;
            public static readonly int MaxHitpoints = 2;
        }
        public static class Player
        {
            public static readonly Vector Size = new Vector(6, 10);
            public static readonly double Velocity = 2;
        }

        public static class Weapon
        {
            public static readonly double FireRate = 4;
            public static readonly double BulletVelocity = 3;
            public static readonly double BulletLength = 3;
        }
    }
}
