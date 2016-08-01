// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames
{
    /// <summary>
    /// Represents a pitch (the "height" of a tone).
    /// </summary>
    public struct Pitch
    {
        /// <summary>
        /// The frequency in hertz.
        /// </summary>
        public double Frequency;

        private static readonly double _halfToneFactor = Math.Pow(2, 1.0 / 12.0);
        private static readonly double _noteZero = 440 * Math.Pow(2, -57 / 12.0); // note 57 is A4 (note 4 * 12 + 9) is 440 Hz

        /// <summary>
        /// Initializes a new instance of the <see cref="Pitch"/> struct.
        /// </summary>
        /// <param name="frequency">The frequency in hertz.</param>
        public Pitch(double frequency)
        {
            Frequency = frequency;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pitch"/> struct.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <param name="note">The note in half-tones, 0 is C.</param>
        private Pitch(int octave, int note)
        {
            int n = octave * 12 + note;
            Frequency = _noteZero * Math.Pow(_halfToneFactor, n);
        }

        /// <summary>
        /// Creates a C.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch C(int octave)
        {
            return new Pitch(octave, 0);
        }

        /// <summary>
        /// Creates a C#/Db.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch Db(int octave)
        {
            return new Pitch(octave, 1);
        }

        /// <summary>
        /// Creates a D.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch D(int octave)
        {
            return new Pitch(octave, 2);
        }

        /// <summary>
        /// Creates a D#/Eb.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch Eb(int octave)
        {
            return new Pitch(octave, 3);
        }

        /// <summary>
        /// Creates an E.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch E(int octave)
        {
            return new Pitch(octave, 4);
        }

        /// <summary>
        /// Creates an F.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch F(int octave)
        {
            return new Pitch(octave, 5);
        }

        /// <summary>
        /// Creates an F#/Gb.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch Gb(int octave)
        {
            return new Pitch(octave, 6);
        }

        /// <summary>
        /// Creates a G.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch G(int octave)
        {
            return new Pitch(octave, 7);
        }

        /// <summary>
        /// Creates a G#/Ab.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch Ab(int octave)
        {
            return new Pitch(octave, 8);
        }

        /// <summary>
        /// Creates an A.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch A(int octave)
        {
            return new Pitch(octave, 9);
        }

        /// <summary>
        /// Creates an A#/Bb.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch Bb(int octave)
        {
            return new Pitch(octave, 10);
        }

        /// <summary>
        /// Creates a B.
        /// </summary>
        /// <param name="octave">The octave.</param>
        /// <returns>The pitch.</returns>
        public static Pitch B(int octave)
        {
            return new Pitch(octave, 11);
        }
    }
}
