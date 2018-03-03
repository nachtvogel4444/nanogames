// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


namespace NanoGames.Games.Infinity
{
    internal class Star
    {
        public Vector Position;
        public bool IsDiscovered = false;

        public Star(Vector pos)
        {
            Position = pos;
        }
    }
}
