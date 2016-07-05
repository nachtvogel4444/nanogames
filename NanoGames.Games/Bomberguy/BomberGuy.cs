// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal class BomberGuy : Player<BomberMatch>, BomberThing
    {
        private Vector _position;

        public bool Dying
        {
            get; private set;
        }

        public bool Dead
        {
            get; private set;
        }

        public bool Alive
        {
            get { return !Dead && !Dying; }
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
            get { return _position; }

            set
            {
                if (!Alive) return;
                _position = value;
            }
        }

        public Vector Size
        {
            get; set;
        }

        public Vector Center { get { return Position + new Vector(Size.X / 2d, Size.Y / 2d); } }

        public void Destroy()
        {
            if (!Alive) return;

            Dying = true;

            Match.TimeCyclic(200, (t) =>
            {
                if (Dead)
                {
                    t.Stop();
                    t.Dispose();
                    return;
                }
                this.LocalColor = GetComplementary(LocalColor);
            });

            Match.TimeOnce(1400, () => { Dead = true; });
        }

        private Color GetComplementary(Color color)
        {
            return new Color(1 - color.R, 1 - color.G, 1 - color.B);
        }
    }
}
