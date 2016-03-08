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

        /// <summary>
        /// The y coordinate of the title line.
        /// </summary>
        public const double TitleY = 32;

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

            ProcessInput(terminal.KeyEvents);

            SelectedIndex = (SelectedIndex + items) % items;

            double stride = FontSize + 8;

            double top = 100 - 0.5 * (stride * (items - 1) + FontSize);

            terminal.Graphics.PrintCenter(Colors.Title, FontSize, new Vector(160, TitleY), Title);

            for (int i = 0; i < items; ++i)
            {
                Items[i]?.Update(terminal, new Vector(160, top + i * stride), SelectedIndex == i);
            }
        }

        private void ProcessInput(List<KeyEvent> keyEvents)
        {
            foreach (var keyEvent in keyEvents)
            {
                switch (keyEvent.Code)
                {
                    case KeyCode.Up:
                        --SelectedIndex;
                        break;

                    case KeyCode.Down:
                        ++SelectedIndex;
                        break;

                    case KeyCode.Escape:
                        OnBack?.Invoke();
                        break;

                    default:
                        Items?[SelectedIndex]?.HandleKeyEvent(keyEvent);
                        break;
                }
            }
        }
    }
}
