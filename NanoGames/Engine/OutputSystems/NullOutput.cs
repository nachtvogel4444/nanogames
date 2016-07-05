// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Engine.OutputSystems
{
    /// <summary>
    /// An <see cref="IOutput"/> implementation that doesn't render its output anywhere.
    /// </summary>
    internal sealed class NullOutput : IOutput
    {
        /// <summary>
        /// The <see cref="NullOutput"/> instance.
        /// </summary>
        public static readonly IOutput Instance = new NullOutput();

        private NullOutput()
        {
        }

        /// <inheritdoc/>
        public IParticleSystem Particles => NullParticleSystem.Instance;

        /// <inheritdoc/>
        public void SetFrame(int frame)
        {
        }

        private sealed class NullParticleSystem : IParticleSystem
        {
            public static readonly NullParticleSystem Instance = new NullParticleSystem();

            public double MeanDistance { get; set; }

            public Vector Gravity { get; set; }

            public Vector Velocity { get; set; }

            public double Intensity { get; set; }

            public void Line(Color color, Vector start, Vector end)
            {
            }
        }
    }
}
