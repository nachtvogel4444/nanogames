// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Cluster
{
    internal class Star
    {
        public Vector Position;
        public double Brightness;


        public Star(Vector pos, double b)
        {
            Position = pos;
            Brightness = b;
        }


        public void Draw(ClusterPlayer observer)
        {
            double m = observer.Magnification;
            Vector obs = observer.Position;
            Color c =  Brightness * Math.Pow(m, 0.3) * Colors.White;
            IGraphics g = observer.Output.Graphics;

            Vector p = Position.Translated(-obs).Scaled(m).ToOrigin();

            g.PPoint(c, p);
        }
    }
}
