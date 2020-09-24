using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Protsyk.RayTracer.Challenge.Core;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/materials.feature")]
    public class MaterialsTest : Feature
    {
        private readonly IDictionary<string, BaseMaterial> materials = new Dictionary<string, BaseMaterial>();
        private readonly IDictionary<string, Tuple4> points = new Dictionary<string, Tuple4>();

        private readonly ITestOutputHelper testOutputHelper;

        public MaterialsTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }


        [Given(@"([a-z][a-z0-9]*) ← material\(\)")]
        public void Given_material(string id)
        {
            materials[id] = new BaseMaterial();
        }

        [Then(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_named_point(string id, double x, double y, double z)
        {
            points[id] = new Tuple4(x, y, z, TupleFlavour.Point);
        }

        [Then(@"([a-z][a-z0-9]*).color = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*).color = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_color(string id, double r, double g, double b)
        {
            var c = materials[id].Color;
            Assert.True(Constants.EpsilonCompare(r, c.X));
            Assert.True(Constants.EpsilonCompare(g, c.Y));
            Assert.True(Constants.EpsilonCompare(b, c.Z));
            Assert.True(c.IsPoint());
        }


        [Then(@"([a-z][a-z0-9]*).([a-z]*) = ([+-.0-9]+)")]
        [ And(@"([a-z][a-z0-9]*).([a-z]*) = ([+-.0-9]+)")]
        public void Then_property(string id, string propertyName, double value)
        {
            switch (propertyName)
            {
                case "ambient":
                    Assert.Equal(materials[id].Ambient, value);
                    break;
                case "diffuse":
                    Assert.Equal(materials[id].Diffuse, value);
                    break;
                case "specular":
                    Assert.Equal(materials[id].Specular, value);
                    break;
                case "shininess":
                    Assert.Equal(materials[id].Shininess, value);
                    break;
                case "reflective":
                    Assert.Equal(materials[id].Reflective, value);
                    break;
                default:
                    throw new Exception($"Unknown property {propertyName}");
            }
        }
    }
}
