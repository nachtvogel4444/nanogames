// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Application.Ui;
using NanoGames.Engine;
using NanoGames.Synchronization;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGames.Application
{
    /// <summary>
    /// Represents the game lobby.
    /// </summary>
    internal sealed class TournamentView : IView
    {
        private readonly Action _goBack;
        private readonly Menu _menu;

        private readonly Tournament _tournament;

        private IView _currentView;

        /// <summary>
        /// Initializes a new instance of the <see cref="TournamentView"/> class.
        /// </summary>
        /// <param name="goBack">The action invoked to navigate back in the menu.</param>
        /// <param name="tournament">The tournament.</param>
        public TournamentView(Action goBack, Tournament tournament)
        {
            _tournament = tournament;

            _goBack = () =>
            {
                Task.Run(() => _tournament.DisposeAsync());
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
                escape = false;
                enter = false;
            }

            _tournament.LocalPlayer.Name = Settings.Instance.PlayerName;

            if (enter)
            {
                _tournament.LocalPlayer.IsReady = !_tournament.LocalPlayer.IsReady;
            }

            _tournament.Update(null);

            if (_tournament.IsConnecting)
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

            if (!_tournament.IsConnected)
            {
                if (escape || enter)
                {
                    _goBack();
                    return;
                }

                var message = _tournament.WasConnected ? "CONNECTION LOST" : "CONNECTION FAILED";
                terminal.Graphics.PrintCenter(Colors.Error, 8, new Vector(160, Menu.TitleY), message);
                terminal.Graphics.PrintCenter(Colors.FocusedControl, 8, new Vector(160, 96), "BACK");
                return;
            }

            DrawLobby(terminal);
        }

        private void DrawLobby(Terminal terminal)
        {
            double fontSize = 8;
            double x = 160 - 0.5 * ((Settings.MaxPlayerNameLength + 6) * fontSize);
            double y = 100 - 0.5 * _tournament.Players.Count * fontSize;

            foreach (var playerInfo in _tournament.Players.OrderBy(p => p.TournamentScore).ThenBy(p => p.Name, StringComparer.InvariantCultureIgnoreCase).ThenBy(p => p.Id))
            {
                terminal.Graphics.Print(Colors.Control, fontSize, new Vector(x + fontSize, y), playerInfo.Name);
                terminal.Graphics.Print(
                    new Color(0.7, 0.7, 0.7),
                    fontSize,
                    new Vector(x + (Settings.MaxPlayerNameLength + 2) * fontSize, y),
                    playerInfo.TournamentScore.ToString("0000", CultureInfo.InvariantCulture));

                if (playerInfo.IsReady)
                {
                    terminal.Graphics.Print(Colors.Title, fontSize, new Vector(x, y), "✓");
                }

                y += fontSize;
            }

            if (_tournament.LocalPlayer.IsReady)
            {
                terminal.Graphics.PrintCenter(Colors.Title, 8, new Vector(160, Menu.TitleY), "WAITING");
            }
            else
            {
                terminal.Graphics.PrintCenter(Colors.Error, 8, new Vector(160, Menu.TitleY), "ENTER TO START");
            }
        }
    }
}
