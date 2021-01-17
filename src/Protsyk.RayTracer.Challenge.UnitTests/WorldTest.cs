using Protsyk.RayTracer.Challenge.Core.Scene;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/world.feature")]
    public class WorldTests : Feature
    {
        private readonly IDictionary<string, BaseScene> worlds = new Dictionary<string, BaseScene>();

        private readonly ITestOutputHelper testOutputHelper;

        public WorldTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][a-z0-9]*) ← world\(\)")]
        public void Given_world(string id)
        {
            worlds.Add(id, new BaseScene());
        }

        [Then(@"([a-z][a-z0-9]*) contains no objects")]
        public void world_contains_no_objects(string id)
        {
            Assert.Empty(worlds[id].Figures);
        }

        [And(@"([a-z][a-z0-9]*) has no light source")]
        public void world_has_no_lights(string id)
        {
            Assert.Empty(worlds[id].Lights);
        }
    }
}
