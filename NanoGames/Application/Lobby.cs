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
    internal sealed class Lobby : IView
    {
        private static readonly Color _AnnouncementColor = new Color(1, 1, 1);

        private static readonly Color _ScoreColor = new Color(0.5, 0.5, 0.5);

        private static readonly Vector _Center = new Vector(160, 86);

        private readonly Action _goBack;
        private readonly Menu _menu;

        private readonly Task<Endpoint<Packet>> _endpointTask;

        private IView _currentView;
        private Endpoint<Packet> _endpoint;

        private PlayerInfo _myPlayerInfo;
        private Dictionary<PlayerId, PlayerInfo> _players = new Dictionary<PlayerId, PlayerInfo>();

        private bool _fireWasPressed = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lobby"/> class.
        /// </summary>
        /// <param name="goBack">The action invoked to navigate back in the menu.</param>
        /// <param name="endpointTask">The task that will return the endpoint.</param>
        public Lobby(Action goBack, Task<Endpoint<Packet>> endpointTask)
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
            if (_currentView == null && terminal.Input.Back)
            {
                _currentView = _menu;
            }

            if (_currentView != null)
            {
                _currentView.Update(terminal);

                /* Run the rest of the update method, but hide the output. */
                terminal = Terminal.Null;
            }

            if (!_endpointTask.IsCompleted)
            {
                if (terminal.Input.Back || terminal.Input.AltFire)
                {
                    _goBack();
                    return;
                }

                terminal.TextCenter(Colors.Title, 8, new Vector(160, 150), "CONNECTING");
                terminal.TextCenter(Colors.FocusedControl, 8, new Vector(160, 86), "CANCEL");
                return;
            }

            if (_endpointTask.IsFaulted || _endpointTask.IsCanceled || !_endpointTask.Result.IsConnected)
            {
                if (terminal.Input.Back || terminal.Input.Fire || terminal.Input.AltFire)
                {
                    _goBack();
                    return;
                }

                terminal.TextCenter(Colors.Error, 8, new Vector(160, 150), _endpoint == null ? "CONNECTION FAILED" : "CONNECTION LOST");
                terminal.TextCenter(Colors.FocusedControl, 8, new Vector(160, 86), "BACK");
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

            if (terminal.Input.Fire)
            {
                if (!_fireWasPressed)
                {
                    _fireWasPressed = true;
                    _myPlayerInfo.IsReady = !_myPlayerInfo.IsReady;
                }
            }
            else
            {
                _fireWasPressed = false;
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
            double y = 90 + 0.5 * _players.Count * fontSize - fontSize;

            foreach (var playerInfo in _players.Values.OrderBy(p => p.Score).ThenBy(p => p.Name, StringComparer.InvariantCultureIgnoreCase))
            {
                terminal.Text(Colors.Control, fontSize, new Vector(x + fontSize, y), playerInfo.Name);
                terminal.Text(
                    new Color(0.7, 0.7, 0.7),
                    fontSize,
                    new Vector(x + (Settings.MaxPlayerNameLength + 2) * fontSize, y),
                    playerInfo.Score.ToString("0000", CultureInfo.InvariantCulture));

                if (playerInfo.IsReady)
                {
                    terminal.Text(Colors.Title, fontSize, new Vector(x, y), "✓");
                }

                y -= fontSize;
            }

            if (_myPlayerInfo.IsReady)
            {
                terminal.TextCenter(Colors.Title, 8, new Vector(160, 150), "WAITING");
            }
            else
            {
                terminal.TextCenter(Colors.Error, 8, new Vector(160, 150), "SPACE TO START");
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
