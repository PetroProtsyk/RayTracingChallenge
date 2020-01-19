using System;
using System.Collections.Generic;
using Protsyk.RayTracer.Challenge.Core.Geometry;
using System.Text;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    internal class Sphere
    {
        public Tuple4 Center { get; private set; }

        public double Radius { get; private set; }

        private double Radius2;

        public Sphere(Tuple4 center, double radius)
        {
            Center = center;
            Radius = radius;
            Radius2 = radius * radius;
        }

        public double Intersects(Tuple4 origin, Tuple4 dir)
        {
            var l = Tuple4.Subtract(Center, origin);
            var tca = Tuple4.DotProduct(l, dir);
            var d2 = Tuple4.DotProduct(l, l) - tca * tca;
            if (d2 > Radius2)
            {
                return -1;
            }
            var thc = Math.Sqrt(Radius2 - d2);
            var t0 = tca - thc;
            var t1 = tca + thc;
            if (t0 < 0)
            {
                t0 = t1;
            }
            return t0;
        }
    }
}
