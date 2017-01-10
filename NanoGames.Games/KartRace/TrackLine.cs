using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.KartRace
{
    class TrackLine
    {
        public readonly Vector P;
        public readonly Vector Q;

        public TrackLine(Vector p, Vector q)
        {
            P = p;
            Q = q;
        }
    }
}
