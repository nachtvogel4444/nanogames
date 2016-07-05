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

        private BufferedGraphics _graphics;

        /// <inheritdoc/>
        public IParticleSystem Particles => _particles;

        /// <inheritdoc/>
        public IGraphics Graphics
        {
            get
            {
                if (_graphics == null)
                {
                    _graphics = new BufferedGraphics();
                }

                return _graphics;
            }
        }

        /// <inheritdoc/>
        public void SetFrame(int frame)
        {
            _graphics?.Clear();
            _particles.SetFrame(frame);
        }

        /// <summary>
        /// Renders the output to the screen.
        /// </summary>
        /// <param name="frame">The frame number, can be fractional.</param>
        /// <param name="terminal">The terminal to render to.</param>
        public void Render(double frame, Terminal terminal)
        {
            _graphics?.Render(terminal.Graphics);
            _particles.Render(frame, terminal.Graphics);
        }
    }
}
