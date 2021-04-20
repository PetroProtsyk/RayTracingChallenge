using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

using Xunit;
using Xunit.Gherkin.Quick;
using Xunit.Abstractions;

using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/rays.feature")]
    public class RaysTest : Feature
    {
        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();
        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();
        private readonly IDictionary<string, IMatrix> matrix = new Dictionary<string, IMatrix>();

        private readonly ITestOutputHelper testOutputHelper;

        public RaysTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_ray(string id,
                              double p1, double p2, double p3,
                              double v1, double v2, double v3)
        {
            ray[id] = new Ray(new Tuple4(p1, p2, p3, TupleFlavour.Point),
                              new Tuple4(v1, v2, v3, TupleFlavour.Vector));
        }

        [Given(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Vector_id(string id, double t1, double t2, double t3)
        {
            tuple.Add(id, new Tuple4(t1, t2, t3, TupleFlavour.Vector));
        }

        [Given(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Point_id(string id, double t1, double t2, double t3)
        {
            tuple.Add(id, new Tuple4(t1, t2, t3, TupleFlavour.Point));
        }

        [When(@"([a-z][a-z0-9]*) ← ray\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_ray(string id, string origin, string direction)
        {
            ray.Add(id, new Ray(tuple[origin], tuple[direction]));
        }

        [Then(@"([a-z][a-z0-9]*).origin = ([a-z][a-z0-9]*)")]
        public void Then_origin(string id, string origin)
        {
            Assert.Equal(ray[id].origin, tuple[origin]);
        }

        [Then(@"([a-z][a-z0-9]*).origin = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_origin_point(string id, double t1, double t2, double t3)
        {
            var expected = new Tuple4(t1, t2, t3, TupleFlavour.Point);
            Assert.Equal(expected, ray[id].origin);
        }

        [And(@"([a-z][a-z0-9]*).direction = ([a-z][a-z0-9]*)")]
        public void Then_direction(string id, string direction)
        {
            Assert.Equal(ray[id].dir, tuple[direction]);
        }

        [And(@"([a-z][a-z0-9]*).direction = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_dir_vector(string id, double t1, double t2, double t3)
        {
            var expected = new Tuple4(t1, t2, t3, TupleFlavour.Vector);
            Assert.Equal(expected, ray[id].dir);
        }

        [Then(@"position\(([a-z][a-z0-9]*), ([+-.0-9]+)\) = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"position\(([a-z][a-z0-9]*), ([+-.0-9]+)\) = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_position(string id, double t, double x, double y, double z)
        {
            var expected = new Tuple4(x, y, z, TupleFlavour.Point);
            var actual = ray[id].PositionAt(t);

            Assert.Equal(expected, actual);
        }

        [And(@"([a-z][a-z0-9]*) ← translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void And_translation(string id, double x, double y, double z)
        {
            matrix.Add(id, MatrixOperations.Geometry3D.Translation(x, y, z));
        }

        [And(@"([a-z][a-z0-9]*) ← scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void And_scaling(string id, double x, double y, double z)
        {
            matrix.Add(id, MatrixOperations.Geometry3D.Scale(x, y, z));
        }

        [When(@"([a-z][a-z0-9]*) ← transform\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_transform(string id, string r, string m)
        {
            ray[id] = ray[r].Transform(matrix[m]);
        }

    }
}
