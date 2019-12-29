using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    internal class Sphere
    {
        public Vector3 Center { get; private set; }

        public float Radius { get; private set; }

        private float Radius2;

        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
            Radius2 = radius * radius;
        }

        public float Intersects(Vector3 origin, Vector3 dir)
        {
            var l = Vector3.Subtract(Center, origin);
            var tca = Vector3.Dot(l, dir);
            var d2 = Vector3.Dot(l, l) - tca * tca;
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
            return (float)t0;
        }
    }
}
