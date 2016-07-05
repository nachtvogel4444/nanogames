// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Example
{
    internal class ExamplePlayer : Player<ExampleMatch>
    {
        public bool HasFinished = false;

        public Vector Position;

        public Vector Velocity;
    }
}
