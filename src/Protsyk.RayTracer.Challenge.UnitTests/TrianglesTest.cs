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
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/triangles.feature")]
    public class TrianglesTest : Feature
    {
        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();

        private readonly IDictionary<string, TriangleFigure> figure = new Dictionary<string, TriangleFigure>();

        private readonly IDictionary<string, HitResult[]> intersection = new Dictionary<string, HitResult[]>();

        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly ITestOutputHelper testOutputHelper;

        public TrianglesTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Point_id(string id, double t1, double t2, double t3)
        {
            tuple[id] = Tuple4.Point(t1, t2, t3);
        }

        [And(@"([a-z][a-z0-9]*) ← triangle\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void Given_triangle(string id, string p1, string p2, string p3)
        {
            figure[id] = new TriangleFigure(MatrixOperations.Identity(4), MaterialConstants.Default, tuple[p1], tuple[p2], tuple[p3]);
        }

        [Given(@"([a-z][a-z0-9]*) ← triangle\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_triangle_points(string id,
                                            double p1X, double p1Y, double p1Z,
                                            double p2X, double p2Y, double p2Z,
                                            double p3X, double p3Y, double p3Z)
        {
            figure[id] = new TriangleFigure(MatrixOperations.Identity(4),
                                            MaterialConstants.Default,
                                            Tuple4.Point(p1X, p1Y, p1Z),
                                            Tuple4.Point(p2X, p2Y, p2Z),
                                            Tuple4.Point(p3X, p3Y, p3Z));
        }

        [Then(@"([a-z][a-z0-9]*).p1 = ([a-z][a-z0-9]*)")]
        public void Then_triangle_p1(string id, string p1)
        {
            Assert.Equal(tuple[p1], figure[id].P1);
        }

        [And(@"([a-z][a-z0-9]*).p2 = ([a-z][a-z0-9]*)")]
        public void Then_triangle_p2(string id, string p2)
        {
            Assert.Equal(tuple[p2], figure[id].P2);
        }

        [And(@"([a-z][a-z0-9]*).p3 = ([a-z][a-z0-9]*)")]
        public void Then_triangle_p3(string id, string p3)
        {
            Assert.Equal(tuple[p3], figure[id].P3);
        }

        [And(@"([a-z][a-z0-9]*).e1 = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_triangle_e1(string id, double x, double y, double z)
        {
            Assert.Equal(Tuple4.Vector(x, y, z), figure[id].E1);
        }

        [And(@"([a-z][a-z0-9]*).e2 = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_triangle_e2(string id, double x, double y, double z)
        {
            Assert.Equal(Tuple4.Vector(x, y, z), figure[id].E2);
        }

        [And(@"([a-z][a-z0-9]*).normal = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_triangle_n(string id, double x, double y, double z)
        {
            Assert.Equal(Tuple4.Vector(x, y, z), figure[id].Normal);
        }

        [When(@"([a-z][a-z0-9]*) ← local_normal_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        [And(@"([a-z][a-z0-9]*) ← local_normal_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_local_normal_at(string id, string fId, double p1, double p2, double p3)
        {
            // Local normal is the same as normal with identity transformation
            tuple[id] = figure[fId].GetNormal(null, new Tuple4(p1, p2, p3, TupleFlavour.Point));
        }

        [Then(@"([a-z][a-z0-9]*) = ([a-z][a-z0-9]*).normal")]
        [And(@"([a-z][a-z0-9]*) = ([a-z][a-z0-9]*).normal")]
        public void Then_normal(string id, string fId)
        {
            Assert.Equal(tuple[id], figure[fId].Normal);
        }

        [When(@"([a-z][a-z0-9]*) ← local_intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void Given_local_intersect(string id, string fId, string rId)
        {
            // Local intersect is the same as normal with identity transformation
            intersection[id] = figure[fId].AllHits(ray[rId].origin, ray[rId].dir);
        }

        [Then(@"([a-z][a-z0-9]*) is empty")]
        public void Then_intersections_are_empty(string a)
        {
            Assert.Single(intersection[a]);
            Assert.Equal(HitResult.NoHit, intersection[a][0]);
        }

        [Then(@"([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_vector(string a, double t1, double t2, double t3)
        {
            Assert.Equal(new Tuple4(t1, t2, t3, TupleFlavour.Vector), tuple[a]);
        }

        [And(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_ray(string id,
                              double p1, double p2, double p3,
                              double v1, double v2, double v3)
        {
            ray[id] = new Ray(new Tuple4(p1, p2, p3, TupleFlavour.Point),
                              new Tuple4(v1, v2, v3, TupleFlavour.Vector));
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
    }
}
