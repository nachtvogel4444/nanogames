// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Application.Ui
{
    /// <summary>
    /// A menu item that executes a command when selected.
    /// </summary>
    internal sealed class CommandMenuItem : MenuItem
    {
        private readonly Action _onActivate;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMenuItem"/> class.
        /// </summary>
        /// <param name="text">The text on the menu item.</param>
        /// <param name="onActivate">The action to invoke when the menu item is activated.</param>
        public CommandMenuItem(string text, Action onActivate)
        {
            Text = text;
            _onActivate = onActivate;
        }

        /// <summary>
        /// Gets or sets the text on the menu item.
        /// </summary>
        public string Text { get; set; }

        /// <inheritdoc/>
        public override void Update(Terminal terminal, Vector position, bool isSelected)
        {
            var color = isSelected ? Colors.FocusedControl : Colors.Control;
            terminal.TextCenter(color, Menu.FontSize, position, Text);
        }

        /// <inheritdoc/>
        public override void HandleActivate()
        {
            _onActivate?.Invoke();
        }
    }
}
