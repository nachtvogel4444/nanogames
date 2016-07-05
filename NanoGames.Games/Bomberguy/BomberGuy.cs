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

        public void Draw(IGraphics g)
        {
            /* Draw each player. */
            foreach (var player in Match.Players)
            {
                /* Skip players that have already finished. */
                if (player.Dead) continue;

                Color color = player.Color;

                if (player == this && Alive)
                {
                    /* Always show the current player in white. */
                    color = new Color(1, 1, 1);
                }

                g.Line(color, player.Position + new Vector(player.Size.X / 2d, 0), player.Position + new Vector(player.Size.X, player.Size.Y / 2d));
                g.Line(color, player.Position + new Vector(player.Size.X, player.Size.Y / 2d), player.Position + new Vector(player.Size.X / 2d, player.Size.Y));
                g.Line(color, player.Position + new Vector(player.Size.X / 2d, player.Size.Y), player.Position + new Vector(0, player.Size.Y / 2d));
                g.Line(color, player.Position + new Vector(0, player.Size.Y / 2d), player.Position + new Vector(player.Size.X / 2d, 0));
            }
        }

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
                this.Color = GetComplementary(Color);
            });

            Match.TimeOnce(1400, () => { Dead = true; });
        }

        private Color GetComplementary(Color color)
        {
            return new Color(255 - color.R, 255 - color.G, 255 - color.B);
        }
    }
}
