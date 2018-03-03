// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Infinity
{
    internal class Planet
    {
        public Vector Position;
        public double Radius;
        public List<Interval> Alphas = new List<Interval> { } ;
        public bool IsFullyDiscovered = false;

        public Planet(Vector pos, double r)
        {
            Position = pos;
            Radius = r;
        }

        public void Explore(Vector posTile, InfinityPlayer player)
        {
            var p = player.Position - posTile;
            var v = player.View;
            var pp = Position - p;
            var d = pp.Length;
            
            if (d < v + Radius && !IsFullyDiscovered)
            {
                var a = (Radius * Radius - v * v + d * d) / (2 * d);
                var p2 = Position - a * pp / d;
                var h = Math.Sqrt(Radius * Radius - a * a);

                var x1 = p2.X + h * (p.Y - Position.Y) / d - Position.X;
                var x2 = p2.X - h * (p.Y - Position.Y) / d - Position.X;
                var y1 = p2.Y - h * (p.X - Position.X) / d - Position.Y;
                var y2 = p2.Y + h * (p.X - Position.X) / d - Position.Y;

                var alpha1 = Math.Atan2(y1, x1);
                var alpha2 = Math.Atan2(y2, x2);

                if (alpha1 < alpha2)
                {
                    Alphas = Merge(Alphas, new Interval(alpha1, alpha2));
                }
                if (alpha1 > alpha2)
                {
                    Alphas = Merge(Alphas, new Interval(alpha1, Math.PI));
                    Alphas = Merge(Alphas, new Interval(-Math.PI, alpha2));
                }

                if (Alphas.Count == 1 && Alphas[0].A == -Math.PI && Alphas[0].B == Math.PI)
                {
                    IsFullyDiscovered = true;
                }
            }
        }

        public List<Interval> Merge(List<Interval> intervals, Interval newInterval)
        {
            var output = new List<Interval> { };
            intervals.Add(newInterval);
            
            intervals.Sort();

            var pre = intervals[0];
            for (int i = 0; i < intervals.Count; i++)
            {
                var curr = intervals[i];

                if (curr.A > pre.B)
                {
                    output.Add(pre);
                    pre = curr;
                }
                else
                {
                    var merged = new Interval(pre.A, Math.Max(pre.B, curr.B));
                    pre = merged;
                }
            }

            output.Add(pre);
            return output;
        }
    }
}
