// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal class Bomb : AbstractRectbombularThing
    {
        private BomberGuy player;
        private IMatchTimer timer;

        public Bomb(BomberGuy player, BomberMatch match) : this(player, match, new Vector())
        {
        }

        public Bomb(BomberGuy player, BomberMatch match, Vector position) : base(match, true, false, false, position, match.CellSize * Constants.Bomb.REL_SIZE)
        {
            timer = match.TimeOnce(2000, () => Destroy());
            this.player = player;
        }

        public override void Draw()
        {
            Match.Output.Graphics.Circle(player.Color, this.Center, this.Size.X / 2d * 0.8);
        }

        protected override void OnDestroy(CellCoordinates cell)
        {
            timer.Stop();
            timer.Dispose();

            Match.Output.Audio.Play(Sounds.Explosion);

            CreateExplosions(cell);

            Match.CheckAllDeaths();
        }

        private void CreateExplosions(CellCoordinates cell)
        {
            Match[cell] = new Explosion(Match, Explosion.Type.CENTER, new Vector(), Match.GetScreenCoordinates(cell), Match.CellSize);

            var direction = new CellCoordinates(-1, 0);
            for (int i = 0; i < 4; i++)
            {
                direction = direction.RotatedRight;

                for (int r = 1; r <= Constants.Bomb.REACH; r++)
                {
                    var explosionCell = cell + direction * r;
                    var cellContent = Match[explosionCell];

                    if (cellContent == null)
                    {
                        Match[explosionCell] = new Explosion(Match, getExplosionType(r), new Vector(direction.Column, direction.Row), Match.GetScreenCoordinates(explosionCell), Match.CellSize);
                    }
                    else
                    {
                        if (cellContent.Destroyable)
                        {
                            cellContent.Destroy();
                            Match[explosionCell] = new Explosion(Match, Explosion.Type.END, new Vector(direction.Column, direction.Row), Match.GetScreenCoordinates(explosionCell), Match.CellSize);
                        }
                        break;
                    }
                }
            }
        }

        private Explosion.Type getExplosionType(int r)
        {
            if (r == Constants.Bomb.REACH) return Explosion.Type.END;
            else return Explosion.Type.MIDDLE;
        }
    }
}
