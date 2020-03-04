// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Cluster
{
    internal class Tile
    {
        public List<Segment> Segments;
        public Vector CenterPoint;

        public Tile()
        {
            Segments = new List<Segment> { };
            CenterPoint = new Vector(0, 0);
        }


        public void Draw(ClusterPlayer observer)
        {
            double m = observer.Magnification;
            Vector obs = observer.Position;
            IGraphics g = observer.Output.Graphics;

            foreach (Segment part in Segments)
            {
                Segment seg = part.Translated(-obs).Scaled(m).ToOrigin();
                g.LLine(Colors.Orange, seg.Start, seg.End);
            }
        }
        
    }
}

