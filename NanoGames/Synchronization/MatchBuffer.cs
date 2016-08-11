// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Games;
using System;
using System.Collections.Generic;

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Buffers several copies of the match state to be able predict and render the current state even if not all inputs have arrived yet.
    /// When player inputs arrived, recomputes the match state from a previous known value to account for the new data.
    /// </summary>
    internal class MatchBuffer
    {
        private readonly int _playerCount;

        private readonly List<FrameInputState> _frameInputs = new List<FrameInputState>();

        private IMatch _predictedMatch;
        private IMatch _knownMatch;

        private int _knownFrame = 0;
        private int _predictedFrame = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchBuffer"/> class.
        /// </summary>
        /// <param name="initialMatch">The initial state of the match.</param>
        public MatchBuffer(IMatch initialMatch)
        {
            _playerCount = initialMatch.Players.Count;

            _frameInputs.Add(CreateInitialInputRecord());
            initialMatch.Update(_frameInputs[0].Inputs);

            _knownMatch = _predictedMatch = initialMatch;
        }

        /// <summary>
        /// Gets a value indicating whether the match is confirmed to be completed.
        /// This only returns true if the match result is confirmed, i.e. can't change due to a prediction correction.
        /// </summary>
        public bool IsCompleted => _knownMatch.IsCompleted;

        /// <summary>
        /// Gets the currently known match state.
        /// </summary>
        public IMatch KnownMatch => _knownMatch;

        /// <summary>
        /// Gets the currently predicted match state.
        /// </summary>
        public IMatch PredictedMatch => _predictedMatch;

        /// <summary>
        /// Sets the input for a certain player for a certain frame.
        /// </summary>
        /// <param name="frame">The frame for which to set the input.</param>
        /// <param name="playerIndex">The index of the player for which to set the input.</param>
        /// <param name="input">The player's input.</param>
        public void SetInput(int frame, int playerIndex, InputState input)
        {
            if (frame <= _knownFrame)
            {
                return;
            }

            int frameIndex = frame - _knownFrame;
            FillInputsUpToIndex(frameIndex);

            var frameInput = _frameInputs[frameIndex];

            frameInput.ConfirmedStates[playerIndex] = true;

            var inputs = frameInput.Inputs;
            if (input != inputs[playerIndex])
            {
                inputs[playerIndex] = input;

                /* Propagate the new input as a prediction to all future inputs. */
                for (int i = frameIndex + 1; i < _frameInputs.Count; ++i)
                {
                    _frameInputs[i].Inputs[playerIndex] = input;
                }

                if (frame > _knownFrame && frame <= _predictedFrame)
                {
                    /* Misprediction. Roll back to the last known state. */
                    _predictedMatch = _knownMatch;
                    _predictedFrame = _knownFrame;
                }
            }
        }

        /// <summary>
        /// Updates the current predicted match to a certain frame.
        /// </summary>
        /// <param name="requestedFrame">The index of the frame to render.</param>
        public void UpdateToFrame(int requestedFrame)
        {
            /* Roll the known game state forward as far as the inputs are known. */
            while (_knownFrame < requestedFrame && AllInputsKnown())
            {
                var knownInputs = _frameInputs[1].Inputs;
                _frameInputs.RemoveAt(0);

                _knownMatch.Update(knownInputs);

                /* We have caught up with the prediction, also roll the prediction forward. */
                if (_predictedFrame == _knownFrame)
                {
                    ++_predictedFrame;

                    if (_predictedMatch != _knownMatch)
                    {
                        /*
                         *  There's only a prediction to update if the predicted match is actually a different instance,
                         * otherwise, we'd update the same match twice.
                         */
                        _predictedMatch.Update(knownInputs);
                    }
                }

                ++_knownFrame;
            }

            /* Roll the prediction forward as far as requested. */
            while (_predictedFrame < requestedFrame)
            {
                if (_predictedMatch == _knownMatch)
                {
                    /*
                     * From now on, we are in prediction mode.
                     * Clone the match state and work on a copy called _predictedMatch.
                     */
                    _predictedMatch = Cloning.Clone(_knownMatch);
                }

                ++_predictedFrame;
                int frameIndex = Math.Min(_predictedFrame - _knownFrame, _frameInputs.Count - 1);
                _predictedMatch.Update(_frameInputs[frameIndex].Inputs);
            }
        }

        private bool AllInputsKnown()
        {
            if (_frameInputs.Count <= 1)
            {
                return false;
            }

            var confirmedStates = _frameInputs[1].ConfirmedStates;
            for (int p = 0; p < _playerCount; ++p)
            {
                if (!confirmedStates[p])
                {
                    return false;
                }
            }

            return true;
        }

        private void FillInputsUpToIndex(int frameIndex)
        {
            while (frameIndex >= _frameInputs.Count)
            {
                var frameInput = new FrameInputState(_playerCount);
                Array.Copy(_frameInputs[_frameInputs.Count - 1].Inputs, frameInput.Inputs, _playerCount);
                _frameInputs.Add(frameInput);
            }
        }

        private FrameInputState CreateInitialInputRecord()
        {
            var inputState = new FrameInputState(_playerCount);
            for (int i = 0; i < _playerCount; ++i)
            {
                inputState.ConfirmedStates[i] = true;
            }

            return inputState;
        }

        private struct FrameInputState
        {
            public bool[] ConfirmedStates;

            public InputState[] Inputs;

            public FrameInputState(int playerCount)
            {
                ConfirmedStates = new bool[playerCount];
                Inputs = new InputState[playerCount];
            }
        }
    }
}
