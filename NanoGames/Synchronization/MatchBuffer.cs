// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using NanoGames.Games;
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
        private readonly List<PlayerDescription> _playerDescriptions;
        private readonly int _localPlayerIndex;

        private readonly BufferedRenderer _bufferedRenderer;
        private readonly Graphics _bufferedGraphics;

        private readonly List<PlayerInputState[]> _playerInputs = new List<PlayerInputState[]>();

        private IMatch _predictedMatch;
        private IMatch _knownMatch;

        private int _knownFrame = 0;
        private int _predictedFrame = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchBuffer"/> class.
        /// </summary>
        /// <param name="initialMatch">The initial state of the match.</param>
        /// <param name="playerDescriptions">A list of player descriptions.</param>
        /// <param name="localPlayerIndex">The index of the local player in the list of player descriptions.</param>
        public MatchBuffer(IMatch initialMatch, List<PlayerDescription> playerDescriptions, int localPlayerIndex)
        {
            _playerCount = playerDescriptions.Count;
            _playerDescriptions = playerDescriptions;
            _localPlayerIndex = localPlayerIndex;

            _bufferedRenderer = new BufferedRenderer();
            _bufferedGraphics = new Graphics(_bufferedRenderer);

            _playerDescriptions[localPlayerIndex].Graphics = _bufferedGraphics;
            initialMatch.Update(_bufferedGraphics, playerDescriptions);

            _knownMatch = _predictedMatch = initialMatch;

            _playerInputs.Add(CreateInitialInputRecord());
        }

        /// <summary>
        /// Gets a value indicating whether the match is confirmed to be completed.
        /// This only returns true if the match result is confirmed, i.e. can't change due to a prediction correction.
        /// </summary>
        public bool IsCompleted => _knownMatch.IsCompleted;

        /// <summary>
        /// Gets the current scores for all players.
        /// </summary>
        public IEnumerable<double> PlayerScores => _knownMatch.PlayerScores;

        /// <summary>
        /// Sets the input for a certain player for a certain frame.
        /// </summary>
        /// <param name="frame">The frame for which to set the input.</param>
        /// <param name="playerIndex">The index of the player for which to set the input.</param>
        /// <param name="input">The player's input.</param>
        public void SetInput(int frame, int playerIndex, Input input)
        {
            if (frame <= _knownFrame)
            {
                return;
            }

            int frameIndex = frame - _knownFrame;
            FillInputsUpToIndex(frameIndex);

            _playerInputs[frameIndex][playerIndex].IsKnown = true;

            var oldInput = _playerInputs[frameIndex][playerIndex].Input;
            _playerInputs[frameIndex][playerIndex].Input = input;

            if (frame > _knownFrame && frame <= _predictedFrame && input != oldInput)
            {
                /* Misprediction. Roll back to the last known state. */
                _predictedMatch = _knownMatch;
                _predictedFrame = _knownFrame;
            }
        }

        /// <summary>
        /// Renders the current best prediction of a certain frame.
        /// </summary>
        /// <param name="requestedFrame">The index of the frame to render.</param>
        /// <param name="terminal">The terminal to render to.</param>
        public void RenderFrame(int requestedFrame, Terminal terminal)
        {
            while (true)
            {
                if (requestedFrame <= _predictedFrame)
                {
                    _bufferedRenderer.RenderTo(terminal.Renderer);
                    return;
                }

                if (_predictedMatch != _knownMatch)
                {
                    /*
                     * We are currently successfully predicting the future game state,
                     * but we can "commit" any frames in the past where the input is now confirmed.
                     */

                    while (_knownFrame + 1 < requestedFrame && AllInputsKnown())
                    {
                        var knownInputs = _playerInputs[1];
                        for (int playerIndex = 0; playerIndex < _playerCount; ++playerIndex)
                        {
                            _playerDescriptions[playerIndex].Input = knownInputs[playerIndex].Input;
                            _playerDescriptions[playerIndex].Graphics = Graphics.Null;
                        }

                        _playerInputs.RemoveAt(0);
                        _knownMatch.Update(Graphics.Null, _playerDescriptions);
                        ++_knownFrame;
                    }

                    if (_predictedFrame < _knownFrame)
                    {
                        _predictedMatch = _knownMatch;
                        _predictedFrame = _knownFrame;
                    }
                }

                ++_predictedFrame;

                Graphics localGraphics;
                if (_predictedFrame == requestedFrame)
                {
                    /* This is the frame we actually want to render. */
                    _bufferedRenderer.Clear();
                    localGraphics = _bufferedGraphics;
                }
                else
                {
                    /* Discard the output of this frame. */
                    localGraphics = Graphics.Null;
                }

                FillInputsUpToIndex(_predictedFrame - _knownFrame);

                bool isInputKnown = true;
                var inputs = _playerInputs[_predictedFrame - _knownFrame];
                for (int playerIndex = 0; playerIndex < _playerCount; ++playerIndex)
                {
                    isInputKnown &= inputs[playerIndex].IsKnown;
                    _playerDescriptions[playerIndex].Input = inputs[playerIndex].Input;
                    _playerDescriptions[playerIndex].Graphics = playerIndex == _localPlayerIndex ? localGraphics : Graphics.Null;
                }

                if (_predictedMatch == _knownMatch)
                {
                    if (isInputKnown)
                    {
                        /*
                         * The frame we want to compute is valid (i.e. all inputs are known).
                         * This means that the _predictedMatch is also the _knownMatch, that is,
                         * we commit the changes we make.
                         */
                        _playerInputs.RemoveAt(0);
                        ++_knownFrame;
                    }
                    else
                    {
                        /*
                         * From now on, we are in prediction mode.
                         * Clone the match state and work on a copy called _predictedMatch.
                         */
                        _predictedMatch = Cloning.Clone(_knownMatch);
                    }
                }

                _predictedMatch.Update(localGraphics, _playerDescriptions);
            }
        }

        private bool AllInputsKnown()
        {
            if (_playerInputs.Count <= 1)
            {
                return false;
            }

            for (int p = 0; p < _playerCount; ++p)
            {
                if (!_playerInputs[1][p].IsKnown)
                {
                    return false;
                }
            }

            return true;
        }

        private void FillInputsUpToIndex(int frameIndex)
        {
            while (frameIndex >= _playerInputs.Count)
            {
                var inputs = new PlayerInputState[_playerCount];
                for (int i = 0; i < _playerCount; ++i)
                {
                    inputs[i] = new PlayerInputState(false, _playerInputs[_playerInputs.Count - 1][i].Input);
                }

                _playerInputs.Add(inputs);
            }
        }

        private PlayerInputState[] CreateInitialInputRecord()
        {
            var record = new PlayerInputState[_playerCount];
            for (int i = 0; i < _playerCount; ++i)
            {
                record[i].IsKnown = true;
            }

            return record;
        }

        private struct PlayerInputState
        {
            public bool IsKnown;

            public Input Input;

            public PlayerInputState(bool isKnown, Input input)
            {
                IsKnown = isKnown;
                Input = input;
            }
        }
    }
}
