// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// A class used to access a player's input.
    /// </summary>
    public sealed class Input
    {
        /// <summary>
        /// The state of the Up button.
        /// </summary>
        public readonly ButtonState Up = new ButtonState();

        /// <summary>
        /// The state of the Down button.
        /// </summary>
        public readonly ButtonState Down = new ButtonState();

        /// <summary>
        /// The state of the Left button.
        /// </summary>
        public readonly ButtonState Left = new ButtonState();

        /// <summary>
        /// The state of the Right button.
        /// </summary>
        public readonly ButtonState Right = new ButtonState();

        /// <summary>
        /// The state of the Fire button.
        /// </summary>
        public readonly ButtonState Fire = new ButtonState();

        /// <summary>
        /// The state of the Alt Fire button.
        /// </summary>
        public readonly ButtonState AltFire = new ButtonState();

        private const int _firstRepeatFrames = 20;

        private const int _secondRepeatFrames = 6;

        /// <summary>
        /// Updates the input state.
        /// </summary>
        /// <param name="currentFrame">The index of the current frame.</param>
        /// <param name="state">The input state.</param>
        public void SetState(int currentFrame, InputState state)
        {
            Up.SetState(currentFrame, state.Up);
            Down.SetState(currentFrame, state.Down);
            Left.SetState(currentFrame, state.Left);
            Right.SetState(currentFrame, state.Right);
            Fire.SetState(currentFrame, state.Fire);
            AltFire.SetState(currentFrame, state.AltFire);
        }

        /// <summary>
        /// Represents the state of a button.
        /// </summary>
        public class ButtonState
        {
            private int _nextRepeatFrame;

            private bool _isPressed;

            private bool _wasActivated;

            /// <summary>
            /// Gets a value indicating whether the button is currently pressed.
            /// </summary>
            public bool IsPressed => _isPressed;

            /// <summary>
            /// Gets a value indicating whether the button was activated (or repeated) in this frame.
            /// </summary>
            public bool WasActivated => _wasActivated;

            /// <summary>
            /// Sets the state of current button.
            /// </summary>
            /// <param name="currentFrame">The index of the current frame.</param>
            /// <param name="isPressed">A value indicating whether the button is currently pressed.</param>
            public void SetState(int currentFrame, bool isPressed)
            {
                if (!isPressed)
                {
                    _isPressed = false;
                    _wasActivated = false;
                }
                else
                {
                    if (!_isPressed)
                    {
                        _isPressed = true;
                        _wasActivated = true;
                        _nextRepeatFrame = currentFrame + _firstRepeatFrames;
                    }
                    else
                    {
                        if (currentFrame >= _nextRepeatFrame)
                        {
                            _wasActivated = true;
                            _nextRepeatFrame += _secondRepeatFrames;
                        }
                        else
                        {
                            _wasActivated = false;
                        }
                    }
                }
            }
        }
    }
}
