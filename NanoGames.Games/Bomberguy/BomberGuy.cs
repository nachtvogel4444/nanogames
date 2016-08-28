// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Bomberguy
{
    internal class BomberGuy : Player<BomberMatch>, BomberThing
    {
        private Vector _position;
        private bool _particles;

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

        public Vector Speed
        {
            get; set;
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
            _particles = true;
            Match.TimeOnce(250, () => { _particles = false; });

            Match.TimeOnce(1750, () => { Dead = true; });
        }

        private Color GetComplementary(Color color)
        {
            return new Color(1 - color.R, 1 - color.G, 1 - color.B);
        }

        internal void Draw()
        {
            if (!Alive && !_particles) return;

            IGraphics g = _particles ? Output.Particles : Output.Graphics;

            if(_particles)
            {
                Match.Output.Particles.Gravity = new Vector(0, 0);
                Match.Output.Particles.Velocity = Speed;
                Match.Output.Particles.Intensity = Speed.Length > 0 ? 1 : 2; 
            }

            g.Line(LocalColor, Position + new Vector(Size.X / 2d, 0), Position + new Vector(Size.X, Size.Y / 2d));
            g.Line(LocalColor, Position + new Vector(Size.X, Size.Y / 2d), Position + new Vector(Size.X / 2d, Size.Y));
            g.Line(LocalColor, Position + new Vector(Size.X / 2d, Size.Y), Position + new Vector(0, Size.Y / 2d));
            g.Line(LocalColor, Position + new Vector(0, Size.Y / 2d), Position + new Vector(Size.X / 2d, 0));
        }
    }
}
