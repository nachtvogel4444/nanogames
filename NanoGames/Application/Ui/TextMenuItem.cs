// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using System;
using System.Diagnostics;

namespace NanoGames.Application.Ui
{
    /// <summary>
    /// A menu item that allows entering text.
    /// </summary>
    internal sealed class TextMenuItem : MenuItem
    {
        private double _cursorBlinkTime = 0;
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

        /// <summary>
        /// Gets or sets the action invoked when the menu item is activated.
        /// </summary>
        public Action OnActivate { get; set; }

        /// <inheritdoc/>
        public override void Update(Terminal terminal, Vector position, bool isSelected)
        {
            string text = Label + ": " + _text;

            if (isSelected)
            {
                terminal.Graphics.PrintCenter(Colors.FocusedControl, Menu.FontSize, position, text);

                var cursorBrightness = 0.5 + 0.5 * Math.Cos((Stopwatch.GetTimestamp() - _cursorBlinkTime) / (double)Stopwatch.Frequency * 2 * Math.PI);
                var cursorColor = new Color(cursorBrightness, cursorBrightness, cursorBrightness);

                var x = position.X + text.Length * Menu.FontSize * 0.5 - Menu.FontSize * (_text.Length - _cursorPosition);
                var dy = Menu.FontSize / 6;
                terminal.Graphics.Line(cursorColor, new Vector(x, position.Y - dy), new Vector(x, position.Y + Menu.FontSize + dy));
            }
            else
            {
                terminal.Graphics.PrintCenter(Colors.Control, Menu.FontSize, position, text);
                _cursorPosition = _text.Length;
                ResetCursorBlink();
            }
        }

        /// <inheritdoc/>
        public override void HandleKeyEvent(KeyEvent keyEvent)
        {
            if (keyEvent.Char != 0)
            {
                HandleChar(keyEvent.Char);
            }
            else
            {
                switch (keyEvent.Code)
                {
                    case KeyCode.Enter:
                        OnActivate?.Invoke();
                        break;

                    case KeyCode.Left:
                        HandleLeft();
                        break;

                    case KeyCode.Right:
                        HandleRight();
                        break;

                    case KeyCode.Backspace:
                        HandleBackspace();
                        break;

                    case KeyCode.Delete:
                        HandleDelete();
                        break;
                }
            }
        }

        private void HandleChar(char c)
        {
            ResetCursorBlink();

            if (MaxLength > 0 && _text.Length >= MaxLength)
            {
                return;
            }

            c = char.ToUpperInvariant(c);

            if (Font.GetGlyph(c) == null)
            {
                return;
            }

            _text = _text.Substring(0, _cursorPosition) + c + _text.Substring(_cursorPosition, _text.Length - _cursorPosition);
            ++_cursorPosition;
            OnChange?.Invoke(_text);
        }

        private void HandleLeft()
        {
            ResetCursorBlink();
            if (_cursorPosition > 0)
            {
                --_cursorPosition;
            }
        }

        private void HandleRight()
        {
            ResetCursorBlink();
            if (_cursorPosition < _text.Length)
            {
                ++_cursorPosition;
            }
        }

        private void HandleBackspace()
        {
            ResetCursorBlink();
            if (_text.Length > 0 && _cursorPosition > 0)
            {
                _text = _text.Substring(0, _cursorPosition - 1) + _text.Substring(_cursorPosition, _text.Length - _cursorPosition);
                --_cursorPosition;
                OnChange?.Invoke(_text);
            }
        }

        private void HandleDelete()
        {
            ResetCursorBlink();
            if (_text.Length > 0 && _cursorPosition < _text.Length)
            {
                _text = _text.Substring(0, _cursorPosition) + _text.Substring(_cursorPosition + 1, _text.Length - _cursorPosition - 1);
                OnChange?.Invoke(_text);
            }
        }

        private void ResetCursorBlink()
        {
            _cursorBlinkTime = Stopwatch.GetTimestamp();
        }
    }
}
