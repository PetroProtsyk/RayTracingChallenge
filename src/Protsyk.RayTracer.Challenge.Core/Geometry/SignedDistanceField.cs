using System;
using System.Collections.Generic;

// Geometric Primitives
namespace Protsyk.RayTracer.Challenge.Core.Geometry
{
    public abstract class SignedDistanceField
    {
        public IMatrix Transformation 
        {
            get
            {
                return transformation;
            }
            set
            {
                transformation = value;
                inverseTransformation = null;
                if (value != null)
                {
                    inverseTransformation = MatrixOperations.Invert(value);
                }
            }
        }

        protected readonly double epsilon = 0.0001;

        private readonly double maxDistance = 200;

        private IMatrix transformation;

        private IMatrix inverseTransformation;

        protected internal virtual double DistanceFrom(Tuple4 point)
        {
            if (point.IsVector()) {
                throw new ArgumentException("Argument is not a point");
            }
            throw new NotImplementedException();
        }

        protected internal Tuple4 GetObjectPoint(Tuple4 point)
        {
            var objectPoint = point;
            if (Transformation != null)
            {
                objectPoint = MatrixOperations.Geometry3D.Transform(inverseTransformation, point);
            }
            return objectPoint;
        }

        public virtual Tuple4 GetNormal(Tuple4 point)
        {
            var objectPoint = GetObjectPoint(point);
            var estimateNormal = new Tuple4(
                                    DistanceFrom(new Tuple4(objectPoint.X + epsilon, objectPoint.Y, objectPoint.Z, TupleFlavour.Point)) - DistanceFrom(new Tuple4(objectPoint.X - epsilon, objectPoint.Y, objectPoint.Z, TupleFlavour.Point)),
                                    DistanceFrom(new Tuple4(objectPoint.X, objectPoint.Y + epsilon, objectPoint.Z, TupleFlavour.Point)) - DistanceFrom(new Tuple4(objectPoint.X, objectPoint.Y - epsilon, objectPoint.Z, TupleFlavour.Point)),
                                    DistanceFrom(new Tuple4(objectPoint.X, objectPoint.Y, objectPoint.Z + epsilon, TupleFlavour.Point)) - DistanceFrom(new Tuple4(objectPoint.X, objectPoint.Y, objectPoint.Z - epsilon, TupleFlavour.Point)),
                                    TupleFlavour.Vector
                                );

            if (Transformation != null)
            {
               estimateNormal = MatrixOperations.Geometry3D.Transform(MatrixOperations.Transpose(inverseTransformation, false), estimateNormal);
               if (estimateNormal.W != 0.0)
               {
                    estimateNormal = new Tuple4(estimateNormal.X, estimateNormal.Y, estimateNormal.Z, TupleFlavour.Vector);
               }
            }

            return Tuple4.Normalize(estimateNormal);
        }

        public virtual double[] GetIntersections(Ray ray)
        {
            if (Transformation != null)
            {
                ray = ray.Transform(inverseTransformation);
            }

            var result = GetIntersections(ray.origin, Tuple4.Normalize(ray.dir));

            if (Transformation != null)
            {
                if (result != null)
                {
                    var len = ray.dir.Length();
                    for (int i=0; i<result.Length; ++i)
                    {
                        result[i] /= len;
                    }
                }
            }

            return result;
        }

        private double[] GetIntersections(Tuple4 origin, Tuple4 dir)
        {
            if (!Constants.EpsilonCompare(1.0, dir.Length()))
            {
                throw new ArgumentException("Direction should be normalized", nameof(dir));
            }

            var t = 0.0;
            while (t < maxDistance) {
                var p = Tuple4.Geometry3D.MovePoint(origin, dir, t);
                var d = DistanceFrom(p);
                if (Math.Abs(d) < epsilon) {
                    return new double[] { t };
                }
                t += d;
            }

            return null;
        }
   }
}
