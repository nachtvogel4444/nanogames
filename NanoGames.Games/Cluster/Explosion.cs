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

        public Vector Position;
        private int maxtime;
        private int number;
        private Random random;
        private List<Flake> flakes;

        public bool AudioActive;
        public bool ReadyToDelete;
        public int Time;


        public Explosion(Vector pos, Random ran)
        {
            Position = pos;
            number = 200;
            random = ran;
            AudioActive = true;
            
            maxtime = 22;
            Time = 0;

            flakes = new List<Flake> { };
            for (int i = 0; i < number; i++)
            {
                flakes.Add(new Flake(Position, maxtime, random));
            }
        }


        public void Update()
        {
            foreach (Flake flake in flakes)
            {
                flake.Update();
            }

            Time++;
            if (Time > maxtime-1) { ReadyToDelete = true; }
        }

        public void Move()
        {
            foreach (Flake flake in flakes)
            {
                flake.Move();
            }
        }


        public void Draw(ClusterPlayer observer)
        {
            foreach (Flake flake in flakes)
            {
                flake.Draw(observer);
            }

        }

        public void PlaySound(ClusterPlayer observer)
        {
            IAudio a = observer.Output.Audio;

            double m = observer.Magnification;
            Vector obs = observer.Position;

            Vector posOnScreen = Position.Translated(-obs).Scaled(m).ToOrigin();

            if (AudioActive &&
                posOnScreen.X > 0 && posOnScreen.X < 320 && posOnScreen.Y > 0 && posOnScreen.Y < 200)
            {
                a.Play(Sounds.Explosion);
                AudioActive = false;
            }
        }

    }
}
