// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Engine
{
    /// <summary>
    /// A view that is displayed on a terminal.
    /// </summary>
    internal interface IView
    {
        /// <summary>
        /// Gets a value indicating whether to show the background.</summary>
        bool ShowBackground { get; }

        /// <summary>
        /// Updates and renders the view.
        /// </summary>
        /// <param name="terminal">The terminal this view should render to.</param>
        void Update(Terminal terminal);
    }
}
