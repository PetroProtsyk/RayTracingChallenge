using System;
using System.IO;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public class Matrix4x4 : IMatrix
    {
        #region Fields
        private readonly double a11;
        private readonly double a12;
        private readonly double a13;
        private readonly double a14;

        private readonly double a21;
        private readonly double a22;
        private readonly double a23;
        private readonly double a24;

        private readonly double a31;
        private readonly double a32;
        private readonly double a33;
        private readonly double a34;

        private readonly double a41;
        private readonly double a42;
        private readonly double a43;
        private readonly double a44;
        #endregion

        #region Contructor
        public Matrix4x4(double[] values)
        {
            if (values.Length != 16)
            {
                throw new ArgumentException("Incompatible array size");
            }

            a11 = values[00]; a12 = values[01]; a13 = values[02]; a14 = values[03];
            a21 = values[04]; a22 = values[05]; a23 = values[06]; a24 = values[07];
            a31 = values[08]; a32 = values[09]; a33 = values[10]; a34 = values[11];
            a41 = values[12]; a42 = values[13]; a43 = values[14]; a44 = values[15];
        }
        #endregion

        #region IMatrix
        public int Columns => 4;

        public int Rows => 4;

        public double this[int row, int column] => GetValue(row, column);
        #endregion

        #region IEquatable<IMatrix>
        public override bool Equals(object obj)
        {
            var m = obj as IMatrix;
            return (m != null) && Equals(m);
        }

        public bool Equals(IMatrix other)
        {
            return MatrixOperations.EpsilonEquals(this, other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                HashCode.Combine(a11, a12, a13, a14),
                HashCode.Combine(a21, a22, a23, a24),
                HashCode.Combine(a31, a32, a33, a34),
                HashCode.Combine(a41, a42, a43, a44)
            );
        }
        #endregion

        #region Methods
        public override string ToString()
        {
            return MatrixOperations.ToString(this);
        }

        private double GetValue(int row, int column)
        {
            switch(row)
            {
                case 0:
                    return GetByIndex(column, a11, a12, a13, a14);
                case 1:
                    return GetByIndex(column, a21, a22, a23, a24);
                case 2:
                    return GetByIndex(column, a31, a32, a33, a34);
                case 3:
                    return GetByIndex(column, a41, a42, a43, a44);
                default:
                    throw new ArgumentOutOfRangeException(nameof(row));
            }
        }

        private static double GetByIndex(int index, double t0, double t1, double t2, double t3)
        {
            switch(index)
            {
                case 0:
                    return t0;
                case 1:
                    return t1;
                case 2:
                    return t2;
                case 3:
                    return t3;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }
        #endregion

        #region Static Methods
        public static Matrix4x4 FromArray(int rows, int columns, double[] values)
        {
            if (rows != 4 || columns != 4)
            {
                throw new ArgumentException("Incompatible array size");
            }
            if (rows * columns != values.Length)
            {
                throw new ArgumentException("Incompatible array size");
            }
            return new Matrix4x4(values);
        }
        #endregion
    }
}
