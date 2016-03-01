// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Ui
{
    /// <summary>
    /// A menu item that allows entering text.
    /// </summary>
    internal class TextMenuItem : MenuItem
    {
        private const double _cursorBlinkFrames = 60;
        private int _cursorBlinkFrame = 0;
        private int _cursorPosition = 0;

        private string _text = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMenuItem"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        public TextMenuItem(string label)
        {
            Label = label;
        }

        /// <summary>
        /// Gets or sets the maximum length of the text, or 0 for no maximum.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the user-entered text.
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                _text = value ?? string.Empty;
                _cursorPosition = _text.Length;
            }
        }

        /// <summary>
        /// Gets or sets the action invoked when the text changes.
        /// </summary>
        public Action<string> OnChange { get; set; }

        /// <inheritdoc/>
        public override void Update(Terminal terminal, Vector position, bool isSelected)
        {
            string text = Label + ": " + _text;

            if (isSelected)
            {
                terminal.TextCenter(SelectedItemColor, Menu.FontSize, position, text);

                ++_cursorBlinkFrame;
                var cursorBrightness = 0.5 + 0.5 * Math.Cos(_cursorBlinkFrame / _cursorBlinkFrames * 2 * Math.PI);
                var cursorColor = new Color(cursorBrightness, cursorBrightness, cursorBrightness);

                var x = position.X + text.Length * Menu.FontSize * 0.5 - Menu.FontSize * (_text.Length - _cursorPosition);
                var dy = Menu.FontSize / 6;
                terminal.Line(cursorColor, new Vector(x, position.Y - dy), new Vector(x, position.Y + Menu.FontSize + dy));
            }
            else
            {
                terminal.TextCenter(ItemColor, Menu.FontSize, position, text);
                _cursorPosition = _text.Length;
                _cursorBlinkFrame = 0;
            }
        }

        /// <inheritdoc/>
        public override void HandleText(string text)
        {
            if (text == null)
            {
                return;
            }

            _cursorBlinkFrame = 0;

            bool wasModified = false;
            foreach (var c in text.ToUpperInvariant())
            {
                if (MaxLength > 0 && _text.Length >= MaxLength)
                {
                    break;
                }

                if (Font.GetGlyph(c) != null)
                {
                    _text = _text.Substring(0, _cursorPosition) + c + _text.Substring(_cursorPosition, _text.Length - _cursorPosition);
                    ++_cursorPosition;
                    wasModified = true;
                }
            }

            if (wasModified)
            {
                OnChange?.Invoke(_text);
            }
        }

        /// <inheritdoc/>
        public override void HandleLeft()
        {
            _cursorBlinkFrame = 0;
            if (_cursorPosition > 0)
            {
                --_cursorPosition;
            }
        }

        /// <inheritdoc/>
        public override void HandleRight()
        {
            _cursorBlinkFrame = 0;
            if (_cursorPosition < _text.Length)
            {
                ++_cursorPosition;
            }
        }

        /// <inheritdoc/>
        public override void HandleBackspace()
        {
            _cursorBlinkFrame = 0;
            if (_text.Length > 0 && _cursorPosition > 0)
            {
                _text = _text.Substring(0, _cursorPosition - 1) + _text.Substring(_cursorPosition, _text.Length - _cursorPosition);
                --_cursorPosition;
                OnChange?.Invoke(_text);
            }
        }

        /// <inheritdoc/>
        public override void HandleDelete()
        {
            _cursorBlinkFrame = 0;
            if (_text.Length > 0 && _cursorPosition < _text.Length)
            {
                _text = _text.Substring(0, _cursorPosition) + _text.Substring(_cursorPosition + 1, _text.Length - _cursorPosition - 1);
                OnChange?.Invoke(_text);
            }
        }
    }
}
