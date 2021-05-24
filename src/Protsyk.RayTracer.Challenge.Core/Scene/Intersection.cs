using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

namespace Protsyk.RayTracer.Challenge.Core.Scene
{
    public struct Intersection
    {
        public readonly IFigure figure;
        public readonly double t;
        public readonly double u;
        public readonly double v;

        public Intersection(double t, IFigure figure)
            : this(t, figure, 0.0, 0.0)
        {
        }

        public Intersection(double t, IFigure figure, double u, double v)
        {
            this.t = t;
            this.figure = figure;
            this.u = u;
            this.v = v;
        }

        public override bool Equals(object obj)
        {
            return obj is Intersection intersection &&
                   ReferenceEquals(figure, intersection.figure) &&
                   t == intersection.t &&
                   u == intersection.u &&
                   v == intersection.v;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(figure, t);
        }
    }
}
