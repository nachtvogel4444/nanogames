using System;

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

        internal void Draw(Player best, Player secondBest)
        {
            Match.Output.Graphics.Line(LocalColor, Top, BottomRight);
            Match.Output.Graphics.Line(LocalColor, BottomRight, BottomLeft);
            Match.Output.Graphics.Line(LocalColor, BottomLeft, Top);

            Color scoreColor = this == best ? new Color(0, 1, 0) : this == secondBest ? new Color(1, 0.5, 0) : new Color(1, 0, 0);

            Match.Output.Graphics.Print(scoreColor, Constants.Match.ScoreFontSize, BottomLeft + new Vector(GetScoreXOffset(), 2), string.Format("{0}", this.Score));
        }

        internal Bullet Fire()
        {
            CanFire = false;
            Match.TimeOnce((int)(1000 / Constants.Weapon.FireRate), () => { CanFire = true; });
            return new Bullet { Player = this, Position = Top };
        }

        private double GetScoreXOffset()
        {
            int digitCount = 0;
            for (int i = (int)Score; i > 0; i /= 10) digitCount++;

            return -Math.Floor(digitCount / 2d) * Constants.Match.ScoreFontSize / 2d;
        }
    }
}
