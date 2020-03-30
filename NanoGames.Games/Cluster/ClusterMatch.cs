// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;


namespace NanoGames.Games.Cluster
{
    internal class ClusterMatch : Match<ClusterPlayer>
    {
        private World world;
        private String state;
        
        protected override void Initialize()
        {   
            world = new World(Players, Random);
            state = "buildingWorld";
            world.Init();

            foreach (ClusterPlayer player in Players)
            {
                player.Birth();
                player.MagnificationMin = Math.Min(160.0 / world.XMax, 100.0 / world.YMax);
            }
            
        }

        protected override void Update()
        {
            switch (state)
            {
                case "buildingWorld":

                    world.Build();

                    break;



            }



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
        }
    }
}
