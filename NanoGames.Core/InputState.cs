// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using ProtoBuf;

namespace NanoGames
{
    /// <summary>
    /// Represents the input state of a player.
    /// </summary>
    [ProtoContract]
    public struct InputState
    {
        [ProtoMember(1)]
        private int _value;

        /// <summary>
        /// Gets or sets a value indicating whether the up button is pressed.
        /// </summary>
        public bool Up
        {
            get { return GetBit(0); }
            set { SetBit(0, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the down button is pressed.
        /// </summary>
        public bool Down
        {
            get { return GetBit(1); }
            set { SetBit(1, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the left button is pressed.
        /// </summary>
        public bool Left
        {
            get { return GetBit(2); }
            set { SetBit(2, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the right button is pressed.
        /// </summary>
        public bool Right
        {
            get { return GetBit(3); }
            set { SetBit(3, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the fire button is pressed.
        /// </summary>
        public bool Fire
        {
            get { return GetBit(4); }
            set { SetBit(4, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the alt fire button is pressed.
        /// </summary>
        public bool AltFire
        {
            get { return GetBit(5); }
            set { SetBit(5, value); }
        }

        public static bool operator ==(InputState a, InputState b)
        {
            return a._value == b._value;
        }

        public static bool operator !=(InputState a, InputState b)
        {
            return a._value != b._value;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is InputState && this == (InputState)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        private bool GetBit(int n)
        {
            return (_value & (1 << n)) != 0;
        }

        private void SetBit(int n, bool value)
        {
            if (value)
            {
                _value |= 1 << n;
            }
            else
            {
                _value &= ~(1 << n);
            }
        }
    }
}
