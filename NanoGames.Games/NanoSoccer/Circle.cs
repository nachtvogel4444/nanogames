// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.NanoSoccer
{
    internal interface Circle
    {
        Vector Position { get; set; }

        double Radius { get; }

        Vector Velocity { get; set; }

        double MaximumVelocity { get; }

        double Mass { get; }
    }
}
