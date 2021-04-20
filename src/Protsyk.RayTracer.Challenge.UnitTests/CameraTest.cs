using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene;
using Protsyk.RayTracer.Challenge.Core.Scene.Cameras;
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
    [FeatureFile("./features/camera.feature")]
    public class CameraTests : Feature
    {
        private readonly IDictionary<string, ICamera> cameras = new Dictionary<string, ICamera>();
        private readonly IDictionary<string, double> variables = new Dictionary<string, double>();
        private readonly IDictionary<string, Ray> rays = new Dictionary<string, Ray>();
        private readonly IDictionary<string, BaseScene> worlds = new Dictionary<string, BaseScene>();
        private readonly IDictionary<string, Tuple4> tuple = new Dictionary<string, Tuple4>();

        private readonly bool useFovCamera2 = true;

        private readonly ITestOutputHelper testOutputHelper;

        public CameraTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][_a-z0-9]*) ← ([+-.0-9]+)")]
        [And(@"([a-z][_a-z0-9]*) ← ([+-.0-9]+)")]
        public void Given_value(string id, double value)
        {
            variables[id] = value;
        }

        [When(@"([a-z][a-z0-9]*) ← camera\(([_a-z+-.0-9]+), ([_a-z+-.0-9]+), ([_a-z+-.0-9]+)\)")]
        [Given(@"([a-z][a-z0-9]*) ← camera\(([_a-z+-.0-9]+), ([_a-z+-.0-9]+), ([_a-z+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← camera\(([_a-z+-.0-9]+), ([_a-z+-.0-9]+), ([_a-z+-.0-9]+)\)")]
        public void Given_fov_camera(string id, string hsize, string vsize, string field_of_view)
        {
            if (!double.TryParse(hsize, out var hsizeValue))
            {
                hsizeValue = variables[hsize];
            }
            if (!double.TryParse(vsize, out var vsizeValue))
            {
                vsizeValue = variables[vsize];
            }
            if (!double.TryParse(field_of_view, out var fovValue))
            {
                fovValue = variables[field_of_view];
            }

            cameras[id] = useFovCamera2 ?
                (ICamera)new FovCamera2(Matrix4x4.Identity, fovValue, hsizeValue, vsizeValue) :
                (ICamera)new FovCamera(Tuple4.ZeroPoint, MatrixOperations.Geometry3D.RotateY(Math.PI), fovValue, hsizeValue, vsizeValue);
        }

        [Then(@"([a-z][a-z0-9]*).([a-z][_a-z0-9]*) = ([_a-z+-.0-9]+)")]
        [And(@"([a-z][a-z0-9]*).([a-z][_a-z0-9]*) = ([_a-z+-.0-9]+)")]
        public void Then_world_contains_no_objects(string id, string field, string value)
        {
            switch (field)
            {
                case "hsize":
                    Assert.True(Core.Constants.EpsilonCompare(double.Parse(value), cameras[id].ScreenWidth));
                    break;
                case "vsize":
                    Assert.True(Core.Constants.EpsilonCompare(double.Parse(value), cameras[id].ScreenHeight));
                    break;
                case "field_of_view":
                    Assert.True(Core.Constants.EpsilonCompare(double.Parse(value), cameras[id].FieldOfView));
                    break;
                case "pixel_size":
                    Assert.True(Core.Constants.EpsilonCompare(double.Parse(value), cameras[id].PixleSize));
                    break;
                case "transform":
                    Assert.Equal(getNamedMatrix(value), cameras[id].Transformation);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        [When(@"([a-z][a-z0-9]*) ← ray_for_pixel\(([a-z][a-z0-9]*), ([0-9]+), ([0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← ray_for_pixel\(([a-z][a-z0-9]*), ([0-9]+), ([0-9]+)\)")]
        public void When_ray_for_pixel(string id, string cId, double x, double y)
        {
            var r = cameras[cId].GetRay(x, y);
            rays[id] = r;
        }

        [Then(@"([a-z][a-z0-9]*).origin = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_ray_origin(string id, double x, double y, double z)
        {
            Assert.Equal(new Tuple4(x, y, z, TupleFlavour.Point), rays[id].origin);
        }

        [And(@"([a-z][a-z0-9]*).direction = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_ray_dir(string id, double x, double y, double z)
        {
            Assert.Equal(new Tuple4(x, y, z, TupleFlavour.Vector), rays[id].dir);
        }

        [Given(@"([a-z][a-z0-9]*) ← default_world\(\)")]
        [When(@"([a-z][a-z0-9]*) ← default_world\(\)")]
        public void Given_default_world(string id)
        {
            worlds.Add(id, WorldTests.CreateDefault());
        }

        [Given(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Vector_id(string id, double t1, double t2, double t3)
        {
            tuple.Add(id, new Tuple4(t1, t2, t3, TupleFlavour.Vector));
        }

        [Given(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Point_id(string id, double t1, double t2, double t3)
        {
            tuple.Add(id, new Tuple4(t1, t2, t3, TupleFlavour.Point));
        }

        [And(@"([a-z][a-z0-9]*).transform ← view_transform\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void And_transformation(string id, string from, string to, string up)
        {
            if (!useFovCamera2)
            {
                throw new NotSupportedException("This camera does not support view transformation");
            }

            cameras[id].Transformation = MatrixOperations.Geometry3D.LookAtTransform(tuple[from], tuple[to], tuple[up]);
        }

        [When(@"([a-z][a-z0-9]*).transform ← rotation_y\(([+-.0-9]+)\) \* translation\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void When_transformation(string id, double r_y, double t_x, double t_y, double t_z)
        {
            if (useFovCamera2)
            {
                var fovCamera = (FovCamera2)cameras[id];
                fovCamera.Transformation = MatrixOperations.Multiply(
                                            MatrixOperations.Geometry3D.RotateY(r_y),
                                            MatrixOperations.Geometry3D.Translation(t_x, t_y, t_z));
            }
            else
            {
                var fovCamera = (FovCamera)cameras[id];
                fovCamera.Transformation = MatrixOperations.Multiply(
                                            fovCamera.Transformation,
                                            MatrixOperations.Geometry3D.RotateY(-r_y));
                fovCamera.Origin = new Tuple4(-t_x, -t_y, -t_z, TupleFlavour.Point);
            }
        }

        [When(@"([a-z][a-z0-9]*) ← render\(([a-z][a-z0-9]*), ([a-z][a-z0-9]*)\)")]
        public void Dummy_render(string id, string camera, string world)
        {
        }

        [Then(@"pixel_at\(([a-z][a-z0-9]*), ([+-.0-9]+), ([+-.0-9]+)\) = color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_color(string id, int x, int y, double r, double g, double b)
        {
            var ray = cameras["c"].GetRay(x, y);
            var color = worlds["w"].CastRay(ray);

            Assert.True(Core.Constants.EpsilonCompare(r, color.X));
            Assert.True(Core.Constants.EpsilonCompare(g, color.Y));
            Assert.True(Core.Constants.EpsilonCompare(b, color.Z));
            Assert.True(color.IsVector());
        }

        private IMatrix getNamedMatrix(string value)
        {
            if (value.Equals("identity_matrix"))
            {
                return useFovCamera2 ?
                            (IMatrix)Matrix4x4.Identity :
                            (IMatrix)MatrixOperations.Geometry3D.RotateY(Math.PI);
            }
            throw new NotSupportedException($"Unknow matrix {value}");
        }
    }
}
