using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Protsyk.RayTracer.Challenge.Core;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene;
using Protsyk.RayTracer.Challenge.Core.Scene.Lights;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/materials.feature")]
    public class MaterialsTest : Feature
    {
        private readonly IDictionary<string, SolidColorMaterial> materials = new Dictionary<string, SolidColorMaterial>();
        private readonly IDictionary<string, Tuple4> points = new Dictionary<string, Tuple4>();
        private readonly IDictionary<string, Tuple4> vectors = new Dictionary<string, Tuple4>();
        private readonly IDictionary<string, Tuple4> colors = new Dictionary<string, Tuple4>();
        private readonly IDictionary<string, ILight> lights = new Dictionary<string, ILight>();
        private readonly IDictionary<string, bool> variables = new Dictionary<string, bool>();

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
            materials[id] = new SolidColorMaterial(); // Default Material
        }

        [Then(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_named_point(string id, double x, double y, double z)
        {
            points[id] = new Tuple4(x, y, z, TupleFlavour.Point);
        }

        [Then(@"([a-z][a-z0-9]*).color = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*).color = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_material_color(string id, double r, double g, double b)
        {
            var c = materials[id].Color;
            Assert.True(Constants.EpsilonCompare(r, c.X));
            Assert.True(Constants.EpsilonCompare(g, c.Y));
            Assert.True(Constants.EpsilonCompare(b, c.Z));
            Assert.True(c.IsVector());
        }

        [Then(@"([a-z][a-z0-9]*) = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_color(string id, double r, double g, double b)
        {
            var c = colors[id];
            Assert.True(Constants.EpsilonCompare(r, c.X));
            Assert.True(Constants.EpsilonCompare(g, c.Y));
            Assert.True(Constants.EpsilonCompare(b, c.Z));
            Assert.True(c.IsVector());
        }

        [Then(@"([a-z][a-z0-9]*).([_a-z]*) = ([+-.0-9]+)")]
        [ And(@"([a-z][a-z0-9]*).([_a-z]*) = ([+-.0-9]+)")]
        public void Then_property(string id, string propertyName, double value)
        {
            var material = materials[id];
            switch (propertyName)
            {
                case "ambient":
                    Assert.Equal(material.Ambient, value);
                    break;
                case "diffuse":
                    Assert.Equal(material.Diffuse, value);
                    break;
                case "specular":
                    Assert.Equal(material.Specular, value);
                    break;
                case "shininess":
                    Assert.Equal(material.Shininess, value);
                    break;
                case "reflective":
                    Assert.Equal(material.Reflective, value);
                    break;
                case "refractive_index":
                    Assert.Equal(material.RefractiveIndex, value);
                    break;
                case "transparency":
                    Assert.Equal(material.Transparency, value);
                    break;
                default:
                    throw new Exception($"Unknown property {propertyName}");
            }
        }

        [Given(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_vector(string id, double t1, double t2, double t3)
        {
            vectors.Add(id, new Tuple4(t1, t2, t3, TupleFlavour.Vector));
        }

        [And(@"([a-z][a-z0-9]*) ← point_light\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_point_light(string id, double t1, double t2, double t3, double c1, double c2, double c3)
        {
            var pointLight = new SpotLight(ColorModel.WhiteNormalized, new Tuple4(t1, t2, t3, TupleFlavour.Point), 1.0);
            lights.Add(id, pointLight);
        }

        [When(@"([a-z][a-z0-9]*) ← lighting\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_lighting(string id, string m, string light, string position, string eyev, string normalv)
        {
            colors[id] = shade_point(id, m, light, position, eyev, normalv, false);
        }

        [And(@"([a-z][_a-z0-9]*) ← (true|false)")]
        public void Given_value(string id, bool value)
        {
            variables[id] = value;
        }

        [When(@"([a-z][a-z0-9]*) ← lighting\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][_a-z0-9]*)\)")]
        public void When_lighting_with_shadow(string id, string m, string light, string position, string eyev, string normalv, string in_shadow)
        {
            colors[id] = shade_point(id, m, light, position, eyev, normalv, variables[in_shadow]);
        }

        private Tuple4 shade_point(string id, string m, string light, string position, string eyev, string normalv, bool in_shadow)
        {
            var shadedColor = Tuple4.ZeroVector;
            if (!in_shadow)
            {
                shadedColor = Tuple4.Add(shadedColor, lights[light].GetShadedColor(materials[m], vectors[eyev], points[position], vectors[normalv]));
            }
            shadedColor = Tuple4.Add(shadedColor, DirectionLightCommon.GetAmbientColor(materials[m]));
            return shadedColor;
        }

    }
}
