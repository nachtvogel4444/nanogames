// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;

namespace NanoGames.Application.Ui
{
    /// <summary>
    /// Represents an item in a <see cref="Menu"/>.
    /// </summary>
    internal abstract class MenuItem
    {
        /// <summary>
        /// Updates and renders the current menu item.
        /// </summary>
        /// <param name="terminal">The terminal to render to.</param>
        /// <param name="position">The menu item's position.</param>
        /// <param name="isSelected">A value indicating whether the menu item is currently selected.</param>
        public abstract void Update(Terminal terminal, Vector position, bool isSelected);

        /// <summary>
        /// Called when the menu item is activated.
        /// </summary>
        public virtual void HandleActivate()
        {
        }

        /// <summary>
        /// Called when the user navigates left.
        /// </summary>
        public virtual void HandleLeft()
        {
        }

        /// <summary>
        /// Called when the user navigates right.
        /// </summary>
        public virtual void HandleRight()
        {
        }

        /// <summary>
        /// Called when the user enters text.
        /// </summary>
        /// <param name="text">The text entered by the user.</param>
        public virtual void HandleText(string text)
        {
        }

        /// <summary>
        /// Called when the user presses backspace.
        /// </summary>
        public virtual void HandleBackspace()
        {
        }

        /// <summary>
        /// Called when the user presses delete.
        /// </summary>
        public virtual void HandleDelete()
        {
        }
    }
}
