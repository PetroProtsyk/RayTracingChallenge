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
using Protsyk.RayTracer.Challenge.Core.Canvas;
using Protsyk.RayTracer.Challenge.Core.Geometry;

using DocString = Gherkin.Ast.DocString;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials.Patterns;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;
using Protsyk.RayTracer.Challenge.Core.Scene;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/patterns.feature")]
    public class PatternsTest : Feature
    {
        private readonly IDictionary<string, Tuple4> colors = new Dictionary<string, Tuple4>();
        private readonly IDictionary<string, IColorPattern> patterns = new Dictionary<string, IColorPattern>();
        private readonly IDictionary<string, TestSphere> figure = new Dictionary<string, TestSphere>();

        private readonly ITestOutputHelper testOutputHelper;

        public PatternsTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][a-z0-9]*) ← color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Color_id(string id, double t1, double t2, double t3)
        {
            colors[id] = new Tuple4(t1, t2, t3, TupleFlavour.Vector);
        }

        [Given(@"([a-z][a-z0-9]*) ← stripe_pattern\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← stripe_pattern\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void Color_stripe_pattern(string id, string c1, string c2)
        {
            patterns[id] = new StripePattern(colors[c1], colors[c2]);
        }

        [Then(@"([a-z][a-z0-9]*).(a|b) = ([a-z][a-z0-9]*)")]
        [ And(@"([a-z][a-z0-9]*).(a|b) = ([a-z][a-z0-9]*)")]
        public void Then_stripe_color(string id, string p, string c)
        {
            switch(p)
            {
                case "a":
                    Assert.Equal(colors[c], ((StripePattern)patterns[id]).ColorA);
                    break;
                case "b":
                    Assert.Equal(colors[c], ((StripePattern)patterns[id]).ColorB);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        [Then(@"([a-z][a-z0-9]*) = (white|black)")]
        [ And(@"([a-z][a-z0-9]*) = (white|black)")]
        public void Then_color(string c, string expected)
        {
            Assert.Equal(colors[expected], colors[c]);
        }


        [Then(@"stripe_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\) = ([a-z][a-z0-9]*)")]
        [ And(@"stripe_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\) = ([a-z][a-z0-9]*)")]
        public void Then_stripe_at(string id, double x, double y, double z, string cId)
        {
            Assert.Equal(colors[cId], ((StripePattern)patterns[id]).GetColor(new Tuple4(x, y, z, TupleFlavour.Point)));
        }

        [Given(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        [And(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        public void Given_sphere(string id)
        {
            figure[id] = new TestSphere(MatrixOperations.Identity(4), MaterialConstants.Default);
        }

        [And(@"set_transform\(([a-z][a-z0-9]*), scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_set_object_scale_transform(string figureId, double t1, double t2, double t3)
        {
            figure[figureId].Transformation = MatrixOperations.Geometry3D.Scale(t1, t2, t3);
        }

        [And(@"set_pattern_transform\(([a-z][a-z0-9]*), scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_set_pattern_scale_transform(string id, double t1, double t2, double t3)
        {
            ((StripePattern)patterns[id]).Transformation = MatrixOperations.Geometry3D.Scale(t1, t2, t3);
        }

        [And(@"set_pattern_transform\(([a-z][a-z0-9]*), translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_set_pattern_translation_transform(string id, double t1, double t2, double t3)
        {
            ((StripePattern)patterns[id]).Transformation = MatrixOperations.Geometry3D.Translation(t1, t2, t3);
        }



        [When(@"([a-z][a-z0-9]*) ← stripe_at_object\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Then_stripe_at_object(string cId, string pId, string oId, double x, double y, double z)
        {
            TestSphere f = figure[oId];
            IMaterial m = f.Material;
            IMaterial pm = new PatternMaterial(patterns[pId], m.Ambient, m.Diffuse, m.Specular, m.Shininess, m.Reflective, m.RefractiveIndex, m.Transparency);
            colors[cId] = pm.GetColor(f.TransformToObjectPoint(new Tuple4(x, y, z, TupleFlavour.Point)));
        }

        public class TestSphere : SphereFigure
        {
            public TestSphere(IMatrix transformation, IMaterial material)
                : base(transformation, material)
            {
            }

            public Tuple4 TransformToObjectPoint(Tuple4 worldPoint)
            {
                return base.TransformWorldPointToObjectPoint(worldPoint);
            }
        }

    }
}

