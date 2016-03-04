// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using NanoGames.Network;
using NanoGames.Ui;
using System;
using System.Threading.Tasks;

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
        private Menu _multiplayerMenu;
        private Menu _settingsMenu;

        private string _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            Window.Current.IsFullscreen = Settings.Instance.IsFullscreen;

            _mainMenu = new Menu("NANOGAMES")
            {
                OnBack = OnQuit,
                Items =
                {
                    new CommandMenuItem("MULTIPLAYER", () => _currentView = _multiplayerMenu),
                    new CommandMenuItem("PRACTICE", () => _currentView = new PracticeView(() => _currentView = _mainMenu)),
                    new CommandMenuItem("SETTINGS", () => _currentView = _settingsMenu),
                    new CommandMenuItem("QUIT", OnQuit),
                },
            };

            _server = Settings.Instance.LastServer;
            _multiplayerMenu = new Menu("MULTIPLAYER")
            {
                OnBack = () => _currentView = _mainMenu,
                Items =
                {
                    new CommandMenuItem(
                        "HOST GAME",
                        () =>
                        {
                            _currentView = new Lobby(
                                () => _currentView = _multiplayerMenu,
                                Task.Run(() => new Server().GetLocalEndpoint<Packet>()));
                        }),
                    new TextMenuItem("CONNECT TO")
                    {
                        Text = _server,
                        OnChange = v => _server = v,
                        OnActivate = () =>
                        {
                            Settings.Instance.LastServer = _server;
                            _currentView = new Lobby(
                                () => _currentView = _multiplayerMenu,
                                Client<Packet>.ConnectAsync(_server));
                        },
                    },
                    new CommandMenuItem("BACK", () => _currentView = _mainMenu),
                },
            };

            _settingsMenu = new Menu("SETTINGS")
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
