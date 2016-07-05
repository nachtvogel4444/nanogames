// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// Represents the game output.
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        /// Gets the particle system.
        /// </summary>
        IParticleSystem Particles { get; }

        /// <summary>
        /// Gets the graphics system.
        /// </summary>
        IGraphics Graphics { get; }

        /// <summary>
        /// Sets the current frame index.
        /// </summary>
        /// <param name="frame">The frame index.</param>
        void SetFrame(int frame);
    }
}
