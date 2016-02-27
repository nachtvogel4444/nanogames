// Copyright (c) the authors of NanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Collections.Generic;

namespace NanoGames
{
    /// <summary>
    /// Contains glyphs for all supported characters.
    /// </summary>
    internal static class Font
    {
        private static readonly Dictionary<int, Glyph> _glyphs = new Dictionary<int, Glyph>()
        {
            {
                '0', new Glyph(6, 6)
                {
                    { 1, 1, 5, 1, 5, 5, 1, 5, 1, 1 },
                    { 1, 2, 5, 4 },
                }
            },
            {
                '1', new Glyph(6, 6)
                {
                    { 2, 5, 3, 5, 3, 1 },
                    { 1, 1, 5, 1 },
                }
            },
            {
                '2', new Glyph(6, 6)
                {
                    { 1, 5, 5, 5, 5, 3, 1, 3, 1, 1, 5, 1 },
                }
            },
            {
                '3', new Glyph(6, 6)
                {
                    { 1, 5, 5, 5, 5, 1, 1, 1 },
                    { 2, 3, 5, 3 },
                }
            },
            {
                '4', new Glyph(6, 6)
                {
                    { 1, 5, 1, 3, 5, 3 },
                    { 5, 5, 5, 1 },
                }
            },
            {
                '5', new Glyph(6, 6)
                {
                    { 5, 5, 1, 5, 1, 3, 5, 3, 5, 1, 1, 1 },
                }
            },
            {
                '6', new Glyph(6, 6)
                {
                    { 5, 5, 1, 5, 1, 1, 5, 1, 5, 3, 1, 3 },
                }
            },
            {
                '7', new Glyph(6, 6)
                {
                    { 1, 5, 5, 5, 2, 1 },
                }
            },
            {
                '8', new Glyph(6, 6)
                {
                    { 1, 1, 5, 1, 5, 5, 1, 5, 1, 1 },
                    { 1, 3, 5, 3 },
                }
            },
            {
                '9', new Glyph(6, 6)
                {
                    { 1, 1, 5, 1, 5, 5, 1, 5, 1, 3, 5, 3 },
                }
            },
        };

        /// <summary>
        /// Gets the glyph for the specified character.
        /// </summary>
        /// <param name="c">The character.</param>
        /// <returns>The glyph, or null if this character has no glyph.</returns>
        public static Glyph GetGlyph(char c)
        {
            Glyph glyph;
            _glyphs.TryGetValue(c, out glyph);
            return glyph;
        }
    }
}
