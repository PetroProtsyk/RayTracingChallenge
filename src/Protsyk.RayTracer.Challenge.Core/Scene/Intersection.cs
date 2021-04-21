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

        public Intersection(double t, IFigure figure)
        {
            this.t = t;
            this.figure = figure;
        }

        public override bool Equals(object obj)
        {
            return obj is Intersection intersection &&
                   ReferenceEquals(figure, intersection.figure) &&
                   t == intersection.t;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(figure, t);
        }
    }
}
