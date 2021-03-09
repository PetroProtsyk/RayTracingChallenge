using System;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public static class MatrixOperations
    {
        public static bool EpsilonEquals(IMatrix a, IMatrix b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            if (a.Columns != b.Columns)
            {
                return false;
            }
            if (a.Rows != b.Rows)
            {
                return false;
            }
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    if (!Constants.EpsilonCompare(a[i, j], b[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string ToString(IMatrix a)
        {
            var result = new StringBuilder();
            result.Append("[");
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result.Append(a[i, j]);
                    if (j != a.Columns - 1)
                    {
                        result.Append(", ");
                    }
                }
                if (i != a.Rows - 1)
                {
                    result.AppendLine();
                }
            }
            result.Append("]");
            return result.ToString();
        }

        public static double Determinant(IMatrix m)
        {
            if (m.Rows < 10 && m.Columns < 10)
            {
                return Determinant(m, MatrixOperation.Cofactor);
            }
            else
            {
                return Determinant(m, MatrixOperation.Gauss);
            }
        }

        public static double Determinant(IMatrix m, MatrixOperation operationType)
        {
            if (m.Rows != m.Columns)
            {
                throw new ArgumentException("Undetermined");
            }
            switch (operationType)
            {
                case MatrixOperation.Cofactor:
                    return DeterminantWithCofactors(m);
                case MatrixOperation.Gauss:
                    return DeterminantWithGaussEliminitation(m);
                default:
                    throw new NotSupportedException();
            }
        }

        private static double DeterminantForSmallValues(IMatrix m)
        {
            if (m.Rows == 1)
            {
                return m[0, 0];
            }
            if (m.Rows == 2)
            {
                return m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];
            }
            if (m.Rows == 3)
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

        private static double DeterminantWithCofactors(IMatrix m)
        {
            if (m.Rows != m.Columns)
            {
                throw new ArgumentException("Undetermined");
            }
            if (m.Rows < 4)
            {
                return DeterminantForSmallValues(m);
            }
            double result = 0.0;
            for (int i = 0; i < m.Columns; ++i)
            {
                if (m[0, i] != 0)
                {
                    result += m[0, i] * Cofactor(m, MatrixOperation.Cofactor, 0, i);
                }
            }
            return result;
        }

        private static double DeterminantWithGaussEliminitation(IMatrix m)
        {
            if (m.Rows != m.Columns)
            {
                throw new ArgumentException("Undetermined");
            }
            if (m.Rows < 4)
            {
                return DeterminantForSmallValues(m);
            }

            var diagonal = CopyFromToArray(m);
            GaussEliminitation(diagonal, _ => _, _ => _);
            var result = 1.0;
            for (int i = 0; i < m.Rows; i++)
            {
                result *= diagonal[i, i];
            }
            return result;
        }

        public static Matrix CopyFrom(IMatrix m)
        {
            return new Matrix(CopyFromToArray(m), false);
        }

        public static double[,] CopyFromToArray(IMatrix m)
        {
            var result = new double[m.Rows, m.Columns];
            for (int i = 0; i < m.Rows; i++)
            {
                for (int j = 0; j < m.Columns; j++)
                {
                    result[i, j] = m[i, j];
                }
            }
            return result;
        }

        public static Matrix FromArray(int rows, int columns, double[] values)
        {
            if (rows * columns != values.Length)
            {
                throw new ArgumentException("Incompatible array size");
            }
            var result = new double[rows, columns];
            int v = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = values[v++];
                }
            }
            return new Matrix(result, false);
        }

        public static Matrix Zero(int rank)
        {
            return new Matrix(rank);
        }

        public static IMatrix Identity(int rank)
        {
            if (rank == 4)
            {
                return Matrix4x4.Identity;
            }
            var result = new double[rank, rank];
            for (int i = 0; i < rank; ++i)
            {
                result[i, i] = 1.0;
            }
            return new Matrix(result, false);
        }

        public static IMatrix Transpose(IMatrix a, bool copy)
        {
            return copy ? TransposeCopy(a) : TransposeNoCopy(a);
        }

        private static IMatrix TransposeNoCopy(IMatrix a)
        {
            return new TransposeMatrix(a);
        }

        private static Matrix TransposeCopy(IMatrix a)
        {
            var result = new double[a.Columns, a.Rows];
            for (int i = 0; i < a.Columns; i++)
            {
                for (int j = 0; j < a.Rows; j++)
                {
                    result[i, j] = a[j, i];
                }
            }
            return new Matrix(result, false);
        }

        public static Matrix Subtract(IMatrix a, IMatrix b)
        {
            if ((a.Columns != b.Columns) || (a.Rows != b.Rows))
            {
                throw new ArgumentException();
            }
            var result = new double[a.Rows, a.Columns];
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result[i, j] = a[i, j] - b[i, j];
                }
            }
            return new Matrix(result, false);
        }

        public static Matrix Multiply(IMatrix a, IMatrix b)
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

            var result = new double[a.Rows, b.Columns];
            if ((a.Rows == 4 && a.Columns == 4) && (b.Rows == 4))
            {
                result[0, 0] = a[0, 0] * b[0, 0] + a[0, 1] * b[1, 0] + a[0, 2] * b[2, 0] + a[0, 3] * b[3, 0];
                result[1, 0] = a[1, 0] * b[0, 0] + a[1, 1] * b[1, 0] + a[1, 2] * b[2, 0] + a[1, 3] * b[3, 0];
                result[2, 0] = a[2, 0] * b[0, 0] + a[2, 1] * b[1, 0] + a[2, 2] * b[2, 0] + a[2, 3] * b[3, 0];
                result[3, 0] = a[3, 0] * b[0, 0] + a[3, 1] * b[1, 0] + a[3, 2] * b[2, 0] + a[3, 3] * b[3, 0];

                if (b.Columns == 4)
                {
                    result[0, 1] = a[0, 0] * b[0, 1] + a[0, 1] * b[1, 1] + a[0, 2] * b[2, 1] + a[0, 3] * b[3, 1];
                    result[1, 1] = a[1, 0] * b[0, 1] + a[1, 1] * b[1, 1] + a[1, 2] * b[2, 1] + a[1, 3] * b[3, 1];
                    result[2, 1] = a[2, 0] * b[0, 1] + a[2, 1] * b[1, 1] + a[2, 2] * b[2, 1] + a[2, 3] * b[3, 1];
                    result[3, 1] = a[3, 0] * b[0, 1] + a[3, 1] * b[1, 1] + a[3, 2] * b[2, 1] + a[3, 3] * b[3, 1];

                    result[0, 2] = a[0, 0] * b[0, 2] + a[0, 1] * b[1, 2] + a[0, 2] * b[2, 2] + a[0, 3] * b[3, 2];
                    result[1, 2] = a[1, 0] * b[0, 2] + a[1, 1] * b[1, 2] + a[1, 2] * b[2, 2] + a[1, 3] * b[3, 2];
                    result[2, 2] = a[2, 0] * b[0, 2] + a[2, 1] * b[1, 2] + a[2, 2] * b[2, 2] + a[2, 3] * b[3, 2];
                    result[3, 2] = a[3, 0] * b[0, 2] + a[3, 1] * b[1, 2] + a[3, 2] * b[2, 2] + a[3, 3] * b[3, 2];

                    result[0, 3] = a[0, 0] * b[0, 3] + a[0, 1] * b[1, 3] + a[0, 2] * b[2, 3] + a[0, 3] * b[3, 3];
                    result[1, 3] = a[1, 0] * b[0, 3] + a[1, 1] * b[1, 3] + a[1, 2] * b[2, 3] + a[1, 3] * b[3, 3];
                    result[2, 3] = a[2, 0] * b[0, 3] + a[2, 1] * b[1, 3] + a[2, 2] * b[2, 3] + a[2, 3] * b[3, 3];
                    result[3, 3] = a[3, 0] * b[0, 3] + a[3, 1] * b[1, 3] + a[3, 2] * b[2, 3] + a[3, 3] * b[3, 3];
                }
            }
            else
            {
                for (int i = 0; i < a.Rows; i++)
                {
                    for (int j = 0; j < b.Columns; j++)
                    {
                        double cij = MultiplyRowAndColumn(i, j, a, b);
                        result[i, j] = cij;
                    }
                }
            }
            return new Matrix(result, false);
        }

        public static IMatrix Power(IMatrix m, uint pow)
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
               return CopyFrom(m);
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

        private static void GaussEliminitation(double[,] result,
                                               Func<int, int> rowFunction,
                                               Func<int, int> columnFunction)
        {
            for (int i = 0; i < result.GetLength(0) - 1; i++)
            {
                for (int r = i + 1; r < result.GetLength(0); r++)
                {
                    var rr = rowFunction(r);
                    var ii = rowFunction(i);
                    double d = result[rr, ii] / result[ii, ii];
                    for (int j = 0; j < result.GetLength(1); j++)
                    {
                        var jj = columnFunction(j);
                        result[rr, jj] = result[rr, jj] - d * result[ii, jj];
                    }
                }
            }
        }

        public static Matrix Minor(IMatrix a, int r, int c)
        {
            var result = new double[a.Rows - 1, a.Columns - 1];
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
                    result[ii, jj] = a[i, j];
                    jj++;
                }
                ii++;
            }
            return new Matrix(result, false);
        }

        public static IMatrix Invert(IMatrix a)
        {
            return Invert(a, MatrixOperation.Cofactor);
        }

        public static IMatrix Invert(IMatrix a, MatrixOperation operationType)
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

        public static IMatrix InvertGauss(IMatrix a)
        {
            var tempRows = a.Rows;
            var tempColumns = a.Columns + a.Columns;
            var temp = new double[tempRows, tempColumns];
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    temp[i, j] = a[i, j];
                }
                temp[i, i + a.Columns] = 1.0;
            }
            GaussEliminitation(temp, i => i, j => j);
            GaussEliminitation(temp, i => tempRows - 1 - i, j => tempColumns - 1 - j);
            for (int i = 0; i < tempRows; i++)
            {
                double d = temp[i, i];
                for (int j = 0; j < tempColumns; j++)
                {
                    temp[i, j] = temp[i, j] / d;
                }
            }
            var result = new double[a.Rows, a.Columns];
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    result[i, j] = temp[i, a.Columns + j];
                }
            }
            return new Matrix(result, false);
        }

        public static IMatrix InvertCofactor(IMatrix a)
        {
            var det = Determinant(a, MatrixOperation.Cofactor);
            if (Constants.EpsilonCompare(det, 0.0))
            {
                throw new ArgumentException("Zero determinant");
            }
            var result = new double[a.Rows, a.Columns];
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    double c = Cofactor(a, MatrixOperation.Cofactor, i, j);
                    result[j, i] = c / det;
                }
            }
            return new Matrix(result, false);
        }

        public static double Cofactor(IMatrix a, MatrixOperation operationType, int r, int c)
        {
            double minorDeterminant = Determinant(Minor(a, r, c), operationType);
            if (((r + c) & 1) == 1)
            {
                return -minorDeterminant;
            }
            else
            {
                return minorDeterminant;
            }
        }

        private static double MultiplyRowAndColumn(int row, int column, IMatrix a, IMatrix b)
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

            public static Tuple4 ToTuple(IMatrix a)
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

            public static IMatrix FromTuple(Tuple4 tuple)
            {
                return new Matrix(new double[,]
                {
                    {  tuple.X },
                    {  tuple.Y },
                    {  tuple.Z },
                    {  tuple.W }
                }, false);
            }

            public static IMatrix ViewTransform(Tuple4 from, Tuple4 to, Tuple4 up)
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

                return Multiply(orientation, Translation(-from.X, -from.Y, -from.Z));
            }

            public static Tuple4 Transform(IMatrix matrix, Tuple4 tuple)
            {
                return MatrixOperations.Geometry3D.ToTuple(
                            MatrixOperations.Multiply(matrix, MatrixOperations.Geometry3D.FromTuple(tuple)));
            }
        }
    }
}
