// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.NanoSoccer
{
    internal class FieldLine
    {
        private const double _dashLength = 5;

        private static readonly Color _color = new Color(0.05, 0.05, 0.05);

        private readonly double _dashes;

        public FieldLine(Vector start, Vector end)
        {
            Start = start;
            End = end;

            _dashes = Math.Round(((end - start).Length / _dashLength - 1) * 0.5) * 2 + 1;
        }

        public Vector Start { get; }

        public Vector End { get; }

        public void Draw(Graphics g)
        {
            for (double i = 1; i < _dashes; i += 2)
            {
                var m1 = i / _dashes;
                var m2 = (i + 1) / _dashes;

                g.Line(_color, m1 * Start + (1 - m1) * End, m2 * Start + (1 - m2) * End);
            }
        }
    }
}
