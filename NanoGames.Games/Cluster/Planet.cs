// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Cluster
{
    internal class Planet
    {
        public Vector Position;
        public double Radius;

        public Planet(Vector pos, double r)
        {
            Position = pos;
            Radius = r;
        }
        
        public void Draw(ClusterPlayer observer)
        {
            double m = observer.Magnification;
            double r = Radius * m;

            if (r > 0.5)
            {
                Vector obs = observer.Position;
                Color c = Colors.White;
                IGraphics g = observer.Output.Graphics;
                Vector p = Position.Translated(-obs).Scaled(m).ToOrigin();
                
                g.CCircle(c, p, r);
            }
        }
    }
}
