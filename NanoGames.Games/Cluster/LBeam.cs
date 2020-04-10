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

        private Vector velocity;
        private double length;
        
        public bool ReadyToDelete;


        public LBeam(Vector pos, Vector heading, Random ran)
        {
            velocity = (heading + 0.02 *(ran.NextDouble() - 0.5) * heading.RotatedLeft) * Constants.LBeam.Speed;
            length = Constants.LBeam.Length;
            Position = pos + velocity.Normalized * length;
        }


        public void Move()
        {
            Position += velocity * Constants.dt;
        }

        public void Collide(World world)
        {
            foreach (Planet planet in world.Planets)
            {
                if ((planet.Position - Position).LengthBBox < planet.Radius &&
                    (planet.Position - Position).Length < planet.Radius)
                {
                    world.Explosions.Add(new Explosion(Position, world.Random));
                    ReadyToDelete = true;
                    break;
                }
            }
            
            foreach (ClusterPlayer player in world.Players)
            {
                if ((player.Position - Position).LengthBBox < player.Size &&
                    (player.Position - Position).Length < player.Size)
                {
                    world.Explosions.Add(new Explosion(Position, world.Random));
                    ReadyToDelete = true;
                    break;
                }
            }
        }

        
        public void Draw(ClusterPlayer observer)
        {
            double m = observer.Magnification;
            Vector obs = observer.Position;
            Color c = 1.5 * Math.Pow(m, 0.4) * Colors.Blue;
            IGraphics g = observer.Output.Graphics;
            
            Vector p1 = Position.Translated(-obs).Scaled(m).ToOrigin();
            Vector p2 = (Position - velocity.Normalized*length).Translated(-obs).Scaled(m).ToOrigin();

            g.LLine(c, p1, p2);
        }
    }
}
