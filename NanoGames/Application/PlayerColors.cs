// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames.Application
{
    /// <summary>
    /// Contains a list of player colors.
    /// </summary>
    internal static class PlayerColors
    {
        /// <summary>
        /// A list of possible player color values.
        /// </summary>
        public static readonly IReadOnlyList<Color> Values =
            new List<Color>
            {
                new Color(1, 0.25, 0),
                new Color(1, 0.75, 0),
                new Color(0.75, 1, 0),
                new Color(0.25, 1, 0),
                new Color(0, 1, 0.25),
                new Color(0, 1, 0.75),
                new Color(0, 0.75, 1),
                new Color(0, 0.25, 1),
                new Color(0.25, 0, 1),
                new Color(0.75, 0, 1),
                new Color(1, 0, 0.75),
                new Color(1, 0, 0.25),
            }.AsReadOnly();
    }
}
