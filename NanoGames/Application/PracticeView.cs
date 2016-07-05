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
    /// A view that allows players to play singleplayer practice games.
    /// </summary>
    internal sealed class PracticeView : IView
    {
        private readonly Action _goBack;
        private readonly Menu _selectDisciplineMenu;

        private Output _output;

        private IMatch _match;

        private long _matchStartTimestamp;
        private long _matchFrameCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="PracticeView"/> class.
        /// </summary>
        /// <param name="goBack">The action to call when the player leaves the practice view.</param>
        public PracticeView(Action goBack)
        {
            _goBack = goBack;

            _selectDisciplineMenu = new Menu("SELECT GAME")
            {
                OnBack = GoBack,
                Items = DisciplineDirectory.Disciplines.Select(d => new CommandMenuItem(d.Name, () => StartMatch(d))).ToList<MenuItem>(),
            };

            _selectDisciplineMenu.Items.Insert(0, new CommandMenuItem("BACK", GoBack));
        }

        /// <inheritdoc/>
        public void Update(Terminal terminal)
        {
            if (_match != null)
            {
                if (terminal.KeyEvents.Any(k => k.Code == KeyCode.Escape))
                {
                    _match = null;
                    terminal.KeyEvents.Clear();
                }
                else
                {
                    UpdateMatch(terminal);
                }
            }

            if (_match == null)
            {
                _selectDisciplineMenu.Update(terminal);
            }
        }

        private void UpdateMatch(Terminal terminal)
        {
            var currentTimestamp = Stopwatch.GetTimestamp();

            /* Check if the buffered frame is still valid and compute new frames as needed. */
            while (Stopwatch.GetTimestamp() - _matchStartTimestamp - _matchFrameCount * GameSpeed.FrameDuration > GameSpeed.FrameDuration)
            {
                ++_matchFrameCount;

                _match.Update(
                    new InputState[]
                    {
                        terminal.Input,
                    });

                if (_match.IsCompleted)
                {
                    _match = null;
                    return;
                }
            }

            /* Render the buffered frame. */
            var output = _match.Output as Output;
            var frame = (Stopwatch.GetTimestamp() - _matchStartTimestamp) / (double)GameSpeed.FrameDuration;
            output?.Render(frame, terminal);
        }

        private void GoBack()
        {
            _goBack?.Invoke();
        }

        private void StartMatch(Discipline discipline)
        {
            _output = new Output();

            var description = new MatchDescription
            {
                Players =
                {
                    new PlayerDescription
                    {
                        Color = Settings.Instance.PlayerColor,
                        Output = _output,
                    },
                },

                Random = new Random(),
                Output = _output,
                LocalPlayerIndex = 0,
            };

            _match = discipline.CreateMatch(description);
            _matchStartTimestamp = Stopwatch.GetTimestamp();
            _matchFrameCount = 0;
        }
    }
}
