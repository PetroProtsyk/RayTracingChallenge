using Protsyk.RayTracer.Challenge.Core.Geometry;
using Protsyk.RayTracer.Challenge.Core.Scene;
using Protsyk.RayTracer.Challenge.Core.Scene.Figures;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.FileParser
{
    public interface IFileParser
    {
        int Lines { get; }

        IReadOnlyList<Tuple4> Vertices { get; }

        IReadOnlyList<Tuple4> Normals { get; }

        IReadOnlyList<Triangle> Triangles { get; }

        IReadOnlyList<TriangleGroup> Groups { get; }

        TriangleGroup DefaultGroup { get; }

        GroupFigure ToFigure();
    }

    public class Triangle
    {
        public readonly Tuple4 P1;
        public readonly Tuple4 P2;
        public readonly Tuple4 P3;

        public readonly Tuple4 N1;
        public readonly Tuple4 N2;
        public readonly Tuple4 N3;

        public Triangle(Tuple4 p1, Tuple4 p2, Tuple4 p3)
             : this(p1, p2, p3, null, null, null)
        {
        }

        public Triangle(Tuple4 p1, Tuple4 p2, Tuple4 p3, Tuple4 n1, Tuple4 n2, Tuple4 n3)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.P3 = p3;
            this.N1 = n1;
            this.N2 = n2;
            this.N3 = n3;
        }
    }

    public class TriangleGroup
    {
        public string Name { get; private set; }

        public readonly List<Triangle> Triangles = new List<Triangle>();

        public TriangleGroup(string name)
        {
            this.Name = name;
        }
    }
}
