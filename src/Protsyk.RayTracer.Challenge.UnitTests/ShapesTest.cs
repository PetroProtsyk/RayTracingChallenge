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
    [FeatureFile("./features/shapes.feature")]
    public class ShapesTest : Feature
    {
        private readonly IDictionary<string, Ray> ray = new Dictionary<string, Ray>();

        private readonly IDictionary<string, IFigure> figure = new Dictionary<string, IFigure>();

        private readonly IDictionary<string, HitResult[]> intersection = new Dictionary<string, HitResult[]>();

        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly IDictionary<string, IMatrix> matrix = new Dictionary<string, IMatrix>();

        private readonly IDictionary<string, IMaterial> material = new Dictionary<string, IMaterial>();

        private readonly ITestOutputHelper testOutputHelper;

        public ShapesTest(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            matrix["identity_matrix"] = MatrixOperations.Identity(4);
        }

        [Given(@"([a-z][a-z0-9]*) ← ray\(point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void Given_ray(string id,
                              double p1, double p2, double p3,
                              double v1, double v2, double v3)
        {
            ray[id] = new Ray(new Tuple4(p1, p2, p3, TupleFlavour.Point),
                              new Tuple4(v1, v2, v3, TupleFlavour.Vector));
        }

        [Given(@"([a-z][a-z0-9]*) ← test_shape\(\)")]
        [And(@"([a-z][a-z0-9]*) ← test_shape\(\)")]
        public void Given_test_shape(string id)
        {
            figure[id] = new TestFigure(MatrixOperations.Identity(4), MaterialConstants.Default);
        }

        [And(@"([a-z][a-z0-9]*) ← sphere\(\)")]
        public void Given_sphere(string id)
        {
            figure[id] = new SphereFigure(Matrix4x4.Identity, MaterialConstants.Default);
        }

        [When(@"([a-z][a-z0-9]*) ← intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← intersect\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_intersect(string id, string figureId, string rayId)
        {
            var r = ray[rayId];
            var result = figure[figureId].AllHits(r.origin, r.dir);
            intersection[id] = result;
        }

        [When(@"([a-z][a-z0-9]*) ← ([a-z][a-z0-9]*).material")]
        public void When_assign_material(string id, string figureId)
        {
            material[id] = figure[figureId].Material;
        }

        [And(@"([a-z][a-z0-9]*) ← material\(\)")]
        public void When_assign_default_material(string id)
        {
            material[id] = MaterialConstants.Default;
        }

        [Then(@"([a-z][a-z0-9]*) = material\(\)")]
        public void Then_default_material(string id)
        {
            Assert.Equal(MaterialConstants.Default, material[id]);
        }

        [When(@"([a-z][a-z0-9]*).material ← ([a-z][a-z0-9]*)")]
        public void When_assign_material_to_figure(string figureId, string mId)
        {
            figure[figureId].Material = material[mId];
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

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\] = ([+-.0-9]+)")]
        public void Then_intersect_value(string id, int i, double v)
        {
            Assert.True(Constants.EpsilonCompare(v, intersection[id][i].Distance));
        }

        [And(@"([a-z][a-z0-9]*)\[([0-9]+)\].object = ([a-z][a-z0-9]*)")]
        public void Then_intersect_object(string id, int i, string figureId)
        {
            Assert.Equal(figure[figureId], intersection[id][i].Figure);
        }

        [When(@"([a-z][a-z0-9]*) ← normal_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        [And(@"([a-z][a-z0-9]*) ← normal_at\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_normal_at(string id, string figureId,
                                   double p1, double p2, double p3)
        {
            var result = figure[figureId].GetNormal(null, new Tuple4(p1, p2, p3, TupleFlavour.Point));
            tuple[id] = result;
        }

        [Then(@"([a-z][a-z0-9]*) = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_vector(string id,
                                double t1, double t2, double t3)
        {
            var v = new Tuple4(t1, t2, t3, TupleFlavour.Vector);
            Assert.Equal(v, tuple[id]);
        }

        [And(@"([a-zA-Z][a-zA-Z0-9]*) ← translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_translation(string id, double t1, double t2, double t3)
        {
            matrix.Add(id, MatrixOperations.Geometry3D.Translation(t1, t2, t3));
        }

        [And(@"([a-zA-Z][a-zA-Z0-9]*) ← scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_scaling(string id, double t1, double t2, double t3)
        {
            matrix.Add(id, MatrixOperations.Geometry3D.Scale(t1, t2, t3));
        }

        [And(@"([a-zA-Z][a-zA-Z0-9]*) ← scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\) \* rotation_z\(π/5\)")]
        public void Given_scaling_rotation(string id, double t1, double t2, double t3)
        {
            matrix.Add(id, MatrixOperations.Multiply(
                                 MatrixOperations.Geometry3D.Scale(t1, t2, t3),
                                 MatrixOperations.Geometry3D.RotateZ(Math.PI / 5)));
        }

        [When(@"set_transform\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [And(@"set_transform\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_set_transform(string figureId, string matrixId)
        {
            figure[figureId].Transformation = matrix[matrixId];
        }

        [When(@"set_transform\(([a-z][a-z0-9]*), translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        [And(@"set_transform\(([a-z][a-z0-9]*), translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_set_translation_transform(string figureId, double t1, double t2, double t3)
        {
            figure[figureId].Transformation = MatrixOperations.Geometry3D.Translation(t1, t2, t3);
        }

        [When(@"set_transform\(([a-z][a-z0-9]*), scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        [And(@"set_transform\(([a-z][a-z0-9]*), scaling\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_set_scale_transform(string figureId, double t1, double t2, double t3)
        {
            figure[figureId].Transformation = MatrixOperations.Geometry3D.Scale(t1, t2, t3);
        }

        [And(@"set_transform\(([a-z][a-z0-9]*), rotation_y\(π/2\)\)")]
        public void When_set_rotation_y_transform(string figureId)
        {
            figure[figureId].Transformation = MatrixOperations.Geometry3D.RotateY(Math.PI / 2);
        }

        [Then(@"([a-z][a-z0-9]*).transform = ([a-z][_a-z0-9]*)")]
        public void Then_transformation(string id, string matrixId)
        {
            Assert.Equal(figure[id].Transformation, matrix[matrixId]);
        }

        [Then(@"([a-z][a-z0-9]*).transform = translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_translation_transformation(string id, double t1, double t2, double t3)
        {
            Assert.Equal(figure[id].Transformation, MatrixOperations.Geometry3D.Translation(t1, t2, t3));
        }

        [Then(@"([a-z][a-z0-9]*) = normalize\(([a-z][a-z0-9]*)\)")]
        public void Then_n(string a, string b)
        {
            Assert.Equal(tuple[a], Tuple4.Normalize(tuple[b]));
        }

        [Then(@"([a-z][a-z0-9]*).saved_ray.origin = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_saved_ray_origin(string a, double t1, double t2, double t3)
        {
            Assert.Equal(Tuple4.Point(t1, t2, t3), ((TestFigure)figure[a]).SavedRay.origin);
        }

        [And(@"([a-z][a-z0-9]*).saved_ray.direction = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_saved_ray_direction(string a, double t1, double t2, double t3)
        {
            Assert.Equal(Tuple4.Vector(t1, t2, t3), ((TestFigure)figure[a]).SavedRay.dir);
        }

        [And(@"([a-z][a-z0-9]*).ambient ← ([+-.0-9]+)")]
        public void Replace_material_color(string id, double ambient)
        {
            var m = material[id];
            material[id] = new SolidColorMaterial(
                m.GetColor(Tuple4.ZeroPoint),
                ambient,
                m.Diffuse,
                m.Specular,
                m.Shininess,
                m.Reflective,
                m.RefractiveIndex,
                m.Transparency);
        }

        [Then(@"([a-z][a-z0-9]*).material = ([a-z][_a-z0-9]*)")]
        public void Then_material(string id, string mId)
        {
            Assert.Equal(figure[id].Material, material[mId]);
        }

        [And(@"([a-z][a-z0-9]*).parent = ([a-z][a-z0-9]*)")]
        public void And_parent(string id, string gId)
        {
            Assert.Equal(figure[gId], figure[id].Parent);
        }

        [Given(@"([a-z][a-z0-9]*) ← group\(\)")]
        [And(@"([a-z][a-z0-9]*) ← group\(\)")]
        public void Given_group(string id)
        {
            figure[id] = new GroupFigure();
        }

        [When(@"add_child\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        [And(@"add_child\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void When_add_child(string id, string cId)
        {
            ((GroupFigure)figure[id]).Add(figure[cId]);
        }

        [When(@"([a-z][a-z0-9]*) ← world_to_object\(([a-z][a-z0-9]*), point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_world_to_object(string a, string s, double t1, double t2, double t3)
        {
            tuple[a] = ((BaseFigure)figure[s]).TransformWorldPointToObjectPoint(Tuple4.Point(t1, t2, t3));
        }

        [When(@"([a-z][a-z0-9]*) ← normal_to_world\(([a-z][a-z0-9]*), vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)\)")]
        public void When_normal_to_world(string a, string s, double t1, double t2, double t3)
        {
            tuple[a] = ((BaseFigure)figure[s]).TransformObjectNormalToWorldNormal(Tuple4.Vector(t1, t2, t3));
        }

        [Then(@"([a-z][a-z0-9]*).parent is nothing")]
        public void Then_group_is_not_empty(string id)
        {
            Assert.Null(figure[id].Parent);
        }

        [Then(@"([a-z][a-z0-9]*) = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_point(string a, double t1, double t2, double t3)
        {
            Assert.Equal(Tuple4.Point(t1, t2, t3), tuple[a]);
        }

        private class TestFigure : BaseFigure
        {
            public TestFigure(IMatrix transformation, IMaterial material)
            {
                Transformation = transformation;
                Material = material;
            }

            public Ray SavedRay { get; private set; }

            protected override Intersection[] GetBaseIntersectionsWithAnyDirection(Ray ray)
            {
                SavedRay = ray;
                return base.GetBaseIntersectionsWithAnyDirection(ray);
            }

            protected override Intersection[] GetBaseIntersections(Ray ray)
            {
                return new Intersection[] { new Intersection(0, this) };
            }

            protected override Tuple4 GetBaseNormal(IFigure figure, Tuple4 pointOnSurface)
            {
                return Tuple4.Vector(pointOnSurface.X, pointOnSurface.Y, pointOnSurface.Z);
            }
        }
    }
}
