// Copyright (c) the authors of nanoGames. All rights reserved.

// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal class BomberGuy : Player<BomberMatch>, BomberThing
    {
        public bool Dead
        {
            get; set;
        }

        public bool Destroyable
        {
            get { return true; }
        }

        public bool Passable
        {
            get { return true; }
        }

        public bool Deadly
        {
            get { return false; }
        }

        public Vector Position
        {
            get; set;
        }

        public Vector Size
        {
            get; set;
        }

        public Vector Center { get { return Position + new Vector(Size.X / 2d, Size.Y / 2d); } }

        public void Draw(Graphics g)
        {
            Draw(g, this, this.Color);
        }

        public void Destroy()
        {
            if (Dead) return;

            Dead = true;

            this.Score = Match.DeadPlayers++;
        }

        private void Draw(Graphics g, BomberGuy p, Color c)
        {
            g.Line(c, p.Position + new Vector(p.Size.X / 2d, 0), p.Position + new Vector(p.Size.X, p.Size.Y / 2d));
            g.Line(c, p.Position + new Vector(p.Size.X, p.Size.Y / 2d), p.Position + new Vector(p.Size.X / 2d, p.Size.Y));
            g.Line(c, p.Position + new Vector(p.Size.X / 2d, p.Size.Y), p.Position + new Vector(0, p.Size.Y / 2d));
            g.Line(c, p.Position + new Vector(0, p.Size.Y / 2d), p.Position + new Vector(p.Size.X / 2d, 0));
        }
    }
}
