using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

using Xunit;
using Xunit.Abstractions;

using Protsyk.RayTracer.Challenge.Core;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.UnitTests
{
    public class MatrixTests
    {
        private readonly ITestOutputHelper output;

        public MatrixTests(ITestOutputHelper testOutputHelper)
        {
            this.output = testOutputHelper;
        }

        [Fact]
        public void AcceptanceTest()
        {
            {
                var a = MatrixOperations.FromArray(4, 4, new double[]{
                    1, 2, 3, 4,
                    5, 6, 7, 8,
                    9, 8, 7, 6,
                    5, 4, 3, 2
                });

                var b = MatrixOperations.FromArray(4, 4, new double[]{
                    -2, 1, 2, 3,
                    3, 2, 1, -1,
                    4, 3, 6, 5,
                    1, 2, 7, 8
                });

                var c = MatrixOperations.Multiply(a, b);

                var expected = MatrixOperations.FromArray(4, 4, new double[]{
                    20, 22, 50, 48,
                    44, 54, 114, 108,
                    40, 58, 110, 102,
                    16, 26, 46, 42
                });

                if (!expected.Equals(c))
                {
                    throw new Exception();
                }

                output.WriteLine(c.ToString());
            }

            {
                var a = MatrixOperations.FromArray(4, 4, new double[]{
                    1, 2, 3, 4,
                    2, 4, 4, 2,
                    8, 6, 4, 1,
                    0, 0, 0, 1
                });

                var b = MatrixOperations.FromArray(4, 1, new double[]{
                    1, 2, 3, 1
                });

                var c = MatrixOperations.Multiply(a, b);

                var expected = MatrixOperations.FromArray(4, 1, new double[]{
                    18,
                    24,
                    33,
                    1
                });

                if (!expected.Equals(c))
                {
                    throw new Exception();
                }

                output.WriteLine(c.ToString());
            }

            {
                var a = MatrixOperations.FromArray(2, 2, new double[]{
                    1, 5,
                    -3, 2,
                });

                if (!Constants.EpsilonCompare(17, MatrixOperations.Determinant(a, MatrixOperation.Cofactor)))
                {
                    throw new Exception();
                }

                output.WriteLine(MatrixOperations.Determinant(a, MatrixOperation.Cofactor).ToString());
            }

            {
                var a = MatrixOperations.FromArray(3, 3, new double[]{
                    1, 2, 6,
                    -5, 8, -4,
                    2, 6, 4
                });

                if (!Constants.EpsilonCompare(-196, MatrixOperations.Determinant(a, MatrixOperation.Cofactor)))
                {
                    throw new Exception();
                }
                if (!Constants.EpsilonCompare(-196, MatrixOperations.Determinant(a, MatrixOperation.Gauss)))
                {
                    throw new Exception();
                }

                output.WriteLine(MatrixOperations.Determinant(a, MatrixOperation.Cofactor).ToString());
            }

            {
                var a = MatrixOperations.FromArray(4, 4, new double[]{
                    -2, -8, 3, 5,
                    -3, 1, 7, 3,
                    1, 2, -9, 6,
                    -6, 7, 7, -9
                });

                if (!Constants.EpsilonCompare(-4071, MatrixOperations.Determinant(a, MatrixOperation.Gauss)))
                {
                    throw new Exception();
                }
                if (!Constants.EpsilonCompare(-4071, MatrixOperations.Determinant(a, MatrixOperation.Cofactor)))
                {
                    throw new Exception();
                }

                output.WriteLine(MatrixOperations.Determinant(a, MatrixOperation.Cofactor).ToString());
            }

            {
                var a = MatrixOperations.FromArray(5, 5, new double[]{
                    -2, -8, 3, 5, 0,
                    -3, 1, 7, 3, 0,
                    1, 2, -9, 6, 0,
                    -6, 7, 7, -9, 0,
                    1, 4, 5, 6, 7
                });

                if (!Constants.EpsilonCompare(-28497, MatrixOperations.Determinant(a, MatrixOperation.Gauss)))
                {
                    throw new Exception();
                }
                if (!Constants.EpsilonCompare(-28497, MatrixOperations.Determinant(a, MatrixOperation.Cofactor)))
                {
                    throw new Exception();
                }

                output.WriteLine(MatrixOperations.Determinant(a, MatrixOperation.Cofactor).ToString());
            }

            {
                var a = MatrixOperations.FromArray(4, 4, new double[]{
                    -2, -8, 3, 5,
                    -3, 1, 7, 3,
                    1, 2, -9, 6,
                    -6, 7, 7, -9
                });
                var a11 = MatrixOperations.Invert(a, MatrixOperation.Gauss);
                var a12 = MatrixOperations.Invert(a, MatrixOperation.Cofactor);

                var r1 = MatrixOperations.Multiply(a, a11);
                var r2 = MatrixOperations.Multiply(a, a12);

                var i = MatrixOperations.Identity(4);

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

                output.WriteLine(a11.ToString());
            }

            {
                var a = MatrixOperations.FromArray(4, 4, new double[]{
                    -2, -8, 3, 5,
                    -3, 1, 7, 3,
                    1, 2, -9, 6,
                    -6, 7, 7, -9
                });

                // NOTE: For this input starts to fail when p = 15
                for (int p=0; p<15; ++p)
                {
                    var b = MatrixOperations.Identity(4);
                    for (int i=0; i<p; ++i)
                    {
                        b = MatrixOperations.Multiply(b, a);
                    }

                    var c = MatrixOperations.Power(a, (uint)p);

                    if (!c.Equals(b))
                    {
                        throw new Exception();
                    }
                }
            }
        }

    }
}