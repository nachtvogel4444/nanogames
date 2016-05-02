// Copyright (c) the authors of nanoGames. All rights reserved.

// Licensed under the MIT license. See LICENSE.txt in the project root.

using System.Timers;

namespace NanoGames.Games.Bomberguy
{
    internal class Bomb : AbstractBomberThing
    {
        private Timer bombTimer;
        private int reach;

        public Bomb(int reach, BomberMatch match, Vector size) : this(reach, match, new Vector(), size)
        {
        }

        public Bomb(int reach, BomberMatch match, Vector position, Vector size) : base(match, true, false, false, position, size)
        {
            bombTimer = new Timer(3000);
            bombTimer.Elapsed += BombTimer_Elapsed;
            bombTimer.Start();
            this.reach = reach;
        }

        public delegate void ExplodingHandler();

        public event ExplodingHandler Exploding;

        public override void Draw(Graphics g)
        {
            g.Circle(Colors.White, this.Center, this.Size.X / 2d * 0.8);
        }

        protected override void OnDestroy(Vector cell)
        {
            CreateExplosions(cell);

            Match.CheckExplosions();
        }

        private void BombTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bombTimer.Stop();
            bombTimer.Dispose();
            Destroy();
        }

        private void CreateExplosions(Vector cell)
        {
            Match[cell] = new Explosion(Explosion.Type.CENTER, Match, Match.GetCoordinates(cell), Match.CellSize);

            var direction = new Vector(-1, 0);
            for (int i = 0; i < 4; i++)
            {
                direction = direction.RotatedRight;

                for (int r = 1; r <= reach; r++)
                {
                    var explosionCell = cell + direction * r;
                    var cellContent = Match[explosionCell];

                    if (cellContent == null)
                    {
                        Match[explosionCell] = new Explosion(getExplosionType(r, direction), Match, Match.GetCoordinates(explosionCell), Match.CellSize);
                    }
                    else
                    {
                        if (cellContent.Destroyable)
                        {
                            cellContent.Destroy();
                            Match[explosionCell] = new Explosion(getExplosionType(reach, direction), Match, Match.GetCoordinates(explosionCell), Match.CellSize);
                        }
                        break;
                    }
                }
            }
        }

        private Explosion.Type getExplosionType(int r, Vector direction)
        {
            if (direction.Y == -1)
            {
                if (r == reach) return Explosion.Type.TOPEND;
                else return Explosion.Type.COLUMN;
            }
            else if (direction.X == 1)
            {
                if (r == reach) return Explosion.Type.RIGHTEND;
                else return Explosion.Type.ROW;
            }
            else if (direction.Y == 1)
            {
                if (r == reach) return Explosion.Type.BOTTOMEND;
                else return Explosion.Type.COLUMN;
            }
            else
            {
                if (r == reach) return Explosion.Type.LEFTEND;
                else return Explosion.Type.ROW;
            }
        }
    }
}
