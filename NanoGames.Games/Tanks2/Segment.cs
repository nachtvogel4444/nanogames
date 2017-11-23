// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


namespace NanoGames.Games.Tanks2
{
    public class Segment
    {
        public Vector3 Start;
        public Vector3 Stop;
        public Vector3 ProjectionStart;
        public Vector3 ProjectionStop;
        public double ZMax;
        public double ZMin;
        public Color Color;


        public Segment(Color c, Vector3 start, Vector3 stop)
        {
            Start = start;
            Stop = stop;
            Color = c;
        }


    }
}
