// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Application.Ui;
using NanoGames.Engine;
using NanoGames.Engine.OutputSystems;
using NanoGames.Games;
using NanoGames.Synchronization;
using System;
using System.Diagnostics;
using System.Linq;

namespace NanoGames.Application
{
    /// <summary>
    /// A view that allows debugging multiplayer games.
    /// </summary>
    internal class MultiplayerDebugView : IView
    {
        private readonly Action _goBack;
        private readonly Menu _selectDisciplineMenu;

        private Menu _currentMenu;
        private int _numberOfPlayers = 2;

        private Discipline _discipline;
        private IMatch _match;
        private long _matchStartTimestamp;
        private int _matchFrameCount;

        private int _currentPlayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplayerDebugView"/> class.
        /// </summary>
        /// <param name="goBack">The action to call when the player leaves the view.</param>
        public MultiplayerDebugView(Action goBack)
        {
            _goBack = goBack;

            _selectDisciplineMenu = new Menu("SELECT GAME")
            {
                OnBack = _goBack,
                Items = DisciplineDirectory.Disciplines.Select(d => new CommandMenuItem(d.Name, () => SelectDiscipline(d))).ToList<MenuItem>(),
            };

            _selectDisciplineMenu.Items.Insert(0, new CommandMenuItem("BACK", _goBack));

            _currentMenu = _selectDisciplineMenu;
        }

        /// <inheritdoc/>
        public bool ShowBackground => _match == null;

        /// <inheritdoc/>
        public void Update(Terminal terminal)
        {
            if (_match != null)
            {
                foreach (var keyEvent in terminal.KeyEvents)
                {
                    switch (keyEvent.Code)
                    {
                        case KeyCode.Escape:
                            _match = null;
                            break;

                        case KeyCode.F1:
                            _currentPlayer = 0 % _numberOfPlayers;
                            break;

                        case KeyCode.F2:
                            _currentPlayer = 1 % _numberOfPlayers;
                            break;

                        case KeyCode.F3:
                            _currentPlayer = 2 % _numberOfPlayers;
                            break;

                        case KeyCode.F4:
                            _currentPlayer = 3 % _numberOfPlayers;
                            break;

                        case KeyCode.F5:
                            _currentPlayer = 4 % _numberOfPlayers;
                            break;

                        case KeyCode.F6:
                            _currentPlayer = 5 % _numberOfPlayers;
                            break;

                        case KeyCode.F7:
                            _currentPlayer = 6 % _numberOfPlayers;
                            break;

                        case KeyCode.F8:
                            _currentPlayer = 7 % _numberOfPlayers;
                            break;
                    }
                }

                if (_match == null)
                {
                    terminal.KeyEvents.Clear();
                }
                else
                {
                    UpdateMatch(terminal);
                }
            }

            if (_match == null)
            {
                if (_currentMenu != null)
                {
                    _currentMenu.Update(terminal);

                    var hint = "USE THE F-KEYS TO SWITCH BETWEEN PLAYERS";
                    terminal.Graphics.Print(Colors.Title, 8, new Vector(160 - 4 * hint.Length, 186), hint);
                }
            }
        }

        private void SelectDiscipline(Discipline discipline)
        {
            _discipline = discipline;

            _currentMenu = new Menu("NUMBER OF PLAYERS")
            {
                OnBack = () => _currentMenu = _selectDisciplineMenu,
                SelectedIndex = _numberOfPlayers - 1,
                Items = Enumerable.Range(2, 7).Select(i => (MenuItem)new CommandMenuItem(i == 1 ? "1 PLAYER" : $"{i} PLAYERS", () => StartMatch(i))).ToList(),
            };

            _currentMenu.Items.Insert(0, new CommandMenuItem("BACK", () => _currentMenu = _selectDisciplineMenu));
        }

        private void StartMatch(int numberOfPlayers)
        {
            _numberOfPlayers = numberOfPlayers;
            _currentPlayer = 0;

            var description = new MatchDescription
            {
                Players = Enumerable.Range(0, numberOfPlayers).Select(i =>
                    new PlayerDescription
                    {
                        Color = PlayerColors.Values[(i * 5) % PlayerColors.Values.Count],
                        Output = new Output(),
                        Name = "PLAYER" + (i + 1),
                    }).ToList(),

                Random = new Random(),
                Output = new Output(),
                LocalPlayerIndex = 0,
            };

            _match = _discipline.CreateMatch(description);
            _matchStartTimestamp = Stopwatch.GetTimestamp();
            _matchFrameCount = 0;
        }

        private void UpdateMatch(Terminal terminal)
        {
            var currentTimestamp = Stopwatch.GetTimestamp();

            /* Check if the buffered frame is still valid and compute new frames as needed. */
            while (Stopwatch.GetTimestamp() - _matchStartTimestamp - _matchFrameCount * GameSpeed.FrameDuration > GameSpeed.FrameDuration)
            {
                ++_matchFrameCount;

                var inputStates = new InputState[_numberOfPlayers];
                inputStates[_currentPlayer] = terminal.Input;

                for (int i = 0; i < _numberOfPlayers; ++i)
                {
                    var player = _match.Players[i];
                    player.LocalColor = i == _currentPlayer ? Colors.White : player.Color;
                }

                _match.Update(inputStates);

                if (_match.IsCompleted)
                {
                    _match = null;
                    return;
                }
            }

            /* Render the buffered frame. */
            var frame = (Stopwatch.GetTimestamp() - _matchStartTimestamp) / (double)GameSpeed.FrameDuration;

            RenderOutput(terminal, _match.Output as Output, frame);
            RenderOutput(terminal, _match.Players[_currentPlayer].Output as Output, frame);
        }

        private void RenderOutput(Terminal terminal, Output output, double frame)
        {
            output?.Render(frame, terminal);
        }
    }
}
