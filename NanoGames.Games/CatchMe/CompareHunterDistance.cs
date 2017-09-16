// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames.Games.CatchMe
{
    internal class CompareHunterDistance : IComparer<CatchMePlayer>
    {
        public int Compare(CatchMePlayer a, CatchMePlayer b)
        {
            double distA = a.IntegratedDistance;
            double distB = b.IntegratedDistance;

            if (distA == distB)
            {
                return 0;
            }
            if (distA < distB)
            {
                return -1;
            }

            return 1;
        }
    }
}
