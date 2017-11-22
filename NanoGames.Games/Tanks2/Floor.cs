// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Tanks2
{
    public class Floor
    {
        public double XDensity;
        public double YDensity;
        public int N;

        public Vector3[] Start;
        public Vector3[] Stop;
        public Color[] Colors;

        private double width = 200;
        private double length = 200;

        private Color grey;

        public Floor(double xdensity, double ydensity)
        {
            grey = new Color(0.3, 0.3, 0.3);
            
            int nx = (int)(width / xdensity) + 1;
            int ny = (int)(length / ydensity) + 1;
            XDensity = width / nx;
            YDensity = length / ny;
            N = nx + ny;

            Start = new Vector3[N];
            Stop = new Vector3[N];
            Colors = new Color[N];
            
            for(int i = 0; i < nx; i++)
            {
                Start[i] = new Vector3(-width / 2 + i * XDensity, length / 2, 0);
                Stop[i] = new Vector3(-width / 2 + i * XDensity, -length / 2, 0);
                Colors[i] = grey;
            }

            int ii;
            for (int i = nx; i < N; i++)
            {
                ii = i - nx;

                Start[i] = new Vector3(width / 2, -length / 2 + ii * YDensity, 0);
                Stop[i] = new Vector3(-width / 2, -length / 2 + ii * YDensity, 0);
                Colors[i] = grey;
            }
        }
        
    }
}
