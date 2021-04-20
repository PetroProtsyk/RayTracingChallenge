using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Xunit.Gherkin.Quick;
using Protsyk.RayTracer.Challenge.Core;
using System.Threading;
using System.Globalization;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using DataTable = Gherkin.Ast.DataTable;
using Xunit.Abstractions;
using System.IO;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/transformations.feature")]
    public class MatrixTransformationTests : Feature
    {
        private readonly IDictionary<string, IMatrix> cache = new Dictionary<string, IMatrix>();
        private readonly ITestOutputHelper testOutputHelper;

        public MatrixTransformationTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            cache["identity_matrix"] = MatrixOperations.Identity(4);
        }

        [Given(@"([a-zA-Z][a-zA-Z0-9]*) ← translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-zA-Z][a-zA-Z0-9]*) ← translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_translation(string id, double t1, double t2, double t3)
        {
            cache.Add(id, MatrixOperations.Geometry3D.Translation(t1, t2, t3));
        }

        [And(@"([a-zA-Z][a-zA-Z0-9]*) ← inverse\(([a-zA-Z][_a-zA-Z0-9]*)\)")]
        public void Given_inverse(string b, string id)
        {
            cache.Add(b, MatrixOperations.Invert(cache[id]));
        }

        [Given(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_point(string id, double t1, double t2, double t3)
        {
            cache.Add(id, MatrixOperations.Geometry3D.Point(t1, t2, t3));
        }

        [Given(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_vector(string id, double t1, double t2, double t3)
        {
            cache.Add(id, MatrixOperations.Geometry3D.Vector(t1, t2, t3));
        }

        [Then(@"([a-z][a-z0-9]*) \* ([a-z][a-z0-9]*) = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_multiply_to_point(string a, string b, double t1, double t2, double t3)
        {
            var m = MatrixOperations.Multiply(cache[a], cache[b]);
            Assert.Equal(MatrixOperations.Geometry3D.Point(t1, t2, t3), m);
        }

        [Then(@"([a-z][a-z0-9]*) \* ([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_multiply_to_vector(string a, string b, double t1, double t2, double t3)
        {
            var m = MatrixOperations.Multiply(cache[a], cache[b]);
            Assert.Equal(MatrixOperations.Geometry3D.Vector(t1, t2, t3), m);
        }

        [Then(@"([a-z][a-z0-9]*) \* ([a-z][a-z0-9]*) = ([a-z][a-z0-9]*)")]
        public void Then_multiply_to_value(string a, string b, string c)
        {
            var m = MatrixOperations.Multiply(cache[a], cache[b]);
            Assert.Equal(cache[c], m);
        }

        [Then(@"([a-zA-Z][_a-zA-Z0-9]*) = ([a-zA-Z][_a-zA-Z0-9]*)")]
        public void Then_matrix(string a, string b)
        {
            Assert.Equal(cache[a], cache[b]);
        }

        [Then(@"([a-zA-Z][_a-zA-Z0-9]*) = translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_translation(string a, double t1, double t2, double t3)
        {
            Assert.Equal(MatrixOperations.Geometry3D.Translation(t1, t2, t3), cache[a]);
        }

        [Then(@"([a-zA-Z][_a-zA-Z0-9]*) = scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_scaling(string a, double t1, double t2, double t3)
        {
            Assert.Equal(cache[a], MatrixOperations.Geometry3D.Scale(t1, t2, t3));
        }

        [When(@"([a-zA-Z][a-zA-Z0-9]*) ← ([a-zA-Z][a-zA-Z0-9]*) \* ([a-zA-Z][a-zA-Z0-9]*)")]
        public void When_multiply(string id, string a, string b)
        {
            cache.Add(id, MatrixOperations.Multiply(cache[a], cache[b]));
        }

        [When(@"([a-zA-Z][a-zA-Z0-9]*) ← ([a-zA-Z][a-zA-Z0-9]*) \* ([a-zA-Z][a-zA-Z0-9]*) \* ([a-zA-Z][a-zA-Z0-9]*)")]
        public void When_multiply_3(string id, string a, string b, string c)
        {
            cache.Add(id, MatrixOperations.Multiply(MatrixOperations.Multiply(cache[a], cache[b]), cache[c]));
        }

        [Given(@"([a-zA-Z][a-zA-Z0-9]*) ← scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-zA-Z][a-zA-Z0-9]*) ← scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_scaling(string id, double t1, double t2, double t3)
        {
            cache.Add(id, MatrixOperations.Geometry3D.Scale(t1, t2, t3));
        }

        [Given(@"([a-z][a-z0-9]*) ← shearing\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_shearing(string id, double t1, double t2, double t3, double t4, double t5, double t6)
        {
            cache.Add(id, MatrixOperations.Geometry3D.Shearing(t1, t2, t3, t4, t5, t6));
        }

        [And(@"([a-zA-Z][_a-zA-Z0-9]*) ← rotation_([xyz])\(π / ([+-.0-9]+)\)")]
        public void Given_rotation(string id, string axis, double n)
        {
            var angle = Math.PI / n;
            switch (axis)
            {
                case "x":
                    cache.Add(id, MatrixOperations.Geometry3D.RotateX(angle));
                    break;
                case "y":
                    cache.Add(id, MatrixOperations.Geometry3D.RotateY(angle));
                    break;
                case "z":
                    cache.Add(id, MatrixOperations.Geometry3D.RotateZ(angle));
                    break;
                default:
                    throw new Exception($"Unknown axis {axis}");
            }
        }

        [Then(@"([a-zA-Z][_a-zA-Z0-9]*) = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_point(string a, double t1, double t2, double t3)
        {
            Assert.Equal(MatrixOperations.Geometry3D.Point(t1, t2, t3), cache[a]);
        }

        [Then(@"([a-zA-Z][_a-zA-Z0-9]*) \* ([a-zA-Z][_a-zA-Z0-9]*) = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-zA-Z][_a-zA-Z0-9]*) \* ([a-zA-Z][_a-zA-Z0-9]*) = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_multiply_point(string a, string b, double t1, double t2, double t3)
        {
            Assert.Equal(MatrixOperations.Geometry3D.Point(t1, t2, t3), MatrixOperations.Multiply(cache[a], cache[b]));
        }

        [When(@"([a-zA-Z][a-zA-Z0-9]*) ← view_transform\(([a-zA-Z][_a-zA-Z0-9]*), ([a-zA-Z][_a-zA-Z0-9]*), ([a-zA-Z][_a-zA-Z0-9]*)\)")]
        public void Given_view_transform(string id, string from, string to, string up)
        {
            cache.Add(id, MatrixOperations.Geometry3D.LookAtTransform(MatrixOperations.Geometry3D.ToTuple(cache[from]),
                                                                    MatrixOperations.Geometry3D.ToTuple(cache[to]),
                                                                    MatrixOperations.Geometry3D.ToTuple(cache[up])));
        }

        [Then(@"([a-zA-Z][a-zA-Z0-9]*) is the following (\d+)x(\d+) matrix:")]
        public void Then_matrix(string t, int rows, int cols, DataTable dataTable)
        {
            var expectedId = $"expected_{t}";
            Given_matrix_with_dimensions(rows, cols, expectedId, dataTable);
            Assert.Equal(cache[expectedId], cache[t]);
            cache.Remove(expectedId);
        }

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

    }
}
