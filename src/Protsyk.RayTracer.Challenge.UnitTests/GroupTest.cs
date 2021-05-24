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
using Protsyk.RayTracer.Challenge.Core.Scene.Materials;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/groups.feature")]
    public class GroupTest : Feature
    {
        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();

        private readonly IDictionary<string, IFigure> figure = new Dictionary<string, IFigure>();

        private readonly IDictionary<string, HitResult[]> intersection = new Dictionary<string, HitResult[]>();

        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly IDictionary<string, IMatrix> matrix = new Dictionary<string, IMatrix>();

        private readonly IDictionary<string, IMaterial> material = new Dictionary<string, IMaterial>();

        private readonly ITestOutputHelper testOutputHelper;

        public GroupTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            matrix["identity_matrix"] = MatrixOperations.Identity(4);
        }

        [Given(@"([a-z][a-z0-9]*) ← group\(\)")]
        [And(@"([a-z][a-z0-9]*) ← group\(\)")]
        public void Given_group(string id)
        {
            figure[id] = new GroupFigure();
        }

        [And(@"([a-z][a-z0-9]*) ← test_shape\(\)")]
        public void Given_test_shape(string id)
        {
            figure[id] = new TestFigure(Matrix4x4.Identity, MaterialConstants.Default);
        }

        [And(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        public void Given_sphere(string id)
        {
            figure[id] = new SphereFigure(Matrix4x4.Identity, MaterialConstants.Default);
        }

        [Then(@"([a-z][a-z0-9]*).transform = ([a-z][_a-z0-9]*)")]
        public void Then_default_transform(string id, string mId)
        {
            Assert.Equal(matrix[mId], figure[id].Transformation);
        }

        [And(@"([a-z][a-z0-9]*) is empty")]
        public void Then_default_group_is_empty(string id)
        {
            Assert.Equal(0, ((GroupFigure)figure[id]).Figures.Count);
        }

        [When(@"add_child\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [And(@"add_child\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_add_child(string id, string cId)
        {
            ((GroupFigure)figure[id]).Add(figure[cId]);
        }

        [Then(@"([a-z][a-z0-9]*) is not empty")]
        public void Then_group_is_not_empty(string id)
        {
            Assert.True(((GroupFigure)figure[id]).Figures.Count > 0);
        }

        [And(@"([a-z][a-z0-9]*) includes ([a-z][a-z0-9]*)")]
        public void Then_group_includes(string id, string fId)
        {
            Assert.Contains(figure[fId], ((GroupFigure)figure[id]).Figures);
        }

        [And(@"([a-z][a-z0-9]*).parent = ([a-z][a-z0-9]*)")]
        public void And_parent(string id, string gId)
        {
            Assert.Same(figure[gId], figure[id].Parent);
        }

        [When(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        [And(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_ray(string id,
                      double p1, double p2, double p3,
                      double v1, double v2, double v3)
        {
            ray[id] = new Ray(new Tuple4(p1, p2, p3, TupleFlavour.Point),
                              new Tuple4(v1, v2, v3, TupleFlavour.Vector));
        }

        [When(@"([a-z][a-z0-9]*) ← local_intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← local_intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersect(string id, string figureId, string rayId)
        {
            var r = ray[rayId];
            var result = figure[figureId].AllHits(r.origin, r.dir);
            intersection[id] = result;
        }

        [Then(@"([a-z][a-z0-9]*) is empty")]
        public void Then_intersection_is_empty(string id)
        {
            Assert.Equal(HitResult.NoHit, intersection[id][0]);
        }

        [And(@"set_transform\(([a-z][a-z0-9]*), translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_set_translation_transform(string figureId, double t1, double t2, double t3)
        {
            figure[figureId].Transformation = MatrixOperations.Geometry3D.Translation(t1, t2, t3);
        }

        [And(@"set_transform\(([a-z][a-z0-9]*), scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_set_scale_transform(string figureId, double t1, double t2, double t3)
        {
            figure[figureId].Transformation = MatrixOperations.Geometry3D.Scale(t1, t2, t3);
        }

        [Then(@"([a-z][a-z0-9]*).count = ([+-.0-9]+)")]
        public void Then_intersect_count(string id, int v)
        {
            if (v == 0)
            {
                Assert.Equal(HitResult.NoHit, intersection[id][0]);
            }
            else
            {
                Assert.Equal(v, intersection[id].Length);
            }
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\].object = ([a-z][a-z0-9]*)")]
        public void Then_intersect_object(string id, int i, string figureId)
        {
            Assert.Equal(figure[figureId], intersection[id][i].Figure);
        }

        private class TestFigure : BaseFigure
        {
            public TestFigure(IMatrix transformation, IMaterial material)
            {
                Transformation = transformation;
                Material = material;
            }

            protected override Intersection[] GetBaseIntersections(Ray ray)
            {
                return new Intersection[] { new Intersection(0, this) };
            }

            protected override Tuple4 GetBaseNormal(IFigure figure, Tuple4 pointOnSurface, double u, double v)
            {
                return Tuple4.Vector(pointOnSurface.X, pointOnSurface.Y, pointOnSurface.Z);
            }
        }
    }
}
