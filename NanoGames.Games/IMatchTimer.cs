using System.Timers;

namespace NanoGames.Games
{
    public interface IMatchTimer
    {
        event ElapsedEventHandler Elapsed;

        double Interval { get; set; }

        bool Enabled { get; set; }

        void Start();

        void Stop();

        void Dispose();
    }
}
