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
        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();

        private readonly IDictionary<string, IFigure> figure = new Dictionary<string, IFigure>();

        private readonly IDictionary<string, HitResult[]> intersection = new Dictionary<string, HitResult[]>();

        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly IDictionary<string, IMatrix> matrix = new Dictionary<string, IMatrix>();

        private readonly IDictionary<string, IMaterial> material = new Dictionary<string, IMaterial>();

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

    }
}
