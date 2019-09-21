using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Gherkin.Quick;
using Protsyk.RayTracer.Challenge.Core;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/tuples.feature")]
    public class TupleTests : Feature
    {
        private readonly IDictionary<string, Tuple4> cache = new Dictionary<string, Tuple4>();

        [Given(@"([a-z][a-z0-9]*) ← tuple\((-?\d+), (-?\d+), (-?\d+), (-?\d+)\)")]
        [And  (@"([a-z][a-z0-9]*) ← tuple\((-?\d+), (-?\d+), (-?\d+), (-?\d+)\)")]
        public void Tuple_id(string id, double t1, double t2, double t3, double t4)
        {
            cache.Add(id, new Tuple4(t1, t2, t3, t4));
        }

        [Given(@"([a-z][a-z0-9]*) ← vector\((-?\d+), (-?\d+), (-?\d+)\)")]
        [And  (@"([a-z][a-z0-9]*) ← vector\((-?\d+), (-?\d+), (-?\d+)\)")]
        public void Vector_id(string id, double t1, double t2, double t3)
        {
            Tuple_id(id, t1, t2, t3, 0);
        }

        [Given(@"([a-z][a-z0-9]*) ← point\((-?\d+), (-?\d+), (-?\d+)\)")]
        [And  (@"([a-z][a-z0-9]*) ← point\((-?\d+), (-?\d+), (-?\d+)\)")]
        public void Point_id(string id, double t1, double t2, double t3)
        {
            Tuple_id(id, t1, t2, t3, 1);
        }

        [Given(@"([a-z][a-z0-9]*) ← color\((-?\d+), (-?\d+), (-?\d+)\)")]
        [And  (@"([a-z][a-z0-9]*) ← color\((-?\d+), (-?\d+), (-?\d+)\)")]
        public void Color_id(string id, double t1, double t2, double t3, double t4)
        {
            Tuple_id(id, t1, t2, t3, 0);
        }

        [Then(@"Then ([a-z][a-z0-9]*).x = {float}")]
        public void Then_x(string id, double expectedX)
        {
        }

        [Then(@"dot\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\) = (-?\d+)")]
        public void Then_dot(string a, string b, double expected)
        {
            Assert.True(Constants.Equals(expected, Tuple4.DotProduct(cache[a], cache[b])));
        }
    }
}
