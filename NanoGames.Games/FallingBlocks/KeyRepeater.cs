namespace NanoGames.Games.FallingBlocks
{
    internal class KeyRepeater
    {
        private readonly int _keyCooldown = 6;

        private int _lastFrame = int.MinValue;

        public KeyRepeater(int cooldown)
        {
            _keyCooldown = cooldown;
        }

        public bool IsPressed(int currentFrame, bool currentInput)
        {
            if (currentInput)
            {
                if (_lastFrame < currentFrame - _keyCooldown)
                {
                    _lastFrame = currentFrame;
                    return true;
                }
            }
            else
            {
                _lastFrame = int.MinValue;
            }

            return false;
        }

        public void Release()
        {
            _lastFrame = int.MinValue;
        }
    }
}
