// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames
{
    /// <summary>
    /// Contains standard colors.
    /// </summary>
    public static class Colors
    {
        /// <summary>
        /// Full white.
        /// </summary>
        public static readonly Color White = new Color(1, 1, 1);

        /// <summary>
        /// The color used for controls.
        /// </summary>
        public static readonly Color Control = new Color(0.0, 0.15, 0.60);

        /// <summary>
        /// The color used for controls when they are focused.
        /// </summary>
        public static readonly Color FocusedControl = new Color(0.60, 0.60, 0.60);

        /// <summary>
        /// The color used for screen titles.
        /// </summary>
        public static readonly Color Title = new Color(0.05, 0.45, 0);

        /// <summary>
        /// The color used for errors.
        /// </summary>
        public static readonly Color Error = new Color(1.0, 0.15, 0.0);
    }
}
