using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;
using Protsyk.RayTracer.Challenge.Core.Scene.Lights;
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using DataTable = Gherkin.Ast.DataTable;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/world.feature")]
    public class WorldTests : Feature
    {
        private readonly IDictionary<string, BaseScene> worlds = new Dictionary<string, BaseScene>();
        private readonly IDictionary<string, ILight> lights = new Dictionary<string, ILight>();
        private readonly IDictionary<string, SphereFigure> figure = new Dictionary<string, SphereFigure>();
        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();
        private readonly IDictionary<string, HitResult[]> hits = new Dictionary<string, HitResult[]>();

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

        [Given(@"([a-z][a-z0-9]*) ← default_world\(\)")]
        [When(@"([a-z][a-z0-9]*) ← default_world\(\)")]
        public void Given_default_world(string id)
        {
            var defaultWorld = new BaseScene()
                .WithFigures(new SphereFigure(
                        Matrix4x4.Identity,
                        new SolidColorMaterial(new Tuple4(0.8, 1.0, 0.6, TupleFlavour.Vector), 1.0, 0.7, 0.2, 0.0, 0.0, 0.0, 0.0)),
                    new SphereFigure(
                        MatrixOperations.Geometry3D.Scale(0.5, 0.5, 0.5),
                        new SolidColorMaterial(new Tuple4(1.0, 1.0, 1.0, TupleFlavour.Vector), 1.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0))
                )
                .WithLights(new SpotLight(ColorModel.WhiteNormalized, new Tuple4(-10, 10, -10, TupleFlavour.Point), 1.0));

            worlds.Add(id, defaultWorld);
        }

        [Then(@"([a-z][a-z0-9]*) contains no objects")]
        public void Then_world_contains_no_objects(string id)
        {
            Assert.Empty(worlds[id].Figures);
        }

        [And(@"([a-z][a-z0-9]*) has no light source")]
        public void Then_world_has_no_lights(string id)
        {
            Assert.Empty(worlds[id].Lights);
        }

        [Given(@"([a-z][a-z0-9]*) ← point_light\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_point_light(string id, double t1, double t2, double t3, double c1, double c2, double c3)
        {
            var pointLight = new SpotLight(ColorModel.WhiteNormalized, new Tuple4(t1, t2, t3, TupleFlavour.Point), 1.0);
            lights.Add(id, pointLight);
        }

        [And(@"([a-z][a-z0-9]*) ← sphere\(\) with:")]
        public void And_sphere(string id, DataTable dataTable)
        {
            IMatrix transformation = Matrix4x4.Identity;
            Tuple4 color = new Tuple4(1.0, 1.0, 1.0, TupleFlavour.Vector);
            double diffuse = 0.0;
            double specular = 0.0;

            foreach (var row in dataTable.Rows)
            {
                var cells = row.Cells.ToArray();
                switch (cells[0].Value)
                {
                    case "material.color":
                        {
                            var entries = cells[1].Value
                                .Split(new char[] { ',', '(', ')', ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
                                .ToArray();

                            color = new Tuple4(double.Parse(entries[0]), double.Parse(entries[1]), double.Parse(entries[2]), TupleFlavour.Vector);
                            break;
                        }
                    case "material.diffuse":
                        diffuse = double.Parse(cells[1].Value);
                        break;
                    case "material.specular":
                        specular = double.Parse(cells[1].Value);
                        break;
                    case "transform":
                        {
                            var entries = cells[1].Value
                                .Split(new char[] { ',', ')', '(', ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
                                .ToArray();

                            switch(entries[0])
                            {
                                case "scaling":
                                    transformation = MatrixOperations.Multiply(transformation,
                                        MatrixOperations.Geometry3D.Scale(
                                            double.Parse(entries[1]),
                                            double.Parse(entries[2]),
                                            double.Parse(entries[3])));
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }

                            break;
                        }
                    default:
                        throw new NotImplementedException();
                }
            }

            figure[id] = new SphereFigure(transformation, new SolidColorMaterial(color, 1.0, diffuse, specular, 0.0, 0.0, 0.0, 0.0));
        }

        [Then(@"([a-z][a-z0-9]*).light = ([a-z][_a-z0-9]*)")]
        public void Then_light(string id, string lightId)
        {
           Assert.Contains(lights[lightId], worlds[id].Lights);
        }

        [And(@"([a-z][a-z0-9]*) contains ([a-z][_a-z0-9]*)")]
        public void Then_world_contains_figure(string id, string figureId)
        {
            Assert.Contains(figure[figureId], worlds[id].Figures);
        }

        [And(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_ray(string id,
                              double p1, double p2, double p3,
                              double v1, double v2, double v3)
        {
            ray[id] = new Ray(new Tuple4(p1, p2, p3, TupleFlavour.Point),
                              new Tuple4(v1, v2, v3, TupleFlavour.Vector));
        }

        [When(@"([a-z][a-z0-9]*) ← intersect_world\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_ray_intersects_world(string id, string worldId, string rayId)
        {
            var hit = worlds[worldId]
                        .CalculateAllIntersectionSorted(ray[rayId].origin, ray[rayId].dir)
                        .ToArray();
            hits[id] = hit;
        }

        [Then(@"([a-z][a-z0-9]*).count = ([+-.0-9]+)")]
        public void Then_hit_count(string id, int count)
        {
            Assert.Equal(count, hits[id].Length);
        }

        [And(@"([a-z][a-z0-9]*)\[([+-.0-9]+)\].t = ([+-.0-9]+)")]
        public void Then_hit_distance(string id, int index, double t)
        {
            Assert.True(Core.Constants.EpsilonCompare(t, hits[id][index].Distance));
        }
    }
}
