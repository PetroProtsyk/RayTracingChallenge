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
        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly IDictionary<string, IFigure> figure = new Dictionary<string, IFigure>();

        private readonly IDictionary<string, HitResult> hit = new Dictionary<string, HitResult>();

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
        [ And(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        public void Given_sphere(string id)
        {
            figure[id] = new SphereFigure(MatrixOperations.Identity(4), MaterialConstants.Default);
        }

        [And(@"([a-z][a-z0-9]*) ← sphere\(\) with:")]
        public void And_sphere(string id, DataTable dataTable)
        {
            IMatrix transformation = Matrix4x4.Identity;

            foreach (var row in dataTable.Rows)
            {
                var cells = row.Cells.ToArray();
                switch (cells[0].Value)
                {
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

            figure[id] = new SphereFigure(transformation, MaterialConstants.Default);
        }

        [When(@"([a-z][a-z0-9]*) ← intersection\(([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        [ And(@"([a-z][a-z0-9]*) ← intersection\(([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        public void When_intersection(string id, double t, string figureId)
        {
            intersection[id] = new Intersection(t, figure[figureId]);
        }

        [When(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [ And(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersections(string id, string i1, string i2)
        {
            intersections[id] = new Intersection[]
            {
                intersection[i1],
                intersection[i2]
            };
        }

        [And(@"([a-z][a-z0-9]*) ← intersections\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersections(string id, string i1, string i2, string i3, string i4)
        {
            intersections[id] = new Intersection[]
            {
                intersection[i1],
                intersection[i2],
                intersection[i3],
                intersection[i4]
            };
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
        public void Given_ray(string id,
                              double p1, double p2, double p3,
                              double v1, double v2, double v3)
        {
            ray[id] = new Ray(new Tuple4(p1, p2, p3, TupleFlavour.Point),
                              new Tuple4(v1, v2, v3, TupleFlavour.Vector));
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
            Assert.Equal(new Tuple4(x, y, z, TupleFlavour.Point), hit[ComputationsId].PointOnSurface);
        }

        [And(@"comps.eyev = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_hit_eyev(double x, double y, double z)
        {
            Assert.Equal(new Tuple4(x, y, z, TupleFlavour.Vector), hit[ComputationsId].EyeVector);
        }

        [And(@"comps.normalv = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_hit_normalv(double x, double y, double z)
        {
            Assert.Equal(new Tuple4(x, y, z, TupleFlavour.Vector), hit[ComputationsId].SurfaceNormal);
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
        public void Then_point_is_above_over_point()
        {
            Assert.True(hit[ComputationsId].PointOnSurface.Z > hit[ComputationsId].PointOverSurface.Z);
        }

    }
}
