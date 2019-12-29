using System;
using System.Text;
using System.Numerics;

namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public class MatrixSimple
    {
        public int rows;
        public int cols;
        public BigInteger[,] mat;

        public MatrixSimple(int iRows, int iCols)
        {
            rows = iRows;
            cols = iCols;
            mat = new BigInteger[rows, cols];
        }

        public BigInteger this[int iRow, int iCol]
        {
            get { return mat[iRow, iCol]; }
            set { mat[iRow, iCol] = value; }
        }

        public MatrixSimple Duplicate()
        {
            var matrix = new MatrixSimple(rows, cols);
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    matrix[i, j] = mat[i, j];
            return matrix;
        }

        public static MatrixSimple ZeroMatrix(int iRows, int iCols)
        {
            var matrix = new MatrixSimple(iRows, iCols);
            for (int i = 0; i < iRows; i++)
                for (int j = 0; j < iCols; j++)
                    matrix[i, j] = 0;
            return matrix;
        }

        public static MatrixSimple IdentityMatrix(int iRows, int iCols)
        {
            var matrix = ZeroMatrix(iRows, iCols);
            for (int i = 0; i < Math.Min(iRows, iCols); i++)
                matrix[i, i] = 1;
            return matrix;
        }

        public static MatrixSimple Power(MatrixSimple m, uint pow)
        {
            if (pow == 0)
            {
               return IdentityMatrix(m.rows, m.cols);
            }
            if (pow == 1)
            {
               return m.Duplicate();
            }

            var x = m.Duplicate();

            var ret = IdentityMatrix(m.rows, m.cols);
            while (pow != 0)
            {
                if ((pow & 1) == 1) ret = Mutiply(ret, x);
                x = Mutiply(x, x);
                pow >>= 1;
            }
            return ret;
        }

        public static MatrixSimple Mutiply(MatrixSimple m1, MatrixSimple m2)
        {
            if (m1.cols != m2.rows)
            {
                throw new Exception("Wrong dimensions");
            }

            var result = ZeroMatrix(m1.rows, m2.cols);
            for (int i = 0; i < result.rows; i++)
            {
                for (int j = 0; j < result.cols; j++)
                {
                    for (int k = 0; k < m1.cols; k++)
                    {
                        result[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }
            return result;
        }
    }
}