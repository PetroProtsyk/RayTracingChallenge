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
using Protsyk.RayTracer.Challenge.Core.Scene.Cameras;
using Protsyk.RayTracer.Challenge.Core.Geometry;

namespace Protsyk.RayTracer.Challenge.Core.UnitTests
{
    public class CameraTests
    {
        private readonly ITestOutputHelper output;

        public CameraTests(ITestOutputHelper testOutputHelper)
        {
            this.output = testOutputHelper;
        }

        [Fact]
        public void ParallelCameraTest()
        {
            var camera = new ParCamera(-5.0, 1.0, 600, 800);

            // Ray cast for screen coordinate 0,0
            {
                var ray = camera.GetRay(0, 0);
                var expectedRay = new Ray(
                        new Tuple4(-0.5, 0.5, -5.0, TupleFlavour.Point),
                        new Tuple4(0.0, 0.0, 1.0, TupleFlavour.Vector));
                Assert.Equal(expectedRay.origin, ray.origin);
                Assert.Equal(expectedRay.dir, ray.dir);
            }

            // Ray cast for screen coordinate width,height
            {
                var ray = camera.GetRay(600, 800);
                var expectedRay = new Ray(
                        new Tuple4(0.5, -0.5, -5.0, TupleFlavour.Point),
                        new Tuple4(0.0, 0.0, 1.0, TupleFlavour.Vector));
                Assert.Equal(expectedRay.origin, ray.origin);
                Assert.Equal(expectedRay.dir, ray.dir);
            }
        }

        [Fact]
        public void SimpleCameraTest()
        {
            // Ray cast for screen coordinate 0,0
            {
                var camera = new SimpleCamera(new Tuple4(0.0, 0.0, -4.0, TupleFlavour.Point), 6.0, 600, 800);
                var ray = camera.GetRay(0, 0);
                var expectedRay = new Ray(
                        new Tuple4(0.0, 0.0, -4.0, TupleFlavour.Point),
                        Tuple4.Normalize(new Tuple4(-3.0, 3.0, 1.0, TupleFlavour.Vector)));
                Assert.Equal(expectedRay.origin, ray.origin);
                Assert.Equal(expectedRay.dir, ray.dir);

                var t = Tuple4.Add(ray.origin, Tuple4.Scale(ray.dir, 4.35890));
                Assert.Equal(new Tuple4(-3.0, 3.0, -3.0, TupleFlavour.Point), t);
            }

            // Ray cast for screen coordinate width,height
            {
                var camera = new SimpleCamera(new Tuple4(0.0, 0.0, -4.0, TupleFlavour.Point), 6.0, 600, 800);
                var ray = camera.GetRay(600, 800);
                var expectedRay = new Ray(
                        new Tuple4(0.0, 0.0, -4.0, TupleFlavour.Point),
                        Tuple4.Normalize(new Tuple4(3.0, -3.0, 1.0, TupleFlavour.Vector)));
                Assert.Equal(expectedRay.origin, ray.origin);
                Assert.Equal(expectedRay.dir, ray.dir);
            }

            {
                var camera = new SimpleCamera(new Tuple4(0, 0, -1.0, TupleFlavour.Point), 2.0, 600, 600);
                var ray = camera.GetRay(0, 0);

                var t = Tuple4.Add(ray.origin, Tuple4.Scale(ray.dir, 1.73205));
                Assert.Equal(new Tuple4(-1.0, 1.0, 0.0, TupleFlavour.Point), t);
            }

        }

    }
}
