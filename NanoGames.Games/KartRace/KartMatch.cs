using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.KartRace
{
    class KartMatch : Match<KartPlayer>
    {
        public readonly List<TrackLine> TrackLines = new List<TrackLine>();

        protected override void Initialize()
        {
            TrackLines.Add(new TrackLine(new Vector(10, -10), new Vector(10, 10)));

            for (int i = 0; i < Players.Count; ++i)
            {
                Players[i].Position = i * new Vector(0, 1);
            }
        }

        protected override void Update()
        {
            foreach (var player in Players)
            {
                if (player.Input.Left.IsPressed)
                {
                    player.Direction *= Constants.TurnSpeed;
                }

                if (player.Input.Right.IsPressed)
                {
                    player.Direction *= Constants.TurnSpeed.Inverse;
                }

                if (player.Input.Up.IsPressed)
                {
                    player.Speed += Constants.Acceleration;
                }

                if (player.Input.Down.IsPressed)
                {
                    player.Speed -= Constants.Acceleration;
                }

                if (Math.Abs(player.Speed) > Constants.MaxSpeed)
                {
                    player.Speed = Math.Sign(player.Speed) * Constants.MaxSpeed;
                }

                player.Position += player.Speed * (player.Direction * new Vector(1, 0));
            }

            foreach (var player in Players)
            {
                player.Render();
            }
        }
    }
}
