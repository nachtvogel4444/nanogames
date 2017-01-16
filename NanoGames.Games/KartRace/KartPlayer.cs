using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.KartRace
{
    class KartPlayer : Player<KartMatch>
    {
        public Vector Position;
        public Vector Velocity;
        public Rotation Direction;

        public bool HasFinished;

        public int Round = -1;
        public double MaxAngle;

        public void Render()
        {
            var graphics = Output.Graphics;

            var roundString = $"ROUND {Math.Max(1, Round + 1)}";
            graphics.Print(new Color(1, 1, 1), 8, new Vector(316 - roundString.Length * 8, 16), roundString);

            var trackAngle = Math.Atan2(Position.Y, Position.X);
            if (Vector.Dot(Match.GetTrackTangent(trackAngle), Direction.ToVector()) < 0)
            {
                graphics.PrintCenter(Colors.Error, 8, new Vector(160, 24), "WRONG WAY");
            }

            graphics.Line(Constants.HorizonColor, new Vector(0, 100), new Vector(320, 100));

            foreach (var line in Match.TrackLines)
            {
                var p = Translate(line.P);
                var q = Translate(line.Q);

                if (p.X <= 0 && q.X <= 0)
                {
                    continue;
                }
                else if (p.X < 0 && q.X > 0)
                {
                    p.Y = p.Y - p.X * (q.Y - p.Y) / (q.X - p.X);
                    p.X = 0;
                }
                else if (p.X > 0 && q.X < 0)
                {
                    q.Y = p.Y - p.X * (q.Y - p.Y) / (q.X - p.X);
                    q.X = 0;
                }

                p = Project(p);
                q = Project(q);

                graphics.Line(line.Color, p, q);
            }

            foreach (var player in Match.Players)
            {
                if (player.HasFinished) continue;

                var p = Translate(player.Position);
                if (p.X < 0) continue;

                var r = Constants.PlayerRadius * 160 / (p.X + Constants.NearOffset);
                p = Project(p);

                graphics.Circle(player.LocalColor, p, r);
            }
        }

        private Vector Translate(Vector v)
        {
            return Direction.Inverse * (v - Position) + new Vector(Constants.CameraOffset, 0);
        }

        private Vector Project(Vector v)
        {
            double x = v.X + Constants.NearOffset;
            return new Vector(160 - v.Y / x * 160, 100 + (1.0 * 160) / x);
        }
    }
}
