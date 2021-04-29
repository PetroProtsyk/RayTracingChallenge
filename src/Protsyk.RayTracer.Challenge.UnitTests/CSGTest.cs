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
    [FeatureFile("./features/csg.feature")]
    public class CSGTest : Feature
    {
        private readonly IDictionary<string, IFigure> figure = new Dictionary<string, IFigure>();

        private readonly IDictionary<string, bool> values = new Dictionary<string, bool>();

        private readonly IDictionary<string, IMatrix> matrix = new Dictionary<string, IMatrix>();

        private readonly IDictionary<string, Intersection[]> intersections = new Dictionary<string, Intersection[]>();

        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();

        private readonly IDictionary<string, HitResult[]> intersection = new Dictionary<string, HitResult[]>();

        private readonly ITestOutputHelper testOutputHelper;

        public CSGTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            matrix["identity_matrix"] = MatrixOperations.Identity(4);
        }

        [And(@"([a-z][a-z0-9]*) ← cube\(\)")]
        public void Given_test_cube(string id)
        {
            figure[id] = new CubeFigure(Matrix4x4.Identity, MaterialConstants.Default);
        }

        [Given(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        [And(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        public void Given_sphere(string id)
        {
            figure[id] = new SphereFigure(Matrix4x4.Identity, MaterialConstants.Default);
        }

        [When("([a-z][a-z0-9]*) ← csg\\(\\\"([a-z][a-z0-9]*)\\\", ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\\)")]
        [And("([a-z][a-z0-9]*) ← csg\\(\\\"([a-z][a-z0-9]*)\\\", ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\\)")]
        public void Given_csg(string id, string op, string f1, string f2)
        {
            figure[id] = new CSGFigure(op, figure[f1], figure[f2]);
        }

        [Then("([a-z][a-z0-9]*).operation = \\\"([a-z][_a-z0-9]*)\\\"")]
        public void Then_operation(string id, string op)
        {
            Assert.Equal(op, ((CSGFigure)figure[id]).Operator);
        }

        [And(@"([a-z][a-z0-9]*).left = ([a-z][_a-z0-9]*)")]
        public void Then_left(string id, string fid)
        {
            Assert.Equal(figure[fid], ((CSGFigure)figure[id]).Left);
        }

        [And(@"([a-z][a-z0-9]*).right = ([a-z][_a-z0-9]*)")]
        public void Then_right(string id, string fid)
        {
            Assert.Equal(figure[fid], ((CSGFigure)figure[id]).Right);
        }

        [And(@"([a-z][a-z0-9]*).parent = ([a-z][a-z0-9]*)")]
        public void And_parent(string id, string gId)
        {
            Assert.Same(figure[gId], figure[id].Parent);
        }

        [When("([a-z][a-z0-9]*) ← intersection_allowed\\(\\\"([a-z][a-z0-9]*)\\\", (true|false), (true|false), (true|false)\\)")]
        public void When_intersection_allowed(string id, string op, bool lhit, bool inl, bool inr)
        {
            values[id] = CSGFigure.IntersectionAllowed(op, lhit, inl, inr);
        }

        [Then("([a-z][a-z0-9]*) = (true|false)")]
        public void Then_result(string id, bool value)
        {
            Assert.Equal(value, values[id]);
        }

        [And(@"([a-z][a-z0-9]*) ← intersections\(([-0-9a-zA-Z.,: ]+)+\)")]
        public void When_intersections_opts(string id, string ixs)
        {
            var li = new List<Intersection>();
            foreach (var si in ixs.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var tk = si.Split(':');
                li.Add(new Intersection(double.Parse(tk[0]), figure[tk[1]]));
            }
            intersections[id] = li.ToArray();
        }

        [When(@"([a-z][a-z0-9]*) ← filter_intersections\(([a-z][_a-z0-9]*), ([a-z][_a-z0-9]*)\)")]
        public void When_filter_intersections(string id, string cId, string xId)
        {
            intersections[id] = ((CSGFigure)figure[cId]).FilterIntersections(intersections[xId]);
        }

        [Then(@"result.count = ([+-.0-9]+)")]
        public void Then_hit_count(int count)
        {
            Assert.Equal(count, intersections["result"].Length);
        }

        [Then(@"xs.count = ([+-.0-9]+)")]
        public void Then_xs_count(int count)
        {
            Assert.Equal(count, intersection["xs"].Length);
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\] = ([a-z][a-z0-9]*)\[([0-9]+)\]")]
        public void Then_intersections(string id, int i, string xid, int xi)
        {
            Assert.Equal(intersections[id][i], intersections[xid][xi]);
        }

        [And(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_ray(string id,
                              double p1, double p2, double p3,
                              double v1, double v2, double v3)
        {
            ray[id] = new Ray(new Tuple4(p1, p2, p3, TupleFlavour.Point),
                              new Tuple4(v1, v2, v3, TupleFlavour.Vector));
        }

        [When(@"([a-z][a-z0-9]*) ← local_intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← local_intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersect(string id, string figureId, string rayId)
        {
            var r = ray[rayId];
            var result = figure[figureId].AllHits(r.origin, r.dir);
            intersection[id] = result;
        }

        [Then(@"([a-z][a-z0-9]*) is empty")]
        public void Then_default_group_is_empty(string id)
        {
            Assert.Equal(HitResult.NoHit, intersection[id][0]);
        }

        [And(@"set_transform\(([a-z][a-z0-9]*), translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_set_translation_transform(string figureId, double t1, double t2, double t3)
        {
            figure[figureId].Transformation = MatrixOperations.Geometry3D.Translation(t1, t2, t3);
        }

        [And(@"([a-z][a-z0-9]*)\[([+-.0-9]+)\].t = ([+-.0-9]+)")]
        public void Then_hit_distance(string id, int index, double t)
        {
            Assert.True(Core.Constants.EpsilonCompare(t, intersection[id][index].Distance));
        }

        [And(@"([a-z][a-z0-9]*)\[([+-.0-9]+)\].object = ([a-z][a-z0-9]*)")]
        public void Then_hit_object(string id, int index, string fId)
        {
            Assert.Same(figure[fId], intersection[id][index].Figure);
        }

    }
}
