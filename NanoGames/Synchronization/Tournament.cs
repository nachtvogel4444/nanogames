// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Network;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Represents the state of a tournament.
    /// </summary>
    internal sealed class Tournament
    {
        private readonly Task<Endpoint<Packet>> _endpointTask;

        private readonly Dictionary<PlayerId, PlayerState> _players = new Dictionary<PlayerId, PlayerState>();

        private string _sentPlayerName = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tournament"/> class.
        /// </summary>
        /// <param name="endpointTask">A task returning the network endpoint used to communicate.</param>
        public Tournament(Task<Endpoint<Packet>> endpointTask)
        {
            _endpointTask = endpointTask;
            LocalPlayer = new PlayerState(PlayerId.Create());
            _players[LocalPlayer.Id] = LocalPlayer;
        }

        /// <summary>
        /// Gets a value indicating whether the tournament is currently connecting to the network.
        /// </summary>
        public bool IsConnecting { get; private set; } = true;

        /// <summary>
        /// Gets a value indicating whether the tournament is connected to the network.
        /// </summary>
        public bool IsConnected { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the tournament was ever connected to the network.
        /// </summary>
        public bool WasConnected { get; private set; } = false;

        /// <summary>
        /// Gets a collection of all players in the tournament.
        /// </summary>
        public IReadOnlyCollection<PlayerState> Players => _players.Values;

        /// <summary>
        /// Gets the state of the local player.
        /// </summary>
        public PlayerState LocalPlayer { get; private set; }

        /// <summary>
        /// Disposes this object asynchronously.
        /// </summary>
        /// <returns>A value representing the asynchronous task.</returns>
        public async Task DisposeAsync()
        {
            if (!_endpointTask.IsCanceled || !_endpointTask.IsFaulted)
            {
                try
                {
                    (await _endpointTask).Dispose();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Updates the state of the tournament.
        /// </summary>
        /// <param name="gameInput">The current input to the game, if any.</param>
        public void Update(Input gameInput)
        {
            if (!_endpointTask.IsCompleted)
            {
                return;
            }
            else if (_endpointTask.IsCanceled || _endpointTask.IsFaulted)
            {
                IsConnecting = false;
                return;
            }

            IsConnecting = false;
            var endpoint = _endpointTask.Result;

            if (!endpoint.IsConnected)
            {
                IsConnected = false;
                return;
            }

            if (!IsConnected)
            {
                IsConnected = true;
                WasConnected = true;
            }

            Packet packet;
            while (endpoint.TryReceive(out packet))
            {
                HandlePacket(packet);
            }

            SendPlayerState(endpoint);
        }

        private void SendPlayerState(Endpoint<Packet> endpoint)
        {
            var packet = new Packet();
            packet.PlayerId = LocalPlayer.Id;

            if (LocalPlayer.Name != _sentPlayerName)
            {
                packet.PlayerName = LocalPlayer.Name;
                _sentPlayerName = LocalPlayer.Name;
            }

            packet.IsReady = LocalPlayer.IsReady;

            endpoint.Send(packet);
        }

        private void HandlePacket(Packet packet)
        {
            PlayerState playerState;
            if (!_players.TryGetValue(packet.PlayerId, out playerState))
            {
                /* New player, resend all our info. */
                _sentPlayerName = null;

                playerState = new PlayerState(packet.PlayerId);
                _players[packet.PlayerId] = playerState;
            }

            if (packet.PlayerName != null)
            {
                playerState.Name = packet.PlayerName;
            }

            playerState.TournamentScore = packet.TournamentScore;
            playerState.IsReady = packet.IsReady;
        }
    }
}
