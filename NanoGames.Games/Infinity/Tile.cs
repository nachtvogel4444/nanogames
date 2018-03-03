// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Infinity
{
    internal class Tile
    {
        // a tile is rectangular region (320x200) of space

        public Vector Position;
        public double Size;
        public List<Star> Stars = new List<Star> { };
        public List<Planet> Planets = new List<Planet> { };
        public List<Tile> Neighbors = new List<Tile> { };
        private int NumberofStars;
        public List<InfinityPlayer> Players = new List<InfinityPlayer> { };

        public int X = 0;
        public int Y = 0;
        
        public Tile(Vector pos, double size)
        {
            Position = pos;
            Size = size;
            Neighbors.Add(this);
        }

        public void AddStars(Random r, int nos, InfinityPlayer player)
        {
            // flo
            NumberofStars = nos;

            for (int i = 0; i < NumberofStars; i++)
            {
                var pos = Size * new Vector(r.NextDouble(), r.NextDouble());
                var add = true;

                // check if star is behind planet
                foreach (Tile tile in Neighbors)
                {
                    foreach (Planet planet in tile.Planets)
                    {
                        if ((tile.Position + planet.Position - Position - pos).Length <= planet.Radius)
                        {
                            add = false;
                            break;
                        }
                    }
                }

                if (add)
                {
                    Star newStar = new Star(pos);
                    Stars.Add(newStar);
                }
            }
        }

        public void DiscoverStars(InfinityPlayer player)
        {
            foreach (Tile tile in Neighbors)
            {
                foreach (Star star in tile.Stars)
                {
                    if ((tile.Position + star.Position - player.Position).Length < player.View)
                    {
                        star.IsDiscovered = true;
                    }
                }
            }
        }

        public void DiscoverPlanets(InfinityPlayer player)
        {
            foreach (Tile tile in Neighbors)
            {
                foreach (Planet planet in tile.Planets)
                {
                    planet.Explore(tile.Position, player);
                }
            }
        }

        public void AddPlanets(Random r, double probability, double radius)
        {
            if (r.NextDouble() < probability)
            {
                var pos = Size * new Vector(r.NextDouble(), r.NextDouble());
                var add = true;

                foreach (Tile tile in Neighbors)
                {
                    foreach (Planet planet in tile.Planets)
                    {
                        if ((tile.Position + planet.Position - Position - pos).Length < planet.Radius + radius)
                        {
                            add = false;
                            break;
                        }
                    }
                }

                if (add)
                {
                    Planets.Add(new Planet(pos, Math.Min(Size, radius)));

                    foreach (Tile tile in Neighbors)
                    {
                        var newList = new List<Star> { };

                        foreach (Star star in tile.Stars)
                        {
                            if ((tile.Position + star.Position - Position - pos).Length > radius)
                            {
                                newList.Add(star);
                            }
                        }

                        tile.Stars = newList;
                    }
                }
            }
        }
        
        public void AddNeighbors(List<Tile> tiles)
        {
            int index;

            // add neigbors of this tile (as it is born)
            index = tiles.FindIndex(x => x.X == X - 1 && x.Y == Y - 1);
            if (index != -1) { Neighbors.Add(tiles[index]); tiles[index].Neighbors.Add(this); }
            index = tiles.FindIndex(x => x.X == X && x.Y == Y - 1);
            if (index != -1) { Neighbors.Add(tiles[index]); tiles[index].Neighbors.Add(this); }
            index = tiles.FindIndex(x => x.X == X + 1 && x.Y == Y - 1);
            if (index != -1) { Neighbors.Add(tiles[index]); tiles[index].Neighbors.Add(this); }
            index = tiles.FindIndex(x => x.X == X + 1 && x.Y == Y);
            if (index != -1) { Neighbors.Add(tiles[index]); tiles[index].Neighbors.Add(this); }
            index = tiles.FindIndex(x => x.X == X + 1 && x.Y == Y + 1);
            if (index != -1) { Neighbors.Add(tiles[index]); tiles[index].Neighbors.Add(this); }
            index = tiles.FindIndex(x => x.X == X && x.Y == Y + 1);
            if (index != -1) { Neighbors.Add(tiles[index]); tiles[index].Neighbors.Add(this); }
            index = tiles.FindIndex(x => x.X == X - 1 && x.Y == Y + 1);
            if (index != -1) { Neighbors.Add(tiles[index]); tiles[index].Neighbors.Add(this); }
            index = tiles.FindIndex(x => x.X == X - 1 && x.Y == Y);
            if (index != -1) { Neighbors.Add(tiles[index]); tiles[index].Neighbors.Add(this); }

            // add this tile as neighbor to others

        }
    }
}
