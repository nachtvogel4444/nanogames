// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.

using System;
using System.Collections.Generic;

namespace NanoGames.Games.Banana
{
    /// <summary>
    /// A 2D segment.
    /// </summary>
    internal class Segment
    {
        /// <summary>
        /// The postion vector to the startpoint.
        /// </summary>
        public Vector Start;

        /// <summary>
        /// The postion vector to the endpoint.
        /// </summary>
        public Vector End;

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class.
        /// </summary>
        /// <param name="start">The postion vector to the startpoint.</param>
        /// <param name="stop">The postion vector to the endpoint.</param>
        public Segment(Vector start, Vector stop)
        {
            if (start == stop)
            {
                throw new System.ArgumentException("Segment has no length!");
            }

            else
            {
                Start = start;
                End = stop;
            }
        }

        /// <summary>
        /// Gets the squared length of the segment.
        /// </summary>
        public double SquaredLength => (Start - End).SquaredLength;

        /// <summary>
        /// Gets the length of the segement.
        /// </summary>
        public double Length => (End - Start).Length;

        /// <summary>
        /// Gets the directional vector of the segment, pointing from start to end.
        /// </summary>
        public Vector DirectionalVector => End - Start;

        /// <summary>
        /// Gets the midpoint of the segment.
        /// </summary>
        public Vector MidPoint => Start + 0.5 * DirectionalVector;
        
        /// <summary>
        /// Gets the normal unit vector to the segment, counterclockwise.
        /// </summary>
        public Vector Normal => new Vector(DirectionalVector.Y, -DirectionalVector.X).Normalized;

        /// <summary>
        /// Gets the angle from (1,0) to segment, counterclockwise.
        /// </summary>
        public double AngleNormal => Math.Atan2(Normal.Y, Normal.X);

        /// <summary>
        /// States if segment is orientated like a slash without regarding its orientation.
        /// </summary>
        public bool IsSlash => (
            ((Start.X < End.X) && (Start.Y > End.Y)) ||
            ((Start.X > End.X) && (Start.Y < End.Y))
            );

        /// <summary>
        /// Gives list of integers of the bounding box of the segment [x1, x2, y1, y2].
        /// </summary>
        public List<int> BBoxInt => GetBBoxInt();

        /// <summary>
        /// Gets boundig box with int values, so that the resulting bbox is always bigger.
        /// </summary>
        public List<int> GetBBoxInt()
        {
            List<int> list = new List<int> { 0, 0, 0, 0 };

            if (Start.X < End.X)
            {
                list[0] = (int)Start.X;
                list[1] = (int)End.X + 1;
            }
            else
            {
                list[0] = (int)End.X;
                list[1] = (int)Start.X + 1;
            }

            if (Start.Y < End.Y)
            {
                list[2] = (int)Start.Y;
                list[3] = (int)End.Y + 1;
            }
            else
            {
                list[2] = (int)End.Y;
                list[3] = (int)Start.Y + 1;
            }

            return list;
        }

        /// <summary>
        /// Rotates the segment counterclockwise around a given point.
        /// </summary>
        /// <param name="origin">The origin of the rotation.</param>
        /// <param name="angle">The angle of the rotation.</param>
        public void Rotate(double angle, Vector origin)
        {
            //Start = Start.RotateAngle(angle, origin);
            //End = End.RotateAngle(angle, origin);
        }

        /// <summary>
        /// Draws the segment.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="c">Color.</param>
        public void Draw(IGraphics g, Color c)
        {
            g.Line(c, Start, End);
        }

        /// <summary>
        /// Draws the segment plus debug info.
        /// </summary>
        /// <param name="g">Graphics object.</param>
        /// <param name="c">Color.</param>
        public void DrawDebug(IGraphics g, Color c)
        {
            g.Line(c, Start, End);
            g.Circle(c, Start, 1);
            g.Line(c, MidPoint, MidPoint + Normal);
        }

    }
}
