// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;


namespace NanoGames.Games.Cluster
{
    internal class World
    {
        public List<Star> Stars = new List<Star> { };
        public List<Planet> Planets = new List<Planet> { };
        public List<LBeam> LBeams = new List<LBeam> { };
        public List<Explosion> Explosions = new List<Explosion> { };
        public IReadOnlyList<ClusterPlayer> Players;

        public double XMax;
        public double YMax;
        public Random Random;


        public World(IReadOnlyList<ClusterPlayer> players, Random random)
        {
            Players = players;
            Random = random;
        }

        public void Init()
        {
            AddPlanets();
            AddStars();
            PositionPlayers();
        }


        public void Update()
        {
            foreach (Explosion explosion in Explosions)
            {
                explosion.Update();
            }
        }

        public void Move()
        {
            foreach (LBeam lBeam in LBeams)
            {
                lBeam.Move();
            }

            foreach (Explosion explosion in Explosions)
            {
                explosion.Move();
            }

            foreach (ClusterPlayer player in Players)
            {
                player.Move();
            }
        }

        public void Act()
        {
            foreach (ClusterPlayer player in Players)
            {
                player.Shoot(this, Random);
                player.DoMagnification();
            }
        }

        public void Collide()
        {
            foreach (LBeam lBeam in LBeams)
            {
                lBeam.Collide(this);
            }

            foreach (ClusterPlayer player in Players)
            {
                player.Collide(this);
            }
        }

        public void CleanUp()
        {
            List<Explosion> tmp = new List<Explosion> { };
            foreach (Explosion explosion in Explosions)
            {
                if (!explosion.ReadyToDelete)
                {
                    tmp.Add(explosion);
                }
            }
            Explosions = tmp;

            List<LBeam> tmp2 = new List<LBeam> { };
            foreach (LBeam lBeam in LBeams)
            {
                if (!lBeam.ReadyToDelete)
                {
                    tmp2.Add(lBeam);
                }
            }
            LBeams = tmp2;
        }


        public void Draw(ClusterPlayer observer)
        {
            Vector obs = observer.Position;
            double m = observer.Magnification;
            IGraphics g = observer.Output.Graphics;

            foreach (ClusterPlayer player in Players)
            {
                player.Draw(observer);
            }

            foreach (Star star in Stars)
            {
                star.Draw(observer);
            }

            foreach (Planet planet in Planets)
            {
                planet.Draw(observer);
            }
            
            foreach (LBeam lbeam in LBeams)
            {
                lbeam.Draw(observer);
            }

            foreach (Explosion explosion in Explosions)
            {
                explosion.Draw(observer);
            }
        }

        public void PlaySound(ClusterPlayer observer)
        {
            foreach (Explosion explosion in Explosions)
            {
                explosion.PlaySound(observer);
            }

            foreach (ClusterPlayer player in Players)
            {
                player.PlaySound(observer);
            }
        }


        public void AddPlanets()
        {
            double xmax = 0;
            double ymax = 0;

            for (int i = 1; i <= Constants.World.NumberOfPlanets; i++)
            {
                bool foundplace = false;
                Vector p = new Vector(0, 0);
                double r = 0;
                int counter = 0;

                while (!foundplace)
                {
                    bool valid = true;

                    p = new Vector(Functions.NextDoubleNormal(Random, 0, Constants.World.SigmaX),
                        Functions.NextDoubleNormal(Random, 0, Constants.World.SigmaY));
                    r = Constants.World.MaxR * Math.Pow(i, -Constants.World.C);

                    foreach (Planet planet in Planets)
                    {
                        double d = (planet.Position - p).Length - (planet.Radius + r);

                        if (d < Constants.World.MinD)
                        {
                            valid = false;
                        }
                    }

                    if (valid)
                    {
                        foundplace = true;
                    }

                    counter++;

                    if (counter > 100000)
                    {
                        throw new Exception("Too many iterations to find a place for the planet");
                    }
                }

                Planets.Add(new Planet(p, r));

                xmax = Math.Max(xmax, Math.Abs(p.X) + r);
                ymax = Math.Max(ymax, Math.Abs(p.Y) + r);
            }

            XMax = 1.1 * xmax;
            YMax = 1.1 * ymax;
        }

        public void AddStars()
        {
            int n = (int)(Constants.World.DensityOfStars * 4 * XMax * YMax);

            for (int i = 0; i < n; i++)
            {
                Vector p = new Vector(Functions.NextDoubleNormal(Random, 0, Constants.World.SigmaX),
                    Functions.NextDoubleNormal(Random, 0, Constants.World.SigmaY));
                bool canBeSeen = true;

                foreach (Planet planet in Planets)
                {
                    if ((p - planet.Position).Length <= planet.Radius)
                    {
                        canBeSeen = false;
                        break;
                    }
                }

                if (canBeSeen)
                {
                    Stars.Add(new Star(p, 0.2 + 0.8 * Random.NextDouble()));
                }
            }
        }

        public void PositionPlayers()
        {
            for (int i = 0; i < Players.Count; i++)
            {
                bool foundplace = false;
                int counter = 0;
                ClusterPlayer player = Players[i];
                double size = player.Size;
                Vector p = new Vector(0, 0);

                while (!foundplace)
                {
                    bool valid = true;

                    p = new Vector(Functions.NextDoubleNormal(Random, 0, Constants.World.SigmaX),
                        Functions.NextDoubleNormal(Random, 0, Constants.World.SigmaY));

                    foreach (Planet planet in Planets)
                    {
                        double d = (planet.Position - p).Length - (planet.Radius + size);

                        if (d < Constants.World.MinD)
                        {
                            valid = false;
                        }
                    }

                    for (int j = 0; j < i; j++)
                    {
                        double d = (Players[j].Position - p).Length - (Players[j].Size + size);

                        if (d < Constants.World.MinD)
                        {
                            valid = false;
                        }
                    }

                    if (valid)
                    {
                        foundplace = true;
                    }

                    counter++;

                    if (counter > 100000)
                    {
                        throw new Exception("Too many iterations to find a place for the player");
                    }
                }

                player.Position = p;

            }
        }
    }
}