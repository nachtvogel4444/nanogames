// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using NanoGames.Games;
using System.Collections.Generic;
using System.Linq;

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

        private readonly List<Input?[]> _knownPlayerInputs = new List<Input?[]>();

        private Match _match;

        private int _bufferedFrame = 0;
        private int _lastValidFrame = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchBuffer"/> class.
        /// </summary>
        /// <param name="initialMatch">The initial state of the match.</param>
        /// <param name="playerDescriptions">A list of player descriptions.</param>
        /// <param name="localPlayerIndex">The index of the local player in the list of player descriptions.</param>
        public MatchBuffer(Match initialMatch, List<PlayerDescription> playerDescriptions, int localPlayerIndex)
        {
            _playerCount = playerDescriptions.Count;
            _playerDescriptions = playerDescriptions;
            _localPlayerIndex = localPlayerIndex;

            _bufferedRenderer = new BufferedRenderer();
            _bufferedGraphics = new Graphics(_bufferedRenderer);

            _match = initialMatch;
            _playerDescriptions[localPlayerIndex].Graphics = _bufferedGraphics;
            _match.Update(playerDescriptions);

            _knownPlayerInputs.Add(CreateInitialInputRecord());
        }

        /// <summary>
        /// Sets the input for a certain player for a certain frame.
        /// </summary>
        /// <param name="frame">The frame for which to set the input.</param>
        /// <param name="playerIndex">The index of the player for which to set the input.</param>
        /// <param name="input">The player's input.</param>
        public void SetInput(int frame, int playerIndex, Input input)
        {
            if (frame < _lastValidFrame)
            {
                return;
            }

            int frameIndex = frame - _lastValidFrame;
            while (frameIndex >= _knownPlayerInputs.Count)
            {
                _knownPlayerInputs.Add(new Input?[_playerCount]);
            }

            _knownPlayerInputs[frameIndex][playerIndex] = input;

            if (frame <= _bufferedFrame)
            {
                _bufferedFrame = -1;
            }
        }

        /// <summary>
        /// Renders the current best prediction of a certain frame.
        /// </summary>
        /// <param name="requestedFrame">The index of the frame to render.</param>
        /// <param name="terminal">The terminal to render to.</param>
        public void RenderFrame(int requestedFrame, Terminal terminal)
        {
            int currentFrame = _lastValidFrame;
            int currentFrameIndex = 0;
            var currentFrameInput = _knownPlayerInputs[0].Select(i => (Input)i).ToArray();

            bool allInputWasValid = true;
            bool matchWasAlreadyCloned = false;
            var match = _match;

            while (true)
            {
                if (requestedFrame <= _bufferedFrame)
                {
                    _bufferedRenderer.RenderTo(terminal.Renderer);
                    return;
                }

                ++currentFrameIndex;
                ++currentFrame;

                for (int playerIndex = 0; playerIndex < _playerCount; ++playerIndex)
                {
                    Input input;
                    if (_knownPlayerInputs[currentFrameIndex][playerIndex] == null)
                    {
                        allInputWasValid = false;
                        input = currentFrameInput[playerIndex];
                    }
                    else
                    {
                        input = (Input)_knownPlayerInputs[currentFrameIndex][playerIndex];
                        currentFrameInput[playerIndex] = input;
                    }

                    _playerDescriptions[playerIndex].Input = input;
                    if (currentFrame == requestedFrame && playerIndex == _localPlayerIndex)
                    {
                        /* This is the frame we actually want to render. */
                        _bufferedRenderer.Clear();
                        _playerDescriptions[playerIndex].Graphics = _bufferedGraphics;
                        _bufferedFrame = requestedFrame;
                    }
                    else
                    {
                        /* Set this to null just to make sure. */
                        _playerDescriptions[playerIndex].Graphics = Graphics.Null;
                    }
                }

                if (allInputWasValid)
                {
                    /*
                     * The frame we want to compute is valid (i.e. all inputs are known).
                     * Throw away the current _lastValidFrame because we never need to roll back to it.
                     */
                    _knownPlayerInputs.RemoveAt(0);
                    ++_lastValidFrame;
                    --currentFrameIndex;
                }
                else
                {
                    if (!matchWasAlreadyCloned)
                    {
                        /*
                         * From now on, we are in prediction mode.
                         * Clone the match state and work on a copy.
                         */
                        match = Cloning.Clone(match);
                    }
                }

                match.Update(_playerDescriptions);
            }
        }

        private Input?[] CreateInitialInputRecord()
        {
            var record = new Input?[_playerCount];
            for (int i = 0; i < _playerCount; ++i)
            {
                record[i] = default(Input);
            }

            return record;
        }
    }
}
