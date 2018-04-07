// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Cluster
{
    internal class LBeam
    {
        public Vector Position;
        private Vector Velocity;
        private double length;
        
        public bool IsGone;

        public LBeam(Vector pos, Vector heading)
        {
            Velocity = heading * Constants.LBeam.Speed;
            IsGone = false;
            length = Constants.LBeam.Length;
            Position = pos + Velocity.Normalized * length;
        }

        public void Move()
        {
            Position += Velocity * Constants.dt;
        }


        public void Collide(Planet planet)
        {
            if ((planet.Position - Position).LengthBBox < planet.Radius &&
                (planet.Position - Position).Length < planet.Radius)
            {
                IsGone = true;
            }
        }

        public void Draw(ClusterPlayer observer)
        {
            double m = observer.Magnification;
            Vector obs = observer.Position;
            Color c = Colors.Blue;
            IGraphics g = observer.Output.Graphics;
            
            Vector p1 = Position.Translated(-obs).Scaled(m).ToOrigin();
            Vector p2 = (Position - Velocity.Normalized*length).Translated(-obs).Scaled(m).ToOrigin();

            g.LLine(c, p1, p2);
        }
    }
}
