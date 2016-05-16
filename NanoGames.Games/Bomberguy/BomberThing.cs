// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal interface BomberThing
    {
        Vector Position { get; set; }

        Vector Size { get; }

        Vector Center { get; }

        Vector TopLeft { get; }

        Vector TopRight { get; }

        Vector BottomLeft { get; }

        Vector BottomRight { get; }

        bool Destroyable { get; }

        bool Passable { get; }

        bool Deadly { get; }

        void Draw(Graphics g);

        void Destroy();
    }
}
