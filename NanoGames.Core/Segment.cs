// Copyright (c) the authors of nanoGames. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt in the project root.


namespace NanoGames
{
    /// <summary>
    /// A 2D segment.
    /// </summary>
    public class Segment
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

            // check for start = stop
            Start = start;
            End = stop;
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
        /// Gets the directional vector of the segment, pointing from start to stop.
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
        /// Gets a segment rotated counterclockwise around origin.
        /// </summary>
        /// <param name="angle">The angle of the rotation.</param>
        /// <returns>"Returns rotated Segment"</returns>
        public Segment Rotated(double angle) =>
            new Segment(Start.Rotated(angle), End.Rotated(angle));

        /// <summary>
        /// Gets a translated segment.
        /// </summary>
        /// <param name="t">The translation vector.</param>
        /// <returns>"Returns translated Segment"</returns>
        public Segment Translated(Vector t) => new Segment(Start + t, End + t);

        /// <summary>
        /// Gets a scaled segment.
        /// </summary>
        /// <param name="s">The scaling factor.</param>
        /// <returns>"Returns the scaled Segment"</returns>
        public Segment Scaled(double s) => new Segment(s * Start, s * End);

        /// <summary>
        /// Gets a to the origin translated segment.
        /// </summary>
        /// <returns>"Returns the translated Segment"</returns>
        public Segment ToOrigin() => Translated(new Vector(160, 100));
    }
}