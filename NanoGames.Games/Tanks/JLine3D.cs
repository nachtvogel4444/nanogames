// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Tanks
{
    public struct JLine3D
    {
        public JVertex3D Start;
        public JVertex3D Stop;

        
        public JLine3D(JVertex3D start, JVertex3D stop)
        {
            Start = start;
            Stop = stop;

            Start.W = 1;
            Stop.W = 1;

            if (start == stop)
            {
                throw new ArgumentException("Start and Stoppoint of line are the same.");
            }
        }


        public double SquaredLength => (Start.X - Stop.X) * (Start.X - Stop.X) +
                                       (Start.Y - Stop.Y) * (Start.Y - Stop.Y) + 
                                       (Start.Z - Stop.Z) * (Start.Z - Stop.Z);

        public double Length => Math.Sqrt(SquaredLength);

        public JVertex3D Direction => new JVertex3D(Stop.X - Start.X, Stop.Y - Start.Y, Stop.Z - Start.Z, 0);

        public JVertex3D DirectionN => Direction / Length;

        public JVertex3D MidPoint => new JVertex3D(Start.X + 0.5 * Direction.X,
                                                   Start.Y + 0.5 * Direction.Y,
                                                   Start.Z + 0.5 * Direction.Z,
                                                   0);
    }
}
