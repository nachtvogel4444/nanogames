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
        /// Handles a key event.
        /// </summary>
        /// <param name="keyEvent">The key event to handle.</param>
        public virtual void HandleKeyEvent(KeyEvent keyEvent)
        {
        }
    }
}
