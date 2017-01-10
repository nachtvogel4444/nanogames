﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.KartRace
{
    class Constants
    {
        public const double Acceleration = 0.002;
        
        public const double Brake = 0.01;

        public const double MaxSpeed = 0.5;

        public const double TrackRadius =  64;

        public const double MinTrackRadius = 32;

        public const double TrackSegments = 90;

        public const double TrackSegmentLines = 2;

        public const double TrackWidth = 6;

        public static readonly Color[] TrackSegmentColors = new[] { new Color(0.8, 0, 0), new Color(0.7, 0.7, 0.7) };

        public static readonly double[] TrackAmplitudes = new double[] { 3, 2, 2, 2, 2, 1, 4 };

        public static readonly Rotation TurnSpeed = Rotation.FromDegrees(1);

        public static readonly double CameraOffset = 2.5;

        public static readonly double NearOffset = 0.1;

        public static readonly Color HorizonColor = new Color(0, 0.5, 0);
    }
}
