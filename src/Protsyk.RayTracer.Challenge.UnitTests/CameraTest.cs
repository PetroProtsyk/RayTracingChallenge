using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene;
using Protsyk.RayTracer.Challenge.Core.Scene.Cameras;
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
    [FeatureFile("./features/camera.feature")]
    public class CameraTests : Feature
    {
        private readonly IDictionary<string, ICamera> cameras = new Dictionary<string, ICamera>();
        private readonly IDictionary<string, double> variables = new Dictionary<string, double>();

        private readonly ITestOutputHelper testOutputHelper;

        private static readonly string ComputationsId = "comps";

        public CameraTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][_a-z0-9]*) ← ([+-.0-9]+)")]
        [ And(@"([a-z][_a-z0-9]*) ← ([+-.0-9]+)")]
        public void Given_value(string id, double value)
        {
            variables[id] = value;
        }

        [When(@"([a-z][a-z0-9]*) ← camera\(([_a-z+-.0-9]+), ([_a-z+-.0-9]+), ([_a-z+-.0-9]+)\)")]
        [Given(@"([a-z][a-z0-9]*) ← camera\(([_a-z+-.0-9]+), ([_a-z+-.0-9]+), ([_a-z+-.0-9]+)\)")]
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
            cameras[id] = new FovCamera(Tuple4.ZeroPoint, fovValue, hsizeValue, vsizeValue);
        }

        [Then(@"([a-z][a-z0-9]*).([a-z][_a-z0-9]*) = ([_a-z+-.0-9]+)")]
        [ And(@"([a-z][a-z0-9]*).([a-z][_a-z0-9]*) = ([_a-z+-.0-9]+)")]
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

        private IMatrix getNamedMatrix(string value)
        {
            if (value.Equals("identity_matrix"))
            {
                return Matrix4x4.Identity;
            }
            throw new NotSupportedException($"Unknow matrix {value}");
        }
    }
}
