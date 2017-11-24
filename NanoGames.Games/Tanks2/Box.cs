// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames.Games.Tanks2
{
    public class Box
    {
        public Vector3 A;
        public Vector3 B;
        public Vector3 C;
        public Vector3 D;
        public Vector3 E;
        public Vector3 F;
        public Vector3 G;
        public Vector3 H;

        public List<Segment> Segments;


        public Box(Color c, double w, double l, double h, Vector3 pos)
        {
            A = pos;
            B = new Vector3(pos.X + w, pos.Y, pos.Z);
            C = new Vector3(pos.X + w, pos.Y + l, pos.Z);
            D = new Vector3(pos.X, pos.Y + l, pos.Z);

            E = new Vector3(pos.X, pos.Y, pos.Z + h);
            F = new Vector3(pos.X + w, pos.Y, pos.Z + h);
            G = new Vector3(pos.X + w, pos.Y + l, pos.Z + h);
            H = new Vector3(pos.X, pos.Y + l, pos.Z + h);

            Segments = new List<Segment> { };
            Segments.Add(new Segment(c, A, B));
            Segments.Add(new Segment(c, B, C));
            Segments.Add(new Segment(c, C, D));
            Segments.Add(new Segment(c, D, A));

            Segments.Add(new Segment(c, E, F));
            Segments.Add(new Segment(c, F, G));
            Segments.Add(new Segment(c, G, H));
            Segments.Add(new Segment(c, H, E));

            Segments.Add(new Segment(c, A, E));
            Segments.Add(new Segment(c, B, F));
            Segments.Add(new Segment(c, C, G));
            Segments.Add(new Segment(c, D, H));
        }
    }
}
