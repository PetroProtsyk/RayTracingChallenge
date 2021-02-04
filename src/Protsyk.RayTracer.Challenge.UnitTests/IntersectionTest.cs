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
using Protsyk.RayTracer.Challenge.Core.Scene;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/intersections.feature")]
    public class IntersectionTest : Feature
    {
        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly IDictionary<string, IFigure> figure = new Dictionary<string, IFigure>();

        private readonly IDictionary<string, HitResult> hit = new Dictionary<string, HitResult>();

        private readonly IDictionary<string, Intersection> intersection = new Dictionary<string, Intersection>();

        private readonly IDictionary<string, Intersection[]> intersections = new Dictionary<string, Intersection[]>();

        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();

        private readonly ITestOutputHelper testOutputHelper;

        private static readonly string ComputationsId = "comps";

        public IntersectionTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        [ And(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        public void Given_sphere(string id)
        {
            figure[id] = new SphereFigure(MatrixOperations.Identity(4), MaterialConstants.Default);
        }

        [When(@"([a-z][a-z0-9]*) ← intersection\(([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        [ And(@"([a-z][a-z0-9]*) ← intersection\(([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        public void When_intersection(string id, double t, string figureId)
        {
            intersection[id] = new Intersection(t, figure[figureId]);
        }

        [When(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [ And(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersections(string id, string i1, string i2)
        {
            intersections[id] = new Intersection[]
            {
                intersection[i1],
                intersection[i2]
            };
        }

        [And(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersections(string id, string i1, string i2, string i3, string i4)
        {
            intersections[id] = new Intersection[]
            {
                intersection[i1],
                intersection[i2],
                intersection[i3],
                intersection[i4]
            };
        }

        [When(@"([a-z][a-z0-9]*) ← hit\(([a-z][a-z0-9]*)\)")]
        public void When_hit(string id, string xs)
        {
            // Same as HitResult.ClosestPositiveHit
            intersection[id] = intersections[xs].Where(i => i.t > 0).OrderBy(i => i.t).FirstOrDefault();
        }

        [Then(@"([a-z][a-z0-9]*).t = ([+-.0-9]+)")]
        public void Then_origin(string id, double t)
        {
            Assert.Equal(t, intersection[id].t);
        }

        [And(@"([a-z][a-z0-9]*).object = ([a-z][a-z0-9]*)")]
        public void Then_object(string id, string figureId)
        {
            Assert.Equal(figure[figureId], intersection[id].figure);
        }

        [Then(@"([a-z][a-z0-9]*).count = ([+-.0-9]+)")]
        public void Then_intersect_count(string id, int v)
        {
            if (v == 0)
            {
                Assert.Null(intersections[id][0]);
            }
            else
            {
                Assert.Equal(v, intersections[id].Length);
            }
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\].t = ([+-.0-9]+)")]
        public void Then_intersect_distance(string id, int i, double v)
        {
            Assert.Equal(v, intersections[id][i].t);
        }

        [Then(@"([a-z][a-z0-9]*) = ([a-z][a-z0-9]*)")]
        public void Then_hit(string i1, string i2)
        {
            Assert.Equal(intersection[i1], intersection[i2]);
        }

        [Then(@"([a-z][a-z0-9]*) is nothing")]
        public void Then_no_hit(string i)
        {
            Assert.Null(intersection[i]);
        }

        [Given(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_ray(string id,
                              double p1, double p2, double p3,
                              double v1, double v2, double v3)
        {
            ray[id] = new Ray(new Tuple4(p1, p2, p3, TupleFlavour.Point),
                              new Tuple4(v1, v2, v3, TupleFlavour.Vector));
        }

        [When(@"comps ← prepare_computations\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_prepare_computations(string id, string rayId)
        {
            var i = intersection[id];
            var r = ray[rayId];
            var point = r.PositionAt(intersection[id].t);
            var normal = i.figure.GetNormal(point);
            hit[ComputationsId] = new HitResult(true, i.figure, i.t, point, normal, Tuple4.Negate(r.dir));
        }

        [Then(@"comps.t = ([a-z][a-z0-9]*).t")]
        public void Then_hit_t(string id)
        {
            Assert.Equal(intersection[id].t, hit[ComputationsId].Distance);
        }

        [And(@"comps.object = ([a-z][a-z0-9]*).object")]
        public void Then_hit_object(string id)
        {
            Assert.Equal(intersection[id].figure, hit[ComputationsId].Figure);
        }

        [And(@"comps.point = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_hit_point(double x, double y, double z)
        {
            Assert.Equal(new Tuple4(x, y, z, TupleFlavour.Point), hit[ComputationsId].PointOnSurface);
        }

        [And(@"comps.eyev = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_hit_eyev(double x, double y, double z)
        {
            Assert.Equal(new Tuple4(x, y, z, TupleFlavour.Vector), hit[ComputationsId].EyeVector);
        }

        [And(@"comps.normalv = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_hit_normalv(double x, double y, double z)
        {
            Assert.Equal(new Tuple4(x, y, z, TupleFlavour.Vector), hit[ComputationsId].SurfaceNormal);
        }

        private class Intersection
        {
            public IFigure figure;
            public double t;

            public Intersection(double t, IFigure figure)
            {
                this.t = t;
                this.figure = figure;
            }
        }
    }
}
