using System;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

using Xunit;
using Xunit.Gherkin.Quick;

using Protsyk.RayTracer.Challenge.Core;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/tuples.feature")]
    public class TupleTests : Feature
    {
        private readonly IDictionary<string, Tuple4> cache = new Dictionary<string, Tuple4>();

        public TupleTests()
        {
            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][a-z0-9]*) ← tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Tuple_id(string id, double t1, double t2, double t3, double t4)
        {
            cache.Add(id, new Tuple4(t1, t2, t3, t4));
        }

        [Given(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Vector_id(string id, double t1, double t2, double t3)
        {
            Tuple_id(id, t1, t2, t3, 0);
        }

        [Given(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Point_id(string id, double t1, double t2, double t3)
        {
            Tuple_id(id, t1, t2, t3, 1);
        }

        [Given(@"([a-z][a-z0-9]*) ← color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Color_id(string id, double t1, double t2, double t3)
        {
            Tuple_id(id, t1, t2, t3, 0);
        }

        [Then(@"([a-z][a-z0-9]*).x = ([+-.0-9]+)")]
        [Then(@"([a-z][a-z0-9]*).red = ([+-.0-9]+)")]
        [And(@"([a-z][a-z0-9]*).x = ([+-.0-9]+)")]
        [And(@"([a-z][a-z0-9]*).red = ([+-.0-9]+)")]
        public void Then_x(string id, double expected)
        {
            Assert.True(Constants.EpsilonCompare(expected, cache[id].X));
        }

        [Then(@"([a-z][a-z0-9]*).y = ([+-.0-9]+)")]
        [Then(@"([a-z][a-z0-9]*).green = ([+-.0-9]+)")]
        [And(@"([a-z][a-z0-9]*).y = ([+-.0-9]+)")]
        [And(@"([a-z][a-z0-9]*).green = ([+-.0-9]+)")]
        public void Then_y(string id, double expected)
        {
            Assert.True(Constants.EpsilonCompare(expected, cache[id].Y));
        }

        [Then(@"([a-z][a-z0-9]*).z = ([+-.0-9]+)")]
        [Then(@"([a-z][a-z0-9]*).blue = ([+-.0-9]+)")]
        [And(@"([a-z][a-z0-9]*).z = ([+-.0-9]+)")]
        [And(@"([a-z][a-z0-9]*).blue = ([+-.0-9]+)")]
        public void Then_z(string id, double expected)
        {
            Assert.True(Constants.EpsilonCompare(expected, cache[id].Z));
        }

        [Then(@"([a-z][a-z0-9]*).w = ([+-.0-9]+)")]
        [And(@"([a-z][a-z0-9]*).w = ([+-.0-9]+)")]
        public void Then_w(string id, double expected)
        {
            Assert.True(Constants.EpsilonCompare(expected, cache[id].W));
        }

        [Then(@"dot\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\) = ([+-.0-9]+)")]
        public void Then_dot(string a, string b, double expected)
        {
            Assert.True(Constants.EpsilonCompare(expected, Tuple4.DotProduct(cache[a], cache[b])));
        }

        [Then(@"magnitude\(([a-z][a-z0-9]*)\) = ([+-.0-9]+)")]
        public void Then_magnitude(string a, double expected)
        {
            Assert.True(Constants.EpsilonCompare(expected, cache[a].Length()));
        }

        [When(@"([a-z][a-z0-9]*) ← reflect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_reflect(string id, string v, string n)
        {
            cache[id] = Tuple4.Reflect(cache[v], cache[n]);
        }

        [When(@"([a-z][a-z0-9]*) ← normalize\(([a-z][a-z0-9]*)\)")]
        public void When_normalize(string id, string v)
        {
            cache[id] = Tuple4.Normalize(cache[v]);
        }

        [And(@"([a-z][a-z0-9]*) is not a point")]
        [And(@"([a-z][a-z0-9]*) is a vector")]
        public void Then_not_a_point(string a)
        {
            Then_w(a, 0);
        }

        [And(@"([a-z][a-z0-9]*) is not a vector")]
        [And(@"([a-z][a-z0-9]*) is a point")]
        public void Then_a_point(string a)
        {
            Then_w(a, 1);
        }

        [Then(@"cross\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"cross\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_cross(string a, string b, double t1, double t2, double t3)
        {
            var c = Tuple4.CrossProduct(cache[a], cache[b]);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
        }

        [Then(@"normalize\(([a-z][a-z0-9]*)\) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [Then(@"normalize\(([a-z][a-z0-9]*)\) = approximately vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_normalize(string a, double t1, double t2, double t3)
        {
            var c = Tuple4.Normalize(cache[a]);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
        }

        [Then(@"([a-z][a-z0-9]*) = tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_tuple(string a, double t1, double t2, double t3, double t4)
        {
            var c = cache[a];
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
            Assert.True(Constants.EpsilonCompare(t4, c.W));
        }

        [Then(@"([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_vector(string a, double t1, double t2, double t3)
        {
            var c = cache[a];
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
            Assert.True(c.IsVector());
        }

        [Then(@"([a-z][a-z0-9]*) \+ ([a-z][a-z0-9]*) = tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_plus4(string a, string b, double t1, double t2, double t3, double t4)
        {
            var c = Tuple4.Add(cache[a], cache[b]);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
            Assert.True(Constants.EpsilonCompare(t4, c.W));
        }

        [Then(@"([a-z][a-z0-9]*) \+ ([a-z][a-z0-9]*) = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_plus3(string a, string b, double t1, double t2, double t3)
        {
            var c = Tuple4.Add(cache[a], cache[b]);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
        }

        [Then(@"([a-z][a-z0-9]*) - ([a-z][a-z0-9]*) = tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_minus4(string a, string b, double t1, double t2, double t3, double t4)
        {
            var c = Tuple4.Subtract(cache[a], cache[b]);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
            Assert.True(Constants.EpsilonCompare(t4, c.W));
        }

        [Then(@"([a-z][a-z0-9]*) - ([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [Then(@"([a-z][a-z0-9]*) - ([a-z][a-z0-9]*) = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_minus3(string a, string b, double t1, double t2, double t3)
        {
            var c = Tuple4.Subtract(cache[a], cache[b]);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
            Assert.True(c.IsVector());
        }

        [Then(@"([a-z][a-z0-9]*) - ([a-z][a-z0-9]*) = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_minus_points(string a, string b, double t1, double t2, double t3)
        {
            var c = Tuple4.Subtract(cache[a], cache[b]);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
            Assert.True(c.IsPoint());
        }

        [Then(@"([a-z][a-z0-9]*) \* ([a-z][a-z0-9]*) = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_scale_vector(string a, string b, double t1, double t2, double t3)
        {
            var c = Tuple4.Scale(cache[a], cache[b]);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
            Assert.True(c.IsVector());
        }

        [Then(@"([a-z][a-z0-9]*) \* ([+-.0-9]+) = tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_scale_scalar4(string a, double s, double t1, double t2, double t3, double t4)
        {
            var c = Tuple4.Scale(cache[a], s);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
            Assert.True(Constants.EpsilonCompare(t4, c.W));
        }

        [Then(@"([a-z][a-z0-9]*) \* ([+-.0-9]+) = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_scale_scalar3(string a, double s, double t1, double t2, double t3)
        {
            var c = Tuple4.Scale(cache[a], s);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
        }

        [Then(@"([a-z][a-z0-9]*) / ([+-.0-9]+) = tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_scale_div_scalar4(string a, double s, double t1, double t2, double t3, double t4)
        {
            var c = Tuple4.Scale(cache[a], 1 / s);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
            Assert.True(Constants.EpsilonCompare(t4, c.W));
        }

        [Then(@"-([a-z][a-z0-9]*) = tuple\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_scale_scalar(string a, double t1, double t2, double t3, double t4)
        {
            var c = Tuple4.Negate(cache[a]);
            Assert.True(Constants.EpsilonCompare(t1, c.X));
            Assert.True(Constants.EpsilonCompare(t2, c.Y));
            Assert.True(Constants.EpsilonCompare(t3, c.Z));
            Assert.True(Constants.EpsilonCompare(t4, c.W));
        }

        [Then(@"magnitude\(([a-z][a-z0-9]*)\) = ([+-.0-9]+)")]
        public void Then_scale_magnitude(string a, double d)
        {
            var c = cache[a].Length();
            Assert.True(Constants.EpsilonCompare(c, d));
        }
    }
}
