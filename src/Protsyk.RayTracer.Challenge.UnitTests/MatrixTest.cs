using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using DataTable = Gherkin.Ast.DataTable;

using Protsyk.RayTracer.Challenge.Core;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/matrices.feature")]
    public class MatrixTests : Feature
    {
        private readonly IDictionary<string, IMatrix> cache = new Dictionary<string, IMatrix>();
        private readonly ITestOutputHelper testOutputHelper;

        public MatrixTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            cache["identity_matrix"] = MatrixOperations.Identity(4);
        }

        [Given(@"the following matrix ([a-zA-Z][a-zA-Z0-9]*):")]
        [And(@"the following matrix ([a-zA-Z][a-zA-Z0-9]*):")]
        public void Given_matrix(string id, DataTable dataTable)
        {
            var n = dataTable.Rows.Count();
            Given_matrix_with_dimensions(n, n, id, dataTable);
        }


        [Given(@"the following (\d+)x(\d+) matrix ([a-zA-Z][a-zA-Z0-9]*):")]
        [And(@"the following (\d+)x(\d+) matrix ([a-zA-Z][a-zA-Z0-9]*):")]
        public void Given_matrix_with_dimensions(int rows, int cols, string id, DataTable dataTable)
        {
            var m = new double[rows, cols];
            int r = 0;
            int c = 0;
            foreach (var row in dataTable.Rows)
            {
                c = 0;
                foreach (var cell in row.Cells)
                {
                    m[r, c] = double.Parse(cell.Value);
                    ++c;
                }
                ++r;
            }
            cache.Add(id, new Matrix(m));
        }

        [Given(@"([a-z][a-z0-9]*) ← tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_tuple_id(string id, double t1, double t2, double t3, double t4)
        {
            var m = new double[4, 1] {
                { t1 },
                { t2 },
                { t3 },
                { t4 }
            };
            cache.Add(id, new Matrix(m));
        }

        [Given(@"([a-zA-Z][a-zA-Z0-9]*) ← transpose\(([a-zA-Z][_a-zA-Z0-9]*)\)")]
        public void Given_transpose(string a, string b)
        {
            cache.Add(a, MatrixOperations.Transpose(cache[b], false));
        }

        [Then(@"([a-zA-Z][a-zA-Z0-9]*)\[(\d+),(\d+)\] = ([+-.0-9]+)")]
        [And(@"([a-zA-Z][a-zA-Z0-9]*)\[(\d+),(\d+)\] = ([+-.0-9]+)")]
        public void Then_cell_value(string id, int r, int c, double v)
        {
            var m = cache[id];
            Assert.True(Constants.EpsilonCompare(m[r, c], v));
        }

        [Then(@"([a-zA-Z][a-zA-Z0-9]*) = ([a-zA-Z][_a-zA-Z0-9]*)")]
        public void Then_matrix_equals(string idA, string idB)
        {
            Assert.True(cache[idA].Equals(cache[idB]));
        }

        [Then(@"([a-zA-Z][a-zA-Z0-9]*) != ([a-zA-Z][a-zA-Z0-9]*)")]
        public void Then_matrix_not_equals(string idA, string idB)
        {
            Assert.False(cache[idA].Equals(cache[idB]));
        }

        [Then(@"([a-zA-Z][a-zA-Z0-9]*) \* ([a-zA-Z][a-zA-Z0-9]*) = tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_matrix_multiply(string a, string b, double t1, double t2, double t3, double t4)
        {
            var expectedResult = new Matrix(new double[4, 1] {
                { t1 },
                { t2 },
                { t3 },
                { t4 }
            });
            var actualResult = MatrixOperations.Multiply(cache[a], cache[b]);
            Assert.True(expectedResult.Equals(actualResult));
        }

        [Then(@"([a-zA-Z][_a-zA-Z0-9]*) \* ([a-zA-Z][_a-zA-Z0-9]*) = ([a-zA-Z][a-zA-Z0-9]*)")]
        public void Then_matrix_matrix_multiply(string a, string i, string b)
        {
            var actualResult = MatrixOperations.Multiply(cache[a], cache[i]);
            Assert.True(cache[b].Equals(actualResult));
        }

        [Then(@"([a-zA-Z][a-zA-Z0-9]*) \* ([a-zA-Z][a-zA-Z0-9]*) is the following (\d+)x(\d+) matrix:")]
        public void Then_inverse(string a, string b, int rows, int cols, DataTable dataTable)
        {
            var expectedId = $"{a} * {b}";
            Given_matrix(expectedId, dataTable);
            Assert.True(cache[expectedId].Equals(MatrixOperations.Multiply(cache[a], cache[b])));
            cache.Remove(expectedId);
        }

        [And(@"([a-zA-Z][a-zA-Z0-9]*) ← ([a-zA-Z][a-zA-Z0-9]*) \* ([a-zA-Z][a-zA-Z0-9]*)")]
        public void Then_assing_matrix_multiply(string c, string a, string b)
        {
            cache[c] = MatrixOperations.Multiply(cache[a], cache[b]);
        }

        [Then(@"([a-zA-Z][a-zA-Z0-9]*) \* inverse\(([a-zA-Z][a-zA-Z0-9]*)\) = ([a-zA-Z][a-zA-Z0-9]*)")]
        public void Then_matrix_inverse_multiply(string c, string b, string a)
        {
            // Invert using Cofactor
            {
                var actualResult = MatrixOperations.Multiply(cache[c],
                                        MatrixOperations.Invert(cache[b], MatrixOperation.Cofactor));
                Assert.True(cache[a].Equals(actualResult));
            }

            // Invert using Gauss
            {
                var actualResult = MatrixOperations.Multiply(cache[c],
                                        MatrixOperations.Invert(cache[b], MatrixOperation.Gauss));
                Assert.True(cache[a].Equals(actualResult));
            }
        }

        [Then(@"transpose\(([a-zA-Z][a-zA-Z0-9]*)\) is the following matrix:")]
        public void Then_transpose(string id, DataTable dataTable)
        {
            var expectedId = $"transpose({id})";
            Given_matrix(expectedId, dataTable);
            Assert.True(cache[expectedId].Equals(MatrixOperations.Transpose(cache[id], false)));
            cache.Remove(expectedId);
        }

        [Then(@"determinant\(([a-zA-Z][a-zA-Z0-9]*)\) = ([+-.0-9]+)")]
        [And(@"determinant\(([a-zA-Z][a-zA-Z0-9]*)\) = ([+-.0-9]+)")]
        public void Then_matrix_determinant(string a, double det)
        {
            // Invert using Cofactor
            {
                var actualResult = MatrixOperations.Determinant(cache[a], MatrixOperation.Cofactor);
                Assert.True(Constants.EpsilonCompare(det, actualResult));
            }

            // Invert using Gauss
            {
                var actualResult = MatrixOperations.Determinant(cache[a], MatrixOperation.Gauss);
                Assert.True(Constants.EpsilonCompare(det, actualResult));
            }
        }

        [Then(@"submatrix\(([a-zA-Z][a-zA-Z0-9]*), (\d+), (\d+)\) is the following (\d+)x(\d+) matrix:")]
        public void Then_submatrix_with_dimensions(string id, int row, int col, int rows, int cols, DataTable dataTable)
        {
            var minorId = $"submatrix({id}, {row}, {col})";
            Given_matrix_with_dimensions(rows, cols, minorId, dataTable);
            var actualResult = MatrixOperations.Minor(cache[id], row, col);
            Assert.True(cache[minorId].Equals(actualResult));
            cache.Remove(minorId);
        }

        [And(@"([a-zA-Z][a-zA-Z0-9]*) ← submatrix\(([a-zA-Z][a-zA-Z0-9]*), (\d+), (\d+)\)")]
        public void And_assign_submatrix(string b, string a, int row, int col)
        {
            cache[b] = MatrixOperations.Minor(cache[a], row, col);
        }

        [Then(@"minor\(([a-zA-Z][a-zA-Z0-9]*), (\d+), (\d+)\) = ([+-.0-9]+)")]
        [And(@"minor\(([a-zA-Z][a-zA-Z0-9]*), (\d+), (\d+)\) = ([+-.0-9]+)")]
        public void Then_determinant_of_minor(string a, int row, int col, double det)
        {
            {
                var actualResult = MatrixOperations.Determinant(MatrixOperations.Minor(cache[a], row, col), MatrixOperation.Cofactor);
                Assert.True(Constants.EpsilonCompare(det, actualResult));
            }

            {
                var actualResult = MatrixOperations.Determinant(MatrixOperations.Minor(cache[a], row, col), MatrixOperation.Gauss);
                Assert.True(Constants.EpsilonCompare(det, actualResult));
            }
        }

        [And(@"([a-zA-Z][a-zA-Z0-9]*) is invertible")]
        public void And_matrix_is_invertible(string id)
        {
            var det = MatrixOperations.Determinant(cache[id], MatrixOperation.Gauss);
            Assert.False(Constants.EpsilonZero(det));
        }

        [And(@"([a-zA-Z][a-zA-Z0-9]*) is not invertible")]
        public void And_matrix_is_not_invertible(string id)
        {
            var det = MatrixOperations.Determinant(cache[id], MatrixOperation.Gauss);
            Assert.True(Constants.EpsilonZero(det));
        }

        [Then(@"cofactor\(([a-zA-Z][a-zA-Z0-9]*), (\d+), (\d+)\) = ([+-.0-9]+)")]
        [And(@"cofactor\(([a-zA-Z][a-zA-Z0-9]*), (\d+), (\d+)\) = ([+-.0-9]+)")]
        public void Then_cofactor(string id, int row, int col, double value)
        {
            var actualResult = MatrixOperations.Cofactor(cache[id], MatrixOperation.Cofactor, row, col);
            Assert.True(Constants.EpsilonCompare(value, actualResult));
        }

        [And(@"([a-zA-Z][a-zA-Z0-9]*) ← inverse\(([a-zA-Z][a-zA-Z0-9]*)\)")]
        public void Then_assing_matrix_inverse(string b, string a)
        {
            cache[b] = MatrixOperations.Invert(cache[a], MatrixOperation.Cofactor);
        }

        [And(@"([a-zA-Z][a-zA-Z0-9]*) is the following (\d+)x(\d+) matrix:")]
        public void And_matrix_equals(string id, int rows, int cols, DataTable dataTable)
        {
            var expectedId = $"expected_{id}";
            Given_matrix_with_dimensions(rows, cols, expectedId, dataTable);
            Then_matrix_equals(id, expectedId);
            cache.Remove(expectedId);
        }

        [Then(@"inverse\(([a-zA-Z][a-zA-Z0-9]*)\) is the following (\d+)x(\d+) matrix:")]
        public void Then_inverse(string id, int rows, int cols, DataTable dataTable)
        {
            var expectedId = $"inverse({id})";
            Given_matrix(expectedId, dataTable);
            Assert.True(cache[expectedId].Equals(MatrixOperations.Invert(cache[id], MatrixOperation.Cofactor)));
            Assert.True(cache[expectedId].Equals(MatrixOperations.Invert(cache[id], MatrixOperation.Gauss)));
            cache.Remove(expectedId);
        }
    }
}
