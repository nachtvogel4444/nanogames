// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal class Bomb : AbstractRectbombularThing
    {
        private BomberGuy player;
        private int reach;

        public Bomb(int reach, BomberGuy player, BomberMatch match, Vector size) : this(reach, player, match, new Vector(), size)
        {
        }

        public Bomb(int reach, BomberGuy player, BomberMatch match, Vector position, Vector size) : base(match, true, false, false, position, size)
        {
            match.TimeOnce(2000, () => Destroy());
            this.reach = reach;
            this.player = player;
        }

        public override void Draw(IGraphics g)
        {
            g.Circle(player.Color, this.Center, this.Size.X / 2d * 0.8);
        }

        protected override void OnDestroy(Vector cell)
        {
            Match.Output.Audio.Play(Sounds.Explosion);

            CreateExplosions(cell);

            Match.CheckAllDeaths();
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
