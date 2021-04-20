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
using Protsyk.RayTracer.Challenge.Core.Canvas;
using Protsyk.RayTracer.Challenge.Core.Geometry;

using DocString = Gherkin.Ast.DocString;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/canvas.feature")]
    public class CanvasTests : Feature
    {
        private readonly IDictionary<string, Color> color = new Dictionary<string, Color>();

        private readonly IDictionary<string, ICanvas> canvas = new Dictionary<string, ICanvas>();

        private readonly IDictionary<string, string> ppm = new Dictionary<string, string>();

        private readonly ITestOutputHelper testOutputHelper;

        public CanvasTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][a-z0-9]*) ← canvas\(([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Given_canvas(string id, int width, int height)
        {
            canvas[id] = new MemoryCanvas(width, height);
        }

        [Given(@"([a-z][a-z0-9]*) ← color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*) ← color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Color_id(string id, double t1, double t2, double t3)
        {
            color[id] = ColorConverters.Tuple1.From(new Tuple4(t1, t2, t3, TupleFlavour.Vector));
        }

        [Then(@"([a-z][a-z0-9]*).([a-z]*) = ([+-.0-9]+)")]
        [And(@"([a-z][a-z0-9]*).([a-z]*) = ([+-.0-9]+)")]
        public void Then_property(string id, string propertyName, int value)
        {
            switch (propertyName)
            {
                case "height":
                    Assert.Equal(canvas[id].Height, value);
                    break;
                case "width":
                    Assert.Equal(canvas[id].Width, value);
                    break;
                default:
                    throw new Exception($"Unknown property {propertyName}");
            }
        }

        [And(@"every pixel of ([a-z]*) is color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_every_pixel(string id, double r, double g, double b)
        {
            var c = canvas[id];
            for (int y = 0; y < c.Height; ++y)
            {
                for (int x = 0; x < c.Width; ++x)
                {
                    var pix = c.GetPixel(x, y);
                    Assert.True(Constants.EpsilonCompare(r, pix.R));
                    Assert.True(Constants.EpsilonCompare(g, pix.G));
                    Assert.True(Constants.EpsilonCompare(b, pix.B));
                }
            }
        }

        [When(@"write_pixel\(([a-z]*), ([+-.0-9]+), ([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        [And(@"write_pixel\(([a-z]*), ([+-.0-9]+), ([+-.0-9]+), ([a-z][a-z0-9]*)\)")]
        public void When_write_pixel(string id, int x, int y, string color_id)
        {
            canvas[id].SetPixel(x, y, color[color_id]);
        }

        [Then(@"pixel_at\(([a-z]*), ([+-.0-9]+), ([+-.0-9]+)\) = ([a-z][a-z0-9]*)")]
        public void Then_pixel_at(string id, int x, int y, string color_id)
        {
            var color1 = canvas[id].GetPixel(x, y);
            var color2 = color[color_id];

            Assert.True(Constants.EpsilonCompare(color1.R, color2.R));
            Assert.True(Constants.EpsilonCompare(color1.G, color2.G));
            Assert.True(Constants.EpsilonCompare(color1.B, color2.B));
        }

        [When(@"([a-z][a-z0-9]*) ← canvas_to_ppm\(([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← canvas_to_ppm\(([a-z][a-z0-9]*)\)")]
        public void When_canvas_to_ppm(string id, string canvas_id)
        {
            using (var mem = new MemoryStream())
            {
                CanvasConverters.PPM_P3.Convert(canvas[canvas_id], mem);

                mem.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(mem))
                {
                    var text = reader.ReadToEnd();
                    ppm[id] = text;
                }
            }
        }

        [Then(@"lines ([0-9]+)-([0-9]+) of ([a-z][a-z0-9]*) are")]
        public void Then_ppm_is(int l1, int l2, string id, DocString text)
        {
            var expected_lines = text.Content.Split('\n');
            var actual_lines = ppm[id].Split('\n');

            for (int i = l1; i <= l2; ++i)
            {
                Assert.Equal(expected_lines[i - l1].Trim('\r'), actual_lines[i - 1].Trim('\r'));
            }
        }

        [Then(@"([a-z][a-z0-9]*) ends with a newline character")]
        public void Then_ppm_ends_with_newline(string id)
        {
            Assert.Equal('\n', ppm[id].Last());
        }

        [When(@"every pixel of ([a-z]*) is set to color\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void When_every_pixel(string id, double r, double g, double b)
        {
            var c = canvas[id];
            var pix = ColorConverters.Tuple1.From(new Tuple4(r, g, b, TupleFlavour.Point));
            for (int y = 0; y < c.Height; ++y)
            {
                for (int x = 0; x < c.Width; ++x)
                {
                    c.SetPixel(x, y, pix);
                }
            }
        }

    }
}
