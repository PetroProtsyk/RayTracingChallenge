using System;
using System.IO;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public class TransposeMatrix : IMatrix
    {
        private readonly IMatrix original;

        public int Columns => original.Rows;

        public int Rows => original.Columns;

        public double this[int row, int column] => original[column, row];

        public TransposeMatrix(IMatrix original)
        {
            this.original = original;
        }

        public override string ToString()
        {
            return MatrixOperations.ToString(this);
        }

        public override int GetHashCode()
        {
            return System.HashCode.Combine(original.GetHashCode(), 1);
        }

        public override bool Equals(object obj)
        {
            var m = obj as IMatrix;
            return (m != null) && Equals(m);
        }

        public bool Equals(IMatrix other)
        {
            return MatrixOperations.EpsilonEquals(this, other);
        }
    }
}
