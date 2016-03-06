// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using System;
using System.Collections.Generic;

namespace NanoGames.Application.Ui
{
    /// <summary>
    /// Represents a menu that the player can navigate.
    /// </summary>
    internal sealed class Menu : IView
    {
        /// <summary>
        /// The font size used inside the menu.
        /// </summary>
        public const double FontSize = 8;

        private const int _keyRepeatFrames = 12;

        private int _framesSinceKeyPress;

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        /// <param name="title">The menu title.</param>
        public Menu(string title)
        {
            Title = title;
        }

        /// <summary>
        /// Gets or sets the menu title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the action invoked when the user navigates back.
        /// </summary>
        public Action OnBack { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected menu item.
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Gets or sets the list of menu items.
        /// </summary>
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();

        /// <inheritdoc/>
        public void Update(Terminal terminal)
        {
            int items = Items?.Count ?? 0;
            if (items == 0)
            {
                return;
            }

            ProcessInput(terminal.Input);

            SelectedIndex = (SelectedIndex + items) % items;

            double stride = FontSize + 8;

            double top = 90 + 0.5 * (stride * (items - 1) + FontSize) - FontSize;

            terminal.TextCenter(Colors.Title, FontSize, new Vector(160, 150), Title);

            for (int i = 0; i < items; ++i)
            {
                Items[i]?.Update(terminal, new Vector(160, top - i * stride), SelectedIndex == i);
            }
        }

        private void ProcessInput(Input input)
        {
            if (_framesSinceKeyPress < _keyRepeatFrames)
            {
                ++_framesSinceKeyPress;
            }

            if (input.Text != null)
            {
                Items?[SelectedIndex]?.HandleText(input.Text);
            }

            if (input.Back || input.AltFire)
            {
                if (_framesSinceKeyPress >= _keyRepeatFrames)
                {
                    OnBack?.Invoke();
                    _framesSinceKeyPress = 0;
                }
            }
            else if (input.Down)
            {
                if (_framesSinceKeyPress >= _keyRepeatFrames)
                {
                    ++SelectedIndex;
                    _framesSinceKeyPress = 0;
                }
            }
            else if (input.Up)
            {
                if (_framesSinceKeyPress >= _keyRepeatFrames)
                {
                    --SelectedIndex;
                    _framesSinceKeyPress = 0;
                }
            }
            else if (input.Fire)
            {
                if (_framesSinceKeyPress >= _keyRepeatFrames)
                {
                    Items?[SelectedIndex]?.HandleActivate();
                    _framesSinceKeyPress = 0;
                }
            }
            else if (input.Left)
            {
                if (_framesSinceKeyPress >= _keyRepeatFrames)
                {
                    Items?[SelectedIndex]?.HandleLeft();
                    _framesSinceKeyPress = 0;
                }
            }
            else if (input.Right)
            {
                if (_framesSinceKeyPress >= _keyRepeatFrames)
                {
                    Items?[SelectedIndex]?.HandleRight();
                    _framesSinceKeyPress = 0;
                }
            }
            else if (input.Backspace)
            {
                if (_framesSinceKeyPress >= _keyRepeatFrames)
                {
                    Items?[SelectedIndex]?.HandleBackspace();
                    _framesSinceKeyPress = 0;
                }
            }
            else if (input.Delete)
            {
                if (_framesSinceKeyPress >= _keyRepeatFrames)
                {
                    Items?[SelectedIndex]?.HandleDelete();
                    _framesSinceKeyPress = 0;
                }
            }
            else
            {
                _framesSinceKeyPress = _keyRepeatFrames;
            }
        }
    }
}
