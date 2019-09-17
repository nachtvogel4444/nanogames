// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Cluster
{
    internal class Explosion
    {

        private Vector position;
        private double speed;
        private int duration;
        private int lifetime;
        private int number;
        private Random random;
        private List<Flake> flakes;
        public bool IsExploded;

        public Explosion(Vector pos, double s, int dur, int n, Random ran)
        {
            position = pos;
            number = 200;
            random = ran;
            IsExploded = false;

            flakes = new List<Flake> { };
            for (int i = 0; i < number; i++)
            {
                flakes.Add(new Flake(position, random));
            }
        }

        public void Draw(ClusterPlayer observer)
        {
            foreach (Flake flake in flakes)
            {
                flake.Update();
                flake.Draw(observer);
            }

        }

        public void PlaySound(ClusterPlayer observer)
        {
            IAudio a = observer.Output.Audio;

            if (!IsExploded)
            {
                a.Play(Sounds.Explosion);
                IsExploded = true;
            }
        }

    }
}
