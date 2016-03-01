// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using NanoGames.Ui;
using System;

namespace NanoGames.Application
{
    /// <summary>
    /// The game's main menu.
    /// </summary>
    internal sealed class MainView : IView, IDisposable
    {
        private IView _fpsView = new FpsView();
        private IView _background = new Backgrounds.Starfield();

        private IView _currentView;

        private Menu _mainMenu;
        private Menu _settingsMenu;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            Window.Current.IsFullscreen = Settings.Instance.IsFullscreen;

            _mainMenu = new Menu
            {
                OnBack = OnQuit,
                Items =
                {
                    new CommandMenuItem("PRACTICE", () => _currentView = new PracticeView(() => _currentView = _mainMenu)),
                    new CommandMenuItem("SETTINGS", () => _currentView = _settingsMenu),
                    new CommandMenuItem("QUIT", OnQuit),
                },
            };

            _settingsMenu = new Menu
            {
                OnBack = () => _currentView = _mainMenu,
                SelectedIndex = 1,
                Items =
                {
                    new TextMenuItem("NAME")
                    {
                        Text = Settings.Instance.PlayerName,
                        MaxLength = 12,
                        OnChange = value => Settings.Instance.PlayerName = value,
                    },
                    new ChoiceMenuItem<bool>("FULLSCREEN")
                    {
                        Choices =
                        {
                            new Choice<bool>(false, "NO"),
                            new Choice<bool>(true, DebugMode.IsEnabled ? "DEBUG" : "YES"),
                        },
                        SelectedValue = Settings.Instance.IsFullscreen,
                        OnSelect = v =>
                        {
                            Window.Current.IsFullscreen = v;
                            Settings.Instance.IsFullscreen = v;
                        },
                    },
                    new ChoiceMenuItem<bool>("SHOW FPS")
                    {
                        Choices =
                        {
                            new Choice<bool>(false, "NO"),
                            new Choice<bool>(true, "YES"),
                        },
                        SelectedValue = Settings.Instance.ShowFps,
                        OnSelect = value => Settings.Instance.ShowFps = value,
                    },
                    new CommandMenuItem("BACK", () => _currentView = _mainMenu),
                },
            };

            _currentView = _mainMenu;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }

        /// <inheritdoc/>
        public void Update(Terminal terminal)
        {
            _fpsView.Update(terminal);
            _currentView?.Update(terminal);
            _background.Update(terminal);
        }

        private void OnQuit()
        {
            throw new QuitException();
        }
    }
}
