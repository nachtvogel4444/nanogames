using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.KartRace
{
    class Constants
    {
        public const double Acceleration = 0.001;

        public const double MaxSpeed = 0.05;

        public static readonly Rotation TurnSpeed = Rotation.FromDegrees(1);

        public static readonly double CameraOffset = 2.5;

        public static readonly double NearOffset = 0.1;
    }
}
