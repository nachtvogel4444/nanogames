// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Infinity
{
    internal class Interval : IComparable // flo
    {
        public double A;
        public double B;

        public Interval(double a, double b)
        {

            A = a;
            B = b;
        }

        public int CompareTo(object obj)
        {
            Interval other = (Interval)obj;
            if (A != other.A)
            {
                return A.CompareTo(other.A);
            }
            else
            {
                return B.CompareTo(other.B);
            }
        }
    }
}
