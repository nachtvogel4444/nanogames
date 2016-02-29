// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using System;

namespace NanoGames.Menu
{
    /// <summary>
    /// The game's main menu.
    /// </summary>
    internal sealed class MainView : IView, IDisposable
    {
        private IView _fpsView = new FpsView();
        private IView _background = new Backgrounds.Starfield();

        private Menu _menu;

        private Menu _mainMenu;
        private Menu _settingsMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        ///
        /// </summary>
        public MainView()
        {
            _mainMenu = new Menu
            {
                OnBack = OnQuit,
                Items =
                {
                    new CommandMenuItem("SETTINGS", () => _menu = _settingsMenu),
                    new CommandMenuItem("QUIT", OnQuit),
                },
            };

            _settingsMenu = new Menu
            {
                OnBack = () => _menu = _mainMenu,
                SelectedIndex = 1,
                Items =
                {
                    new ChoiceMenuItem<bool>("FULLSCREEN")
                    {
                        Choices =
                        {
                            new Choice<bool>(false, "NO"),
                            new Choice<bool>(true, "YES"),
                        },
                        OnSelect = v => Window.Current.IsFullscreen = v,
                    },
                    new CommandMenuItem("BACK", () => _menu = _mainMenu),
                },
            };

            _menu = _mainMenu;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public void Update(Terminal terminal)
        {
            _fpsView.Update(terminal);
            _menu?.Update(terminal);
            _background.Update(terminal);
        }

        private void OnQuit()
        {
            throw new QuitException();
        }
    }
}
