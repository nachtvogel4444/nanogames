// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoGames.Games
{
    /// <summary>
    /// Represents a single match.
    /// </summary>
    /// <typeparam name="TPlayer">The player type associated with the match.</typeparam>
    internal abstract class Match<TPlayer> : IMatch
        where TPlayer : Player
    {
        private List<MatchTimer> _timers = new List<MatchTimer>();

        /// <summary>
        /// Gets the list of players.
        /// </summary>
        public IReadOnlyList<TPlayer> Players { get; private set; }

        IReadOnlyList<Player> IMatch.Players => Players;

        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        public Random Random { get; internal set; }

        /// <summary>
        /// Gets the match Output instance, which can be used to output sound or graphics for every player.
        /// </summary>
        public IOutput Output { get; internal set; }

        /// <inheritdoc/>
        public IEnumerable<double> PlayerScores => Players.Select(p => p.Score);

        public bool IsCompleted { get; protected set; }

        /// <summary>
        /// The current frame index, count from 0 at the start of the match up.
        /// </summary>
        public int Frame { get; private set; }

        /// <summary>
        /// Sets the list of players.
        /// </summary>
        /// <param name="players">The list of players.</param>
        public void Initialize(List<TPlayer> players)
        {
            if (Players != null)
            {
                throw new InvalidOperationException("The players can only be set once.");
            }

            Players = players.AsReadOnly();

            Initialize();
        }

        /// <inheritdoc/>
        public void Update(InputState[] inputs)
        {
            if (IsCompleted)
            {
                return;
            }

            Output.SetFrame(Frame);

            for (int i = 0; i < Players.Count; ++i)
            {
                Players[i].Output.SetFrame(Frame);
                Players[i].Input.SetState(Frame, inputs[i]);
            }

            foreach (var t in new List<MatchTimer>(_timers))
            {
                t.Tick();
            }

            /* Update the match. */
            Update();

            ++Frame;
        }

        public IMatchTimer GetTimer(int interval)
        {
            var matchTimer = new MatchTimer(this, interval);
            _timers.Add(matchTimer);
            return matchTimer;
        }

        public void TimeOnce(int interval, Action action)
        {
            var timer = GetTimer(interval);
            timer.Elapsed += () =>
            {
                action.Invoke();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        public void TimeCyclic(int interval, Action<IMatchTimer> action)
        {
            var timer = GetTimer(interval);
            timer.Elapsed += () =>
            {
                action.Invoke(timer);
            };
            timer.Start();
        }

        /// <summary>
        /// Initializes the match. This is called before <see cref="Player.Initialize"/>.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Updates and renders the match for all players. This is called before <see cref="Player.Update"/>.
        /// </summary>
        protected abstract void Update();

        private void DisposeTimer(MatchTimer timer)
        {
            _timers.Remove(timer);
        }

        private sealed class MatchTimer : IMatchTimer
        {
            private double _runningTime = 0;
            private Match<TPlayer> _match;
            private bool _disposed = false;

            public MatchTimer(Match<TPlayer> match, int interval)
            {
                Interval = interval;
                _match = match;
            }

            public event MatchTimerElapsedHandler Elapsed;

            public double Interval { get; set; }

            public bool Enabled { get; set; }

            public void Tick()
            {
                if (Enabled && !_disposed)
                {
                    _runningTime += 1d / 60d * 1000d;
                    if (_runningTime >= Interval)
                    {
                        if (Elapsed != null)
                        {
                            Elapsed();
                        }
                        _runningTime = 0;
                    }
                }
            }

            public void Start()
            {
                // TODO : throw exception if disposed?
                Enabled = true;
            }

            public void Stop()
            {
                // TODO : throw exception if disposed?
                Enabled = false;
                _runningTime = 0;
            }

            public void Dispose()
            {
                _match.DisposeTimer(this);
            }
        }
    }
}
