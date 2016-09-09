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
        private const int _maxParticles = 10000;

        private const double _fadeOutFrames = 6;

        private readonly Random _random = new Random();

        private readonly List<Particle> _particles = new List<Particle>();

        private double _lastFrame;

        private double _frame;

        /// <inheritdoc/>
        public Vector Gravity { get; set; }

        /// <inheritdoc/>
        public Vector Velocity { get; set; }

        /// <inheritdoc/>
        public double Frequency { get; set; } = 1;

        /// <inheritdoc/>
        public double Intensity { get; set; } = 2;

        /// <inheritdoc/>
        public double Lifetime { get; set; } = 30;

        /// <summary>
        /// Sets the current frame index.
        /// </summary>
        /// <param name="frame">The frame index.</param>
        public void SetFrame(double frame)
        {
            _lastFrame = _frame;
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

                if (t >= particle.Lifetime)
                {
                    _particles.RemoveAt(i);
                    --i;
                    continue;
                }

                var v = particle.Velocity + t * particle.Gravity;
                var p = particle.Position + t * particle.Velocity + (0.5 * t * t) * particle.Gravity;

                var vhalf = 0.5 * v;

                double glow = Math.Min(1, (particle.Lifetime - t) / _fadeOutFrames);

                graphics.Line(glow * particle.Color, p - 2 * v, p + 2 * v);
            }
        }

        /// <inheritdoc/>
        public void Line(Color color, Vector start, Vector end)
        {
            var length = (end - start).Length;
            if (length <= 0)
            {
                return;
            }

            var direction = (end - start).Normalized;

            double x = 0;
            while (true)
            {
                x += -Math.Log(_random.NextDouble()) / (Frequency * (_frame - _lastFrame)) / length;
                if (double.IsNaN(x) || double.IsInfinity(x) || x > 1)
                {
                    break;
                }

                var position = Vector.Mix(start, end, x);
                CreateParticle(color, position);
            }
        }

        /// <inheritdoc/>
        public void Point(Color color, Vector vector)
        {
            if (_random.NextDouble() < Frequency * (_frame - _lastFrame))
            {
                CreateParticle(color, vector);
            }
        }

        private void CreateParticle(Color color, Vector position)
        {
            if (_particles.Count == _maxParticles)
            {
                /* Kill the oldest particle to make room. */
                _particles.RemoveAt(0);
            }

            var lifetime = -Math.Log(_random.NextDouble()) * Lifetime;

            _particles.Add(new Particle
            {
                Position = position,
                Velocity = Velocity + Intensity * RandomVector(),
                Gravity = Gravity,
                Color = color,
                Frame = _frame,
                Lifetime = lifetime,
            });
        }

        private Vector RandomVector()
        {
            while (true)
            {
                var vector = new Vector(2 * _random.NextDouble() - 1, 2 * _random.NextDouble() - 1);
                if (vector.SquaredLength <= 1)
                {
                    return vector;
                }
            }
        }

        private sealed class Particle
        {
            public Vector Position;

            public Vector Velocity;

            public Vector Gravity;

            public Color Color;

            public double Frame;

            public double Lifetime;
        }
    }
}
