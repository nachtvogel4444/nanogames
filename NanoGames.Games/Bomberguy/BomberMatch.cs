// Copyright (c) the authors of nanoGames. All rights reserved.

// Licensed under the MIT license. See LICENSE.txt in the project root.

namespace NanoGames.Games.Bomberguy
{
    internal class BomberMatch : Match<BomberGuy>
    {
        public const int FIELD_SIZE = 15;

        private double _pixelsPerUnit;
        private double _widthOffset;
        private BomberThing[,] _field = new BomberThing[FIELD_SIZE, FIELD_SIZE];

        protected override void Initialize()
        {
            _pixelsPerUnit = Graphics.Height / FIELD_SIZE;
            _widthOffset = (Graphics.Width - Graphics.Height) / 2d;

            // initialize static obstacles
            for (int r = 0; r < FIELD_SIZE; r++)
            {
                for (int c = 0; c < FIELD_SIZE; c++)
                {
                    if (r == 0 || r == FIELD_SIZE - 1 || c == 0 || c == FIELD_SIZE - 1 || r % 2 == 0 && c % 2 == 0)
                    {
                        _field[r, c] = new Unbombable(new Vector(_widthOffset + c * _pixelsPerUnit, r * _pixelsPerUnit), new Vector(_pixelsPerUnit, _pixelsPerUnit));
                    }
                }
            }

            // initialize players
            double distance = FIELD_SIZE - 2;
            int count = 0;
            Vector pos = new Vector(_widthOffset + _pixelsPerUnit, _pixelsPerUnit);
            Vector direction = new Vector(1, 0);
            foreach (BomberGuy p in this.Players)
            {
                p.Size = new Vector(_pixelsPerUnit, _pixelsPerUnit);
                p.Position = pos;

                direction = direction.RotatedRight;

                pos = pos + direction * (distance * _pixelsPerUnit);

                count++;
                if (count % 4 == 0)
                    distance = System.Math.Floor(distance / 2d);
            }
        }

        protected override void Update()
        {
            foreach (Player p in this.Players)
            {
                for (int r = 0; r < FIELD_SIZE; r++)
                {
                    for (int c = 0; c < FIELD_SIZE; c++)
                    {
                        BomberThing thing = _field[r, c];

                        if (thing != null)
                            thing.Draw(p.Graphics);
                    }
                }
            }
        }
    }
}
