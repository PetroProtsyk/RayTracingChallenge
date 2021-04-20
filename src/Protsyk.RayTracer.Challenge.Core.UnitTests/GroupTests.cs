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
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;
using Protsyk.RayTracer.Challenge.Core.Scene;

namespace Protsyk.RayTracer.Challenge.Core.UnitTests
{
    public class GroupTests
    {
        private readonly ITestOutputHelper output;

        public GroupTests(ITestOutputHelper testOutputHelper)
        {
            this.output = testOutputHelper;
        }

        [Fact]
        public void GroupManipulations()
        {
            var s1 = new SphereFigure(Matrix4x4.Identity, MaterialConstants.Default);
            var s2 = new CubeFigure(Matrix4x4.Identity, MaterialConstants.Default);
            var s3 = new PlaneFigure(Matrix4x4.Identity, MaterialConstants.Default);

            var g = new GroupFigure();
            var g2 = new GroupFigure();

            s1.Parent = g;
            s2.Parent = g;
            g.Add(s3);

            Assert.Equal(g, s1.Parent);
            Assert.Equal(g, s2.Parent);
            Assert.Equal(g, s3.Parent);

            Assert.Contains(s1, g.Figures);
            Assert.Contains(s2, g.Figures);
            Assert.Contains(s3, g.Figures);

            g.Remove(s1);
            s3.Parent = null;

            Assert.Null(s1.Parent);
            Assert.Equal(g, s2.Parent);
            Assert.Null(s3.Parent);

            Assert.DoesNotContain(s1, g.Figures);
            Assert.Contains(s2, g.Figures);
            Assert.DoesNotContain(s3, g.Figures);

            s2.Parent = g2;

            Assert.DoesNotContain(s2, g.Figures);
            Assert.Equal(g2, s2.Parent);
            Assert.Contains(s2, g2.Figures);
        }

    }
}
