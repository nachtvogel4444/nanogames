// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Threading;

namespace NanoGames.Engine.OutputSystems
{
    /// <summary>
    /// The sound synthesizer.
    /// </summary>
    internal sealed class Audio : IAudio, IWaveProvider, IDisposable
    {
        /*
         * This emulates a "PC speaker" type sound device, which can only output one type of sound, a square wave.
         *
         * The value of a square wave is -1 in the first half of the cycle and +1 in the second half. In the old days,
         * synthesizers generated this wave by incrementing a simple 1-byte counter that was incremented at the exact
         * rate that it would wrap around every cycle, and outputting -1 or +1 according to the highest counter bit.
         *
         * Because a perfect square sounds too sharp (in reality, these devices had an effective low-pass filter),
         * the output is slightly smoothed by averaging it with previous values.
         */

        private const double _volume = 0.05;

        private const int _rate = 44100;
        private const double _rateConversion = 1.0 / _rate;

        private const double _mixToTargetValue = 0.1;
        private const double _mixToZero = 0.001;

        private static readonly WaveFormat _waveFormat = new WaveFormat(_rate, 16, 1);

        private double _value = -1;
        private double _correctionToZero = -1;

        private int _sample;

        private IWavePlayer _player;

        private double _counter;

        private SoundEntry _currentSound;

        private bool _isDisposing;

        /// <summary>
        /// Initializes a new instance of the <see cref="Audio"/> class.
        /// </summary>
        public Audio()
        {
            _player = new DirectSoundOut(50);
            _player.Init(this);
            _player.Play();
        }

        /// <inheritdoc/>
        WaveFormat IWaveProvider.WaveFormat => _waveFormat;

        /// <inheritdoc/>
        public void Play(Sound sound)
        {
            Play(0, sound);
        }

        /// <summary>
        /// Plays the specified sound after a delay.
        /// </summary>
        /// <param name="delay">The delay in seconds.</param>
        /// <param name="sound">The sound to play.</param>
        public void Play(double delay, Sound sound)
        {
            if (sound == null)
            {
                Volatile.Write(ref _currentSound, null);
            }
            else
            {
                Volatile.Write(ref _currentSound, new SoundEntry(_sample * _rateConversion + delay, sound));
            }
        }

        /// <inheritdoc/>
        int IWaveProvider.Read(byte[] buffer, int offset, int count)
        {
            try
            {
                int end = offset + count;

                for (int p = offset; p < end; p += 2)
                {
                    WriteSample(buffer, p, GetSample());
                }
            }
            catch (Exception)
            {
            }

            return count;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Volatile.Write(ref _isDisposing, true);
            if (Volatile.Read(ref _currentSound) != null)
            {
                var startDispose = Stopwatch.GetTimestamp();
                Thread.Sleep(1000);
            }

            _player.Dispose();
        }

        private static void WriteSample(byte[] buffer, int position, double value)
        {
            value *= _volume * 32767;
            if (value > 32767)
            {
                value = 32767;
            }
            else if (value < -32767)
            {
                value = -32767;
            }

            int v = (int)value;

            buffer[position] = (byte)(v & 0xff);
            buffer[position + 1] = (byte)((v >> 8) & 0xff);
        }

        private double GetSample()
        {
            double absoluteTime = _sample * _rateConversion;

            ++_sample;

            double frequency = 0;

            var currentSound = Volatile.Read(ref _currentSound);
            if (currentSound != null)
            {
                if (absoluteTime > currentSound.Start + currentSound.Sound.Duration)
                {
                    Interlocked.CompareExchange(ref _currentSound, null, currentSound);
                    _currentSound = null;
                }
                else if (currentSound.Start <= absoluteTime)
                {
                    var relativeTime = absoluteTime - currentSound.Start;
                    frequency = Math.Max(frequency, currentSound.Sound.GetFrequency(relativeTime));
                }
            }

            if (!Volatile.Read(ref _isDisposing))
            {
                _counter += _rateConversion * frequency;
                _counter %= 1.0;
            }

            var targetValue = _counter < 0.5 ? -1.0 : 1.0;
            _value = targetValue * _mixToTargetValue + _value * (1 - _mixToTargetValue);
            _correctionToZero = _value * _mixToZero + _correctionToZero * (1 - _mixToZero);
            return _value - _correctionToZero;
        }

        private class SoundEntry
        {
            public readonly double Start;

            public readonly Sound Sound;

            public SoundEntry(double start, Sound sound)
            {
                Start = start;
                Sound = sound;
            }
        }
    }
}
