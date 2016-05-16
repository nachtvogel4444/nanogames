// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal class Bombstacle : AbstractRectbombularThing
    {
        public Bombstacle(BomberMatch match, Vector size) : this(match, new Vector(), size)
        {
        }

        public Bombstacle(BomberMatch match, Vector position, Vector size) : base(match, true, false, false, position, size)
        {
        }

        public override void Draw(Graphics g)
        {
            g.Line(Colors.White, Position, Position + new Vector(Size.X, 0));
            g.Line(Colors.White, Position + new Vector(Size.X, 0), Position + new Vector(Size.X, Size.Y));
            g.Line(Colors.White, Position + new Vector(Size.X, Size.Y), Position + new Vector(0, Size.Y));
            g.Line(Colors.White, Position + new Vector(0, Size.Y), Position);
            g.Line(Colors.White, Position, Position + new Vector(Size.X, Size.Y));
            g.Line(Colors.White, Position + new Vector(Size.X, 0), Position + new Vector(0, Size.Y));
        }
    }
}
