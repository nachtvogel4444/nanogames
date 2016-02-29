// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Menu
{
    /// <summary>
    /// A menu item giving the user a choice between several alternatives.
    /// </summary>
    /// <typeparam name="T">The type of the choice value.</typeparam>
    internal sealed class ChoiceMenuItem<T> : MenuItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChoiceMenuItem{T}"/> class.
        /// </summary>
        /// <param name="text">The menu item text.</param>
        public ChoiceMenuItem(string text)
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
        public List<Choice<T>> Choices { get; set; } = new List<Choice<T>>();

        /// <summary>
        /// Gets or sets an action invoked when a new item is selected.
        /// </summary>
        public Action<T> OnSelect { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Gets the selected choice.
        /// </summary>
        public Choice<T> SelectedChoice
        {
            get
            {
                if (SelectedIndex >= 0 && SelectedIndex < Choices.Count)
                {
                    return Choices[SelectedIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the selected value.
        /// </summary>
        public T SelectedValue
        {
            get
            {
                var choice = SelectedChoice;
                return choice == null ? default(T) : choice.Value;
            }
        }

        /// <inheritdoc/>
        public override void HandleActivate()
        {
            HandleRight();
        }

        /// <inheritdoc/>
        public override void HandleLeft()
        {
            if (Choices != null && Choices.Count > 0)
            {
                SelectedIndex = (SelectedIndex + Choices.Count - 1) % Choices.Count;
                InvokeOnSelect();
            }
        }

        /// <inheritdoc/>
        public override void HandleRight()
        {
            if (Choices != null && Choices.Count > 0)
            {
                SelectedIndex = (SelectedIndex + 1) % Choices.Count;
                InvokeOnSelect();
            }
        }

        /// <inheritdoc/>
        public override void Update(Terminal terminal, Vector position, bool isSelected)
        {
            string text = Text + ": " + (SelectedChoice?.Text ?? "NONE");

            if (isSelected)
            {
                terminal.TextCenter(SelectedItemColor, Menu.FontSize, position, "< " + text + " >");
            }
            else
            {
                terminal.TextCenter(ItemColor, Menu.FontSize, position, text);
            }
        }

        private void InvokeOnSelect()
        {
            OnSelect?.Invoke(SelectedValue);
        }
    }
}
