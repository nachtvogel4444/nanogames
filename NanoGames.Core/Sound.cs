// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Linq;

namespace NanoGames
{
    /// <summary>
    /// A playable sound.
    /// </summary>
    public abstract class Sound
    {
        /// <summary>
        /// Gets the duration of the sound, in seconds.
        /// </summary>
        public abstract double Duration { get; }

        /// <summary>
        /// Creates a new sound that plays a sequence of sounds.
        /// </summary>
        /// <param name="sounds">The sounds to play.</param>
        /// <returns>The sound sequence.</returns>
        public static Sound Sequence(params Sound[] sounds)
        {
            return new SequenceSound(sounds);
        }

        /// <summary>
        /// A pause, that is, an empty sound. Used together with s
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <returns>The pause sound.</returns>
        public static Sound Pause(double duration)
        {
            return new PauseSound(duration);
        }

        /// <summary>
        /// A simple steady tone.
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="pitch">The pitch to play.</param>
        /// <returns>The sound.</returns>
        public static Sound Tone(double duration, Pitch pitch)
        {
            return new ToneSound(duration, pitch);
        }

        /// <summary>
        /// A chirp sound, that is, a sound that shifts its frequency at a constant rate.
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="startPitch">The start pitch.</param>
        /// <param name="endPitch">The end pitch.</param>
        /// <returns>The chirp sound.</returns>
        public static Sound Chirp(double duration, Pitch startPitch, Pitch endPitch)
        {
            return new ChirpSound(duration, startPitch, endPitch);
        }

        /// <summary>
        /// A noise sound that randomly varies within a frequency range.
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="lowPitch">The lower range of the noise.</param>
        /// <param name="highPitch">The upper range of the noise.</param>
        /// <returns>The noise sound.</returns>
        public static Sound Noise(double duration, Pitch lowPitch, Pitch highPitch)
        {
            return new NoiseSound(duration, lowPitch, highPitch);
        }

        /// <summary>
        /// Gets the frequency of the sound at the specified time since the start.
        /// </summary>
        /// <param name="time">The time since the start in seconds.</param>
        /// <returns>The frequency.</returns>
        public abstract double GetFrequency(double time);

        private class SequenceSound : Sound
        {
            private readonly Sound[] _sounds;

            public SequenceSound(params Sound[] sounds)
            {
                _sounds = sounds;
                Duration = sounds.Sum(s => s.Duration);
            }

            public override double Duration { get; }

            public override double GetFrequency(double time)
            {
                for (int i = 0; i < _sounds.Length; ++i)
                {
                    var sound = _sounds[i];
                    var duration = sound.Duration;
                    if (time < duration)
                    {
                        return sound.GetFrequency(time);
                    }

                    time -= duration;
                }

                return 0;
            }
        }

        private class PauseSound : Sound
        {
            public PauseSound(double duration)
            {
                Duration = duration;
            }

            public override double Duration { get; }

            public override double GetFrequency(double time)
            {
                return 0;
            }
        }

        private class ToneSound : Sound
        {
            private readonly Pitch _pitch;

            public ToneSound(double duration, Pitch pitch)
            {
                Duration = duration;
                _pitch = pitch;
            }

            public override double Duration { get; }

            public override double GetFrequency(double time)
            {
                return _pitch.Frequency;
            }
        }

        private class ChirpSound : Sound
        {
            private readonly Pitch _startPitch;
            private readonly Pitch _endPitch;

            public ChirpSound(double duration, Pitch startPitch, Pitch endPitch)
            {
                Duration = duration;
                _startPitch = startPitch;
                _endPitch = endPitch;
            }

            public override double Duration { get; }

            public override double GetFrequency(double time)
            {
                var mix = time / Duration;
                var frequency = (1 - mix) * _startPitch.Frequency + mix * _endPitch.Frequency;
                return frequency;
            }
        }

        private class NoiseSound : Sound
        {
            private const double _randomizeLifespan = 1.0 / 240;
            private static readonly Random _random = new Random();
            private readonly Pitch _lowPitch;
            private readonly Pitch _highPitch;

            private int _randomizeIndex = -1;
            private double _frequency;

            public NoiseSound(double duration, Pitch lowPitch, Pitch highPitch)
            {
                Duration = duration;
                _lowPitch = lowPitch;
                _highPitch = highPitch;
            }

            public override double Duration { get; }

            public override double GetFrequency(double time)
            {
                int index = (int)(time / _randomizeLifespan);
                if (index != _randomizeIndex)
                {
                    _randomizeIndex = index;
                    var mix = _random.NextDouble();
                    _frequency = (1 - mix) * _lowPitch.Frequency + mix * _highPitch.Frequency;
                }

                return _frequency;
            }
        }
    }
}
