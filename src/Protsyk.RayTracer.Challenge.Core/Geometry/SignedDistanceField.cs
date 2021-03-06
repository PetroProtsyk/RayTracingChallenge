using System;
using System.Collections.Generic;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public abstract class SignedDistanceField
    {
        protected readonly double epsilon = 0.0001;

        private readonly double maxDistance = 200;

        protected internal virtual double DistanceFrom(Tuple4 point)
        {
            if (point.IsVector())
            {
                throw new ArgumentException("Argument is not a point");
            }
            throw new NotImplementedException();
        }

        public virtual Tuple4 GetNormal(Tuple4 objectPoint)
        {
            var estimateNormal = new Tuple4(
                                    DistanceFrom(new Tuple4(objectPoint.X + epsilon, objectPoint.Y, objectPoint.Z, TupleFlavour.Point)) - DistanceFrom(new Tuple4(objectPoint.X - epsilon, objectPoint.Y, objectPoint.Z, TupleFlavour.Point)),
                                    DistanceFrom(new Tuple4(objectPoint.X, objectPoint.Y + epsilon, objectPoint.Z, TupleFlavour.Point)) - DistanceFrom(new Tuple4(objectPoint.X, objectPoint.Y - epsilon, objectPoint.Z, TupleFlavour.Point)),
                                    DistanceFrom(new Tuple4(objectPoint.X, objectPoint.Y, objectPoint.Z + epsilon, TupleFlavour.Point)) - DistanceFrom(new Tuple4(objectPoint.X, objectPoint.Y, objectPoint.Z - epsilon, TupleFlavour.Point)),
                                    TupleFlavour.Vector
                                );
            return estimateNormal;
        }

        public virtual double[] GetIntersections(Ray ray)
        {
            var result = GetIntersections(ray.origin, Tuple4.Normalize(ray.dir));
            return result;
        }

        private double[] GetIntersections(Tuple4 origin, Tuple4 dir)
        {
            if (!Constants.EpsilonCompare(1.0, dir.Length()))
            {
                throw new ArgumentException("Direction should be normalized", nameof(dir));
            }

            var t = 0.0;
            while (t < maxDistance)
            {
                var p = Tuple4.Geometry3D.MovePoint(origin, dir, t);
                var d = DistanceFrom(p);
                if (Math.Abs(d) < epsilon)
                {
                    return new double[] { t };
                }
                t += d;
            }

            return null;
        }
    }
}
