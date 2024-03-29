﻿using Protsyk.RayTracer.Challenge.Core.Geometry;
using System;
using System.Collections.Generic;

namespace Protsyk.RayTracer.Challenge.Core.Scene.Figures
{
    public class TriangleWithNormalsFigure : BaseFigure
    {
        public Tuple4 P1 { get; private set; }
        public Tuple4 P2 { get; private set; }
        public Tuple4 P3 { get; private set; }

        public Tuple4 E1 { get; private set; }
        public Tuple4 E2 { get; private set; }
        public Tuple4 N1 { get; private set; }
        public Tuple4 N2 { get; private set; }
        public Tuple4 N3 { get; private set; }

        public TriangleWithNormalsFigure(IMatrix transformation, IMaterial material, Tuple4 p1, Tuple4 p2, Tuple4 p3, Tuple4 n1, Tuple4 n2, Tuple4 n3)
        {
            this.Material = material;
            this.Transformation = transformation;

            this.P1 = p1;
            this.P2 = p2;
            this.P3 = p3;
            this.E1 = Tuple4.Subtract(p2, p1);
            this.E2 = Tuple4.Subtract(p3, p1);
            this.N1 = n1;
            this.N2 = n2;
            this.N3 = n3;
        }

        protected override Tuple4 GetBaseNormal(IFigure figure, Tuple4 pointOnSurface, double u, double v)
        {
            return Tuple4.Add(Tuple4.Scale(N2, u),
                        Tuple4.Add(Tuple4.Scale(N3, v), Tuple4.Scale(N1, (1.0 - u - v))));
        }

        protected override Intersection[] GetBaseIntersections(Ray ray)
        {
            var dir_cross_e2 = Tuple4.CrossProduct(ray.dir, E2);
            var det = Tuple4.DotProduct(E1, dir_cross_e2);
            if (Constants.EpsilonZero(Math.Abs(det)))
            {
                return null;
            }

            var f = 1.0 / det;
            var p1_to_origin = Tuple4.Subtract(ray.origin, P1);
            var u = f * Tuple4.DotProduct(p1_to_origin, dir_cross_e2);
            if ((u < 0.0) || (u > 1.0))
            {
                return null;
            }

            var origin_cross_e1 = Tuple4.CrossProduct(p1_to_origin, E1);
            var v = f * Tuple4.DotProduct(ray.dir, origin_cross_e1);
            if ((v < 0.0) || ((u + v) > 1.0))
            {
                return null;
            }

            var t = f * Tuple4.DotProduct(E2, origin_cross_e1);
            return new Intersection[] { new Intersection(t, this, u, v) };
        }

        public override bool Equals(object obj)
        {
            return obj is TriangleWithNormalsFigure figure &&
                   EqualityComparer<IMaterial>.Default.Equals(Material, figure.Material) &&
                   EqualityComparer<IMatrix>.Default.Equals(Transformation, figure.Transformation) &&
                   EqualityComparer<Tuple4>.Default.Equals(P1, figure.P1) &&
                   EqualityComparer<Tuple4>.Default.Equals(P2, figure.P2) &&
                   EqualityComparer<Tuple4>.Default.Equals(P3, figure.P3) &&
                   EqualityComparer<Tuple4>.Default.Equals(N1, figure.N1) &&
                   EqualityComparer<Tuple4>.Default.Equals(N2, figure.N2) &&
                   EqualityComparer<Tuple4>.Default.Equals(N3, figure.N3);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Material.GetHashCode(), Transformation.GetHashCode(), P1, P2, P3, N1, N2, N3);
        }
    }
}
