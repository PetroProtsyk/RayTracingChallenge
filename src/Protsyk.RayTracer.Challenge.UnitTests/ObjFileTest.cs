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
using Protsyk.RayTracer.Challenge.Core.FileParser;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;

namespace Protsyk.RayTracer.Challenge.UnitTests
{
    [FeatureFile("./features/obj_file.feature")]
    public class ObjFileTests : Feature
    {
        private readonly IDictionary<string, IFileParser> parser = new Dictionary<string, IFileParser>();

        private readonly IDictionary<string, string> content = new Dictionary<string, string>();

        private readonly IDictionary<string, TriangleGroup> groups = new Dictionary<string, TriangleGroup>();

        private readonly IDictionary<string, Triangle> triangles = new Dictionary<string, Triangle>();

        private readonly IDictionary<string, GroupFigure> figure = new Dictionary<string, GroupFigure>();

        private readonly ITestOutputHelper testOutputHelper;

        public ObjFileTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;

            var ci = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        [Given(@"([a-z][a-z0-9]*) ← a file containing:")]
        public void Given_content(string id, DocString text)
        {
            content.Add(id, text.Content);
        }

        [Given(@"([a-z][a-z0-9]*) ← the file ([a-z][.a-z0-9]*)")]
        public void Given_file(string id, string file)
        {
            content.Add(id, File.ReadAllText(Path.Combine("./files", file)));
        }

        [When(@"([a-z][a-z0-9]*) ← parse_obj_file\(([a-z][a-z0-9]*)\)")]
        [And(@"([a-z][a-z0-9]*) ← parse_obj_file\(([a-z][a-z0-9]*)\)")]
        public void When_parser(string id, string cId)
        {
            parser.Add(id, WavefrontObjParser.FromText(content[cId]));
        }

        [Then(@"([a-z][a-z0-9]*) should have ignored ([0-9]+) lines")]
        public void Then_parser_ignores_lines(string id, int lines)
        {
            Assert.Equal(lines, parser[id].Lines);
        }

        [Then(@"([a-z][a-z0-9]*).vertices\[([0-9]+)\] = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*).vertices\[([0-9]+)\] = point\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_parser_vertex(string id, int i, double x, double y, double z)
        {
            Assert.Equal(Tuple4.Point(x, y, z), parser[id].Vertices[i - 1]);
        }

        [Then(@"([a-z][a-z0-9]*).normals\[([0-9]+)\] = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        [And(@"([a-z][a-z0-9]*).normals\[([0-9]+)\] = vector\(([+-.0-9]+), ([+-.0-9]+), ([+-.0-9]+)\)")]
        public void Then_parser_normal(string id, int i, double x, double y, double z)
        {
            Assert.Equal(Tuple4.Vector(x, y, z), parser[id].Normals[i - 1]);
        }


        [And(@"([a-z][a-z0-9]*) ← ([a-z][a-z0-9]*).default_group")]
        public void And_group(string id, string pId)
        {
            groups[id] = parser[pId].DefaultGroup;
        }

        [And(@"([a-z][a-z0-9]*) ← ([a-zA-Z][a-zA-Z0-9]*) from ([a-z][a-z0-9]*)")]
        public void And_group_from_parser(string id, string gId, string pId)
        {
            groups[id] = parser[pId].Groups.First(g => g.Name == gId);
        }

        [And(@"([a-z][a-z0-9]*) ← (first|second|third) child of ([a-z][a-z0-9]*)")]
        public void And_child_of_group(string id, string which, string gId)
        {
            int i;
            switch (which)
            {
                case "first":
                    i = 0;
                    break;
                case "second":
                    i = 1;
                    break;
                case "third":
                    i = 2;
                    break;
                default:
                    throw new NotSupportedException();
            }

            triangles.Add(id, groups[gId].Triangles[i]);
        }

        [Then(@"([a-z][a-z0-9]*).p1 = ([a-z][a-z0-9]*).vertices\[([0-9]+)\]")]
        [And(@"([a-z][a-z0-9]*).p1 = ([a-z][a-z0-9]*).vertices\[([0-9]+)\]")]
        public void Then_p1(string id, string pId, int i)
        {
            Assert.Equal(triangles[id].P1, parser[pId].Vertices[i - 1]);
        }

        [And(@"([a-z][a-z0-9]*).p2 = ([a-z][a-z0-9]*).vertices\[([0-9]+)\]")]
        public void Then_p2(string id, string pId, int i)
        {
            Assert.Equal(triangles[id].P2, parser[pId].Vertices[i - 1]);
        }

        [And(@"([a-z][a-z0-9]*).p3 = ([a-z][a-z0-9]*).vertices\[([0-9]+)\]")]
        public void Then_p3(string id, string pId, int i)
        {
            Assert.Equal(triangles[id].P3, parser[pId].Vertices[i - 1]);
        }

        [And(@"([a-z][a-z0-9]*).n1 = ([a-z][a-z0-9]*).normals\[([0-9]+)\]")]
        public void Then_n1(string id, string pId, int i)
        {
            Assert.Equal(triangles[id].N1, parser[pId].Normals[i - 1]);
        }

        [And(@"([a-z][a-z0-9]*).n2 = ([a-z][a-z0-9]*).normals\[([0-9]+)\]")]
        public void Then_n2(string id, string pId, int i)
        {
            Assert.Equal(triangles[id].N2, parser[pId].Normals[i - 1]);
        }

        [And(@"([a-z][a-z0-9]*).n3 = ([a-z][a-z0-9]*).normals\[([0-9]+)\]")]
        public void Then_n3(string id, string pId, int i)
        {
            Assert.Equal(triangles[id].N3, parser[pId].Normals[i - 1]);
        }

        [And(@"([a-z][a-z0-9]*) = ([a-z][a-z0-9]*)")]
        public void Then_objects_quals(string a, string b)
        {
            Assert.Equal(triangles[a].P1, triangles[b].P1);
            Assert.Equal(triangles[a].P2, triangles[b].P2);
            Assert.Equal(triangles[a].P3, triangles[b].P3);

            Assert.Equal(triangles[a].N1, triangles[b].N1);
            Assert.Equal(triangles[a].N2, triangles[b].N2);
            Assert.Equal(triangles[a].N3, triangles[b].N3);
        }

        [When(@"([a-z][a-z0-9]*) ← obj_to_group\(([a-z][a-z0-9]*)\)")]
        public void When_obj_to_group(string id, string pId)
        {
            figure[id] = parser[pId].ToFigure();
        }

        [Then(@"([a-z][a-z0-9]*) includes ([a-zA-Z][a-zA-Z0-9]*) from ([a-z][a-z0-9]*)")]
        [And(@"([a-z][a-z0-9]*) includes ([a-zA-Z][a-zA-Z0-9]*) from ([a-z][a-z0-9]*)")]
        public void Then_obj_includes(string id, string gId, string pId)
        {
            var g = parser[pId].Groups.First(g => g.Name == gId);
            var gt = g.Triangles.ToArray();

            foreach (var f in figure[id].Figures)
            {
                var ft = ((GroupFigure)f).Figures.Cast<TriangleFigure>().ToArray();

                if (gt.Length != ft.Length)
                {
                    continue;
                }

                var found = true;
                for (int i = 0; i < gt.Length; ++i)
                {
                    if (!gt[i].P1.Equals(ft[i].P1) ||
                        !gt[i].P2.Equals(ft[i].P2) ||
                        !gt[i].P3.Equals(ft[i].P3))
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    Assert.True(true);
                    return;
                }
            }

            throw new Exception("No Group found");
        }


    }
}
