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

        public double XMax;
        public double YMax;

        public void AddPlanets(Random rand)
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

                    p = new Vector(normDouble(rand, 0, Constants.World.SigmaX), normDouble(rand, 0, Constants.World.SigmaY));
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

        public void AddStars(Random rand)
        {
            int n = (int)(Constants.World.DensityOfStars * 4 * XMax * YMax);

            for (int i = 0; i < n; i++)
            {
                Vector p = new Vector(normDouble(rand, 0, Constants.World.SigmaX), normDouble(rand, 0, Constants.World.SigmaY));
                bool canBeSeen = true;

                foreach (Planet planet in Planets)
                {
                    if((p - planet.Position).Length <= planet.Radius)
                    {
                        canBeSeen = false;
                        break;
                    }
                }

                if (canBeSeen)
                {
                    Stars.Add(new Star(p, 0.2 + 0.8 * rand.NextDouble()));
                }
            }
        }

        public void PlayerPositions(Random rand, IReadOnlyList<ClusterPlayer> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                bool foundplace = false;
                int counter = 0;
                ClusterPlayer player = players[i];
                double size = player.Size;
                Vector p = new Vector(0, 0);

                while (!foundplace)
                {
                    bool valid = true;

                    p = new Vector(normDouble(rand, 0, Constants.World.SigmaX), normDouble(rand, 0, Constants.World.SigmaY));

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
                        double d = (players[j].Position - p).Length - (players[j].Size + size);

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

                player.Position = p;

            }
        }

        public void Draw(ClusterPlayer observer)
        {
            Vector obs = observer.Position;
            double m = observer.Magnification;
            IGraphics g = observer.Output.Graphics;
            
            foreach (Star star in Stars)
            {
                star.Draw(observer);
            }

            foreach (Planet planet in Planets)
            {
                planet.Draw(observer);
            }
        }

        private double normDouble(Random rand, double mu, double sigma)
        {
            double u1 = 1 - rand.NextDouble();
            double u2 = 1 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);


            return mu + sigma * randStdNormal;
        }
    }
}