﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.KartRace
{
    class KartMatch : Match<KartPlayer>
    {
        public readonly List<TrackLine> TrackLines = new List<TrackLine>();

        private readonly double[] _offsets = new double[Constants.TrackAmplitudes.Length];

        protected override void Initialize()
        {
            for (int i = 0; i < Constants.TrackAmplitudes.Length; ++i)
            {
                _offsets[i] = Random.NextDouble() * 2 * Math.PI;
            }

            double sf = Math.PI * 2 / Constants.TrackSegments;
            double lf = sf / Constants.TrackSegmentLines;
            for (int s = 0; s < Constants.TrackSegments; ++s)
            {
                double sa = s * sf;
                var sc = Constants.TrackSegmentColors[s % Constants.TrackSegmentColors.Length];

                for (int l = 0; l < Constants.TrackSegmentLines; ++l)
                {
                    double a1 = sa + l * lf;

                    double r1 = GetTrackRadius(a1);
                    double cos1 = Math.Cos(a1);
                    double sin1 = Math.Sin(a1);

                    double a2 = a1 + lf;
                    double r2 = GetTrackRadius(a2);
                    double cos2 = Math.Cos(a2);
                    double sin2 = Math.Sin(a2);

                    TrackLines.Add(
                        new TrackLine(
                            sc,
                            new Vector(cos1 * r1, sin1 * r1),
                            new Vector(cos2 * r2, sin2 * r2)));

                    r1 += Constants.TrackWidth;
                    r2 += Constants.TrackWidth;

                    TrackLines.Add(
                        new TrackLine(
                            sc,
                            new Vector(cos1 * r1, sin1 * r1),
                            new Vector(cos2 * r2, sin2 * r2)));
                }
            }

            var startPos = new Vector(Constants.TrackWidth * 0.5 + GetTrackRadius(0), 0);

            for (int i = 0; i < Players.Count; ++i)
            {
                Players[i].Position = startPos + i * new Vector(0, 1);
                Players[i].Direction = Rotation.Quarter;
            }
        }

        protected override void Update()
        {
            foreach (var player in Players)
            {
                if (player.Input.Left.IsPressed)
                {
                    player.Direction *= Constants.TurnSpeed;
                }

                if (player.Input.Right.IsPressed)
                {
                    player.Direction *= Constants.TurnSpeed.Inverse;
                }

                if (player.Input.Up.IsPressed)
                {
                    player.Speed += Constants.Acceleration;
                    if (player.Speed > Constants.MaxSpeed)
                    {
                        player.Speed = Constants.MaxSpeed;
                    }
                }

                if (player.Input.Down.IsPressed)
                {
                    player.Speed -= Constants.Brake;
                    if (player.Speed < 0)
                    {
                        player.Speed = 0;
                    }
                }

                player.Position += player.Speed * (player.Direction * new Vector(1, 0));
            }

            foreach (var player in Players)
            {
                player.Render();
            }
        }

        private double GetTrackRadius(double angle)
        {
            double r = Constants.TrackRadius;
            for (int i = 0; i < Constants.TrackAmplitudes.Length; ++i)
            {
                r += Math.Sin(angle * (i + 1) + _offsets[i]) * Constants.TrackAmplitudes[i];
            }

            return Math.Max(Constants.MinTrackRadius, r);
        }
    }
}
