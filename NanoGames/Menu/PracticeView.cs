// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Games;
using System;
using System.Linq;

namespace NanoGames.Menu
{
    /// <summary>
    /// A view that allows players to play singleplayer practice games.
    /// </summary>
    internal sealed class PracticeView : IView
    {
        private static readonly Color _playerColor = new Color(0.1, 0.4, 0.8);

        private readonly Action _goBack;
        private readonly Menu _selectDisciplineMenu;

        private Terminal _currentTerminal;
        private Match _match;

        /// <summary>
        /// Initializes a new instance of the <see cref="PracticeView"/> class.
        /// </summary>
        /// <param name="goBack">The action to call when the player leaves the practice view.</param>
        public PracticeView(Action goBack)
        {
            _goBack = goBack;

            _selectDisciplineMenu = new Menu
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
                if (terminal.Input.Back)
                {
                    _match = null;
                }
                else
                {
                    _match.Update();
                }
            }

            if (_match == null)
            {
                _currentTerminal = terminal;
                _selectDisciplineMenu.Update(terminal);
            }
        }

        private void GoBack()
        {
            _goBack?.Invoke();
        }

        private void StartMatch(Discipline discipline)
        {
            var description = new MatchDescription
            {
                Players =
                {
                    new Player
                    {
                        Index = 0,
                        Color = _playerColor,
                        Terminal = _currentTerminal,
                    },
                },
            };

            _match = DisciplineDirectory.Disciplines[0].CreateMatch(description);
        }
    }
}
