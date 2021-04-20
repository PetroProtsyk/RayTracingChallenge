using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;
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
    [FeatureFile("./features/intersections.feature")]
    public class IntersectionTest : Feature
    {
        private readonly IDictionary<string, IFigure> figure = new Dictionary<string, IFigure>();

        private readonly IDictionary<string, HitResult> hit = new Dictionary<string, HitResult>();

        private readonly IDictionary<string, double> refractiveIndex = new Dictionary<string, double>();

        private readonly IDictionary<string, double> schlick = new Dictionary<string, double>();

        private readonly IDictionary<string, Intersection> intersection = new Dictionary<string, Intersection>();

        private readonly IDictionary<string, Intersection[]> intersections = new Dictionary<string, Intersection[]>();

        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();

        private readonly ITestOutputHelper testOutputHelper;

        private static readonly string ComputationsId = "comps";

        public IntersectionTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        [And(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        public void Given_sphere(string id)
        {
            figure[id] = new SphereFigure(MatrixOperations.Identity(4), MaterialConstants.Default);
        }

        [Given(@"([a-z][a-z0-9]*) ← plane\(\)")]
        public void Given_plane(string id)
        {
            figure[id] = new PlaneFigure(MatrixOperations.Identity(4), MaterialConstants.Default);
        }

        [Given(@"([a-z][a-z0-9]*) ← glass_sphere\(\)")]
        public void Given_glass_sphere(string id)
        {
            figure[id] = SphereTest.create_glass_sphere(MatrixOperations.Identity(4), 1.5);
        }


        [Given(@"([a-zA-Z][a-z0-9]*) ← (glass_sphere|sphere)\(\) with:")]
        [And(@"([a-zA-Z][a-z0-9]*) ← (glass_sphere|sphere)\(\) with:")]
        public void And_sphere(string id, string type, DataTable dataTable)
        {
            IMatrix transformation = Matrix4x4.Identity;
            var refractiveIndex = 1.0;

            foreach (var row in dataTable.Rows)
            {
                var cells = row.Cells.ToArray();
                switch (cells[0].Value)
                {
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
                        throw new NotImplementedException();
                }
            }

            switch (type)
            {
                case "sphere":
                    figure[id] = new SphereFigure(transformation, MaterialConstants.Default);
                    break;
                case "glass_sphere":
                    figure[id] = SphereTest.create_glass_sphere(transformation, refractiveIndex);
                    break;
                default:
                    throw new NotImplementedException(type);
            }
        }

        [When(@"([a-z][a-z0-9]*) ← intersection\(([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← intersection\(([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        public void When_intersection(string id, double t, string figureId)
        {
            intersection[id] = new Intersection(t, figure[figureId]);
        }

        [And(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*)\)")]
        public void When_intersections_1(string id, string i1)
        {
            intersections[id] = new Intersection[]
            {
                intersection[i1]
            };
        }

        [When(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersections_2(string id, string i1, string i2)
        {
            intersections[id] = new Intersection[]
            {
                intersection[i1],
                intersection[i2]
            };
        }

        [And(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersection_4(string id, string i1, string i2, string i3, string i4)
        {
            intersections[id] = new Intersection[]
            {
                intersection[i1],
                intersection[i2],
                intersection[i3],
                intersection[i4]
            };
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

        [When(@"([a-z][a-z0-9]*) ← hit\(([a-z][a-z0-9]*)\)")]
        public void When_hit(string id, string xs)
        {
            // Same as HitResult.ClosestPositiveHit
            intersection[id] = intersections[xs].Where(i => i.t > 0).OrderBy(i => i.t).FirstOrDefault();
        }

        [Then(@"([a-z][a-z0-9]*).t = ([+-.0-9]+)")]
        public void Then_origin(string id, double t)
        {
            Assert.Equal(t, intersection[id].t);
        }

        [And(@"([a-z][a-z0-9]*).object = ([a-z][a-z0-9]*)")]
        public void Then_object(string id, string figureId)
        {
            Assert.Equal(figure[figureId], intersection[id].figure);
        }

        [Then(@"([a-z][a-z0-9]*).count = ([+-.0-9]+)")]
        public void Then_intersect_count(string id, int v)
        {
            if (v == 0)
            {
                Assert.Null(intersections[id][0]);
            }
            else
            {
                Assert.Equal(v, intersections[id].Length);
            }
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\].t = ([+-.0-9]+)")]
        public void Then_intersect_distance(string id, int i, double v)
        {
            Assert.Equal(v, intersections[id][i].t);
        }

        [Then(@"([a-z][a-z0-9]*) = ([a-z][a-z0-9]*)")]
        public void Then_hit(string i1, string i2)
        {
            Assert.Equal(intersection[i1], intersection[i2]);
        }

        [Then(@"([a-z][a-z0-9]*) is nothing")]
        public void Then_no_hit(string i)
        {
            Assert.Null(intersection[i]);
        }

        [Given(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        [And(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_ray(string id,
                              double p1, double p2, double p3,
                              double v1, double v2, double v3)
        {
            ray[id] = new Ray(Tuple4.Point(p1, p2, p3), Tuple4.Vector(v1, v2, v3));
        }

        [When(@"comps ← prepare_computations\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_prepare_computations(string id, string rayId)
        {
            var i = intersection[id];
            var r = ray[rayId];

            var comps = prepareComputations(r, i, null);

            hit[ComputationsId] = comps.hit;
        }

        [When(@"comps ← prepare_computations\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_prepare_computations_with_intersections(string id, string rayId, string xs)
        {
            var ixs = intersections[xs];
            var i = intersection[id];
            var r = ray[rayId];

            var comps = prepareComputations(r, i, ixs);

            hit[ComputationsId] = comps.hit;
            refractiveIndex["n1"] = comps.refractiveIndexEntering;
            refractiveIndex["n2"] = comps.refractiveIndexExiting;
        }

        [When(@"comps ← prepare_computations\(([0-9]+), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_prepare_computations_with_intersections_array(int index, string rayId, string xs)
        {
            var ixs = intersections[xs];
            var i = ixs[index];
            var r = ray[rayId];

            var comps = prepareComputations(r, i, ixs);

            hit[ComputationsId] = comps.hit;
            refractiveIndex["n1"] = comps.refractiveIndexEntering;
            refractiveIndex["n2"] = comps.refractiveIndexExiting;
        }

        [When(@"comps ← prepare_computations\(xs\[([0-9]+)\], ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_prepare_computations_with_xs(int index, string rayId, string xs)
        {
            var ixs = intersections[xs];
            var i = ixs[index];
            var r = ray[rayId];

            var comps = prepareComputations(r, i, ixs);

            hit[ComputationsId] = comps.hit;
            refractiveIndex["n1"] = comps.refractiveIndexEntering;
            refractiveIndex["n2"] = comps.refractiveIndexExiting;
        }

        [Then(@"comps.t = ([a-z][a-z0-9]*).t")]
        public void Then_hit_t(string id)
        {
            Assert.Equal(intersection[id].t, hit[ComputationsId].Distance);
        }

        [And(@"comps.object = ([a-z][a-z0-9]*).object")]
        public void Then_hit_object(string id)
        {
            Assert.Equal(intersection[id].figure, hit[ComputationsId].Figure);
        }

        [Then(@"comps.point = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"comps.point = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_hit_point(double x, double y, double z)
        {
            Assert.Equal(Tuple4.Point(x, y, z), hit[ComputationsId].PointOnSurface);
        }

        [And(@"comps.eyev = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_hit_eyev(double x, double y, double z)
        {
            Assert.Equal(Tuple4.Vector(x, y, z), hit[ComputationsId].EyeVector);
        }

        [And(@"comps.normalv = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_hit_normalv(double x, double y, double z)
        {
            Assert.Equal(Tuple4.Vector(x, y, z), hit[ComputationsId].SurfaceNormal);
        }

        [Then(@"comps.inside = (true|false)")]
        [And(@"comps.inside = (true|false)")]
        public void Then_hit_inside(bool isInside)
        {
            Assert.Equal(isInside, hit[ComputationsId].IsInside);
        }

        [Then(@"comps.over_point.z < -EPSILON/2")]
        public void Then_over_point()
        {
            Assert.True(hit[ComputationsId].PointOverSurface.Z < -Core.Constants.HalfEpsilon);
        }

        [And(@"comps.point.z > comps.over_point.z")]
        public void Then_point_is_below_over_point()
        {
            Assert.True(hit[ComputationsId].PointOnSurface.Z > hit[ComputationsId].PointOverSurface.Z);
        }

        [Then(@"comps.reflectv = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_hit_reflectv(double x, double y, double z)
        {
            Assert.Equal(Tuple4.Vector(x, y, z), hit[ComputationsId].ReflectionVector);
        }

        [Then(@"comps.n1 = ([+-.0-9]+)")]
        public void Then_hit_n1(double value)
        {
            Assert.Equal(value, refractiveIndex["n1"]);
        }

        [And(@"comps.n2 = ([+-.0-9]+)")]
        public void Then_hit_n2(double value)
        {
            Assert.Equal(value, refractiveIndex["n2"]);
        }

        [Then(@"comps.under_point.z > EPSILON/2")]
        public void Then_under_point()
        {
            Assert.True(hit[ComputationsId].PointUnderSurface.Z > Core.Constants.HalfEpsilon);
        }

        [And(@"comps.point.z < comps.under_point.z")]
        public void Then_point_is_above_under_point()
        {
            Assert.True(hit[ComputationsId].PointOnSurface.Z < hit[ComputationsId].PointUnderSurface.Z);
        }

        [And(@"([a-z][a-z0-9]*) ← schlick\(([a-z][a-z0-9]*)\)")]
        public void And_reflectance_schlick(string id, string comps)
        {
            var h = hit[ComputationsId];
            var reflectance = BaseScene.Schlick(h, refractiveIndex["n1"], refractiveIndex["n2"]);
            schlick[id] = reflectance;
        }

        [Then(@"reflectance = ([+-.0-9]+)")]
        public void Then_reflectance(double r)
        {
            Assert.True(Core.Constants.EpsilonCompare(r, schlick["reflectance"]));
        }

        public static (HitResult hit, double refractiveIndexEntering, double refractiveIndexExiting) prepareComputations(Ray r, Intersection i, Intersection[] ixs)
        {
            var h = i.figure
                    .AllHits(r.origin, r.dir)
                    .First(hit => Core.Constants.EpsilonCompare(hit.Distance, i.t));

            if (ixs == null)
            {
                return (h, 1.0, 1.0);
            }

            var all = ixs.Select(x => x.figure
                                       .AllHits(r.origin, r.dir)
                                       .First(hit => Core.Constants.EpsilonCompare(hit.Distance, x.t)))
                         .ToArray();
            var (refractiveIndexEntering, refractiveIndexExiting) = Refraction.computeRefractiveIndexes(h.Distance, all);

            return (h, refractiveIndexEntering, refractiveIndexExiting);
        }
    }
}
