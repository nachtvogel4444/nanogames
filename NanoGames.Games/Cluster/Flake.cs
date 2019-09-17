// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Cluster
{
    internal class Flake
    {
        public Vector Position;
        public Vector Velocity;
        public Color Color;
        public List<Color> ColorList;

        public int MaxTime;
        public int StartTime;
        public int Time;
        private int lifeTime;

        public bool IsStarted;
        public bool IsDead;

        public Flake(Vector pos, Random ran)
        {
            Position = pos;
            double angle = ran.NextDouble() * 2 * Math.PI;
            Velocity = getRan(ran, 0.5, 1) * new Vector(Math.Sin(angle), Math.Cos(angle));
            MaxTime = (int)getRan(ran, 10, 30);
            StartTime = (int)getRan(ran, -15, 0);
            ColorList = new List<Color> { new Color(0.996, 1, 0.710), new Color(0.996, 1, 0.710), new Color(0.996, 1, 0.710), new Color(0.996, 1, 0.710), new Color(0.996, 1, 0.710), new Color(0.996, 1, 0.710),
                new Color(1, 0.753, 0.443), new Color(1, 0.753, 0.443), new Color(1, 0.753, 0.443), new Color(1, 0.753, 0.443), new Color(1, 0.753, 0.443), new Color(1, 0.753, 0.443),
                new Color(1, 0.635, 0.333), new Color(1, 0.635, 0.333), new Color(1, 0.635, 0.333), new Color(1, 0.635, 0.333), new Color(1, 0.635, 0.333), new Color(1, 0.635, 0.333),
                new Color(1, 0.357, 0.0784), new Color(1, 0.357, 0.0784), new Color(1, 0.357, 0.0784), new Color(1, 0.357, 0.0784), new Color(1, 0.357, 0.0784), new Color(1, 0.357, 0.0784),
                new Color(1, 0.137, 0.137), new Color(1, 0.137, 0.137), new Color(1, 0.137, 0.137), new Color(1, 0.137, 0.137), new Color(1, 0.137, 0.137), new Color(1, 0.137, 0.137), new Color(1, 0.137, 0.137) };

            IsDead = false;
            IsStarted = false;

            Time = 0;
        }

        private double getRan(Random ran, double a, double b)
        {
            return a + (b - a) * ran.NextDouble();
        }

        public void Update()
        {
            Position += Velocity;
            IsDead = Time > MaxTime + StartTime;
            IsStarted = Time > StartTime;
            Time++; 
        }

        public void Draw(ClusterPlayer observer)
        {
            if (IsStarted && !IsDead)
            {
                double m = observer.Magnification;
                Vector obs = observer.Position;
                Color = ColorList[Time - StartTime];
                IGraphics g = observer.Output.Graphics;

                Vector p = Position.Translated(-obs).Scaled(m).ToOrigin();
                g.PPoint(Color, p);
            }
        }

    }
}
