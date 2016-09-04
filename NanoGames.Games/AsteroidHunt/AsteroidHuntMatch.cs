using System;
using System.Collections.Generic;
using System.Linq;

namespace NanoGames.Games.AsteroidHunt
{
    internal class AsteroidHuntMatch : Match<AsteroidHuntPlayer>
    {
        private List<Bullet> _bullets = new List<Bullet>();
        private List<Asteroid> _asteroids = new List<Asteroid>();
        private double _asteroidRate;
        private bool _spawn = true;
        private Player best = null;
        private Player secondBest = null;

        protected override void Initialize()
        {
            _asteroidRate = Constants.Match.InitialAsteroidRatePerPlayer * Players.Count;

            foreach (var p in Players)
            {
                p.Position = new Vector(160, 200 - p.Size.Y / 2d - Constants.Match.ScoreFontSize - 4);
            }
        }

        protected override void Update()
        {
            foreach (var b in _bullets)
            {
                b.Position += new Vector(0, -1) * Constants.Weapon.BulletVelocity;
                Asteroid a = CheckCollision(b);
                if (a != null)
                {
                    b.Explode(a.Score);
                    a.Hit(b);
                }
                if (b.Valid) b.Draw();
            }
            _bullets.RemoveAll((b) => !b.Valid);

            foreach (var a in _asteroids)
            {
                a.Position += a.Velocity;
                if (a.Valid) a.Draw();
            }
            _asteroids.RemoveAll((a) => !a.Valid);

            SpawnAsteroids();

            _asteroidRate += Constants.Match.AsteroidRateIncreasePerPlayer * Players.Count;

            foreach (var p in Players)
            {
                if (best == null || p.Score > best.Score) best = p;
                if (secondBest == null || (p.Score < best.Score && p.Score > secondBest.Score)) secondBest = p;

                if (p.Dead) return;
                CheckDeath(p);
                MovePlayer(p);
                Fire(p);
                p.Draw(best, secondBest);
            }

            this.IsCompleted = (Players.Count == 1 && Players[0].Dead) || (Players.Count > 1 && Players.Sum((p) => { return p.Dead ? 0 : 1; }) <= 1);
        }

        private void CheckDeath(AsteroidHuntPlayer p)
        {
            foreach (var a in _asteroids)
            {
                Vector distance = p.Position - a.Position;
                if (Math.Abs(distance.X) < (a.Size / 2d) && Math.Abs(distance.Y) < (a.Size / 2d + p.Size.Y / 2d))
                {
                    p.Dead = true;
                }
            }
        }

        private void SpawnAsteroids()
        {
            if (!_spawn) return;

            _asteroids.AddRange(Asteroid.Create(this));

            _spawn = false;
            TimeOnce((int)(1000d / _asteroidRate), () => { _spawn = true; });
        }

        private Asteroid CheckCollision(Bullet b)
        {
            foreach (var a in _asteroids)
            {
                if ((a.Position - b.Position).Length <= a.Size / 2d) return a;
            }
            return null;
        }

        private void MovePlayer(AsteroidHuntPlayer p)
        {
            Vector direction = new Vector();
            if (p.Input.Left.IsPressed && !p.Input.Right.IsPressed)
            {
                if (p.Position.X - Constants.Player.Velocity >= p.Size.X / 2d)
                {
                    direction = new Vector(-1, 0);
                }
            }
            else if (p.Input.Right.IsPressed && !p.Input.Left.IsPressed)
            {
                if (p.Position.X + Constants.Player.Velocity <= 320 - p.Size.X / 2d)
                {
                    direction = new Vector(1, 0);
                }
            }

            p.Position += direction * Constants.Player.Velocity;
        }

        private void Fire(AsteroidHuntPlayer p)
        {
            if (p.Input.Fire.IsPressed && p.CanFire)
            {
                _bullets.Add(p.Fire());
            }
        }
    }
}
