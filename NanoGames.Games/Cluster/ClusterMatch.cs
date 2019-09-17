// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;


namespace NanoGames.Games.Cluster
{
    internal class ClusterMatch : Match<ClusterPlayer>
    {
        private World world = new World();
        
        protected override void Initialize()
        {
            world.AddPlanets(Random);
            world.AddStars(Random);
            world.PlayerPositions(Random, Players);

            foreach (ClusterPlayer player in Players)
            {
                player.Birth();
                player.MagnificationMin = Math.Min(160.0 / world.XMax, 100.0 / world.YMax);
            }
            
        }

        protected override void Update()
        {/*
            double alpha = Frame * Math.PI / 180;
            Vector middle = new Vector(160, 100);
            Vector right = new Vector(180, 100);
            Vector rright = new Vector(200, 100);

            Vector h = new Vector(Math.Cos(alpha), Math.Sin(alpha));
            Vector hh = new Vector(1, 0);

            Output.Graphics.Line(Colors.White, middle, middle + 10 * h);
            Output.Graphics.Line(Colors.White, right, right + 10 * hh.Rotated(alpha));
            Output.Graphics.Line(Colors.White, rright, rright + 10 * Vector.FromAngle(alpha));
            */
            
            // Everything moves
            foreach (ClusterPlayer player in Players)
            {
                player.Move();
            }

            foreach (LBeam lbeam in world.LBeams)
            {
                lbeam.Move();
            }
            
            // Everything collides
            if (Players.Count > 1)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    for (int j = i + 1; j < Players.Count; j++)
                    {
                        Players[i].Collide(Players[j]);
                    }
                }
            }
            
            foreach (ClusterPlayer player in Players)
            {
                foreach (Planet planet in world.Planets)
                {
                    player.Collide(planet);
                }
            }

            List<LBeam> tmp = new List<LBeam> { };
            foreach (LBeam lBeam in world.LBeams)
            {
                foreach (Planet planet in world.Planets)
                {
                    lBeam.Collide(planet);
                }

                if (!lBeam.Explodes)
                {
                    tmp.Add(lBeam);
                }
                else
                {
                    world.Explosions.Add(new Explosion(lBeam.Position, 100, 300, 20, Random));
                }
            }

            world.LBeams = tmp;
            
            // everthing can act
            foreach (ClusterPlayer player in Players)
            {
                player.Shoot(world);
                player.DoMagnification();
            }

            // Everthing is drawn, sound is played
            foreach (ClusterPlayer observer in Players)
            {
                world.Draw(observer);
                world.PlaySound(observer);
                
                foreach (ClusterPlayer player in Players)
                {
                    player.Draw(observer);
                    player.PlaySound(observer);
                }
            }

            // remove everthing which is not needed anymore
            /*
            List<Explosion> tmp2 = new List<Explosion> { };
            foreach (Explosion explosion in world.Explosions)
            {
                if (!explosion.IsExploded)
                {
                    tmp2.Add(explosion);
                }
            }
            world.Explosions = tmp2;
            */
            
        }
    }
}
