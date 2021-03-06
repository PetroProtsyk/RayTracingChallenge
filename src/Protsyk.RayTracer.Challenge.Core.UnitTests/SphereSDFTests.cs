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
using Protsyk.RayTracer.Challenge.Core.Geometry.SignedDistanceFields;

namespace Protsyk.RayTracer.Challenge.Core.UnitTests
{
    public class SphereSDFTests
    {
        private readonly ITestOutputHelper output;

        public SphereSDFTests(ITestOutputHelper testOutputHelper)
        {
            this.output = testOutputHelper;
        }

        [Fact]
        public void SphereSDFIntersection()
        {
            var s = new SphereSDF();
            var r = new Ray(new Tuple4(0, 0, -2, TupleFlavour.Point),
                            new Tuple4(0, 0, 1, TupleFlavour.Vector));

            var actual = s.GetIntersections(r);
            Assert.NotNull(actual);
            Assert.Single(actual);

            Assert.True(Constants.EpsilonCompare(1, actual[0]));
        }
    }
}
