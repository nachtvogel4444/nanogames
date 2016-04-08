// Copyright (c) the authors of nanoGames. All rights reserved.

// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    public abstract class AbstractBomberThing : BomberThing
    {
        public AbstractBomberThing(bool destroyable, bool passable, Vector position, Vector size)
        {
            this.Destroyable = destroyable;
            this.Position = position;
            this.Size = size;
        }

        public bool Destroyable { get; private set; }

        public bool Passable { get; private set; }

        public Vector Position { get; private set; }

        public Vector Size { get; private set; }

        public Vector Center { get { return Position + new Vector(Size.X / 2d, Size.Y / 2d); } }

        public abstract void Draw(Graphics g);
    }
}
