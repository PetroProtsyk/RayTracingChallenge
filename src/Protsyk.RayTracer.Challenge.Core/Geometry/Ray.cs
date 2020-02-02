using System;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public class Ray
    {
        public readonly Tuple4 origin;
        public readonly Tuple4 dir;

        public Ray(Tuple4 origin, Tuple4 dir)
        {
            if (!origin.IsPoint())
            {
                throw new ArgumentException("Not a point", nameof(origin));
            }
            if (!dir.IsVector())
            {
                throw new ArgumentException("Not a vector", nameof(dir));
            }
            this.origin = origin;
            this.dir = dir;
        }
    }
}