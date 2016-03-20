// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using System;
using System.Collections.Generic;

namespace NanoGames.Application.Ui
{
    /// <summary>
    /// A menu item that allows selecting a color.
    /// </summary>
    internal sealed class ColorMenuItem : MenuItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorMenuItem"/> class.
        /// </summary>
        /// <param name="text">The menu item text.</param>
        public ColorMenuItem(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Gets or sets the menu item text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the list of choices.
        /// </summary>
        public List<Color> Colors { get; set; } = new List<Color>();

        /// <summary>
        /// Gets or sets an action invoked when a new item is selected.
        /// </summary>
        public Action<Color> OnSelect { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Gets or sets the selected color.
        /// </summary>
        public Color SelectedColor
        {
            get
            {
                if (SelectedIndex >= 0 && SelectedIndex < Colors.Count)
                {
                    return Colors[SelectedIndex];
                }
                else
                {
                    return default(Color);
                }
            }

            set
            {
                int index = 0;
                if (Colors != null)
                {
                    for (int i = 0; i < Colors.Count; ++i)
                    {
                        if (Colors[i] == value)
                        {
                            index = i;
                            break;
                        }
                    }
                }

                SelectedIndex = index;
            }
        }

        /// <inheritdoc/>
        public override void HandleKeyEvent(KeyEvent keyEvent)
        {
            if (Colors != null && Colors.Count > 0)
            {
                switch (keyEvent.Code)
                {
                    case KeyCode.Left:
                        SelectedIndex = (SelectedIndex + Colors.Count - 1) % Colors.Count;
                        InvokeOnSelect();
                        break;

                    case KeyCode.Right:
                    case KeyCode.Enter:
                        SelectedIndex = (SelectedIndex + 1) % Colors.Count;
                        InvokeOnSelect();
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public override void Update(Terminal terminal, Vector position, bool isSelected)
        {
            if (isSelected)
            {
                terminal.Graphics.PrintCenter(SelectedColor, Menu.FontSize, position, "< " + Text + " >");
            }
            else
            {
                terminal.Graphics.PrintCenter(SelectedColor, Menu.FontSize, position, Text);
            }
        }

        private void InvokeOnSelect()
        {
            OnSelect?.Invoke(SelectedColor);
        }
    }
}
