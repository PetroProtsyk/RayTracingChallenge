using System;
using System.IO;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public class Matrix : IMatrix
    {
        #region Fields
        private readonly double[,] m;

        private Lazy<int> cachedHash;
        #endregion

        #region Properties
        public int Columns => m.GetLength(1);

        public int Rows => m.GetLength(0);

        public double this[int row, int column]
        {
            get => m[row, column];
        }
        #endregion

        #region Constructors
        public Matrix(int rank)
            : this(rank, rank)
        {
        }

        public Matrix(int rows, int columns)
        {
            this.m = new double[rows, columns];
            this.cachedHash = new Lazy<int>(CalculateHash, true);
        }

        public Matrix(double[,] items)
            : this(items, true)
        {
        }

        protected internal Matrix(double[,] items, bool clone)
        {
            if (clone)
            {
                this.m = (double[,])items.Clone();
            }
            else
            {
                this.m = items;
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return MatrixOperations.ToString(this);
        }

        public override int GetHashCode()
        {
            return cachedHash.Value;
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

        private int CalculateHash()
        {
            int hash = 0;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    hash = System.HashCode.Combine(hash, m[i, j]);
                }
            }
            return hash;
        }

        private void EnsureNotHashed()
        {
            if (cachedHash.IsValueCreated)
            {
                throw new InvalidOperationException("Hash exists");
            }
        }
        #endregion

        #region Methods
        public Matrix Clone()
        {
            return MatrixOperations.CopyFrom(this);
        }
        #endregion
    }

    public enum MatrixOperation
    {
        Gauss,
        Cofactor
    }
}
