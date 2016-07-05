// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Engine.OutputSystems
{
    /// <summary>
    /// The default <see cref="IOutput"/>  implementation.
    /// </summary>
    internal sealed class Output : IOutput
    {
        private readonly ParticleSystem _particles = new ParticleSystem();

        /// <inheritdoc/>
        public IParticleSystem Particles => _particles;

        /// <inheritdoc/>
        public void SetFrame(int frame)
        {
            _particles.SetFrame(frame);
        }

        /// <summary>
        /// Renders the output to the screen.
        /// </summary>
        /// <param name="frame">The frame number, can be fractional.</param>
        /// <param name="graphics">The graphics to render to.</param>
        public void Render(double frame, Graphics graphics)
        {
            _particles.Render(frame, graphics);
        }
    }
}
