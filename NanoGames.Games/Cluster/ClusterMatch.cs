// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;


namespace NanoGames.Games.Cluster
{
    internal class ClusterMatch : Match<ClusterPlayer>
    {
        private World world;
        
        protected override void Initialize()
        {
            world = new World(Players, Random);
            world.Init();

            foreach (ClusterPlayer player in Players)
            {
                player.Birth();
                player.MagnificationMin = Math.Min(160.0 / world.XMax, 100.0 / world.YMax);
            }
            
        }

        protected override void Update()
        {
            // Collide -> Move(move) -> Draw -> PlaySound -> Update -> CleanUp
            world.Collide();

            world.Move();

            
            /*// Everything collides (major change needed here)
            if (Players.Count > 1)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    for (int j = i + 1; j < Players.Count; j++)
                    {
                        Players[i].Collide(Players[j]);
                    }
                }
            }*/

            
            // everthing can act
            foreach (ClusterPlayer player in Players)
            {
                player.Shoot(world, Random);
                player.DoMagnification();
            }

            // Everthing is drawn, sound is played
            foreach (ClusterPlayer observer in Players)
            {
                world.Draw(observer);
                world.PlaySound(observer);
            }

            world.Update();

            world.CleanUp();
            


        }
    }
}
