using System;
using System.IO;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public class Matrix : IEquatable<Matrix>
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

        private Matrix(double[,] items, bool clone)
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
            var result = new StringBuilder();
            result.Append("[");
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result.Append(m[i, j]);
                    if (j != Columns - 1)
                    {
                        result.Append(", ");
                    }
                }
                if (i != Rows - 1)
                {
                    result.AppendLine();
                }
            }
            result.Append("]");
            return result.ToString();
        }

        public override int GetHashCode()
        {
            return cachedHash.Value;
        }

        public override bool Equals(object obj)
        {
            var m = obj as Matrix;
            return (m != null) && Equals(m);
        }

        public bool Equals(Matrix other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (Columns != other.Columns)
            {
                return false;
            }
            if (Rows != other.Rows)
            {
                return false;
            }
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (!Constants.EpsilonCompare(this[i, j], other[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
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
            var result = new Matrix(Rows, Columns);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    result.m[i, j] = m[i, j];
                }
            }
            return result;
        }

        public double Determinant(MatrixOperation operationType)
        {
            if (Rows != Columns)
            {
                throw new ArgumentException("Undetermined");
            }
            switch (operationType)
            {
                case MatrixOperation.Cofactor:
                    return DeterminantWithCofactors();
                case MatrixOperation.Gauss:
                    return DeterminantWithGaussEliminitation();
                default:
                    throw new NotSupportedException();
            }
        }

        private double DeterminantForSmallValues()
        {
            if (Rows == 1)
            {
                return m[0, 0];
            }
            if (Rows == 2)
            {
                return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
            }
            if (Rows == 3)
            {
                return m[0, 0] * m[1, 1] * m[2, 2] +
                       m[0, 1] * m[1, 2] * m[2, 0] +
                       m[0, 2] * m[1, 0] * m[2, 1] -
                       m[0, 2] * m[1, 1] * m[2, 0] -
                       m[0, 1] * m[1, 0] * m[2, 2] -
                       m[0, 0] * m[1, 2] * m[2, 1];
            }
            throw new NotSupportedException();
        }

        private double DeterminantWithCofactors()
        {
            if (Rows != Columns)
            {
                throw new ArgumentException("Undetermined");
            }
            if (Rows < 4)
            {
                return DeterminantForSmallValues();
            }
            double result = 0.0;
            for (int i = 0; i < Columns; ++i)
            {
                if (m[0, i] != 0)
                {
                    result += m[0, i] * Cofactor(this, MatrixOperation.Cofactor, 0, i);
                }
            }
            return result;
        }

        private double DeterminantWithGaussEliminitation()
        {
            if (Rows != Columns)
            {
                throw new ArgumentException("Undetermined");
            }
            if (Rows < 4)
            {
                return DeterminantForSmallValues();
            }
            var diagonal = GaussEliminitation(this, _ => _, _ => _, true);
            var result = 1.0;
            for (int i = 0; i < Rows; i++)
            {
                result *= diagonal[i, i];
            }
            return result;
        }
        #endregion

        #region Static Methods
        public static Matrix FromArray(int rows, int columns, double[] values)
        {
            if (rows * columns != values.Length)
            {
                throw new ArgumentException("Incompatible array size");
            }
            var result = new Matrix(rows, columns);
            int v = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result.m[i, j] = values[v++];
                }
            }
            return result;
        }

        public static Matrix Zero(int rank)
        {
            return new Matrix(rank);
        }

        public static Matrix Identity(int rank)
        {
            var result = new Matrix(rank);
            for (int i = 0; i < rank; ++i)
            {
                result.m[i, i] = 1.0;
            }
            return result;
        }

        public static Matrix Transpose(Matrix a)
        {
            var result = new Matrix(a.Columns, a.Rows);
            for (int i = 0; i < a.Columns; i++)
            {
                for (int j = 0; j < a.Rows; j++)
                {
                    result.m[i, j] = a[j, i];
                }
            }
            return result;
        }

        public static Matrix Subtract(Matrix a, Matrix b)
        {
            if ((a.Columns != b.Columns) || (a.Rows != b.Rows))
            {
                throw new ArgumentException();
            }
            var result = new Matrix(a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result.m[i, j] = a[i, j] - b[i, j];
                }
            }
            return result;
        }

        public static Matrix Multiply(Matrix a, Matrix b)
        {
            if (a == null)
            {
                throw new ArgumentNullException(nameof(a));
            }
            if (b == null)
            {
                throw new ArgumentNullException(nameof(b));
            }
            if (a.Columns != b.Rows)
            {
                throw new ArgumentException($"Incompatible matricies {a.Columns} != {b.Rows}");
            }

            var result = new Matrix(a.Rows, b.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Columns; j++)
                {
                    double cij = MultiplyRowAndColumn(i, j, a, b);
                    result.m[i, j] = cij;
                }
            }
            return result;
        }

        public static Matrix Power(Matrix m, uint pow)
        {
            if (m.Rows != m.Columns)
            {
                throw new ArgumentException();
            }

            if (pow == 0)
            {
                return Identity(m.Rows);
            }
            if (pow == 1)
            {
               return m.Clone();
            }

            var x = m;
            var result = Identity(m.Rows);
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                {
                    result = Multiply(result, x);
                }
                x = Multiply(x, x);
                pow >>= 1;
            }
            return result;
        }

        public static Matrix GaussEliminitation(Matrix a, Func<int, int> rowFunction, Func<int, int> columnFunction, bool clone)
        {
            if (!clone)
            {
                a.EnsureNotHashed();
            }
            var result = clone ? a.Clone() : a;
            for (int i = 0; i < result.Rows - 1; i++)
            {
                for (int r = i + 1; r < result.Rows; r++)
                {
                    var rr = rowFunction(r);
                    var ii = rowFunction(i);
                    double d = result.m[rr, ii] / result.m[ii, ii];
                    for (int j = 0; j < result.Columns; j++)
                    {
                        var jj = columnFunction(j);
                        result.m[rr, jj] = result.m[rr, jj] - d * result.m[ii, jj];
                    }
                }
            }
            return result;
        }

        public static Matrix Minor(Matrix a, int r, int c)
        {
            var result = new Matrix(a.Rows - 1, a.Columns - 1);
            var ii = 0;
            var jj = 0;
            for (int i = 0; i < a.Rows; i++)
            {
                if (i == r)
                {
                    continue;
                }
                jj = 0;
                for (int j = 0; j < a.Columns; j++)
                {
                    if (j == c)
                    {
                        continue;
                    }
                    result.m[ii, jj] = a.m[i, j];
                    jj++;
                }
                ii++;
            }
            return result;
        }

        public static Matrix Invert(Matrix a)
        {
            return Invert(a, MatrixOperation.Cofactor);
        }

        public static Matrix Invert(Matrix a, MatrixOperation operationType)
        {
            switch (operationType)
            {
                case MatrixOperation.Cofactor:
                    return InvertCofactor(a);
                case MatrixOperation.Gauss:
                    return InvertGauss(a);
                default:
                    throw new NotSupportedException();
            }
        }

        public static Matrix InvertGauss(Matrix a)
        {
            var temp = new Matrix(a.Rows, a.Columns + a.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    temp.m[i, j] = a.m[i, j];
                }
                temp.m[i, i + a.Columns] = 1.0;
            }
            GaussEliminitation(temp, i => i, j => j, false);
            GaussEliminitation(temp, i => temp.Rows - 1 - i, j => temp.Columns - 1 - j, false);
            for (int i = 0; i < temp.Rows; i++)
            {
                double d = temp.m[i, i];
                for (int j = 0; j < temp.Columns; j++)
                {
                    temp.m[i, j] = temp.m[i, j] / d;
                }
            }
            var result = new Matrix(a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result.m[i, j] = temp.m[i, a.Columns + j];
                }
            }
            return result;
        }

        public static Matrix InvertCofactor(Matrix a)
        {
            var det = a.Determinant(MatrixOperation.Cofactor);
            if (Constants.EpsilonCompare(det, 0.0))
            {
                throw new ArgumentException("Zero determinant");
            }
            var result = new Matrix(a.Rows, a.Columns);
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    double c = Cofactor(a, MatrixOperation.Cofactor, i, j);
                    result.m[j, i] = c / det;
                }
            }
            return result;
        }

        public static double Cofactor(Matrix a, MatrixOperation operationType, int r, int c)
        {
            double minorDeterminant = Minor(a, r, c).Determinant(operationType);
            if (((r + c) & 1) == 1)
            {
                return -minorDeterminant;
            }
            else
            {
                return minorDeterminant;
            }
        }

        private static double MultiplyRowAndColumn(int row, int column, Matrix a, Matrix b)
        {
            var result = 0.0;
            for (int i = 0; i < a.Columns; i++)
            {
                result += a[row, i] * b[i, column];
            }
            return result;
        }

        public static class Geometry3D
        {
            public static Matrix Translation(double x, double y, double z)
            {
                return new Matrix(new double[,]
                {
                    { 1.0, 0.0, 0.0,  x  },
                    { 0.0, 1.0, 0.0,  y  },
                    { 0.0, 0.0, 1.0,  z  },
                    { 0.0, 0.0, 0.0, 1.0 }
                }, false);
            }

            public static Matrix Scale(double x, double y, double z)
            {
                return new Matrix(new double[,]
                {
                    {  x , 0.0, 0.0, 0.0 },
                    { 0.0,  y , 0.0, 0.0 },
                    { 0.0, 0.0,  z , 0.0 },
                    { 0.0, 0.0, 0.0, 1.0 }
                }, false);
            }

            public static Matrix RotateX(double r)
            {
                var s = Math.Sin(r);
                var c = Math.Cos(r);

                return new Matrix(new double[,]
                {
                    { 1.0, 0.0, 0.0, 0.0 },
                    { 0.0,  c , -s , 0.0 },
                    { 0.0,  s ,  c , 0.0 },
                    { 0.0, 0.0, 0.0, 1.0 }
                }, false);
            }

            public static Matrix RotateY(double r)
            {
                var s = Math.Sin(r);
                var c = Math.Cos(r);

                return new Matrix(new double[,]
                {
                    {  c , 0.0,  s , 0.0 },
                    { 0.0, 1.0, 0.0, 0.0 },
                    { -s , 0.0,  c , 0.0 },
                    { 0.0, 0.0, 0.0, 1.0 }
                }, false);
            }

            public static Matrix RotateZ(double r)
            {
                var s = Math.Sin(r);
                var c = Math.Cos(r);

                return new Matrix(new double[,]
                {
                    {  c , -s , 0.0, 0.0 },
                    {  s ,  c , 0.0, 0.0 },
                    { 0.0, 0.0, 1.0, 0.0 },
                    { 0.0, 0.0, 0.0, 1.0 }
                }, false);
            }

            public static Matrix Shearing(double xy, double xz, double yx, double yz, double zx, double zy)
            {
                return new Matrix(new double[,]
                {
                    { 1.0 , xy,  xz, 0.0 },
                    {  yx, 1.0,  yz, 0.0 },
                    {  zx,  zy, 1.0, 0.0 },
                    { 0.0, 0.0, 0.0, 1.0 }
                }, false);
            }

            public static Matrix Vector(double x, double y, double z)
            {
                return new Matrix(new double[,]
                {
                    {  x  },
                    {  y  },
                    {  z  },
                    { 0.0 }
                }, false);
            }

            public static Matrix Point(double x, double y, double z)
            {
                return new Matrix(new double[,]
                {
                    {  x  },
                    {  y  },
                    {  z  },
                    { 1.0 }
                }, false);
            }

            public static Tuple4 ToTuple(Matrix a)
            {
                if (a.Rows == 4 && a.Columns == 1)
                {
                    return new Tuple4(a[0, 0], a[1, 0], a[2, 0], a[3, 0]);
                }

                if (a.Rows == 1 && a.Columns == 4)
                {
                    return new Tuple4(a[0, 1], a[0, 1], a[0, 2], a[0, 3]);
                }

                throw new ArgumentException();
            }

            public static Matrix ViewTransform(Tuple4 from, Tuple4 to, Tuple4 up)
            {
                var forward = Tuple4.Normalize(Tuple4.Subtract(to, from));
                var upNormalized = Tuple4.Normalize(up);
                var left = Tuple4.CrossProduct(forward, upNormalized);
                var trueUp = Tuple4.CrossProduct(left, forward);

                var orientation = new Matrix(new double[,]
                {
                    {      left.X,     left.Y,     left.Z, 0.0 },
                    {    trueUp.X,   trueUp.Y,   trueUp.Z, 0.0 },
                    {  -forward.X, -forward.Y, -forward.Z, 0.0 },
                    {         0.0,        0.0,        0.0, 1.0 }
                }, false);

                return Matrix.Multiply(orientation, Translation(-from.X, -from.Y, -from.Z));
            }
        }
        #endregion
    }

    public enum MatrixOperation
    {
        Gauss,
        Cofactor
    }

    public class MatrixProgram
    {
        public static void AcceptanceTest(TextWriter output)
        {
            {
                var a = Matrix.FromArray(4, 4, new double[]{
                    1, 2, 3, 4,
                    5, 6, 7, 8,
                    9, 8, 7, 6,
                    5, 4, 3, 2
                });

                var b = Matrix.FromArray(4, 4, new double[]{
                    -2, 1, 2, 3,
                    3, 2, 1, -1,
                    4, 3, 6, 5,
                    1, 2, 7, 8
                });

                var c = Matrix.Multiply(a, b);

                var expected = Matrix.FromArray(4, 4, new double[]{
                    20, 22, 50, 48,
                    44, 54, 114, 108,
                    40, 58, 110, 102,
                    16, 26, 46, 42
                });

                if (!expected.Equals(c))
                {
                    throw new Exception();
                }

                output.WriteLine(c);
            }

            {
                var a = Matrix.FromArray(4, 4, new double[]{
                    1, 2, 3, 4,
                    2, 4, 4, 2,
                    8, 6, 4, 1,
                    0, 0, 0, 1
                });

                var b = Matrix.FromArray(4, 1, new double[]{
                    1, 2, 3, 1
                });

                var c = Matrix.Multiply(a, b);

                var expected = Matrix.FromArray(4, 1, new double[]{
                    18,
                    24,
                    33,
                    1
                });

                if (!expected.Equals(c))
                {
                    throw new Exception();
                }

                output.WriteLine(c);
            }

            {
                var a = Matrix.FromArray(2, 2, new double[]{
                    1, 5,
                    -3, 2,
                });

                if (!Constants.EpsilonCompare(17, a.Determinant(MatrixOperation.Cofactor)))
                {
                    throw new Exception();
                }

                output.WriteLine(a.Determinant(MatrixOperation.Cofactor));
            }

            {
                var a = Matrix.FromArray(3, 3, new double[]{
                    1, 2, 6,
                    -5, 8, -4,
                    2, 6, 4
                });

                if (!Constants.EpsilonCompare(-196, a.Determinant(MatrixOperation.Cofactor)))
                {
                    throw new Exception();
                }
                if (!Constants.EpsilonCompare(-196, a.Determinant(MatrixOperation.Gauss)))
                {
                    throw new Exception();
                }

                output.WriteLine(a.Determinant(MatrixOperation.Cofactor));
            }

            {
                var a = Matrix.FromArray(4, 4, new double[]{
                    -2, -8, 3, 5,
                    -3, 1, 7, 3,
                    1, 2, -9, 6,
                    -6, 7, 7, -9
                });

                if (!Constants.EpsilonCompare(-4071, a.Determinant(MatrixOperation.Gauss)))
                {
                    throw new Exception();
                }
                if (!Constants.EpsilonCompare(-4071, a.Determinant(MatrixOperation.Cofactor)))
                {
                    throw new Exception();
                }

                output.WriteLine(a.Determinant(MatrixOperation.Cofactor));
            }

            {
                var a = Matrix.FromArray(5, 5, new double[]{
                    -2, -8, 3, 5, 0,
                    -3, 1, 7, 3, 0,
                    1, 2, -9, 6, 0,
                    -6, 7, 7, -9, 0,
                    1, 4, 5, 6, 7
                });

                if (!Constants.EpsilonCompare(-28497, a.Determinant(MatrixOperation.Gauss)))
                {
                    throw new Exception();
                }
                if (!Constants.EpsilonCompare(-28497, a.Determinant(MatrixOperation.Cofactor)))
                {
                    throw new Exception();
                }

                output.WriteLine(a.Determinant(MatrixOperation.Cofactor));
            }

            {
                var a = Matrix.FromArray(4, 4, new double[]{
                    -2, -8, 3, 5,
                    -3, 1, 7, 3,
                    1, 2, -9, 6,
                    -6, 7, 7, -9
                });
                var a11 = Matrix.Invert(a, MatrixOperation.Gauss);
                var a12 = Matrix.Invert(a, MatrixOperation.Cofactor);

                var r1 = Matrix.Multiply(a, a11);
                var r2 = Matrix.Multiply(a, a12);

                var i = Matrix.Identity(4);

                if (!a11.Equals(a12))
                {
                    throw new Exception();
                }
                if (!i.Equals(r1))
                {
                    throw new Exception();
                }
                if (!i.Equals(r2))
                {
                    throw new Exception();
                }

                output.WriteLine(a11);
            }

            {
                var a = Matrix.FromArray(4, 4, new double[]{
                    -2, -8, 3, 5,
                    -3, 1, 7, 3,
                    1, 2, -9, 6,
                    -6, 7, 7, -9
                });

                // NOTE: For this input starts to fail when p = 15
                for (int p=0; p<15; ++p)
                {
                    var b = Matrix.Identity(4);
                    for (int i=0; i<p; ++i)
                    {
                        b = Matrix.Multiply(b, a);
                    }

                    var c = Matrix.Power(a, (uint)p);

                    if (!c.Equals(b))
                    {
                        throw new Exception();
                    }
                }
            }

        }
    }
}
