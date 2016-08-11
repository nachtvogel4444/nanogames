// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;

namespace NanoGames.Games.Snake
{
    internal sealed class SegmentBuffer
    {
        /* Implements a ring buffer of segments. */

        private int _segmentOffset;
        private Position[] _segments;

        public int Count
        {
            get
            {
                if (_segments == null)
                {
                    return 0;
                }
                else
                {
                    return _segments.Length;
                }
            }
        }

        public Position this[int index]
        {
            get
            {
                return _segments[(index + _segmentOffset) % _segments.Length];
            }
        }

        public void Add(Position position)
        {
            if (_segments == null)
            {
                _segments = new Position[1];
            }
            else
            {
                var newSegments = new Position[_segments.Length + 1];
                Array.Copy(_segments, _segmentOffset, newSegments, 1, _segments.Length - _segmentOffset);
                Array.Copy(_segments, 0, newSegments, 1 + _segments.Length - _segmentOffset, _segmentOffset);
                _segments = newSegments;
            }

            _segmentOffset = 0;
            _segments[0] = position;
        }

        public void Rotate(Position position)
        {
            _segmentOffset = (_segmentOffset + _segments.Length - 1) % _segments.Length;
            _segments[_segmentOffset] = position;
        }
    }
}
