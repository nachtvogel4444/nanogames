using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.AsteroidHunt
{
    class Asteroid
    {
        private double _damage;
        private bool _exploded;

        public AsteroidHuntMatch Match { private get; set; }
        public int Score { get; set; }
        public Vector Position { get; set; }
        public Vector Velocity { get; set; }
        public double Size { get; set; }
        public int Hitpoints { get; set; }
        public Action<Asteroid> ExplodeAction { get; set; }
        public bool Valid => !_exploded && Position.X > -Size / 2d && Position.X < 320 + Size / 2d && Position.Y < 200 + Size / 2d;

        internal void Hit(Bullet b)
        {
            _damage += b.Damage;
            if (_damage >= Hitpoints)
            {
                _exploded = true;
                Match.Output.Particles.Gravity = new Vector(0, 0);
                Match.Output.Particles.Velocity = Velocity;
                Match.Output.Particles.Intensity = 1;

                Match.Output.Particles.Circle(new Color(1, 0, 0), Position, Size / 2d);
            }
        }

        internal void Draw()
        {
            Match.Output.Graphics.Circle(new Color(1, 0, 0), Position, Size / 2d);
        }
    }
}
