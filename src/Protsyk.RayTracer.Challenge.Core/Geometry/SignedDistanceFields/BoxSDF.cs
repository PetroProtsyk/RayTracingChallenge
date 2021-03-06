using System;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry.SignedDistanceFields
{
    public class BoxSDF : SignedDistanceField
    {
        public BoxSDF()
        {
        }

        protected internal override double DistanceFrom(Tuple4 point) {
            if (point.IsVector()) {
                throw new ArgumentException("Argument is not a point");
            }
            // If d.x < 0, then -1 < p.x < 1, and same logic applies to p.y, p.z
            // So if all components of d are negative, then p is inside the unit cube
            // vec3 d = abs(p) - vec3(1.0, 1.0, 1.0);
            double dX = Math.Abs(point.X) - 1.0;
            double dY = Math.Abs(point.Y) - 1.0;
            double dZ = Math.Abs(point.Z) - 1.0;

            // Assuming p is inside the cube, how far is it from the surface?
            // Result will be negative or zero.
            double insideDistance = Math.Min(Math.Max(dX, Math.Max(dY, dZ)), 0.0);
            
            // Assuming p is outside the cube, how far is it from the surface?
            // Result will be positive or zero.
            if (dX < 0.0) dX = 0.0;
            if (dY < 0.0) dY = 0.0;
            if (dZ < 0.0) dZ = 0.0;
            double outsideDistance =  Math.Sqrt(dX * dX + dY * dY + dZ * dZ);

            return insideDistance + outsideDistance;
        }
   }
}
