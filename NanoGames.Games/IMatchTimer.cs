// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games
{
    public delegate void MatchTimerElapsedHandler();

    public interface IMatchTimer
    {
        event MatchTimerElapsedHandler Elapsed;

        double Interval { get; set; }

        bool Enabled { get; set; }

        void Start();

        void Stop();

        void Dispose();
    }
}
