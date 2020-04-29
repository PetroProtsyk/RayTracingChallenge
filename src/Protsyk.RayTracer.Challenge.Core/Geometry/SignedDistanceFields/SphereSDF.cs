using System;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry.SignedDistanceFields
{
    public class SphereSDF : SignedDistanceField
    {
        public SphereSDF(IMatrix transformation)
        {
            Transformation = transformation;
        }

        protected internal override double DistanceFrom(Tuple4 point) {
            if (point.IsVector()) {
                throw new ArgumentException("Argument is not a point");
            }

            return Math.Sqrt(point.X*point.X + point.Y*point.Y + point.Z*point.Z) - 1.0;
        }
   }
}
