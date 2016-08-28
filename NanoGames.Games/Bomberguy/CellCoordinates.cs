using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoGames.Games.Bomberguy
{
    internal class CellCoordinates
    {
        private Vector _vector;

        public int Row
        {
            get { return (int)_vector.X; }
        }
        public int Column
        {
            get { return (int)_vector.Y; }
        }

        public CellCoordinates( int row, int column) : this(new Vector(row, column)) { }

        private CellCoordinates(Vector v)
        {
            _vector = v;
        }

        public CellCoordinates RotatedRight => new CellCoordinates(_vector.RotatedRight);

        public static CellCoordinates operator +(CellCoordinates a, CellCoordinates b) => new CellCoordinates(a._vector + b._vector);

        public static CellCoordinates operator -(CellCoordinates a, CellCoordinates b) => new CellCoordinates(a._vector - b._vector);

        public static CellCoordinates operator *(CellCoordinates c, int factor) => new CellCoordinates(c._vector * factor);

        public static CellCoordinates operator *(int factor, CellCoordinates c) => c * factor;

    }
}
