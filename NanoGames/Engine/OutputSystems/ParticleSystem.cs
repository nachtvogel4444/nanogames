// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Engine.OutputSystems
{
    /// <summary>
    /// Implements a particle system.
    /// </summary>
    internal sealed class ParticleSystem : IParticleSystem
    {
        private const double _maxLifetime = 120;

        private readonly Random _random = new Random();

        private readonly List<Particle> _particles = new List<Particle>();

        private double _frame;

        /// <inheritdoc/>
        public Vector Gravity { get; set; }

        /// <inheritdoc/>
        public Vector Velocity { get; set; }

        /// <inheritdoc/>
        public double MeanDistance { get; set; } = 2;

        /// <inheritdoc/>
        public double Intensity { get; set; } = 2;

        /// <summary>
        /// Sets the current frame index.
        /// </summary>
        /// <param name="frame">The frame index.</param>
        public void SetFrame(int frame)
        {
            _frame = frame;
        }

        /// <summary>
        /// Renders the particles.
        /// </summary>
        /// <param name="frame">The frame index. Can be fractional.</param>
        /// <param name="graphics">The graphics to render to.</param>
        public void Render(double frame, IGraphics graphics)
        {
            for (int i = 0; i < _particles.Count; ++i)
            {
                var particle = _particles[i];

                var t = frame - particle.Frame;

                if (t >= _maxLifetime)
                {
                    _particles.RemoveAt(i);
                    --i;
                    continue;
                }

                var v = particle.Velocity + t * particle.Gravity;
                var p = particle.Position + t * particle.Velocity + (0.5 * t * t) * particle.Gravity;

                var vhalf = 0.5 * v;
                graphics.Line((1 - t / _maxLifetime) * particle.Color, p - vhalf, p + vhalf);
            }
        }

        /// <inheritdoc/>
        public void Line(Color color, Vector start, Vector end)
        {
            var length = (end - start).Length;
            var distance = MeanDistance / length;

            if (distance <= 0)
            {
                return;
            }

            for (double p = 0; p < 1; p += distance)
            {
                var position = Vector.Mix(start, end, p);
                CreateParticle(color, position);
            }
        }

        /// <inheritdoc/>
        public void Point(Color color, Vector vector)
        {
            CreateParticle(color, vector);
        }

        private void CreateParticle(Color color, Vector position)
        {
            _particles.Add(new Particle
            {
                Position = position,
                Velocity = Velocity + Intensity * Vector.FromAngle(_random.NextDouble() * 2 * Math.PI),
                Gravity = Gravity,
                Color = color + new Color(0.5, 0.5, 0.5),
                Frame = _frame,
            });
        }

        private sealed class Particle
        {
            public Vector Position;

            public Vector Velocity;

            public Vector Gravity;

            public Color Color;

            public double Frame;
        }
    }
}
