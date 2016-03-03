// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Network;
using System;
using System.Collections.Generic;
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
        private readonly Task<Endpoint<Packet>> _endpointTask;

        private Endpoint<Packet> _endpoint;

        private PlayerInfo _myPlayerInfo;
        private Dictionary<PlayerId, PlayerInfo> _players = new Dictionary<PlayerId, PlayerInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Lobby"/> class.
        /// </summary>
        /// <param name="goBack">The action invoked to navigate back in the menu.</param>
        /// <param name="endpointTask">The task that will return the endpoint.</param>
        public Lobby(Action goBack, Task<Endpoint<Packet>> endpointTask)
        {
            _goBack = () => Task.Run(
                async () =>
                {
                    try
                    {
                        if (!_endpointTask.IsCanceled && !_endpointTask.IsFaulted)
                        {
                            (await _endpointTask).Dispose();
                        }
                    }
                    catch
                    {
                    }

                    goBack();
                });

            _endpointTask = endpointTask;
        }

        /// <inheritdoc/>
        public void Update(Terminal terminal)
        {
            if (!_endpointTask.IsCompleted)
            {
                if (terminal.Input.Back)
                {
                    _goBack();
                    return;
                }

                terminal.TextCenter(_AnnouncementColor, 8, _Center, "CONNECTING...");
                return;
            }

            if (_endpointTask.IsFaulted || _endpointTask.IsCanceled || !_endpointTask.Result.IsConnected)
            {
                if (terminal.Input.Back || terminal.Input.Fire || terminal.Input.AltFire)
                {
                    _goBack();
                    return;
                }

                terminal.TextCenter(_AnnouncementColor, 8, _Center, _endpoint == null ? "CONNECTION FAILED!" : "CONNECTION LOST!");
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
            double x = 320 - 12 * fontSize;
            double y = 160;
            foreach (var playerInfo in _players.Values)
            {
                terminal.Text(_AnnouncementColor, fontSize, new Vector(x, y), playerInfo.Name);
                y -= fontSize;
            }
        }

        private class PlayerInfo
        {
            public PlayerId Id;

            public string Name;
        }
    }
}
