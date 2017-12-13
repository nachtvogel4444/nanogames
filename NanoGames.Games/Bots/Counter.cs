// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bots
{
    internal class Counter
    {
        public int DurationF;
        public int StartFrame;

        public Counter(double duration, int startFrame)
        {
            DurationF = (int)(duration * 60.0);
            StartFrame = startFrame;
        }

        public void Reset(int frame)
        {
            StartFrame = frame;
        }
        
        public bool AfterFirstEvent(int frame) => (frame - StartFrame) >= DurationF;

        public bool IsEvent(int frame) => ((frame - StartFrame) % DurationF) == 0;
        
    }
}
