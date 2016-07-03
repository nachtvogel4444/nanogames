// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal interface RectbombularThing : BomberThing
    {
        Vector TopLeft { get; }

        Vector TopRight { get; }

        Vector BottomLeft { get; }

        Vector BottomRight { get; }
    }
}
