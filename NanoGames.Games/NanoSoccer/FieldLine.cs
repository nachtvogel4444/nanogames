// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.NanoSoccer
{
    internal class FieldLine
    {
        private const double _dashLength = 4;

        private static readonly Color _color = new Color(0.25, 0.25, 0.25);

        private readonly double _dashes;

        public FieldLine(Vector start, Vector end)
        {
            Start = start;
            End = end;

            _dashes = Math.Round((end - start).Length / _dashLength * 0.5) * 2;
        }

        public Vector Start { get; }

        public Vector End { get; }

        public void Draw(Graphics g)
        {
            for (double i = 0.5; i < _dashes; i += 2)
            {
                var m1 = i / _dashes;
                var m2 = (i + 1) / _dashes;

                g.Line(_color, m1 * Start + (1 - m1) * End, m2 * Start + (1 - m2) * End);
            }
        }
    }
}
