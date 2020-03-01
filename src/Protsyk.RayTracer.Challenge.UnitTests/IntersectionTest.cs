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
    [FeatureFile("./features/intersections_done.feature")]
    public class IntersectionTest : Feature
    {
        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly IDictionary<string, IFigure> figure = new Dictionary<string, IFigure>();

        private readonly IDictionary<string, HitResult> hit = new Dictionary<string, HitResult>();

        private readonly IDictionary<string, HitResult[]> intersection = new Dictionary<string, HitResult[]>();

        private readonly ITestOutputHelper testOutputHelper;

        public IntersectionTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        public void Given_sphere(string id)
        {
            figure[id] = new SphereFigure(MatrixOperations.Identity(4), MaterialConstants.Default);
        }

        [When(@"([a-z][a-z0-9]*) ← intersection\(([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        [ And(@"([a-z][a-z0-9]*) ← intersection\(([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        public void When_intersection(string id, double t, string figureId)
        {
            hit[id] = new HitResult(true, figure[figureId], t, Tuple4.ZeroPoint, Tuple4.ZeroVector);
        }

        [When(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [ And(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersections(string id, string i1, string i2)
        {
            intersection[id] = new HitResult[]
            {
                hit[i1],
                hit[i2]
            };
        }

        [And(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersections(string id, string i1, string i2, string i3, string i4)
        {
            intersection[id] = new HitResult[]
            {
                hit[i1],
                hit[i2],
                hit[i3],
                hit[i4]
            };
        }

        [When(@"([a-z][a-z0-9]*) ← hit\(([a-z][a-z0-9]*)\)")]
        public void When_hit(string id, string xs)
        {
            hit[id] = HitResult.ClosestPositiveHit(intersection[xs]);
        }

        [Then(@"([a-z][a-z0-9]*).t = ([+-.0-9]+)")]
        public void Then_origin(string id, double t)
        {
            Assert.Equal(t, hit[id].Distance);
        }

        [And(@"([a-z][a-z0-9]*).object = ([a-z][a-z0-9]*)")]
        public void Then_object(string id, string figureId)
        {
            Assert.Equal(figure[figureId], hit[id].Figure);
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
        public void Then_intersect_distance(string id, int i, double v)
        {
            Assert.Equal(v, intersection[id][i].Distance);
        }

        [Then(@"([a-z][a-z0-9]*) = ([a-z][a-z0-9]*)")]
        public void Then_hit(string i1, string i2)
        {
            Assert.Equal(hit[i1], hit[i2]);
        }

        [Then(@"([a-z][a-z0-9]*) is nothing")]
        public void Then_no_hit(string i)
        {
            Assert.Equal(HitResult.NoHit, hit[i]);
        }
    }
}
