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
    [FeatureFile("./features/smooth-triangles.feature")]
    public class SmoothTrianglesTest : Feature
    {
        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();

        private readonly IDictionary<string, TriangleWithNormalsFigure> figure = new Dictionary<string, TriangleWithNormalsFigure>();
        private readonly IDictionary<string, Intersection?> intersection = new Dictionary<string, Intersection?>();
        private readonly IDictionary<string, Intersection[]> intersections = new Dictionary<string, Intersection[]>();
        private readonly IDictionary<string, HitResult[]> hits = new Dictionary<string, HitResult[]>();
        private readonly IDictionary<string, HitResult> hit = new Dictionary<string, HitResult>();
        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly ITestOutputHelper testOutputHelper;

        private static readonly string ComputationsId = "comps";

        public SmoothTrianglesTest(ITestOutputHelper testOutputHelper)
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

        [Given(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Vector_id(string id, double t1, double t2, double t3)
        {
            tuple[id] = Tuple4.Vector(t1, t2, t3);
        }

        [When(@"([a-z][a-z0-9]*) ← smooth_triangle\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void Given_triangle(string id, string p1, string p2, string p3, string n1, string n2, string n3)
        {
            figure[id] = new TriangleWithNormalsFigure(MatrixOperations.Identity(4),
                                                        MaterialConstants.Default,
                                                        tuple[p1],
                                                        tuple[p2],
                                                        tuple[p3],
                                                        tuple[n1],
                                                        tuple[n2],
                                                        tuple[n3]);
        }

        [When(@"([a-z][a-z0-9]*) ← intersection_with_uv\(([+-.0-9]+), ([a-z][a-z0-9]*), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← intersection_with_uv\(([+-.0-9]+), ([a-z][a-z0-9]*), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void When_intersection(string id, double t, string figureId, double u, double v)
        {
            intersection[id] = new Intersection(t, figure[figureId], u, v);
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

        [And(@"([a-z][a-z0-9]*).n1 = ([a-z][a-z0-9]*)")]
        public void Then_triangle_n1(string id, string n1)
        {
            Assert.Equal(tuple[n1], figure[id].N1);
        }

        [And(@"([a-z][a-z0-9]*).n2 = ([a-z][a-z0-9]*)")]
        public void Then_triangle_n2(string id, string n2)
        {
            Assert.Equal(tuple[n2], figure[id].N2);
        }

        [And(@"([a-z][a-z0-9]*).n3 = ([a-z][a-z0-9]*)")]
        public void Then_triangle_n3(string id, string n3)
        {
            Assert.Equal(tuple[n3], figure[id].N3);
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

        [When(@"([a-z][a-z0-9]*) ← normal_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← normal_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), ([a-z][a-z0-9]*)\)")]
        public void Given_normal_at(string id, string fId, double p1, double p2, double p3, string iId)
        {
            tuple[id] = figure[fId].GetNormal(figure[fId], new Tuple4(p1, p2, p3, TupleFlavour.Point), intersection[iId].Value.u, intersection[iId].Value.v);
        }

        [When(@"([a-z][a-z0-9]*) ← local_intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← local_intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void Given_local_intersect(string id, string fId, string rId)
        {
            // Local intersect is the same as normal with identity transformation
            hits[id] = figure[fId].AllHits(ray[rId].origin, ray[rId].dir);
        }

        [Then(@"([a-z][a-z0-9]*) is empty")]
        public void Then_intersections_are_empty(string a)
        {
            Assert.Single(hits[a]);
            Assert.Equal(HitResult.NoHit, hits[a][0]);
        }

        [Then(@"([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_vector(string a, double t1, double t2, double t3)
        {
            Assert.Equal(new Tuple4(t1, t2, t3, TupleFlavour.Vector), tuple[a]);
        }

        [When(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
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
                Assert.Equal(HitResult.NoHit, hits[id][0]);
            }
            else
            {
                Assert.Equal(v, hits[id].Length);
            }
        }

        [Then(@"([a-z][a-z0-9]*)\[([0-9]+)\].u = ([+-.0-9]+)")]
        public void Then_intersect_u(string id, int i, double u)
        {
            Assert.True(Constants.EpsilonCompare(u, hits[id][i].U));
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\].v = ([+-.0-9]+)")]
        public void Then_intersect_v(string id, int i, double v)
        {
            Assert.True(Constants.EpsilonCompare(v, hits[id][i].V));
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\].t = ([+-.0-9]+)")]
        public void Then_intersect_value(string id, int i, double v)
        {
            Assert.True(Constants.EpsilonCompare(v, hits[id][i].Distance));
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\].object = ([a-z][a-z0-9]*)")]
        public void Then_intersect_object(string id, int i, string figureId)
        {
            Assert.Equal(figure[figureId], hits[id][i].Figure);
        }

        [And(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*)\)")]
        public void When_intersections_1(string id, string i1)
        {
            intersections[id] = new Intersection[]
            {
                intersection[i1].Value
            };
        }

        [And(@"comps ← prepare_computations\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_prepare_computations_with_xs(string id, string rayId, string xs)
        {
            var i = intersection[id].Value;
            var ixs = intersections[xs];
            var r = ray[rayId];

            var comps = IntersectionTest.prepareComputations(r, i, ixs);

            hit[ComputationsId] = comps.hit;
        }

        [Then(@"comps.normalv = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_hit_eyev(double x, double y, double z)
        {
            Assert.Equal(Tuple4.Vector(x, y, z), hit[ComputationsId].SurfaceNormal);
        }

    }
}
