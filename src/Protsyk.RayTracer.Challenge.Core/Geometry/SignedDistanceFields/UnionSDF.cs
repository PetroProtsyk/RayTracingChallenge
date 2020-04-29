using System;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry.SignedDistanceFields
{
    public class UnionSDF : SignedDistanceField
    {
        private readonly SignedDistanceField a;
        private readonly SignedDistanceField b;

        public UnionSDF(SignedDistanceField a, SignedDistanceField b)
        {
            this.a = a;
            this.b = b;
        }

        public override double[] GetIntersections(Ray ray)
        {
            var ia = a.GetIntersections(ray);
            var ib = b.GetIntersections(ray);
            if (ia == null)
            {
                return ib;
            }
            if (ib == null)
            {
                return ia;
            }
            return new double[] { Math.Min(ia[0], ib[0]) };
        }

        public override Tuple4 GetNormal(Tuple4 point)
        {
            var d1 = a.DistanceFrom(a.GetObjectPoint(point));
            var d2 = b.DistanceFrom(b.GetObjectPoint(point));
            if (Math.Abs(d1) < epsilon) {
                 return a.GetNormal(point);
            }
            return b.GetNormal(point);
        }
   }
}
