// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;


namespace NanoGames.Games.CatchMe
{
    internal class FrameCounter
    {
        private int internalCounter = 0;
        private int maxCount;

        public FrameCounter(double mc)
        {
            maxCount = (int)(mc * 60);
        }

        public void Tick()
        {
            internalCounter++;
        }

        public bool Tock()
        {
            if (internalCounter % maxCount == 0)
            {
                return true;
            }

            return false;
        }

        public double Ratio()
        {
            return (internalCounter % maxCount) / (double)maxCount;
        }

        public double RatioExpGrowth()
        {
            double x = Ratio();

            return 1 - Math.Exp(-1 / 0.1 * x);
        }
    }
}
