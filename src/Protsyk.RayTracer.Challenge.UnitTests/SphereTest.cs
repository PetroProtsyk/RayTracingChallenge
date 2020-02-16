using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

using Xunit;
using Xunit.Gherkin.Quick;
using Xunit.Abstractions;

using Protsyk.RayTracer.Challenge.Core;
using Protsyk.RayTracer.Challenge.Core.Geometry;

using DocString = Gherkin.Ast.DocString;
using Protsyk.RayTracer.Challenge.Core.Scene;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/spheres.feature")]
    public class SphereTest : Feature
    {
        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();

        private readonly IDictionary<string, Sphere> figure = new Dictionary<string, Sphere>();

        private readonly IDictionary<string, double[]> intersection = new Dictionary<string, double[]>();

        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly IDictionary<string, IMatrix> matrix = new Dictionary<string, IMatrix>();


        private readonly ITestOutputHelper testOutputHelper;

        public SphereTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            matrix["identity_matrix"] = MatrixOperations.Identity(4);
        }

        [Given(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_ray(string id, 
                              double p1, double p2, double p3,
                              double v1, double v2, double v3)
        {
            ray[id] = new Ray(new Tuple4(p1, p2, p3, TupleFlavour.Point),
                              new Tuple4(v1, v2, v3, TupleFlavour.Vector));
        }

        [Given(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        [  And(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        public void Given_sphere(string id)
        {
            figure[id] = new Sphere(new Tuple4(0.0, 0.0, 0.0, TupleFlavour.Point), 1.0);
        }

        [When(@"([a-z][a-z0-9]*) ← intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [ And(@"([a-z][a-z0-9]*) ← intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersect(string id, string figureId, string rayId)
        {
            var result = figure[figureId].GetIntersections(ray[rayId]);
            intersection[id] = result;
        }

        [Then(@"([a-z][a-z0-9]*).count = ([+-.0-9]+)")]
        public void Then_intersect_count(string id, int v)
        {
            if (v == 0)
            {
                Assert.Null(intersection[id]);
            }
            else
            {
                Assert.Equal(v, intersection[id].Length);
            }
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\] = ([+-.0-9]+)")]
        public void Then_intersect_value(string id, int i, double v)
        {
            Assert.Equal(v, intersection[id][i]);
        }

        [When(@"([a-z][a-z0-9]*) ← normal_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_normal_at(string id, string figureId, 
                                   double p1, double p2, double p3)
        {
            var result = figure[figureId].GetNormal(new Tuple4(p1, p2, p3, TupleFlavour.Point));
            tuple[id] = result;
        }

        [Then(@"([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_vector(string id,
                                double t1, double t2, double t3)
        {
            var v = new Tuple4(t1, t2, t3, TupleFlavour.Vector);
            Assert.Equal(v, tuple[id]);
        }

        [  And(@"([a-zA-Z][a-zA-Z0-9]*) ← translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_translation(string id, double t1, double t2, double t3)
        {
            matrix.Add(id, MatrixOperations.Geometry3D.Translation(t1, t2, t3));
        }

        [  And(@"([a-zA-Z][a-zA-Z0-9]*) ← scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_scaling(string id, double t1, double t2, double t3)
        {
            matrix.Add(id, MatrixOperations.Geometry3D.Scale(t1, t2, t3));
        }

        [  And(@"([a-zA-Z][a-zA-Z0-9]*) ← scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\) \* rotation_z\(π/5\)")]
        public void Given_scaling_rotation(string id, double t1, double t2, double t3)
        {
            matrix.Add(id, MatrixOperations.Multiply(
                                 MatrixOperations.Geometry3D.Scale(t1, t2, t3),
                                 MatrixOperations.Geometry3D.RotateZ(Math.PI/5)));
        }

        [When(@"set_transform\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [ And(@"set_transform\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_set_transform(string figureId, string matrixId)
        {
            figure[figureId].Transformation = matrix[matrixId];
        }

        [Then(@"([a-z][a-z0-9]*).transform = ([a-z][_a-z0-9]*)")]
        public void Then_transformation(string id, string matrixId)
        {
            Assert.Equal(figure[id].Transformation, matrix[matrixId]);
        }

        [Then(@"([a-z][a-z0-9]*) = normalize\(([a-z][a-z0-9]*)\)")]
        public void Then_n(string a, string b)
        {
            Assert.Equal(tuple[a], Tuple4.Normalize(tuple[b]));
        }
    }
}
