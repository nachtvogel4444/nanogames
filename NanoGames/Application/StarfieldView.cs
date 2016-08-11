// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NanoGames.Application
{
    /// <summary>
    /// A view showing a moving starfield.
    /// </summary>
    internal sealed class StarfieldView : IView
    {
        private const double _starsPerInterval = 4096;
        private readonly Random _random = new Random();
        private readonly List<Star> _stars = new List<Star>();
        private readonly List<int> _freeIndexes = new List<int>();
        private long _lastTimestamp = 0;

        private long _lastStarTimestamp = 0;

        /// <summary>
        /// Gets or sets the velocity.
        /// </summary>
        public double Velocity { get; set; } = 0.25;

        /// <summary>
        /// Gets or sets the "warp" factor, which makes the star trails appear longer.
        /// </summary>
        public double Warp { get; set; } = 1.0;

        /// <inheritdoc/>
        public void Update(Terminal terminal)
        {
            if (_stars.Count == 0)
            {
                double starDistanceInterval = 1 / _starsPerInterval;
                for (double z = 1; z > 0; z -= starDistanceInterval)
                {
                    var star = new Star();
                    CreateStar(star);
                    star.Z = z;
                    _stars.Add(star);
                }
            }

            var currentTimestamp = Stopwatch.GetTimestamp();
            var deltaTimestamp = currentTimestamp - _lastTimestamp;
            _lastTimestamp = currentTimestamp;
            if (currentTimestamp == deltaTimestamp)
            {
                _lastStarTimestamp = currentTimestamp;
                return;
            }

            var deltaTime = deltaTimestamp / (double)Stopwatch.Frequency;

            for (int i = 0; i < _stars.Count; ++i)
            {
                var star = _stars[i];

                if (!star.IsUsed)
                {
                    continue;
                }

                var oldZ = star.Z;
                var z = oldZ - Velocity * deltaTime;

                if (z < 0)
                {
                    _stars[i].IsUsed = false;
                    _freeIndexes.Add(i);
                    z = 0.001;
                }

                star.Z = z;
                var x = star.X;
                var y = star.Y;

                var c = 0.33 * (1 - oldZ);

                terminal.Graphics.Line(
                    new Color(c, c, c),
                    GetScreenVector(x, y, z + Velocity / 32 * Warp),
                    GetScreenVector(x, y, z));
            }

            double starsPerSecond = Velocity * _starsPerInterval;
            long starInterval = (long)(Stopwatch.Frequency / starsPerSecond);

            while (currentTimestamp > _lastStarTimestamp + starInterval)
            {
                _lastStarTimestamp += starInterval;

                int f = _freeIndexes.Count - 1;
                if (f >= 0)
                {
                    CreateStar(_stars[_freeIndexes[f]]);
                    _freeIndexes.RemoveAt(f);
                }
                else
                {
                    var star = new Star();
                    CreateStar(star);
                    _stars.Add(star);
                }
            }
        }

        private void CreateStar(Star star)
        {
            star.IsUsed = true;
            star.X = _random.NextDouble() * 2 - 1;
            star.Y = _random.NextDouble() * 2 - 1;
            star.Z = 1;
        }

        private Vector GetScreenVector(double x, double y, double z)
        {
            return new Vector((x / z) * 0.5 * GraphicsConstants.Width + 0.5 * GraphicsConstants.Width, (y / z) * 0.5 * GraphicsConstants.Width + 0.5 * GraphicsConstants.Height);
        }

        private sealed class Star
        {
            public bool IsUsed;
            public double X;
            public double Y;
            public double Z;
        }
    }
}
