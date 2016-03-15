// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Application.Ui;
using NanoGames.Engine;
using NanoGames.Network;
using NanoGames.Synchronization;
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
        private IView _background = new StarfieldView();

        private IView _currentView;

        private Menu _mainMenu;
        private Menu _multiplayerMenu;

        private string _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            Window.Current.IsFullscreen = Settings.Instance.IsFullscreen;
            Window.Current.IsVSynced = Settings.Instance.IsVSynced;

            _mainMenu = new Menu("NANOGAMES")
            {
                OnBack = OnQuit,
                Items =
                {
                    new CommandMenuItem("MULTIPLAYER", () => _currentView = _multiplayerMenu),
                    new CommandMenuItem("PRACTICE", () => _currentView = new PracticeView(() => _currentView = _mainMenu)),
                    new CommandMenuItem("SETTINGS", () => _currentView = new SettingsView(() => _currentView = _mainMenu)),
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
                            _currentView = new TournamentView(
                                () => _currentView = _multiplayerMenu,
                                new Tournament(Task.Run(() => new Server().GetLocalEndpoint<PacketData>())));
                        }),
                    new TextMenuItem("CONNECT TO")
                    {
                        Text = _server,
                        OnChange = v => _server = v,
                        OnActivate = () =>
                        {
                            Settings.Instance.LastServer = _server;
                            _currentView = new TournamentView(
                                () => _currentView = _multiplayerMenu,
                                new Tournament(Client<PacketData>.ConnectAsync(_server)));
                        },
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
