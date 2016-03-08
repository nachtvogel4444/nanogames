// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Application.Ui;
using NanoGames.Engine;
using NanoGames.Network;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGames.Application
{
    /// <summary>
    /// Represents the game lobby.
    /// </summary>
    internal sealed class LobbyView : IView
    {
        private readonly Action _goBack;
        private readonly Menu _menu;

        private readonly Task<Endpoint<Packet>> _endpointTask;

        private IView _currentView;
        private Endpoint<Packet> _endpoint;

        private PlayerInfo _myPlayerInfo;
        private Dictionary<PlayerId, PlayerInfo> _players = new Dictionary<PlayerId, PlayerInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LobbyView"/> class.
        /// </summary>
        /// <param name="goBack">The action invoked to navigate back in the menu.</param>
        /// <param name="endpointTask">The task that will return the endpoint.</param>
        public LobbyView(Action goBack, Task<Endpoint<Packet>> endpointTask)
        {
            _goBack = () =>
            {
                if (!_endpointTask.IsCanceled && !_endpointTask.IsFaulted)
                {
                    Task.Run(
                    async () =>
                    {
                        try
                        {
                            (await _endpointTask).Dispose();
                        }
                        catch
                        {
                        }
                    });
                }

                goBack();
            };

            _menu = new Menu("NANOGAMES")
            {
                OnBack = () => _currentView = null,
                Items =
                {
                    new CommandMenuItem("BACK", () => _currentView = null),
                    new CommandMenuItem("SETTINGS", () => _currentView = new SettingsView(() => _currentView = _menu)),
                    new CommandMenuItem("DISCONNECT", _goBack),
                },
            };

            _endpointTask = endpointTask;
        }

        /// <inheritdoc/>
        public void Update(Terminal terminal)
        {
            bool escape = false;
            bool enter = false;
            foreach (var keyEvent in terminal.KeyEvents)
            {
                switch (keyEvent.Code)
                {
                    case KeyCode.Escape:
                        escape = true;
                        break;

                    case KeyCode.Enter:
                        enter = true;
                        break;
                }
            }

            if (_currentView == null && escape)
            {
                _currentView = _menu;
                terminal.KeyEvents.Clear();
            }

            if (_currentView != null)
            {
                _currentView.Update(terminal);

                /* Run the rest of the update method, but hide the output. */
                terminal = new Terminal(null);
            }

            if (!_endpointTask.IsCompleted)
            {
                if (escape || enter)
                {
                    _goBack();
                    return;
                }

                terminal.Graphics.PrintCenter(Colors.Title, 8, new Vector(160, Menu.TitleY), "CONNECTING");
                terminal.Graphics.PrintCenter(Colors.FocusedControl, 8, new Vector(160, 96), "CANCEL");
                return;
            }

            if (_endpointTask.IsFaulted || _endpointTask.IsCanceled || !_endpointTask.Result.IsConnected)
            {
                if (escape || enter)
                {
                    _goBack();
                    return;
                }

                terminal.Graphics.PrintCenter(Colors.Error, 8, new Vector(160, Menu.TitleY), _endpoint == null ? "CONNECTION FAILED" : "CONNECTION LOST");
                terminal.Graphics.PrintCenter(Colors.FocusedControl, 8, new Vector(160, 96), "BACK");
                return;
            }

            if (_endpoint == null)
            {
                _endpoint = _endpointTask.Result;

                _myPlayerInfo = new PlayerInfo { Id = PlayerId.Create(), Name = Settings.Instance.PlayerName };
                _players[_myPlayerInfo.Id] = _myPlayerInfo;

                SendAnnouncement();
            }

            Packet packet;
            while (_endpoint.TryReceive(out packet))
            {
                HandlePacket(packet);
            }

            if (enter)
            {
                _myPlayerInfo.IsReady = !_myPlayerInfo.IsReady;
            }

            UpdateLobby(terminal);
        }

        private void SendAnnouncement()
        {
            _endpoint.Send(new Packet { PlayerId = _myPlayerInfo.Id, PlayerName = _myPlayerInfo.Name });
        }

        private void HandlePacket(Packet packet)
        {
            if (packet.PlayerId != default(PlayerId) && !_players.ContainsKey(packet.PlayerId))
            {
                _players[packet.PlayerId] = new PlayerInfo { Name = packet.PlayerName };
                SendAnnouncement();
            }
        }

        private void UpdateLobby(Terminal terminal)
        {
            double fontSize = 8;
            double x = 160 - 0.5 * ((Settings.MaxPlayerNameLength + 6) * fontSize);
            double y = 100 - 0.5 * _players.Count * fontSize;

            foreach (var playerInfo in _players.Values.OrderBy(p => p.Score).ThenBy(p => p.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                terminal.Graphics.Print(Colors.Control, fontSize, new Vector(x + fontSize, y), playerInfo.Name);
                terminal.Graphics.Print(
                    new Color(0.7, 0.7, 0.7),
                    fontSize,
                    new Vector(x + (Settings.MaxPlayerNameLength + 2) * fontSize, y),
                    playerInfo.Score.ToString("0000", CultureInfo.InvariantCulture));

                if (playerInfo.IsReady)
                {
                    terminal.Graphics.Print(Colors.Title, fontSize, new Vector(x, y), "✓");
                }

                y += fontSize;
            }

            if (_myPlayerInfo.IsReady)
            {
                terminal.Graphics.PrintCenter(Colors.Title, 8, new Vector(160, Menu.TitleY), "WAITING");
            }
            else
            {
                terminal.Graphics.PrintCenter(Colors.Error, 8, new Vector(160, Menu.TitleY), "ENTER TO START");
            }
        }

        private class PlayerInfo
        {
            public PlayerId Id;

            public string Name;

            public bool IsReady;

            public int Score = 0;
        }
    }
}
