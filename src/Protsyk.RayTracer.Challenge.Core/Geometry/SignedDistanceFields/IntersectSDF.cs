using System;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry.SignedDistanceFields
{
    public class IntersectSDF : SignedDistanceField
    {
        private readonly SignedDistanceField a;
        private readonly SignedDistanceField b;

        public IntersectSDF(SignedDistanceField a, SignedDistanceField b)
        {
            this.a = a;
            this.b = b;
        }

        public override double[] GetIntersections(Ray ray)
        {
            //TODO: Rotations, etc
            var ia = a.GetIntersections(ray);
            var ib = b.GetIntersections(ray);
            if (ia == null)
            {
                return null;
            }
            if (ib == null)
            {
                return null;
            }
            return new double[] { Math.Max(ia[0], ib[0]) };
        }

        public override Tuple4 GetNormal(Tuple4 point)
        {
            var d1 = a.DistanceFrom(point);
            var d2 = b.DistanceFrom(point);
            if (Math.Abs(d1) < epsilon) {
                 return a.GetNormal(point);
            }
            return b.GetNormal(point);
        }
   }
}
