// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Synchronization;

namespace NanoGames.Engine.OutputSystems
{
    /// <summary>
    /// A buffered audio implementation that keeps track of the sound a game wanted to play in the last valid frame.
    /// </summary>
    internal sealed class BufferedAudio : IAudio
    {
        private double _frame;

        private double _soundStart;
        private Sound _sound;

        /// <summary>
        /// Sets the current frame.
        /// </summary>
        /// <param name="frame">The frame number.</param>
        public void SetFrame(int frame)
        {
            _frame = frame;
        }

        /// <inheritdoc/>
        public void Play(Sound sound)
        {
            _soundStart = _frame;
            _sound = sound;
        }

        /// <summary>
        /// Renders, i.e. outputs, the buffered sound to the real audio device.
        /// </summary>
        /// <param name="frame">The current frame.</param>
        /// <param name="audio">The audio device to output to.</param>
        public void Render(double frame, Audio audio)
        {
            if (_sound != null)
            {
                audio.Play((_soundStart - frame) / GameSpeed.FramesPerSecond, _sound);

                /* Set _sound to null to mark the sound as already queued. */
                _sound = null;
            }
        }
    }
}
