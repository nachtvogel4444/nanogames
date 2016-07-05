// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Engine;
using NanoGames.Engine.OutputSystems;
using NanoGames.Games;
using NanoGames.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGames.Synchronization
{
    /// <summary>
    /// Represents the state of a tournament.
    /// </summary>
    internal sealed class Tournament
    {
        /*
         * This is basically a huge state machine that attempts to figure out the current state of the tournament
         * based on what it knows and what it receives from other players.
         */

        private const int _latencyFrames = 1;

        private const int _winScore = 20;

        private const int _tieScore = 10;

        private const double _scoreDecay = 0.875;

        private readonly Random _random = new Random();

        private readonly Task<Endpoint<PacketData>> _endpointTask;

        private readonly Dictionary<PlayerId, PlayerState> _players = new Dictionary<PlayerId, PlayerState>();

        private string _sentPlayerName = null;

        private Color _sentPlayerColor = default(Color);

        private int _roundPriority;

        private long _roundStartTimestamp;

        private int _roundSeed;

        private int _lastRoundSeed;

        private Random _roundRandom;

        private int _localPlayerIndex = -1;

        private int _lastUpdatedFrame;

        private MatchBuffer _matchBuffer;

        private List<Discipline> _voteOptions = new List<Discipline>();

        private List<InputEntry> _inputEntries = new List<InputEntry>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Tournament"/> class.
        /// </summary>
        /// <param name="endpointTask">A task returning the network endpoint used to communicate.</param>
        public Tournament(Task<Endpoint<PacketData>> endpointTask)
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
        /// Gets the current phase of the tournament.
        /// </summary>
        public TournamentPhase TournamentPhase { get; private set; }

        /// <summary>
        /// Gets the Stopwatch timestamp where the next tournament phase starts.
        /// </summary>
        public long NextPhaseTimestamp { get; private set; }

        /// <summary>
        /// Gets the options the players can choose between. The first element is always null, which means "skip this round".
        /// </summary>
        public IReadOnlyList<Discipline> VoteOptions => _voteOptions.AsReadOnly();

        /// <summary>
        /// Gets the name of the current discipline.
        /// </summary>
        public string DiscipleName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a round is in progress.
        /// </summary>
        public bool IsRoundInProgress => _roundSeed != 0 || _players.Any(p => p.Value.IsInMatch);

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
        /// <param name="terminal">The local player's terminal.</param>
        public void Update(Terminal terminal)
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

            Packet<PacketData> packet;
            while (endpoint.TryReceive(out packet))
            {
                HandlePacket(packet);
            }

            RemoveDisconnectedPlayers();

            /* We suggest a new round if no other round was started and at least 2/3 of the players are ready. */
            if (_roundSeed == 0 && _players.Count >= 2 && _players.Values.Count(p => p.IsReady) * 3 >= 2 * _players.Count)
            {
                _roundSeed = _random.Next();
                _roundPriority = _random.Next();
                _roundStartTimestamp = Stopwatch.GetTimestamp();
            }

            if (_roundSeed == 0)
            {
                TournamentPhase = TournamentPhase.Lobby;
            }
            else
            {
                UpdateRound(terminal);
            }

            SendPlayerState(endpoint);
        }

        private void UpdateRound(Terminal terminal)
        {
            var roundDuration = Stopwatch.GetTimestamp() - _roundStartTimestamp;

            if (roundDuration < Timestamps.VoteStart)
            {
                TournamentPhase = TournamentPhase.VoteCountdown;
                NextPhaseTimestamp = _roundStartTimestamp + Timestamps.VoteStart;
                LocalPlayer.VoteOption = 0;
            }
            else
            {
                if (roundDuration < Timestamps.MatchTransitionStart)
                {
                    LocalPlayer.IsReady = false;

                    if (_voteOptions.Count == 0)
                    {
                        LocalPlayer.TournamentScore = (int)(LocalPlayer.TournamentScore * _scoreDecay);

                        _roundRandom = new Random(_roundSeed);
                        _voteOptions = _roundRandom.Shuffle(DisciplineDirectory.Disciplines).Take(3).ToList();
                        _voteOptions.Insert(0, null);
                    }

                    TournamentPhase = TournamentPhase.Vote;
                    NextPhaseTimestamp = _roundStartTimestamp + Timestamps.MatchTransitionStart;
                }
                else
                {
                    if (LocalPlayer.VoteOption == 0)
                    {
                        /* Local player opted out of the match. */
                        ResetRound();
                        return;
                    }

                    if (roundDuration < Timestamps.MatchCountdownStart)
                    {
                        TournamentPhase = TournamentPhase.MatchTransition;
                        NextPhaseTimestamp = _roundStartTimestamp + Timestamps.MatchCountdownStart;
                    }
                    else
                    {
                        if (_matchBuffer == null)
                        {
                            var activePlayers = _roundRandom.Shuffle(_players.Values.Where(p => p.VoteOption != 0).OrderBy(p => p.Id));
                            if (activePlayers.Count < 2)
                            {
                                ResetRound();
                                return;
                            }

                            _localPlayerIndex = activePlayers.IndexOf(LocalPlayer);

                            var winningDiscipline = _voteOptions[activePlayers[_roundRandom.Next(activePlayers.Count)].VoteOption];
                            _voteOptions.Clear();

                            DiscipleName = winningDiscipline.Name;

                            var newOutput = new Output();

                            var playerDescriptions = activePlayers.Select(
                                p => new PlayerDescription
                                {
                                    Color = p.PlayerColor,
                                    Name = p.Name,
                                    Output = p == LocalPlayer ? newOutput : NullOutput.Instance,
                                }).ToList();

                            var matchDescription = new MatchDescription
                            {
                                Players = playerDescriptions,
                                Random = _roundRandom,
                                Output = newOutput,
                                LocalPlayerIndex = _localPlayerIndex,
                            };

                            var match = winningDiscipline.CreateMatch(matchDescription);
                            _matchBuffer = new MatchBuffer(match);
                            _lastUpdatedFrame = 0;
                        }

                        int currentFrame;
                        if (roundDuration < Timestamps.MatchStart)
                        {
                            TournamentPhase = TournamentPhase.MatchCountdown;
                            NextPhaseTimestamp = _roundStartTimestamp + Timestamps.MatchStart;
                            currentFrame = 0;
                        }
                        else
                        {
                            TournamentPhase = TournamentPhase.Match;
                            NextPhaseTimestamp = 0;
                            currentFrame = (int)((roundDuration - Timestamps.MatchStart) / GameSpeed.FrameDuration);

                            var input = terminal.Input;
                            while (currentFrame > _lastUpdatedFrame)
                            {
                                ++_lastUpdatedFrame;
                                _matchBuffer.SetInput(_lastUpdatedFrame, _localPlayerIndex, input);
                                _inputEntries.Add(new InputEntry(_lastUpdatedFrame, input));
                            }
                        }

                        /* Render a 50ms old frame to hide a certain amount of lag. */
                        _matchBuffer.UpdateToFrame(currentFrame - _latencyFrames);

                        var output = _matchBuffer.PredictedMatch.Output as Output;
                        if (output != null)
                        {
                            double frame = (roundDuration - Timestamps.MatchStart) / (double)GameSpeed.FrameDuration - _latencyFrames;
                            output.Render(frame, terminal);
                        }

                        if (_matchBuffer.IsCompleted)
                        {
                            var scores = _matchBuffer.KnownMatch.Players.Select(p => p.Score).ToList();
                            var localPlayerScore = scores[_localPlayerIndex];
                            int roundScore = scores.Sum(s => s < localPlayerScore ? _winScore : (s == localPlayerScore ? _tieScore : 0));
                            LocalPlayer.TournamentScore += roundScore;
                            ResetRound();
                        }
                    }
                }
            }
        }

        private void ResetRound()
        {
            _voteOptions.Clear();
            _lastRoundSeed = _roundSeed;
            _roundSeed = 0;
            _roundPriority = 0;
            _roundStartTimestamp = 0;
            _localPlayerIndex = -1;
            _matchBuffer = null;
        }

        private void SendPlayerState(Endpoint<PacketData> endpoint)
        {
            var packet = new PacketData();
            packet.PlayerId = LocalPlayer.Id;

            if (LocalPlayer.Name != _sentPlayerName)
            {
                packet.PlayerName = LocalPlayer.Name;
                _sentPlayerName = LocalPlayer.Name;
            }

            if (LocalPlayer.PlayerColor != _sentPlayerColor)
            {
                packet.PlayerColor = LocalPlayer.PlayerColor;
                _sentPlayerColor = LocalPlayer.PlayerColor;
            }

            packet.TournamentScore = LocalPlayer.TournamentScore;
            packet.IsReady = LocalPlayer.IsReady;
            packet.VoteOption = LocalPlayer.VoteOption;

            packet.RoundSeed = _roundSeed;
            packet.RoundPriority = _roundPriority;
            packet.RoundMilliFrames = (Stopwatch.GetTimestamp() - _roundStartTimestamp) * 1000 / GameSpeed.FrameDuration;

            packet.RoundPlayerIndex = _localPlayerIndex;

            packet.InputEntries = _inputEntries.ToArray();
            _inputEntries.Clear();

            endpoint.Send(packet);
        }

        private void HandlePacket(Packet<PacketData> packet)
        {
            var packetData = packet.Data;

            PlayerState playerState;
            if (!_players.TryGetValue(packetData.PlayerId, out playerState))
            {
                /* New player, resend all our info. */
                _sentPlayerName = null;
                _sentPlayerColor = default(Color);

                playerState = new PlayerState(packetData.PlayerId);
                _players[packetData.PlayerId] = playerState;
            }

            if (packetData.PlayerName != null)
            {
                playerState.Name = packetData.PlayerName;
            }

            if (packetData.PlayerColor != default(Color))
            {
                playerState.PlayerColor = packetData.PlayerColor;
            }

            playerState.LastPacketArrivalTimestamp = packet.ArrivalTimestamp;
            playerState.TournamentScore = packetData.TournamentScore;
            playerState.IsReady = packetData.IsReady;
            playerState.VoteOption = packetData.VoteOption;

            playerState.IsInMatch = packetData.RoundSeed != 0;
            playerState.RoundPlayerIndex = packetData.RoundPlayerIndex;

            if (packetData.RoundSeed != 0 && packetData.RoundSeed != _roundSeed && packetData.RoundSeed != _lastRoundSeed)
            {
                if (_roundSeed == 0 || packetData.RoundPriority < _roundPriority)
                {
                    var matchStartTimeStamp = packet.ArrivalTimestamp - packetData.RoundMilliFrames * GameSpeed.FrameDuration / 1000;

                    /* Only accepts rounds if they aren't yet in the voting phase. */
                    if (Stopwatch.GetTimestamp() - matchStartTimeStamp < Timestamps.VoteStart)
                    {
                        if (_roundSeed == 0)
                        {
                            _roundStartTimestamp = matchStartTimeStamp;
                        }
                        else
                        {
                            _roundStartTimestamp = Math.Min(_roundStartTimestamp, matchStartTimeStamp);
                        }

                        _roundSeed = packetData.RoundSeed;
                        _roundPriority = packetData.RoundPriority;
                    }
                }
            }

            if (packetData.InputEntries != null && _matchBuffer != null && packetData.RoundPlayerIndex >= 0)
            {
                foreach (var inputEntry in packetData.InputEntries)
                {
                    _matchBuffer.SetInput(inputEntry.Frame, packetData.RoundPlayerIndex, inputEntry.Input);
                }
            }
        }

        private void RemoveDisconnectedPlayers()
        {
            long currentTimestamp = Stopwatch.GetTimestamp();

            List<PlayerId> playersToRemove = null;
            foreach (var entry in _players)
            {
                if (entry.Value != LocalPlayer && currentTimestamp - entry.Value.LastPacketArrivalTimestamp > Durations.IdleDisconnect)
                {
                    if (playersToRemove == null)
                    {
                        playersToRemove = new List<PlayerId>();
                    }

                    playersToRemove.Add(entry.Key);
                }
            }

            if (playersToRemove != null)
            {
                foreach (var playerId in playersToRemove)
                {
                    _players.Remove(playerId);
                }

                if (_roundSeed != 0)
                {
                    ResetRound();
                }
            }
        }
    }
}
