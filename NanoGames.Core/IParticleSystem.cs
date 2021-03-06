﻿// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// The particle system.
    /// </summary>
    public interface IParticleSystem : IGraphics
    {
        /// <summary>
        /// Gets or sets the gravity acceleration acting on the generated particles, in pixels per square frame.
        /// </summary>
        Vector Gravity { get; set; }

        /// <summary>
        /// Gets or sets the initial velocity of the generated particles in pixels per frame. Use this for explosions of moving objects.
        /// </summary>
        Vector Velocity { get; set; }

        /// <summary>
        /// Gets or sets the frequency of the generated particles, in particles per pixel.
        /// </summary>
        double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the intensity of the particle explosion, as a veloctiy in pixels per frame.
        /// </summary>
        double Intensity { get; set; }

        /// <summary>
        /// Gets or sets the lifetime of the generated particles, in frames.
        /// </summary>
        double Lifetime { get; set; }
    }
}
