using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;
using System.IO;

namespace Protsyk.RayTracer.Challenge.Core.FileParser
{
    public class WavefrontObjParser : IFileParser
    {
        private readonly List<Tuple4> vertices = new List<Tuple4>();

        private readonly List<Tuple4> normals = new List<Tuple4>();

        private readonly List<Triangle> triangles = new List<Triangle>();

        private readonly List<TriangleGroup> groups = new List<TriangleGroup>();

        public int Lines { get; private set; }

        public IReadOnlyList<Tuple4> Vertices => vertices.AsReadOnly();

        public IReadOnlyList<Tuple4> Normals => normals.AsReadOnly();

        public IReadOnlyList<Triangle> Triangles => triangles.AsReadOnly();

        public IReadOnlyList<TriangleGroup> Groups => groups.AsReadOnly();

        public TriangleGroup DefaultGroup { get; private set; }

        public GroupFigure ToFigure()
        {
            var result = new GroupFigure();
            foreach (var g in Groups)
            {
                var subGroup = new GroupFigure();
                foreach (var t in g.Triangles)
                {
                    if (t.N1 != null && t.N2 != null && t.N3 != null)
                    {
                        subGroup.Add(new TriangleWithNormalsFigure(Matrix4x4.Identity, MaterialConstants.Default, t.P1, t.P2, t.P3, t.N1, t.N2, t.N3));
                    }
                    else
                    {
                        subGroup.Add(new TriangleFigure(Matrix4x4.Identity, MaterialConstants.Default, t.P1, t.P2, t.P3));
                    }
                }
                result.Add(subGroup);
            }
            return result;
        }

        public WavefrontObjParser(IEnumerable<String> inputLines)
        {
            DefaultGroup = new TriangleGroup("Default");

            var activeGroup = DefaultGroup;
            groups.Add(activeGroup);

            foreach (var line in inputLines)
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    switch (parts[0])
                    {
                        case "v":
                            vertices.Add(ReadVertex(parts));
                            break;
                        case "vn":
                            normals.Add(ReadNormal(parts));
                            break;
                        case "f":
                            var newTriangles = line.Contains('/') ?
                                                ReadTrianglesWithNormals(parts.Skip(1).Select(x => ReadInt(x.Split('/')[0])).ToArray(),
                                                                         parts.Skip(1).Select(x => ReadInt(x.Split('/')[2])).ToArray()) :
                                                ReadTriangles(parts.Skip(1).Select(ReadInt).ToArray());
                            activeGroup.Triangles.AddRange(newTriangles);
                            triangles.AddRange(newTriangles);
                            break;
                        case "g":
                            activeGroup = new TriangleGroup(parts[1]);
                            groups.Add(activeGroup);
                            break;
                        default:
                            break;
                    }
                }
                Lines++;
            }
        }

        private IEnumerable<Triangle> ReadTriangles(int[] vIndex)
        {
            for (int i = 1; i < vIndex.Length - 1; ++i)
            {
                yield return new Triangle(vertices[vIndex[0] - 1], vertices[vIndex[i] - 1], vertices[vIndex[i + 1] - 1]);
            }
        }

        private IEnumerable<Triangle> ReadTrianglesWithNormals(int[] vIndex, int[] nIndex)
        {
            for (int i = 1; i < vIndex.Length - 1; ++i)
            {
                yield return new Triangle(vertices[vIndex[0] - 1], vertices[vIndex[i] - 1], vertices[vIndex[i + 1] - 1],
                                          normals[nIndex[0] - 1], normals[nIndex[i] - 1], normals[nIndex[i + 1] - 1]);
            }
        }

        private Tuple4 ReadVertex(string[] parts)
        {
            return Tuple4.Point(ReadDouble(parts[1]), ReadDouble(parts[2]), ReadDouble(parts[3]));
        }

        private Tuple4 ReadNormal(string[] parts)
        {
            return Tuple4.Vector(ReadDouble(parts[1]), ReadDouble(parts[2]), ReadDouble(parts[3]));
        }

        private static int ReadInt(string value)
        {
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        private static double ReadDouble(string value)
        {
            return double.Parse(value, CultureInfo.InvariantCulture);
        }

        public static IFileParser FromFile(string fileName)
        {
            return FromText(File.ReadAllText(fileName));
        }

        public static IFileParser FromText(string content)
        {
            var lines = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return new WavefrontObjParser(lines);
        }
    }
}
