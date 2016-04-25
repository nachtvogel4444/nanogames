// Copyright (c) the authors of nanoGames. All rights reserved.

// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal class BomberGuy : Player<BomberMatch>, BomberThing
    {
        private bool dead;

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
            g.Line(Colors.White, Position + new Vector(Size.X / 2d, 0), Position + new Vector(Size.X, Size.Y / 2d));
            g.Line(Colors.White, Position + new Vector(Size.X, Size.Y / 2d), Position + new Vector(Size.X / 2d, Size.Y));
            g.Line(Colors.White, Position + new Vector(Size.X / 2d, Size.Y), Position + new Vector(0, Size.Y / 2d));
            g.Line(Colors.White, Position + new Vector(0, Size.Y / 2d), Position + new Vector(Size.X / 2d, 0));
        }

        public void Destroy()
        {
            if (dead) return;

            dead = true;

            this.Score = Match.DeadPlayers++;
        }
    }
}
