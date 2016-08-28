// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal class Bombstacle : AbstractRectbombularThing
    {
        private Color _color = new Color(0.65, 0.4, 0.2);

        private Glyph _glyph = new Glyph(11, 11) {
            // outer shape
            { 0, 0, 1, 0, 2, 1, 4, 1, 4, 0, 11, 0 },
            { 11, 0, 11, 6, 10, 8, 10, 9, 11, 10, 11, 11 },
            { 11, 11, 5, 11, 4, 10, 3, 10, 2, 11, 0, 11 },
            { 0, 11, 0, 0 },
            // inner crack
            { 2, 3, 3, 4, 2, 5 },
            { 3, 4, 4, 4, 6, 6 },
            { 6, 6, 9, 3 },
            { 8, 4, 7, 3 },
            { 6, 6, 4, 7, 2, 9 },
            { 3, 8, 4, 9 },
            { 6, 6, 7, 9, 7, 10},
            { 7, 9, 8, 9 }
        };

        public Bombstacle(BomberMatch match, Vector size) : this(match, new Vector(), size)
        {
        }

        public Bombstacle(BomberMatch match, Vector position, Vector size) : base(match, true, false, false, position, size)
        {
        }

        public override void Draw()
        {
            Match.Output.Graphics.Glyph(_color, _glyph, this.TopLeft, new Vector(this.Size.X * 0.98, 0), new Vector(0, this.Size.Y * 0.98));
        }
    }
}
