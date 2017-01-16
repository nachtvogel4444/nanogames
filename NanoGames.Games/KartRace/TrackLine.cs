using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.KartRace
{
    class TrackLine
    {
        public readonly Color Color;
        public readonly Vector P;
        public readonly Vector Q;

        public TrackLine(Color color, Vector p, Vector q)
        {
            Color = color;
            P = p;
            Q = q;
        }
    }
}
