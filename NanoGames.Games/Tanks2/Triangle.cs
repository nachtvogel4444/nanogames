// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


namespace NanoGames.Games.Tanks2
{
    public class Triangle
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;
        public Vector3 ProjectionA;
        public Vector3 ProjectionB;
        public Vector3 ProjectionC;
        public double ZMin;
        public double ZMax;
        public Color Color;


        public Triangle(Color col, Vector3 a, Vector3 b, Vector3 c)
        {
            A = a;
            B = b;
            C = c;
            Color = col;
        }

    }
}
