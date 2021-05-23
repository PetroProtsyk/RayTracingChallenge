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

        public Triangle(Tuple4 p1, Tuple4 p2, Tuple4 p3)
        {
            this.P1 = p1;
            this.P2 = p2;
            this.P3 = p3;
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
