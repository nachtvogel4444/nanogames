// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.FallingBlocks
{
    internal class KeyRepeater
    {
        private readonly int _keyCooldown;

        private int _lastFrame = int.MinValue;

        public KeyRepeater(int cooldown)
        {
            _keyCooldown = cooldown;
        }

        public bool IsPressed(int currentFrame, bool currentInput)
        {
            if (currentInput)
            {
                if (!IsBlocked(currentFrame))
                {
                    Block(currentFrame);
                    return true;
                }
            }
            else
            {
                _lastFrame = int.MinValue;
            }

            return false;
        }

        public void Block(int currentFrame)
        {
            _lastFrame = currentFrame;
        }

        public void Release()
        {
            _lastFrame = int.MinValue;
        }

        public bool IsBlocked(int currentFrame)
        {
            return _lastFrame >= currentFrame - _keyCooldown;
        }
    }
}
