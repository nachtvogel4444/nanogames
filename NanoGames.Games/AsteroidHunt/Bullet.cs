using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.AsteroidHunt
{
    class Bullet
    {
        private bool _exploded;
      
        public AsteroidHuntPlayer Player { get; set; }
        public Vector Position { get; set; }
        public int Damage { get; } = 1;
        public bool Valid => Position.Y > - Constants.Weapon.BulletLength && !_exploded;

        internal void Draw()
        {
            Player.Match.Output.Graphics.Line(Player.LocalColor, Position, Position + new Vector(0, Constants.Weapon.BulletLength));
        }

        internal void Explode(int points)
        {
            Player.Score += points;
            _exploded = true;
            Player.Match.Output.Particles.Gravity = new Vector(0, 0);
            Player.Match.Output.Particles.Velocity = new Vector(Constants.Weapon.BulletVelocity / 8d, 0);
            Player.Match.Output.Particles.Intensity = .7;

            Player.Match.Output.Particles.Line(Player.LocalColor, Position, Position + new Vector(0, Constants.Weapon.BulletLength));
            Player.Match.Output.Particles.Velocity = new Vector(-Constants.Weapon.BulletVelocity / 8d, 0);
            Player.Match.Output.Particles.Line(Player.LocalColor, Position, Position + new Vector(0, Constants.Weapon.BulletLength));
        }
    }
}
