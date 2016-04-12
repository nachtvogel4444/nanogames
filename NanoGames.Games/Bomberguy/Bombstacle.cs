// Copyright (c) the authors of nanoGames. All rights reserved.

// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Bomberguy
{
    internal class Bombstacle : AbstractBomberThing
    {
        public Bombstacle(BomberMatch match, Vector position, Vector size) : base(match, true, false, position, size)
        {
        }

        public override void Draw(Graphics g)
        {
            throw new NotImplementedException();
        }
    }
}
