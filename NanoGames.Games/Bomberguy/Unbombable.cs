// Copyright (c) the authors of nanoGames. All rights reserved.

// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal class Unbombable : AbstractBomberThing
    {
        public Unbombable(BomberMatch match, Vector size) : this(match, new Vector(), size)
        {
        }

        public Unbombable(BomberMatch match, Vector position, Vector size) : base(match, false, false, false, position, size)
        {
        }

        public override void Draw(Graphics g)
        {
            g.Rectangle(Colors.White, Position, Position + Size);
            for (double i = 1; i < Size.Y - 1; i++)
            {
                g.Line(Colors.White, this.Position + new Vector(1, i), this.Position + new Vector(this.Size.X - 1, i));
            }
        }
    }
}
