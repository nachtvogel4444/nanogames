using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.AsteroidHunt
{
    internal class AsteroidHuntPlayer : Player<AsteroidHuntMatch>
    {
        internal bool Dead;

        public Vector Position { get; set; }
        public Vector Size { get { return Constants.Player.Size; } }

        public Vector Top { get { return Position - new Vector(0, Size.Y / 2d); } }
        public Vector BottomLeft { get { return Position + new Vector(-Size.X / 2d, Size.Y / 2d); } }
        public Vector BottomRight { get { return Position + Size / 2d; } }

        public bool CanFire { get; private set; } = true;

        internal void Draw()
        {
            Match.Output.Graphics.Line(LocalColor, Top, BottomRight);
            Match.Output.Graphics.Line(LocalColor, BottomRight, BottomLeft);
            Match.Output.Graphics.Line(LocalColor, BottomLeft, Top);
            Match.Output.Graphics.Print(LocalColor, Constants.Match.ScoreFontSize, BottomLeft + new Vector(GetScoreXOffset(), 2), string.Format("{0}", this.Score));
        }

        private double GetScoreXOffset()
        {
            int digitCount = 0;
            for (int i = (int)Score; i > 0; i /= 10) digitCount++;

            return -Math.Floor(digitCount / 2d) * Constants.Match.ScoreFontSize /2d;
        }

        internal Bullet Fire()
        {
            CanFire = false;
            Match.TimeOnce((int) (1000 / Constants.Weapon.FireRate), () => { CanFire = true; });
            return new Bullet { Player = this, Position = Top };
        }
    }
}
