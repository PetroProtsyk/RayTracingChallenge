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
        private readonly IDictionary<string, IFigure> figure = new Dictionary<string, IFigure>();
        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();
        private readonly IDictionary<string, HitResult[]> hits = new Dictionary<string, HitResult[]>();
        private readonly IDictionary<string, Intersection> intersection = new Dictionary<string, Intersection>();
        private readonly IDictionary<string, Intersection[]> intersections = new Dictionary<string, Intersection[]>();
        private readonly IDictionary<string, HitResult> hit = new Dictionary<string, HitResult>();
        private readonly IDictionary<string, double> refractiveIndex = new Dictionary<string, double>();
        private readonly IDictionary<string, Tuple4> colors = new Dictionary<string, Tuple4>();
        private readonly IDictionary<string, Tuple4> tuples = new Dictionary<string, Tuple4>();

        private readonly ITestOutputHelper testOutputHelper;

        private static readonly string ComputationsId = "comps";

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
            worlds.Add(id, new BaseScene().WithColorModel(ColorModel.WhiteNormalized));
        }

        [Given(@"([a-z][a-z0-9]*) ← default_world\(\)")]
        [When(@"([a-z][a-z0-9]*) ← default_world\(\)")]
        public void Given_default_world(string id)
        {
            worlds.Add(id, CreateDefault());
        }

        public static BaseScene CreateDefault()
        {
            var defaultMaterial = MaterialConstants.Default;
            var defaultWorld = new BaseScene()
                .WithFigures(
                    new SphereFigure(
                            Matrix4x4.Identity,
                            new SolidColorMaterial(new Tuple4(0.8, 1.0, 0.6, TupleFlavour.Vector),
                                                   defaultMaterial.Ambient,
                                                   0.7,
                                                   0.2,
                                                   defaultMaterial.Shininess,
                                                   defaultMaterial.Reflective,
                                                   defaultMaterial.RefractiveIndex,
                                                   defaultMaterial.Transparency)),
                    new SphereFigure(
                            MatrixOperations.Geometry3D.Scale(0.5, 0.5, 0.5),
                            defaultMaterial)
                )
                .WithLights(new SpotLight(ColorModel.WhiteNormalized, Tuple4.Point(-10, 10, -10), 1.0),
                            new AmbientLight(1.0))
                .WithShadows(true)
                .WithColorModel(ColorModel.WhiteNormalized);

            return defaultWorld;
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

        [And(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        public void And_defaul_sphere(string id)
        {
            figure[id] = new SphereFigure(Matrix4x4.Identity, MaterialConstants.Default);
        }

        [And(@"([a-z][a-z0-9]*) ← sphere\(\) with:")]
        public void And_sphere(string id, DataTable dataTable)
        {
            And_figure(id, "sphere", dataTable);
        }

        [And(@"([a-z][a-z0-9]*) ← plane\(\) with:")]
        public void And_plane(string id, DataTable dataTable)
        {
            And_figure(id, "plane", dataTable);
        }

        private void And_figure(string id, string figureType, DataTable dataTable)
        {
            IMatrix transformation = Matrix4x4.Identity;
            var defaultMaterial = MaterialConstants.Default;

            Tuple4 color = defaultMaterial.GetColor(Tuple4.ZeroPoint);
            double ambient = defaultMaterial.Ambient;
            double diffuse = defaultMaterial.Diffuse;
            double specular = defaultMaterial.Specular;
            double reflective = defaultMaterial.Reflective;
            double refractiveIndex = defaultMaterial.RefractiveIndex;
            double transparency = defaultMaterial.Transparency;

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
                    case "material.ambient":
                        ambient = double.Parse(cells[1].Value);
                        break;
                    case "material.diffuse":
                        diffuse = double.Parse(cells[1].Value);
                        break;
                    case "material.specular":
                        specular = double.Parse(cells[1].Value);
                        break;
                    case "material.reflective":
                        reflective = double.Parse(cells[1].Value);
                        break;
                    case "material.transparency":
                        transparency = double.Parse(cells[1].Value);
                        break;
                    case "material.refractive_index":
                        refractiveIndex = double.Parse(cells[1].Value);
                        break;
                    case "transform":
                        {
                            var entries = cells[1].Value
                                .Split(new char[] { ',', ')', '(', ' ' }, System.StringSplitOptions.RemoveEmptyEntries)
                                .ToArray();

                            switch (entries[0])
                            {
                                case "scaling":
                                    transformation = MatrixOperations.Multiply(transformation,
                                        MatrixOperations.Geometry3D.Scale(
                                            double.Parse(entries[1]),
                                            double.Parse(entries[2]),
                                            double.Parse(entries[3])));
                                    break;
                                case "translation":
                                    transformation = MatrixOperations.Multiply(transformation,
                                        MatrixOperations.Geometry3D.Translation(
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
                        throw new NotImplementedException(cells[0].Value);
                }
            }

            var material = new SolidColorMaterial(color,
                                                    ambient,
                                                    diffuse,
                                                    specular,
                                                    defaultMaterial.Shininess,
                                                    reflective,
                                                    refractiveIndex,
                                                    transparency);

            switch (figureType)
            {
                case "sphere":
                    figure[id] = new SphereFigure(transformation, material);
                    break;
                case "plane":
                    figure[id] = new PlaneFigure(transformation, material);
                    break;
                default:
                    throw new NotImplementedException(figureType);
            }

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
                        .CalculateAllIntersectionsSorted(ray[rayId].origin, ray[rayId].dir)
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

        [And(@"([A-Za-z][a-z0-9]*) ← the (first|second) object in ([a-z][a-z0-9]*)")]
        public void And_shape(string id, string order, string wId)
        {
            int skip;
            switch (order)
            {
                case "first":
                    skip = 0;
                    break;
                case "second":
                    skip = 1;
                    break;
                default:
                    throw new NotSupportedException();
            }
            var shape = worlds[wId].Figures.Skip(skip).FirstOrDefault();
            figure[id] = shape;
        }

        [And(@"([a-z][a-z0-9]*) is added to ([a-z][a-z0-9]*)")]
        public void And_shape_is_added_to_world(string id, string wId)
        {
            worlds[wId].Add(figure[id]);
        }

        [When(@"([a-z][a-z0-9]*) ← intersection\(([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← intersection\(([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        public void When_intersection(string id, double t, string figureId)
        {
            intersection[id] = new Intersection(t, figure[figureId]);
        }

        [When(@"comps ← prepare_computations\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_prepare_computations(string id, string rayId)
        {
            var i = intersection[id];
            var r = ray[rayId];
            var h = i.figure
                    .AllHits(r.origin, r.dir)
                    .FirstOrDefault(hit => Core.Constants.EpsilonCompare(hit.Distance, i.t));

            hit[ComputationsId] = h;
        }

        [And(@"([a-z][a-z0-9]*) ← shade_hit\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void And_shade_hit(string id, string wId, string hId)
        {
            var w = worlds[wId];
            var h = hit[hId];

            var shadedColor = w.CalculateColorAt(h, 0.0, 0.0);
            colors[id] = shadedColor;
        }

        [And(@"([a-z][a-z0-9]*) ← shade_hit\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([0-9]+)\)")]
        public void And_shade_hit_with_recursion(string id, string wId, string hId, int depth)
        {
            var w = worlds[wId];
            var h = hit[hId];

            var shadedColor = w.WithRecursionDepth(depth).CalculateColorAt(h, refractiveIndex["n1"], refractiveIndex["n2"]);
            colors[id] = shadedColor;
        }

        [Then(@"([a-z][a-z0-9]*) = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_color(string id, double r, double g, double b)
        {
            var c = colors[id];
            Assert.Equal(Tuple4.Vector(r, g, b), c);
        }

        [And(@"([a-z][a-z0-9]*).light ← point_light\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void And_replace_light(string id, double t1, double t2, double t3, double c1, double c2, double c3)
        {
            var pointLight = new SpotLight(ColorModel.WhiteNormalized, new Tuple4(t1, t2, t3, TupleFlavour.Point), 1.0);
            worlds[id].ClearLights();
            worlds[id].AddLight(pointLight);
            worlds[id].AddLight(new AmbientLight(1.0));
        }

        [When(@"([a-z][a-z0-9]*) ← color_at\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_color(string id, string wId, string rId)
        {
            colors[id] = color_at(worlds[wId], ray[rId]);
        }

        [And(@"([a-z][a-z0-9]*).material.ambient ← ([+-.0-9]+)")]
        public void Replace_material_color(string id, double ambient)
        {
            var f = figure[id];

            IMaterial m = f.Material;
            f.Material = new SolidColorMaterial(
                m.GetColor(Tuple4.ZeroPoint),
                ambient,
                m.Diffuse,
                m.Specular,
                m.Shininess,
                m.Reflective,
                m.RefractiveIndex,
                m.Transparency);
        }

        [Then(@"([a-z][a-z0-9]*) = ([a-z][a-z0-9]*).material.color")]
        public void Then_color_as_figure_material(string id, string fId)
        {
            var c = colors[id];
            var mc = figure[fId].Material.GetColor(Tuple4.ZeroPoint);
            Assert.Equal(mc, c);
        }

        [And(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Point_id(string id, double t1, double t2, double t3)
        {
            tuples.Add(id, new Tuple4(t1, t2, t3, TupleFlavour.Point));
        }

        [Then(@"is_shadowed\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\) is (true|false)")]
        public void Then_is_shadowed(string wid, string pId, bool value)
        {
            Assert.Equal(value, worlds[wid].IsShadowed(tuples[pId]));
        }

        [And(@"([a-z][a-z0-9]*) ← reflected_color\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void And_reflected_color(string id, string wId, string hId)
        {
            var w = worlds[wId];
            var h = hit[hId];

            var shadedColor = w.CalculateReflectedColorAt(h);
            colors[id] = shadedColor;
        }

        [And(@"([a-z][a-z0-9]*) ← reflected_color\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([0-9]+)\)")]
        public void And_reflected_color_with_recursion(string id, string wId, string hId, int depth)
        {
            var w = worlds[wId];
            var h = hit[hId];

            var shadedColor = w.WithRecursionDepth(depth).CalculateReflectedColorAt(h);
            colors[id] = shadedColor;
        }

        [Then(@"color_at\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\) should terminate successfully")]
        public void Then_color_terminates(string wId, string rId)
        {
            Assert.NotNull(color_at(worlds[wId], ray[rId]));
        }

        private Tuple4 color_at(BaseScene w, Ray r)
        {
            return w.CastRay(r);
        }

        [And(@"([a-z][a-z0-9]*) ← intersections\(([-0-9a-zA-Z.,: ]+)+\)")]
        public void When_intersections_opts(string id, string ixs)
        {
            var li = new List<Intersection>();
            foreach (var si in ixs.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var tk = si.Split(':');
                li.Add(new Intersection(double.Parse(tk[0]), figure[tk[1]]));
            }
            intersections[id] = li.ToArray();
        }

        [When(@"comps ← prepare_computations\(xs\[([0-9]+)\], ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_prepare_computations_with_xs(int index, string rayId, string xs)
        {
            var ixs = intersections[xs];
            var i = ixs[index];
            var r = ray[rayId];

            var comps = IntersectionTest.prepareComputations(r, i, ixs);

            hit[ComputationsId] = comps.hit;
            refractiveIndex["n1"] = comps.refractiveIndexEntering;
            refractiveIndex["n2"] = comps.refractiveIndexExiting;
        }

        [And(@"([a-z][a-z0-9]*) ← refracted_color\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([0-9]+)\)")]
        public void And_refracted_color_with_recursion(string id, string wId, string hId, int depth)
        {
            var w = worlds[wId];
            var h = hit[hId];
            var refractiveIndexEntering = refractiveIndex["n1"];
            var refractiveIndexExiting = refractiveIndex["n2"];

            var shadedColor = w.WithRecursionDepth(depth).CalculateRefractedColorAt(h, refractiveIndexEntering, refractiveIndexExiting);
            colors[id] = shadedColor;
        }

        [And(@"([A-Za-z][a-z0-9]*) has:")]
        public void And_shape_has(string id, DataTable dataTable)
        {
            IFigure shape = figure[id];
            double ambient = shape.Material.Ambient;
            double transparency = shape.Material.Transparency;
            double refractiveIndex = shape.Material.RefractiveIndex;
            bool hasPattern = false;

            foreach (var row in dataTable.Rows)
            {
                var cells = row.Cells.ToArray();
                switch (cells[0].Value)
                {
                    case "material.ambient":
                        ambient = double.Parse(cells[1].Value);
                        break;
                    case "material.transparency":
                        transparency = double.Parse(cells[1].Value);
                        break;
                    case "material.refractive_index":
                        refractiveIndex = double.Parse(cells[1].Value);
                        break;
                    case "material.pattern":
                        if (!cells[1].Value.Equals("test_pattern()"))
                        {
                            throw new NotSupportedException();
                        }
                        hasPattern = true;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            var material = hasPattern ? (IMaterial)new PatternMaterial(new PatternsTest.TestPattern(),
                                                    ambient,
                                                    shape.Material.Diffuse,
                                                    shape.Material.Specular,
                                                    shape.Material.Shininess,
                                                    shape.Material.Reflective,
                                                    refractiveIndex,
                                                    transparency) :
                                        (IMaterial)new SolidColorMaterial(shape.Material.GetColor(Tuple4.ZeroPoint),
                                                    ambient,
                                                    shape.Material.Diffuse,
                                                    shape.Material.Specular,
                                                    shape.Material.Shininess,
                                                    shape.Material.Reflective,
                                                    refractiveIndex,
                                                    transparency);
            shape.Material = material;
        }
    }
}
