// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Menu
{
    /// <summary>
    /// The game's main menu.
    /// </summary>
    internal sealed class MainMenu : IView, IDisposable
    {
        private IView _fpsView = new FpsView();
        private IView _background = new Backgrounds.Starfield();

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public void Refresh(Terminal terminal)
        {
            _fpsView.Refresh(terminal);

            terminal.Line(new Color(0.5, 0, 0), new Vector(10, 10), new Vector(200, 10));
            terminal.Line(new Color(0.5, 0, 0.5), new Vector(10, 10), new Vector(310, 150));

            terminal.Line(new Color(0, 0.5, 0.8), new Vector(10, 60), new Vector(200, 60));
            terminal.Line(new Color(0, 0.5, 0.8), new Vector(200, 60), new Vector(310, 60));

            terminal.Line(new Color(0.8, 1.0, 0.2), new Vector(0, 108), new Vector(320, 108));
            for (int i = 0; i < 20; ++i)
            {
                terminal.Line(new Color(0.8, 1.0, 0.2), new Vector(8 + 16 * i, 92), new Vector(8 + 16 * i, 124));
            }

            _background.Refresh(terminal);
        }
    }
}
