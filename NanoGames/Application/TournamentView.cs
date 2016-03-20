// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using NanoGames.Application.Ui;
using NanoGames.Engine;
using NanoGames.Synchronization;
using System;
using System.Diagnostics;
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

        private Menu _voteMenu = new Menu(null);

        private StarfieldView _warpStarfield = new StarfieldView();

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
            _tournament.LocalPlayer.PlayerColor = Settings.Instance.PlayerColor;

            if (_tournament.IsConnecting)
            {
                if (escape || enter)
                {
                    _goBack();
                }

                terminal.Graphics.PrintCenter(Colors.Title, 8, new Vector(160, Menu.TitleY), "CONNECTING");
                terminal.Graphics.PrintCenter(Colors.FocusedControl, 8, new Vector(160, 96), "CANCEL");
            }
            else if (!_tournament.IsConnected)
            {
                if (escape || enter)
                {
                    _goBack();
                    return;
                }

                var message = _tournament.WasConnected ? "CONNECTION LOST" : "CONNECTION FAILED";
                terminal.Graphics.PrintCenter(Colors.Error, 8, new Vector(160, Menu.TitleY), message);
                terminal.Graphics.PrintCenter(Colors.FocusedControl, 8, new Vector(160, 96), "BACK");
            }
            else
            {
                switch (_tournament.TournamentPhase)
                {
                    case TournamentPhase.Lobby:
                    case TournamentPhase.VoteCountdown:
                        if (enter)
                        {
                            _tournament.LocalPlayer.IsReady = !_tournament.LocalPlayer.IsReady;
                        }

                        UpdateLobbyView(terminal);
                        break;

                    case TournamentPhase.Vote:
                        UpdateVoteView(terminal);
                        break;

                    case TournamentPhase.MatchTransition:
                        double transitionTime = (Stopwatch.GetTimestamp() - _tournament.NextPhaseTimestamp + Durations.MatchTransition) / (double)Durations.MatchTransition;
                        _warpStarfield.Velocity = 3 * (0.5 - 0.5 * Math.Cos(transitionTime * transitionTime * 2 * Math.PI));
                        _warpStarfield.Warp = 1 + _warpStarfield.Velocity;
                        _warpStarfield.Update(terminal);
                        break;

                    case TournamentPhase.MatchCountdown:
                        terminal.Graphics.PrintCenter(Colors.Title, 8, new Vector(160, Menu.TitleY), _tournament.DiscipleName);
                        terminal.Graphics.PrintCenter(Colors.Title, 8, new Vector(160, 96), SecondsUntilNextPhase().ToString());
                        break;

                    case TournamentPhase.Match:
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }

            _tournament.Update(terminal);
        }

        private void UpdateLobbyView(Terminal terminal)
        {
            double fontSize = 8;
            double x = 160 - 0.5 * ((Settings.MaxPlayerNameLength + 6) * fontSize);
            double y = 100 - 0.5 * _tournament.Players.Count * fontSize;

            foreach (var playerInfo in _tournament.Players.OrderByDescending(p => p.TournamentScore).ThenBy(p => p.Name, StringComparer.InvariantCultureIgnoreCase).ThenBy(p => p.Id))
            {
                terminal.Graphics.Print(playerInfo.PlayerColor, fontSize, new Vector(x + fontSize, y), playerInfo.Name);
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

            if (_tournament.TournamentPhase == TournamentPhase.VoteCountdown)
            {
                var title = string.Format("NEXT ROUND IN {0}", SecondsUntilNextPhase());
                terminal.Graphics.PrintCenter(Colors.Title, 8, new Vector(160, Menu.TitleY), title);
            }
            else
            {
                if (_tournament.IsRoundInProgress)
                {
                    var c = Colors.Title * (0.5 + 0.5 * Math.Sin(Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency * 2 * Math.PI));
                    terminal.Graphics.PrintCenter(c, 8, new Vector(160, Menu.TitleY), "GAME IN PROGRESS");
                }
                else if (_tournament.LocalPlayer.IsReady)
                {
                    terminal.Graphics.PrintCenter(Colors.Title, 8, new Vector(160, Menu.TitleY), "WAITING FOR OTHERS");
                }
                else
                {
                    var c = Colors.Error * (0.5 + 0.5 * Math.Sin(Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency * 2 * Math.PI));
                    terminal.Graphics.PrintCenter(c, 8, new Vector(160, Menu.TitleY), "PRESS ENTER");
                }
            }
        }

        private void UpdateVoteView(Terminal terminal)
        {
            if (_tournament.VoteOptions.Count <= 1)
            {
                return;
            }

            string skipTitle = "SKIP THIS ROUND";
            var maxDisciplineNameLength = Math.Max(skipTitle.Length, _tournament.VoteOptions.Skip(1).Max(d => d.Name.Length));

            if (_voteMenu.Items.Count != _tournament.VoteOptions.Count)
            {
                skipTitle += new string(' ', maxDisciplineNameLength + 3 - skipTitle.Length);

                _voteMenu.Items = _tournament.VoteOptions
                    .Select(discipline => new CommandMenuItem(skipTitle, null))
                    .Cast<MenuItem>()
                    .ToList();
            }

            var secondsToVote = SecondsUntilNextPhase();
            _voteMenu.Title = secondsToVote == 1 ? "1 SECOND TO VOTE" : string.Format(CultureInfo.InvariantCulture, "{0} SECONDS TO VOTE", SecondsUntilNextPhase());

            for (int i = 0; i < _tournament.VoteOptions.Count; ++i)
            {
                var discipline = _tournament.VoteOptions[i];
                if (discipline != null)
                {
                    var count = _tournament.Players.Count(p => p.VoteOption == i);
                    string disciplineName = discipline.Name + new string(' ', maxDisciplineNameLength - discipline.Name.Length);
                    ((CommandMenuItem)_voteMenu.Items[i]).Text = string.Format(CultureInfo.InvariantCulture, "{0} {1:00}", disciplineName, count);
                }
            }

            _voteMenu.SelectedIndex = _tournament.LocalPlayer.VoteOption;
            _voteMenu.Update(terminal);
            _tournament.LocalPlayer.VoteOption = _voteMenu.SelectedIndex;
        }

        private int SecondsUntilNextPhase()
        {
            return (int)Math.Ceiling((_tournament.NextPhaseTimestamp - Stopwatch.GetTimestamp()) / (double)Stopwatch.Frequency);
        }
    }
}
