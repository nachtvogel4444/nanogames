﻿// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal abstract class AbstractRectbombularThing : RectbombularThing
    {
        public AbstractRectbombularThing(BomberMatch match, bool destroyable, bool passable, bool deadly, Vector size) : this(match, destroyable, passable, deadly, new Vector(), size)
        {
        }

        public AbstractRectbombularThing(BomberMatch match, bool destroyable, bool passable, bool deadly, Vector position, Vector size)
        {
            this.Destroyable = destroyable;
            this.Passable = passable;
            this.Deadly = deadly;
            this.Position = position;
            this.Size = size;
            this.Match = match;
        }

        public bool Destroyable { get; private set; }

        public bool Passable { get; private set; }

        public bool Deadly { get; private set; }

        public Vector Position { get; set; }

        public Vector Size { get; private set; }

        public Vector Center { get { return Position + new Vector(Size.X / 2d, Size.Y / 2d); } }

        public Vector TopLeft { get { return Position; } }

        public Vector TopRight { get { return Position + new Vector(Size.X, 0); } }

        public Vector BottomLeft { get { return Position + new Vector(0, Size.Y); } }

        public Vector BottomRight { get { return Position + Size; } }

        internal BomberMatch Match { get; private set; }

        public abstract void Draw();

        public void Destroy()
        {
            if (!Destroyable) return;

            var cell = Match.GetCellCoordinates(this);

            Match[cell] = null;

            OnDestroy(cell);
        }

        protected virtual void OnDestroy(CellCoordinates cell)
        {
        }
    }
}
