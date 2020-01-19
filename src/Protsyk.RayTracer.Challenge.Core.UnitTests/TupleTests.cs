using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;

using Xunit;
using Xunit.Abstractions;

using Protsyk.RayTracer.Challenge.Core;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.UnitTests
{
    public class TupleTests
    {
        private readonly ITestOutputHelper output;

        public TupleTests(ITestOutputHelper testOutputHelper)
        {
            this.output = testOutputHelper;
        }

        [Fact]
        public void TupleAddTest()
        {
            var t1 = new Tuple4(1, 2, 3, TupleFlavour.Vector);
            var t2 = new Tuple4(4, 5, 6, TupleFlavour.Vector);

            var expected = new Tuple4(5, 7, 9, TupleFlavour.Vector);
            Assert.Equal(expected, Tuple4.Add(t1, t2));
        }

        [Fact]
        public void TupleSubtractTest()
        {
            var t1 = new Tuple4(1, 7, 3, TupleFlavour.Vector);
            var t2 = new Tuple4(4, 5, 6, TupleFlavour.Vector);

            var expected = new Tuple4(-3, 2, -3, TupleFlavour.Vector);
            Assert.Equal(expected, Tuple4.Subtract(t1, t2));
        }

        [Fact]
        public void TupleScaleTest()
        {
            var t1 = new Tuple4(1, 7, 3, TupleFlavour.Vector);
            var t2 = new Tuple4(4, 5, 6, TupleFlavour.Vector);

            var expected = new Tuple4(4, 35, 18, TupleFlavour.Vector);
            Assert.Equal(expected, Tuple4.Scale(t1, t2));
        }

        [Fact]
        public void TupleScaleByTest()
        {
            var t1 = new Tuple4(1, 7, 3, TupleFlavour.Vector);

            var expected = new Tuple4(0.5, 3.5, 1.5, TupleFlavour.Vector);
            Assert.Equal(expected, Tuple4.Scale(t1, 0.5));
        }

        [Fact]
        public void TupleNegateTest()
        {
            var t1 = new Tuple4(1, 7, 3, TupleFlavour.Vector);

            var expected = new Tuple4(-1, -7, -3, TupleFlavour.Vector);
            Assert.Equal(expected, Tuple4.Negate(t1));
        }

        [Fact]
        public void TupleNormalizeTest()
        {
            var t1 = new Tuple4(1, 2, 3, TupleFlavour.Vector);
            var len = t1.Length();
            Assert.Equal(Math.Sqrt(14), len);

            var expected = new Tuple4(1.0/len, 2.0/len, 3.0/len, TupleFlavour.Vector);
            Assert.Equal(expected, Tuple4.Normalize(t1));
        }

        [Fact]
        public void TupleDotProductTest()
        {
            var t1 = new Tuple4(-1, 7, 3, TupleFlavour.Vector);
            var t2 = new Tuple4(4, 5, 6, TupleFlavour.Vector);

            Assert.Equal(49.0, Tuple4.DotProduct(t1, t2));
        }

        [Fact]
        public void TupleCrossProductTest()
        {
            var t1 = new Tuple4(1, 7, 3, TupleFlavour.Vector);
            var t2 = new Tuple4(4, 5, 6, TupleFlavour.Vector);

            var expected = new Tuple4(27, 6, -23, TupleFlavour.Vector);
            Assert.Equal(expected, Tuple4.CrossProduct(t1, t2));
        }

        [Fact]
        public void TupleReflectTest()
        {
            var t1 = new Tuple4(1, 7, 3, TupleFlavour.Vector);
            var t2 = new Tuple4(4, 5, 6, TupleFlavour.Vector);

            var expected = new Tuple4(-455, -563, -681, TupleFlavour.Vector);
            Assert.Equal(expected, Tuple4.Reflect(t1, t2));
        }

    }
}
