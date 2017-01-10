using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames
{
    /// <summary>
    /// Represents a 2D rotation.
    /// </summary>
    public struct Rotation
    {
        /// <summary>
        /// A rotation by 90 degrees.
        /// </summary>
        public static readonly Rotation Quarter = new Rotation(-1, 1);

        /// <summary>
        /// A rotation by -90 degrees.
        /// </summary>
        public static readonly Rotation InverseQuarter = new Rotation(-1, -1);

        /// <summary>
        /// A rotation by 180 degrees.
        /// </summary>
        public static readonly Rotation Half = new Rotation(-2, 0);

        // Store the rotation as a complex number (r, i).
        // Instead of r, store r - 1 so that default(Rotation) is the null rotation (1, 0).
        private double _rMinusOne;

        private double _i;

        private Rotation(double rMinusOne, double i)
        {
            _rMinusOne = rMinusOne;
            _i = i;
        }

        /// <summary>
        /// Gets the reverse rotation.
        /// </summary>
        public Rotation Inverse => new Rotation(_rMinusOne, -_i);

        private double R => _rMinusOne + 1;

        private double I => _i;

        public static Rotation operator *(Rotation a, Rotation b)
        {
            var r = a.R * b.R - a.I * b.I;
            var i = a.R * b.I + a.I * b.R;

            /* Renormalize the vector to prevent accumulating errors. */
            var n = 1 / Math.Sqrt(r * r + i * i);

            return new Rotation(r * n - 1, i * n);
        }

        public static Vector operator *(Rotation a, Vector v)
        {
            return new Vector(a.R * v.X - a.I * v.Y, a.R * v.Y + a.I * v.X);
        }

        /// <summary>
        /// Rotate by the specified number of degrees.
        /// </summary>
        /// <param name="angle">The rotation angle in degrees.</param>
        /// <returns>The rotation.</returns>
        public static Rotation FromDegrees(double angle)
        {
            return FromRadians(angle * (Math.PI / 180));
        }

        /// <summary>
        /// Rotate by the specified number of radians.
        /// </summary>
        /// <param name="angle">The rotation angle in radians.</param>
        /// <returns>The rotation.</returns>
        public static Rotation FromRadians(double angle)
        {
            return new Rotation(Math.Cos(angle) - 1, Math.Sin(angle));
        }
    }
}
