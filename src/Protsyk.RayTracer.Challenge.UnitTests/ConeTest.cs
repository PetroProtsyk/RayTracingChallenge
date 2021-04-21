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
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;

using DocString = Gherkin.Ast.DocString;
using Protsyk.RayTracer.Challenge.Core.Scene;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/cones.feature")]
    public class ConeTest : Feature
    {
        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();

        private readonly IDictionary<string, ConeFigure> figure = new Dictionary<string, ConeFigure>();

        private readonly IDictionary<string, HitResult[]> intersection = new Dictionary<string, HitResult[]>();

        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly IDictionary<string, IMatrix> matrix = new Dictionary<string, IMatrix>();

        private readonly ITestOutputHelper testOutputHelper;

        public ConeTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            matrix["identity_matrix"] = MatrixOperations.Identity(4);
        }

        [Given(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        [And(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_ray(string id,
                              double p1, double p2, double p3,
                              double v1, double v2, double v3)
        {
            ray[id] = new Ray(Tuple4.Point(p1, p2, p3), Tuple4.Vector(v1, v2, v3));
        }

        [Given(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), ([a-z][a-z0-9]*)\)")]
        public void Given_ray(string id, double p1, double p2, double p3, string dId)
        {
            ray[id] = new Ray(Tuple4.Point(p1, p2, p3), tuple[dId]);
        }

        [Given(@"([a-z][a-z0-9]*) ← cone\(\)")]
        public void Given_sphere(string id)
        {
            figure[id] = new TestCone(MatrixOperations.Identity(4), MaterialConstants.Default);
        }

        [Given(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Point_id(string id, double t1, double t2, double t3)
        {
            tuple.Add(id, Tuple4.Point(t1, t2, t3));
        }

        [Given(@"([a-z][a-z0-9]*) ← normalize\(vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        [And(@"([a-z][a-z0-9]*) ← normalize\(vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Normalize_vector(string id, double t1, double t2, double t3)
        {
            tuple.Add(id, Tuple4.Normalize(Tuple4.Vector(t1, t2, t3)));
        }

        [When(@"([a-z][a-z0-9]*) ← local_intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void Given_local_intersect(string id, string fId, string rId)
        {
            // Local intersect is the same as normal with identity transformation
            intersection[id] = figure[fId].AllHits(ray[rId].origin, ray[rId].dir);
        }

        [Then(@"([a-z][a-z0-9]*).count = ([+-.0-9]+)")]
        public void Then_intersect_count(string id, int v)
        {
            if (v == 0)
            {
                Assert.Equal(HitResult.NoHit, intersection[id][0]);
            }
            else
            {
                Assert.Equal(v, intersection[id].Length);
            }
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\].t = ([+-.0-9]+)")]
        public void Then_intersect_value(string id, int i, double v)
        {
            Assert.True(Constants.EpsilonCompare(v, intersection[id][i].Distance));
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\].object = ([a-z][a-z0-9]*)")]
        public void Then_intersect_object(string id, int i, string figureId)
        {
            Assert.Equal(figure[figureId], intersection[id][i].Figure);
        }

        [When(@"([a-z][a-z0-9]*) ← local_normal_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        [And(@"([a-z][a-z0-9]*) ← local_normal_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_local_normal_at(string id, string fId, double p1, double p2, double p3)
        {
            tuple[id] = ((TestCone)figure[fId]).GetLocalNormal(null, Tuple4.Point(p1, p2, p3));
        }

        [Then(@"([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_vector(string a, double t1, double t2, double t3)
        {
            Assert.Equal(Tuple4.Vector(t1, t2, t3), tuple[a]);
        }

        [Then(@"([a-z][a-z0-9]*).minimum = -infinity")]
        public void Then_default_minimum(string id)
        {
            Assert.Equal(double.NegativeInfinity, figure[id].Minimum);
        }

        [And(@"([a-z][a-z0-9]*).maximum = infinity")]
        public void Then_default_maximum(string id)
        {
            Assert.Equal(double.PositiveInfinity, figure[id].Maximum);
        }

        [Then(@"([a-z][a-z0-9]*).closed = false")]
        public void Then_default_closed(string id)
        {
            Assert.False(figure[id].IsClosed);
        }

        [And(@"([a-z][a-z0-9]*).minimum ← ([+-.0-9]+)")]
        public void And_cylinder_minimum(string id, double m)
        {
            figure[id].Minimum = m;
        }

        [And(@"([a-z][a-z0-9]*).maximum ← ([+-.0-9]+)")]
        public void And_cylinder_maximum(string id, double m)
        {
            figure[id].Maximum = m;
        }

        [And(@"([a-z][a-z0-9]*).closed ← true")]
        public void And_cylinder_closed(string id)
        {
            figure[id].IsClosed = true;
        }

        private class TestCone : ConeFigure
        {
            public TestCone(IMatrix transformation, IMaterial material)
                : this(transformation, material, double.NegativeInfinity, double.PositiveInfinity, false)
            {
            }

            public TestCone(IMatrix transformation, IMaterial material, double min, double max, bool isClosed)
                : base(transformation, material, min, max, isClosed)
            {
            }

            public Tuple4 GetLocalNormal(IFigure figure, Tuple4 point)
            {
                var objectPoint = TransformWorldPointToObjectPoint(point);
                var normal = GetBaseNormal(figure, objectPoint);
                return normal;
            }

        }
    }
}
