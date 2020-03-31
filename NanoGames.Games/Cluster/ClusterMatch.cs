// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;


namespace NanoGames.Games.Cluster
{
    internal class ClusterMatch : Match<ClusterPlayer>
    {
        private World world;
        private String gameState;
        
        protected override void Initialize()
        {   
            world = new World(Players, Random);
            gameState = "BuildingWorld";

            foreach (ClusterPlayer player in Players)
            {
                player.MagnificationMin = Math.Min(160.0 / world.XMax, 100.0 / world.YMax);
                player.Birth();
            }
            
        }

        protected override void Update()
        {
            switch (gameState)
            {
                case "BuildingWorld":

                    bool worldIsBuild = world.Build();
                    //bool worldIsBuild = false;
                    //bool d = world.Build();

                    if (worldIsBuild)
                    {
                        gameState = "GameRunning";
                    }
                    
                    foreach (ClusterPlayer observer in Players)
                    {
                        world.Draw(observer);
                        world.PlaySound(observer);
                    }

                    break;

                case "GameRunning":

                    world.Collide();
                    world.Move();
                    world.Act();

                    foreach (ClusterPlayer observer in Players)
                    {
                        world.Draw(observer);
                        world.PlaySound(observer);
                    }

                    world.Update();
                    world.CleanUp();

                    break;

            }
        }
    }
}
