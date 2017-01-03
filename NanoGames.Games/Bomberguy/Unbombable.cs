// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal class Unbombable : AbstractRectbombularThing
    {
        private Color _color = new Color(0.4, 0.4, 0.4);

        private Glyph _glyph = new Glyph(8, 8) {
            // outer rectangle
            { 0, 0, 0, 8 },
            { 0, 8, 8, 8 },
            { 8, 8, 8, 0 },
            { 8, 0, 0, 0 },
            // inner part
            { 0, 2, 6, 2 },
            { 3, 4, 8, 4 },
            { 0, 6, 3, 6 }
        };

        public Unbombable(BomberMatch match, Vector size) : this(match, new Vector(), size)
        {
        }

        public Unbombable(BomberMatch match, Vector position, Vector size) : base(match, false, false, false, position, size)
        {
        }

        public override void Draw()
        {
            Match.Output.Graphics.Glyph(_color, _glyph, this.TopLeft, new Vector(this.Size.X * 0.98, 0), new Vector(0, this.Size.Y * 0.98));
        }
    }
}
